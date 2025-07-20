using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeolocationController : ControllerBase
    {
        private readonly IGeolocationService _geolocationService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de geolocalización.
        /// </summary>
        /// <param name="geolocationService">Servicio para el almacenamiento de coordenadas GPS.</param>
        public GeolocationController(IGeolocationService geolocationService)
        {
            _geolocationService = geolocationService;
        }

        /// <summary>
        /// Almacena una coordenada GPS recibida.
        /// </summary>
        /// <param name="coordinateDto">Datos de la coordenada GPS a almacenar.</param>
        /// <returns>Resultado de la operación de almacenamiento.</returns>
        /// <response code="200">Coordenada GPS almacenada correctamente.</response>
        /// <response code="400">Datos de coordenada GPS inválidos.</response>
        /// <response code="500">Error interno al almacenar la coordenada GPS.</response>
        [HttpPost("store-coordinate")]
        public async Task<IActionResult> StoreCoordinate([FromBody] GpsCoordinateDto coordinateDto)
        {
            if (coordinateDto == null)
            {
                return BadRequest("Invalid GPS coordinate data.");
            }
            try
            {
                await _geolocationService.StoreCoordinateAsync(coordinateDto);
                return Ok("GPS coordinate stored successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while storing the GPS coordinate: " + ex.Message);
            }
        }
    }
}
