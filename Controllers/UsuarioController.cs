﻿using API_CredentialManager.Data;
using API_CredentialManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CredentialManager.Controllers
{
    public struct _usuario
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DataBaseContext _context;

        public UsuarioController(DataBaseContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        [HttpGet("ObtenerUsuarios")]
        public async Task<ActionResult<Respuesta<List<_usuario>>>> obtenerUsuarios()
        {
            Respuesta<List<_usuario>> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var Usuarios = await _context.Usuarios.ToListAsync();

                if (Usuarios.Any())
                {
                    List<_usuario> _Usuarios = new List<_usuario>();

                    foreach (var usuario in Usuarios)
                    {
                        _Usuarios.Add(new _usuario
                        {
                            ID = usuario.ID,
                            Nombre = usuario.Nombre,
                            Correo = usuario.Correo,
                            Activo = usuario.Activo
                        });

                    }

                    mensaje = "Se encontraron usuarios";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<_usuario>>(codigo, true, mensaje, _Usuarios);
                }
                else
                {
                    mensaje = "No se encontraron usuarios";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<_usuario>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<_usuario>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Obtener todos los usuarios activos
        [HttpGet("ObtenerUsuariosActivos")]
        public async Task<ActionResult<Respuesta<List<_usuario>>>> obtenerUsuariosActivos()
        {
            Respuesta<List<_usuario>> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var Usuarios = await _context.Usuarios
                                .Where(u => u.Activo == true)
                                .ToListAsync();

                if (Usuarios.Any())
                {
                    List<_usuario> _Usuarios = new List<_usuario>();

                    foreach (var usuario in Usuarios)
                    {
                        _Usuarios.Add(new _usuario
                        {
                            ID = usuario.ID,
                            Nombre = usuario.Nombre,
                            Correo = usuario.Correo,
                            Activo = usuario.Activo
                        });
                    }

                    mensaje = "Se encontraron usuarios activos";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<List<_usuario>>(codigo, true, mensaje, _Usuarios);
                }
                else
                {
                    mensaje = "No se encontraron usuarios activos";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<List<_usuario>>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<List<_usuario>>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Obtener usuario por ID
        [HttpGet("ObtenerUsuario/{id}")]
        public async Task<ActionResult<Respuesta<_usuario>>> obtenerUsuario(int id)
        {
            Respuesta<_usuario> respuesta;
            string mensaje;
            int codigo;

            try
            {
                var usuario = await _context.Usuarios
                                .Where(e => e.ID == id)
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    var _usuario = new _usuario
                    {
                        ID = usuario.ID,
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Activo = usuario.Activo
                    };

                    mensaje = "Se encontró el usuario";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_usuario>(codigo, true, mensaje, _usuario);
                }
                else
                {
                    mensaje = "No se encontró el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Crear un usuario
        [HttpPost("CrearUsuario")]
        public async Task<ActionResult<Respuesta<_usuario>>> crearUsuario(t_Usuario usuario)
        {
            Respuesta<_usuario> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                // Verificar si el usuario ya existe
                var usuarioConNombre = await _context.Usuarios
                                        .Where(e => e.Nombre == usuario.Nombre)
                                        .FirstOrDefaultAsync();

                var usuarioConCorreo = await _context.Usuarios
                                          .Where(e => e.Correo == usuario.Correo)
                                          .FirstOrDefaultAsync();

                if (usuarioConNombre != null && usuarioConCorreo != null)
                {
                    mensaje = "El nombre de usuario y el correo ya están registrados.";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
                else if (usuarioConNombre != null)
                {
                    mensaje = "El nombre de usuario ya está registrado.";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
                else if (usuarioConCorreo != null)
                {
                    mensaje = "El correo ya está registrado.";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
                else
                {
                    if (usuario.Clave.Equals(String.Empty))
                    {
                        mensaje = "La clave es requerida";
                        codigo = StatusCodes.Status400BadRequest;
                        respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                    }
                    else
                    {
                        usuario.UsuarioModificacion = _usuarioModificacion;
                        usuario.crearKey();
                        usuario.encriptarClave();

                        _context.Usuarios.Add(usuario);
                        await _context.SaveChangesAsync();

                        var _usuario = new _usuario
                        {
                            ID = usuario.ID,
                            Nombre = usuario.Nombre,
                            Correo = usuario.Correo,
                            Activo = usuario.Activo
                        };

                        mensaje = "Usuario creado";
                        codigo = StatusCodes.Status201Created;
                        respuesta = new Respuesta<_usuario>(codigo, true, mensaje, _usuario);
                    }
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Actualizar un usuario
        [HttpPut("ActualizarUsuario/{id}")]
        public async Task<ActionResult<Respuesta<_usuario>>> actualizarUsuario(int id, t_Usuario usuario)
        {
            Respuesta<_usuario> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                if (id != usuario.ID)
                {
                    mensaje = "El ID no coincide con el usuario";
                    codigo = StatusCodes.Status400BadRequest;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
                else
                {

                    var usuarioActual = await _context.Usuarios
                                    .Where(e => e.ID == id && e.Nombre == usuario.Nombre)
                                    .FirstOrDefaultAsync();

                    if (usuarioActual != null)
                    {
                        // Verificar si el nombre de usuario ya existe
                        var usuarioConNombre = await _context.Usuarios
                                                .Where(e => e.Nombre == usuario.Nombre && e.ID != id)
                                                .FirstOrDefaultAsync();

                        var usuarioConCorreo = await _context.Usuarios
                                                .Where(e => e.Correo == usuario.Correo && e.ID != id)
                                                .FirstOrDefaultAsync();

                        if (usuarioConNombre != null && usuarioConCorreo != null)
                        {
                            mensaje = "El nombre de usuario y el correo ya están registrados.";
                            codigo = StatusCodes.Status400BadRequest;
                            respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                        }
                        else if (usuarioConNombre != null)
                        {
                            mensaje = "El nombre de usuario ya está registrado.";
                            codigo = StatusCodes.Status400BadRequest;
                            respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                        }
                        else if (usuarioConCorreo != null)
                        {
                            mensaje = "El correo ya está registrado.";
                            codigo = StatusCodes.Status400BadRequest;
                            respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                        }
                        else
                        {
                            //usuarioActual.Nombre = usuario.Nombre;
                            usuarioActual.Correo = usuario.Correo;
                            usuarioActual.UsuarioModificacion = _usuarioModificacion;
                            usuarioActual.FechaModificacion = System.DateTime.Now;

                            _context.Usuarios.Update(usuarioActual);
                            await _context.SaveChangesAsync();

                            var _usuario = new _usuario
                            {
                                ID = usuarioActual.ID,
                                Nombre = usuarioActual.Nombre,
                                Correo = usuarioActual.Correo,
                                Activo = usuarioActual.Activo
                            };

                            mensaje = "Usuario actualizado";
                            codigo = StatusCodes.Status200OK;
                            respuesta = new Respuesta<_usuario>(codigo, true, mensaje, _usuario);
                        }
                    }
                    else
                    {
                        mensaje = "No se encontró el usuario";
                        codigo = StatusCodes.Status404NotFound;
                        respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                    }
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Activar un usuario
        [HttpPut("ActivarUsuario/{id}")]
        public async Task<ActionResult<Respuesta<_usuario>>> activarUsuario(int id)
        {
            Respuesta<_usuario> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                var usuario = await _context.Usuarios
                                .Where(e => e.ID == id)
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    usuario.Activo = true;
                    usuario.UsuarioModificacion = _usuarioModificacion;
                    usuario.FechaModificacion = System.DateTime.Now;

                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();

                    usuario.ocultarClave();
                    usuario.ocultarKey();

                    mensaje = "Usuario activado";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_usuario>(codigo, true, mensaje);
                }
                else
                {
                    mensaje = "No se encontró el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
            }

            return respuesta;
        }

        // Desactivar un usuario
        [HttpPut("DesactivarUsuario/{id}")]
        public async Task<ActionResult<Respuesta<_usuario>>> desactivarUsuario(int id)
        {
            Respuesta<_usuario> respuesta;
            string mensaje;
            int codigo;
            string _usuarioModificacion = "API";

            try
            {
                var usuario = await _context.Usuarios
                                .Where(e => e.ID == id)
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    usuario.Activo = false;
                    usuario.UsuarioModificacion = _usuarioModificacion;
                    usuario.FechaModificacion = System.DateTime.Now;

                    _context.Usuarios.Update(usuario);
                    await _context.SaveChangesAsync();

                    usuario.ocultarClave();
                    usuario.ocultarKey();

                    mensaje = "Usuario desactivado";
                    codigo = StatusCodes.Status200OK;
                    respuesta = new Respuesta<_usuario>(codigo, true, mensaje);
                }
                else
                {
                    mensaje = "No se encontró el usuario";
                    codigo = StatusCodes.Status404NotFound;
                    respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = e.Message;
                codigo = StatusCodes.Status500InternalServerError;
                respuesta = new Respuesta<_usuario>(codigo, false, mensaje);
            }

            return respuesta;
        }
    }
}
