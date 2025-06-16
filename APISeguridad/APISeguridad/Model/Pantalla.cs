using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Pantalla
    {
        [Key]
        [Required]
        public int idPantalla { get; set; }

        [Required]
        public int idSistema { get; set; } 

        [Required] 
        public string nombre { get; set; } = string.Empty; 

        [Required]
        public string descripcion { get; set; } = string.Empty; 

        [Required]
        public string ruta { get; set; } = string.Empty; 

    }
}
