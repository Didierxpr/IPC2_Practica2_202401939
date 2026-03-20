# Practica 2 — Sistema de Turnos Médicos (Windows Forms + Graphviz)

## ¿Qué implementa?

- Cola FIFO (orden FIFO/FIFO) de turnos usando **solo `List<T>`**.
- Interfaz con **Windows Forms** para:
  - Registrar pacientes (nombre, edad, especialidad).
  - Atender al siguiente (desencolar).
  - Mostrar la tabla con tiempos: servicio, espera y total estimado.
- Visualización en **Graphviz** en “tiempo real” al registrar/atender:
  - Se genera un `.dot` con los nodos en el orden de la cola.
  - Se intenta convertirlo a PNG con el ejecutable `dot`.

## Especialidades y tiempos

- `Medicina General` => 10 min
- `Pediatria` => 15 min
- `Ginecologia` => 20 min
- `Dermatologia` => 25 min

## Requisitos

- [.NET SDK (8.x)](https://dotnet.microsoft.com/) con soporte de WinForms.
- [Graphviz](https://graphviz.org/) instalado.
  - Debe estar disponible el comando `dot` en el `PATH`.

## Ejecución

1. Abrir el proyecto `Practica2_TurnosMedicos.csproj` en Visual Studio.
2. Ejecutar `Start` (botón verde).
3. Registrar turnos con los botones.
4. Presionar “Atender Siguiente” para desencolar.

## Si Graphviz no genera la imagen

- El programa igual muestra el contenido `.dot` en el cuadro de texto (`DOT`).
- Para que se vea el gráfico, instala Graphviz y asegúrate que `dot` se ejecute desde `cmd`/PowerShell.

