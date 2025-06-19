using APISeguridad.Model;
using Microsoft.EntityFrameworkCore;


namespace APISeguridad.Seeders
{
    public class PantallaSeeder
    {
        private readonly DbContextSeguridad _context;

        public PantallaSeeder(DbContextSeguridad context)
        {
            _context = context;
        }

        public async Task SeedPantallasAsync()
        {
            int idSistemaSeguridad = 8080;
            string nombreSistemaSeguridad = "Seguridad";

            // Verificar si el sistema ya existe
            var sistema = await _context.sistemas.FindAsync(idSistemaSeguridad);
            if (sistema == null)
            {
                sistema = new Sistema
                {
                    idSistema = idSistemaSeguridad,
                    nombre = nombreSistemaSeguridad,
                    descripcion = "Sistema de Seguridad para controlar accesos y permisos"
                };
                _context.sistemas.Add(sistema);
                await _context.SaveChangesAsync();
            }

            var pantallas = new List<(string nombre, string ruta)>
    {
        // Pantallas
        ("Agregar", "/Pantallas/Agregar"),
        ("Detalles", "/Pantallas/Detalles"),
        ("Editar", "/Pantallas/Editar"),
        ("Eliminar", "/Pantallas/Eliminar"),
        ("List", "/Pantallas/List"),

        // Bitacoras
        ("Agregar", "/Bitacoras/Agregar"),
        ("List", "/Bitacoras/List"),

        // Roles
        ("Agregar", "/Roles/Agregar"),
        ("Detalles", "/Roles/Detalles"),
        ("Editar", "/Roles/Editar"),
        ("Eliminar", "/Roles/Eliminar"),
        ("List", "/Roles/List"),

        // Sistemas
        ("Create", "/Sistemas/Create"),
        ("Details", "/Sistemas/Details"),
        ("Edit", "/Sistemas/Edit"),
        ("Delete", "/Sistemas/Delete"),
        ("List", "/Sistemas/List"),

        // Usuarios
        ("Agregar", "/Usuarios/Agregar"),
        ("Details", "/Usuarios/Details"),
        ("Edit", "/Usuarios/Edit"),
        ("Delete", "/Usuarios/Delete"),
        ("List", "/Usuarios/List"),
        ("Login", "/Usuarios/Login"),
    };


            foreach (var pantallaInfo in pantallas)
            {
                var count = await _context.pantallas
                    .CountAsync(p => p.idSistema == idSistemaSeguridad 
                    && p.nombre == pantallaInfo.nombre 
                    && p.ruta == pantallaInfo.ruta);

                if (count == 0)
                {
                    var nuevaPantalla = new Pantalla
                    {
                        idSistema = idSistemaSeguridad,
                        nombre = pantallaInfo.nombre,
                        ruta = pantallaInfo.ruta,
                        descripcion = $"Pantalla {pantallaInfo.nombre} del sistema Seguridad"
                    };

                    _context.pantallas.Add(nuevaPantalla);
                }
            }


            await _context.SaveChangesAsync();
        }

    }
}
