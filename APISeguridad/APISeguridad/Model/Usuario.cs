using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class Usuario
    {
        [Key] [Required]
        public int idUsuario { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string correo { get; set; }

        [Required]
        public string clave { get; set; }

        [Required]
        public string estado { get; set; }
    }
}
