using API_CredentialManager.Data;
using API_CredentialManager.Models;
using API_CredentialManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CredentialManager.Controllers
{
    public struct _historialClaves
    {
        public string CredencialClave { get; set; }
        public DateTime FechaCreacionClave { get; set; }
    }

    public struct _credencialConHistorialClaves
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialDescripcion { get; set; }
        public bool Activo { get; set; }
        public string CredencialUsuario { get; set; }
        public List<_historialClaves> CredencialClaves { get; set; }
    }

    public struct _credencialConClave
    {
        public int ID { get; set; }
        public string UsuarioNombre { get; set; }
        public string ServidorNombre { get; set; }
        public string CredencialDescripcion { get; set; }
        public bool Activo { get; set; }
        public string CredencialUsuario { get; set; }
        public string CredencialClave { get; set; }
        public DateTime FechaCreacionClave { get; set; }
        public DateTime FechaEncripcionClave { get; set; }
    }

    public struct _credencialNueva
    {
        public int UsuarioID { get; set; }
        public int ServidorID { get; set; }
        public string CredencialDescripcion { get; set; }
        public string CredencialUsuario { get; set; }
        public string CredencialClave { get; set; }

        public _credencialNueva()
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
        public async Task<ActionResult<Respuesta<List<_credencialConHistorialClaves>>>> obtenerCredencialesUsuario(int usuarioID)
        {
            Respuesta<List<_credencialConHistorialClaves>> respuesta;
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
                    List<_credencialConHistorialClaves> credencialConHistorial = new List<_credencialConHistorialClaves>();

                    foreach (var c in credenciales)
                    {
                        List<_historialClaves> claves = new List<_historialClaves>();

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
                                new _historialClaves
                                {
                                    CredencialClave = _encripcion.Decrypt(clave.CredencialClave),
                                    FechaCreacionClave = clave.FechaCreacion
                                });
                        }

                        

                        credencialConHistorial.Add(
                            new _credencialConHistorialClaves
                            {
                                ID = c.ID,
                                UsuarioNombre = usuario.Nombre,
                                ServidorNombre = servidor.Nombre,
                                CredencialDescripcion = c.Descripcion,
                                Activo = c.Activo,
                                CredencialUsuario = c.CredencialUsuario,
                                CredencialClaves = claves
                            });
                    }

                    mensaje = "Credenciales encontradas";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<_credencialConHistorialClaves>>(codigo, true, mensaje, credencialConHistorial);
                }
                else
                {
                    mensaje = "No se encontraron credencial para el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<_credencialConHistorialClaves>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<_credencialConHistorialClaves>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener todas las credenciales por usuario
        [HttpGet("ObtenerCredencialesPorUsuario/{usuarioID}")]
        public async Task<ActionResult<Respuesta<List<_credencialConClave>>>> obtenerCredencialesPorUsuario(int usuarioID)
        {
            Respuesta<List<_credencialConClave>> respuesta;
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
                    List<_credencialConClave> credencialConClave = new List<_credencialConClave>();

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
                                new _credencialConClave
                                {
                                    ID = c.ID,
                                    UsuarioNombre = usuario.Nombre,
                                    ServidorNombre = servidor.Nombre,
                                    CredencialDescripcion = c.Descripcion,
                                    Activo = c.Activo,
                                    CredencialUsuario = c.CredencialUsuario,
                                    CredencialClave = _encripcion.Decrypt(credencial.CredencialClave),
                                    FechaCreacionClave = credencial.FechaCreacion,
                                    FechaEncripcionClave = credencial.FechaEncripcion
                                });
                    }

                    mensaje = "Credenciales encontradas";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<_credencialConClave>>(codigo, true, mensaje, credencialConClave);
                }
                else
                {
                    mensaje = "No se encontraron credencial para el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<_credencialConClave>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<_credencialConClave>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener credencial con historial de claves por ID
        [HttpGet("ObtenerCredencialHistorial/{credencialID}")]
        public async Task<ActionResult<Respuesta<_credencialConHistorialClaves>>> obtenerCredencialHistorialPorID(int credencialID)
        {
            Respuesta<_credencialConHistorialClaves> respuesta;
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
                    List<_historialClaves> claves = new List<_historialClaves>();

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
                            new _historialClaves
                            {
                                CredencialClave = _encripcion.Decrypt(clave.CredencialClave),
                                FechaCreacionClave = clave.FechaCreacion
                            });
                    }

                    _credencialConHistorialClaves credencialConHistorial =
                                                 new _credencialConHistorialClaves
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialDescripcion = credencial.Descripcion,
                                                     Activo = credencial.Activo,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     CredencialClaves = claves
                                                 };

                    mensaje = "Credencial encontrada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_credencialConHistorialClaves>(codigo, true, mensaje, credencialConHistorial);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_credencialConHistorialClaves>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConHistorialClaves>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Obtener credencial por ID
        [HttpGet("ObtenerCredencial/{credencialID}")]
        public async Task<ActionResult<Respuesta<_credencialConClave>>> obtenerCredencialPorID(int credencialID)
        {
            Respuesta<_credencialConClave> respuesta;
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

                    _credencialConClave credencialConClave =
                                                 new _credencialConClave
                                                 {
                                                     ID = credencial.ID,
                                                     UsuarioNombre = usuario.Nombre,
                                                     ServidorNombre = servidor.Nombre,
                                                     CredencialDescripcion = credencial.Descripcion,
                                                     Activo = credencial.Activo,
                                                     CredencialUsuario = credencial.CredencialUsuario,
                                                     CredencialClave = _encripcion.Decrypt(credencialHistorial.CredencialClave),
                                                     FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                     FechaEncripcionClave = credencialHistorial.FechaEncripcion
                                                 };

                    mensaje = "Credencial encontrada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_credencialConClave>(codigo, true, mensaje, credencialConClave);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Agregar credencial
        [HttpPost("AgregarCredencial")]
        public async Task<ActionResult<Respuesta<_credencialConClave>>> agregarCredencial(_credencialNueva credencial)
        {
            Respuesta<_credencialConClave> respuesta;
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
                    respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
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
                        respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else if (usuarioExiste == null)
                    {
                        mensaje = "Usuario no existe";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else if (servidorExiste == null)
                    {
                        mensaje = "Servidor no existe";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                        return respuesta;
                    }
                    else
                    {
                        _encripcion = new AESCryptoService(usuarioExiste.Key);

                        t_Credencial nuevaCredencial = new t_Credencial
                        {
                            UsuarioID = credencial.UsuarioID,
                            ServidorID = credencial.ServidorID,
                            Descripcion = credencial.CredencialDescripcion,
                            CredencialUsuario = credencial.CredencialUsuario,
                            UsuarioModificacion = _usuarioModificacion
                        };

                        _context.Credenciales.Add(nuevaCredencial);
                        await _context.SaveChangesAsync();

                        t_CredencialHistorial credencialHistorial = new t_CredencialHistorial
                        {
                            CredencialID = nuevaCredencial.ID,
                            CredencialClave = _encripcion.Encrypt(credencial.CredencialClave),
                            UsuarioModificacion = _usuarioModificacion
                        };

                        _context.CredencialHistoriales.Add(credencialHistorial);
                        await _context.SaveChangesAsync();

                        _credencialConClave credencialConClave =
                                                     new _credencialConClave
                                                     {
                                                         ID = nuevaCredencial.ID,
                                                         UsuarioNombre = usuarioExiste.Nombre,
                                                         ServidorNombre = servidorExiste.Nombre,
                                                         CredencialDescripcion = nuevaCredencial.Descripcion,
                                                         Activo = nuevaCredencial.Activo,
                                                         CredencialUsuario = nuevaCredencial.CredencialUsuario,
                                                         CredencialClave = credencial.CredencialClave,
                                                         FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                         FechaEncripcionClave = credencialHistorial.FechaEncripcion
                                                     };

                        mensaje = "Credencial agregada";
                        codigo = StatusCodes.Status201Created;
                        respuesta = new Respuesta<_credencialConClave>(codigo, true, mensaje, credencialConClave);
                    }
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Actualizar credencial
        [HttpPut("ActualizarCredencialClave")]
        public async Task<ActionResult<Respuesta<_credencialConClave>>> actualizarCredencial(_credencialNueva credencial)
        {
            Respuesta<_credencialConClave> respuesta;
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
                    respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
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

                    t_CredencialHistorial credencialHistorial = new t_CredencialHistorial
                    {
                        CredencialID = credencialExiste.ID,
                        CredencialClave = _encripcion.Encrypt(credencial.CredencialClave),
                        UsuarioModificacion = _usuarioModificacion
                    };

                    _context.CredencialHistoriales.Add(credencialHistorial);
                    await _context.SaveChangesAsync();

                    _credencialConClave credencialConClave =
                                                 new _credencialConClave
                                                 {
                                                     ID = credencialExiste.ID,
                                                     UsuarioNombre = usuarioExiste.Nombre,
                                                     ServidorNombre = servidorExiste.Nombre,
                                                     CredencialDescripcion = credencialExiste.Descripcion,
                                                     Activo = credencialExiste.Activo,
                                                     CredencialUsuario = credencialExiste.CredencialUsuario,
                                                     CredencialClave = credencial.CredencialClave,
                                                     FechaCreacionClave = credencialHistorial.FechaCreacion,
                                                     FechaEncripcionClave = credencialHistorial.FechaEncripcion
                                                 };

                    mensaje = "Credencial actualizada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_credencialConClave>(codigo, true, mensaje, credencialConClave);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Activar credencial
        [HttpPut("ActivarCredencial/{credencialID}")]
        public async Task<ActionResult<Respuesta<_credencialConClave>>> activarCredencial(int credencialID)
        {
            Respuesta<_credencialConClave> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                var credencial = await _context.Credenciales
                                                .Where(c => c.ID == credencialID)
                                                .FirstOrDefaultAsync();
                if (credencial != null)
                {
                    credencial.Activo = true;
                    credencial.UsuarioModificacion = _usuarioModificacion;
                    credencial.FechaModificacion = DateTime.Now;
                    _context.Credenciales.Update(credencial);
                    await _context.SaveChangesAsync();

                    mensaje = "Credencial activada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_credencialConClave>(codigo, true, mensaje);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }

        //Desactivar credencial
        [HttpPut("DesactivarCredencial/{credencialID}")]
        public async Task<ActionResult<Respuesta<_credencialConClave>>> desactivarCredencial(int credencialID)
        {
            Respuesta<_credencialConClave> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                var credencial = await _context.Credenciales
                                                .Where(c => c.ID == credencialID)
                                                .FirstOrDefaultAsync();
                if (credencial != null)
                {
                    credencial.Activo = false;
                    credencial.UsuarioModificacion = _usuarioModificacion;
                    credencial.FechaModificacion = DateTime.Now;
                    _context.Credenciales.Update(credencial);
                    await _context.SaveChangesAsync();

                    mensaje = "Credencial desactivada";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_credencialConClave>(codigo, true, mensaje);
                }
                else
                {
                    mensaje = "No se encontró la credencial";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_credencialConClave>(codigo, false, mensaje);
            }

            return respuesta;
        }
    }
}
