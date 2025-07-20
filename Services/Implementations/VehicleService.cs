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

        public async Task<bool> DeleteVehicleDistributedAsync(string vehicleId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await _vehicleRepository.DeleteAsync(vehicleId);

                var redisDeleted = await _redisClient.DeleteVehicleAsync(vehicleId);
                if (!redisDeleted)
                    throw new InvalidOperationException("Error al eliminar el vehículo en Redis.");

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Rollback realizado por fallo en la transacción distribuida.", ex);
            }
        }
    }
}