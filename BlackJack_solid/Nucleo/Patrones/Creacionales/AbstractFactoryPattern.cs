using BlackJack_solid.Blackjack.Reglas;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Patrones.Creacionales
{
    /// <summary>
    /// ABSTRACT FACTORY PATTERN
    /// Define la interfaz para crear familias de objetos relacionados sin especificar sus clases concretas.
    /// Cada familia representa un conjunto completo de reglas coherentes para el juego.
    /// </summary>
    public interface IAbstractFactoryReglas
    {
        IReglasJuego CrearReglasJuego();
        IEstrategiaDealer CrearEstrategiaDealer();
        IPoliticaApuestas CrearPoliticaApuestas();
        string ObtenerNombreVariante();
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Estrategia de Dealer
    /// Define cómo debe comportarse el dealer según las reglas específicas.
    /// </summary>
    public interface IEstrategiaDealer
    {
        bool DebePedirCarta(int puntosActuales);
        int PuntoLimite();
        string ObtenerNombreEstrategia();
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Política de Apuestas
    /// Define los límites y reglas de apuestas según la variante del juego.
    /// </summary>
    public interface IPoliticaApuestas
    {
        double ApuestaMinima();
        double ApuestaMaxima();
        bool ValidarApuesta(double monto);
        double CalcularPagoBlackjack();
        string ObtenerNombrePolitica();
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Factory para Reglas Clásicas
    /// Crea un conjunto completo de reglas para Blackjack clásico/estándar.
    /// </summary>
    public sealed class FabricaReglasClasicas : IAbstractFactoryReglas
    {
        public IReglasJuego CrearReglasJuego()
        {
            // Abstract Factory: Crear reglas clásicas
            return new ReglasBlackjackClasicas();
        }

        public IEstrategiaDealer CrearEstrategiaDealer()
        {
            // Abstract Factory: Crear estrategia clásica (dealer se planta en 17)
            return new EstrategiaDealerClasica();
        }

        public IPoliticaApuestas CrearPoliticaApuestas()
        {
            // Abstract Factory: Crear política clásica
            return new PoliticaApuestasClasica();
        }

        public string ObtenerNombreVariante() => "Blackjack Clásico";
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Factory para Reglas Vegas
    /// Crea un conjunto completo de reglas para Blackjack estilo Las Vegas.
    /// </summary>
    public sealed class FabricaReglasVegas : IAbstractFactoryReglas
    {
        public IReglasJuego CrearReglasJuego()
        {
            // Abstract Factory: Crear reglas Vegas
            return new ReglasBlackjackVegas();
        }

        public IEstrategiaDealer CrearEstrategiaDealer()
        {
            // Abstract Factory: Crear estrategia Vegas (dealer se planta en 17, pero puede rendirse)
            return new EstrategiaDealerVegas();
        }

        public IPoliticaApuestas CrearPoliticaApuestas()
        {
            // Abstract Factory: Crear política Vegas (límites más altos)
            return new PoliticaApuestasVegas();
        }

        public string ObtenerNombreVariante() => "Blackjack Vegas";
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Factory para Reglas Europeas
    /// Crea un conjunto completo de reglas para Blackjack estilo europeo.
    /// </summary>
    public sealed class FabricaReglasEuropeas : IAbstractFactoryReglas
    {
        public IReglasJuego CrearReglasJuego()
        {
            // Abstract Factory: Crear reglas europeas
            return new ReglasBlackjackEuropeas();
        }

        public IEstrategiaDealer CrearEstrategiaDealer()
        {
            // Abstract Factory: Crear estrategia europea (dealer se planta en 17, sin carta oculta)
            return new EstrategiaDealerEuropea();
        }

        public IPoliticaApuestas CrearPoliticaApuestas()
        {
            // Abstract Factory: Crear política europea
            return new PoliticaApuestasEuropea();
        }

        public string ObtenerNombreVariante() => "Blackjack Europeo";
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Implementaciones de Reglas
    /// </summary>
    public sealed class ReglasBlackjackClasicas : IReglasJuego
    {
        public string NombreReglas => "Blackjack Clásico";
        public int MaximoPuntos => 21;

        public bool ValidarApuesta(double monto) => monto >= 1 && monto <= 1000;
        public int ValorCarta(string cara) => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.TryParse(cara, out var v) ? v : 0
        };
        public int CalcularPuntos(IEnumerable<string> cartas)
        {
            var total = cartas.Sum(carta => ValorCarta(carta));
            var ases = cartas.Count(carta => carta == "A");
            while (total > MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }
            return total;
        }
        public bool EsManoValida(IEnumerable<string> cartas) => CalcularPuntos(cartas) <= MaximoPuntos;
    }

    public sealed class ReglasBlackjackVegas : IReglasJuego
    {
        public string NombreReglas => "Blackjack Vegas";
        public int MaximoPuntos => 21;

        public bool ValidarApuesta(double monto) => monto >= 5 && monto <= 10000; // Límites más altos
        public int ValorCarta(string cara) => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.TryParse(cara, out var v) ? v : 0
        };
        public int CalcularPuntos(IEnumerable<string> cartas)
        {
            var total = cartas.Sum(carta => ValorCarta(carta));
            var ases = cartas.Count(carta => carta == "A");
            while (total > MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }
            return total;
        }
        public bool EsManoValida(IEnumerable<string> cartas) => CalcularPuntos(cartas) <= MaximoPuntos;
    }

    public sealed class ReglasBlackjackEuropeas : IReglasJuego
    {
        public string NombreReglas => "Blackjack Europeo";
        public int MaximoPuntos => 21;

        public bool ValidarApuesta(double monto) => monto >= 2 && monto <= 5000;
        public int ValorCarta(string cara) => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.TryParse(cara, out var v) ? v : 0
        };
        public int CalcularPuntos(IEnumerable<string> cartas)
        {
            var total = cartas.Sum(carta => ValorCarta(carta));
            var ases = cartas.Count(carta => carta == "A");
            while (total > MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }
            return total;
        }
        public bool EsManoValida(IEnumerable<string> cartas) => CalcularPuntos(cartas) <= MaximoPuntos;
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Implementaciones de Estrategias de Dealer
    /// </summary>
    public sealed class EstrategiaDealerClasica : IEstrategiaDealer
    {
        public bool DebePedirCarta(int puntosActuales) => puntosActuales < 17;
        public int PuntoLimite() => 17;
        public string ObtenerNombreEstrategia() => "Clásica (17)";
    }

    public sealed class EstrategiaDealerVegas : IEstrategiaDealer
    {
        public bool DebePedirCarta(int puntosActuales) => puntosActuales < 17;
        public int PuntoLimite() => 17;
        public string ObtenerNombreEstrategia() => "Vegas (17, puede rendirse)";
    }

    public sealed class EstrategiaDealerEuropea : IEstrategiaDealer
    {
        public bool DebePedirCarta(int puntosActuales) => puntosActuales < 17;
        public int PuntoLimite() => 17;
        public string ObtenerNombreEstrategia() => "Europea (17, sin carta oculta)";
    }

    /// <summary>
    /// ABSTRACT FACTORY PATTERN - Implementaciones de Políticas de Apuestas
    /// </summary>
    public sealed class PoliticaApuestasClasica : IPoliticaApuestas
    {
        public double ApuestaMinima() => 1.0;
        public double ApuestaMaxima() => 1000.0;
        public bool ValidarApuesta(double monto) => monto >= ApuestaMinima() && monto <= ApuestaMaxima();
        public double CalcularPagoBlackjack() => 1.5; // 3:2
        public string ObtenerNombrePolitica() => "Clásica";
    }

    public sealed class PoliticaApuestasVegas : IPoliticaApuestas
    {
        public double ApuestaMinima() => 5.0;
        public double ApuestaMaxima() => 10000.0;
        public bool ValidarApuesta(double monto) => monto >= ApuestaMinima() && monto <= ApuestaMaxima();
        public double CalcularPagoBlackjack() => 1.5; // 3:2
        public string ObtenerNombrePolitica() => "Vegas";
    }

    public sealed class PoliticaApuestasEuropea : IPoliticaApuestas
    {
        public double ApuestaMinima() => 2.0;
        public double ApuestaMaxima() => 5000.0;
        public bool ValidarApuesta(double monto) => monto >= ApuestaMinima() && monto <= ApuestaMaxima();
        public double CalcularPagoBlackjack() => 1.5; // 3:2
        public string ObtenerNombrePolitica() => "Europea";
    }
}
