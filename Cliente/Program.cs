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

                // Asigno dirección aleatoria al vehículo
                string direccion = new Random().Next(2) == 0 ? "Norte" : "Sur";

                // Creo el objeto Vehiculo con los datos reales.
                Vehiculo v = new Vehiculo();
                v.Id = id;
                v.Direccion = direccion;

                // Fuerzo la posición inicial a 0 para evitar avanzar de golpe
                v.Pos = 0;

                // Muestro por pantalla la velocidad generada para tener contexto
                Console.WriteLine($"→ Velocidad del vehículo: {v.Velocidad} ms entre cada paso.");

                // Etapa 3 y 5: simulo el avance del vehículo en bucle y recibo carretera
                while (!v.Acabado)
                {
                    // Avanzo el vehículo manualmente de 10 en 10 km
                    v.Pos += 10;

                    // Si llega a los 100 km, marco como acabado
                    if (v.Pos >= 100)
                    {
                        v.Pos = 100;
                        v.Acabado = true;
                    }

                    // Envío el estado actualizado del vehículo al servidor
                    NetworkStreamClass.EscribirDatosVehiculoNS(stream, v);
                    Console.WriteLine($"\n→ Vehículo enviado al servidor - Posición: {v.Pos} km");

                    // Recibo el objeto Carretera completo actualizado desde el servidor
                    Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(stream);

                    // Ordeno la lista por posición para mayor claridad visual
                    carreteraRecibida.VehiculosEnCarretera.Sort((a, b) => a.Pos.CompareTo(b.Pos));

                    // Muestro el estado actual de la carretera
                    Console.WriteLine("[Actualización desde servidor] Vehículos en carretera (posición):");
                    carreteraRecibida.MostrarBicicletas();

                    // Espero un tiempo proporcional a la velocidad
                    Thread.Sleep(v.Velocidad); // ← Tiempo entre pasos según velocidad
                }

                Console.WriteLine(" Vehículo ha llegado a destino. Fin de la simulación.");
            }
            catch (Exception e)
            {
                Console.WriteLine(" Error: " + e.Message);
            }

            Console.WriteLine("Pulsa ENTER para cerrar el cliente.");
            Console.ReadLine();
            cliente.Close();
        }
    }
}
