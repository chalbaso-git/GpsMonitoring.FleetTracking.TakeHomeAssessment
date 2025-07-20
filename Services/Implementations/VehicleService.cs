using Interfaces.Infrastructure.EF;
using Interfaces.Infrastructure.Redis;
using Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IRedisClient _redisClient;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly DbContext _dbContext;

        public VehicleService(
            IRedisClient redisClient,
            IVehicleRepository vehicleRepository,
            DbContext dbContext)
        {
            _redisClient = redisClient;
            _vehicleRepository = vehicleRepository;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Elimina un veh�culo de forma distribuida, asegurando la consistencia entre la base de datos y Redis.
        /// <para>
        /// Si ocurre un error al eliminar en Redis, se realiza un rollback de la transacci�n en la base de datos.
        /// </para>
        /// </summary>
        /// <param name="vehicleId">Identificador del veh�culo a eliminar.</param>
        /// <returns>True si la eliminaci�n fue exitosa; false en caso contrario.</returns>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si ocurre un error en Redis o si la transacci�n distribuida falla y se realiza rollback.
        /// </exception>
        public async Task<bool> DeleteVehicleDistributedAsync(string vehicleId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _vehicleRepository.DeleteAsync(vehicleId);

                var redisDeleted = await _redisClient.DeleteVehicleAsync(vehicleId);
                if (!redisDeleted)
                    throw new InvalidOperationException("Error al eliminar el veh�culo en Redis.");

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Rollback realizado por fallo en la transacci�n distribuida.", ex);
            }
        }
    }
}