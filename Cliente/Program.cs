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

                // Etapa 4: obtengo el stream de red para comunicarme.
                NetworkStream stream = cliente.GetStream();

                // Etapa 6: Envío el mensaje "INICIO" para indicar al servidor que estoy listo.
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, "INICIO");
                Console.WriteLine("Mensaje 'INICIO' enviado al servidor.");

                // Espero la respuesta del servidor que debe ser el ID asignado.
                string respuestaServidor = NetworkStreamClass.LeerMensajeNetworkStream(stream);
                int id = int.Parse(respuestaServidor);
                Console.WriteLine($"ID recibido desde el servidor: {id}");

                // Envío de nuevo el ID para confirmar recepción (handshake).
                NetworkStreamClass.EscribirMensajeNetworkStream(stream, id.ToString());
                Console.WriteLine("ID confirmado al servidor.");

                // Asigno dirección aleatoria al vehículo (aunque no sea necesaria para esta etapa, ya la preparo).
                string direccion = new Random().Next(2) == 0 ? "Norte" : "Sur";

                // Creo el objeto Vehiculo con los datos reales.
                Vehiculo v = new Vehiculo();
                v.Id = id;
                v.Direccion = direccion;

                // Envío el objeto serializado al servidor.
                NetworkStreamClass.EscribirDatosVehiculoNS(stream, v);
                Console.WriteLine("Vehículo enviado al servidor.");

                // Etapa 2 Ejercicio 2: Recibo el objeto Carretera completo desde el servidor
                Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(stream);
                Console.WriteLine("Carretera recibida correctamente del servidor.");

                // Muestro el estado de la carretera (posiciones de los vehículos)
                Console.Write("Vehículos en carretera (posición): ");
                carreteraRecibida.MostrarBicicletas();
            }
            catch (Exception e)
            {
                Console.WriteLine("No se pudo conectar al servidor: " + e.Message);
            }

            Console.WriteLine("Pulsa ENTER para cerrar el cliente.");
            Console.ReadLine();
            cliente.Close();
        }
    }
}
