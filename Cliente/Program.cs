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

            bool continuar = true; //Booleano, nos servirá para ver si el programa sigue corriendo o se quiere finalizar

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
                        Console.WriteLine(" 1. Calcular IMC       2. Ver Historial"); //Pregunta de cual opcion a utilizar

                        if (!int.TryParse(Console.ReadLine(), out action) || (action != 1 && action != 2)) //Se evalúa si la opcion ingresada coincide con las unicas disponibles
                        {
                            Console.WriteLine("Opción no existente, por favor ingrese una opción válida.");
                            action = 0; // Se resetea la acción para otra vez preguntar 
                        }
                    }

                    Console.Clear(); //Se limpia la consola 

                    if (action == 1)
                    {
                        CalculoIMC(); //La eleccion 1 llama al metodo de calculo IMC
                    }
                    else if (action == 2)
                    {
                        VerHistorial(); //La opcion 2 brinda el historico
                    }

                    continuar = PreguntarContinuar(); //Llama al metodo para preguntar si quiere continuar

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}"); //Se controla una posible excepcion mostrando el mensaje generado
                    continuar = PreguntarContinuar(); //Si se cierra el programa o se vuelve al inicio
                }
            }    
        }

        static void CalculoIMC() //Metodo para enviar datos al servidor y esperar el calculo del IMC
        {
            try
            {

                TcpClient cliente = new TcpClient("127.0.0.1", 8080); // Crea un cliente TCP que se conecta al servidor en la dirección IP 127.0.0.1 (localhost) y el puerto 8888.
                NetworkStream stream = cliente.GetStream();// Obtiene el flujo de datos (NEtworStream, metodo para enviar y recibir datos) de la conexión del cliente.

                Console.WriteLine("Ingrese su nombre:");
                string nombre = Console.ReadLine(); //Almacenamiento de nombre del cliente
                string fecha;
                while (true)
                {
                    Console.WriteLine("Ingrese la fecha (yyyy-mm-dd):");
                    fecha = Console.ReadLine();
                    if (DateTime.TryParse(fecha, out DateTime parsedFecha)) //Se valida que la fecha tenga el formato adecuado y coincida
                    {
                        if (parsedFecha.Date == DateTime.Now.Date) //Se verifica que la fecha coincida con la del sistema (actual)
                            break;
                        else
                            Console.WriteLine("Fecha no corresponde al día actual, por favor ingrese nuevamente."); //Si falla el día
                    }
                    else
                    {
                        Console.WriteLine("Formato de fecha incorrecto, por favor ingrese nuevamente."); //Si falla el formato
                    }
                }

                string unidadpeso;
                while (true)
                {
                    Console.WriteLine("Ingrese la unidad de peso (g, kg, lb, oz):");//Se pregunta que unidadd e peso utilizará
                    unidadpeso = Console.ReadLine();
                    if (unidadpeso == "g" || unidadpeso == "kg" || unidadpeso == "lb" || unidadpeso == "oz") //De igual manera se evalúa si la opcion ingresada coincide
                        break;
                    else
                        Console.WriteLine("Valor no existente, por favor ingrese una unidad de peso válida."); //Mensaje para que vuelva a intentar
                }

                double peso;
                while (true)
                {
                    Console.WriteLine("Ingrese el peso (en "+unidadpeso.ToString()+"):"); //Ingreso del peso
                    if (double.TryParse(Console.ReadLine(), out peso) && peso > 0) // Validación para asegurar que el peso sea un valor mayor a cero
                        break;
                    else
                        Console.WriteLine("Peso inválido, por favor ingrese un valor positivo mayor que cero.");
                }


                string unidadaltura;
                while (true)
                {
                    Console.WriteLine("Ingrese la unidad de altura (cm, m, plg, ft):"); //Se valida tambien las unidades de medida para la altura, si es que coinciden
                    unidadaltura = Console.ReadLine();
                    if (unidadaltura == "cm" || unidadaltura == "m" || unidadaltura == "plg" || unidadaltura == "ft")
                        break;
                    else
                        Console.WriteLine("Valor no existente, por favor ingrese una unidad de altura válida.");
                }
                double altura;
                while (true)
                {
                    Console.WriteLine("Ingrese la altura (en " + unidadaltura.ToString() + "):");
                    if (double.TryParse(Console.ReadLine(), out altura) && altura > 0) // Validación para asegurar que la altura sea un valor mayor que cero
                        break;
                    else
                        Console.WriteLine("Altura inválida, por favor ingrese un valor positivo mayor que cero.");
                }

                string mensaje = $"{nombre},{fecha},{unidadpeso},{peso},{unidadaltura},{altura}"; //Diseño del mensaje a enviar al servidor
                byte[] data = Encoding.ASCII.GetBytes(mensaje); //se convierte el mensaje a un arreglo de bytes denominado data, usando ASCII
                stream.Write(data, 0, data.Length);//se envía el arreglo de bytes convertidos al servidor a traves del flujo de red stream, desde el inicio hasta la longitud del mensaje, es decir el mensaje completo

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

        static bool PreguntarContinuar() //Método que sirve para pregntar al cliente si quiere finalizar o quiere volver al inicio del programa
        {
            while (true)
            {
                Console.WriteLine("\n¿Desea volver al inicio o finalizar el programa?");
                Console.WriteLine("1. Volver al inicio");
                Console.WriteLine("2. Finalizar");
                string opcion = Console.ReadLine();
                if (opcion == "1")
                    return true; //Depediendo la opcion escogida se actualiza el estado del booleano
                else if (opcion == "2")
                    return false;
                else
                    Console.WriteLine("Opción no válida, por favor intente nuevamente.");
            }
        }
    }
}
