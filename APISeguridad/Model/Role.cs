using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APISeguridad.Model
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idRol { get; set; }

        [Required]
        public int idSistema { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string descripcion { get; set; }
    }
}
