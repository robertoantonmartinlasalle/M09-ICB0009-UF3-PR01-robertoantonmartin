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
            // En esta primera etapa inicializo el servidor para escuchar en el puerto 13000.
            int puerto = 13000;

            // TcpListener escucha conexiones entrantes en cualquier dirección IP local.
            TcpListener servidor = new TcpListener(IPAddress.Any, puerto);
            servidor.Start();

            Console.WriteLine("Servidor iniciado. Esperando conexiones de clientes...");

            // El servidor se queda bloqueado esperando que se conecte un cliente.
            TcpClient clienteConectado = servidor.AcceptTcpClient();
            Console.WriteLine("Cliente conectado correctamente.");

            // Mantengo la consola abierta para observar que todo funciona.
            Console.WriteLine("Pulsa ENTER para cerrar el servidor.");
            Console.ReadLine();

            // Cierro conexiones y detengo el servidor.
            clienteConectado.Close();
            servidor.Stop();
        }

    }
}

