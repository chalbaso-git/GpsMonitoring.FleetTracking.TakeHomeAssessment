using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Interfaces.Services;

namespace Services.Implementations
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _repository;

        public AlertService(IAlertRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAlertAsync(AlertDto dto)
        {
            try
            {
                await _repository.AddAsync(MapToEntity(dto));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al agregar la alerta.", ex);
            }
        }

        public async Task<List<AlertDto>> GetAlertsAsync()
        {
            try
            {
                var alerts = await _repository.GetAllAsync();
                return [.. alerts.Select(MapToDto)];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener las alertas.", ex);
            }
        }

        private static Alert MapToEntity(AlertDto dto) =>
            new()
            {
                VehicleId = dto.VehicleId,
                Type = dto.Type,
                Message = dto.Message,
                CreatedAt = dto.CreatedAt
            };

        private static AlertDto MapToDto(Alert alert) =>
            new()
            {
                Id = alert.Id,
                VehicleId = alert.VehicleId,
                Type = alert.Type,
                Message = alert.Message,
                CreatedAt = alert.CreatedAt
            };
    }
}