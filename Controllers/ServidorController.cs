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

        // GET: api/Server
        [HttpGet("ObtenerServidores")]
        public async Task<ActionResult<IEnumerable<Servidor>>> obtenerServidores()
        {
            return await _context.Servidores.ToListAsync();
        }

        [HttpGet("ObtenerServidoresActivos")]
        public async Task<ActionResult<List<Servidor>>> obtenerServidoresActivos()
        {
            var Servidores = await _context.Servidores
                                .Where(e => e.Activo == true)
                                .ToListAsync();

            return Servidores;
        }

        [HttpGet("ObtenerServidor/{id}")]
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
