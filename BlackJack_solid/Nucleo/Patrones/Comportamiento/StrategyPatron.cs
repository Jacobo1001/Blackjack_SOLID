using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;
using System;
using System.Collections.Generic;

namespace BlackJack_solid.Nucleo.Patrones.Comportamiento
{

    // Define una familia de algoritmos, los encapsula y los hace intercambiables.
    // En Blackjack, permite diferentes estrategias de juego para jugadores y dealers.

    //Interfaz para estrategias de decisión del jugador

    public interface IEstrategiaJugador
    {
        // Decidir qué acción tomar basada en la mano actual

        AccionJugada DecidirAccion(Mano manoJugador, Mano manoDealer, double saldoDisponible, double apuestaActual);

        // Decidir el monto de la apuesta
        double DecidirApuesta(double saldoDisponible, int numeroRonda);

        // Decidir si doblar la apuesta
        bool DecidirDoblar(Mano manoJugador, Mano manoDealer, double saldoDisponible);

        // Decidir si rendirse
        bool DecidirRendirse(Mano manoJugador, Mano manoDealer);

        // Obtener nombre de la estrategia
        string ObtenerNombre();
    }

    //Interfaz para estrategias de decisión del dealer
    public interface IEstrategiaDealer
    {
        // Decidir si el dealer debe pedir otra carta
        bool DebePedirCarta(Mano manoDealer, Mano manoJugador);

        // Obtener nombre de la estrategia
        string ObtenerNombre();
    }

    // Estrategia conservadora para jugadores
    // Juega de manera segura, evitando riesgos innecesarios.
    public sealed class EstrategiaJugadorConservadora : IEstrategiaJugador
    {
        public AccionJugada DecidirAccion(Mano manoJugador, Mano manoDealer, double saldoDisponible, double apuestaActual)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"STRATEGY PATTERN: Estrategia Conservadora - Jugador: {puntosJugador}, Dealer: {puntosDealer}");

            // Lógica conservadora
            if (puntosJugador >= 17)
            {
                Console.WriteLine("✅ Estrategia Conservadora: Se planta en 17+");
                return AccionJugada.Plantarse;
            }

            if (puntosJugador <= 11)
            {
                Console.WriteLine("✅ Estrategia Conservadora: Pide carta con 11 o menos");
                return AccionJugada.Pedir;
            }

