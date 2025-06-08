using System.ComponentModel.DataAnnotations;

namespace AppWebHotelBeach.Models
{
    // Clase que representa un paquete de hospedaje o servicio en el hotel.
    public class Package
    {
        // Identificador único del paquete (clave primaria).
        [Key]
        public int PackageId { get; set; }

        // Nombre del paquete (por ejemplo, "Todo incluido", "Familiar", etc.).
        public string Name { get; set; }

        // Descripción del paquete (detalle de servicios, condiciones, etc.).
        public string Description { get; set; }

        // Costo por persona por noche en colones.
        public decimal CostPerPersonPerNight { get; set; }

        // Porcentaje adicional que se cobra como prima (premium).
        public double PremiumPercentage { get; set; }

        // Duración del paquete en meses (por ejemplo, duración del contrato o suscripción).
        public int TermMonths { get; set; }

        // Método para calcular el valor de la prima en colones, basado en un monto total dado.
        public decimal CalculatePremium(decimal TotalAmount)
        {
            return TotalAmount * (decimal)(PremiumPercentage / 100.0);
        }

        // Método auxiliar para calcular el monto total del paquete
        // basado en la cantidad de personas y la cantidad de noc
        public decimal CalculateTotalAmount(int QuantityPersons, int QuantityNights)
        {
            return CostPerPersonPerNight * QuantityPersons * QuantityNights;
        }
    }
}
