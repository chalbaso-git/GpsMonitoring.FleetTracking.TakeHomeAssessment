using Domain.Entities;

namespace Interfaces.Infrastructure
{
    public interface IRouteCache
    {
        Task<Route?> GetCachedRouteAsync(string vehicleId, string origin, string destination);
        Task SaveRouteAsync(Route route);
        Task<bool> AcquireZoneLockAsync(string origin, string destination, string vehicleId, TimeSpan timeout);
        Task ReleaseZoneLockAsync(string origin, string destination, string vehicleId);
    }
}