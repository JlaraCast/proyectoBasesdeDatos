using System.Collections;
using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PermisosUsuariosController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;

        public PermisosUsuariosController(DbContextSeguridad pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        public List<PermisoUsuario> List()
        {
            List<PermisoUsuario> temp = _context.permisosUsuarios.ToList();

            return temp;
        }

        [HttpGet("SearchID")]
        public IActionResult SearchID(int idUsuario, int idSistema, int idPantalla)
        {
            var temp = _context.permisosUsuarios.FirstOrDefault(x => x.IdUsuario == idUsuario && x.idSistema == idSistema && x.IdPantalla == idPantalla);

            if (temp == null)
            {
                return NotFound($"No existe un permiso con el identificador");
            }
            return Ok(temp);
        }

        [HttpGet("SearchListUserPermissions")]
        public IActionResult SearchListUserPermissions(int idUsuario)
        {
            var permisos = _context.permisosUsuarios.Where(p => p.IdUsuario == idUsuario).ToList();

            if (permisos == null || permisos.Count == 0)
            {
                return NotFound("El usuario no tiene permisos asignados.");
            }

            return Ok(permisos);

        }


        [HttpPost("Save")]
        public IActionResult Save([FromBody] PermisoUsuario temp)
        {
            try
            {
                _context.permisosUsuarios.Add(temp);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("Delete")]
        public string Delete(int idUsuario, int idSistema, int idPantalla)
        {
            string msg = "Permisos del usuario eliminado...";

            try
            {
                var permiso = _context.permisosUsuarios.FirstOrDefault(r => r.IdUsuario == idUsuario && r.idSistema == idSistema && r.IdPantalla == idPantalla);

                if (permiso == null)
                {
                    msg = "No existe";
                }
                else
                {

                    _context.permisosUsuarios.Remove(permiso);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }


        //Sharon 17/6/2025
        //Metodos editar
        [HttpPut("Edit")]
        public IActionResult Edit([FromBody] PermisoUsuario permisoEditado)
        {
            if (permisoEditado == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var permisoExistente = _context.permisosUsuarios.FirstOrDefault(p =>
                p.IdUsuario == permisoEditado.IdUsuario &&
                p.idSistema == permisoEditado.idSistema &&
                p.IdPantalla == permisoEditado.IdPantalla);

            if (permisoExistente == null)
            {
                return NotFound("No se encontró el permiso del usuario.");
            }

            // Actualizar los campos
            permisoExistente.PermisoInsertar = permisoEditado.PermisoInsertar;
            permisoExistente.PermisoModificar = permisoEditado.PermisoModificar;
            permisoExistente.PermisoBorrar = permisoEditado.PermisoBorrar;
            permisoExistente.PermisoConsultar = permisoEditado.PermisoConsultar;

            _context.SaveChanges();

            return Ok("Permiso actualizado correctamente.");
        }


    }
}
