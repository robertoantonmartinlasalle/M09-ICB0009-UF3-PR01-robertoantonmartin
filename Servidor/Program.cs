using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Servidor
{
    class Program
    {
        // Contador global de IDs que se autoincrementará para cada cliente.
        static int contadorId = 1;

        // Objeto de bloqueo para asegurar que solo un hilo a la vez modifica el ID.
        static readonly object lockId = new object();

        static void Main(string[] args)
        {
            // En esta etapa, modifico el servidor para aceptar múltiples clientes al mismo tiempo.
            int puerto = 13000;

            // TcpListener escucha conexiones entrantes en cualquier dirección IP local.
            TcpListener servidor = new TcpListener(IPAddress.Any, puerto);
            servidor.Start();

            Console.WriteLine("Servidor iniciado. Esperando conexiones de clientes...");

            // Con este bucle, dejo el servidor siempre escuchando y aceptando nuevos clientes.
            while (true)
            {
                // Acepto la conexión del cliente.
                TcpClient clienteConectado = servidor.AcceptTcpClient();
                Console.WriteLine("Cliente conectado.");

                // Por cada cliente conectado, creo un nuevo hilo que lo atienda de forma independiente.
                Thread hiloCliente = new Thread(() => GestionarCliente(clienteConectado));
                hiloCliente.Start();
            }
        }

        // Este método será ejecutado por cada hilo individual.
        // Ahora asigno un ID único y una dirección aleatoria al cliente.
        static void GestionarCliente(TcpClient cliente)
        {
            int idAsignado;
            string direccion;

            // Bloqueo para que los hilos no se pisen al asignar ID.
            lock (lockId)
            {
                idAsignado = contadorId;
                contadorId++;
            }

            // Genero dirección aleatoria para el vehículo (Norte o Sur).
            Random rnd = new Random();
            direccion = rnd.Next(2) == 0 ? "Norte" : "Sur";

            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Vehículo conectado. ID asignado: {idAsignado} - Dirección: {direccion}");

            cliente.Close();
        }
    }
}
