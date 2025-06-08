using System.ComponentModel.DataAnnotations;

namespace APIHotelBeachProyecto.Model
{
    public class User
    {
        [Key]
        public string email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public char Status { get; set; }
        public char Roll { get; set; }
    }
}
