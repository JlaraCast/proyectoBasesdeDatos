using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Bitacora
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idBitacora { get; set; }

        [Required]
        public int idUsuario { get; set; }
        
        [Required]
        public int idSistema { get; set; }
       
        [Required]
        public int idPantalla { get; set; }

        [Required]
        public DateTime fecha { get; set; }

        [Required]
        public string accion { get; set; }

        [Required]
        public string detalle { get; set; }
    }
}
