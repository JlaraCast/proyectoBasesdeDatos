using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace APISeguridad.Model
{
    public class DbContextSeguridad : DbContext
    {
        public DbContextSeguridad(
            DbContextOptions<DbContextSeguridad> options) : base(options)
        { 

        }

        //Los modelos y sus respectivas tablas a trabajar
        public DbSet<Usuario> usuarios { get; set; }

        public DbSet<Pantalla> pantallas { get; set; }

        public DbSet<Bitacora> bitacoras { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Sistema> sistemas { get; set; }
    }
}
