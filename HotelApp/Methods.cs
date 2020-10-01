

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace HotelApp
{
    public static class Methods
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DbContext"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string query;
        static SqlCommand comando;
        static SqlDataReader registros;
        public static void Menu()
        {
            string[] opciones = { "1- Registrar cliente", "2- Editar cliente", "3- Check-in", "4- Check-out", "5- Ver todas las habitaciones ocupadas con huésped nombre ", "6- Ver habitaciones vacias ", "7- Ver habitaciones ocupadas ", "8- Salir" };
            for (int i = 0; i < opciones.Length; i++)
            {
                Console.WriteLine(opciones[i]);
                Console.WriteLine();
            }
        }
        public static void Registrar()
        {
            Console.WriteLine("no se puede reserva una habitación si previamente no sea registrado al cliente");

            Thread.Sleep(700);

            Console.WriteLine("Introduce tu Nombre: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Introduce tu Apellido: ");
            string Apellido = Console.ReadLine();
            Console.WriteLine("Introduce tu DNI: ");
            string DNI = Console.ReadLine();

            DNI = CheckDNI(DNI);

            query = $"INSERT INTO Clients (Nombre,Apellido,DNI) VALUES ('{nombre}','{Apellido}','{DNI}')";
            conexion.Open();
            comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();
            conexion.Close();

            Console.WriteLine("te has registrado correctamente");

        }
        public static string CheckDNI(string dni)
        {
            while (dni.Length < 9)
            {
                Console.WriteLine("tu dni no es valido, introduce otra vez: ");
                dni = Console.ReadLine();
            }
            return dni;
        }
        public static string IfDniExists(string DNI)
        {
            conexion.Open();
            query = $"SELECT * FROM Clients where DNI = '{DNI}'";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (!registros.Read())
            {
                Console.WriteLine("El Dni no existe. Introducelo otra vez: ");

                DNI = Console.ReadLine();
                conexion.Close();
                IfDniExists(DNI);
            }
            conexion.Close();
            return DNI;
        }
        public static string IfDniExists()
        {
            conexion.Open();
            Console.WriteLine("Introduce tu DNI: ");
            string DNI = Console.ReadLine();
            query = $"SELECT * FROM Clients where DNI = '{DNI}'";
            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();
            if (!registros.Read())
            {
                Console.WriteLine("El Usario no esta registrado ");
                salir();
            }
            conexion.Close();
            return DNI;
        }
        public static void EditClient(string DNI)
        {
            Console.WriteLine("Introduce nuevo Nombre: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Introduce nuevo Apellido: ");
            string Apellido = Console.ReadLine();

            conexion.Open();

            query = $"UPDATE Clients SET Nombre = '{nombre}', Apellido = '{Apellido}' WHERE DNI = '{DNI}'; ";
            comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();

            Console.WriteLine("El proceso ha sido exitoso");
        }
        public static void CheckIn()
        {
            string DNI = IfDniExists();
            if (DNI != null)
            {
                query = "SELECT * FROM Rooms WHERE Estado = 'Disponible'";
                conexion.Open();
                comando = new SqlCommand(query, conexion);
                registros = comando.ExecuteReader();

                Console.WriteLine("Habitaciones Disponibles: ");
                while (registros.Read())
                {
                    Console.WriteLine($"CodHabitacion: {registros["CodHabitacion"]} _ Estado: {registros["Estado"]}");

                }
                conexion.Close();

                Console.WriteLine("Elige la habitacion que quieres: (Numero de CodHabitacion)");
                int RoomCode = Convert.ToInt32(Console.ReadLine());

                bool isSuccess = ChangeRoomToBusy(RoomCode);

                if (isSuccess)
                {
                    Reservation(DNI, RoomCode, DateTime.Now.ToString(), DateTime.Now.ToString());
                }
                else
                {
                    Console.WriteLine("Ha surgido algun Error!!");
                    salir();
                }


            }
        }
        public static void Reservation(string DNI, int RoomCode, string CheckInDate, string CheckOutDate)
        {
            conexion.Open();
            query = $"INSERT INTO Reservs(DNI, CodHabitacion, FechaCheckin,FechaCheckOut) VALUES('{DNI}',{RoomCode},'{CheckInDate}','{CheckOutDate}')";
            comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();
            conexion.Close();
        }
        public static bool ChangeRoomToBusy(int RoomCode)
        {
            try
            {
                query = $"UPDATE Rooms SET Estado = 'Ocupada' WHERE CodHabitacion = {RoomCode}";
                conexion.Open();
                comando = new SqlCommand(query, conexion);
                comando.ExecuteNonQuery();

                Console.WriteLine("La operacion ha sido exitoso");
                conexion.Close();
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public static void CheckOut(string DNI)
        {
            string CorrectDni = IfDniExists(DNI);

            query = $"UPDATE Reservs SET FechaCheckOut = '{DateTime.Now.ToString()}' WHERE DNI = {DNI}";


            conexion.Open();

            comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            string RoomCode = returnRoomCodeFromReservation(DNI);
            if (RoomCode == null)
            {
                Console.WriteLine("Esta habitacion no existe!");
                salir();
            }
            conexion.Close();
            conexion.Open();
            string UpdateQueryOfRooms = $"UPDATE Rooms SET Estado = 'Disponible' WHERE CodHabitacion = {Convert.ToInt32(RoomCode)}";

            comando = new SqlCommand(UpdateQueryOfRooms, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();

        }
        public static string returnRoomCodeFromReservation(string DNI)
        {
            string QueryRoomCode = $"SELECT CodHabitacion FROM Reservs WHERE DNI = {DNI}";

            comando = new SqlCommand(QueryRoomCode, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                string result = registros["CodHabitacion"].ToString();
                return result;
            }
            else
            {
                return null;
            }
        }
        public static void ShowRoomsWithGuesInfo()
        {

            string query2 = "SELECT * FROM Rooms FULL OUTER JOIN Reservs ON Reservs.CodHabitacion=Rooms.CodHabitacion FULL OUTER JOIN Clients ON Clients.DNI=Reservs.DNI";
            conexion.Open();

            comando = new SqlCommand(query2, conexion);
            registros = comando.ExecuteReader();


            if (registros.Read())
            {
                int i = 1;

                while (registros.Read())
                {
                    if (registros["Estado"].ToString() == "Ocupada")
                    {
                        Console.WriteLine($"{i}- Habitacion numero: '{registros["CodHabitacion"]}' _ '{registros["Estado"]}' _ Nombre de huésped: '{registros["Nombre"]}' '{registros["Apellido"]}' ");
                    }

                    i++;
                }
            }
            conexion.Close();
        }
        public static void ShowRoomsEmpties()
        {
            query = "SELECT * FROM Rooms WHERE Estado = 'Disponible'";
            conexion.Open();

            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();

            if (registros.Read())
            {
                while (registros.Read())
                {
                    Console.WriteLine($"Habitacion numero: {registros["CodHabitacion"]} _ Estado: {registros["Estado"]}");
                }
            }
        }
        public static void ShowRoomsNotEmpties()
        {
            query = "SELECT * FROM Rooms WHERE Estado = 'Ocupada'";
            conexion.Open();

            comando = new SqlCommand(query, conexion);
            registros = comando.ExecuteReader();

            if (registros.Read())
            {
                while (registros.Read())
                {
                    Console.WriteLine($"Habitacion numero: {registros["CodHabitacion"]} _ Estado: {registros["Estado"]}");
                }
            }
        }
        public static void salir()
        {
            Console.WriteLine("Salidendo de la App...");
            Thread.Sleep(3000);
            Environment.Exit(0);
        }
    }
}
