using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class Role
    {
        [Key]
        public int idRol { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string descripcion { get; set; }
    }
}
