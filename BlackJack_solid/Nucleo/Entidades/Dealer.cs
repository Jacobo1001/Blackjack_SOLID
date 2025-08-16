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

        public void JugarDealerHasta17()
        {
            ValidarRondaActiva();
            while (CalcularPuntos(_manoDealer) < 17 || Es17Suave(_manoDealer))
            {
                _manoDealer.Agregar(Robar());
            }
        }

        public int CalcularPuntos(Mano mano)
        {
            int total = 0;
            int ases = 0;

            foreach (var c in mano.Cartas)
            {
                var cara = c.Cara switch
                {
                    Cara.A => "A",
                    Cara.J => "J",
                    Cara.Q => "Q",
                    Cara.K => "K",
                    _ => ((int)c.Cara).ToString().Replace("C", "") 
                };
                int valor = _reglas.ValorCarta(cara);
                total += valor;
                if (c.Cara == Cara.A) ases++;
            }

            while (total > _reglas.MaximoPuntos && ases > 0)
            {
                total -= 10; 
                ases--;
            }

            return total;
        }

        public IReadOnlyDictionary<IJugador, string> EvaluarResultados()
        {
            var resultados = new Dictionary<IJugador, string>();
            var puntosDealer = CalcularPuntos(_manoDealer);
            bool dealerSePasa = puntosDealer > _reglas.MaximoPuntos;

            foreach (var (jug, mano) in _manosJugadores)
            {
                var puntos = CalcularPuntos(mano);
                bool jugadorSePasa = puntos > _reglas.MaximoPuntos;

                string res = jugadorSePasa ? "Pierde (se pasa)"
                           : dealerSePasa ? "Gana (dealer se pasa)"
                           : puntos > puntosDealer ? "Gana"
                           : puntos < puntosDealer ? "Pierde"
                           : "Empate";
                resultados[jug] = res;
            }

            return resultados;
        }

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

        public bool ValidarJugada(string jugada)
        {
            return jugada is "PedirCarta" or "Plantarse";
        }


        private void ValidarRondaActiva()
        {
            if (!_rondaActiva) throw new InvalidOperationException("No hay una ronda activa. Llama a IniciarRonda() primero.");
        }

        private Stack<Carta> CrearMazoBarajado()
        {
            var cartas = new List<Carta>(52);
            foreach (Palo p in Enum.GetValues(typeof(Palo)))
            {
                cartas.Add(new Carta(p, Cara.A));
                for (int n = 2; n <= 10; n++) cartas.Add(new Carta(p, (Cara)Enum.Parse(typeof(Cara), $"C{n}")));
                cartas.Add(new Carta(p, Cara.J));
                cartas.Add(new Carta(p, Cara.Q));
                cartas.Add(new Carta(p, Cara.K));
            }

            for (int i = cartas.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (cartas[i], cartas[j]) = (cartas[j], cartas[i]);
            }

            return new Stack<Carta>(cartas);
        }

        private Carta Robar()
        {
            if (_mazo.Count == 0)
                _mazo = CrearMazoBarajado(); 

            return _mazo.Pop();
        }

        private bool Es17Suave(Mano mano)
        {
            int total = 0; int ases = 0;
            foreach (var c in mano.Cartas)
            {
                var cara = c.Cara switch
                {
                    Cara.A => "A",
                    Cara.J => "J",
                    Cara.Q => "Q",
                    Cara.K => "K",
                    _ => ((int)c.Cara).ToString().Replace("C", "")
                };
                int valor = _reglas.ValorCarta(cara);
                total += valor;
                if (c.Cara == Cara.A) ases++;
            }
            while (ases > 0)
            {
                if (total == 17) return true;
                total -= 10;
                ases--;
            }
            return false;
        }
    }
}
