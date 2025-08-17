using System;
using System.Collections.Generic;
using System.Linq;
using BlackJack_solid.Compartido;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Blackjack.Servicios
{
    public sealed class Dealer : IDealer
    {
        private readonly IReglasJuego _reglas;
        private readonly Random _rng = new();
        private Stack<Carta> _mazo = new();
        private bool _rondaActiva;

        private readonly Dictionary<IJugador, Mano> _manosJugadores = new();
        private readonly Mano _manoDealer = new();

        public Dealer(IReglasJuego reglas)
        {
            _reglas = reglas ?? throw new ArgumentNullException(nameof(reglas));
        }

        public void ConfigurarJugadores(IEnumerable<IJugador> jugadores)
        {
            _manosJugadores.Clear();
            if (jugadores is null) return;
            foreach (var j in jugadores)
                _manosJugadores[j] = new Mano();
        }

        public Mano ObtenerManoJugador(IJugador jugador) => _manosJugadores[jugador];

        public Mano ObtenerManoDealer() => _manoDealer;

        public void PedirCarta(IJugador jugador)
        {
            ValidarRondaActiva();
            var mano = _manosJugadores[jugador];
            mano.Agregar(Robar());
        }

        // Función pura para calcular puntos de una mano
        public int CalcularPuntos(Mano mano)
        {
            // Uso de LINQ para calcular el total de puntos y contar ases
            int total = mano.Cartas.Sum(c => _reglas.ValorCarta(c.Cara.ToString()));
            int ases = mano.Cartas.Count(c => c.Cara == Cara.A);

            // Ajustar el valor de los ases si el total excede el máximo permitido
            while (total > _reglas.MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }

            return total;
        }
        //  Este bloque es funcional porque utiliza LINQ para calcular el total de puntos y no modifica el estado interno de la clase. Es una función pura que siempre devuelve el mismo resultado para los mismos argumentos.

        // Función pura para evaluar resultados de los jugadores
        public IReadOnlyDictionary<IJugador, string> EvaluarResultados()
        {
            var puntosDealer = CalcularPuntos(_manoDealer);
            bool dealerSePasa = puntosDealer > _reglas.MaximoPuntos;

            // Uso de LINQ para evaluar los resultados de cada jugador
            return _manosJugadores.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                {
                    var puntosJugador = CalcularPuntos(kvp.Value);
                    bool jugadorSePasa = puntosJugador > _reglas.MaximoPuntos;

                    return jugadorSePasa ? "Pierde (se pasa)"
                         : dealerSePasa ? "Gana (dealer se pasa)"
                         : puntosJugador > puntosDealer ? "Gana"
                         : puntosJugador < puntosDealer ? "Pierde"
                         : "Empate";
                }
            );
        }
        //  Este bloque es funcional porque utiliza `ToDictionary` para transformar los datos sin modificar el estado interno. Es declarativo y fácil de entender.

        public void IniciarRonda()
        {
            if (_manosJugadores.Count == 0)
                throw new InvalidOperationException("No hay jugadores configurados. Usa ConfigurarJugadores(...) antes de iniciar la ronda.");

            _rondaActiva = true;
            _manoDealer.Limpiar();
            foreach (var k in _manosJugadores.Keys.ToList())
                _manosJugadores[k] = new Mano();

            _mazo = CrearMazoBarajado();
        }

        public void RepartirElementos()
        {
            ValidarRondaActiva();

            for (int i = 0; i < 2; i++)
            {
                foreach (var j in _manosJugadores.Keys)
                    _manosJugadores[j].Agregar(Robar());
                _manoDealer.Agregar(Robar());
            }
        }

        public void FinalizarRonda()
        {
            ValidarRondaActiva();
            JugarDealerHasta17();
            _rondaActiva = false;
        }

        // Función pura para validar una jugada
        public bool ValidarJugada(string jugada) =>
            jugada is "PedirCarta" or "Plantarse";
        //  Este bloque es funcional porque utiliza un patrón declarativo para validar la jugada. Es una función pura que no tiene efectos secundarios.

        private void ValidarRondaActiva()
        {
            if (!_rondaActiva) throw new InvalidOperationException("No hay una ronda activa. Llama a IniciarRonda() primero.");
        }

        // Función para crear un mazo barajado
        private Stack<Carta> CrearMazoBarajado()
        {
            var cartas = Enum.GetValues(typeof(Palo))
                .Cast<Palo>()
                .SelectMany(p => new[]
                {
                    new Carta(p, Cara.A),
                    new Carta(p, Cara.J),
                    new Carta(p, Cara.Q),
                    new Carta(p, Cara.K)
                }.Concat(Enumerable.Range(2, 9).Select(n => new Carta(p, (Cara)Enum.Parse(typeof(Cara), $"C{n}")))))
                .ToList();

            // Uso de LINQ para barajar las cartas
            var barajadas = cartas.OrderBy(_ => _rng.Next()).ToList();
            return new Stack<Carta>(barajadas);
        }
        //  Este bloque es funcional porque utiliza LINQ para generar y barajar las cartas de manera declarativa. No modifica el estado interno y devuelve un nuevo objeto.

        private Carta Robar()
        {
            if (_mazo.Count == 0)
                _mazo = CrearMazoBarajado();

            return _mazo.Pop();
        }

        // Función pura para verificar si el dealer tiene un 17 suave
        private bool Es17Suave(Mano mano)
        {
            int total = mano.Cartas.Sum(c => _reglas.ValorCarta(c.Cara.ToString()));
            int ases = mano.Cartas.Count(c => c.Cara == Cara.A);

            // Uso de un operador ternario para simplificar la lógica
            return ases > 0 && total == 17;
        }
        //  Este bloque es funcional porque utiliza LINQ para calcular el total y los ases, y no modifica el estado interno. Es una función pura que devuelve un resultado basado únicamente en los argumentos.

        // Función para jugar hasta que el dealer alcance 17 puntos
        public void JugarDealerHasta17()
        {
            ValidarRondaActiva();
            while (CalcularPuntos(_manoDealer) < 17 || Es17Suave(_manoDealer))
            {
                _manoDealer.Agregar(Robar());
            }
        }

        public void RepartirElementos()
        {
            ValidarRondaActiva();

            for (int i = 0; i < 2; i++)
            {
                foreach (var j in _manosJugadores.Keys)
                    _manosJugadores[j].Agregar(Robar());
                _manoDealer.Agregar(Robar());
            }
        }
    }
}
