namespace AppWebHotelBeach.Models
{
    public class ApiHotelBeach
    {
        public HttpClient IniciarApi()
        {

            //Variable para manejar la referencia con la API
            HttpClient client = new HttpClient();

            //Se indica la API local 
            client.BaseAddress = new Uri("https://localhost:7225/");

            //Se retorna la referencia
            return client;
        }
    }
}
