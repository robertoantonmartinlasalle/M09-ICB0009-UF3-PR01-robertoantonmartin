using System.Xml.Serialization;
using VehiculoClass;

namespace CarreteraClass;

[Serializable]
public class Carretera
{
    public List<Vehiculo> VehiculosEnCarretera = new List<Vehiculo>();
    public int NumVehiculosEnCarrera = 0;

    public Carretera ()
    {
    }

    // Crea un nuevo vehiculo
    public void CrearVehiculo ()
    {
        Vehiculo V = new Vehiculo();
        VehiculosEnCarretera.Add(V);
    }

    // Añade un vehiculo ya creado a la lista de vehiculos en carretera
    public void AñadirVehiculo (Vehiculo V)
    {
        VehiculosEnCarretera.Add(V);
        NumVehiculosEnCarrera++;
    }

    // Actualiza los datos de un vehiculo ya existente en la lista de vehiculos en carretera. 
    public void ActualizarVehiculo(Vehiculo V)
    {
        Vehiculo veh = VehiculosEnCarretera.FirstOrDefault(x => x.Id == V.Id);

        if (veh != null)
        {
            veh.Pos = V.Pos;
            veh.Velocidad = V.Velocidad;
            veh.Acabado = V.Acabado;
            veh.Parado = V.Parado;

            // ✅ Se añade la actualización de la dirección para mantener la lógica Norte-Sur
            veh.Direccion = V.Direccion;
        }
        else
        {
            VehiculosEnCarretera.Add(V);
            NumVehiculosEnCarrera++;
        }
    }

    // Muestra por pantalla los vehículos en carretera con dirección y posición
    public void MostrarBicicletas()
    {
        if (VehiculosEnCarretera.Count == 0)
        {
            Console.WriteLine("(No hay vehículos en carretera)");
            return;
        }

        foreach (Vehiculo v in VehiculosEnCarretera)
        {
            Console.WriteLine($"  → ID: {v.Id} | Dirección: {v.Direccion} | Posición: {v.Pos} km");
        }
    }

    // Permite serializar Carretera a array de bytes mediante formato XML
    public byte[] CarreteraABytes()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
        MemoryStream MS = new MemoryStream();
        serializer.Serialize(MS, this);
        return MS.ToArray();
    }

    // Permite deserializar una cadena de bytes a un objeto de tipo Carretera
    public static Carretera BytesACarretera(byte[] bytesCarrera)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Carretera));
        MemoryStream MS = new MemoryStream(bytesCarrera);
        return (Carretera)serializer.Deserialize(MS);
    }
}
