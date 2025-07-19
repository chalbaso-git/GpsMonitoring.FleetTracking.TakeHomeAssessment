using Cross.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingController : ControllerBase
    {
        [HttpPost("calculate")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public IActionResult CalculateRoute([FromBody] RouteRequestDto request)
        {
            // Mock de algoritmo A*
            var route = new List<string> { request.Origin, request.Destination };
            return Ok(route);
        }
    }
}