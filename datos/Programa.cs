using System.Text.RegularExpressions;
using TUP;
    // using System.Collections.Generic;

class Program {
    static string ElegirOpcionMenu() {
        Console.Clear();
        Consola.Escribir("=== MENÚ DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos");
        Consola.Escribir("2. Publicar trabajo práctico");
        Consola.Escribir("3. Verificar presentación de trabajos práctico");
        Consola.Escribir("4. Faltan presentar trabajo práctico");
        Consola.Escribir("5. Verificar asistencia");
        Consola.Escribir("6. Mostrar recuperación");
        Consola.Escribir("7. Normalizar créditos");
        Consola.Escribir("0. Salir");
        return Consola.ElegirOpcion("\nElija una opción (0-7): ", "01234567");
    }

    static void OpcionListarAlumnos(Clase clase) {
        Consola.Escribir("=== Listado de alumnos ===", ConsoleColor.Cyan);
        clase.ListarAlumnos();
        clase.ExportarDatos();
    }

    static void OpcionCopiarPractico(Clase clase) {
        Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
        string tp   = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3", "4", "5", "6" });
        bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

        clase.NormalizarCarpetas();
        clase.CopiarPractico(int.Parse(tp), forzar);
    }

    static void OpcionVerificarPresentacion(Clase clase, int practico) {
        Consola.Escribir("=== Verificar presentación de trabajo práctico ===", ConsoleColor.Cyan);
        clase.NormalizarCarpetas();
        clase.Reiniciar();
        for (var p = 1; p <= practico; p++) {
            clase.VerificaPresentacionPractico(p);
        }
        var asistencias = Asistencias.Cargar(false);
        clase.CargarAsistencia(asistencias);
        clase.Guardar();
    }

    static void OpcionListarNoPresentaron(Clase clase, int practico) {
        Consola.Escribir($"=== Alumnos que no presentaron práctico {practico} ===", ConsoleColor.Cyan);
        clase.ListarNoPresentaron(practico);
    }

    static void OpcionVerificarAsistencia() {
        Consola.Escribir("=== Verificar asistencia ===", ConsoleColor.Cyan);
        Asistencias.Cargar(true);
    }

    static void OpcionMostrarRecuperacion(Clase clase) {
        Consola.Escribir("=== Generando reporte de recuperación ===", ConsoleColor.Cyan);
        clase.GenerarReporteRecuperacion();
        Consola.Escribir("Reporte 'recuperacion.md' generado.", ConsoleColor.Green);
        clase.DebenRecuperar().ListarAlumnos();
    }

    static void ConvertirNombreTelefono(Clase clase, string origen) {
        var lineasOriginales = File.ReadAllLines(origen);

        Consola.Escribir($"Leyendo archivo '{origen}'...", ConsoleColor.Cyan);
        if (!File.Exists(origen)) {
            Consola.Escribir($"El archivo '{origen}' no existe. Verifique la ruta y vuelva a intentarlo.", ConsoleColor.Red);
            return;
        } 
        Consola.Escribir($"Tiene {lineasOriginales.Length} líneas.", ConsoleColor.Cyan);

        var alias = new Dictionary<string, string>();
        foreach (var alumno in clase) {
            if (alumno.TieneTelefono) {
                alias[alumno.NombreLimpio] = alumno.TelefonoLimpio;
                // Console.WriteLine($"Alias: {alumno.NombreLimpio} -> {alumno.TelefonoLimpio}");
            }
        }
        // Numero excepcionales 
        alias["Alejandro Di Battista"] = "3815343458";
        alias["gonzalo zamora"] = "3813540535";
        alias["~ Gabriel Carabajal"] = "3815627688";
        alias["Cristian Ivan Soraire"] = "";
        alias["Abigail * Medina Costilla"] = "3816557818";
        alias["~ lauu🥶"] = "3812130484";

        var contar = 0;
        for (int i = 0; i < lineasOriginales.Length; i++) {
            foreach (var kvp in alias) {
                if (lineasOriginales[i].Contains(kvp.Key, StringComparison.OrdinalIgnoreCase)) {
                    lineasOriginales[i] = lineasOriginales[i].Replace(kvp.Key, kvp.Value, StringComparison.OrdinalIgnoreCase);
                    contar++;
                }
            }
        }
        Consola.Escribir($"Se encontraron {contar} coincidencias.", ConsoleColor.Cyan);
        File.WriteAllLines(origen, lineasOriginales);
    }

