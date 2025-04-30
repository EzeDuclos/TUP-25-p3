using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta { get; set; } = "";
}

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}
class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            // Crea la base de datos si no existe
            db.Database.EnsureCreated();
            db.Preguntas.Add(new Pregunta {
                Enunciado = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta = "B"
            });
            db.SaveChanges();
        }
    }
}