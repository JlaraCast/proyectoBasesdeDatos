using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
