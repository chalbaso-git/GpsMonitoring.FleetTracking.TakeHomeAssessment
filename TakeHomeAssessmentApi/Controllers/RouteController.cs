using Cross.Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _routeService;

        public RouteController(RouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoute([FromBody] RouteDto dto)
        {
            await _routeService.AddRouteAsync(dto);
            return Ok("Ruta registrada.");
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _routeService.GetRoutesAsync();
            return Ok(routes);
        }
    }
}