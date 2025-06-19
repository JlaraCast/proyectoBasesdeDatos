using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Pantalla
    {
        public int idPantalla { get; set; }
        [ForeignKey("Sistema")]
        [Required]
        public int idSistema { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public string ruta { get; set; }
    }
}
