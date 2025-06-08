using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AppWebHotelBeach.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiHotelBeach _client;
        private readonly HttpClient _api;

        public UsersController()
        {
            _client = new ApiHotelBeach();
            _api = _client.IniciarApi();
        }

        [HttpPost]  
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Index");
        }


        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Login(User usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.email) || string.IsNullOrEmpty(usuario.Password))
            {
                TempData["ErrorLogin"] = "Por favor, ingrese usuario y contraseña.";
                return View(usuario);
            }

            // Asignar Name para evitar error 400 en la API
            usuario.Name = usuario.email;

            // Llamada a la API para autenticar y obtener token
            var responseAuth = await _api.PostAsJsonAsync("Users/Autenticar", usuario);

            if (!responseAuth.IsSuccessStatusCode)
            {
                var errorContent = await responseAuth.Content.ReadAsStringAsync();
                TempData["ErrorLogin"] = $"Usuario o contraseña incorrectos.";

                return View(usuario);
            }

            var contentAuth = await responseAuth.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AutorizacionResponse>(contentAuth);

            if (authResponse == null || !authResponse.Resultado)
            {
                TempData["ErrorLogin"] = "Error al autenticar usuario.";
                return View(usuario);
            }

            // Guardar token en sesión
            HttpContext.Session.SetString("Token", authResponse.Token);

            // Obtener rol desde la API
            var encodedEmail = Uri.EscapeDataString(usuario.email);
            var responseRol = await _api.GetAsync($"Users/RolPorEmail/{encodedEmail}");

            if (!responseRol.IsSuccessStatusCode)
            {
                TempData["ErrorLogin"] = "No se pudo obtener el rol del usuario.";
                return View(usuario);
            }

            var contentRol = await responseRol.Content.ReadAsStringAsync();
            string rol = contentRol.Trim();

            if (string.IsNullOrEmpty(rol))
            {
                TempData["ErrorLogin"] = "Rol inválido.";
                return View(usuario);
            }


            // Crear claims para la autenticación con rol
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.email),
                new Claim(ClaimTypes.Role, rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirigir según rol
            if (rol == "A")
                return RedirectToAction("AdminDashboard", "Users");
            else if (rol == "C")
                return RedirectToAction("ClientDashboard", "Users");
            else
                return RedirectToAction("Index", "Home");

        }

        public IActionResult ClientDashboard()
        {
            var nombre = User.Identity.Name;
            ViewData["NombreUsuario"] = nombre;

            return View("~/Views/ClientDashboard/Index.cshtml");
        }

        public IActionResult AdminDashboard()
        {
            var nombre = User.Identity.Name;
            ViewData["NombreUsuario"] = nombre;

            return View("~/Views/AdminDashboard/Index.cshtml");
        }
    }
}
