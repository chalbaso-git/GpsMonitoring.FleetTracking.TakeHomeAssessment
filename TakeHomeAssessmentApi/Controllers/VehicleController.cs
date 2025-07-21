using Cross.Dtos;
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
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteVehicle(string vehicleId)
        {
            try
            {
                var result = _vehicleService.DeleteVehicleDistributed(vehicleId);
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

        /// <summary>
        /// Obtiene la lista de veh�culos.
        /// </summary>
        /// <returns>Lista de veh�culos.</returns>
        /// <response code="200">Solicitud exitosa.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<VehicleDto>), 200)]
        public IActionResult GetVehicles()
        {
            var vehicles = _vehicleService.GetVehicles();
            return Ok(vehicles);
        }

        /// <summary>
        /// Obtiene un veh�culo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del veh�culo.</param>
        /// <returns>Veh�culo correspondiente al identificador.</returns>
        /// <response code="200">Solicitud exitosa.</response>
        /// <response code="404">Veh�culo no encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VehicleDto), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetVehicleById(string id)
        {
            var vehicle = _vehicleService.GetVehicleById(id);
            if (vehicle == null)
                return NotFound();
            return Ok(vehicle);
        }

        /// <summary>
        /// Actualiza la informaci�n de un veh�culo existente.
        /// </summary>
        /// <param name="id">Identificador del veh�culo a actualizar.</param>
        /// <param name="vehicleDto">Datos actualizados del veh�culo.</param>
        /// <returns>Resultado de la operaci�n de actualizaci�n.</returns>
        /// <response code="200">Actualizaci�n exitosa.</response>
        /// <response code="400">Solicitud incorrecta.</response>
        /// <response code="404">Veh�culo no encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(404)]
        public IActionResult UpdateVehicle(string id, [FromBody] VehicleDto vehicleDto)
        {
            if (id != vehicleDto.Id)
                return BadRequest("El id de la URL no coincide con el del cuerpo de la solicitud.");

            var updated = _vehicleService.UpdateVehicle(vehicleDto);
            if (!updated)
                return NotFound();
            return Ok(true);
        }
    }
}