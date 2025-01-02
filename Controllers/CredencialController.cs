using API_CredentialManager.Data;
using API_CredentialManager.Models;
using API_CredentialManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CredentialManager.Controllers
{
    public struct HistorialClaves
    {
        public string CredencialClave { get; set; }
        public DateTime FechaCreacionClave { get; set; }
    }

    public struct CredencialConHistorialClaves
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialDescripcion { get; set; }
        public string CredencialUsuario { get; set; }
        public List<HistorialClaves> CredencialClaves { get; set; }
    }

    public struct CredencialConClave
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialDescripcion { get; set; }
        public string CredencialUsuario { get; set; }
        public string CredencialClave { get; set; }
        public DateTime FechaCreacionClave { get; set; }
        public DateTime FechaEncripcionClave { get; set; }
    }

    public struct CrearCredencial
    {
        public int UsuarioID { get; set; }
        public int ServidorID { get; set; }
        public string CredencialDescripcion { get; set; }
        public string CredencialUsuario { get; set; }
        public string CredencialClave { get; set; }

        public CrearCredencial()
        {
            CredencialDescripcion = string.Empty;
        }
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
            AESCryptoService _encripcion;

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
                                                        .OrderByDescending(ch => ch.FechaCreacion)
                                                        .ToListAsync();

                        _encripcion = new AESCryptoService(usuario.Key);

                        foreach (var clave in credencialHistorial)
                        {
                            claves.Add(
                                new HistorialClaves
                                {
                                    CredencialClave = _encripcion.Decrypt(clave.CredencialClave),
                                    FechaCreacionClave = clave.FechaCreacion
                                });
                        }

                        

                        credencialConHistorial.Add(
                            new CredencialConHistorialClaves
                            {
                                ID = c.ID,
                                UsuarioNombre = usuario.Nombre,
                                ServidorNombre = servidor.Nombre,
                                CredencialDescripcion = c.Descripcion,
                                CredencialUsuario = c.CredencialUsuario,
                                CredencialClaves = claves
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
            AESCryptoService _encripcion;

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
                                                        .OrderByDescending(ch => ch.FechaCreacion)
                                                        .FirstOrDefaultAsync();
                        
                        _encripcion = new AESCryptoService(usuario.Key);

                        credencialConClave.Add(
                                new CredencialConClave
                                {
                                    ID = c.ID,
                                    UsuarioNombre = usuario.Nombre,
                                    ServidorNombre = servidor.Nombre,
                                    CredencialDescripcion = c.Descripcion,
                                    CredencialUsuario = c.CredencialUsuario,
                                    CredencialClave = _encripcion.Decrypt(credencial.CredencialClave),
                                    FechaCreacionClave = credencial.FechaCreacion,
                                    FechaEncripcionClave = credencial.FechaEncripcion
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
            AESCryptoService _encripcion;

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
                                                    .OrderByDescending(ch => ch.FechaCreacion)
                                                    .ToListAsync();

                    _encripcion = new AESCryptoService(usuario.Key);

                    foreach (var clave in credencialHistorial)
                    {
                        claves.Add(
                            new HistorialClaves
                            {
                                CredencialClave = _encripcion.Decrypt(clave.CredencialClave),
                                FechaCreacionClave = clave.FechaCreacion
                            });
                    }

                    CredencialConHistorialClaves credencialConHistorial =
                                                 new CredencialConHistorialClaves
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialDescripcion = credencial.Descripcion,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     CredencialClaves = claves
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
            AESCryptoService _encripcion;

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
                                                    .OrderByDescending(ch => ch.FechaCreacion)
                                                    .FirstOrDefaultAsync();

                    _encripcion = new AESCryptoService(usuario.Key);

                    CredencialConClave credencialConClave =
                                                 new CredencialConClave
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialDescripcion = credencial.Descripcion,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     CredencialClave = _encripcion.Decrypt(credencialHistorial.CredencialClave),
                                                     FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                     FechaEncripcionClave = credencialHistorial.FechaEncripcion
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

        //Agregar credencial
        [HttpPost("AgregarCredencial")]
        public async Task<ActionResult<Respuesta<CredencialConClave>>> agregarCredencial(CrearCredencial credencial)
        {
            Respuesta<CredencialConClave> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";
            AESCryptoService _encripcion;

            try
            {
                var credencialExistente = await _context.Credenciales
                                                .Where(c => c.UsuarioID == credencial.UsuarioID && c.ServidorID == credencial.ServidorID && c.CredencialUsuario == credencial.CredencialUsuario)
                                                .FirstOrDefaultAsync();
                if (credencialExistente != null)
                {
                    mensaje = "La credencial ya existe";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                    return respuesta;
                }
                else
                {
                    var usuarioExiste = await _context.Usuarios
                                            .Where(u => u.ID == credencial.UsuarioID)
                                            .FirstOrDefaultAsync();

                    var servidorExiste = await _context.Servidores
                                            .Where(s => s.ID == credencial.ServidorID)
                                            .FirstOrDefaultAsync();

                    if (usuarioExiste == null && servidorExiste == null)
                    {
                        mensaje = "Usuario y servidor no existen";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else if (usuarioExiste == null)
                    {
                        mensaje = "Usuario no existe";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else if (servidorExiste == null)
                    {
                        mensaje = "Servidor no existe";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else
                    {
                        _encripcion = new AESCryptoService(usuarioExiste.Key);

                        Credencial nuevaCredencial = new Credencial
                        {
                            UsuarioID = credencial.UsuarioID,
                            ServidorID = credencial.ServidorID,
                            Descripcion = credencial.CredencialDescripcion,
                            CredencialUsuario = credencial.CredencialUsuario,
                            UsuarioModificacion = _usuarioModificacion
                        };

                        _context.Credenciales.Add(nuevaCredencial);
                        await _context.SaveChangesAsync();

                        CredencialHistorial credencialHistorial = new CredencialHistorial
                        {
                            CredencialID = nuevaCredencial.ID,
                            CredencialClave = _encripcion.Encrypt(credencial.CredencialClave),
                            UsuarioModificacion = _usuarioModificacion
                        };

                        _context.CredencialHistoriales.Add(credencialHistorial);
                        await _context.SaveChangesAsync();

                        CredencialConClave credencialConClave =
                                                     new CredencialConClave
                                                     {
                                                         ID = nuevaCredencial.ID,
                                                         UsuarioNombre = usuarioExiste.Nombre,
                                                         ServidorNombre = servidorExiste.Nombre,
                                                         CredencialDescripcion = nuevaCredencial.Descripcion,
                                                         CredencialUsuario = nuevaCredencial.CredencialUsuario,
                                                         CredencialClave = credencial.CredencialClave,
                                                         FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                         FechaEncripcionClave = credencialHistorial.FechaEncripcion
                                                     };

                        mensaje = "Credencial agregada";
                        codigo = StatusCodes.Status201Created;
                        respuesta = new Respuesta<CredencialConClave>(codigo, true, mensaje, credencialConClave);
                    }
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

        //Actualizar credencial
        [HttpPut("ActualizarCredencial")]
        public async Task<ActionResult<Respuesta<CredencialConClave>>> actualizarCredencial(CrearCredencial credencial)
        {
            Respuesta<CredencialConClave> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";
            AESCryptoService _encripcion;

            try
            {
                var credencialExiste = await _context.Credenciales
                                               .Where(c => c.UsuarioID == credencial.UsuarioID && c.ServidorID == credencial.ServidorID && c.CredencialUsuario == credencial.CredencialUsuario)
                                               .FirstOrDefaultAsync();
                if (credencialExiste == null)
                {
                    mensaje = "La credencial no existe";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<CredencialConClave>(codigo, false, mensaje);
                    return respuesta;
                }
                else
                {
                    var usuarioExiste = await _context.Usuarios
                                            .Where(u => u.ID == credencial.UsuarioID)
                                            .FirstOrDefaultAsync();

                    var servidorExiste = await _context.Servidores
                                            .Where(s => s.ID == credencial.ServidorID)
                                            .FirstOrDefaultAsync();

                    _encripcion = new AESCryptoService(usuarioExiste.Key);

                    CredencialHistorial credencialHistorial = new CredencialHistorial
                    {
                        CredencialID = credencialExiste.ID,
                        CredencialClave = _encripcion.Encrypt(credencial.CredencialClave),
                        UsuarioModificacion = _usuarioModificacion
                    };

                    _context.CredencialHistoriales.Add(credencialHistorial);
                    await _context.SaveChangesAsync();

                    CredencialConClave credencialConClave =
                                                 new CredencialConClave
                                                 {
                                                     ID = credencialExiste.ID,
                                                     UsuarioNombre = usuarioExiste.Nombre,
                                                     ServidorNombre = servidorExiste.Nombre,
                                                     CredencialDescripcion = credencialExiste.Descripcion,
                                                     CredencialUsuario = credencialExiste.CredencialUsuario,
                                                     CredencialClave = credencial.CredencialClave,
                                                     FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                     FechaEncripcionClave = credencialHistorial.FechaEncripcion
                                                 };

                    mensaje = "Credencial actualizada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<CredencialConClave>(codigo, true, mensaje, credencialConClave);
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
