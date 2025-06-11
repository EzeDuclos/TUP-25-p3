using System.Net.Http;
using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

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
}
