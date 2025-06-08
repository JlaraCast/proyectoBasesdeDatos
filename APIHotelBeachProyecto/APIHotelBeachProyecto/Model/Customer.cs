using System.ComponentModel.DataAnnotations;

namespace APIHotelBeachProyecto.Model
{
    // Clase que representa a un cliente del hotel.
    public class Customer
    {
        // Cédula del cliente, funciona como identificador único (clave primaria).
        [Key]
        public int CustomerId { get; set; } // Customer ID

        // Type of ID (e.g., physical or legal).
        public string IDType { get; set; }

        // Customer's first name.
        public string Name { get; set; }

        // Customer's first surname.
        public string LastName { get; set; }

        // Customer's second surname.
        public string SecondLastName { get; set; }

        // Customer's phone number.
        public string Phone { get; set; }

        // Customer's physical address.
        public string Address { get; set; }

        // Customer's email address.
        public string Email { get; set; }

        public string Password { get; set; }

    }
}
