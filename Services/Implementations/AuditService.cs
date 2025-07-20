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

        public void Log(AuditLogDto log)
        {
            try
            {
                _auditLogRepository.Add(MapToEntity(log));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al registrar el log de auditoría.", ex);
            }
        }

        public List<AuditLogDto> GetLogs(string vehicleId)
        {
            try
            {
                var logs = _auditLogRepository.Find(f => f.VehicleId == vehicleId)
                    .OrderBy(f => f.Id)
                    .ToList();
                return [.. logs.Select(MapToDto)];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener los logs de auditoría.", ex);
            }
        }

        private static AuditLog MapToEntity(AuditLogDto dto) =>
            new()
            {
                VehicleId = dto.VehicleId,
                EventType = dto.EventType,
                Details = dto.Details,
                Timestamp = dto.Timestamp
            };

        private static AuditLogDto MapToDto(AuditLog log) =>
            new()
            {
                VehicleId = log.VehicleId,
                EventType = log.EventType,
                Details = log.Details,
                Timestamp = log.Timestamp
            };
    }
}