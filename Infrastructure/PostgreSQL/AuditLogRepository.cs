using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly FleetDbContext _context;

        public AuditLogRepository(FleetDbContext context)
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