using Cross.Dtos;
using Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de alertas.
        /// </summary>
        /// <param name="alertService">Servicio para la gestión de alertas.</param>
        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        /// <summary>
        /// Registra una nueva alerta en el sistema.
        /// </summary>
        /// <param name="dto">Datos de la alerta a registrar.</param>
        /// <returns>Confirmación de registro exitoso.</returns>
        /// <response code="200">Alerta registrada correctamente.</response>
        [HttpPost]
        public IActionResult AddAlert([FromBody] AlertDto dto)
        {
            _alertService.AddAlert(dto);
            return Ok("Alerta registrada.");
        }

        /// <summary>
        /// Obtiene todas las alertas registradas.
        /// </summary>
        /// <returns>Lista de alertas.</returns>
        /// <response code="200">Lista de alertas obtenida correctamente.</response>
        [HttpGet]
        public IActionResult GetAlerts()
        {
            var alerts = _alertService.GetAlerts();
            return Ok(alerts);
        }
    }
}