using System;
using System.Threading;

namespace HotelApp
{
    public class Program
    {
      
        static void Main(string[] args)
        {
            Methods.Menu();
            Console.WriteLine("elige una opcion: ");
            int opcionElegido = Convert.ToInt32(Console.ReadLine());

            switch (opcionElegido)
            {
                case 1:
                    Methods.Registrar();
                    Console.WriteLine("Cargando las habitaciones vacias...");
                    Thread.Sleep(2000);
                    Methods.CheckIn();
                    break;
                case 2:
                    Console.WriteLine("Escribe tu DNI: ");
                    string DNI = Methods.IfDniExists(Console.ReadLine());
                    Methods.EditClient(DNI);
                    break;
                case 3:
                    Methods.CheckIn();
                    break;
                case 4:
                    Console.WriteLine("Escribe tu DNI: ");
                    string Dni = Methods.IfDniExists(Console.ReadLine());
                    Methods.CheckOut(Dni);
                    break;
                case 5:
                    Methods.ShowRoomsWithGuesInfo();
                    break;
                case 6:
                    Methods.ShowRoomsEmpties();
                    break;
                case 7:
                    Methods.ShowRoomsNotEmpties();
                    break;
                case 8:
                    Methods.salir();
                    break;
                default: Console.WriteLine("Ha surgido un Error!!");
                    break;
            }
        }
    
    }
}
