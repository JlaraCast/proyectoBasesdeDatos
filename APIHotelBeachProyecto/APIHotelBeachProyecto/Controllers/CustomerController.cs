using APIHotelBeachProyecto.Model;
using Microsoft.AspNetCore.Mvc;

namespace APIHotelBeachProyecto.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    // Define una clase llamada "PackageController" que hereda de "Controller",
    // lo que indica que esta clase puede manejar solicitudes HTTP como parte del patrón MVC.
    public class CustomerController : Controller
    {

        //Declara una variable de tipo DbContext
        private readonly DbContextHotel _context = null;

        //Constructor con parámetros 
        //recibe la instacia del ORM para interactuar con la base datos
        public CustomerController(DbContextHotel pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }




        // Método 1: Acción HTTP GET para listar todos los clientes.
        // La ruta para acceder a este método sería: /[controller]/List
        [HttpGet("List")]
        public List<Customer> List()
        {
            // Variable local de tipo lista que almacenará los clientes recuperados de la base de datos.
            // Se accede a la tabla Customers mediante el contexto de base de datos (_context).
            List<Customer> temp = _context.Customers.ToList();

            // Se retorna la lista de clientes al cliente que hizo la solicitud HTTP.
            return temp;
        }



        // Método 2: Acción HTTP GET para buscar un paquete por su identificador (ISBN).
        [HttpGet("SearchID")]
        public IActionResult SearchID(int id)
        {
            // Se utiliza el contexto (_context) y el ORM para buscar el primer paquete
            // que coincida con el ISBN proporcionado como parámetro.
            var temp = _context.Customers.FirstOrDefault(x => x.CustomerId == id);

            // Validación: si no se encuentra el paquete, se retorna un mensaje indicando que no existe.
            if (temp == null)
            {
                // Retorna una respuesta 404 (Not Found) con un mensaje personalizado.
                return NotFound($"No existe un paquete con el identificador {id}.");
            }

            // Si el paquete fue encontrado, se retorna con un resultado HTTP 200 (OK).
            return Ok(temp);
        }


        // Método 3: Acción HTTP POST encargada de guardar un nuevo cliente en la base de datos.
        [HttpPost("Save")]
        public string Save(Customer temp)
        {
            // Mensaje inicial que se devolverá al usuario.
            string msj = "Cliente guardado correctamente.";

            try
            {
                // Se agrega el cliente recibido (temp) al contexto de base de datos.
                _context.Customers.Add(temp);

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




        // Método 4: Encargado de eliminar los datos de un paquete.
        [HttpDelete("Delete")]
        public string Delete(int pPackageId)
        {
            // Mensaje inicial que indica que se está intentando eliminar un paquete.
            string msj = "Eliminando paquete...";

            try
            {
                // Se busca el paquete en la base de datos usando el PackageId (anteriormente se usaba ISBN).
                var temp = _context.Customers.FirstOrDefault(r => r.CustomerId == pPackageId);

                // Si no se encuentra el paquete, se informa al usuario que no existe.
                if (temp == null)
                {
                    msj = "No existe el paquete.";
                }
                else
                {

                    string email = temp.Email;
                    var tempUser = _context.Users.FirstOrDefault(r => r.email.StartsWith(email));
                    // Si se encuentra el paquete, se elimina de la base de datos.
                    _context.Customers.Remove(temp);

                    // Se aplican los cambios para reflejar la eliminación en la base de datos.
                    _context.SaveChanges();

                    _context.Users.Remove(tempUser);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error durante el proceso, se captura la excepción y se informa al usuario.
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            // Se retorna el mensaje final, ya sea de éxito o error.
            return msj;
        }


        // Método 5:encargado de modificar los datos de un cliente.
        [HttpPut("Update")]
        public string Update(Customer temp)
        {
            // Mensaje inicial que indica que se está intentando actualizar un cliente.
            string msj = "Actualizando cliente...";

            try
            {
                // Se busca el cliente en la base de datos usando el CustomerId.
                var objCustomer = _context.Customers.FirstOrDefault(r => r.CustomerId == temp.CustomerId);

                // Si no se encuentra el cliente, se informa al usuario que no existe.
                if (objCustomer == null)
                {
                    msj = "No existe el cliente.";
                }
                else
                {
                    // Se actualizan los datos del cliente con la nueva información proporcionada en 'temp'.
                    objCustomer.IDType = temp.IDType;
                    objCustomer.Name = temp.Name;
                    objCustomer.LastName = temp.LastName;
                    objCustomer.SecondLastName = temp.SecondLastName;
                    objCustomer.Phone = temp.Phone;
                    objCustomer.Address = temp.Address;
                    objCustomer.Email = temp.Email;

                    // Se marca el cliente como actualizado en el contexto de la base de datos.
                    _context.Customers.Update(objCustomer);

                    // Se aplican los cambios en la base de datos.
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // En caso de que ocurra un error durante el proceso, se captura la excepción y se informa al usuario.
                msj = ex.InnerException?.Message ?? ex.Message;
            }

            // Se retorna el mensaje final, ya sea de éxito o error.
            return msj;
        }

        


    }
}
