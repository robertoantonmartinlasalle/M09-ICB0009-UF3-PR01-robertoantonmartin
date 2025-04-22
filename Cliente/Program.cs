using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Client
{
    class Program
    {

        static void Main(string[] args)
        {
            // En esta etapa me conecto al servidor que debe estar escuchando en localhost y puerto 13000.
            TcpClient cliente = new TcpClient();

            try
            {
                cliente.Connect("127.0.0.1", 13000);
                Console.WriteLine("Conectado al servidor correctamente.");
            }
            catch (Exception e)
            {
                // Si hay algún problema, lo informo por consola.
                Console.WriteLine("No se pudo conectar al servidor: " + e.Message);
            }

            // Espero a que el usuario pulse ENTER para cerrar.
            Console.WriteLine("Pulsa ENTER para cerrar el cliente.");
            Console.ReadLine();
            cliente.Close();
        }


    }
}