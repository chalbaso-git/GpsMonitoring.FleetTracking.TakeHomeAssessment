using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF; 
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class AlertRepository : IAlertRepository
    {
        private readonly PostgreSqlContext _context;

        public AlertRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Alert alert)
        {
            await _context.Alerts.AddAsync(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Alert>> GetAllAsync()
        {
            return await _context.Alerts.ToListAsync();
        }
    }
}