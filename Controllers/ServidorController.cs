using API_CredentialManager.Data;
using API_CredentialManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.SqlServer.Server;
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

        // Obtener todos los servidores
        [HttpGet("ObtenerServidores")]
        public async Task<ActionResult<Respuesta<List<Servidor>>>> obtenerServidores()
        {
            Respuesta<List<Servidor>> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var Servidores = await _context.Servidores.ToListAsync();

                if (Servidores.Any())
                {
                    mensaje = "Se encontraron servidores";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<Servidor>>(codigo, true, mensaje, Servidores);
                }
                else
                {
                    mensaje = "No se encontraron servidores";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<Servidor>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<Servidor>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Obtener todos los servidores activos
        [HttpGet("ObtenerServidoresActivos")]
        public async Task<ActionResult<Respuesta<List<Servidor>>>> obtenerServidoresActivos()
        {
            Respuesta<List<Servidor>> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var Servidores = await _context.Servidores
                                .Where(e => e.Activo == true)
                                .ToListAsync();

                if (Servidores.Any())
                {
                    mensaje = "Se encontraron servidores activos";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<Servidor>>(codigo, true, mensaje, Servidores);
                }
                else
                {
                    mensaje = "No se encontraron servidores activos";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<Servidor>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<Servidor>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Obtener un servidor por ID
        [HttpGet("ObtenerServidor/{id}")]
        public async Task<ActionResult<Respuesta<Servidor>>> obtenerServidor(int id)
        {
            Respuesta<Servidor> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var Servidor = await _context.Servidores
                                            .Where(e => e.ID == id)
                                            .FirstOrDefaultAsync();

                if (Servidor != null)
                {
                    mensaje = "Se encontró el servidor";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<Servidor>(codigo, true, mensaje, Servidor);
                }
                else
                {
                    mensaje = "No se encontró el servidor";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Crear un servidor
        [HttpPost("CrearServidor")]
        public async Task<ActionResult<Respuesta<Servidor>>> crearServidor(Servidor servidor)
        {
            Respuesta<Servidor> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "UsuarioPrueba";

            try
            {
                //Validar que el servidor no exista
                var existeServidor = await _context.Servidores
                                    .Where(e => e.Nombre == servidor.Nombre)
                                    .FirstOrDefaultAsync();

                if (existeServidor != null)
                {
                    mensaje = "El servidor ya existe";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                }
                else
                {
                    servidor.UsuarioModificacion = _usuarioModificacion;

                    _context.Servidores.Add(servidor);
                    await _context.SaveChangesAsync();

                    mensaje = "Servidor creado";
                    codigo = StatusCodes.Status201Created;
                    respuesta = new Respuesta<Servidor>(codigo, true, mensaje, servidor);


                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Actualizar un servidor
        [HttpPut("ActualizarServidor/{id}")]
        public async Task<ActionResult<Respuesta<Servidor>>> actualizarServidor(int id, Servidor servidor)
        {
            Respuesta<Servidor> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "UsuarioPrueba";

            try
            {
                if (id != servidor.ID)
                {
                    mensaje = "El ID no coincide con el servidor";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                }
                else
                {
                    var existeServidor = await _context.Servidores
                                            .Where(e => e.ID == id)
                                            .FirstOrDefaultAsync();

                    if (existeServidor == null)
                    {
                        mensaje = "El servidor no existe";
                        codigo = StatusCodes.Status404NotFound;
                        respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                    }
                    else
                    {
                        existeServidor.Nombre = servidor.Nombre;
                        existeServidor.IP = servidor.IP;
                        existeServidor.UsuarioModificacion = _usuarioModificacion;
                        existeServidor.FechaModificacion = System.DateTime.Now;

                        _context.Servidores.Update(existeServidor);
                        await _context.SaveChangesAsync();

                        mensaje = "Servidor actualizado";
                        codigo = StatusCodes.Status200OK;
                        respuesta = new Respuesta<Servidor>(codigo, true, mensaje, servidor);
                    }
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Activar un servidor
        [HttpPut("ActivarServidor/{id}")]
        public async Task<ActionResult<Respuesta<Servidor>>> activarServidor(int id)
        {
            Respuesta<Servidor> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "UsuarioPrueba";

            try
            {
                var servidor = await _context.Servidores
                                            .Where(e => e.ID == id)
                                            .FirstOrDefaultAsync();

                if (servidor == null)
                {
                    mensaje = "El servidor no existe";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                }
                else
                {
                    servidor.Activo = true;
                    servidor.UsuarioModificacion = _usuarioModificacion;
                    servidor.FechaModificacion = System.DateTime.Now;

                    _context.Servidores.Update(servidor);
                    await _context.SaveChangesAsync();

                    mensaje = "Servidor activado";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<Servidor>(codigo, true, mensaje, servidor);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Desactivar un servidor
        [HttpPut("DesactivarServidor/{id}")]
        public async Task<ActionResult<Respuesta<Servidor>>> desactivarServidor(int id)
        {
            Respuesta<Servidor> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "UsuarioPrueba";

            try
            {
                var servidor = await _context.Servidores
                                            .Where(e => e.ID == id)
                                            .FirstOrDefaultAsync();

                if (servidor == null)
                {
                    mensaje = "El servidor no existe";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
                }
                else
                {
                    servidor.Activo = false;
                    servidor.UsuarioModificacion = _usuarioModificacion;
                    servidor.FechaModificacion = System.DateTime.Now;

                    _context.Servidores.Update(servidor);
                    await _context.SaveChangesAsync();

                    mensaje = "Servidor desactivado";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<Servidor>(codigo, true, mensaje, servidor);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<Servidor>(codigo, false, mensaje);
            }

            return respuesta;
        }
    }
}
