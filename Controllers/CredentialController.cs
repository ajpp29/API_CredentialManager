using API_CredentialManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CredentialManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        // Endpoint básico de prueba
        [HttpPost("encriptar")]
        public IActionResult Encriptar(string llave, string texto)
        {
            AESCryptoService cryptoService = new AESCryptoService(llave);

            var textoCifrado = new { mensaje = cryptoService.Encrypt(texto) };

            return Ok(textoCifrado);
        }

        [HttpPost("desencriptar")]
        public IActionResult Desencriptar(string llave, string texto)
        {
            AESCryptoService cryptoService = new AESCryptoService(llave);

            var textoDescifrado = new { mensaje = cryptoService.Decrypt(texto) };

            return Ok(textoDescifrado);
        }
    }
}
