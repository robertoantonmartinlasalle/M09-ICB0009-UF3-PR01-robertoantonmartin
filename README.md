#  Simulador de Tráfico - ICB0009 UF3 PR01

Este proyecto es una práctica del módulo ICB0009 (UF3) en el que se simula el tráfico de vehículos que recorren una carretera de 100 km con un puente de un solo carril.

Los vehículos son gestionados como procesos independientes (clientes), y se comunican con un servidor principal que controla el estado de la carretera y el acceso al puente.

---

##  Estructura del proyecto

El proyecto se organiza en varios subproyectos dentro de un workspace en Visual Studio Code:

- `Servidor`: controla la lógica principal y gestiona múltiples clientes concurrentemente.
- `Cliente`: representa un vehículo que avanza por la carretera y se comunica con el servidor.
- `Vehiculo`: contiene la clase con los datos y estado de cada vehículo.
- `Carretera`: contiene la lista de vehículos y métodos para gestionar su estado.
- `NetworkStreamClass`: contiene métodos para la lectura y escritura con NetworkStream.

---

##  Etapa 1 - Estructura Base y Control de Versiones

- Se ha configurado el repositorio en GitHub desde VS Code.
- Se ha añadido un `.gitignore` adecuado para evitar subir archivos de compilación (`bin/`, `obj/`).
- Se ha subido el archivo `Carretera-workspace.code-workspace` para mantener el entorno completo.

---

##  Alumno

- **Nombre completo:** Roberto Antón Martín
- **UF3 - Proyecto:** Simulación de tráfico con sockets y servicios
- **Centro:** La Salle
