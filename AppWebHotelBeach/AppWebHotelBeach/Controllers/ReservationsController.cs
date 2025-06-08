using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AppWebHotelBeach.Controllers
{
    public class ReservationsController : Controller
    {
        private ApiHotelBeach client = null;
        private HttpClient api = null;

        public ReservationsController()
        {

            client = new ApiHotelBeach();
            api = client.IniciarApi();
        }

        public IActionResult Reservations()
        {
            return View("Reservations");
        }


        public async Task<IActionResult> List(int? ReservationId)
        {

           List<Reservation> listado = new List<Reservation>();
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await api.GetAsync("Reservations/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Reservation>>(result);
            }

            // Aplicar el filtro si se proporciona un ReservationId
            if (ReservationId.HasValue)
            {

                listado = listado.FindAll(r => r.ReservationId == ReservationId.Value);
            }

                    return View(listado);
        }


        [HttpGet]
        public IActionResult Create(int? packageId)
        {
            var model = new Reservation();

            if (packageId.HasValue)
                model.PackageId = packageId.Value;

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WebReservation model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor revise los campos del formulario.";
                return View("Create");
            }

            if (model.PackageId == 0)
            {
                TempData["Error"] = "Debe seleccionar un paquete antes de continuar.";
                return View("Create");
            }

            try
            {
                api.DefaultRequestHeaders.Authorization = AutorizacionToken();

                //  Verificar si existe el cliente
                var clienteResponse = await api.GetAsync($"Customer/SearchID?id={model.CustomerId}");
                if (!clienteResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "El cliente con esa Cédula no existe.";
                    return RedirectToAction("Create");
                }

                //  Verificar si ya tiene reserva ese día
                var reservasResponse = await api.GetAsync($"Resevations/SearchByCustomerId{model.CustomerId}");
                List<WebReservation> reservasCliente = new();

                if (reservasResponse.IsSuccessStatusCode)
                {
                    var reservasJson = await reservasResponse.Content.ReadAsStringAsync();
                    reservasCliente = JsonConvert.DeserializeObject<List<WebReservation>>(reservasJson);
                }

                bool yaTieneReserva = reservasCliente.Any(r => r.ReservationDate.Date == model.ReservationDate.Date);
                if (yaTieneReserva)
                {
                    TempData["Error"] = "El cliente ya tiene una reservación para esa fecha.";
                    return View(model);
                }

                //  Redirigir a vista de tarjeta, si es tarjeta, 
                if (model.PaymentMethod == "Tarjeta")
                {
                    TempData["ReservaPendiente"] = JsonConvert.SerializeObject(model);
                    return RedirectToAction("Create", "Cards");

                }

                //  Crear la reservación directamente
                var nuevaReservacion = new
                {
                    CustomerId = model.CustomerId,
                    PackageId = model.PackageId,
                    ReservationDate = model.ReservationDate,
                    NumberOfPeople = model.NumberOfPeople,
                    NumberOfNights = model.NumberOfNights,
                    PaymentMethod = model.PaymentMethod
                };

                var json = JsonConvert.SerializeObject(nuevaReservacion);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await api.PostAsync("Reservations/CreateReserva", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Reservación creada correctamente.";
                    return RedirectToAction("Create");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al crear la reservación: {errorMessage}";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error interno: {ex.Message}";
                return View();
            }
        }




        
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var response = await api.GetAsync($"Reservations/SearchById?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var reservation = JsonConvert.DeserializeObject<Reservation>(json);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var response = await api.DeleteAsync($"Reservations/Delete?reservationId={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la reservación.";
                return RedirectToAction("Delete", new { id });
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage buscar = await api.GetAsync($"Reservations/SearchById?id={id}");

            if (buscar.IsSuccessStatusCode)
            {
                var result = await buscar.Content.ReadAsStringAsync();
                var temp = JsonConvert.DeserializeObject<WebReservation>(result); 
                return View(temp);
            }

            return NotFound(); 
        }




        [HttpPost]
        public async Task<IActionResult> Edit([Bind] WebReservation temp)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage modificar = await api.PutAsJsonAsync<WebReservation>("Reservations/Edit", temp);

            if (modificar.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Error = await modificar.Content.ReadAsStringAsync(); 
                return View(temp);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();

            // Llamada al nuevo endpoint por ReservationId
            HttpResponseMessage buscar = await api.GetAsync($"Reservations/SearchById?id={id}");

            if (buscar.IsSuccessStatusCode)
            {
                var result = await buscar.Content.ReadAsStringAsync();
                var reservation = JsonConvert.DeserializeObject<Reservation>(result);

                return View(reservation);
            }

            TempData["Error"] = "No se pudo obtener la reservación.";
            return RedirectToAction("List"); 
        }



        //Busca reserva por cliente
        public IActionResult SearchByCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchByCustomer(int customerId)
        {
            try
            {
                api.DefaultRequestHeaders.Authorization = AutorizacionToken();
                var response = await api.GetAsync($"Reservation/SearchByCustomerId?customerId={customerId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Message = $"No se encontraron reservaciones para el cliente con ID {customerId}.";
                    return View(new List<WebReservation>());
                }

                var data = await response.Content.ReadFromJsonAsync<List<WebReservation>>();
                return View(data);
            }
            catch (Exception ex)
            {
                
                ViewBag.Message = "Ocurrió un error al buscar las reservaciones. Intenta de nuevo.";
                return View(new List<WebReservation>());
            }
        }



        public async Task<IActionResult> FilterByDate(DateTime? reservationDate)
        {
            List<Reservation> listado = new List<Reservation>();
            api.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await api.GetAsync("Reservations/List");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                listado = JsonConvert.DeserializeObject<List<Reservation>>(result);
            }

            if (reservationDate.HasValue)
            {
                listado = listado.FindAll(r => r.ReservationDate.Date == reservationDate.Value.Date);

                if (listado.Count == 0)
                {
                    ViewBag.Message = $"No se encontraron reservaciones para la fecha {reservationDate.Value:dd/MM/yyyy}.";
                }
            }

            return View("List", listado); 
        }



        private AuthenticationHeaderValue AutorizacionToken()
        {

            var token = HttpContext.Session.GetString("Token");
            AuthenticationHeaderValue authentication = null;

            if (token != null && token.Length != 0)
            {
                authentication = new AuthenticationHeaderValue("Bearer", token);
            }

            return authentication;
        }

    }
}
