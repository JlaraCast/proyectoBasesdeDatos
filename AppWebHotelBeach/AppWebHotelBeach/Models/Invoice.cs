using System.ComponentModel.DataAnnotations;

namespace AppWebHotelBeach.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public int ReservationId { get; set; }

        public int CustomerId { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime IssueDate { get; set; }

    }
}
