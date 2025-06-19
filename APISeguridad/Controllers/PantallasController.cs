using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APISeguridad.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    public class PantallasController : ControllerBase
    {
        //Declara una variable de tipo DbContext
        private readonly DbContextSeguridad _context = null;

        //Constructor con parámetros 
        //recibe la instacia del ORM para interactuar con la base datos
        public PantallasController(DbContextSeguridad pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }


        // Método 1: Acción HTTP GET para listar todos las pantallas
        // La ruta para acceder a este método sería: /[controller]/List
        [HttpGet("List")]
        public List<Pantalla> List()
        {
            // Variable local de tipo lista que almacenará las pantallas recuperados de la base de datos.
            // Se accede a la tabla pantallas mediante el contexto de base de datos (_context).
            List<Pantalla> temp = _context.pantallas.ToList();

            // Se retorna la lista de pantallas
            return temp;
        }

        // Método 2: Acción HTTP GET para buscar una pantalla por su identificador de numero (ID).
        [HttpGet("SearchID")]
        public IActionResult SearchID(int idPantalla, int idSistema)
        {
            var temp = _context.pantallas.FirstOrDefault(x => x.idPantalla == idPantalla && x.idSistema == idSistema);

            if (temp == null)
            {
                return NotFound($"No existe una pantalla con ID {idPantalla} en el sistema {idSistema}.");
            }

            return Ok(temp);
        }


        // Método 3: Acción HTTP POST encargada de guardar una nueva pantalla en la base de datos.
        [HttpPost("Save")]
        public string Save([FromBody]Pantalla temp)
        {
            string msj = "Pantalla guardada correctamente.";

            try
            {
                // Se agrega la pantalla recibido (temp) al contexto de base de datos.
                _context.pantallas.Add(temp);

                // Se aplican los cambios a la base de datos.
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
               
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            // Se retorna el mensaje final, ya sea de éxito o error.
            return msj;
        }

        // Método 4: Encargado de eliminar los datos de la pantalla.
        [HttpDelete("Delete")]
        public IActionResult Delete(int idPantalla, int idSistema)
        {
            try
            {
                var pantalla = _context.pantallas.FirstOrDefault(p => p.idPantalla == idPantalla && p.idSistema == idSistema);

                if (pantalla == null)
                {
                    return NotFound("Pantalla no encontrada.");
                }

                _context.pantallas.Remove(pantalla);
                _context.SaveChanges();

                return Ok("Pantalla eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }



        // Método 5:encargado de modificar los datos de una pantalla.
        [HttpPut("Update")]
        public string Update(Pantalla temp)
        {
            string msj = "Actualizando pantalla...";

            try
            {
                var obj = _context.pantallas.FirstOrDefault(p => p.idPantalla == temp.idPantalla && p.idSistema == temp.idSistema);

                if (obj == null)
                {
                    msj = "No existe la pantalla.";
                }
                else
                {
                    obj.nombre = temp.nombre;
                    obj.descripcion = temp.descripcion;
                    obj.ruta = temp.ruta;

                    _context.pantallas.Update(obj);
                    _context.SaveChanges();
                    msj = "Pantalla actualizada correctamente.";
                }
            }
            catch (Exception ex)
            {
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            return msj;
        }

        [HttpGet("SearchByRuta")]
        public async Task<IActionResult> SearchByRuta(int idSistema, string ruta)
        {
            // Busca la pantalla que coincida con idSistema y ruta exacta
            var pantalla = await _context.pantallas
                .Where(p => p.idSistema == idSistema && p.ruta == ruta)
                .FirstOrDefaultAsync();

            if (pantalla == null)
            {
                return NotFound($"No se encontró pantalla con ruta '{ruta}' y sistema {idSistema}");
            }

            return Ok(pantalla);
        }

        [HttpGet("ObtenerPorSistema")]
        public IActionResult ObtenerPorSistema(int idSistema)
        {
            var pantallas = _context.pantallas
                .Where(p => p.idSistema == idSistema)
                .Select(p => new
                {
                    p.idPantalla,
                    p.ruta
                })
                .ToList();

            return Ok(pantallas);
        }


    }//fin class
}
