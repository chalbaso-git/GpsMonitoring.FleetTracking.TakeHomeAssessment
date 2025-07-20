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
        /// Inicializa una nueva instancia del controlador de veh�culos.
        /// </summary>
        /// <param name="vehicleService">Servicio para la gesti�n de veh�culos.</param>
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Elimina un veh�culo de forma distribuida.
        /// </summary>
        /// <param name="vehicleId">Identificador del veh�culo a eliminar.</param>
        /// <returns>Resultado de la operaci�n de eliminaci�n.</returns>
        /// <response code="200">Veh�culo eliminado correctamente.</response>
        /// <response code="500">No se pudo eliminar el veh�culo o se produjo un error interno.</response>
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