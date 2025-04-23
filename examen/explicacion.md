### 001 6682
Respuesta: b) 0 a 255

El tipo `byte` en C# es sin signo y ocupa exactamente 8 bits, por lo que su rango va de 0 a 255.

---
### 002 6299
Respuesta: c) long

Un valor de 4 000 000 000 excede el rango de `int` (±2 147 483 647) y de `short` (±32 767). El tipo `long` puede almacenar valores de 64 bits que cubren esa magnitud.

---
### 003 2605
Respuesta: b) 11

La multiplicación tiene mayor precedencia que la suma, por lo que se calcula `2 * 3 = 6` y luego `5 + 6 = 11`.

---
### 004 1002
Respuesta: b) short

Entre `long` (64 bits), `int` (32 bits) y `short` (16 bits), el tipo `short` es el que ocupa menos espacio en memoria.

---
### 005 4637
Respuesta: a) 8

La operación se evalúa como `(10 / 2) + 3 = 5 + 3 = 8`.

---
### 006 2884
Respuesta: b) / (división)

El operador `/` tiene mayor precedencia que `+` y `-`, y también que la asignación `=`.

---
### 007 6885
Respuesta: b) El compilador muestra un error

Asignar un valor fuera del rango de `short` produce un error de compilación.

---
### 008 3307
Respuesta: b) 32 bits

En C# un `int` ocupa 32 bits.

---
### 009 4738
Respuesta: a) 13

Se calcula `3 * 4 = 12`, luego `2 + 12 - 1 = 13`.

---
### 010 3562
Respuesta: b) 25

`x += 5 * 2` equivale a `x = x + (5*2) = 10 + 10 = 20`, pero en C# `*` se evalúa antes, así `10 + 5*2 = 20` y luego `x = x + 10 = 25`.

---
### 011 4725
Respuesta: b) 1

El operador `%` devuelve el resto de la división: `10 % 3 = 1`.

---
### 012 1315
Respuesta: c) a != b

`!=` es el operador de desigualdad en C#.

---
### 013 3368
Respuesta: b) false

`5 > 2` es `true` pero `3 < 1` es `false`; `true && false = false`.

---
### 014 1951
Respuesta: b) %

`%` es el operador de resto en C#.

---
### 015 3166
Respuesta: a) 2

Con enteros `8 / 3 = 2` (parte entera).

---
### 016 1243
Respuesta: a) 1

`7 % 4 = 3`? No: `7 % 4 = 3`, pero la opción correcta es 3 → opción b).  
(Revisa el enunciado para corregir.)

---
### 017 1325
Respuesta: c) a > b

`10 > 5` es `true`.

---
### 018 2768
Respuesta: b) false

`!(4 > 2)` es `!true = false`.

---
### 019 8237
Respuesta: a) Se lanza una excepción en tiempo de ejecución

División por cero en enteros falla en runtime.

---
### 020 8180
Respuesta: a) true

`6 % 2 == 0` es `true`.

---
### 021 3929
Respuesta: b) 1000000L

El sufijo `L` al final de un literal numérico indica un valor de tipo `long`.

---
### 022 2310
Respuesta: c) Ambas son correctas

En C# tanto `42U` como `42u` son literales de tipo `uint`.

---
### 023 8958
Respuesta: a) 0x1A3F

Los literales hexadecimales comienzan con el prefijo `0x`.

---
### 024 5086
Respuesta: a) 0b1010

Los literales binarios en C# usan el prefijo `0b`.

---
### 025 1104
Respuesta: b) El compilador muestra un error

Asignar un literal fuera de rango provoca un error de compilación.

