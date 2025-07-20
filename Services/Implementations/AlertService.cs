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

        /// <summary>
        /// Agrega una nueva alerta al sistema.
        /// </summary>
        /// <param name="dto">Datos de la alerta a agregar.</param>
        /// <exception cref="InvalidOperationException">Si ocurre un error al agregar la alerta.</exception>
        public void AddAlert(AlertDto dto)
        {
            try
            {
                _repository.Add(MapToEntity(dto));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al agregar la alerta.", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las alertas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de alertas en formato <see cref="AlertDto"/>.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al obtener las alertas.</exception>
        public List<AlertDto> GetAlerts()
        {
            try
            {
                var alerts = _repository.Find(f => f.Id > 0)
                    .OrderBy(f => f.Id)
                    .ToList();
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