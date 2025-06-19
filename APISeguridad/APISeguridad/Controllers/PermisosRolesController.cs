using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PermisosRolesController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;

        public PermisosRolesController(DbContextSeguridad pContext)
        {
            _context = pContext;
        }

        [HttpGet("List")]
        public List<PermisosRoles> List()
        {
            List<PermisosRoles> temp = _context.permisosRoles.ToList();

            return temp;
        }

        [HttpGet("SearchID")]
        public IActionResult SearchID(int id)
        {
            var temp = _context.permisosRoles.FirstOrDefault(x => x.idRol == id);

            if (temp == null)
            {
                return NotFound($"No existe un permiso con el identificador");
            }
            return Ok(temp);
        }

        [HttpPost("Save")]
        public string Save(PermisosRoles temp)
        {
            string msj = "Permisos del usuario guardados correctamente.";
            try
            {
                _context.permisosRoles.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }


        [HttpDelete("Delete")]
        public string Delete(int id)
        {
            string msg = "Permisos del usuario eliminado...";

            try
            {
                var permiso = _context.permisosRoles.FirstOrDefault(r => r.idRol == id);

                if (permiso == null)
                {
                    msg = "No existe";
                }
                else
                {

                    _context.permisosRoles.Remove(permiso);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }
    }
}
