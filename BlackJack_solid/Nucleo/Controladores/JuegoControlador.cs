using BlackJack_solid.Blackjack.Reglas;
using BlackJack_solid.Blackjack.Servicios;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;

namespace BlackJack_solid.Nucleo.Controladores
{
    public interface IGameController
    {
        void InicializarJuego();
        void JugarMano();
        void MostrarEstadisticas();
        bool PuedeJugar();
        double ObtenerSaldoJugador();
    }

    public sealed class GameController : IGameController
    {
        private readonly IReglasJuego _reglas;
        private readonly IServicioMesa _servicioMesa;
        private readonly IServicioApuestas _servicioApuestas;
        private readonly IDealer _dealer;
        private IJugador _jugador;
        private IMesa _mesa = null!;

        public GameController(
            IReglasJuego reglas,
            IServicioMesa servicioMesa,
            IServicioApuestas servicioApuestas,
            IDealer dealer,
            IJugador jugador)
        {
            _reglas = reglas ?? throw new ArgumentNullException(nameof(reglas));
            _servicioMesa = servicioMesa ?? throw new ArgumentNullException(nameof(servicioMesa));
            _servicioApuestas = servicioApuestas ?? throw new ArgumentNullException(nameof(servicioApuestas));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
        }

        public void InicializarJuego()
        {
            // Crear mesa
            _mesa = _servicioMesa.AbrirMesa(_reglas);
            _mesa.AsignarDealer(_dealer);

            // Configurar jugador en dealer
            if (_dealer is Dealer dealerConcreto)
            {
                dealerConcreto.ConfigurarJugadores(new[] { _jugador });
            }
        }

        public void JugarMano()
        {
            if (!PuedeJugar())
            {
                throw new InvalidOperationException("No tienes suficiente dinero para apostar.");
            }

            // Pedir apuesta
            Console.WriteLine($"\n¿Cuánto quieres apostar? (Máximo: ${_jugador.ObtenerSaldo()})");
            if (!double.TryParse(Console.ReadLine(), out double apuesta) || apuesta <= 0 || apuesta > _jugador.ObtenerSaldo())
            {
                Console.WriteLine("Apuesta inválida. Usando $10 por defecto.");
                apuesta = Math.Min(10, _jugador.ObtenerSaldo());
            }

            // Crear apuesta
            var apuestaJugador = _servicioApuestas.CrearApuesta(_jugador, apuesta);
            Console.WriteLine($"Apuesta de ${apuesta} registrada.");

            // Iniciar ronda
            _dealer.IniciarRonda();
            _dealer.RepartirElementos();

            // Mostrar cartas iniciales
            var manoJugador = ObtenerManoJugador();
            var manoDealer = ObtenerManoDealer();

            Console.WriteLine($"\nTus cartas: {MostrarCartas(manoJugador)} = {CalcularPuntos(manoJugador)} puntos");
            Console.WriteLine($"Dealer: {MostrarCartas(manoDealer)} = {CalcularPuntos(manoDealer)} puntos");

            // Verificar blackjack inicial
            if (CalcularPuntos(manoJugador) == 21)
            {
                Console.WriteLine("¡BLACKJACK! ¡Ganaste!");
                FinalizarMano("Gana (Blackjack)", apuesta * 1.5);
                return;
            }

            // Turno del jugador
            bool seguirJugando = true;
            while (seguirJugando && CalcularPuntos(manoJugador) < 21)
            {
                Console.WriteLine($"\nTus puntos: {CalcularPuntos(manoJugador)}");
                Console.WriteLine("¿Qué quieres hacer?");
                Console.WriteLine("1. Pedir carta (Hit)");
                Console.WriteLine("2. Plantarse (Stand)");

                string decision = Console.ReadLine() ?? "";
                switch (decision)
                {
                    case "1":
                        PedirCartaJugador();
                        Console.WriteLine($"Nueva carta: {MostrarCartas(manoJugador)} = {CalcularPuntos(manoJugador)} puntos");

                        if (CalcularPuntos(manoJugador) > 21)
                        {
                            Console.WriteLine("¡Te pasaste! Pierdes la apuesta.");
                            FinalizarMano("Pierde (se pasa)", 0);
                            return;
                        }
                        break;
                    case "2":
                        seguirJugando = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Intenta de nuevo.");
                        break;
                }
            }

            // Turno del dealer
            Console.WriteLine($"\nTurno del dealer...");
            _dealer.FinalizarRonda();

            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"\nResultado final:");
            Console.WriteLine($"Tus cartas: {MostrarCartas(manoJugador)} = {puntosJugador} puntos");
            Console.WriteLine($"Dealer: {MostrarCartas(manoDealer)} = {puntosDealer} puntos");

