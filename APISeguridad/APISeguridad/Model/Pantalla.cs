using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class Pantalla
    {
        [Key] [Required]
        public int idPantalla { get; set; }

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
