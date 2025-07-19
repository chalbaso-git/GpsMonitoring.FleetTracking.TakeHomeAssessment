using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IWaypointRepository
    {
        Task<List<Waypoint>> GetAllAsync();
    }
}
