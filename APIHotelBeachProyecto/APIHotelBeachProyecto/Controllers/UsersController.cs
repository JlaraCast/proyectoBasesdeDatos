using APIHotelBeach.SA.Services;
using APIHotelBeachProyecto.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIHotelBeachProyecto.Controllers
{
    [ApiController] //Permite al controlador tener un comportamiento Web API
    [Route("[controller]")] //Permite al controlador interpretar rutas
    public class UsersController : ControllerBase
    {

        private readonly DbContextHotel _context;
        private readonly IAutorizacionServices autorizacionbServices;

        public UsersController(DbContextHotel pContext, IAutorizacionServices pAutorizacionbServices)
        {
            _context = pContext;
            autorizacionbServices = pAutorizacionbServices;
        }

        [HttpPost]
        [Route("Autenticar")]
        public async Task<IActionResult> Autenticar(User autenticar)
        {

            var autorizado = await autorizacionbServices.DevolverToken(autenticar);

            if (autorizado == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(autorizado);
            }

        }

        [HttpGet]
        [Route("RolPorEmail/{email}")]
        public async Task<IActionResult> RolPorEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("El email es requerido.");

            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.email == email);

            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            return new ContentResult
            {
                Content = usuario.Roll.ToString(),
                ContentType = "text/plain",
                StatusCode = 200
            };


        }




    }
}
