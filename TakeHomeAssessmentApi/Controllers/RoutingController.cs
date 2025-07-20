using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly IRoutingService _routingService;
        private readonly IAuditService _auditService;
        private readonly ICircuitBreakerService _circuitBreakerService;

        public RoutingController(IRoutingService routingService, IAuditService auditService, ICircuitBreakerService circuitBreakerService)
        {
            _routingService = routingService;
            _auditService = auditService;
            _circuitBreakerService = circuitBreakerService;
        }

        /// <summary>
        /// Calcula la ruta óptima para un vehículo entre dos puntos.
        /// </summary>
        /// <param name="request">Datos de la solicitud de ruteo.</param>
        /// <returns>Respuesta con la ruta calculada o el error correspondiente.</returns>
        /// <response code="200">Ruta calculada correctamente.</response>
        /// <response code="400">Solicitud inválida.</response>
        /// <response code="500">Error interno del servidor.</response>
        /// <response code="503">Servicio desactivado por Circuit Breaker.</response>
        [HttpPost("calculate")]
        [ProducesResponseType(typeof(RouteResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CalculateRoute([FromBody] RouteRequestDto request)
        {
            if (_circuitBreakerService.IsOpen)
            {
                await LogCircuitBreakerEvent(request.VehicleId, "CircuitBreakerOpen", "Servicio de ruteo desactivado.");
                return StatusCode(503, "Servicio de ruteo desactivado por fallos consecutivos.");
            }

            try
            {
                var response = await _routingService.CalculateRouteAsync(request);
                _circuitBreakerService.Reset();
                await LogRouteCalculated(request.VehicleId, response.Path);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                await HandleFailure(request.VehicleId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                await HandleFailure(request.VehicleId, "Error interno: " + ex.Message);
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        /// <summary>
        /// Reinicia el estado del Circuit Breaker.
        /// </summary>
        /// <returns>Mensaje de confirmación.</returns>
        /// <response code="200">Circuito reiniciado correctamente.</response>
        [HttpPost("reset-circuit")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetCircuit()
        {
            _circuitBreakerService.Reset();
            await LogCircuitBreakerEvent("", "CircuitBreakerReset", "Circuito reiniciado.");
            return Ok("Circuito reiniciado.");
        }

        private async Task HandleFailure(string vehicleId, string details)
        {
            _circuitBreakerService.RegisterFailure();
            if (_circuitBreakerService.IsOpen)
            {
                await LogCircuitBreakerEvent(vehicleId,  "CircuitBreakerActivated", $"Circuit Breaker activado por 3 fallos consecutivos. Detalle: {details}");
            }
        }

        private async Task LogRouteCalculated(string vehicleId, List<string> path)
        {
            await _auditService.LogAsync(new AuditLogDto
            {
                VehicleId = vehicleId,
                EventType = "RouteCalculated",
                Details = $"Ruta calculada: {string.Join("->", path)}",
                Timestamp = DateTime.UtcNow
            });
        }

        private async Task LogCircuitBreakerEvent(string vehicleId, string eventType, string details)
        {
            await _auditService.LogAsync(new AuditLogDto
            {
                VehicleId = vehicleId,
                EventType = eventType,
                Details = details,
                Timestamp = DateTime.UtcNow
            });
        }       
    }
}