using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class WaypointRepository : IWaypointRepository
    {
        private readonly PostgreSqlContext _context;

        public WaypointRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task<List<Waypoint>> GetAllAsync()
        {
            return await _context.Waypoints.ToListAsync();
        }
    }
}