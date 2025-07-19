using Cross.Dtos;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;

namespace TakeHomeAssessmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly AlertService _alertService;

        public AlertController(AlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAlert([FromBody] AlertDto dto)
        {
            await _alertService.AddAlertAsync(dto);
            return Ok("Alerta registrada.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAlerts()
        {
            var alerts = await _alertService.GetAlertsAsync();
            return Ok(alerts);
        }
    }
}