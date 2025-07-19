using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class RouteRepository : IRouteRepository
    {
        private readonly PostgreSqlContext _context;

        public RouteRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Route route)
        {
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Route>> GetAllAsync()
        {
            return await _context.Routes.ToListAsync();
        }
    }
}