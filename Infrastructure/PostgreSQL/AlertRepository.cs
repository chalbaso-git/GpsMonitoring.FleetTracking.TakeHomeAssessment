using Domain.Entities;

namespace Infrastructure.PostgreSQL
{
    public class AlertRepository
    {
        private readonly FleetDbContext _context;

        public AlertRepository(FleetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
        }
    }
}