    static void NormalizarCreditos(Clase clase, string origen) {
        if (!File.Exists(origen)) {
            Consola.Escribir($"El archivo '{origen}' no existe. Verifique la ruta y vuelva a intentarlo.", ConsoleColor.Red);
            return;
        }

        var codigos = LeerCodigos(origen);
        var contar = 1;
        foreach (var alumno in clase) {
            if (codigos.ContainsKey(alumno.TelefonoLimpio)) {
                alumno.Creditos = codigos[alumno.TelefonoLimpio].Count;
            } else {
                Consola.Escribir($"{contar++,2}) No se encontró {alumno.NombreCompleto} Tel>: {alumno.TelefonoLimpio} en el archivo.", ConsoleColor.Red);
            }
        }

        foreach (var alumno in clase.OrderByDescending(a => a.Creditos)) {
            if (alumno.Creditos > 0) {
                Consola.Escribir($"{alumno.NombreCompleto,-40} - {alumno.TelefonoLimpio} - {alumno.Creditos,2}");
            }
        }
        clase.Guardar("alumos-normal.md");
    }

    static Dictionary<string, HashSet<string>> LeerCodigos(string origen) {
        Dictionary<string, HashSet<string>> codigos = new();
        var telefono = "";
        var patronTelefono = new Regex(@"\b(\d{10})\b");
        var patronCodigo = new Regex(@"\b[0-9a-fA-F]{6}\b");
        var lineas = File.ReadLines(origen);
        var contar = 0;
        foreach (var linea in lineas) {

            var matchTelefono = patronTelefono.Match(linea);
            if (matchTelefono.Success) {
                contar++;
                if(telefono == "3815235887") Consola.Escribir($"Teléfono: {telefono} -> {linea} {patronCodigo.IsMatch(linea)}");

                telefono = matchTelefono.Groups[1].Value;
                if (!codigos.ContainsKey(telefono)) {
                    codigos[telefono] = new HashSet<string>();
                }
            }
            var matchCodigo = patronCodigo.Matches(linea);
            foreach (Match m in matchCodigo) {
                if(telefono == "3815235887") Consola.Escribir($"Teléfono: {telefono} -> Código: {m.Value}");
                if(telefono.Trim() == "") continue;
                codigos[telefono].Add(m.Value);
            }
        }
        Consola.Escribir($"Hay {lineas.Count()} líneas en el archivo con {contar} telefonos.");
        Consola.EsperarTecla();
        return codigos;
    }

    static void Main(string[] args) {
        var clase = Clase.Cargar();
        int practico = 4;

        Consola.Escribir("=== Bienvenido al sistema de gestión de alumnos ===", ConsoleColor.Cyan);
        while (true) {
            string opcion = ElegirOpcionMenu();
            if (opcion == "0") return;
            Console.Clear();

            Action action = opcion switch {
                "1" => () => OpcionListarAlumnos(clase),
                "2" => () => OpcionCopiarPractico(clase),
                "3" => () => OpcionVerificarPresentacion(clase, practico),
                "4" => () => OpcionListarNoPresentaron(clase, practico),
                "5" => () => OpcionVerificarAsistencia(),
                "6" => () => OpcionMostrarRecuperacion(clase),
                "7" => () => {
                    ConvertirNombreTelefono(clase, "./asistencias/historia-c3.txt");
                    NormalizarCreditos(clase, "./asistencias/historia-c3.txt");
                    ConvertirNombreTelefono(clase, "./asistencias/historia-c5.txt");
                    NormalizarCreditos(clase, "./asistencias/historia-c5.txt");
                },
                _   => () => {}
            };
            action();
            Consola.EsperarTecla();
        }
    }
}