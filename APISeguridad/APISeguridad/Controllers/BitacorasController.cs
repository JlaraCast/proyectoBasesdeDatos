using APISeguridad.Model;
using Microsoft.AspNetCore.Mvc;

namespace APISeguridad.Controllers
{
    [ApiController]//permite al controlador interactuar como una Web API
    [Route("[controller]")] //Permite interpretar rutas para los métodos
    public class BitacorasController : ControllerBase
    {
        //Declara una variable de tipo DbContext
        private readonly DbContextSeguridad _context = null;

        public BitacorasController(DbContextSeguridad pContext)
        {
            _context = pContext; //pContext maneja la info del Servidor DB
        }

        // Método 1: Acción HTTP GET para listar todos las bitaxcoras
        // La ruta para acceder a este método sería: /[controller]/List
        [HttpGet("List")]
        public List<Bitacora> List()
        {
            // Variable local de tipo lista que almacenará las bitacoras recuperados de la base de datos.
            List<Bitacora> temp = _context.bitacoras.ToList();

            // Se retorna la lista de bitacoras
            return temp;
        }

        //Metodo 2: Busca bitacoras por el id del usuario
        [HttpGet("SearchByUserId")]
        public ActionResult<List<Bitacora>> SearchByUserId(int idUsuario)
        {
            //devuelve una lista de bitacoras 
            var bitacoras = _context.bitacoras
                .Where(r => r.idUsuario == idUsuario)
                .ToList();

            if (bitacoras == null || bitacoras.Count == 0)
            {
                return NotFound($"No bitacoras found for user ID {idUsuario}.");
            }

            return Ok(bitacoras); // ASP.NET Core lo convierte a JSON automáticamente
        }

        // Método 3: Busca bitacoras por fecha y acción
        [HttpGet("SearchByDateAndAction")]
        public ActionResult<List<Bitacora>> SearchByDateAndAction(DateTime fecha, string accion)
        {
            // Filtra las bitacoras por fecha exacta y acción
            var bitacoras = _context.bitacoras
                .Where(r => r.fecha.Date == fecha.Date && r.accion == accion)
                .ToList();

            if (bitacoras == null || bitacoras.Count == 0)
            {
                return NotFound($"No bitacoras found for date {fecha.ToShortDateString()} and action '{accion}'.");
            }

            return Ok(bitacoras);
        }


    }
}
