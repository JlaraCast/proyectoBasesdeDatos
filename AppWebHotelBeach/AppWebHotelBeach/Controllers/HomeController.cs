using System.Diagnostics;
using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppWebHotelBeach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("C"))
                {
                    return RedirectToAction("ClientDashboard", "Users");
                }
                else if (User.IsInRole("A"))
                {
                    return RedirectToAction("AdminDashboard", "Users");
                }
                
            }

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
