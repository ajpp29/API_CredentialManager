using API_CredentialManager.Data;
using API_CredentialManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CredentialManager.Controllers
{
    public struct HistorialClaves
    {
        public string CredencialClave { get; set; }
        public DateTime FechaModificacion { get; set; }
    }

    public struct CredencialConHistorialClaves
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialUsuario { get; set; }
        public List<HistorialClaves> Claves { get; set; }
    }

    public struct CredencialConClave
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialUsuario { get; set; }
        public string Clave { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CredencialController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public CredencialController(DataBaseContext context)
        {
            _context = context;
        }

        // Obtener todas las credenciales con historial de claves por usuario
        [HttpGet("ObtenerCredencialesHistorialPorUsuario/{usuarioID}")]
        public async Task<ActionResult<Respuesta<List<CredencialConHistorialClaves>>>> obtenerCredencialesUsuario(int usuarioID)
        {
            Respuesta<List<CredencialConHistorialClaves>> respuesta;
            string mensaje;
            int codigo;


            try
            {
                var credenciales = await _context.Credenciales
                                                .Where(c => c.UsuarioID == usuarioID)
                                                .ToListAsync();
                if (credenciales.Any())
                {
                    List<CredencialConHistorialClaves> credencialConHistorial = new List<CredencialConHistorialClaves>();

                    foreach (var c in credenciales)
                    {
                        List<HistorialClaves> claves = new List<HistorialClaves>();

                        var usuario = await _context.Usuarios
                                                .Where(u => u.ID == c.UsuarioID)
                                                .FirstOrDefaultAsync();

                        var servidor = await _context.Servidores
                                                .Where(s => s.ID == c.ServidorID)
                                                .FirstOrDefaultAsync();

                        var credencialHistorial = await _context.CredencialHistoriales
                                                        .Where(ch => ch.CredencialID == c.ID)
                                                        .OrderByDescending(ch => ch.FechaModificacion)
                                                        .ToListAsync();

                        foreach (var clave in credencialHistorial)
                        {
                            claves.Add(
                                new HistorialClaves
                                {
                                    CredencialClave = clave.CredencialClave,
                                    FechaModificacion = clave.FechaModificacion
                                });
                        }

                        credencialConHistorial.Add(
                            new CredencialConHistorialClaves
                            {
                                ID = c.ID,
                                UsuarioNombre = usuario.Nombre,
                                ServidorNombre = servidor.Nombre,
                                CredencialUsuario = c.CredencialUsuario,
                                Claves = claves
                            });
                    }

                    mensaje = "Credenciales encontradas";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<CredencialConHistorialClaves>>(codigo, true, mensaje, credencialConHistorial);
                }
                else
                {
                    mensaje = "No se encontraron credencial para el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<CredencialConHistorialClaves>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<CredencialConHistorialClaves>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener todas las credenciales por usuario
        [HttpGet("ObtenerCredencialesPorUsuario/{usuarioID}")]
        public async Task<ActionResult<Respuesta<List<CredencialConClave>>>> obtenerCredencialesPorUsuario(int usuarioID)
        {
            Respuesta<List<CredencialConClave>> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var credenciales = await _context.Credenciales
                                                .Where(c => c.UsuarioID == usuarioID)
                                                .ToListAsync();
                if (credenciales.Any())
                {
                    List<CredencialConClave> credencialConClave = new List<CredencialConClave>();

                    foreach (var c in credenciales)
                    {
                        var usuario = await _context.Usuarios
                                                .Where(u => u.ID == c.UsuarioID)
                                                .FirstOrDefaultAsync();

                        var servidor = await _context.Servidores
                                                    .Where(s => s.ID == c.ServidorID)
                                                    .FirstOrDefaultAsync();

                        var credencial = await _context.CredencialHistoriales
                                                        .Where(ch => ch.CredencialID == c.ID)
                                                        .OrderByDescending(ch => ch.FechaModificacion)
                                                        .FirstOrDefaultAsync();

                        credencialConClave.Add(
                                new CredencialConClave
                                {
                                    ID = c.ID,
                                    UsuarioNombre = usuario.Nombre,
                                    ServidorNombre = servidor.Nombre,
                                    CredencialUsuario = c.CredencialUsuario,
                                    Clave = credencial.CredencialClave
                                });
                    }

                    mensaje = "Credenciales encontradas";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<CredencialConClave>>(codigo, true, mensaje, credencialConClave);
                }
                else
                {
                    mensaje = "No se encontraron credencial para el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<CredencialConClave>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<CredencialConClave>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener credencial con historial de claves por ID
        [HttpGet("ObtenerCredencialHistorial/{credencialID}")]
        public async Task<ActionResult<Respuesta<CredencialConHistorialClaves>>> obtenerCredencialHistorialPorID(int credencialID)
        {
            Respuesta<CredencialConHistorialClaves> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var credencial = await _context.Credenciales
                                                .Where(c => c.ID == credencialID)
                                                .FirstOrDefaultAsync();
                if (credencial != null)
                {
                    List<HistorialClaves> claves = new List<HistorialClaves>();

                    var usuario = await _context.Usuarios
                                            .Where(u => u.ID == credencial.UsuarioID)
                                            .FirstOrDefaultAsync();

                    var servidor = await _context.Servidores
                                            .Where(s => s.ID == credencial.ServidorID)
                                            .FirstOrDefaultAsync();

                    var credencialHistorial = await _context.CredencialHistoriales
                                                    .Where(ch => ch.CredencialID == credencial.ID)
                                                    .OrderByDescending(ch => ch.FechaModificacion)
                                                    .ToListAsync();

                    foreach (var clave in credencialHistorial)
                    {
                        claves.Add(
                            new HistorialClaves
                            {
                                CredencialClave = clave.CredencialClave,
                                FechaModificacion = clave.FechaModificacion
                            });
                    }

                    CredencialConHistorialClaves credencialConHistorial =
                                                 new CredencialConHistorialClaves
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     Claves = claves
                                                 };

                    mensaje = "Credencial encontrada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<CredencialConHistorialClaves>(codigo, true, mensaje, credencialConHistorial);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<CredencialConHistorialClaves>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<CredencialConHistorialClaves>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener credencial por ID
        [HttpGet("ObtenerCredencial/{credencialID}")]
        public async Task<ActionResult<Respuesta<CredencialConClave>>> obtenerCredencialPorID(int credencialID)
        {
            Respuesta<CredencialConClave> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var credencial = await _context.Credenciales
                                                .Where(c => c.ID == credencialID)
                                                .FirstOrDefaultAsync();
                if (credencial != null)
                {
                    var usuario = await _context.Usuarios
                                            .Where(u => u.ID == credencial.UsuarioID)
                                            .FirstOrDefaultAsync();

                    var servidor = await _context.Servidores
                                            .Where(s => s.ID == credencial.ServidorID)
                                            .FirstOrDefaultAsync();

                    var credencialHistorial = await _context.CredencialHistoriales
                                                    .Where(ch => ch.CredencialID == credencial.ID)
                                                    .OrderByDescending(ch => ch.FechaModificacion)
                                                    .FirstOrDefaultAsync();

                    CredencialConClave credencialConClave =
                                                 new CredencialConClave
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     Clave = credencialHistorial.CredencialClave
                                                 };

                    mensaje = "Credencial encontrada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<CredencialConClave>(codigo, true, mensaje, credencialConClave);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }
    }
}
