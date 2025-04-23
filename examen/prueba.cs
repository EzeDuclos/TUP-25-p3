#!/usr/bin/env dotnet script
#r "nuget: Spectre.Console, 0.50.0"

using System;
using Spectre.Console;

// Código de ejemplo
var code = $$$"""
int Suma(int a, int b){ 
    return a + b;
}
int a = 10, b = 20;
Console.WriteLine($"{a} + {b} = {Suma(a,b)}");
""";

// Método 1: Crear texto manualmente coloreado con alineación a la izquierda
var markup = new Markup(@"
[blue]int[/] [green]Suma[/]([blue]int[/] a, [blue]int[/] b){ 
    [yellow]return[/] a + b;
}
[blue]int[/] a = [red]10[/], b = [red]20[/];
Console.WriteLine($""{a} + {b} = {Suma(a,b)}"");".Replace("{", "[[").Replace("}", "]]"));

var panel = new Panel(markup)
    .Header("[yellow]Código C#[/]")
    .Border(BoxBorder.Rounded)
    .Expand();

AnsiConsole.Write(panel);

// Método 2: Mostrar texto simple pero con indentación preservada
AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule("[yellow]Código Original[/]"));
AnsiConsole.WriteLine();
AnsiConsole.WriteLine(code);

// Preguntar al usuario
string nombre = AnsiConsole.Ask<string>("¿Cuál es tu [green]nombre[/]?");

// Mostrar un menú de selección
var opcion = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Elige una opción:")
        .AddChoices($"Opción 1\n[red]  {nombre}[/]", "Opción 2", "Salir"));
        
if (opcion == "Salir") {
    AnsiConsole.MarkupLine("[red]Saliendo...[/]");
} else {
    AnsiConsole.MarkupLine($"[green]Seleccionaste:[/] {opcion}");
}