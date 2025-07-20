using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IAuditService
    {
        void Log(AuditLogDto log);
        List<AuditLogDto> GetLogs(string vehicleId);
    }
}