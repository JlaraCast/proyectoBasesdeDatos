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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS"); // Confirma el mapeo de la tabla

                // Mapea las propiedades del modelo C# a los nombres de las columnas en MAYÚSCULAS en Oracle
                entity.Property(e => e.idUsuario).HasColumnName("IDUSUARIO"); // Asumiendo que el ID es IDUSUARIO en DB
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.correo).HasColumnName("CORREO");
                entity.Property(e => e.clave).HasColumnName("CLAVE");
                entity.Property(e => e.estado).HasColumnName("ESTADO");

                // Opcional: Si 'idUsuario' es la clave primaria, lo puedes configurar aquí
                entity.HasKey(e => e.idUsuario);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");
                entity.HasKey(e => e.idRol);

                entity.Property(e => e.idRol).HasColumnName("IDROL");
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
            });

            // SISTEMAS (tabla en DB: SISTEMAS)
            modelBuilder.Entity<Sistema>(entity =>
            {
                entity.ToTable("SISTEMAS");
                entity.HasKey(e => e.idSistema);

                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
            });

            // PANTALLAS (tabla en DB: PANTALLAS)
            modelBuilder.Entity<Pantalla>(entity =>
            {
                entity.ToTable("PANTALLAS");
                entity.HasKey(e => e.idPantalla);

                entity.Property(e => e.idPantalla).HasColumnName("IDPANTALLA");
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
                entity.Property(e => e.ruta).HasColumnName("RUTA");

               
            });

            // BITACORA (tabla en DB: BITACORA)
            modelBuilder.Entity<Bitacora>(entity =>
            {
                entity.ToTable("BITACORA");
                entity.HasKey(e => e.idBitacora);

                entity.Property(e => e.idBitacora).HasColumnName("IDBITACORA");
                entity.Property(e => e.idUsuario).HasColumnName("IDUSUARIO");
                entity.Property(e => e.fecha).HasColumnName("FECHA");
                entity.Property(e => e.accion).HasColumnName("ACCION");
                entity.Property(e => e.detalle).HasColumnName("DETALLE");

              
            });


            base.OnModelCreating(modelBuilder);
        }
        //Los modelos y sus respectivas tablas a trabajar
        public DbSet<Usuario> usuarios { get; set; }

        public DbSet<Pantalla> pantallas { get; set; }

        public DbSet<Bitacora> bitacoras { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Sistema> sistemas { get; set; }
    }
}
