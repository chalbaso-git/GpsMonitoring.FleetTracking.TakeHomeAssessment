using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure.EF;
using Interfaces.Services;

namespace Services.Implementations
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repository;

        public RouteService(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task AddRouteAsync(RouteDto dto)
        {
            try
            {
                await _repository.AddAsync(MapToEntity(dto));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al agregar la ruta.", ex);
            }
        }

        public async Task<List<RouteDto>> GetRoutesAsync()
        {
            try
            {
                var routes = await _repository.GetAllAsync();
                return [.. routes.Select(MapToDto)];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener las rutas.", ex);
            }
        }

        private static Route MapToEntity(RouteDto dto) =>
            new()
            {
                VehicleId = dto.VehicleId,
                Path = dto.Path,
                Distance = dto.Distance,
                CalculatedAt = dto.CalculatedAt
            };

        private static RouteDto MapToDto(Route route) =>
            new()
            {
                Id = route.Id,
                VehicleId = route.VehicleId,
                Path = route.Path,
                Distance = route.Distance,
                CalculatedAt = route.CalculatedAt
            };
    }
}