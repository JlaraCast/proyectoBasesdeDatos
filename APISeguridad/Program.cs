using APIHotelBeach.SA.Services;
using APISeguridad.Model;
using APISeguridad.Seeders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Agregado para trabajar la BD de seguridad
builder.Services.AddDbContext<DbContextSeguridad>(

  options => options.UseOracle(

    builder.Configuration.GetConnectionString("StringConexion")));

// Aquí agregas el servicio de autorización
builder.Services.AddScoped<IAutorizacionServices, AutorizacionServices>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DbContextSeguridad>();
    var seeder = new PantallaSeeder(context);
    await seeder.SeedPantallasAsync();
}


app.Run();
