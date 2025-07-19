using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditLogDto log);
        Task<List<AuditLogDto>> GetLogsAsync(string vehicleId);
    }
}