using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IRouteRepository
    {
        Task AddAsync(Route route);
        Task<List<Route>> GetAllAsync();
    }
}