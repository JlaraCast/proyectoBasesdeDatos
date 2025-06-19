using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class UsuariosRol
    {
        [Key]
        public int idUsuario { get; set; }
        public int idRol { get; set; }
        
    }
}
