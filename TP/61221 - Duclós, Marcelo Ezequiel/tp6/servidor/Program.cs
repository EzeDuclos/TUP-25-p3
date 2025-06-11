using servidor.Data;
using servidor.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios CORS para permitir solicitudes desde el cliente
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuración de EF Core con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar controladores si es necesario
builder.Services.AddControllers();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

// Usar CORS con la política definida
app.UseCors("AllowClientApp");

// Mapear rutas básicas
app.MapGet("/", () => "Servidor API está en funcionamiento");

// Ejemplo de endpoint de API
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// Endpoints CRUD para productos

app.MapGet("/api/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/api/productos/{id:int}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound()
);

app.MapPost("/api/productos", async (Producto producto, TiendaContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/productos/{producto.Id}", producto);
});

app.MapPut("/api/productos/{id:int}", async (int id, Producto input, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = input.Nombre;
    producto.Descripcion = input.Descripcion;
    producto.Precio = input.Precio;
    producto.Stock = input.Stock;
    producto.ImagenUrl = input.ImagenUrl;

    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/api/productos/{id:int}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoints CRUD para clientes

app.MapGet("/api/clientes", async (TiendaContext db) =>
    await db.Clientes.ToListAsync()
);

app.MapGet("/api/clientes/{id:int}", async (int id, TiendaContext db) =>
    await db.Clientes.FindAsync(id) is Cliente cliente
        ? Results.Ok(cliente)
        : Results.NotFound()
);

app.MapPost("/api/clientes", async (Cliente cliente, TiendaContext db) =>
{
    db.Clientes.Add(cliente);
    await db.SaveChangesAsync();
    return Results.Created($"/api/clientes/{cliente.Id}", cliente);
});

app.MapPut("/api/clientes/{id:int}", async (int id, Cliente input, TiendaContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    if (cliente is null) return Results.NotFound();

    cliente.Nombre = input.Nombre;
    cliente.Apellido = input.Apellido;
    cliente.Email = input.Email;

    await db.SaveChangesAsync();
    return Results.Ok(cliente);
});

app.MapDelete("/api/clientes/{id:int}", async (int id, TiendaContext db) =>
{
    var cliente = await db.Clientes.FindAsync(id);
    if (cliente is null) return Results.NotFound();

    db.Clientes.Remove(cliente);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoint POST para ventas con validación de stock

app.MapPost("/api/ventas", async (RegistrarVentaRequest ventaRequest, TiendaContext db) =>
{
    // Validar stock
    foreach (var item in ventaRequest.Detalles)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null)
            return Results.BadRequest($"Producto con ID {item.ProductoId} no existe.");
        if (producto.Stock < item.Cantidad)
            return Results.BadRequest($"Stock insuficiente para el producto {producto.Nombre}.");
    }

    // Crear la venta
    var venta = new Venta
    {
        Fecha = DateTime.Now,
        NombreCliente = ventaRequest.NombreCliente,
        ApellidoCliente = ventaRequest.ApellidoCliente,
        EmailCliente = ventaRequest.EmailCliente,
        Total = 0,
        Detalles = new List<DetalleVenta>()
    };

    decimal total = 0;

    foreach (var item in ventaRequest.Detalles)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto == null) continue;

        producto.Stock -= item.Cantidad;

        var detalle = new DetalleVenta
        {
            ProductoId = producto.Id,
            Cantidad = item.Cantidad,
            PrecioUnitario = producto.Precio
        };
        total += producto.Precio * item.Cantidad;
        venta.Detalles.Add(detalle);
    }

    venta.Total = total;
    db.Ventas.Add(venta);
    await db.SaveChangesAsync();

    return Results.Created($"/api/ventas/{venta.Id}", venta);
});

// Endpoint para consultar historial de compras de un cliente

app.MapGet("/api/ventas/cliente/{clienteId:int}", async (int clienteId, TiendaContext db) =>
{
    var ventas = await db.Ventas
        .Where(v => v.EmailCliente == db.Clientes.Where(c => c.Id == clienteId).Select(c => c.Email).FirstOrDefault())
        .Include(v => v.Detalles)
        .ThenInclude(d => d.Producto)
        .ToListAsync();

    return Results.Ok(ventas);
});

app.Run();