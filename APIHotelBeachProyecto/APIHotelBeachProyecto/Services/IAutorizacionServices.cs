using APIHotelBeachProyecto.Model;

namespace APIHotelBeach.SA.Services
{
    public interface IAutorizacionServices
    {
        //Metodo obligatorio a implementar las clases
        Task<AutorizacionResponse> DevolverToken(User autorizacion);
    }
}
