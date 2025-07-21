using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.Redis;
using Interfaces.Services;
using System.Collections.Concurrent;

namespace Services.Implementations
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IRedisClient _redisClient;
        private readonly IVehicleService _vehicleService;
        private static readonly ConcurrentQueue<GpsCoordinate> _pendingQueue = new();

        public GeolocationService(IRedisClient redisClient, IVehicleService vehicleService)
        {
            _redisClient = redisClient;
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Almacena una coordenada GPS en Redis. Si Redis no está disponible, la coordenada se guarda en una cola temporal
        /// y se reintenta la sincronización cuando el servicio se recupere.
        /// </summary>
        public async Task StoreCoordinateAsync(GpsCoordinateDto coordinateDto)
        {
            ValidateCoordinate(coordinateDto);

            var last = await _redisClient.GetLastCoordinateAsync(coordinateDto.VehicleId);

            const double Tolerance = 1e-6;
            if (last != null &&
                last.VehicleId == coordinateDto.VehicleId &&
                Math.Abs(last.Latitude - coordinateDto.Latitude) < Tolerance &&
                Math.Abs(last.Longitude - coordinateDto.Longitude) < Tolerance &&
                last.Timestamp == coordinateDto.Timestamp)
            {
                return;
            }

            var entity = new GpsCoordinate
            {
                VehicleId = coordinateDto.VehicleId,
                Latitude = coordinateDto.Latitude,
                Longitude = coordinateDto.Longitude,
                Timestamp = coordinateDto.Timestamp
            };

            try
            {
                await _redisClient.SaveCoordinateAsync(entity);

                // Guardar en la base de datos la última ubicación consultada para el vehículo      
                if (entity.VehicleId != null)
                {
                    var vehicle = _vehicleService.GetVehicleById(entity.VehicleId);

                    vehicle!.LastLocation = $"{coordinateDto.Latitude},{coordinateDto.Longitude}";
                    vehicle.LastSeen = coordinateDto.Timestamp;
                    // Reemplaza este bloque en el método StoreCoordinateAsync:

                    if (entity.VehicleId != null)
                    {
                        vehicle = _vehicleService.GetVehicleById(entity.VehicleId);

                        vehicle!.LastLocation = $"{coordinateDto.Latitude},{coordinateDto.Longitude}";
                        vehicle.LastSeen = coordinateDto.Timestamp;
                        _vehicleService.UpdateVehicle(vehicle);
                    }

                    // Por este bloque, agregando comprobación de null para evitar CS8602:

                    if (entity.VehicleId != null)
                    {
                        vehicle = _vehicleService.GetVehicleById(entity.VehicleId);

                        if (vehicle != null)
                        {
                            vehicle.LastLocation = $"{coordinateDto.Latitude},{coordinateDto.Longitude}";
                            vehicle.LastSeen = coordinateDto.Timestamp;
                            _vehicleService.UpdateVehicle(vehicle);
                        }
                    }
                    _vehicleService.UpdateVehicle(vehicle!);
                }

                // Intentar procesar la cola pendiente si existe
                await ProcessPendingQueueAsync();
            }
            catch (Exception ex)
            {
                _pendingQueue.Enqueue(entity);
                throw new InvalidOperationException("Error al almacenar la coordenada GPS. Se guardó en cola para reintento.", ex);
            }
        }

        /// <summary>
        /// Procesa la cola de coordenadas pendientes y las almacena en Redis.
        /// </summary>
        public async Task ProcessPendingQueueAsync()
        {
            while (_pendingQueue.TryDequeue(out var pending))
            {
                try
                {
                    await _redisClient.SaveCoordinateAsync(pending);
                }
                catch
                {
                    // Si vuelve a fallar, re-enqueue y salir para evitar bucle infinito
                    _pendingQueue.Enqueue(pending);
                    break;
                }
            }
        }

        private static void ValidateCoordinate(GpsCoordinateDto dto)
        {
            if (dto.Latitude < -90 || dto.Latitude > 90 || dto.Longitude < -180 || dto.Longitude > 180)
                throw new ArgumentException("Invalid GPS coordinates.");
        }

    }

}
