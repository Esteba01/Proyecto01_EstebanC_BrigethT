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

            TcpListener server = new TcpListener(IPAddress.Any, 8080); //Se define un server tipo TcpListener, se utiliza el puerto 8080. Este sera el que escucha petiicones
            server.Start(); //Se empiezan a escuchar las peticiones en el puerto designado
            Console.WriteLine("Servidor de mantenTuPeso.co");//Mensajes de bienvenida por parte del servidor
            Console.WriteLine("Bienvenido...");
            Console.WriteLine("Servidor iniciado...\n\n");

             //Bucle infinito para aceptar conexiones de clientes.
            while (true)
            {
                TcpClient client = server.AcceptTcpClient(); //Acepta una conexión de un cliente y crea un objeto TcpClient para manejarla.
                Thread clientThread = new Thread(() => HandleClient(client)); // Crea un nuevo hilo para manejar la conexión del cliente utilizando el método HandleClient.
                clientThread.Start();// Inicia el hilo del cliente.
            }
        }

        static void HandleClient(TcpClient client) 

        {
            try
            {
                NetworkStream stream = client.GetStream(); // Obtiene el flujo de datos de la conexión del cliente.
                byte[] buffer = new byte[1024];// crea un array de bytes (buffer) de tamaño 1024 para leer los datos del cliente.
                int bytesRead = stream.Read(buffer, 0, buffer.Length); // Lee los datos del cliente en el buffer. bytesRead contiene el número de bytes leídos.
                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convierte los datos del buffer en una cadena de texto.

                Console.WriteLine($"Mensaje recibido de un cliente: {request}"); 

                if (request.StartsWith("HISTORIAL")) // Comprueba si la solicitud empieza con "HISTORIAL".
                {
                    string nombre = request.Split(',')[1];// Divide la solicitud en partes usando ',' como separador y obtiene el nombre del usuario.
                    string history = ObtenerHistorial(nombre); // Obtiene el historial del usuario llamando al método GetHistory.
                    byte[] responseBytes = Encoding.ASCII.GetBytes(history); // Convierte el historial a un array de bytes.
                    stream.Write(responseBytes, 0, responseBytes.Length); // Envía el historial al cliente.
                    Console.WriteLine($"Historial enviado al cliente: {history}"); // Muestra el historial enviado en la consola.


                }
                else // Si la solicitud no empieza con "HISTORIAL", se asume que es una solicitud para calcular el BMI.
                {
                    string[] data = request.Split(','); // Divide la solicitud en partes usando ',' como separador.
                    string nombre = data[0]; // Obtiene el nombre del usuario
                    string fecha = data[1]; // Obtiene la fecha de la solicitud.
                    string pesounidad = data[2]; // Convierte el peso de cadena a double.
                    double peso = double.Parse(data[3]); // Obtiene la unidad de medida del peso.
                    string alturaunidad = data[4]; // Convierte la altura de cadena a double.
                    double altura = double.Parse(data[5]); // Obtiene la unidad de medida de la altura.

                    // Convertir peso a kilogramos
                    switch (pesounidad)
                    {
                        case "g":
                            peso /= 1000;
                            break;
                        case "lb":
                            peso *= 0.453592;
                            break;
                        case "oz":
                            peso *= 0.0283495;
                            break;
                    }

                    // Convertir altura a metros
                    switch (alturaunidad)
                    {
                        case "cm":
                            altura /= 100;
                            break;
                        case "plg":
                            altura *= 0.0254;
                            break;
                        case "ft":
                            altura *= 0.3048;
                            break;
                    }

                    double bmi = peso / (altura * altura); // Calcula el BMI usando la fórmula peso/altura^2.
                    string category = CategoriaBMI(bmi); // Clasifica el BMI en una categoría llamando al método CategorizeBMI.

                    string response = $"{category},{bmi:F2}"; // Crea una respuesta con la categoría y el BMI formateado a 2 decimales.
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response); // Convierte la respuesta a un array de bytes.
                    stream.Write(responseBytes, 0, responseBytes.Length); // Envía la respuesta al cliente.

                    Console.WriteLine($"Resultado del IMC enviado al cliente: {response}"); // Muestra el resultado del IMC enviado en la consola.

                    GardarHistorial(nombre, fecha, peso, altura, bmi, category); // Guarda el historial del usuario llamando al método SaveHistory.

                    Console.WriteLine("Registro guardado");
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
        static string CategoriaBMI(double bmi) //Metodo que evalua el BMI
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
        static void GardarHistorial(string nombre, string fecha, double peso, double altura, double bmi, string categoria)
        {
            string entry = $"{nombre},{fecha},{peso},{altura},{bmi:F2},{categoria}"; // Crea una entrada con los datos del usuario. (interpolación de cadenas para crear una cadena de texto )
            File.AppendAllText("historial.txt", entry + Environment.NewLine);// Añade la entrada al archivo historial.txt, añadiendo una nueva línea al final.
        }
            

        // Método que obtiene el historial del usuario desde un archivo.
        static string ObtenerHistorial(string nombre)
        {
            if (!File.Exists("historial.txt"))// Comprueba si el archivo historial.txt no existe.
                return "No existen registros previos de " + nombre;

            string[] lines = File.ReadAllLines("historial.txt"); //Lee todas las líneas del archivo historial.txt en un array de cadenas
            StringBuilder history = new StringBuilder(); // Crea un objeto StringBuilder para construir el historial.
            foreach (string line in lines) // Itera sobre cada línea del archivo.
            {
                if (line.StartsWith(nombre)) // Si la línea empieza con el nombre del usuario.
                    history.AppendLine(line);// Añade la línea al objeto StringBuilder.
            }
            return history.ToString(); // Convierte el objeto a ToString a una cadena y la retorna.
        }

    }
}