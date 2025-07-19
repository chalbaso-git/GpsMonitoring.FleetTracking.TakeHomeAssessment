using Domain.Entities;

namespace Interfaces.Infrastructure
{
    public interface IRouteCache
    {
        Task<Route?> GetCachedRouteAsync(string vehicleId, string origin, string destination);
        Task SaveRouteAsync(Route route);
    }
}