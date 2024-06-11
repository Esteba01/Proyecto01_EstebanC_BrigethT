//Importación de namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Cliente
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(" --------- mantenTuPeso.co GYM --------- \n\n"); //Mensaje de bienvenida
                Console.WriteLine(" Ingrese la acción que desea solicitar:\n");
                Console.WriteLine(" 1. Calcular IMC       2. Ver Historial");//Opciones de peticion
                int action = int.Parse(Console.ReadLine()); // Lee la entrada del usuario con Parse convierte una cadena de texto(string) en un entero (int) y la almacena en la variable action

                Console.Clear(); //Se limpia la consola 

                if (action == 1)
                {
                    CalculoIMC();
                }
                else if (action == 2)
                {
                    VerHistorial();
                }
                else
                {
                    Console.WriteLine("Opción no existente");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\n   Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        static void CalculoIMC()
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 8888); // Crea un cliente TCP que se conecta al servidor en la dirección IP 127.0.0.1 (localhost) y el puerto 8888.
                NetworkStream stream = cliente.GetStream();// Obtiene el flujo de datos(NEtworStream, metodo para enviar y recibir datos) de la conexión del cliente.

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese la fecha (yyyy-mm-dd):");
                string fecha = Console.ReadLine();
                Console.WriteLine("Ingrese el peso:");
                double peso = double.Parse(Console.ReadLine()); //Lee el peso desde la consola y lo convierte a double.
                Console.WriteLine("Ingrese la unidad de peso (kg, lb):");
                string unidadpeso = Console.ReadLine();
                Console.WriteLine("Ingrese la altura:");
                double altura = double.Parse(Console.ReadLine()); // Lee la altura desde la consola y la convierte a double.
                Console.WriteLine("Ingrese la unidad de altura (m, in):");
                string alturaunidad = Console.ReadLine();

                string mensaje = $"{nombre},{fecha},{peso},{unidadpeso},{altura},{alturaunidad}"; // Crea un mensaje con los datos del usuario, separados por comas.
                byte[] data = Encoding.ASCII.GetBytes(mensaje); // Convierte el mensaje a un array de bytes usando codificación ASCII.
                stream.Write(data, 0, data.Length); // Envía los datos al servidor a través del flujo de datos.

                byte[] buffer = new byte[1024];  // Crea un buffer de 1024 bytes para recibir datos del servidor.
                int bytesRead = stream.Read(buffer, 0, buffer.Length); // Lee los datos enviados por el servidor en el buffer. bytesRead contiene el número de bytes leídos.
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convierte los datos leídos a una cadena de texto.

                Console.WriteLine($"Respuesta del servidor: {response}");

                cliente.Close(); // Cierra la conexión del cliente.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


         // Define el método VerHistorial.
        static void VerHistorial()
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 8888); // Crea un cliente TCP que se conecta al servidor en la dirección IP 127.0.0.1 (localhost) y el puerto 8888.
                NetworkStream stream = cliente.GetStream(); // Obtiene el flujo de datos de la conexión del cliente.

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine();

                string message = $"HISTORIAL,{nombre}"; // Crea un mensaje solicitando el historial, con el nombre del usuario.
                byte[] data = Encoding.ASCII.GetBytes(message); // Convierte el mensaje a un array de bytes usando codificación ASCII.
                stream.Write(data, 0, data.Length);  // Envía los datos al servidor a través del flujo de datos.

                byte[] buffer = new byte[1024]; // Crea un buffer de 1024 bytes para recibir datos del servidor.
                int bytesRead = stream.Read(buffer, 0, buffer.Length);// Lee los datos enviados por el servidor en el buffer. bytesRead contiene el número de bytes leídos.
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convierte los datos leídos a una cadena de texto.


                Console.WriteLine($"Historial:\n{response}"); // Imprime el historial recibido del servidor en la consola.

                cliente.Close(); // Cierra la conexión del cliente.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
