class Proveedor {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
}

class Producto {
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public double Precio { get; set; }
    public int ProveedorId { get; set; }
}

var cocacomp = new Proveedor{ Id = 1, Nombre = "Coca Cola Company", Telefono = "123456789" };
var pepsico  = new Proveedor{ Id = 2, Nombre = "PepsiCo", Telefono = "987654321" };

var coca    = new Producto{ Id = 11, Codigo = "P001", Nombre = "Coca Cola",  Precio = 1.50, ProveedorId = 1 };
var sprite  = new Producto{ Id = 12, Codigo = "P002", Nombre = "Sprite",     Precio = 1.30, ProveedorId = 1 };
var pepsi   = new Producto{ Id = 13, Codigo = "P003", Nombre = "Pepsi Cola", Precio = 1.40, ProveedorId = 2 };
var mirinda = new Producto{ Id = 14, Codigo = "P004", Nombre = "Mirinda",    Precio = 1.20, ProveedorId = 2 };

var productos = new[] { coca, sprite, pepsi, mirinda };
var proveedores = new[] { cocacomp, pepsico };

void ListarProductoDelProveedor(Proveedor proveedor){
    Console.WriteLine($"\n=== Productos de {proveedor.Nombre} ===");
    foreach (var producto in productos ) {
        if(producto.ProveedorId == proveedor.Id)
            Console.WriteLine($"- {producto.Nombre,-20} ${producto.Precio,5:f2} (Código: {producto.Codigo})");
    }
}


ListarProductoDelProveedor(pepsico);