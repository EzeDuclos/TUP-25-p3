using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class ListaOrdenada<T> : IEnumerable<T> where T : IComparable<T> {
    class Nodo {
        T Elemento;
        Nodo Menor;
        Nodo Mayor;
        int Cantidad; // Cantidad total de elementos en este subárbol

        public Nodo(T elemento) {
            Elemento = elemento;
            Cantidad = 1; // Este nodo contiene 1 elemento
        }

        // Obtiene la cantidad de elementos en un subárbol (evitando NullReferenceException)
        int CantidadEn(Nodo nodo) => nodo?.Cantidad ?? 0;

        public Nodo Agregar(T nuevo) {
            if (nuevo.CompareTo(Elemento) < 0) {
                Menor = Menor?.Agregar(nuevo) ?? new Nodo(nuevo);
            } else {
                Mayor = Mayor?.Agregar(nuevo) ?? new Nodo(nuevo);
            }
            // Actualizar el contador de elementos
            Cantidad = 1 + CantidadEn(Menor) + CantidadEn(Mayor);
            return this;
        }

        public bool Contiene(T elemento) {
            if (elemento.Equals(Elemento)) return true;
            if (elemento.CompareTo(Elemento) < 0) {
                return Menor?.Contiene(elemento) ?? false;
            } else {
                return Mayor?.Contiene(elemento) ?? false;
            }
        }

        public Nodo Eliminar(T elemento) {
            if (elemento.CompareTo(Elemento) < 0) {
                Menor = Menor?.Eliminar(elemento);
            } else if (elemento.CompareTo(Elemento) > 0) {
                Mayor = Mayor?.Eliminar(elemento);
            } else {
                if (Menor == null) return Mayor;
                if (Mayor == null) return Menor;

                var menorMayor = Mayor;
                while (menorMayor.Menor != null) {
                    menorMayor = menorMayor.Menor;
                }
                Elemento = menorMayor.Elemento;
                Mayor = Mayor?.Eliminar(menorMayor.Elemento);
            }
            // Actualizar el contador de elementos después de eliminar
            Cantidad = 1 + CantidadEn(Menor) + CantidadEn(Mayor);
            return this;
        }

        public void InOrden(List<T> elementos) {
            Menor?.InOrden(elementos);
            elementos.Add(Elemento);
            Mayor?.InOrden(elementos);
        }

        // Método para obtener elemento por índice de manera optimizada
        public T ObtenerPorIndice(int indice) {
            int cantidadIzquierda = CantidadEn(Menor);
            
            if (indice < cantidadIzquierda) {
                // El elemento está en el subárbol izquierdo
                return Menor.ObtenerPorIndice(indice);
            } else if (indice == cantidadIzquierda) {
                // El elemento es este nodo
                return Elemento;
            } else {
                // El elemento está en el subárbol derecho
                return Mayor.ObtenerPorIndice(indice - cantidadIzquierda - 1);
            }
        }

        // Recorrido in-order utilizando yield return
        public IEnumerable<T> EnumerarInOrden() {
            // Primero recorremos el subárbol izquierdo
            if (Menor != null) {
                foreach (var elemento in Menor.EnumerarInOrden()) {
                    yield return elemento;
                }
            }
            
            // Luego devolvemos el elemento actual
            yield return Elemento;
            
            // Finalmente recorremos el subárbol derecho
            if (Mayor != null) {
                foreach (var elemento in Mayor.EnumerarInOrden()) {
                    yield return elemento;
                }
            }
        }
    }

    Nodo raiz;
    int cantidad;
    public int Cantidad => cantidad;

    public ListaOrdenada() {
        raiz = null;
        cantidad = 0;
    }

    public ListaOrdenada(IEnumerable<T> elementos) : this() {
        foreach (var elemento in elementos) {
            Agregar(elemento);
        }
    }

    public bool Contiene(T elemento) {
        return raiz?.Contiene(elemento) ?? false;
    }

    public void Agregar(T elemento) {
        if (Contiene(elemento)) return;

        raiz = raiz?.Agregar(elemento) ?? new Nodo(elemento);
        cantidad++;
    }

    public void Eliminar(T elemento) {
        if (!Contiene(elemento)) return;
        raiz = raiz?.Eliminar(elemento);
        cantidad--;
    }

    public T this[int index] {
        get {
            if (index < 0 || index >= cantidad) throw new IndexOutOfRangeException();
            return raiz.ObtenerPorIndice(index);
        }
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> predicado) {
        var elementos = new List<T>();
        raiz?.InOrden(elementos);
        return new ListaOrdenada<T>(elementos.Where(predicado));
    }

    // Implementación de IEnumerable<T>
    public IEnumerator<T> GetEnumerator() {
        if (raiz == null) yield break;
        foreach (var elemento in raiz.EnumerarInOrden()) {
            yield return elemento;
        }
    }

    // Implementación requerida de IEnumerable (no genérica)
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}

class Contacto : IComparable<Contacto> {
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    public Contacto(string nombre, string telefono) {
        Nombre = nombre;
        Telefono = telefono;
    }

