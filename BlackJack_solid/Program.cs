using BlackJack_solid.Nucleo.Controladores;
using BlackJack_solid.Nucleo.Factories;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid
{
    class Program
    {
        private static IGameController _gameController = null!;

        static void Main(string[] args)
        {
            Console.WriteLine("===BLACKJACK INTERACTIVO ===");
            Console.WriteLine("¡Bienvenido al casino!");
            
            // Usar Factory para crear dependencias (DIP)
            var factory = new JuegoFabrica();
            _gameController = factory.CrearGameController();
            
            InicializarJuego();
            
            bool continuarJugando = true;
            while (continuarJugando && _gameController.PuedeJugar())
            {
                MostrarMenu();
                
                string opcion = Console.ReadLine() ?? "";
                
                switch (opcion)
                {
                    case "1":
                        JugarMano();
                        break;
                    case "2":
                        MostrarEstadisticas();
                        break;
                    case "3":
                        continuarJugando = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intenta de nuevo.");
                        break;
                }
            }
            
            MostrarMensajeFinal();
        }

        private static void InicializarJuego()
        {
            _gameController.InicializarJuego();
            Console.WriteLine($"\nReglas: Blackjack estándar");
            Console.WriteLine($"Objetivo: Acercarte a 21 sin pasarte. El dealer debe pedir hasta 17.");
            Console.WriteLine($"Mesa lista para jugar!");
        }

        private static void MostrarMenu()
        {
            Console.WriteLine($"\nTu saldo actual: ${_gameController.ObtenerSaldoJugador()}");
            Console.WriteLine("\n¿Qué quieres hacer?");
            Console.WriteLine("1. Jugar una mano");
            Console.WriteLine("2. Ver estadísticas");
            Console.WriteLine("3. Salir");
        }

        private static void JugarMano()
        {
            try
            {
                _gameController.JugarMano();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private static void MostrarEstadisticas()
        {
            _gameController.MostrarEstadisticas();
        }

        private static void MostrarMensajeFinal()
        {
            if (!_gameController.PuedeJugar())
            {
                Console.WriteLine("\n¡Se acabó tu dinero! ¡Gracias por jugar!");
            }
            else
            {
                Console.WriteLine($"\n¡Gracias por jugar! Tu saldo final: ${_gameController.ObtenerSaldoJugador()}");
            }
        }
    }
}
