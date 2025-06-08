using APIHotelBeachProyecto.Model;
using Microsoft.AspNetCore.Mvc;

namespace APIHotelBeachProyecto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : Controller
    {
        //Declara una variable de tipo DbContext
        private readonly DbContextHotel _context = null;

        //Constructor con parámetros 
        //recibe la instacia del ORM para interactuar con la base datos
        public CardsController(DbContextHotel pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }

        [HttpPost("Save")]
        public string Save(Card temp)
        {
            // Mensaje inicial que se devolverá al usuario.
            string msj = "Tarjeta guardada correctamente.";

            try
            {
                // Se agrega el objeto recibido (temp) al contexto de base de datos.
                _context.Cards.Add(temp);

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
    }
}
