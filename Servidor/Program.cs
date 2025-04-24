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

        // Etapa 3 - Ejercicio 2: Creo una carretera global para simular varios vehículos en movimiento.
        static Carretera carreteraGlobal = new Carretera();

        // Objetos de bloqueo para concurrencia segura
        static readonly object lockId = new object();
        static readonly object lockLista = new object();
        static readonly object lockCarretera = new object();

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

        // Etapa 4 - Ejercicio 2: Envío del objeto Carretera actualizado a todos los clientes conectados
        static void EnviarCarreteraATodos(Carretera carretera)
        {
            lock (lockLista)
            {
                // Lista temporal para guardar los clientes desconectados
                List<ClienteConectado> clientesDesconectados = new List<ClienteConectado>();

                foreach (var cliente in listaClientes)
                {
                    try
                    {
                        NetworkStreamClass.EscribirDatosCarreteraNS(cliente.Stream, carretera);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"[Servidor] Error al enviar carretera al cliente con ID: {cliente.Id}. Lo marcaré como desconectado.");
                        clientesDesconectados.Add(cliente);
                    }
                    catch (ObjectDisposedException)
                    {
                        Console.WriteLine($"[Servidor] El stream del cliente {cliente.Id} ya estaba cerrado. Eliminando.");
                        clientesDesconectados.Add(cliente);
                    }
                }

                // Eliminamos todos los clientes cuyos streams fallaron
                foreach (var desconectado in clientesDesconectados)
                {
                    listaClientes.Remove(desconectado);
                }

                if (clientesDesconectados.Count > 0)
                {
                    Console.WriteLine($"[Servidor] {clientesDesconectados.Count} cliente(s) eliminado(s) de la lista.");
                }
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

            // Una vez confirmado, empiezo a recibir actualizaciones del vehículo en bucle
            while (true)
            {
                try
                {
                    // Recibo el vehículo actualizado desde el cliente
                    Vehiculo vehiculoRecibido = NetworkStreamClass.LeerDatosVehiculoNS(stream);

                    // Etapa 3: Actualizo la carretera global con el nuevo estado del vehículo
                    lock (lockCarretera)
                    {
                        carreteraGlobal.ActualizarVehiculo(vehiculoRecibido);
                    }

                    // Etapa 4: Envío el estado de la carretera a todos los clientes conectados
                    EnviarCarreteraATodos(carreteraGlobal);

                    if (vehiculoRecibido.Pos >= 100)
                    {
                        Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] El vehículo {vehiculoRecibido.Id} ha llegado a destino.");
                        break;
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine($"[Hilo {Thread.CurrentThread.ManagedThreadId}] Error: el cliente se ha desconectado inesperadamente.");
                    break;
                }
            }

            cliente.Close();
        }
    }
}
