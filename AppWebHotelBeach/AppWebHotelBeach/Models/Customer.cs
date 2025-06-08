using System.ComponentModel.DataAnnotations;

namespace AppWebHotelBeach.Models
{
    // Clase que representa a un cliente del hotel.
    public class Customer
    {
        // Cédula del cliente, funciona como identificador único (clave primaria).
        [Key]
        [Required]
        public int CustomerId { get; set; } // Customer ID

        // Type of ID (e.g., physical or legal).
        [Required]
        public string IDType { get; set; }

        // Customer's first name.
        [Required]
        public string Name { get; set; }

        // Customer's first surname.
        [Required]
        public string LastName { get; set; }

        // Customer's second surname.
        [Required]
        public string SecondLastName { get; set; }

        // Customer's phone number.
        [Required]
        public string Phone { get; set; }

        // Customer's physical address.
        [Required]
        public string Address { get; set; }

        // Customer's email address.
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
