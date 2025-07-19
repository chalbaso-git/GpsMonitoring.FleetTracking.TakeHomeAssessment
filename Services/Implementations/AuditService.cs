using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Interfaces.Services;

namespace Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(AuditLogDto log)
        {
            var entity = new AuditLog
            {
                VehicleId = log.VehicleId,
                Action = log.Action,
                Details = log.Details,
                Timestamp = log.Timestamp
            };
            await _auditLogRepository.SaveAsync(entity);
        }

        public async Task<List<AuditLogDto>> GetLogsAsync(string vehicleId)
        {
            var logs = await _auditLogRepository.GetByVehicleIdAsync(vehicleId);
            return [.. logs.Select(l => new AuditLogDto
            {
                VehicleId = l.VehicleId,
                Action = l.Action,
                Details = l.Details,
                Timestamp = l.Timestamp
            })];
        }
    }
}