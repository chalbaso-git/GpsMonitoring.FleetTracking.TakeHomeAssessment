using Domain.Entities;

namespace Interfaces.Infrastructure.EF
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
    }
}