            // Determinar resultado
            string resultado;
            double ganancia;

            if (puntosDealer > 21)
            {
                resultado = "Gana (dealer se pasa)";
                ganancia = apuesta * 2;
            }
            else if (puntosJugador > puntosDealer)
            {
                resultado = "Gana";
                ganancia = apuesta * 2;
            }
            else if (puntosJugador < puntosDealer)
            {
                resultado = "Pierde";
                ganancia = 0;
            }
            else
            {
                resultado = "Empate";
                ganancia = apuesta; // Devuelve la apuesta
            }

            FinalizarMano(resultado, ganancia);
        }

        public void MostrarEstadisticas()
        {
            Console.WriteLine($"\n=== ESTADÍSTICAS ===");
            Console.WriteLine($"Jugador: {_jugador.ObtenerNombre()}");
            Console.WriteLine($"Saldo actual: ${_jugador.ObtenerSaldo()}");
            Console.WriteLine($"Reglas: {_reglas.NombreReglas}");
            Console.WriteLine($"Mesa: {_mesa.ObtenerId()}");
        }

        public bool PuedeJugar() => _jugador.ObtenerSaldo() > 0;

        public double ObtenerSaldoJugador() => _jugador.ObtenerSaldo();

        private Mano ObtenerManoJugador()
        {
            if (_dealer is Dealer dealerConcreto)
            {
                return dealerConcreto.ObtenerManoJugador(_jugador);
            }
            throw new InvalidOperationException("Dealer no soporta obtener mano del jugador.");
        }

        private Mano ObtenerManoDealer()
        {
            if (_dealer is Dealer dealerConcreto)
            {
                return dealerConcreto.ObtenerManoDealer();
            }
            throw new InvalidOperationException("Dealer no soporta obtener mano del dealer.");
        }

        private int CalcularPuntos(Mano mano)
        {
            if (_dealer is Dealer dealerConcreto)
            {
                return dealerConcreto.CalcularPuntos(mano);
            }
            throw new InvalidOperationException("Dealer no soporta calcular puntos.");
        }

        private void PedirCartaJugador()
        {
            if (_dealer is Dealer dealerConcreto)
            {
                dealerConcreto.PedirCarta(_jugador);
            }
            else
            {
                throw new InvalidOperationException("Dealer no soporta pedir carta.");
            }
        }

        private void FinalizarMano(string resultado, double ganancia)
        {
            Console.WriteLine($"\nResultado: {resultado}");

            // Siempre se descuenta la apuesta
            double apuesta = 0;
            if (_servicioApuestas is ServicioApuestas servicioApuestasImpl && servicioApuestasImpl is not null)
            {
                // Si tienes acceso a la última apuesta, úsala aquí
                // Por simplicidad, asumimos que la última apuesta es la actual
                // Si tienes una mejor forma de obtener la apuesta actual, reemplaza esta lógica
                apuesta = servicioApuestasImpl.ObtenerUltimaApuestaMonto(_jugador);
            }
            else
            {
                // Si no puedes obtener la apuesta, pide al usuario que la ingrese
                Console.WriteLine("No se pudo obtener el monto de la apuesta. Ingresa el monto perdido:");
                double.TryParse(Console.ReadLine(), out apuesta);
            }
            _jugador = _jugador.ActualizarSaldo(-apuesta);

            if (ganancia > 0)
            {
                Console.WriteLine($"Ganaste: ${ganancia}");
                // Sumar la ganancia (premio)
                _jugador = _jugador.ActualizarSaldo(ganancia);
            }
            else
            {
                Console.WriteLine("Perdiste tu apuesta.");
            }

            Console.WriteLine($"Nuevo saldo: ${_jugador.ObtenerSaldo()}");
        }

        private string ObtenerNombreCara(Cara cara)
        {
            return cara switch
            {
                Cara.A => "A",
                Cara.J => "J",
                Cara.Q => "Q",
                Cara.K => "K",
                Cara.C2 => "2",
                Cara.C3 => "3",
                Cara.C4 => "4",
                Cara.C5 => "5",
                Cara.C6 => "6",
                Cara.C7 => "7",
                Cara.C8 => "8",
                Cara.C9 => "9",
                Cara.C10 => "10",
                _ => cara.ToString()
            };
        }

        private string MostrarCartas(Mano mano)
        {
            return string.Join(", ", mano.Cartas.Select(c => $"{ObtenerNombreCara(c.Cara)} de {c.Palo}"));
        }
    }
}
