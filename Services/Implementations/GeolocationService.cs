using Cross.Constants;
using Cross.Dtos;
using Cross.Helpers;
using Domain.Entities;
using Interfaces.Infrastructure;
using Interfaces.Services;

namespace Services.Implementations
{
    public class GeolocationService : IGeolocationService
    {
        private readonly IRedisClient _redisClient;

        public GeolocationService(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        // Solución: Convertir 'GpsCoordinate' a 'GpsCoordinateDto' antes de llamar a 'GeoUtils.CalculateDistance'
        public async Task StoreCoordinateAsync(GpsCoordinateDto coordinateDto)
        {
            if (!CoordinateValidator.IsValid(coordinateDto.Latitude, coordinateDto.Longitude))
                throw new ArgumentException("Invalid GPS coordinates.");

            var last = await _redisClient.GetLastCoordinateAsync(coordinateDto.VehicleId);

            var current = new GpsCoordinate
            {
                VehicleId = coordinateDto.VehicleId,
                Latitude = coordinateDto.Latitude,
                Longitude = coordinateDto.Longitude,
                Timestamp = coordinateDto.Timestamp
            };

            if (last != null)
            {
                var lastDto = new GpsCoordinateDto
                {
                    VehicleId = last.VehicleId,
                    Latitude = last.Latitude,
                    Longitude = last.Longitude,
                    Timestamp = last.Timestamp
                };

                var currentDto = new GpsCoordinateDto
                {
                    VehicleId = current.VehicleId,
                    Latitude = current.Latitude,
                    Longitude = current.Longitude,
                    Timestamp = current.Timestamp
                };

                var distance = GeoUtils.CalculateDistance(lastDto, currentDto);
                if (distance < AppConstants.DuplicateThresholdMeters)
                    return; // Ignorar duplicado
            }

            await _redisClient.SaveCoordinateAsync(current);
        }
    }
}
