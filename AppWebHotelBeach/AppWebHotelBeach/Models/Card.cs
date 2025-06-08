using Microsoft.EntityFrameworkCore;

namespace APIHotelBeachProyecto.Model
{
    [PrimaryKey(nameof(CardNumber), nameof(PayId))]

    public class Card
    {
        public int CardNumber { get; set; }
        public string CardOwner { get; set; } //identificador
        public string CardBank { get; set; }

        public int CustomerId { get; set; }

        public int PayId { get; set; } //identificador ya que un numero de tarjeta puede ser usado muchas veces
    }
}
