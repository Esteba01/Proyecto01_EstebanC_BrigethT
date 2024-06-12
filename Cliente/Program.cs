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

            bool continuar = true;

            while (continuar)
            {

                try
                {
                    Console.Clear();
                    Console.WriteLine(" --------- mantenTuPeso.co GYM --------- "); //Mensaje de bienvenida
                    Console.WriteLine(" ------------- Bienvenido ---------- \n\n");

                    int action = 0;
                    while (action != 1 && action != 2)
                    {
                        Console.WriteLine(" Ingrese la acción que desea solicitar:\n");
                        Console.WriteLine(" 1. Calcular IMC       2. Ver Historial");

                        if (!int.TryParse(Console.ReadLine(), out action) || (action != 1 && action != 2))
                        {
                            Console.WriteLine("Opción no existente, por favor ingrese una opción válida.");
                            action = 0; // Reset action to continue the loop
                        }
                    }



                    Console.Clear(); //Se limpia la consola 

                    if (action == 1)
                    {
                        CalculoIMC();
                    }
                    else if (action == 2)
                    {
                        VerHistorial();
                    }

                    continuar = PreguntarContinuar(); //Llama al metodo para preguntar si quiere continuar

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    continuar = PreguntarContinuar();
                }
            }    
        }

        static void CalculoIMC()
        {
            try
            {

                TcpClient cliente = new TcpClient("127.0.0.1", 8080); // Crea un cliente TCP que se conecta al servidor en la dirección IP 127.0.0.1 (localhost) y el puerto 8888.
                NetworkStream stream = cliente.GetStream();// Obtiene el flujo de datos(NEtworStream, metodo para enviar y recibir datos) de la conexión del cliente.

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine(); //Almacenamiento de nombre del cliente
                string fecha;
                while (true)
                {
                    Console.WriteLine("Ingrese la fecha (yyyy-mm-dd):");
                    fecha = Console.ReadLine();
                    if (DateTime.TryParse(fecha, out DateTime parsedFecha))
                    {
                        if (parsedFecha.Date == DateTime.Now.Date) //Se verifica que la fecha coincida con la del sistema
                            break;
                        else
                            Console.WriteLine("Fecha no corresponde al día actual, por favor ingrese nuevamente.");
                    }
                    else
                    {
                        Console.WriteLine("Formato de fecha incorrecto, por favor ingrese nuevamente.");
                    }
                }

                string unidadpeso;
                while (true)
                {
                    Console.WriteLine("Ingrese la unidad de peso (g, kg, lb, oz):");//Se pregunta que unidadd e peso utilizará
                    unidadpeso = Console.ReadLine();
                    if (unidadpeso == "g" || unidadpeso == "kg" || unidadpeso == "lb" || unidadpeso == "oz")
                        break;
                    else
                        Console.WriteLine("Valor no existente, por favor ingrese una unidad de peso válida.");
                }
 
                Console.WriteLine("Ingrese el peso:");//Ingreso del peso
                double peso = double.Parse(Console.ReadLine());


                string unidadaltura;
                while (true)
                {
                    Console.WriteLine("Ingrese la unidad de altura (cm, m, plg, ft):");
                    unidadaltura = Console.ReadLine();
                    if (unidadaltura == "cm" || unidadaltura == "m" || unidadaltura == "plg" || unidadaltura == "ft")
                        break;
                    else
                        Console.WriteLine("Valor no existente, por favor ingrese una unidad de altura válida.");
                }
                Console.WriteLine("Ingrese la altura:");
                double altura = double.Parse(Console.ReadLine());

                string mensaje = $"{nombre},{fecha},{unidadpeso},{peso},{unidadaltura},{altura}"; //Diseño del mensaje a enviar
                byte[] data = Encoding.ASCII.GetBytes(mensaje); //se convierte el mensaje a un arreglo de bytes denominado data, usando ASCII
                stream.Write(data, 0, data.Length);//se envía el arreglo de bytes convertidos al servidor a traves del flujo de red stream

                byte[] buffer = new byte[1024]; //creacion buffer de bytes para almacenar datos enviados por servidor.
                int bytesRead = stream.Read(buffer, 0, buffer.Length); //lee los datos del flujo y se almacena en buffer. El numero de bytes se obtiene con strweam.Read
                string respuestaservidor = Encoding.ASCII.GetString(buffer, 0, bytesRead);//Se convierten los bytes a texto usando ASCII
                //Se almacena entonces en el string respuestaservidor
                Console.WriteLine($"Respuesta del servidor (Categoría, valor IMC): {respuestaservidor}");//Se imprime la respuesta del servidor
                Console.WriteLine("\n\n Gracias por utilizar mantenTuPeso.co");

                cliente.Close();//conexion cerrada

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); //Envio de mensaje de error al ser capturado
            }
        }


         // Define el método VerHistorial.
        static void VerHistorial()
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 8080); // Crea un cliente TCP que se conecta al servidor en la dirección IP 127.0.0.1 (localhost) y el puerto 8080.
                NetworkStream stream = cliente.GetStream(); // Obtiene el flujo de datos de la conexión del cliente.


                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine();

                string message = $"HISTORIAL,{nombre}"; // Crea un mensaje solicitando el historial, con el nombre del usuario (notar que se envía la palabra HISTORIAL como identificador del servicio)
                byte[] data = Encoding.ASCII.GetBytes(message); // Convierte el mensaje a un array de bytes usando codificación ASCII.
                stream.Write(data, 0, data.Length);  // Envía los datos al servidor a través del flujo de datos.

                byte[] buffer = new byte[1024]; // Crea un buffer de 1024 bytes para recibir datos del servidor.
                int bytesRead = stream.Read(buffer, 0, buffer.Length);// Lee los datos enviados por el servidor en el buffer. bytesRead contiene el número de bytes leídos.
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead); // Convierte los datos leídos a una cadena de texto.


                Console.WriteLine($"Historial de {nombre} (nombre:, fecha:, peso (en kg):, altura (en m):, imc," +
                    $"composición corporal:\n {response}"); // Imprime el historial recibido del servidor en la consola.

                cliente.Close(); // Cierra la conexión del cliente.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static bool PreguntarContinuar()
        {
            while (true)
            {
                Console.WriteLine("\n¿Desea volver al inicio o finalizar el programa?");
                Console.WriteLine("1. Volver al inicio");
                Console.WriteLine("2. Finalizar");
                string opcion = Console.ReadLine();
                if (opcion == "1")
                    return true;
                else if (opcion == "2")
                    return false;
                else
                    Console.WriteLine("Opción no válida, por favor intente nuevamente.");
            }
        }
    }
}
