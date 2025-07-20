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

        /// <summary>
        /// Registra un nuevo log de auditoría en el sistema.
        /// </summary>
        /// <param name="log">Datos del log de auditoría a registrar.</param>
        /// <exception cref="InvalidOperationException">Si ocurre un error al registrar el log.</exception>
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

        /// <summary>
        /// Obtiene todos los logs de auditoría asociados a un vehículo.
        /// </summary>
        /// <param name="vehicleId">Identificador del vehículo.</param>
        /// <returns>Lista de logs en formato <see cref="AuditLogDto"/>.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al obtener los logs.</exception>
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