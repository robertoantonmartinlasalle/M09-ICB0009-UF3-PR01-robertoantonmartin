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
        // En esta etapa simplemente muestro un mensaje de que el cliente está siendo atendido,
        // y cierro la conexión porque todavía no intercambiamos datos.
        static void GestionarCliente(TcpClient cliente)
        {
            Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Cliente atendido.");
            cliente.Close();
        }

    }
}

