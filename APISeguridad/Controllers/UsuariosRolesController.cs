using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APISeguridad.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsuariosRolesController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;

        public UsuariosRolesController(DbContextSeguridad pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        public List<UsuariosRol> List()
        {
            List<UsuariosRol> temp = _context.usuariosRoles.ToList();

            return temp;
        }

        [HttpGet("SearchID")]
        public IActionResult SearchID(int idUsuario, int idRol)
        {
            var temp = _context.usuariosRoles.FirstOrDefault(x => x.idUsuario == idUsuario && x.idRol == idRol);

            if (temp == null)
            {
                return NotFound($"No existe un permiso con el identificador");
            }
            return Ok(temp);
        }

        [HttpPost("Save")]
        public string Save(UsuariosRol temp)
        {
            string msj = "Permisos del usuario guardados correctamente.";
            try
            {
                _context.usuariosRoles.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        [HttpGet("SearchListUserRol")]
        public IActionResult SearchListUserRol(int idUsuario)
        {
            var permisos = _context.usuariosRoles.Where(p => p.idUsuario == idUsuario).ToList();

            if (permisos == null || permisos.Count == 0)
            {
                return NotFound("El usuario no tiene permisos asignados.");
            }

            return Ok(permisos);

        }

        [HttpDelete("Delete")]
        public string Delete(int idUsuario, int idRol)
        {
            string msg = "Permisos del usuario eliminado...";

            try
            {
                var permiso = _context.usuariosRoles.FirstOrDefault(r => r.idUsuario == idUsuario && r.idRol == idRol);

                if (permiso == null)
                {
                    msg = "No existe";
                }
                else
                {

                    _context.usuariosRoles.Remove(permiso);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }

        [HttpPut("Edit")]
        public IActionResult Edit([FromBody] UsuariosRol usuarioRolEditado)
        {
            if (usuarioRolEditado == null)
            {
                return BadRequest("Datos inválidos.");
            }

            // Buscar la relación existente del usuario con el rol actual
            var relacionExistente = _context.usuariosRoles
                .FirstOrDefault(ur => ur.idUsuario == usuarioRolEditado.idUsuario && ur.idRol == usuarioRolEditado.idRol);

            if (relacionExistente == null)
            {
                return NotFound("No se encontró el rol asignado al usuario.");
            }

            // Actualizar el rol
            relacionExistente.idRol = usuarioRolEditado.idRol;

            _context.SaveChanges();

            return Ok("Rol del usuario actualizado correctamente.");
        }
    }
}
