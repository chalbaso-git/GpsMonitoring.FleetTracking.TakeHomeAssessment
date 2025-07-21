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
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult DeleteVehicle(string vehicleId)
        {
            try
            {
                var result = _vehicleService.DeleteVehicleDistributed(vehicleId);
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

        /// <summary>
        /// Obtiene la lista de vehículos.
        /// </summary>
        /// <returns>Lista de vehículos.</returns>
        /// <response code="200">Solicitud exitosa.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<VehicleDto>), 200)]
        public IActionResult GetVehicles()
        {
            var vehicles = _vehicleService.GetVehicles();
            return Ok(vehicles);
        }

        /// <summary>
        /// Obtiene un vehículo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del vehículo.</param>
        /// <returns>Vehículo correspondiente al identificador.</returns>
        /// <response code="200">Solicitud exitosa.</response>
        /// <response code="404">Vehículo no encontrado.</response>
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
        /// Actualiza la información de un vehículo existente.
        /// </summary>
        /// <param name="id">Identificador del vehículo a actualizar.</param>
        /// <param name="vehicleDto">Datos actualizados del vehículo.</param>
        /// <returns>Resultado de la operación de actualización.</returns>
        /// <response code="200">Actualización exitosa.</response>
        /// <response code="400">Solicitud incorrecta.</response>
        /// <response code="404">Vehículo no encontrado.</response>
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