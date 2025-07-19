using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle(string vehicleId)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicleDistributedAsync(vehicleId);
                if (result)
                    return Ok("Veh�culo eliminado correctamente.");
                else
                    return StatusCode(500, "No se pudo eliminar el veh�culo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}