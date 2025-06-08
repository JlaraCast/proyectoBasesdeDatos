using APIHotelBeachProyecto.Model;
using AppWebHotelBeach.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace APIHotelBeachProyecto
{ 
    public class CardsController : Controller
    {
        private ApiHotelBeach client = null;
        private HttpClient api = null;

        public CardsController()
        {

            client = new ApiHotelBeach();
            api = client.IniciarApi();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete los campos de la tarjeta.";
                return View(model);
            }

            try
            {
                api.DefaultRequestHeaders.Authorization = AutorizacionToken();

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await api.PostAsync("Cards/Save", content);


                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo guardar la tarjeta.";
                    return View(model);
                }

                // Recuperar la reservación pendiente desde TempData
                if (TempData["ReservaPendiente"] is not string reservaJson)
                {
                    TempData["Error"] = "No se encontró la información de la reservación.";
                    return RedirectToAction("Create", "Reservations");
                }

                var reservaModel = JsonConvert.DeserializeObject<WebReservation>(reservaJson);

                var reservaData = new
                {
                    CustomerId = reservaModel.CustomerId,
                    PackageId = reservaModel.PackageId,
                    ReservationDate = reservaModel.ReservationDate,
                    NumberOfPeople = reservaModel.NumberOfPeople,
                    NumberOfNights = reservaModel.NumberOfNights,
                    PaymentMethod = reservaModel.PaymentMethod
                };

                var reservaContent = new StringContent(JsonConvert.SerializeObject(reservaData), Encoding.UTF8, "application/json");
                var reservaResponse = await api.PostAsync("Reservations/CreateReserva", reservaContent);

                if (reservaResponse.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tarjeta y reservación guardadas correctamente.";
                    return RedirectToAction("List", "Reservations");
                }
                else
                {
                    var msg = await reservaResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al guardar reservación: {msg}";
                    return RedirectToAction("Create", "Reservations");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error interno: {ex.Message}";
                return View(model);
            }

        } 

        private AuthenticationHeaderValue AutorizacionToken()
        {
            
            var token = HttpContext.Session.GetString("JWToken");
            return new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

