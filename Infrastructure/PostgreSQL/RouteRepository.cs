using Domain.Entities;

namespace Infrastructure.PostgreSQL
{
    public class RouteRepository
    {
        private readonly FleetDbContext _context;

        public RouteRepository(FleetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Route route)
        {
            _context.Routes.Add(route);
            await _context.SaveChangesAsync();
        }
    }
}