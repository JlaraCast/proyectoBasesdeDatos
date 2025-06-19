using Microsoft.EntityFrameworkCore;

namespace APISeguridad.Model
{
    public class DbContextSeguridad : DbContext
    {
        public DbContextSeguridad(DbContextOptions<DbContextSeguridad> options) : base(options)
        {
        }

        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Pantalla> pantallas { get; set; }
        public DbSet<Bitacora> bitacoras { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Sistema> sistemas { get; set; }
        public DbSet<PermisoUsuario> permisosUsuarios { get; set; }
        public DbSet<UsuariosRol> usuariosRoles { get; set; }
        public DbSet<PermisosRoles> permisosRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USUARIOS
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");
                entity.HasKey(e => e.idUsuario);
                entity.Property(e => e.idUsuario)
                    .HasColumnName("IDUSUARIO")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.correo).HasColumnName("CORREO");
                entity.Property(e => e.clave).HasColumnName("CLAVE");
                entity.Property(e => e.estado).HasColumnName("ESTADO");
            });

            // SISTEMAS
            modelBuilder.Entity<Sistema>(entity =>
            {
                entity.ToTable("SISTEMAS");
                entity.HasKey(e => e.idSistema);
                entity.Property(e => e.idSistema)
                    .HasColumnName("IDSISTEMA")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
            });

            // ROLES
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("ROLES");
                entity.HasKey(e => e.idRol);
                entity.Property(e => e.idRol)
                    .HasColumnName("IDROL")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
            });

            // PANTALLAS (clave compuesta)
            modelBuilder.Entity<Pantalla>(entity =>
            {
                entity.ToTable("PANTALLAS");
                entity.HasKey(e => new { e.idPantalla, e.idSistema });
                entity.Property(e => e.idPantalla)
                    .HasColumnName("IDPANTALLA")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.nombre).HasColumnName("NOMBRE");
                entity.Property(e => e.descripcion).HasColumnName("DESCRIPCION");
                entity.Property(e => e.ruta).HasColumnName("RUTA");
            });

            // BITACORAS
            modelBuilder.Entity<Bitacora>(entity =>
            {
                entity.ToTable("BITACORAS");
                entity.HasKey(e => e.idBitacora);
                entity.Property(e => e.idBitacora)
                    .HasColumnName("IDBITACORA")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.idUsuario).HasColumnName("IDUSUARIO");
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.idPantalla).HasColumnName("IDPANTALLA");
                entity.Property(e => e.fecha).HasColumnName("FECHA");
                entity.Property(e => e.accion).HasColumnName("ACCION");
                entity.Property(e => e.detalle).HasColumnName("DETALLE");
            });

            // USUARIOSROLES (clave compuesta)
            modelBuilder.Entity<UsuariosRol>(entity =>
            {
                entity.ToTable("USUARIOSROLES");
                entity.HasKey(e => new { e.idUsuario, e.idRol });
                entity.Property(e => e.idUsuario).HasColumnName("IDUSUARIO");
                entity.Property(e => e.idRol).HasColumnName("IDROL");
            });

            // PERMISOSUSUARIOS (clave compuesta)
            modelBuilder.Entity<PermisoUsuario>(entity =>
            {
                entity.ToTable("PERMISOSUSUARIOS");
                entity.HasKey(e => new { e.IdUsuario, e.IdPantalla });
                entity.Property(e => e.IdUsuario).HasColumnName("IDUSUARIO");
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.IdPantalla).HasColumnName("IDPANTALLA");
                entity.Property(e => e.PermisoInsertar).HasColumnName("PERMISOINSERTAR");
                entity.Property(e => e.PermisoModificar).HasColumnName("PERMISOMODIFICAR");
                entity.Property(e => e.PermisoBorrar).HasColumnName("PERMISOBORRAR");
                entity.Property(e => e.PermisoConsultar).HasColumnName("PERMISOCONSULTAR");
            });

            // PERMISOSROLES (clave compuesta)
            modelBuilder.Entity<PermisosRoles>(entity =>
            {
                entity.ToTable("PERMISOSROLES");
                entity.HasKey(e => new { e.idRol, e.IdPantalla });
                entity.Property(e => e.idRol).HasColumnName("IDROL");
                entity.Property(e => e.idSistema).HasColumnName("IDSISTEMA");
                entity.Property(e => e.IdPantalla).HasColumnName("IDPANTALLA");
                entity.Property(e => e.PermisoInsertar).HasColumnName("PERMISOINSERTAR");
                entity.Property(e => e.PermisoModificar).HasColumnName("PERMISOMODIFICAR");
                entity.Property(e => e.PermisoBorrar).HasColumnName("PERMISOBORRAR");
                entity.Property(e => e.PermisoConsultar).HasColumnName("PERMISOCONSULTAR");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

