using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Servidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crea un servidor TCP que escucha en todas las interfaces de red en el puerto 8888.
            TcpListener server = new TcpListener(IPAddress.Any, 8888); //Defino un socket para escucha con protocolo TCP
            server.Start();  // Inicia el servidor para comenzar a escuchar conexiones entrantes.
            Console.WriteLine("Servidor de mantenTuPeso.co");
            Console.WriteLine("Servidor iniciado...");

            //Bucle infinito para aceptar conexiones de clientes.
            while (true)
            {
                TcpClient client = server.AcceptTcpClient(); //Acepta una conexión de un cliente y crea un objeto TcpClient para manejarla.
                Thread clientThread = new Thread(() => HandleClient(client)); // Crea un nuevo hilo para manejar la conexión del cliente utilizando el método HandleClient.
                clientThread.Start();// Inicia el hilo del cliente.
            }
        }



        // Método que maneja la conexión con un cliente.
        static void HandleClient(TcpClient client) //Cuando ya esta con un cliente
        {
            try
            {
                NetworkStream stream = client.GetStream(); // Obtiene el flujo de datos de la conexión del cliente.
                byte[] buffer = new byte[1024];// crea un array de bytes (buffer) de tamaño 1024 para leer los datos del cliente.
                int bytesRead = stream.Read(buffer, 0, buffer.Length); // Lee los datos del cliente en el buffer. bytesRead contiene el número de bytes leídos.
                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convierte los datos del buffer en una cadena de texto.

                if (request.StartsWith("HISTORIAL")) // Comprueba si la solicitud empieza con "HISTORIAL".
                {
                    string name = request.Split(',')[1];// Divide la solicitud en partes usando ',' como separador y obtiene el nombre del usuario.
                    string history = GetHistory(name); // Obtiene el historial del usuario llamando al método GetHistory.
                    byte[] responseBytes = Encoding.ASCII.GetBytes(history); // Convierte el historial a un array de bytes.
                    stream.Write(responseBytes, 0, responseBytes.Length); // Envía el historial al cliente.
                }
                else // Si la solicitud no empieza con "HISTORIAL", se asume que es una solicitud para calcular el BMI.
                {
                    string[] data = request.Split(','); // Divide la solicitud en partes usando ',' como separador.
                    string date = data[0]; // Obtiene la fecha de la solicitud.
                    double weight = double.Parse(data[1]); // Convierte el peso de cadena a double.
                    string weightUnit = data[2]; // Obtiene la unidad de medida del peso.
                    double height = double.Parse(data[3]); // Convierte la altura de cadena a double.
                    string heightUnit = data[4]; // Obtiene la unidad de medida de la altura.
                    string name = data[5]; // Obtiene el nombre del usuario.

                    if (weightUnit == "lb") // Comprueba si la unidad de peso es libras.
                        weight *= 0.453592; // Convierte el peso de libras a kilogramos.
                    if (heightUnit == "in") // Comprueba si la unidad de altura es pulgadas.
                        height *= 0.0254; // Convierte la altura de pulgadas a metros.

                    double bmi = weight / (height * height); // Calcula el BMI usando la fórmula peso/altura^2.
                    string category = CategorizeBMI(bmi); // Clasifica el BMI en una categoría llamando al método CategorizeBMI.

                    string response = $"{category},{bmi:F2}"; // Crea una respuesta con la categoría y el BMI formateado a 2 decimales.
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response); // Convierte la respuesta a un array de bytes.
                    stream.Write(responseBytes, 0, responseBytes.Length); // Envía la respuesta al cliente.

                    SaveHistory(name, date, weight, height, bmi, category); // Guarda el historial del usuario llamando al método SaveHistory.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close(); // Cierra la conexión del cliente.
            }
        }


        // Método que clasifica el BMI en categorías.
        static string CategorizeBMI(double bmi) //Metodo que evalua el BMI
        {
            if (bmi < 18.5)
                return "Peso inferior al normal";
            else if (bmi < 25.0)
                return "Normal";
            else if (bmi < 30.0)
                return "Peso superior al normal";
            else
                return "Obesidad";
        }



        // Método que guarda el historial del usuario en un archivo.
        static void SaveHistory(string name, string date, double weight, double height, double bmi, string category)
        {
            string entry = $"{name},{date},{weight},{height},{bmi:F2},{category}";string entry = $"{name},{date},{weight},{height},{bmi:F2},{category}"; // Crea una entrada con los datos del usuario. (interpolación de cadenas para crear una cadena de texto )
            File.AppendAllText("historial.txt", entry + Environment.NewLine);// Añade la entrada al archivo historial.txt, añadiendo una nueva línea al final.
        }


        // Método que obtiene el historial del usuario desde un archivo.
        static string GetHistory(string name)
        {
            if (!File.Exists("historial.txt"))// Comprueba si el archivo historial.txt no existe.
                return "No existen registros previos de " + name;// Si no existe, retorna un mensaje indicando que no hay registros.

            string[] lines = File.ReadAllLines("historial.txt"); //Lee todas las líneas del archivo historial.txt en un array de cadenas
            StringBuilder history = new StringBuilder(); // Crea un objeto StringBuilder para construir el historial.
            foreach (string line in lines) // Itera sobre cada línea del archivo.
            {
                if (line.StartsWith(name)) // Si la línea empieza con el nombre del usuario.
                    history.AppendLine(line);// Añade la línea al objeto StringBuilder.
            }
            return history.ToString(); // Convierte el objeto StringBuilder a una cadena y la retorna.
        }

    }
}