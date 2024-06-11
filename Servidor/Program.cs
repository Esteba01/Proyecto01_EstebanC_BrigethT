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
            TcpListener server = new TcpListener(IPAddress.Any, 8888); //Defino un socket para escuchar
            server.Start();
            Console.WriteLine("Servidor de mantenTuPeso.co");
            Console.WriteLine("Servidor iniciado...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        static void HandleClient(TcpClient client) //Cuando ya esta con un cliente
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (request.StartsWith("HISTORIAL"))
                {
                    string name = request.Split(',')[1];
                    string history = GetHistory(name);
                    byte[] responseBytes = Encoding.ASCII.GetBytes(history);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
                else
                {
                    string[] data = request.Split(',');
                    string date = data[0];
                    double weight = double.Parse(data[1]);
                    string weightUnit = data[2];
                    double height = double.Parse(data[3]);
                    string heightUnit = data[4];
                    string name = data[5];

                    if (weightUnit == "lb")
                        weight *= 0.453592; // Convert to kg
                    if (heightUnit == "in")
                        height *= 0.0254; // Convert to meters

                    double bmi = weight / (height * height);
                    string category = CategorizeBMI(bmi);

                    string response = $"{category},{bmi:F2}";
                    byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);

                    SaveHistory(name, date, weight, height, bmi, category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

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

        static void SaveHistory(string name, string date, double weight, double height, double bmi, string category)
        {
            string entry = $"{name},{date},{weight},{height},{bmi:F2},{category}";
            File.AppendAllText("historial.txt", entry + Environment.NewLine);
        }

        static string GetHistory(string name)
        {
            if (!File.Exists("historial.txt"))
                return "No existen registros previos de " + name;

            string[] lines = File.ReadAllLines("historial.txt");
            StringBuilder history = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.StartsWith(name))
                    history.AppendLine(line);
            }
            return history.ToString();
        }

    }
}