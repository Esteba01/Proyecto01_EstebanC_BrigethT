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
                Console.WriteLine("Ingrese la acción (1. Calcular IMC, 2. Ver Historial):");
                int action = int.Parse(Console.ReadLine());

                if (action == 1)
                {
                    CalculateIMC();
                }
                else if (action == 2)
                {
                    ViewHistory();
                }
                else
                {
                    Console.WriteLine("Acción no válida.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
            }
        }

        static void CalculateIMC()
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 8888);
                NetworkStream stream = client.GetStream();

                Console.WriteLine("Ingrese su nombre:");
                string name = Console.ReadLine();
                Console.WriteLine("Ingrese la fecha (yyyy-mm-dd):");
                string date = Console.ReadLine();
                Console.WriteLine("Ingrese el peso:");
                double weight = double.Parse(Console.ReadLine());
                Console.WriteLine("Ingrese la unidad de peso (kg, lb):");
                string weightUnit = Console.ReadLine();
                Console.WriteLine("Ingrese la altura:");
                double height = double.Parse(Console.ReadLine());
                Console.WriteLine("Ingrese la unidad de altura (m, in):");
                string heightUnit = Console.ReadLine();

                string message = $"{date},{weight},{weightUnit},{height},{heightUnit},{name}";
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Respuesta del servidor: {response}");

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ViewHistory()
        {
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 8888);
                NetworkStream stream = client.GetStream();

                Console.WriteLine("Ingrese su nombre:");
                string name = Console.ReadLine();

                string message = $"HISTORIAL,{name}";
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"Historial:\n{response}");

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
