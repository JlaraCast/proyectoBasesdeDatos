using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;

        public RoleController(DbContextSeguridad pContext)
        {
            _context = pContext;
        }

        // Método 1: Listar todos los roles
        [HttpGet("List")]
        public List<Role> List()
        {
            List<Role> temp = _context.roles.ToList();
            return temp;
        }

        // Método 2: Buscar un rol por ID
        [HttpGet("SearchID")]
        public IActionResult SearchID(int idRol, int idSistema)
        {
            var temp = _context.roles
                .FirstOrDefault(x => x.idRol == idRol && x.idSistema == idSistema);

            if (temp == null)
            {
                return NotFound($"No existe un rol con el identificador (idRol: {idRol}, idSistema: {idSistema}).");
            }

            return Ok(temp);
        }


        // Método 3: Guardar un nuevo rol
        [HttpPost("Save")]
        public string Save([FromBody] Role temp)
        {
            string msj = "Rol guardado correctamente.";
            try
            {
                _context.roles.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }

        [HttpDelete("Delete")]
        public string Delete(int pIdRole, int pIdSistema)
        {
            string msg = "Eliminando rol...";
            try
            {
                var role = _context.roles.FirstOrDefault(r => r.idRol == pIdRole && r.idSistema == pIdSistema);
                if (role == null)
                {
                    msg = "Rol no encontrado.";
                }
                else
                {
                    _context.roles.Remove(role);
                    _context.SaveChanges();
                    msg = "Rol eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }
            return msg;
        }

        [HttpPut("Update")]
        public string Update(Role temp)
        {
            string msj = "Actualizando rol...";
            try
            {
                var obj = _context.roles.FirstOrDefault(r => r.idRol == temp.idRol && r.idSistema == temp.idSistema);
                if (obj == null)
                {
                    msj = "No existe el rol.";
                }
                else
                {
                    obj.nombre = temp.nombre;
                    obj.descripcion = temp.descripcion;
                    _context.roles.Update(obj);
                    _context.SaveChanges();
                    msj = "Rol actualizado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }


    }
}