    public int CompareTo(Contacto otro) {
        return string.Compare(Nombre, otro.Nombre, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) {
        if (obj is Contacto otro) {
            return Nombre == otro.Nombre && Telefono == otro.Telefono;
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Nombre, Telefono);
    }
}


/// --------------------------------------------------------///
///   Desde aca para abajo no se puede modificar el código  ///
/// --------------------------------------------------------///

/// 
/// PRUEBAS AUTOMATIZADAS
///

// Funcion auxiliar para las pruebas
public static void Assert<T>(T real, T esperado, string mensaje){
    if (!Equals(esperado, real)) throw new Exception($"[ASSERT FALLÓ] {mensaje} → Esperado: {esperado}, Real: {real}");
    Console.WriteLine($"[OK] {mensaje}");
}


/// Pruebas de lista ordenada (con enteros)

var lista = new ListaOrdenada<int>();
lista.Agregar(5);
lista.Agregar(1);
lista.Agregar(3);

Assert(lista[0], 1, "Primer elemento");
Assert(lista[1], 3, "Segundo elemento");
Assert(lista[2], 5, "Tercer elemento");
Assert(lista.Cantidad, 3, "Cantidad de elementos");

Assert(lista.Filtrar(x => x > 2).Cantidad, 2, "Cantidad de elementos filtrados");
Assert(lista.Filtrar(x => x > 2)[0], 3, "Primer elemento filtrado");
Assert(lista.Filtrar(x => x > 2)[1], 5, "Segundo elemento filtrado");

Assert(lista.Contiene(1), true,  "Contiene");
Assert(lista.Contiene(2), false, "No contiene");

lista.Agregar(3);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras agregar un elemento repetido");

lista.Agregar(2);
Assert(lista.Cantidad, 4, "Cantidad de elementos tras agregar 2");
Assert(lista[0], 1, "Primer elemento tras agregar 2");
Assert(lista[1], 2, "Segundo elemento tras agregar 2");
Assert(lista[2], 3, "Tercer elemento tras agregar 2");

lista.Eliminar(2);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras eliminar elemento existente");
Assert(lista[0], 1, "Primer elemento tras eliminar 2");
Assert(lista[1], 3, "Segundo elemento tras eliminar 2");
lista.Eliminar(100);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras eliminar elemento inexistente");

// Cambiamos el nombre de la variable lista a lista2 para evitar conflictos
var lista2 = new ListaOrdenada<int>();
lista2.Agregar(5);
lista2.Agregar(3);
lista2.Agregar(7);

// Uso directo en foreach
foreach (var elemento in lista2) {
    Console.WriteLine(elemento);
}

// Uso con LINQ
var mayoresQueCinco = lista2.Where(x => x > 5);


/// Pruebas de lista ordenada (con cadenas)
/// 
var nombres = new ListaOrdenada<string>(new string[] { "Juan", "Pedro", "Ana" });
Assert(nombres.Cantidad, 3, "Cantidad de nombres");

Assert(nombres[0], "Ana", "Primer nombre");
Assert(nombres[1], "Juan", "Segundo nombre");
Assert(nombres[2], "Pedro", "Tercer nombre");

Assert(nombres.Filtrar(x => x.StartsWith("A")).Cantidad, 1, "Cantidad de nombres que empiezan con A");
Assert(nombres.Filtrar(x => x.Length > 3).Cantidad, 2, "Cantidad de nombres con más de 3 letras");

Assert(nombres.Contiene("Ana"), true, "Contiene Ana");
Assert(nombres.Contiene("Domingo"), false, "No contiene Domingo");

nombres.Agregar("Pedro");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras agregar Pedro nuevamente");

nombres.Agregar("Carlos");
Assert(nombres.Cantidad, 4, "Cantidad de nombres tras agregar Carlos");

Assert(nombres[0], "Ana", "Primer nombre tras agregar Carlos");
Assert(nombres[1], "Carlos", "Segundo nombre tras agregar Carlos");

nombres.Eliminar("Carlos");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras agregar Carlos");

Assert(nombres[0], "Ana", "Primer nombre tras eliminar Carlos");
Assert(nombres[1], "Juan", "Segundo nombre tras eliminar Carlos");

nombres.Eliminar("Domingo");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras eliminar un elemento inexistente");

Assert(nombres[0], "Ana", "Primer nombre tras eliminar Domingo");
Assert(nombres[1], "Juan", "Segundo nombre tras eliminar Domingo");


/// Pruebas de lista ordenada (con contactos) 

var juan  = new Contacto("Juan",  "123456");
var pedro = new Contacto("Pedro", "654321");
var ana   = new Contacto("Ana",   "789012");
var otro  = new Contacto("Otro",  "345678");

var contactos = new ListaOrdenada<Contacto>(new Contacto[] { juan, pedro, ana });
Assert(contactos.Cantidad, 3, "Cantidad de contactos");
Assert(contactos[0].Nombre, "Ana", "Primer contacto");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto");

Assert(contactos.Filtrar(x => x.Nombre.StartsWith("A")).Cantidad, 1, "Cantidad de contactos que empiezan con A");
Assert(contactos.Filtrar(x => x.Nombre.Contains("a")).Cantidad, 2, "Cantidad de contactos que contienen a");

Assert(contactos.Contiene(juan), true, "Contiene Juan");
Assert(contactos.Contiene(otro), false, "No contiene Otro");

contactos.Agregar(otro);
Assert(contactos.Cantidad, 4, "Cantidad de contactos tras agregar Otro");
Assert(contactos.Contiene(otro), true, "Contiene Otro");

Assert(contactos[0].Nombre, "Ana", "Primer contacto tras agregar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras agregar Otro");
Assert(contactos[2].Nombre, "Otro", "Tercer contacto tras agregar Otro");
Assert(contactos[3].Nombre, "Pedro", "Cuarto contacto tras agregar Otro");

contactos.Eliminar(otro);
Assert(contactos.Cantidad, 3, "Cantidad de contactos tras eliminar Otro");
Assert(contactos[0].Nombre, "Ana", "Primer contacto tras eliminar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras eliminar Otro");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto tras eliminar Otro");

contactos.Eliminar(otro);
Assert(contactos.Cantidad, 3, "Cantidad de contactos tras eliminar un elemento inexistente");
Assert(contactos[0].Nombre, "Ana", "Primer contacto tras eliminar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras eliminar Otro");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto tras eliminar Otro");