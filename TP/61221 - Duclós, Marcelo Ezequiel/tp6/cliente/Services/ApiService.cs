using System.Net.Http;
using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos") ?? new List<Producto>();
        }

        public async Task<bool> RegistrarVentaAsync(RegistrarVentaRequest venta)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ventas", venta);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<VentaResponse>> ObtenerHistorialPorEmailAsync(string email)
        {
            var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"/api/clientes/email/{email}");
            if (cliente == null) return new List<VentaResponse>();

            return await _httpClient.GetFromJsonAsync<List<VentaResponse>>($"/api/ventas/cliente/{cliente.Id}")
                   ?? new List<VentaResponse>();
        }

        public class Cliente
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
        }
    }
}
