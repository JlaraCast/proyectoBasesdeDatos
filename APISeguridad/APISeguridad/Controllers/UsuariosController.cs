using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    public class UsuariosController : ControllerBase
    {
        //Declara una variable de tipo DbContext
        private readonly DbContextSeguridad _context = null;

        //Constructor con parámetros 
        //recibe la instacia del ORM para interactuar con la base datos
        public UsuariosController(DbContextSeguridad pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }

        // Método 1: Acción HTTP GET para listar todos los usuarios
        // La ruta para acceder a este método sería: /[controller]/List
        [HttpGet("List")]
        public List<Usuario> List()
        {
            // Variable local de tipo lista que almacenará los usuarios recuperados de la base de datos.
            // Se accede a la tabla usuarios mediante el contexto de base de datos (_context).
            List<Usuario> temp = _context.usuarios.ToList();

            // Se retorna la lista de clientes al cliente que hizo la solicitud HTTP.
            return temp;
        }

        // Método 2: Acción HTTP GET para buscar un usuario por su identificador (ID).
        [HttpGet("SearchID")]
        public IActionResult SearchID(int id)
        {
            // Se utiliza el contexto (_context) y el ORM para buscar el primer usuario
            // que coincida con el ID proporcionado como parámetro.
            var temp = _context.usuarios.FirstOrDefault(x => x.idUsuario == id);

            // Validación: si no se encuentra el usuario, se retorna un mensaje indicando que no existe.
            if (temp == null)
            {
                // Retorna una respuesta 404 (Not Found) con un mensaje personalizado.
                return NotFound($"No existe un usuario con el identificador {id}.");
            }

            // Si el paquete fue encontrado, se retorna con un resultado HTTP 200 (OK).
            return Ok(temp);
        }

        // Método 3: Acción HTTP POST encargada de guardar un nuevo usuario en la base de datos.
        [HttpPost("Save")]
        public string Save(Usuario temp)
        {
            // Mensaje inicial que se devolverá al usuario.
            string msj = "Usuario guardado correctamente.";

            try
            {
                // Se agrega el usuario recibido (temp) al contexto de base de datos.
                _context.usuarios.Add(temp);

                // Se aplican los cambios a la base de datos.
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // En caso de que ocurra un error durante la inserción, se captura el detalle del error.
                // El mensaje de error reemplaza el mensaje inicial para informar al usuario.
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            // Se retorna el mensaje final, ya sea de éxito o error.
            return msj;
        }

        // Método 4: Encargado de eliminar los datos de un usuario.
        [HttpDelete("Delete")]
        public string Delete(int pIdUsuario)
        {
            string msg = "Delete usuario...";

            try
            {
                //se obtiene el usuario
                var usuario = _context.usuarios.FirstOrDefault(r => r.idUsuario == pIdUsuario);

                //se verifica que no sea un valor nulo
                if (usuario == null)
                {
                    msg = "Usuario not found.";
                }
                else
                {
                    //se elimina de la BD
                    _context.usuarios.Remove(usuario);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }

        // Método 5:encargado de modificar los datos de un usuario.
        [HttpPut("Update")]
        public string Update(Usuario temp)
        {
            string msj = "Actualizando usuario...";

            try
            {
                // Se busca el usuario en la base de datos 
                var obj= _context.usuarios.FirstOrDefault(r => r.idUsuario == temp.idUsuario);

                if (obj == null)
                {
                    msj = "No existe el usuario.";
                }
                else
                {
                    // Se actualizan los datos del usuario con la nueva información proporcionada en 'temp'.
                    obj.nombre = temp.nombre;
                    obj.correo = temp.correo;
                    obj.clave = temp.clave;
                    obj.estado = temp.estado;

                    // Se marca el usuario como actualizado en el contexto de la base de datos.
                    _context.usuarios.Update(obj);

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
