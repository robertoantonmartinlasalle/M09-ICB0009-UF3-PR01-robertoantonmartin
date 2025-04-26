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

                // Establezco la posición inicial en función de la dirección
                v.Pos = v.Direccion == "Norte" ? 0 : 100;

                // Muestro por pantalla la velocidad generada y dirección para tener contexto
                Console.WriteLine($"→ Dirección asignada: {v.Direccion}");
                Console.WriteLine($"→ Velocidad del vehículo: {v.Velocidad} ms entre cada paso.");

                // Etapa 3, 5 y 6: simulo el avance del vehículo en bucle y recibo carretera
                while (!v.Acabado)
                {
                    // Avanzo el vehículo manualmente de 10 en 10 km según su dirección
                    if (v.Direccion == "Norte")
                    {
                        v.Pos += 10;
                        if (v.Pos >= 100)
                        {
                            v.Pos = 100;
                            v.Acabado = true;
                        }
                    }
                    else // Dirección Sur
                    {
                        v.Pos -= 10;
                        if (v.Pos <= 0)
                        {
                            v.Pos = 0;
                            v.Acabado = true;
                        }
                    }

                    // Antes de avanzar, controlo si estamos cerca del puente
                    if ((v.Direccion == "Norte" && v.Pos == 50) || (v.Direccion == "Sur" && v.Pos == 50))
                    {
                        Console.WriteLine($"→ Intentando entrar al puente...");

                        bool puedeCruzar = false;

                        // Hasta que pueda pasar, me mantengo preguntando
                        while (!puedeCruzar)
                        {
                            // Envío mi estado actual al servidor
                            NetworkStreamClass.EscribirDatosVehiculoNS(stream, v);

                            // Recibo la carretera para verificar
                            Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(stream);

                            // Busco si soy el que está autorizado a cruzar
                            Vehiculo yo = carreteraRecibida.VehiculosEnCarretera.Find(veh => veh.Id == v.Id);

                            if (yo != null && (yo.Pos == 50 || yo.Pos == 51))
                            {
                                // Me dejan cruzar
                                puedeCruzar = true;
                                Console.WriteLine("→ Acceso permitido: cruzando el puente.");
                            }
                            else
                            {
                                Console.WriteLine("→ Puente ocupado. Esperando turno...");
                                Thread.Sleep(500); // Espero medio segundo antes de volver a intentar
                            }
                        }
                    }
                    else
                    {
                        // Envío el estado actualizado del vehículo al servidor
                        NetworkStreamClass.EscribirDatosVehiculoNS(stream, v);

                        Console.WriteLine($"\n→ Vehículo enviado al servidor - Posición: {v.Pos} km");

                        // Recibo el objeto Carretera completo actualizado desde el servidor
                        Carretera carreteraRecibida = NetworkStreamClass.LeerDatosCarreteraNS(stream);

                        // Ordeno la lista por posición para mayor claridad visual
                        carreteraRecibida.VehiculosEnCarretera.Sort((a, b) => a.Pos.CompareTo(b.Pos));

                        // Muestro el estado actual de la carretera
                        Console.WriteLine("[Actualización desde servidor] Vehículos en carretera:");
                        foreach (Vehiculo veh in carreteraRecibida.VehiculosEnCarretera)
                        {
                            Console.WriteLine($"  → ID: {veh.Id} | Dirección: {veh.Direccion} | Posición: {veh.Pos} km");
                        }

                        // Espero un tiempo proporcional a la velocidad
                        Thread.Sleep(v.Velocidad); // ← Tiempo entre pasos según velocidad
                    }
                }

                Console.WriteLine("✓ Vehículo ha llegado a destino. Fin de la simulación.");
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
