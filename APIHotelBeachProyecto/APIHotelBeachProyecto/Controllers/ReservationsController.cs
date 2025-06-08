using APIHotelBeachProyecto.Model;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIHotelBeachProyecto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly DbContextHotel _context = null;
        private readonly ExchangeRate _exchangeRate;
        private readonly InvoiceService _invoiceService;
        decimal premiumAmount = 0;

        public ReservationsController(DbContextHotel pContext, ExchangeRate pExchangeRate, InvoiceService invoiceService)
        {
            _context = pContext;
            _exchangeRate = pExchangeRate;
            _invoiceService = invoiceService;
        }

        //Metodo 1: Muestra una lista de las reservaciones existentes
        [HttpGet("List")]
        public List<Reservation> List()
        {

            List<Reservation> temp = _context.Reservations.ToList();
            return temp;

        }

        // Método 2: Crea y guarda una reservación completa en la base de datos.
        // Este método se llama solo cuando el usuario ya confirmó su compra en la aplicación web.
        // Si el usuario eligió pagar con cheque, también se extraen sus datos para usarlos luego en la factura.
        [HttpPost("CreateReserva")]
        public async Task<string> CreateReservaAsync(WebReservation temp)
        {
            //Para reservacion y factura
            string msg = "Save Reservacion...";
            decimal IVA = 0;
            decimal totalColones = 0;
            decimal totalDollars = 0;
            decimal exchangeRate = 0;
            decimal discountRate = 0;
            bool discountApplied = false;
            decimal downPaymentRate = 0;
            decimal monthlyPayment = 0;
            decimal downPaymentAmount = 0;

            

            try
            {
                // 1. Obtener el tipo de cambio actual (para convertir dolares que es en que vienen los precios a colones)
                exchangeRate = await _exchangeRate.GetExchangeRateAsync();

                // 2. Obtener los datos del paquete desde la base de datos
                var package = _context.Packages.FirstOrDefault(p => p.PackageId == temp.PackageId);
                if (package == null)
                    return "Package not found.";

                // 3. Convertir el precio por persona (en dólares) a colones
                decimal pricePerPersonColones = package.CostPerPersonPerNight * exchangeRate;

                // 4. Calcular el subtotal (sin descuento ni IVA)
                totalColones = pricePerPersonColones * temp.NumberOfNights * temp.NumberOfPeople;

                // 5. Aplicar descuento si la forma de pago es efectivo y según la cantidad de noches
                if (temp.PaymentMethod == "Efectivo")
                {
                    if (temp.NumberOfNights >= 3 && temp.NumberOfNights <= 6)
                        discountRate = 0.10m;
                    else if (temp.NumberOfNights >= 7 && temp.NumberOfNights <= 9)
                        discountRate = 0.15m;
                    else if (temp.NumberOfNights >= 10 && temp.NumberOfNights <= 12)
                        discountRate = 0.20m;
                    else if (temp.NumberOfNights > 13)
                        discountRate = 0.25m;

                    // Aplicar el descuento
                    totalColones -= (totalColones * discountRate);
                    discountApplied =true; // es como decir true
                }
                
                // 7. Calcular el IVA (13%) y sumarlo al total
                IVA = totalColones * 0.13m;
                totalColones += IVA;

                // 8. Calcular el total en dólares para mostrar y guardar también
                totalDollars = totalColones / exchangeRate;

                // 9. Determinar la prima según el paquete seleccionado
                downPaymentRate = temp.PackageId switch
                {
                    1 => 0.45m, // Todo incluido
                    2 => 0.35m, // Alimentación
                    3 => 0.15m, // Hospedaje
                    _ => 1m
                };

                // 10. Calcular el monto de la prima a pagar al momento
                downPaymentAmount = totalColones * downPaymentRate;

                // 11. Calcular la cantidad de cuotas para el resto del monto
                int installments = temp.PackageId switch
                {
                    1 => 24,
                    2 => 18,
                    3 => 12,
                    _ => 1
                };

                // 12. Calcular el monto mensual a pagar después de la prima
                monthlyPayment = (totalColones - downPaymentAmount) / installments;

                // 13. Crear el objeto Reservacion ya con todos los valores calculados
                var reservation = new Reservation
                {
                    CustomerId = temp.CustomerId,
                    PackageId = temp.PackageId,
                    ReservationDate = temp.ReservationDate,
                    NumberOfPeople = temp.NumberOfPeople,
                    NumberOfNights = temp.NumberOfNights,
                    PaymentMethod = temp.PaymentMethod,
                    DiscountApplied = discountApplied,
                    IVA = IVA,
                    TotalColones = totalColones,
                    TotalDollars = totalDollars,
                    ExchangeRate = exchangeRate.ToString()
                };


                // 14. Agregar la reservación a la base de datos
                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                // 15. Crea el objeto factura
                var invoice = new Invoice
                {
                    //La factura va con el mismo id de factura, pero igual se le ingresa un id de reservacion
                    //InvoiceId = reservation.ReservationId, //usa el id real
                    ReservationId = reservation.ReservationId,
                    CustomerId = temp.CustomerId,
                    PaymentMethod = temp.PaymentMethod,
                    IssueDate = DateTime.Now


                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(); // Aquí se genera el InvoiceId

                //envio de factura
                //Llamada al metodo 
                //_invoiceService.GenerateAndSendInvoice(invoice.InvoiceId);
                var result = _invoiceService.GenerateAndSendInvoice(invoice.InvoiceId);
                if (result.StartsWith("Error"))
                {
                    return result; // <-- para ver si falló al enviar
                }


                // Insertas manualmente en CustomersInvoices
                var customerInvoice = new CustomerInvoice
                {
                    CustomerId = invoice.CustomerId,
                    InvoiceID = invoice.InvoiceId
                };

                _context.CustomersInvoices.Add(customerInvoice);
                await _context.SaveChangesAsync(); // Guardas la relación



            }
            catch (Exception ex)
            {
                msg = $"ERROR: {ex.Message}\nSTACK: {ex.StackTrace}";

                if (ex.InnerException != null)
                    msg += $"\nINNER: {ex.InnerException.Message}";
            }
            return msg;
        }

        //Metodo 3: Edita a una reservacion, re calcula los valores que debe tener y meterlos al objeto
        [HttpPut("Edit")]
        public async Task<string> Edit(Reservation temp)
        {
            string msg = "Reservation updated";

            try
            {
                var existingReservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == temp.ReservationId);

                if (existingReservation == null)
                {
                    msg = "Reservation not found.";
                }
                else
                {
                    // 1. Obtener tipo de cambio actual
                    decimal exchangeRate = await _exchangeRate.GetExchangeRateAsync();

                    // 2. Obtener paquete seleccionado
                    var package = _context.Packages.FirstOrDefault(p => p.PackageId == temp.PackageId);
                    if (package == null)
                        return "Package not found.";

                    // 3. Convertir el precio del paquete de dólares a colones
                    decimal pricePerPersonColones = package.CostPerPersonPerNight * exchangeRate;

                    // 4. Calcular subtotal base
                    decimal subtotalColones = pricePerPersonColones * temp.NumberOfNights * temp.NumberOfPeople;

                    // 5. Aplicar descuento si pago en efectivo
                    decimal discountRate = 0;
                    bool discountApplied = false;

                    if (temp.PaymentMethod.ToLower() == "efectivo")
                    {
                        if (temp.NumberOfNights >= 3 && temp.NumberOfNights <= 6)
                            discountRate = 0.10m;
                        else if (temp.NumberOfNights >= 7 && temp.NumberOfNights <= 9)
                            discountRate = 0.15m;
                        else if (temp.NumberOfNights >= 10 && temp.NumberOfNights <= 12)
                            discountRate = 0.20m;
                        else if (temp.NumberOfNights > 13)
                            discountRate = 0.25m;

                        subtotalColones -= (subtotalColones * discountRate);
                        discountApplied =true;
                    }

                    // 6. Calcular IVA y total
                    decimal vat = subtotalColones * 0.13m;
                    decimal totalColones = subtotalColones + vat;
                    decimal totalDollars = totalColones / exchangeRate;

                    // 7. Actualizar todos los campos
                    existingReservation.CustomerId = temp.CustomerId;
                    existingReservation.PackageId = temp.PackageId;
                    existingReservation.ReservationDate = temp.ReservationDate;
                    existingReservation.NumberOfPeople = temp.NumberOfPeople;
                    existingReservation.NumberOfNights = temp.NumberOfNights;
                    existingReservation.PaymentMethod = temp.PaymentMethod;

                    existingReservation.DiscountApplied = discountApplied;
                    existingReservation.IVA = vat;
                    existingReservation.TotalColones = totalColones;
                    existingReservation.TotalDollars = totalDollars;
                    existingReservation.ExchangeRate = exchangeRate.ToString();

                    // 8. Guardar cambios
                    _context.Reservations.Update(existingReservation);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException.Message;
            }

            return msg;
        }

        //Metodo 4: Elimina una reservacion
        [HttpDelete("Delete")]
        public string Delete(int reservationId)
        {
            string msg = "Delete reservation...";

            try
            {
                var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);

                if (reservation == null)
                {
                    msg = "Reservation not found.";
                }
                else
                {
                    _context.Reservations.Remove(reservation);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                msg = ex.InnerException?.Message ?? ex.Message;
            }

            return msg;
        }

        //Metodo 5: Busca una reservacion por el id cliente 

        [HttpGet("SearchByCustomerId")]
        public ActionResult<List<Reservation>> SearchByCustomerId(int customerId)
        {
            var reservations = _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .ToList();

            if (reservations == null || reservations.Count == 0)
            {
                return NotFound($"No reservations found for customer ID {customerId}.");
            }

            return Ok(reservations); // ASP.NET Core lo convierte a JSON automáticamente
        }

        [HttpGet("GetReservationById")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);

                if (reservation == null)
                {
                    return NotFound(new { message = "Reservación no encontrada." });
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al buscar la reservación.", error = ex.Message });
            }
        }



        //Metodo 6:
        // Method 6:Calcula el anticipo inicial (prima) 
        // sin guardar nada en la base de datos. Se utiliza para informar al usuario 
        // de cuánto pagaría en el momento de hacer la reserva.
        [HttpPost("GetDownPaymentAmount")]
        public async Task<decimal> GetDownPaymentAmount(WebReservation temp)
        {
            // 1. Buscar el paquete según el ID recibido
            var package = _context.Packages.FirstOrDefault(p => p.PackageId == temp.PackageId);
            if (package == null) return 0;

            // 2. Obtener tipo de cambio actual para convertir USD → CRC
            decimal exchangeRate = await _exchangeRate.GetExchangeRateAsync();

            // 3. Calcular el costo por persona por noche en colones
            decimal pricePerPersonColones = package.CostPerPersonPerNight * exchangeRate;

            // 4. Calcular subtotal sin descuento ni impuestos
            decimal subtotal = pricePerPersonColones * temp.NumberOfPeople * temp.NumberOfNights;

            // 5. Calcular descuento según número de noches si paga en efectivo
            decimal discount = 0;
            if (temp.PaymentMethod?.ToLower() == "efectivo")
            {
                if (temp.NumberOfNights >= 3 && temp.NumberOfNights <= 6) discount = 0.10m;
                else if (temp.NumberOfNights >= 7 && temp.NumberOfNights <= 9) discount = 0.15m;
                else if (temp.NumberOfNights >= 10 && temp.NumberOfNights <= 12) discount = 0.20m;
                else if (temp.NumberOfNights > 13) discount = 0.25m;
            }

            // 6. Aplicar descuento si corresponde
            decimal discountAmount = subtotal * discount;
            decimal discountedSubtotal = subtotal - discountAmount;

            // 7. Calcular IVA (13%) y total en colones
            decimal vat = discountedSubtotal * 0.13m;
            decimal totalColones = discountedSubtotal + vat;

            // 8. Determinar el porcentaje de prima según el paquete
            decimal downPaymentPercentage = temp.PackageId switch
            {
                1 => 0.45m,
                2 => 0.35m,
                3 => 0.15m,
                _ => 1m // Por defecto, cobra el 100% si es un paquete no reconocido
            };

            // 9. Calcular el monto de la prima
            decimal downPaymentAmount = totalColones * downPaymentPercentage;

            return downPaymentAmount;
        }







    }
}
