using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_CredentialManager.Services;

namespace API_CredentialManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialManagerController : ControllerBase
    {
        // Endpoint básico de prueba
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API de Estudiantes funcionando correctamente.");
        }
    }
}
