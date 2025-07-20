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

        /// <summary>
        /// Agrega una nueva ruta al sistema.
        /// <para>
        /// <b>Nota sobre el algoritmo:</b>
        /// El cálculo de rutas en este sistema utiliza un mock y no implementa el algoritmo A* real.
        /// Las rutas se generan con puntos intermedios aleatorios para efectos de demostración.
        /// </para>
        /// </summary>
        /// <param name="dto">Datos de la ruta a agregar.</param>
        /// <exception cref="InvalidOperationException">Si ocurre un error al agregar la ruta.</exception>
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

        /// <summary>
        /// Obtiene todas las rutas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de rutas en formato <see cref="RouteDto"/>.</returns>
        /// <exception cref="InvalidOperationException">Si ocurre un error al obtener las rutas.</exception>
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

        /// <summary>
        /// Obtiene las rutas de un vehículo en un rango de fechas.
        /// </summary>
        /// <param name="vehicleId">Identificador del vehículo.</param>
        /// <param name="from">Fecha inicial (UTC).</param>
        /// <param name="to">Fecha final (UTC).</param>
        /// <returns>Lista de rutas.</returns>
        public List<RouteDto> GetRoutesByVehicleAndDate(string vehicleId, DateTime from, DateTime to)
        {
            try
            {
                var routes = _repository.Find(f => f.VehicleId == vehicleId && f.CalculatedAt >= from && f.CalculatedAt <= to)
                    .OrderBy(f => f.CalculatedAt)
                    .ToList();
                return [.. routes.Select(MapToDto)];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al obtener las rutas históricas.", ex);
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