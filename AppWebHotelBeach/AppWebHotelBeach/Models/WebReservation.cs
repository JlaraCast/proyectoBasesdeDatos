

using System.ComponentModel.DataAnnotations;

namespace AppWebHotelBeach.Models
{
    public class WebReservation
    {

        public int ReservationId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un paquete.")]
        public int PackageId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfPeople { get; set; }
        public int NumberOfNights { get; set; }
        public string PaymentMethod { get; set; }

    }
}
