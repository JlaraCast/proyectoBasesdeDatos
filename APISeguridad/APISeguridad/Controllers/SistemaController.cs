using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SistemaController : ControllerBase
    {
        private readonly DbContextSeguridad _context = null;
        public SistemaController(DbContextSeguridad pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }
        // Método 1: Listar todos los sistemas
        [HttpGet("List")]
        public List<Sistema> List()
        {
            List<Sistema> temp = _context.sistemas.ToList();
            return temp;
        }
        // Método 2: Buscar un sistema por ID
        [HttpGet("SearchID")]
        public IActionResult SearchID(int id)
        {
            var temp = _context.sistemas.FirstOrDefault(x => x.idSistema == id);
            if (temp == null)
            {
                return NotFound($"No existe un sistema con el identificador {id}.");
            }
            return Ok(temp);
        }
        // Método 3: Guardar un nuevo sistema
        [HttpPost("Save")]
        public string Save(Sistema temp)
        {
            string msj = "Sistema guardado correctamente.";
            try
            {
                _context.sistemas.Add(temp);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }
            return msj;
        }
        // Método 4: Eliminar un sistema
        [HttpDelete("Delete")]
        public string Delete(int pIdSistema)
        {
            string msg = "Delete sistema...";
            try
            {
                var sistema = _context.sistemas.FirstOrDefault(x => x.idSistema == pIdSistema);
                if (sistema != null)
                {
                    _context.sistemas.Remove(sistema);
                    _context.SaveChanges();
                    msg = "Sistema eliminado correctamente.";
                }
                else
                {
                    msg = $"No existe un sistema con el identificador {pIdSistema}.";
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }
            return msg;
        }
        // Método 5: Modificar un sistema
        [HttpPut("Update")]
        public string Update(Sistema temp)
        {
            string msj = "Actualizando sistema...";
            try
            {
                var obj = _context.sistemas.FirstOrDefault(s => s.idSistema == temp.idSistema);
                if (obj == null)
                {
                    msj = "No existe el sistema.";
                }
                else
                {
                    obj.nombre = temp.nombre;
                    obj.descripcion = temp.descripcion;
                    _context.sistemas.Update(obj);

                    // Se aplican los cambios en la base de datos.
                    _context.SaveChanges();
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
