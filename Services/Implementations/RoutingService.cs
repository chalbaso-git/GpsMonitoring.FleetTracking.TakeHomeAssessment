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
                    Path = cached.Path,
                    Distance = cached.Distance,
                    CalculatedAt = cached.CalculatedAt
                };
            }

            // Mock de cálculo de ruta (A* simplificado)
            var path = new List<string> { request.Origin, "Waypoint1", request.Destination };
            var route = new Route
            {
                VehicleId = request.VehicleId,
                Path = path,
                Distance = 10.5,
                CalculatedAt = DateTime.UtcNow
            };

            await _routeCache.SaveRouteAsync(route);

            return new RouteResponseDto
            {
                VehicleId = route.VehicleId,
                Path = route.Path,
                Distance = route.Distance,
                CalculatedAt = route.CalculatedAt
            };
        }
    }
}