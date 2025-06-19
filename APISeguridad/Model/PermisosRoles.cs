using System.ComponentModel.DataAnnotations;

namespace APISeguridad.Model
{
    public class PermisosRoles
    {
        [Key]
        public int idRol { get; set; }
        public int idSistema { get; set; }
        public int IdPantalla { get; set; }
        public int PermisoInsertar { get; set; }
        public int PermisoModificar { get; set; }
        public int PermisoBorrar { get; set; }
        public int PermisoConsultar { get; set; }
    }
}
