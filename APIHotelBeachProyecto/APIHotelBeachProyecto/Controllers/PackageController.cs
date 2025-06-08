using APIHotelBeachProyecto.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace APIHotelBeachProyecto.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    // Define una clase llamada "PackageController" que hereda de "Controller",
    
    public class PackageController : Controller
    {

        //Declara una variable de tipo DbContext
        private readonly DbContextHotel _context = null;

        //Constructor con parámetros 
        //recibe la instacia del ORM para interactuar con la base datos
        public PackageController(DbContextHotel pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }



        //Metodos

        //Metodo 1:Acción HTTP GET que permite obtener una lista de todos los paquetes registrados en la base de datos.
        // Se accede a este método mediante la ruta: /[controller]/List
        [HttpGet("List")]
        public List<Package> List()
        {
            // Variable local de tipo lista que almacenará los paquetes obtenidos de la base de datos.
            // Se utiliza _context para acceder a la tabla Packages y convertir el resultado en una lista.
            List<Package> temp = _context.Packages.ToList();

            // Se retorna la lista de paquetes al cliente que realizó la solicitud HTTP.
            return temp;
        }


        // Método 2: Acción HTTP GET para buscar un paquete por su identificador (ISBN).



        [HttpGet("SearchName")]
        public List<Package> SearchName([FromQuery] string pName = "")
        {
            // Función para normalizar texto quitando tildes y pasando a minúsculas
            string NormalizeText(string text)
            {
                if (string.IsNullOrEmpty(text)) return text;

                var normalized = text.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();

                foreach (var c in normalized)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                        sb.Append(c);
                }

                return sb.ToString().ToLowerInvariant();
            }

            if (string.IsNullOrWhiteSpace(pName))
            {
                // Si no se proporciona nombre, devolver todos los paquetes
                return _context.Packages.ToList();
            }

            var normalizedSearch = NormalizeText(pName);

            var temp = _context.Packages
                .AsEnumerable()  // Traer a memoria para usar la función NormalizeText
                .Where(x => NormalizeText(x.Name).StartsWith(normalizedSearch))
                .ToList();

            return temp;
        }





        [HttpGet("SearchID")]
        public ActionResult<Package> SearchID(int id)
        {
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        } 


        // Método 3: Acción HTTP POST encargada de guardar un nuevo cliente en la base de datos.
        [HttpPost("Save")]
        public string Save(Package temp)
        {
            // Mensaje inicial que se devolverá al usuario.
            string msj = "Cliente guardado correctamente.";

            try
            {
                // Se agrega el cliente recibido (temp) al contexto de base de datos.
                _context.Packages.Add(temp);

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
                var temp = _context.Packages.FirstOrDefault(r => r.PackageId == pPackageId);

                // Si no se encuentra el paquete, se informa al usuario que no existe.
                if (temp == null)
                {
                    msj = "No existe el paquete.";
                }
                else
                {
                    // Si se encuentra el paquete, se elimina de la base de datos.
                    _context.Packages.Remove(temp);

                    // Se aplican los cambios para reflejar la eliminación en la base de datos.
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



        // Método 5: encargado de modificar los datos de un paquete.
        [HttpPut("Update")]
        public string Update(Package temp)
        {
            // Mensaje inicial que indica que se está intentando actualizar un paquete.
            string msj = "Actualizando paquete...";

            try
            {
                // Se busca el paquete en la base de datos usando el PackageId.
                var objPackage = _context.Packages.FirstOrDefault(r => r.PackageId == temp.PackageId);

                // Si no se encuentra el paquete, se informa al usuario que no existe.
                if (objPackage == null)
                {
                    msj = "No existe el paquete.";
                }
                else
                {
                    // Se actualizan los datos del paquete con la nueva información proporcionada en 'temp'.
                    objPackage.Name = temp.Name;
                    objPackage.Description = temp.Description;
                    objPackage.CostPerPersonPerNight = temp.CostPerPersonPerNight;
                    objPackage.PremiumPercentage = temp.PremiumPercentage;
                    objPackage.TermMonths = temp.TermMonths;

                    // Se marca el paquete como actualizado en el contexto de la base de datos.
                    _context.Packages.Update(objPackage);

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
