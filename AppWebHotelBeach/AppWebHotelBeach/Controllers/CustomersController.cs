using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AppWebHotelBeach.Controllers
{
    public class CustomersController : Controller
    {
        //Vartiable para manejar la referencia para la web api
        private ApiHotelBeach _client = null;

        //Variable para usar cada metodo publicado en la api
        private HttpClient _api = null;

        public CustomersController()
        {
            _client = new ApiHotelBeach();
            //se crea la instancia cliente para iniciar la APi
            _api = _client.IniciarApi();
        }

        public IActionResult Customers()
        {
            return View("Customers");
        }


        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {

            //variable tipo lista para almacenar la informacion de todos los libros
            List<Customer> listado = new List<Customer>();

            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            //Se utiliza el metodo list parea leer toda la informacion
            HttpResponseMessage response = await _api.GetAsync("Customer/List");

            //Se verifica si la respuesta es correcta
            if (response.IsSuccessStatusCode)
            {
                //se lee todos los datos en formato JSON
                var result = response.Content.ReadAsStringAsync().Result;

                //Se toma la informacion y se convierte en una lista de objetos
                listado = JsonConvert.DeserializeObject<List<Customer>>(result);
            }

            //se envia el listado al front-end para mostrar al usuario
            return View(listado);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            // Paso 1: Traer todos los clientes existentes desde la API
            HttpResponseMessage responseList = await _api.GetAsync("Customer/List");

            if (responseList.IsSuccessStatusCode)
            {
                var json = await responseList.Content.ReadAsStringAsync();
                var clientesExistentes = JsonConvert.DeserializeObject<List<Customer>>(json);

                // Paso 2: Verificar si ya existe un cliente con la misma cédula o email
                bool cedulaExistente = clientesExistentes.Any(c => c.CustomerId == customer.CustomerId);
                bool emailExistente = clientesExistentes.Any(c => c.Email == customer.Email);

                if (cedulaExistente)
                {
                    ModelState.AddModelError("Identification", "Ya existe un cliente con esta cédula.");
                }

                if (emailExistente)
                {
                    ModelState.AddModelError("Email", "Ya existe un cliente con este correo electrónico.");
                }

                // Si ya existe cédula o correo, se muestra el formulario con errores
                if (cedulaExistente || emailExistente)
                {
                    return View(customer);
                }
            }

            // Paso 3: Crear el cliente si no hay duplicados
            var jsonContent = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _api.PostAsync("Customer/Save", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Index","Home");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al guardar: {error}");
                return View(customer);
            }
        }


        [HttpGet]
        public async Task<IActionResult> BuscarPorCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
                return Json(null);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://apis.gometa.org/cedulas/{cedula}");
                    if (!response.IsSuccessStatusCode)
                        return Json(null);

                    var jsonString = await response.Content.ReadAsStringAsync();

                    dynamic gometaData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

                    // Verificamos que "results" exista y tenga al menos un elemento
                    if (gometaData == null || gometaData.results == null || gometaData.results.Count == 0)
                        return Json(null);

                    var firstResult = gometaData.results[0];

                    return Json(new
                    {
                        name = (string)firstResult.firstname1 ?? "",
                        lastName = (string)firstResult.lastname1 ?? "",
                        secondLastName = (string)firstResult.lastname2 ?? "",
                        phone = "",    // No viene en la respuesta
                        address = "",  // No viene en la respuesta
                        email = ""     // No viene en la respuesta
                    });
                }
                catch (Exception ex)
                {
                    // opcional: loguear ex.Message
                    return Json(null);
                }
            }
        }







        [HttpGet]
        public IActionResult SearchByID()
        {
            return View("Search"); // ya que tu vista se llama Search.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> SearchByID(int customerId)
        {
            // Llamada a la API que busca por ID
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await _api.GetAsync($"Customer/SearchID?id={customerId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = customerId });
            }

            ViewBag.Mensaje = $"No se encontró ningún cliente con el ID {customerId}.";
            return View("Search");
        }


        public async Task<IActionResult> Details(int id)
        {
            Customer cliente = null;
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await _api.GetAsync($"Customer/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Customer>(json);
            }
            else
            {
                ViewBag.Mensaje = $"No se encontró ningún cliente con el ID {id}";
                return View("Details", null);
            }

            return View("Details", cliente);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Customer cliente = null;
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await _api.GetAsync($"Customer/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                cliente = JsonConvert.DeserializeObject<Customer>(json);
                return View(cliente); // Asume que tienes Edit.cshtml
            }

            return NotFound(); // o redirige a una vista de error
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer cliente)
        {

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _api.PutAsync($"Customer/Update", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List"); // O a donde quieras volver
            }

            ViewBag.Mensaje = "Error al actualizar el cliente.";
            return View(cliente);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await _api.GetAsync($"Customer/SearchID?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Customer cliente = JsonConvert.DeserializeObject<Customer>(json);
                return View(cliente); // ← Cliente se pasa a la vista correctamente
            }

            return NotFound($"No se encontró ningún cliente con el ID {id}.");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")] // Esto hace que siga respondiendo a /Delete
        public async Task<IActionResult> DeleteConfirmed(int CustomerId)
        {
            _api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await _api.DeleteAsync($"Customer/Delete?pPackageId={CustomerId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }

            HttpResponseMessage getResponse = await _api.GetAsync($"Customer/SearchID?id={CustomerId}");

            if (getResponse.IsSuccessStatusCode)
            {
                var json = await getResponse.Content.ReadAsStringAsync();
                Customer cliente = JsonConvert.DeserializeObject<Customer>(json);

                ViewBag.Mensaje = "Ocurrió un error al eliminar el cliente.";
                return View("Delete", cliente);
            }

            return NotFound("Error al eliminar y no se pudo volver a cargar el cliente.");
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {

            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
                return new AuthenticationHeaderValue("Bearer", token);

            return null;
        }

    }//fin class
}//fin namespace