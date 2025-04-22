#  Simulador de Tráfico - ICB0009 UF3 PR01

Este proyecto es una práctica del módulo **ICB0009 (UF3)** en el que se simula el tráfico de vehículos que recorren una carretera de 100 km con un puente de un solo carril.

Los vehículos son gestionados como procesos independientes (**clientes**) y se comunican con un **servidor principal**, que controla el estado de la carretera y el acceso al puente.

---

##  Estructura del proyecto

El proyecto se organiza en varios subproyectos dentro de un workspace en Visual Studio Code:

- `Servidor`: controla la lógica principal y gestiona múltiples clientes concurrentemente.
- `Cliente`: representa un vehículo que avanza por la carretera y se comunica con el servidor.
- `Vehiculo`: contiene la clase con los datos y estado de cada vehículo.
- `Carretera`: contiene la lista de vehículos y métodos para gestionar su estado.
- `NetworkStreamClass`: contiene métodos para la lectura y escritura con NetworkStream.

---

## Estructura Base y Control de Versiones

- Se ha configurado el repositorio en GitHub desde Visual Studio Code.
- Se ha añadido un archivo `.gitignore` adecuado para evitar subir carpetas de compilación (`bin/`, `obj/`).
- Se ha subido el archivo `Carretera-workspace.code-workspace` para mantener el entorno completo.
- Se ha creado la rama `developer` para trabajar progresivamente con control de versiones.

---

##  Etapa 1 - Conexión servidor-cliente

###  Objetivo

Establecer una conexión básica entre un cliente (vehículo) y el servidor. El servidor debe quedar escuchando en un puerto y aceptar una única conexión. El cliente debe conectarse correctamente y mostrarse el estado por consola.

---

###  Explicación técnica

- El servidor se ha configurado para escuchar conexiones entrantes a través de `TcpListener` en el puerto 13000.
- El cliente utiliza `TcpClient` para conectarse al servidor utilizando `localhost`.
- Se han incluido mensajes por consola que permiten verificar en tiempo real si la conexión se ha realizado correctamente.
- El cliente incorpora un bloque `try-catch` para capturar posibles errores si el servidor no está disponible en ese momento.
- Ambos proyectos han sido probados de manera secuencial para verificar el éxito de la conexión.

---

###  Resultado de la prueba

- El **servidor** se inicia correctamente y queda esperando conexiones.
- El **cliente** se conecta sin problemas al servidor.
- Ambos programas muestran mensajes claros en consola que indican el estado de la conexión.
- Se ha confirmado visualmente que la conexión ha tenido éxito mediante pruebas funcionales y captura de pantalla.

---

###  Captura de pantalla

![Captura conexión cliente-servidor](./img/etapa1-cliente-servidor.png)

---

###  Comentario personal

Durante esta etapa he validado que el cliente y el servidor pueden comunicarse correctamente. He seguido la estructura proporcionada en clase y he utilizado mensajes claros para comprobar el flujo de la conexión. También he aplicado buenas prácticas como el uso de `try-catch` en el cliente para evitar errores imprevistos. Este es el punto de partida sobre el que se construirán las siguientes etapas.

---

##  Alumno

- **Nombre completo:** Roberto Antón Martín  
- **UF:** UF3 – Simulación de tráfico con sockets y servicios  
- **Centro:** La Salle  
