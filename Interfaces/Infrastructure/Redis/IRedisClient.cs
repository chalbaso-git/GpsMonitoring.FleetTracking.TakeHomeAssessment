using Domain.Entities;

namespace Interfaces.Infrastructure.Redis
{
    public interface IRedisClient
    {
        Task<GpsCoordinate?> GetLastCoordinateAsync(string vehicleId);
        Task SaveCoordinateAsync(GpsCoordinate coordinate);
        Task<bool> DeleteVehicleAsync(string vehicleId);
    }
}
