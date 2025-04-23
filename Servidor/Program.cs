using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic; // Lo añado para la lista de la etapa 7.
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Servidor
{
    // Etapa 7: Creo una clase que me permitirá guardar la información de cada cliente
    public class ClienteConectado
    {
        public int Id { get; set; }
        public NetworkStream Stream { get; set; }

        public ClienteConectado(int id, NetworkStream stream)
        {
            Id = id;
            Stream = stream;
        }
    }
    class Program
    {
        // Contador global de IDs que se autoincrementará para cada cliente.
        static int contadorId = 1;

        // Lista de todos los clientes conectados (etapa 7)
        static List<ClienteConectado> listaClientes = new List<ClienteConectado>();

        // Objeto de bloqueo para asegurar que solo un hilo a la vez modifica el ID.
        static readonly object lockId = new object();
        static readonly object lockLista = new object();
        
        static void Main(string[] args)
        {
            // En esta etapa, modifico el servidor para aceptar múltiples clientes al mismo tiempo.
            int puerto = 13000;

            TcpListener servidor = new TcpListener(IPAddress.Any, puerto);
            servidor.Start();

            Console.WriteLine("Servidor iniciado. Esperando conexiones de clientes...");

            while (true)
            {
                TcpClient clienteConectado = servidor.AcceptTcpClient();
                Console.WriteLine("Cliente conectado.");

                Thread hiloCliente = new Thread(() => GestionarCliente(clienteConectado));
                hiloCliente.Start();
            }
        }

        static void GestionarCliente(TcpClient cliente)
        {
            // Obtengo el stream para comunicarme con el cliente.
            NetworkStream stream = cliente.GetStream();
            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] NetworkStream obtenido correctamente.");

            // Etapa 6: Espero el mensaje "INICIO" del cliente.
            string mensajeInicio = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            if (mensajeInicio != "INICIO")
            {
                Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Error: no se recibió 'INICIO'.");
                cliente.Close();
                return;
            }

            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Mensaje 'INICIO' recibido correctamente.");

            // Asigno ID y dirección como en etapas anteriores.
            int idAsignado;
            string direccion;
            lock (lockId)
            {
                idAsignado = contadorId;
                contadorId++;
            }

            direccion = new Random().Next(2) == 0 ? "Norte" : "Sur";

            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] ID asignado: {idAsignado} - Dirección: {direccion}");
            // Etapa 7: Añado el cliente a la lista global de clientes conectados
            lock (lockLista)
            {
                listaClientes.Add(new ClienteConectado(idAsignado, stream));
                Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Cliente añadido a la lista. Total conectados: {listaClientes.Count}");
            }

            // Envío el ID al cliente como respuesta.
            NetworkStreamClass.EscribirMensajeNetworkStream(stream, idAsignado.ToString());
            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] ID enviado al cliente.");

            // Espero la confirmación del mismo ID.
            string confirmacion = NetworkStreamClass.LeerMensajeNetworkStream(stream);
            if (confirmacion == idAsignado.ToString())
            {
                Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Confirmación recibida del cliente con ID: {confirmacion}");
            }
            else
            {
                Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Error: el cliente ha confirmado un ID incorrecto.");
                cliente.Close();
                return;
            }

            // Una vez confirmado, recibo el objeto Vehiculo.
            Vehiculo vehiculoRecibido = NetworkStreamClass.LeerDatosVehiculoNS(stream);

            // Muestro los datos del vehículo recibido por consola.
            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Vehículo recibido:");
            Console.WriteLine($"  ID: {vehiculoRecibido.Id}");
            Console.WriteLine($"  Dirección: {vehiculoRecibido.Direccion}");
            Console.WriteLine($"  Velocidad: {vehiculoRecibido.Velocidad}");
            Console.WriteLine($"  Posición inicial: {vehiculoRecibido.Pos}");
            Console.WriteLine($"  Acabado: {vehiculoRecibido.Acabado}");
            Console.WriteLine($"  Parado: {vehiculoRecibido.Parado}");

            // Etapa 2 (Ejercicio 2): Creo el objeto Carretera y añado el vehículo recibido
            Carretera carretera = new Carretera();
            carretera.AñadirVehiculo(vehiculoRecibido);

            // Envío el objeto Carretera al cliente usando la clase NetworkStreamClass
            NetworkStreamClass.EscribirDatosCarreteraNS(stream, carretera);
            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Objeto Carretera enviado al cliente con 1 vehículo.");

            

            cliente.Close();
        }
    }
}
