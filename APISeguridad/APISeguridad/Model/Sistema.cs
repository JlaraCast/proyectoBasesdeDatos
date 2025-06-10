
using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class Sistema
    {
        
        [Key]
        public int idSistema { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string descripcion { get; set; }
    }
}

