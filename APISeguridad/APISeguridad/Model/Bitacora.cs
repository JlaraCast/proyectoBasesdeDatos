using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class Bitacora
    {
        [Key]
        [Required]
        public int idBitacora { get; set; }

        [Required]
        public int idUsuario { get; set; }

        [Required]
        public DateTime fecha { get; set; }

        [Required]
        public string accion { get; set; }

        [Required]
        public string detalle { get; set; }
    }
}
