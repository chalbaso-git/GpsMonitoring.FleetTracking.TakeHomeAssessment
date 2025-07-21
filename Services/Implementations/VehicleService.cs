using Cross.Dtos;
using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;
using Interfaces.Infrastructure.Redis;
using Interfaces.Services;

namespace Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IRedisClient _redisClient;
        private readonly PostgreSqlContext _dbContext; 

        public VehicleService(IVehicleRepository vehicleRepository, IRedisClient redisClient, PostgreSqlContext dbContext)
        {
            _vehicleRepository = vehicleRepository;
            _redisClient = redisClient;
            _dbContext = dbContext;
        }
        
        /// <summary>
        /// Elimina un veh�culo de forma distribuida, asegurando la consistencia entre la base de datos y Redis.
        /// Si ocurre un error en Redis, se realiza rollback de la transacci�n.
        /// </summary>
        /// <param name="vehicleId">Identificador del veh�culo a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa, false en caso contrario.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error y se realiza rollback.</exception>
        public bool DeleteVehicleDistributed(string vehicleId)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var vehicle = _vehicleRepository.Find(f => f.Id == vehicleId).FirstOrDefault() ?? throw new InvalidOperationException("El veh�culo no existe.");
                _vehicleRepository.Delete(vehicle);
                _dbContext.SaveChanges();

                var redisDeleted = _redisClient.DeleteVehicleAsync(vehicleId).GetAwaiter().GetResult();
                if (!redisDeleted)
                    throw new InvalidOperationException("Error al eliminar el veh�culo en Redis.");

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException("Rollback realizado por fallo en la transacci�n distribuida.", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de todos los veh�culos registrados.
        /// </summary>
        /// <returns>Lista de veh�culos en formato <see cref="VehicleDto"/>.</returns>
        public List<VehicleDto> GetVehicles()
        {
            var vehicles = _vehicleRepository.Find(f => true).ToList();
            return [.. vehicles.Select(MapToDto)];
        }

        /// <summary>
        /// Obtiene la informaci�n de un veh�culo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del veh�culo.</param>
        /// <returns>El veh�culo encontrado o null si no existe.</returns>
        public VehicleDto? GetVehicleById(string id)
        {
            var vehicle = _vehicleRepository.Find(f => f.Id == id).FirstOrDefault();
            return vehicle is null ? null : MapToDto(vehicle);
        }

        /// <summary>
        /// Actualiza la informaci�n de un veh�culo existente.
        /// </summary>
        /// <param name="vehicleDto">Datos actualizados del veh�culo.</param>
        /// <returns>True si la actualizaci�n fue exitosa, false si el veh�culo no existe.</returns>
        public bool UpdateVehicle(VehicleDto vehicleDto)
        {
            var existingVehicle = _vehicleRepository.Find(f => f.Id == vehicleDto.Id).FirstOrDefault();


            if (existingVehicle == null)
                return false;

            existingVehicle.Name = vehicleDto.Name;
            existingVehicle.LicensePlate = vehicleDto.LicensePlate;
            existingVehicle.Model = vehicleDto.Model;
            existingVehicle.Year = vehicleDto.Year;
            existingVehicle.Status = vehicleDto.Status;
            existingVehicle.LastLocation = vehicleDto.LastLocation;
            existingVehicle.LastSeen = vehicleDto.LastSeen;
            _vehicleRepository.Update(existingVehicle);

            return true;
        }

        /// <summary>
        /// Convierte una entidad <see cref="Vehicle"/> a un DTO <see cref="VehicleDto"/>.
        /// </summary>
        /// <param name="v">Entidad Vehicle.</param>
        /// <returns>DTO de veh�culo.</returns>
        private static VehicleDto MapToDto(Vehicle v) => new()
        {
            Id = v.Id,
            Name = v.Name,
            LicensePlate = v.LicensePlate,
            Model = v.Model,
            Year = v.Year,
            Status = v.Status,
            LastLocation = v.LastLocation,
            LastSeen = v.LastSeen
        };
    }
}