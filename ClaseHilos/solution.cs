namespace ClaseHilos
{
    internal class Producto
    {
        public string Nombre { get; set; }
        public decimal PrecioUnitarioDolares { get; set; }
        public int CantidadEnStock { get; set; }

        public Producto(string nombre, decimal precioUnitario, int cantidadEnStock)
        {
            Nombre = nombre;
            PrecioUnitarioDolares = precioUnitario;
            CantidadEnStock = cantidadEnStock;
        }
    }
    internal class Solution //reference: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock
    {
        // integrantes del grupo: Gallardo Bautista y Idiarte Joaquín

        static List<Producto> productos = new List<Producto>
        {
            new Producto("Camisa", 10, 50),
            new Producto("Pantalón", 8, 30),
            new Producto("Zapatilla/Champión", 7, 20),
            new Producto("Campera", 25, 100),
            new Producto("Gorra", 16, 10)
        };

        static int precio_dolar = 500;

        // Declaramos el semaforo binario
        static new Mutex mutex = new Mutex();

        // Declaramos barrera
        static new Barrier barrera = new Barrier(2, (b) =>
        {
            Console.WriteLine($"Post-Phase action: {b.CurrentPhaseNumber}");
            Tarea3();
        });

        // Actualizacion de stock
        static void Tarea1()
        {
            // Entra a la region critica
            mutex.WaitOne();
            Console.WriteLine("Agregando productos");
            foreach (var producto in productos)
            {
                producto.CantidadEnStock += 10;
            }
            Console.WriteLine("Productos agregados");
            // Sale de la region critica
            mutex.ReleaseMutex();
            // Espera a que el otro hilo llegue si es que no llego
            barrera.SignalAndWait();
        }
        static void Tarea2()
        {
            // Entra a la region critica
            mutex.WaitOne();
            Console.WriteLine("Agregando precios");
            precio_dolar = 1280;
            Console.WriteLine("Precios agregaos");
            // Sale de la region critica
            mutex.ReleaseMutex();
            // Espera a que el otro hilo llegue si es que no llego
            barrera.SignalAndWait();
        }
        static void Tarea3()
        {
            // pasamos dolares a pesos
            Console.WriteLine("Listando productos");
            foreach (var producto in productos)
            {
                producto.PrecioUnitarioDolares = producto.PrecioUnitarioDolares * precio_dolar;
            }

            // generamos informe de la actualizacion de precios y stock
            Console.WriteLine("Productos actualizados");
            foreach (var producto in productos)
            {
                Console.WriteLine($"Nombre: {producto.Nombre}, Precio: {producto.PrecioUnitarioDolares}, Stock: {producto.CantidadEnStock}");
            }
            Console.ReadLine();
        }

        internal static void Excecute()
        {
            // instanciamos los hilos
            Thread task1 = new Thread(Tarea1);
            Thread task2 = new Thread(Tarea2);
            task1.Start();
            task2.Start();



        }
    }
}