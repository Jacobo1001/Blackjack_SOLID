using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;

namespace BlackJack_solid.Nucleo.Patrones.Comportamiento
{
    /// <summary>
    /// STATE PATTERN
    /// Permite que un objeto altere su comportamiento cuando su estado interno cambia.
    /// En Blackjack, permite manejar los diferentes estados de una ronda de juego.
    /// </summary>

    /// <summary>
    /// STATE PATTERN - Estados posibles de una ronda de Blackjack
    /// </summary>
    public enum EstadoRonda
    {
        EsperandoApuestas,    // Los jugadores pueden hacer apuestas
        RepartiendoCartas,     // Se reparten las cartas iniciales
        TurnoJugador,          // El jugador puede pedir cartas o plantarse
        TurnoDealer,           // El dealer juega su mano
        CalculandoResultado,   // Se calculan los resultados
        Finalizada             // La ronda ha terminado
    }

    /// <summary>
    /// STATE PATTERN - Interfaz para los estados de la ronda
    /// Define las operaciones que cada estado puede realizar.
    /// </summary>
    public interface IEstadoRonda
    {
        /// <summary>
        /// STATE PATTERN: Obtener el estado actual
        /// </summary>
        EstadoRonda ObtenerEstado();

        /// <summary>
        /// STATE PATTERN: Procesar una acción en el estado actual
        /// </summary>
        void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null);

        /// <summary>
        /// STATE PATTERN: Verificar si una acción es válida en el estado actual
        /// </summary>
        bool EsAccionValida(string accion);

        /// <summary>
        /// STATE PATTERN: Obtener las acciones disponibles en el estado actual
        /// </summary>
        IEnumerable<string> ObtenerAccionesDisponibles();

