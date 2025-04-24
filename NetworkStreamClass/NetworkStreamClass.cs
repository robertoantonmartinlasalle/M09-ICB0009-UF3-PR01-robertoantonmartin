using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        // Método para escribir en un NetworkStream los datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            // Serializo el objeto Carretera a un array de bytes
            byte[] datos = C.CarreteraABytes();

            // Envío primero los 4 bytes que indican la longitud del mensaje
            byte[] longitud = BitConverter.GetBytes(datos.Length);
            NS.Write(longitud, 0, longitud.Length);

            // Luego envío el contenido real de la carretera
            NS.Write(datos, 0, datos.Length);
        }

        // Método para leer de un NetworkStream los datos de un objeto Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            // Primero leo los 4 bytes iniciales que me indican la longitud del mensaje
            byte[] longitudBuffer = new byte[4];
            int leidosLongitud = NS.Read(longitudBuffer, 0, 4);
            int longitudDatos = BitConverter.ToInt32(longitudBuffer, 0);

            // Ahora sé cuántos bytes exactamente tengo que leer
            byte[] buffer = new byte[longitudDatos];
            int totalLeidos = 0;

            while (totalLeidos < longitudDatos)
            {
                int leidos = NS.Read(buffer, totalLeidos, longitudDatos - totalLeidos);
                totalLeidos += leidos;
            }

            // Devuelvo el objeto deserializado
            return Carretera.BytesACarretera(buffer);
        }

        // Método para enviar datos de tipo Vehiculo en un NetworkStream
        public static void EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {
            byte[] datos = V.VehiculoaBytes();
            NS.Write(datos, 0, datos.Length);
        }

        // Método para leer de un NetworkStream los datos de un objeto Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            byte[] buffer = new byte[2048];
            int totalBytes = 0;
            MemoryStream tmpStream = new MemoryStream();

            do
            {
                int bytesLeidos = NS.Read(buffer, 0, buffer.Length);
                tmpStream.Write(buffer, 0, bytesLeidos);
                totalBytes += bytesLeidos;
            }
            while (NS.DataAvailable);

            byte[] datosRecibidos = tmpStream.ToArray();
            return Vehiculo.BytesAVehiculo(datosRecibidos);
        }

        // Método que permite leer un mensaje de tipo texto (string) de un NetworkStream
        public static string LeerMensajeNetworkStream(NetworkStream NS)
        {
            byte[] bufferLectura = new byte[1024];
            int bytesLeidos = 0;
            var tmpStream = new MemoryStream();

            do
            {
                int bytesLectura = NS.Read(bufferLectura, 0, bufferLectura.Length);
                tmpStream.Write(bufferLectura, 0, bytesLectura);
                bytesLeidos += bytesLectura;
            }
            while (NS.DataAvailable);

            byte[] bytesTotales = tmpStream.ToArray();
            return Encoding.Unicode.GetString(bytesTotales, 0, bytesLeidos);
        }

        // Método que permite escribir un mensaje de tipo texto (string) al NetworkStream
        public static void EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {
            byte[] MensajeBytes = Encoding.Unicode.GetBytes(Str);
            NS.Write(MensajeBytes, 0, MensajeBytes.Length);
        }
    }
}
