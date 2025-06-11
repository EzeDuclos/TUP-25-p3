namespace cliente.Models;

public class VentaResponse
{
    public DateTime Fecha { get; set; }
    public List<DetalleResponse> Detalles { get; set; }
}

public class DetalleResponse
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
}