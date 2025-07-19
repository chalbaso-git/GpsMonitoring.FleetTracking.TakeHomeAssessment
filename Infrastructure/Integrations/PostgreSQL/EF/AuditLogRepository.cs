using Domain.Entities;
using Infrastructure.Integrations.PostgreSQL.Base;
using Interfaces.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Integrations.PostgreSQL.EF
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly PostgreSqlContext _context;

        public AuditLogRepository(PostgreSqlContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(AuditLog log)
        {
            _context.Set<AuditLog>().Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetByVehicleIdAsync(string vehicleId)
        {
            return await _context.Set<AuditLog>()
                .Where(a => a.VehicleId == vehicleId)
                .ToListAsync();
        }
    }
}