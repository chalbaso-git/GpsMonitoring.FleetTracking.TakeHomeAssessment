using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IGpsCoordinateRepository
    {
        Task SaveAsync(GpsCoordinate coordinate);
        Task<GpsCoordinate?> GetLastAsync(string vehicleId);
    }
}
