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
        public GeolocationController(IGeolocationService geolocationService)
        {
            _geolocationService = geolocationService;
        }
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
