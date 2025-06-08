using System.ComponentModel.DataAnnotations;

namespace APIHotelBeachProyecto.Model
{
    public class WebReservation
    {

        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int PackageId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfPeople { get; set; }
        public int NumberOfNights { get; set; }
        public string PaymentMethod { get; set; }

    }
}
