using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppWebHotelBeach.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ApiHotelBeach client;
        private readonly HttpClient api;

        public PackagesController()
        {
            client = new ApiHotelBeach();
            api = client.IniciarApi();
        }

        [HttpGet]
        public IActionResult Packages()
        {
            return View("Packages");
        }

        [HttpGet]
        public async Task<IActionResult> List(int? PackageId)
        {
            List<Package> listado = new List<Package>();

            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await api.GetAsync("Package/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Package>>(result);
            }

            if (PackageId.HasValue)
            {
                // Filtramos
                var filtrado = listado.FindAll(r => r.PackageId == PackageId.Value);

                if (filtrado.Count > 0)
                {
                    // Si hay resultados, mostrar solo los filtrados
                    listado = filtrado;
                }
                else
                {
                    // No se encontraron paquetes con ese ID
                    ViewBag.Mensaje = $"No se encontró ningún paquete con el ID {PackageId.Value}.";
                    
                }
            }

            return View(listado);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Package pPackage)
        {

            
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var response = await api.PostAsJsonAsync("Package/Save", pPackage);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            TempData["Error"] = "Error, no se logró almacenar el Package";
            return View(pPackage);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await api.GetAsync($"Package/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Package package = JsonConvert.DeserializeObject<Package>(json);
                return View(package); //  Cliente se pasa a la vista correctamente
            }

            return NotFound($"No se encontró ningún cliente con el ID {id}.");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Package package)
        {
            int id = package.PackageId;

            if (id == 0)
            {
                TempData["Error"] = "ID no válido recibido (0).";
                return RedirectToAction("List");
            }

            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await api.DeleteAsync($"Package/Delete?pPackageId={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            TempData["Error"] = "No se pudo eliminar el paquete.";
            return RedirectToAction("List");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Package package = null;

            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await api.GetAsync($"Package/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                package = JsonConvert.DeserializeObject<Package>(result);
            }

            return View(package);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind] Package temp)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();


            HttpResponseMessage response = await api.PutAsJsonAsync("Package/Update", temp);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            TempData["Error"] = "Error al actualizar el paquete.";
            return View(temp);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Package package = null;

            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await api.GetAsync($"Package/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                package = JsonConvert.DeserializeObject<Package>(result);
            }

            return View(package);
        }

        [HttpGet]
        public async Task<IActionResult> PackagesList(string returnUrl)
        {
            List<Package> listado = new List<Package>();

            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            HttpResponseMessage response = await api.GetAsync("Package/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Package>>(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(listado);
        }



        [HttpGet]
        public async Task<JsonResult> GetAvailablePackages()
        {
            try
            {
                // Configurar autenticación
                api.DefaultRequestHeaders.Authorization = AutorizacionToken();

                // Agregar timeout para evitar esperas infinitas
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(30)); // 30 segundos de timeout

                // Hacer la petición a la API
                var response = await api.GetAsync("Package/List", cts.Token);

                // Verificar si la respuesta fue exitosa
                if (!response.IsSuccessStatusCode)
                {
                    // Log del error para diagnóstico
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al obtener paquetes. Código: {response.StatusCode}, Contenido: {errorContent}");

                    return Json(new
                    {
                        success = false,
                        message = response.StatusCode == HttpStatusCode.NotFound
                            ? "No se encontraron paquetes disponibles"
                            : $"Error en el servidor: {response.ReasonPhrase}"
                    });
                }

                // Procesar respuesta exitosa
                var jsonString = await response.Content.ReadAsStringAsync();
                var packages = JsonConvert.DeserializeObject<List<Package>>(jsonString);

                // Validar que hay paquetes
                if (packages == null || !packages.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No hay paquetes disponibles en este momento"
                    });
                }

                return Json(new
                {
                    success = true,
                    packages = packages.Select(p => new
                    {
                        packageId = p.PackageId,
                        name = p.Name,
                        description = p.Description,
                        costPerPersonPerNight = p.CostPerPersonPerNight
                    }).ToList()
                });
            }
            catch (TaskCanceledException)
            {
                return Json(new
                {
                    success = false,
                    message = "Tiempo de espera agotado al cargar los paquetes"
                });
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                Console.WriteLine($"Excepción al obtener paquetes: {ex}");

                return Json(new
                {
                    success = false,
                    message = $"Error interno: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name)
        {
            List<Package> listado = new List<Package>();
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            // Enviar el parámetro aunque esté vacío
            var response = await api.GetAsync($"Package/SearchName?pName={Uri.EscapeDataString(name ?? "")}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Package>>(json);

                if (string.IsNullOrWhiteSpace(name))
                {
                    ViewBag.Message = "Mostrando todos los paquetes.";
                }
                else if (listado.Count == 0)
                {
                    ViewBag.Message = $"No se encontraron paquetes con el nombre '{name}'.";
                }
            }
            else
            {
                ViewBag.Message = $"Error al consultar los paquetes. Código: {response.StatusCode}";
            }

            return View("List", listado);
        }










        // Método para obtener el token desde la sesión
        private AuthenticationHeaderValue AutorizacionToken()
        {

            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
                return new AuthenticationHeaderValue("Bearer", token);

            return null;
        }
    }
}


