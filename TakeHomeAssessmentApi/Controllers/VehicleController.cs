using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de vehículos.
        /// </summary>
        /// <param name="vehicleService">Servicio para la gestión de vehículos.</param>
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Elimina un vehículo de forma distribuida.
        /// </summary>
        /// <param name="vehicleId">Identificador del vehículo a eliminar.</param>
        /// <returns>Resultado de la operación de eliminación.</returns>
        /// <response code="200">Vehículo eliminado correctamente.</response>
        /// <response code="500">No se pudo eliminar el vehículo o se produjo un error interno.</response>
        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle(string vehicleId)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicleDistributedAsync(vehicleId);
                if (result)
                    return Ok("Vehículo eliminado correctamente.");
                else
                    return StatusCode(500, "No se pudo eliminar el vehículo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}