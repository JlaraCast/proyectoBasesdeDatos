using Microsoft.EntityFrameworkCore;

namespace APIHotelBeachProyecto.Model
{
    public class DbContextHotel : DbContext
    {
        public DbContextHotel(
           DbContextOptions<DbContextHotel> options) : base(options)
        {


        }

        //Los modelos y sus respectivas tablas a trabajar
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Check> Checks { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CustomerInvoice> CustomersInvoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // InvoiceId se genera automáticamente
            modelBuilder.Entity<Invoice>()
                .Property(i => i.InvoiceId)
                .ValueGeneratedOnAdd();

            // Definir clave primaria compuesta para CustomersInvoice
            modelBuilder.Entity<CustomerInvoice>()
                .HasKey(ci => new { ci.CustomerId, ci.InvoiceID });

            // Configurar la relación si lo deseas (opcional pero recomendado)
            modelBuilder.Entity<CustomerInvoice>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(ci => ci.CustomerId);

            modelBuilder.Entity<CustomerInvoice>()
                .HasOne<Invoice>()
                .WithMany()
                .HasForeignKey(ci => ci.InvoiceID);

            // ReservationId también se puede generar automáticamente si deseas
            modelBuilder.Entity<Reservation>()
                .Property(r => r.ReservationId)
                .ValueGeneratedOnAdd();
        }


    }
}
