using API_CredentialManager.Data;
using API_CredentialManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace API_CredentialManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServidorController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public ServidorController(DataBaseContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servidor>> GetServidor(int id)
        {
            var Servidor = await _context.Servidores
                                .Where(e => e.ID == id)
                                .FirstOrDefaultAsync();

            // Si no se encuentra, devuelve un 404 Not Found
            if (Servidor == null)
            {
                return NotFound(new { message = $"No se encontro ningun servidor con el id {id}" });
            }

            return Ok(Servidor);
        }
    }
}
