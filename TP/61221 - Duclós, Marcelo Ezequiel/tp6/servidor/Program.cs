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

app.Run();
