using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de rutas.
        /// </summary>
        /// <param name="routeService">Servicio para la gestión de rutas.</param>
        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        /// <summary>
        /// Registra una nueva ruta en el sistema.
        /// </summary>
        /// <param name="dto">Datos de la ruta a registrar.</param>
        /// <returns>Confirmación de registro exitoso.</returns>
        /// <response code="200">Ruta registrada correctamente.</response>
        [HttpPost]
        public async Task<IActionResult> AddRoute([FromBody] RouteDto dto)
        {
            await _routeService.AddRouteAsync(dto);
            return Ok("Ruta registrada.");
        }

        /// <summary>
        /// Obtiene todas las rutas registradas.
        /// </summary>
        /// <returns>Lista de rutas.</returns>
        /// <response code="200">Lista de rutas obtenida correctamente.</response>
        [HttpGet]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _routeService.GetRoutesAsync();
            return Ok(routes);
        }
    }
}