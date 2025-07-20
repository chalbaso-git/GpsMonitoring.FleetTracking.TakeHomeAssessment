using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IRouteService
    {
        Task AddRouteAsync(RouteDto dto);
        List<RouteDto> GetRoutes();
        List<RouteDto> GetRoutesByVehicleAndDate(string vehicleId, DateTime from, DateTime to);
    }
}