using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult SearchID(int id)
        {
            // Se utiliza el contexto (_context) y el ORM para buscar la pantalla por numero
            // que coincida con el ID proporcionado como parámetro.
            var temp = _context.pantallas.FirstOrDefault(x => x.idPantalla == id);

            // Validación: si no se encuentra la pantalla, se retorna un mensaje indicando que no existe.
            if (temp == null)
            {
                // Retorna una respuesta 404 (Not Found) con un mensaje personalizado.
                return NotFound($"No existe una pantalla con el identificador {id}.");
            }

            // Si la pantalla fue encontrado, se retorna con un resultado HTTP 200 (OK).
            return Ok(temp);
        }

        // Método 3: Acción HTTP POST encargada de guardar una nueva pantalla en la base de datos.
        [HttpPost("Save")]
        public string Save(Pantalla temp)
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
        public string Delete(int pIdPantalla)
        {
            string msg = "Delete Pantalla...";

            try
            {
                //se obtiene la pantalla
                var pantalla = _context.pantallas.FirstOrDefault(r => r.idPantalla== pIdPantalla);

                //se verifica que no sea un valor nulo
                if (pantalla == null)
                {
                    msg = "Pantalla not found.";
                }
                else
                {
                    //se elimina de la BD
                    _context.pantallas.Remove(pantalla);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }

        // Método 5:encargado de modificar los datos de una pantalla.
        [HttpPut("Update")]
        public string Update(Pantalla temp)
        {
            string msj = "Actualizando pantalla...";

            try
            {
                // Se busca la pantalla en la base de datos.
                var obj = _context.pantallas.FirstOrDefault(r => r.idPantalla == temp.idPantalla);

                if (obj == null)
                {
                    msj = "No existe la pantalla.";
                }
                else
                {
                    // Se actualizan los datos de la pantalla con la nueva información proporcionada en 'temp'.
                    obj.idSistema = temp.idSistema;
                    obj.nombre = temp.nombre;
                    obj.descripcion = temp.descripcion;
                    obj.ruta = temp.ruta;

                    // Se marca el usuario como actualizado en el contexto de la base de datos.
                    _context.pantallas.Update(obj);

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
