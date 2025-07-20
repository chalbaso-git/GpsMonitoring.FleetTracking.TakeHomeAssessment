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
        [ProducesResponseType(typeof(string), 200)]
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
        [ProducesResponseType(typeof(List<RouteDto>), 200)]
        public IActionResult GetRoutes()
        {
            var routes = _routeService.GetRoutes();
            return Ok(routes);
        }

        /// <summary>
        /// Obtiene las rutas de un vehículo en un rango de fechas.
        /// </summary>
        /// <param name="vehicleId">Identificador del vehículo.</param>
        /// <param name="from">Fecha de inicio.</param>
        /// <param name="to">Fecha de fin.</param>
        /// <returns>Lista de rutas filtradas.</returns>
        /// <response code="200">Lista de rutas obtenida correctamente.</response>
        [HttpGet("history")]
        [ProducesResponseType(typeof(List<RouteDto>), 200)]
        public IActionResult GetRoutesByVehicleAndDate([FromQuery] string vehicleId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var routes = _routeService.GetRoutesByVehicleAndDate(vehicleId, from, to);
            return Ok(routes);
        }
    }
}