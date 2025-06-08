using Microsoft.AspNetCore.Mvc;

namespace ProyectoVista.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Contacto()
        {
            return View();
        }
    }
}