        /// <summary>
        /// STATE PATTERN: Obtener descripción del estado
        /// </summary>
        string ObtenerDescripcion();
    }

    /// <summary>
    /// STATE PATTERN - Contexto que mantiene el estado actual
    /// </summary>
    public interface IContextoRonda
    {
        /// <summary>
        /// STATE PATTERN: Cambiar el estado actual
        /// </summary>
        void CambiarEstado(IEstadoRonda nuevoEstado);

        /// <summary>
        /// STATE PATTERN: Obtener el estado actual
        /// </summary>
        IEstadoRonda ObtenerEstadoActual();

        /// <summary>
        /// STATE PATTERN: Obtener información de la ronda
        /// </summary>
        Ronda ObtenerRonda();
        IDealer ObtenerDealer();
        IEnumerable<IJugador> ObtenerJugadores();
    }

    /// <summary>
    /// STATE PATTERN - Estado: Esperando Apuestas
    /// Los jugadores pueden hacer apuestas antes de que comience la ronda.
    /// </summary>
    public sealed class EstadoEsperandoApuestas : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.EsperandoApuestas;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Esperando Apuestas'");

            switch (accion.ToLower())
            {
                case "hacer_apuesta":
                    Console.WriteLine("✅ Apuesta registrada. Esperando más apuestas o inicio de ronda.");
                    break;

                case "iniciar_ronda":
                    Console.WriteLine("🎯 Iniciando ronda - cambiando a estado 'Repartiendo Cartas'");
                    contexto.CambiarEstado(new EstadoRepartiendoCartas());
                    break;

                case "cancelar_ronda":
                    Console.WriteLine("❌ Ronda cancelada.");
                    contexto.CambiarEstado(new EstadoFinalizada());
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Esperando Apuestas'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "hacer_apuesta", "iniciar_ronda", "cancelar_ronda" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "hacer_apuesta", "iniciar_ronda", "cancelar_ronda" };
        }

        public string ObtenerDescripcion() => "Esperando que los jugadores hagan sus apuestas";
    }

    /// <summary>
    /// STATE PATTERN - Estado: Repartiendo Cartas
    /// Se reparten las cartas iniciales a jugadores y dealer.
    /// </summary>
    public sealed class EstadoRepartiendoCartas : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.RepartiendoCartas;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Repartiendo Cartas'");

            switch (accion.ToLower())
            {
                case "repartir_cartas":
                    Console.WriteLine("🃏 Repartiendo cartas iniciales...");
                    // Lógica para repartir cartas
                    Console.WriteLine("✅ Cartas repartidas - cambiando a estado 'Turno Jugador'");
                    contexto.CambiarEstado(new EstadoTurnoJugador());
                    break;

                case "cancelar_ronda":
                    Console.WriteLine("❌ Ronda cancelada durante reparto.");
                    contexto.CambiarEstado(new EstadoFinalizada());
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Repartiendo Cartas'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "repartir_cartas", "cancelar_ronda" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "repartir_cartas", "cancelar_ronda" };
        }

        public string ObtenerDescripcion() => "Repartiendo cartas iniciales a jugadores y dealer";
    }

    /// <summary>
    /// STATE PATTERN - Estado: Turno del Jugador
    /// El jugador puede pedir cartas, plantarse, doblar o rendirse.
    /// </summary>
    public sealed class EstadoTurnoJugador : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.TurnoJugador;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Turno Jugador'");

            switch (accion.ToLower())
            {
                case "pedir_carta":
                    Console.WriteLine("🃏 Jugador pide una carta...");
                    // Lógica para dar carta al jugador
                    Console.WriteLine("✅ Carta entregada al jugador.");
                    break;

                case "plantarse":
                    Console.WriteLine("✋ Jugador se planta - cambiando a estado 'Turno Dealer'");
                    contexto.CambiarEstado(new EstadoTurnoDealer());
                    break;

                case "doblar_apuesta":
                    Console.WriteLine("💰 Jugador dobla su apuesta y recibe una carta final.");
                    // Lógica para doblar apuesta
                    Console.WriteLine("✅ Apuesta doblada - cambiando a estado 'Turno Dealer'");
                    contexto.CambiarEstado(new EstadoTurnoDealer());
                    break;

                case "rendirse":
                    Console.WriteLine("🏳️ Jugador se rinde - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                case "blackjack":
                    Console.WriteLine("🎉 ¡BLACKJACK! - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                case "se_paso":
                    Console.WriteLine("💥 Jugador se pasó de 21 - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Turno Jugador'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "pedir_carta", "plantarse", "doblar_apuesta", "rendirse", "blackjack", "se_paso" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "pedir_carta", "plantarse", "doblar_apuesta", "rendirse" };
        }

        public string ObtenerDescripcion() => "Es el turno del jugador para tomar decisiones";
    }

    /// <summary>
    /// STATE PATTERN - Estado: Turno del Dealer
    /// El dealer juega su mano siguiendo las reglas del casino.
    /// </summary>
    public sealed class EstadoTurnoDealer : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.TurnoDealer;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Turno Dealer'");

            switch (accion.ToLower())
            {
                case "dealer_pide_carta":
                    Console.WriteLine("🤖 Dealer pide una carta...");
                    // Lógica para dar carta al dealer
                    Console.WriteLine("✅ Carta entregada al dealer.");
                    break;

                case "dealer_se_planta":
                    Console.WriteLine("✋ Dealer se planta - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                case "dealer_se_paso":
                    Console.WriteLine("💥 Dealer se pasó de 21 - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                case "dealer_blackjack":
                    Console.WriteLine("🎉 ¡Dealer tiene BLACKJACK! - cambiando a estado 'Calculando Resultado'");
                    contexto.CambiarEstado(new EstadoCalculandoResultado());
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Turno Dealer'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "dealer_pide_carta", "dealer_se_planta", "dealer_se_paso", "dealer_blackjack" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "dealer_pide_carta", "dealer_se_planta" };
        }

        public string ObtenerDescripcion() => "Es el turno del dealer para jugar su mano";
    }

    /// <summary>
    /// STATE PATTERN - Estado: Calculando Resultado
    /// Se calculan los resultados y se determinan los ganadores.
    /// </summary>
    public sealed class EstadoCalculandoResultado : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.CalculandoResultado;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Calculando Resultado'");

            switch (accion.ToLower())
            {
                case "calcular_resultado":
                    Console.WriteLine("📊 Calculando resultados de la ronda...");
                    // Lógica para calcular resultados
                    Console.WriteLine("✅ Resultados calculados - cambiando a estado 'Finalizada'");
                    contexto.CambiarEstado(new EstadoFinalizada());
                    break;

                case "pagar_ganancias":
                    Console.WriteLine("💰 Pagando ganancias a los jugadores...");
                    // Lógica para pagar ganancias
                    Console.WriteLine("✅ Ganancias pagadas.");
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Calculando Resultado'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "calcular_resultado", "pagar_ganancias" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "calcular_resultado", "pagar_ganancias" };
        }

        public string ObtenerDescripcion() => "Calculando resultados y determinando ganadores";
    }

    /// <summary>
    /// STATE PATTERN - Estado: Finalizada
    /// La ronda ha terminado y no se pueden realizar más acciones.
    /// </summary>
    public sealed class EstadoFinalizada : IEstadoRonda
    {
        public EstadoRonda ObtenerEstado() => EstadoRonda.Finalizada;

        public void ProcesarAccion(IContextoRonda contexto, string accion, object? parametros = null)
        {
            Console.WriteLine("STATE PATTERN: Procesando acción en estado 'Finalizada'");

            switch (accion.ToLower())
            {
                case "nueva_ronda":
                    Console.WriteLine("🔄 Iniciando nueva ronda - cambiando a estado 'Esperando Apuestas'");
                    contexto.CambiarEstado(new EstadoEsperandoApuestas());
                    break;

                case "cerrar_mesa":
                    Console.WriteLine("🚪 Cerrando mesa.");
                    // Lógica para cerrar la mesa
                    break;

                default:
                    Console.WriteLine($"❌ Acción '{accion}' no válida en estado 'Finalizada'");
                    break;
            }
        }

        public bool EsAccionValida(string accion)
        {
            var accionesValidas = new[] { "nueva_ronda", "cerrar_mesa" };
            return Array.Exists(accionesValidas, a => a.Equals(accion, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<string> ObtenerAccionesDisponibles()
        {
            return new[] { "nueva_ronda", "cerrar_mesa" };
        }

        public string ObtenerDescripcion() => "La ronda ha terminado";
    }

    /// <summary>
    /// STATE PATTERN - Implementación concreta del contexto
    /// Mantiene el estado actual y delega las operaciones al estado correspondiente.
    /// </summary>
    public sealed class ContextoRonda : IContextoRonda
    {
        private IEstadoRonda _estadoActual;
        private readonly Ronda _ronda;
        private readonly IDealer _dealer;
        private readonly List<IJugador> _jugadores;

        public ContextoRonda(Ronda ronda, IDealer dealer, IEnumerable<IJugador> jugadores)
        {
            _ronda = ronda ?? throw new ArgumentNullException(nameof(ronda));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
            _jugadores = new List<IJugador>(jugadores ?? throw new ArgumentNullException(nameof(jugadores)));
            
            // STATE PATTERN: Inicializar con el estado inicial
            _estadoActual = new EstadoEsperandoApuestas();
            Console.WriteLine($"STATE PATTERN: ContextoRonda inicializado en estado '{_estadoActual.ObtenerDescripcion()}'");
        }

        /// <summary>
        /// STATE PATTERN: Cambiar el estado actual
        /// </summary>
        public void CambiarEstado(IEstadoRonda nuevoEstado)
        {
            if (nuevoEstado == null) throw new ArgumentNullException(nameof(nuevoEstado));
            
            var estadoAnterior = _estadoActual.ObtenerEstado();
            _estadoActual = nuevoEstado;
            
            Console.WriteLine($"STATE PATTERN: Estado cambiado de '{estadoAnterior}' a '{nuevoEstado.ObtenerEstado()}'");
        }

        /// <summary>
        /// STATE PATTERN: Obtener el estado actual
        /// </summary>
        public IEstadoRonda ObtenerEstadoActual() => _estadoActual;

        /// <summary>
        /// STATE PATTERN: Procesar una acción en el estado actual
        /// </summary>
        public void ProcesarAccion(string accion, object? parametros = null)
        {
            Console.WriteLine($"STATE PATTERN: Procesando acción '{accion}' en estado '{_estadoActual.ObtenerEstado()}'");
            
            if (!_estadoActual.EsAccionValida(accion))
            {
                Console.WriteLine($"❌ Acción '{accion}' no válida en estado '{_estadoActual.ObtenerEstado()}'");
                Console.WriteLine($"✅ Acciones disponibles: {string.Join(", ", _estadoActual.ObtenerAccionesDisponibles())}");
                return;
            }

            _estadoActual.ProcesarAccion(this, accion, parametros);
        }

        /// <summary>
        /// STATE PATTERN: Obtener información de la ronda
        /// </summary>
        public Ronda ObtenerRonda() => _ronda;
        public IDealer ObtenerDealer() => _dealer;
        public IEnumerable<IJugador> ObtenerJugadores() => _jugadores.AsReadOnly();

        /// <summary>
        /// STATE PATTERN: Obtener información del estado actual
        /// </summary>
        public string ObtenerInformacionEstado()
        {
            return $"Estado actual: {_estadoActual.ObtenerEstado()}\n" +
                   $"Descripción: {_estadoActual.ObtenerDescripcion()}\n" +
                   $"Acciones disponibles: {string.Join(", ", _estadoActual.ObtenerAccionesDisponibles())}";
        }
    }
}
