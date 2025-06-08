using System.ComponentModel.DataAnnotations;

namespace AppWebHotelBeach.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public int CustomerId { get; set; }

        public int PackageId { get; set; }

        public DateTime ReservationDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de personas debe ser al menos 1.")]
        public int NumberOfPeople { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de noches debe ser al menos 1.")]
        public int NumberOfNights { get; set; }


        public string PaymentMethod { get; set; }

        public bool  DiscountApplied { get; set; }

        public decimal IVA { get; set; }

        public decimal TotalColones { get; set; }

        public decimal TotalDollars { get; set; }

        public string ExchangeRate { get; set; }

        

    }
}
