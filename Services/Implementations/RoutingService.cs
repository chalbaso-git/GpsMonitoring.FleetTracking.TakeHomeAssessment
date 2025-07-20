using Cross.Dtos;
using Domain.Entities;
using Interfaces.Infrastructure;
using Interfaces.Infrastructure.EF;
using Interfaces.Services;

namespace Services.Implementations
{
    public class RoutingService : IRoutingService
    {
        private readonly IRouteCache _routeCache;
        private readonly IRouteService _routeService;
        private readonly IAlertService _alertService;
        private readonly IWaypointRepository _waypointRepository;

        public RoutingService(
            IRouteCache routeCache,
            IRouteService routeService,
            IAlertService alertService,
            IWaypointRepository waypointRepository)
        {
            _routeCache = routeCache;
            _routeService = routeService;
            _alertService = alertService;
            _waypointRepository = waypointRepository;
        }

        public async Task<RouteResponseDto> CalculateRouteAsync(RouteRequestDto request)
        {
            try
            {
                ValidateRequest(request);

                var cached = await _routeCache.GetCachedRouteAsync(request.VehicleId, request.Origin, request.Destination);
                if (cached != null)
                    return await HandleCachedRoute(cached);

                var lockTimeout = TimeSpan.FromSeconds(10);
                var lockAcquired = await _routeCache.AcquireZoneLockAsync(request.Origin, request.Destination, request.VehicleId, lockTimeout);
                if (!lockAcquired)
                    return HandleDeadlock(request);

                return await CalculateAndStoreRouteAsync(request);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error en el servicio de ruteo.", ex);
            }
        }

        private async Task<RouteResponseDto> CalculateAndStoreRouteAsync(RouteRequestDto request)
        {
            try
            {
                var waypoints = GetDynamicWaypoints(request.Origin, request.Destination, 2);
                var path = BuildPath(request.Origin, waypoints, request.Destination);
                var route = CreateRoute(request.VehicleId, path);

                await _routeCache.SaveRouteAsync(route);
                await _routeService.AddRouteAsync(MapToDto(route));

                return MapToResponseDto(route, path);
            }
            catch (Exception ex)
            {
                _alertService.AddAlert(new AlertDto
                {
                    VehicleId = request.VehicleId,
                    Type = "Error",
                    Message = $"Error al calcular ruta: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                });
                throw;
            }
            finally
            {
                await _routeCache.ReleaseZoneLockAsync(request.Origin, request.Destination, request.VehicleId);
            }
        }

        private static void ValidateRequest(RouteRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Origin) || string.IsNullOrWhiteSpace(request.Destination))
                throw new ArgumentException("Origen y destino son obligatorios.");
        }

        private async Task<RouteResponseDto> HandleCachedRoute(Route cached)
        {
            await _routeService.AddRouteAsync(MapToDto(cached));
            return MapToResponseDto(cached, [.. cached.Path.Split(',')]);
        }

        private RouteResponseDto HandleDeadlock(RouteRequestDto request)
        {
            _alertService.AddAlert(new AlertDto
            {
                VehicleId = request.VehicleId,
                Type = "Deadlock",
                Message = $"Zona ocupada: {request.Origin} -> {request.Destination}",
                CreatedAt = DateTime.UtcNow
            });
            throw new InvalidOperationException("Zona ocupada, intente nuevamente más tarde.");
        }

        private List<string> GetDynamicWaypoints(string origin, string destination, int count = 2)
        {
            var waypoints = _waypointRepository.Find(f => f.Id > 0)
                .OrderBy(f => f.Id)
                .ToList();

            var candidates = waypoints.Where(w => w.Name != origin && w.Name != destination).ToList();
            var random = new Random();
            return [.. candidates.OrderBy(_ => random.Next()).Take(count).Select(w => w.Name)];
        }

        private static List<string> BuildPath(string origin, List<string> waypoints, string destination)
        {
            var path = new List<string> { origin };
            path.AddRange(waypoints);
            path.Add(destination);
            return path;
        }

        private static Route CreateRoute(string vehicleId, List<string> path)
        {
            return new Route
            {
                VehicleId = vehicleId,
                Path = string.Join(",", path),
                Distance = 10.5 + (path.Count - 2) * 2,
                CalculatedAt = DateTime.UtcNow
            };
        }

        private static RouteDto MapToDto(Route route) =>
            new()
            {
                Id = route.Id,
                VehicleId = route.VehicleId,
                Path = route.Path,
                Distance = route.Distance,
                CalculatedAt = route.CalculatedAt
            };

        private static RouteResponseDto MapToResponseDto(Route route, List<string> path) =>
            new()
            {
                VehicleId = route.VehicleId,
                Path = path,
                Distance = route.Distance,
                CalculatedAt = route.CalculatedAt
            };
    }
}