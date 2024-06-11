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
                int action = int.Parse(Console.ReadLine());//se obtiene la entrada del usuario y se convierte a un int con parse

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
                TcpClient cliente = new TcpClient("127.0.0.1", 8888);
                NetworkStream stream = cliente.GetStream();

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese la fecha (yyyy-mm-dd):");
                string fecha = Console.ReadLine();
                Console.WriteLine("Ingrese el peso:");
                double peso = double.Parse(Console.ReadLine());
                Console.WriteLine("Ingrese la unidad de peso (kg, lb):");
                string unidadpeso = Console.ReadLine();
                Console.WriteLine("Ingrese la altura:");
                double altura = double.Parse(Console.ReadLine());
                Console.WriteLine("Ingrese la unidad de altura (m, in):");
                string alturaunidad = Console.ReadLine();

                string mensaje = $"{nombre},{fecha},{peso},{unidadpeso},{altura},{alturaunidad}";//Delimitacion de campos por comas
                byte[] data = Encoding.ASCII.GetBytes(mensaje);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024]; 
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Respuesta del servidor: {response}");

                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void VerHistorial()
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 8888);
                NetworkStream stream = cliente.GetStream();

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine();

                string message = $"HISTORIAL,{nombre}";
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Historial:\n{response}");

                cliente.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
