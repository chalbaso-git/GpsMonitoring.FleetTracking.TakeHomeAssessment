using Cross.Dtos;

namespace Interfaces.Services
{
    public interface IRoutingService
    {
        Task<RouteResponseDto> CalculateRouteAsync(RouteRequestDto request);
    }
}