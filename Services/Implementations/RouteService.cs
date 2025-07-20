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

        public Task AddRouteAsync(RouteDto dto)
        {
            try
            {
                _repository.Add(MapToEntity(dto));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al agregar la ruta.", ex);
            }

            return Task.CompletedTask;
        }

        public List<RouteDto> GetRoutes()
        {
            try
            {
                var routes = _repository.Find(f => f.Id > 0)
                       .OrderBy(f => f.Id)
                       .ToList();
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