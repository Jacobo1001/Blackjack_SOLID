using BlackJack_solid.Blackjack.Reglas;
using BlackJack_solid.Blackjack.Servicios;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Controladores
{
    public interface IGameController
    {
        void InicializarJuego();
        IGameController JugarMano();
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
        private readonly IJugador _jugador;
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

        public IGameController JugarMano()
        {
            if (!PuedeJugar())
            {
                throw new InvalidOperationException("No tienes suficiente dinero para apostar.");
            }

            // Pedir apuesta
            Console.WriteLine($"\n💵 ¿Cuánto quieres apostar? (Máximo: ${_jugador.ObtenerSaldo()})");
            if (!double.TryParse(Console.ReadLine(), out double apuesta) || apuesta <= 0 || apuesta > _jugador.ObtenerSaldo())
            {
                Console.WriteLine("❌ Apuesta inválida. Usando $10 por defecto.");
                apuesta = Math.Min(10, _jugador.ObtenerSaldo());
            }

            // Crear apuesta
            var apuestaJugador = _servicioApuestas.CrearApuesta(_jugador, apuesta);
            Console.WriteLine($"✅ Apuesta de ${apuesta} registrada.");

            // Iniciar ronda
            _dealer.IniciarRonda();
            _dealer.RepartirElementos();

            // Mostrar cartas iniciales
            var manoJugador = ObtenerManoJugador();
            var manoDealer = ObtenerManoDealer();

            Console.WriteLine($"\n🃏 Tus cartas: {MostrarCartas(manoJugador)} = {CalcularPuntos(manoJugador)} puntos");
            Console.WriteLine($"🃏 Dealer: {MostrarCartas(manoDealer)} = {CalcularPuntos(manoDealer)} puntos");

            // Verificar blackjack inicial
            if (CalcularPuntos(manoJugador) == 21)
            {
                Console.WriteLine("🎉 ¡BLACKJACK! ¡Ganaste!");
                return new GameController(
                    _reglas,
                    _servicioMesa,
                    _servicioApuestas,
                    _dealer, 
                    FinalizarMano("Gana (Blackjack)", apuesta * 1.5)
                );
            }

            // Turno del jugador
            bool seguirJugando = true;
            while (seguirJugando && CalcularPuntos(manoJugador) < 21)
            {
                Console.WriteLine($"\n🎯 Tus puntos: {CalcularPuntos(manoJugador)}");
                Console.WriteLine("¿Qué quieres hacer?");
                Console.WriteLine("1. Pedir carta (Hit)");
                Console.WriteLine("2. Plantarse (Stand)");

                string decision = Console.ReadLine() ?? "";
                switch (decision)
                {
                    case "1":
                        PedirCartaJugador();
                        Console.WriteLine($"🃏 Nueva carta: {MostrarCartas(manoJugador)} = {CalcularPuntos(manoJugador)} puntos");

                        if (CalcularPuntos(manoJugador) > 21)
                        {
                            Console.WriteLine("💥 ¡Te pasaste! Pierdes la apuesta.");
                            return new GameController(
                                _reglas,
                                _servicioMesa,
                                _servicioApuestas,
                                _dealer, 
                                FinalizarMano("Pierde (se pasa)", 0)
                            );
                        }
                        break;
                    case "2":
                        seguirJugando = false;
                        break;
                    default:
                        Console.WriteLine("❌ Opción no válida. Intenta de nuevo.");
                        break;
                }
            }

            // Turno del dealer
            Console.WriteLine($"\n🤖 Turno del dealer...");
            _dealer.FinalizarRonda();

            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"\n📊 Resultado final:");
            Console.WriteLine($"🃏 Tus cartas: {MostrarCartas(manoJugador)} = {puntosJugador} puntos");
            Console.WriteLine($"🃏 Dealer: {MostrarCartas(manoDealer)} = {puntosDealer} puntos");

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

            return new GameController(
                _reglas,
                _servicioMesa,
                _servicioApuestas,
                _dealer, 
                FinalizarMano(resultado, ganancia)
            );
        }

        public void MostrarEstadisticas()
        {
            Console.WriteLine($"\n📈 === ESTADÍSTICAS ===");
            Console.WriteLine($"👤 Jugador: {_jugador.ObtenerNombre()}");
            Console.WriteLine($"💰 Saldo actual: ${_jugador.ObtenerSaldo()}");
            Console.WriteLine($"🎯 Reglas: {_reglas.NombreReglas}");
            Console.WriteLine($"🎲 Mesa: {_mesa.ObtenerId()}");
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

        private IJugador FinalizarMano(string resultado, double ganancia)
        {
            Console.WriteLine($"\n🎯 Resultado: {resultado}");

            IJugador jugador;
            if (ganancia > 0)
            {
                Console.WriteLine($"💰 Ganaste: ${ganancia}");
                // Actualizar saldo del jugador
                jugador = _jugador.ActualizarSaldo(ganancia);
                // En una implementación real, esto se manejaría a través de un servicio
            }
            else
            {
                jugador = _jugador;
                Console.WriteLine("💸 Perdiste tu apuesta.");
            }

            Console.WriteLine($"💵 Nuevo saldo: ${jugador.ObtenerSaldo()}");
            return jugador;
        }

        private string MostrarCartas(Mano mano)
        {
            return string.Join(", ", mano.Cartas.Select(c => $"{c.Cara} de {c.Palo}"));
        }
    }
}
