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
        private int _consecutiveFailures = 0;
        private bool _circuitOpen = false;

        public RoutingController(IRoutingService routingService, IAuditService auditService)
        {
            _routingService = routingService;
            _auditService = auditService;
        }

        [HttpPost("calculate")]
        [ProducesResponseType(typeof(RouteResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CalculateRoute([FromBody] RouteRequestDto request)
        {
            if (_circuitOpen)
            {
                await _auditService.LogAsync(new AuditLogDto
                {
                    VehicleId = request.VehicleId,
                    EventType = "CircuitBreakerOpen",
                    Details = "Servicio de ruteo desactivado.",
                    Timestamp = DateTime.UtcNow
                });
                return StatusCode(503, "Servicio de ruteo desactivado por fallos consecutivos.");
            }

            try
            {
                var response = await _routingService.CalculateRouteAsync(request);
                _consecutiveFailures = 0;
                await _auditService.LogAsync(new AuditLogDto
                {
                    VehicleId = request.VehicleId,
                    EventType = "RouteCalculated",
                    Details = $"Ruta calculada: {string.Join("->", response.Path)}",
                    Timestamp = DateTime.UtcNow
                });
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        [HttpPost("reset-circuit")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetCircuit()
        {
            _circuitOpen = false;
            _consecutiveFailures = 0;
            await _auditService.LogAsync(new AuditLogDto
            {
                VehicleId = "",
                EventType = "CircuitBreakerReset",
                Details = "Circuito reiniciado.",
                Timestamp = DateTime.UtcNow
            });
            return Ok("Circuito reiniciado.");
        }
    }
}