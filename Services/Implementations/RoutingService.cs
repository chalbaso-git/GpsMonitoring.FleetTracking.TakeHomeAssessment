using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure;
using Interfaces.Services;

namespace Services.Implementations
{
    public class RoutingService : IRoutingService
    {
        private readonly IRouteCache _routeCache;

        public RoutingService(IRouteCache routeCache)
        {
            _routeCache = routeCache;
        }

        public async Task<RouteResponseDto> CalculateRouteAsync(RouteRequestDto request)
        {
            // Intentar obtener la ruta cacheada
            var cached = await _routeCache.GetCachedRouteAsync(request.VehicleId, request.Origin, request.Destination);
            if (cached != null)
            {
                return new RouteResponseDto
                {
                    VehicleId = cached.VehicleId,
                    Path = cached.Path.Split(',').ToList(),
                    Distance = cached.Distance,
                    CalculatedAt = cached.CalculatedAt
                };
            }

            // Locking distribuido por zona
            var lockTimeout = TimeSpan.FromSeconds(10);
            var lockAcquired = await _routeCache.AcquireZoneLockAsync(request.Origin, request.Destination, request.VehicleId, lockTimeout);
            if (!lockAcquired)
                throw new InvalidOperationException("Zona ocupada, intente nuevamente más tarde.");

            try
            {
                // Mock de cálculo de ruta (A* simplificado)
                var path = new List<string> { request.Origin, "Waypoint1", request.Destination };
                // Reemplaza la asignación de 'Path' en la creación de 'Route' para convertir la lista en string
                var route = new Route
                {
                    VehicleId = request.VehicleId,
                    Path = string.Join(",", path), // Convierte List<string> a string
                    Distance = 10.5,
                    CalculatedAt = DateTime.UtcNow
                };

                await _routeCache.SaveRouteAsync(route);

                return new RouteResponseDto
                {
                    VehicleId = route.VehicleId,
                    Path = route.Path.Split(',').ToList(),
                    Distance = route.Distance,
                    CalculatedAt = route.CalculatedAt
                };
            }
            finally
            {
                await _routeCache.ReleaseZoneLockAsync(request.Origin, request.Destination, request.VehicleId);
            }
        }
    }
}