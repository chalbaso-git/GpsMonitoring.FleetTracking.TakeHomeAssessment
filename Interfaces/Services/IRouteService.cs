using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IRouteService
    {
        Task AddRouteAsync(RouteDto dto);
        List<RouteDto> GetRoutes();
    }
}