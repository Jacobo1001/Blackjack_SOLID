using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Patrones.Estructurales
{
    // DECORATOR PATTERN
    // Permite agregar comportamiento a objetos dinámicamente sin alterar su estructura.
    // En Blackjack, permite combinar diferentes tipos de apuestas (base + seguro + doble + etc.)
    
    // DECORATOR PATTERN - Componente base para apuestas
    // Define la interfaz común para todas las apuestas, tanto simples como decoradas.
    public abstract class ApuestaDecoratorBase : IApuesta
    {
        protected readonly IApuesta _apuesta;

        protected ApuestaDecoratorBase(IApuesta apuesta)
        {
            _apuesta = apuesta ?? throw new ArgumentNullException(nameof(apuesta));
        }

        // DECORATOR PATTERN: Delegar métodos básicos al componente decorado
        public virtual IJugador ObtenerJugador() => _apuesta.ObtenerJugador();
        public virtual double ObtenerMonto() => _apuesta.ObtenerMonto();
        public virtual DateTime ObtenerFecha() => _apuesta.ObtenerFecha();
        public virtual bool EstaActiva() => _apuesta.EstaActiva();

        // DECORATOR PATTERN: Métodos que pueden ser extendidos por decoradores específicos
        public virtual string ObtenerTipoApuesta() => _apuesta.GetType().Name;
        public virtual double CalcularPago(double multiplicadorBase) => ObtenerMonto() * multiplicadorBase;
        public virtual string ObtenerDescripcion() => $"Apuesta básica de ${ObtenerMonto()}";
    }

    // DECORATOR PATTERN - Implementación base de apuesta
    // Representa la apuesta básica sin decoraciones adicionales.
    public sealed class ApuestaBase : IApuesta
    {
        public IJugador Jugador { get; }
        public double Monto { get; }
        public DateTime Fecha { get; }
        private bool _activa = true;

        public ApuestaBase(IJugador jugador, double monto, DateTime? fecha = null)
        {
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Monto = monto;
            Fecha = fecha ?? DateTime.UtcNow;
        }

        public IJugador ObtenerJugador() => Jugador;
        public double ObtenerMonto() => Monto;
        public DateTime ObtenerFecha() => Fecha;
        public bool EstaActiva() => _activa;

        public ApuestaBase Cerrar()
        {
            _activa = false;
            return this;
        }
    }

    // DECORATOR PATTERN - Decorador para apuesta con seguro
    // Agrega la funcionalidad de seguro a cualquier apuesta base.
    public sealed class ApuestaConSeguro : ApuestaDecoratorBase
    {
        private readonly double _montoSeguro;

        public ApuestaConSeguro(IApuesta apuesta, double montoSeguro) : base(apuesta)
        {
            _montoSeguro = montoSeguro;
        }

        // DECORATOR PATTERN: Extender el monto total incluyendo el seguro
        public override double ObtenerMonto() => _apuesta.ObtenerMonto() + _montoSeguro;

        // DECORATOR PATTERN: Extender la descripción con información del seguro
        public override string ObtenerDescripcion() => 
            $"Apuesta básica de ${_apuesta.ObtenerMonto()} + Seguro ${_montoSeguro}";

        // DECORATOR PATTERN: Extender el tipo de apuesta
        public override string ObtenerTipoApuesta() => "Apuesta con Seguro";

        // DECORATOR PATTERN: Calcular pago considerando el seguro
        public override double CalcularPago(double multiplicadorBase)
        {
            // El seguro se paga 2:1 si el dealer tiene blackjack
            return (_apuesta.ObtenerMonto() * multiplicadorBase) + (_montoSeguro * 2);
        }

        public double ObtenerMontoSeguro() => _montoSeguro;
    }

    // DECORATOR PATTERN - Decorador para apuesta doble
    // Permite doblar la apuesta original y recibir solo una carta adicional.
    public sealed class ApuestaDoble : ApuestaDecoratorBase
    {
        public ApuestaDoble(IApuesta apuesta) : base(apuesta)
        {
            // DECORATOR PATTERN: Validar que la apuesta base sea válida para doblar
            if (apuesta.ObtenerMonto() <= 0)
                throw new ArgumentException("No se puede doblar una apuesta de monto cero o negativo");
        }

        // DECORATOR PATTERN: Doblar el monto de la apuesta
        public override double ObtenerMonto() => _apuesta.ObtenerMonto() * 2;

        // DECORATOR PATTERN: Extender la descripción
        public override string ObtenerDescripcion() => 
            $"Apuesta básica de ${_apuesta.ObtenerMonto()} (DOBLADA)";

        // DECORATOR PATTERN: Extender el tipo de apuesta
        public override string ObtenerTipoApuesta() => "Apuesta Doble";

        // DECORATOR PATTERN: Calcular pago doblado
        public override double CalcularPago(double multiplicadorBase)
        {
            return (_apuesta.ObtenerMonto() * multiplicadorBase) * 2;
        }
    }

    // DECORATOR PATTERN - Decorador para apuesta de rendirse
    // Permite rendirse y recuperar la mitad de la apuesta.
    public sealed class ApuestaRendirse : ApuestaDecoratorBase
    {
        public ApuestaRendirse(IApuesta apuesta) : base(apuesta)
        {
        }

        // DECORATOR PATTERN: El monto no cambia al rendirse
        public override double ObtenerMonto() => _apuesta.ObtenerMonto();

        // DECORATOR PATTERN: Extender la descripción
        public override string ObtenerDescripcion() => 
            $"Apuesta básica de ${_apuesta.ObtenerMonto()} (RENDIRSE)";

        // DECORATOR PATTERN: Extender el tipo de apuesta
        public override string ObtenerTipoApuesta() => "Apuesta Rendirse";

        // DECORATOR PATTERN: Calcular pago de rendirse (50% de la apuesta)
        public override double CalcularPago(double multiplicadorBase)
        {
            return _apuesta.ObtenerMonto() * 0.5; // Recuperar la mitad
        }
    }

    // DECORATOR PATTERN - Decorador para apuesta lateral (side bet)
    // Agrega apuestas adicionales como "Perfect Pairs" o "21+3".
    public sealed class ApuestaSideBet : ApuestaDecoratorBase
    {
        private readonly double _montoSideBet;
        private readonly string _tipoSideBet;

        public ApuestaSideBet(IApuesta apuesta, double montoSideBet, string tipoSideBet) : base(apuesta)
        {
            _montoSideBet = montoSideBet;
            _tipoSideBet = tipoSideBet;
        }

        // DECORATOR PATTERN: Agregar el monto del side bet
        public override double ObtenerMonto() => _apuesta.ObtenerMonto() + _montoSideBet;

        // DECORATOR PATTERN: Extender la descripción con side bet
        public override string ObtenerDescripcion() => 
            $"Apuesta básica de ${_apuesta.ObtenerMonto()} + {_tipoSideBet} ${_montoSideBet}";

        // DECORATOR PATTERN: Extender el tipo de apuesta
        public override string ObtenerTipoApuesta() => $"Apuesta con {_tipoSideBet}";

        // DECORATOR PATTERN: Calcular pago incluyendo side bet
        public override double CalcularPago(double multiplicadorBase)
        {
            var pagoBase = _apuesta.ObtenerMonto() * multiplicadorBase;
            var pagoSideBet = CalcularPagoSideBet();
            return pagoBase + pagoSideBet;
        }

        private double CalcularPagoSideBet()
        {
            // DECORATOR PATTERN: Lógica específica para cada tipo de side bet
            return _tipoSideBet switch
            {
                "Perfect Pairs" => _montoSideBet * 25, // 25:1
                "21+3" => _montoSideBet * 9,           // 9:1
                "Insurance" => _montoSideBet * 2,       // 2:1
                _ => _montoSideBet * 1                 // 1:1 por defecto
            };
        }

        public double ObtenerMontoSideBet() => _montoSideBet;
        public string ObtenerTipoSideBet() => _tipoSideBet;
    }

    // DECORATOR PATTERN - Factory para crear apuestas decoradas
    // Simplifica la creación de apuestas con múltiples decoradores.
    public static class ApuestaDecoratorFactory
    {
        // DECORATOR PATTERN: Crear apuesta base
        public static IApuesta CrearApuestaBase(IJugador jugador, double monto)
        {
            return new ApuestaBase(jugador, monto);
        }

        // DECORATOR PATTERN: Crear apuesta con seguro
        public static IApuesta CrearApuestaConSeguro(IApuesta apuesta, double montoSeguro)
        {
            return new ApuestaConSeguro(apuesta, montoSeguro);
        }

        // DECORATOR PATTERN: Crear apuesta doble
        public static IApuesta CrearApuestaDoble(IApuesta apuesta)
        {
            return new ApuestaDoble(apuesta);
        }

        // DECORATOR PATTERN: Crear apuesta de rendirse
        public static IApuesta CrearApuestaRendirse(IApuesta apuesta)
        {
            return new ApuestaRendirse(apuesta);
        }

        // DECORATOR PATTERN: Crear apuesta con side bet
        public static IApuesta CrearApuestaConSideBet(IApuesta apuesta, double montoSideBet, string tipoSideBet)
        {
            return new ApuestaSideBet(apuesta, montoSideBet, tipoSideBet);
        }

        // DECORATOR PATTERN: Crear apuesta compleja con múltiples decoradores
        // Ejemplo: Apuesta base + seguro + side bet
        public static IApuesta CrearApuestaCompleja(IJugador jugador, double montoBase, double montoSeguro, double montoSideBet, string tipoSideBet)
        {
            var apuestaBase = CrearApuestaBase(jugador, montoBase);
            var conSeguro = CrearApuestaConSeguro(apuestaBase, montoSeguro);
            var conSideBet = CrearApuestaConSideBet(conSeguro, montoSideBet, tipoSideBet);
            return conSideBet;
        }
    }
}
