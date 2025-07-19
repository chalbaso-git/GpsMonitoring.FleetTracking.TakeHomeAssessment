using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IAuditLogRepository
    {
        Task SaveAsync(AuditLog log);
        Task<List<AuditLog>> GetByVehicleIdAsync(string vehicleId);
    }
}