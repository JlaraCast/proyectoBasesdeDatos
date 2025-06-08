using System.Text.Json;

namespace APIHotelBeachProyecto.Model
{
    public class ExchangeRate
    {
        private readonly HttpClient _httpClient;

        public ExchangeRate(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetExchangeRateAsync()
        {
            var response = await _httpClient.GetAsync("https://api.hacienda.go.cr/indicadores/tc");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error retrieving the exchange rate");

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            // Accede a dolar → venta → valor
            if (json.RootElement.TryGetProperty("dolar", out JsonElement dolarElement) &&
                dolarElement.TryGetProperty("venta", out JsonElement ventaElement) &&
                ventaElement.TryGetProperty("valor", out JsonElement valorElement))
            {
                return valorElement.GetDecimal(); // ✅ Aquí está el número real: 508.35
            }
            else
            {
                throw new Exception("No se encontró la ruta 'dolar.venta.valor' en la respuesta.");
            }
        }


    }
}