            // Entre 12-16, depende de la carta visible del dealer
            if (puntosDealer >= 7)
            {
                Console.WriteLine("✅ Estrategia Conservadora: Dealer fuerte, pide carta");
                return AccionJugada.Pedir;
            }
            else
            {
                Console.WriteLine("✅ Estrategia Conservadora: Dealer débil, se planta");
                return AccionJugada.Plantarse;
            }
        }

        public double DecidirApuesta(double saldoDisponible, int numeroRonda)
        {
            // Apuesta conservadora (2-5% del saldo)
            var apuesta = Math.Min(saldoDisponible * 0.03, 50.0);
            Console.WriteLine($"✅ Estrategia Conservadora: Apuesta ${apuesta:F2} (3% del saldo)");
            return apuesta;
        }

        public bool DecidirDoblar(Mano manoJugador, Mano manoDealer, double saldoDisponible)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // Solo dobla en situaciones muy favorables
            bool doblar = (puntosJugador == 11) || 
                        (puntosJugador == 10 && puntosDealer < 10) ||
                        (puntosJugador == 9 && puntosDealer >= 3 && puntosDealer <= 6);

            Console.WriteLine($"✅ Estrategia Conservadora: {(doblar ? "Dobla" : "No dobla")} apuesta");
            return doblar;
        }

        public bool DecidirRendirse(Mano manoJugador, Mano manoDealer)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // Solo se rinde en situaciones muy desfavorables
            bool rendirse = puntosJugador == 16 && puntosDealer >= 9;

            Console.WriteLine($"✅ Estrategia Conservadora: {(rendirse ? "Se rinde" : "No se rinde")}");
            return rendirse;
        }

        public string ObtenerNombre() => "Conservadora";

        private int CalcularPuntos(Mano mano)
        {
            return mano.Cartas.Sum(c => c.Cara == Cara.A ? 11 : (int)c.Cara);
        }
    }

    // Estrategia agresiva para jugadores
    // Toma más riesgos para maximizar las ganancias.
    public sealed class EstrategiaJugadorAgresiva : IEstrategiaJugador
    {
        public AccionJugada DecidirAccion(Mano manoJugador, Mano manoDealer, double saldoDisponible, double apuestaActual)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"STRATEGY PATTERN: Estrategia Agresiva - Jugador: {puntosJugador}, Dealer: {puntosDealer}");

            // Lógica agresiva
            if (puntosJugador >= 19)
            {
                Console.WriteLine("✅ Estrategia Agresiva: Se planta en 19+");
                return AccionJugada.Plantarse;
            }

            if (puntosJugador <= 13)
            {
                Console.WriteLine("✅ Estrategia Agresiva: Pide carta con 13 o menos");
                return AccionJugada.Pedir;
            }

            // Entre 14-18, siempre pide si el dealer es fuerte
            if (puntosDealer >= 6)
            {
                Console.WriteLine("✅ Estrategia Agresiva: Dealer fuerte, pide carta");
                return AccionJugada.Pedir;
            }
            else
            {
                Console.WriteLine("✅ Estrategia Agresiva: Dealer débil, se planta");
                return AccionJugada.Plantarse;
            }
        }

        public double DecidirApuesta(double saldoDisponible, int numeroRonda)
        {
            // Apuesta agresiva (5-10% del saldo)
            var apuesta = Math.Min(saldoDisponible * 0.08, 100.0);
            Console.WriteLine($"✅ Estrategia Agresiva: Apuesta ${apuesta:F2} (8% del saldo)");
            return apuesta;
        }

        public bool DecidirDoblar(Mano manoJugador, Mano manoDealer, double saldoDisponible)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // Dobla en más situaciones
            bool doblar = (puntosJugador >= 9 && puntosJugador <= 11) ||
                         (puntosJugador == 8 && puntosDealer >= 5 && puntosDealer <= 6);

            Console.WriteLine($"✅ Estrategia Agresiva: {(doblar ? "Dobla" : "No dobla")} apuesta");
            return doblar;
        }

        public bool DecidirRendirse(Mano manoJugador, Mano manoDealer)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // STRATEGY PATTERN: Se rinde en más situaciones
            bool rendirse = (puntosJugador == 16 && puntosDealer >= 8) ||
                           (puntosJugador == 15 && puntosDealer >= 9);

            Console.WriteLine($"✅ Estrategia Agresiva: {(rendirse ? "Se rinde" : "No se rinde")}");
            return rendirse;
        }

        public string ObtenerNombre() => "Agresiva";

        private int CalcularPuntos(Mano mano)
        {
            return mano.Cartas.Sum(c => c.Cara == Cara.A ? 11 : (int)c.Cara);
        }
    }

    /// STRATEGY PATTERN - Estrategia básica (matemáticamente óptima)
    /// Basada en la estrategia básica del Blackjack.
    public sealed class EstrategiaJugadorBasica : IEstrategiaJugador
    {
        public AccionJugada DecidirAccion(Mano manoJugador, Mano manoDealer, double saldoDisponible, double apuestaActual)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"STRATEGY PATTERN: Estrategia Básica - Jugador: {puntosJugador}, Dealer: {puntosDealer}");

            // STRATEGY PATTERN: Estrategia básica matemáticamente óptima
            if (puntosJugador >= 17)
            {
                Console.WriteLine("✅ Estrategia Básica: Se planta en 17+");
                return AccionJugada.Plantarse;
            }

            if (puntosJugador <= 11)
            {
                Console.WriteLine("✅ Estrategia Básica: Pide carta con 11 o menos");
                return AccionJugada.Pedir;
            }

            // STRATEGY PATTERN: Estrategia básica para manos duras
            if (puntosJugador == 12)
            {
                bool pedir = puntosDealer >= 4 && puntosDealer <= 6;
                Console.WriteLine($"✅ Estrategia Básica: {(pedir ? "Pide" : "Se planta")} carta con 12");
                return pedir ? AccionJugada.Pedir : AccionJugada.Plantarse;
            }

            if (puntosJugador >= 13 && puntosJugador <= 16)
            {
                bool pedir = puntosDealer >= 7;
                Console.WriteLine($"✅ Estrategia Básica: {(pedir ? "Pide" : "Se planta")} carta con {puntosJugador}");
                return pedir ? AccionJugada.Pedir : AccionJugada.Plantarse;
            }

            return AccionJugada.Plantarse;
        }

        public double DecidirApuesta(double saldoDisponible, int numeroRonda)
        {
            // STRATEGY PATTERN: Apuesta básica (3-7% del saldo)
            var apuesta = Math.Min(saldoDisponible * 0.05, 75.0);
            Console.WriteLine($"✅ Estrategia Básica: Apuesta ${apuesta:F2} (5% del saldo)");
            return apuesta;
        }

        public bool DecidirDoblar(Mano manoJugador, Mano manoDealer, double saldoDisponible)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // STRATEGY PATTERN: Estrategia básica para doblar
            bool doblar = (puntosJugador == 11) ||
                         (puntosJugador == 10 && puntosDealer < 10) ||
                         (puntosJugador == 9 && puntosDealer >= 3 && puntosDealer <= 6);

            Console.WriteLine($"✅ Estrategia Básica: {(doblar ? "Dobla" : "No dobla")} apuesta");
            return doblar;
        }

        public bool DecidirRendirse(Mano manoJugador, Mano manoDealer)
        {
            var puntosJugador = CalcularPuntos(manoJugador);
            var puntosDealer = CalcularPuntos(manoDealer);

            // STRATEGY PATTERN: Estrategia básica para rendirse
            bool rendirse = puntosJugador == 16 && puntosDealer >= 9;

            Console.WriteLine($"✅ Estrategia Básica: {(rendirse ? "Se rinde" : "No se rinde")}");
            return rendirse;
        }

        public string ObtenerNombre() => "Básica";

        private int CalcularPuntos(Mano mano)
        {
            return mano.Cartas.Sum(c => c.Cara == Cara.A ? 11 : (int)c.Cara);
        }
    }

    // STRATEGY PATTERN - Estrategia clásica del dealer
    // El dealer siempre pide carta hasta llegar a 17.
    public sealed class EstrategiaDealerClasico : IEstrategiaDealer
    {
        public bool DebePedirCarta(Mano manoDealer, Mano manoJugador)
        {
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"STRATEGY PATTERN: Estrategia Dealer Clásico - Dealer: {puntosDealer}");

            // STRATEGY PATTERN: Dealer clásico pide hasta 17
            bool pedir = puntosDealer < 17;
            Console.WriteLine($"✅ Estrategia Dealer Clásico: {(pedir ? "Pide" : "Se planta")} carta");
            return pedir;
        }

        public string ObtenerNombre() => "Clásico";

        private int CalcularPuntos(Mano mano)
        {
            return mano.Cartas.Sum(c => c.Cara == Cara.A ? 11 : (int)c.Cara);
        }
    }

    // STRATEGY PATTERN - Estrategia agresiva del dealer
    // El dealer pide carta hasta llegar a 18.
    public sealed class EstrategiaDealerAgresivo : IEstrategiaDealer
    {
        public bool DebePedirCarta(Mano manoDealer, Mano manoJugador)
        {
            var puntosDealer = CalcularPuntos(manoDealer);

            Console.WriteLine($"STRATEGY PATTERN: Estrategia Dealer Agresivo - Dealer: {puntosDealer}");

            // STRATEGY PATTERN: Dealer agresivo pide hasta 18
            bool pedir = puntosDealer < 18;
            Console.WriteLine($"✅ Estrategia Dealer Agresivo: {(pedir ? "Pide" : "Se planta")} carta");
            return pedir;
        }

        public string ObtenerNombre() => "Agresivo";

        private int CalcularPuntos(Mano mano)
        {
            return mano.Cartas.Sum(c => c.Cara == Cara.A ? 11 : (int)c.Cara);
        }
    }

    // STRATEGY PATTERN - Contexto que usa las estrategias
    public sealed class ContextoJugador
    {
        private IEstrategiaJugador _estrategia;

        public ContextoJugador(IEstrategiaJugador estrategia)
        {
            _estrategia = estrategia ?? throw new ArgumentNullException(nameof(estrategia));
            Console.WriteLine($"STRATEGY PATTERN: ContextoJugador inicializado con estrategia '{estrategia.ObtenerNombre()}'");
        }

        // STRATEGY PATTERN: Cambiar la estrategia en tiempo de ejecución
        public void CambiarEstrategia(IEstrategiaJugador nuevaEstrategia)
        {
            _estrategia = nuevaEstrategia ?? throw new ArgumentNullException(nameof(nuevaEstrategia));
            Console.WriteLine($"STRATEGY PATTERN: Estrategia cambiada a '{nuevaEstrategia.ObtenerNombre()}'");
        }

        // STRATEGY PATTERN: Delegar las decisiones a la estrategia actual
        public AccionJugada DecidirAccion(Mano manoJugador, Mano manoDealer, double saldoDisponible, double apuestaActual)
        {
            return _estrategia.DecidirAccion(manoJugador, manoDealer, saldoDisponible, apuestaActual);
        }

        public double DecidirApuesta(double saldoDisponible, int numeroRonda)
        {
            return _estrategia.DecidirApuesta(saldoDisponible, numeroRonda);
        }

        public bool DecidirDoblar(Mano manoJugador, Mano manoDealer, double saldoDisponible)
        {
            return _estrategia.DecidirDoblar(manoJugador, manoDealer, saldoDisponible);
        }

        public bool DecidirRendirse(Mano manoJugador, Mano manoDealer)
        {
            return _estrategia.DecidirRendirse(manoJugador, manoDealer);
        }

        public string ObtenerNombreEstrategia() => _estrategia.ObtenerNombre();
    }

    // STRATEGY PATTERN - Contexto para el dealer
    public sealed class ContextoDealer
    {
        private IEstrategiaDealer _estrategia;

        public ContextoDealer(IEstrategiaDealer estrategia)
        {
            _estrategia = estrategia ?? throw new ArgumentNullException(nameof(estrategia));
            Console.WriteLine($"STRATEGY PATTERN: ContextoDealer inicializado con estrategia '{estrategia.ObtenerNombre()}'");
        }

        // STRATEGY PATTERN: Cambiar la estrategia del dealer
        public void CambiarEstrategia(IEstrategiaDealer nuevaEstrategia)
        {
            _estrategia = nuevaEstrategia ?? throw new ArgumentNullException(nameof(nuevaEstrategia));
            Console.WriteLine($"STRATEGY PATTERN: Estrategia del dealer cambiada a '{nuevaEstrategia.ObtenerNombre()}'");
        }

        //Delegar la decisión a la estrategia actual

        public bool DebePedirCarta(Mano manoDealer, Mano manoJugador)
        {
            return _estrategia.DebePedirCarta(manoDealer, manoJugador);
        }

        public string ObtenerNombreEstrategia() => _estrategia.ObtenerNombre();
    }
}
