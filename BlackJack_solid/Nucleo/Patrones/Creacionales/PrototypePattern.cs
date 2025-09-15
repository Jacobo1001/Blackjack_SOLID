using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;

namespace BlackJack_solid.Nucleo.Patrones.Creacionales
{
    /// <summary>
    /// PROTOTYPE PATTERN
    /// Define la interfaz para clonar objetos sin conocer sus clases concretas.
    /// Permite crear nuevos objetos copiando instancias existentes.
    /// </summary>
    public interface IPrototype<T>
    {
        T Clonar();
    }

    /// <summary>
    /// PROTOTYPE PATTERN - Carta clonable
    /// Permite clonar cartas para crear múltiples copias de la misma carta.
    /// Útil para crear barajas completas a partir de prototipos.
    /// </summary>
    public sealed record CartaPrototype(Palo Palo, Cara Cara) : Carta(Palo, Cara), IPrototype<CartaPrototype>
    {

        /// <summary>
        /// PROTOTYPE PATTERN: Crear una copia exacta de esta carta
        /// </summary>
        public CartaPrototype Clonar()
        {
            // Prototype: Crear nueva instancia con los mismos valores
            return new CartaPrototype(Palo, Cara);
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear múltiples clones de esta carta
        /// </summary>
        public CartaPrototype[] ClonarMultiples(int cantidad)
        {
            var clones = new CartaPrototype[cantidad];
            for (int i = 0; i < cantidad; i++)
            {
                clones[i] = Clonar();
            }
            return clones;
        }
    }

    /// <summary>
    /// PROTOTYPE PATTERN - Apuesta clonable
    /// Permite clonar apuestas base antes de aplicar decoradores.
    /// Útil para crear variaciones de apuestas sin modificar la original.
    /// </summary>
    public sealed class ApuestaPrototype : IApuesta, IPrototype<ApuestaPrototype>
    {
        public IJugador Jugador { get; }
        public double Monto { get; }
        public DateTime Fecha { get; }
        private bool _activa = true;

        public ApuestaPrototype(IJugador jugador, double monto)
        {
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Monto = monto;
            Fecha = DateTime.UtcNow;
        }

        public IJugador ObtenerJugador() => Jugador;
        public double ObtenerMonto() => Monto;
        public DateTime ObtenerFecha() => Fecha;
        public bool EstaActiva() => _activa;

        /// <summary>
        /// PROTOTYPE PATTERN: Crear una copia exacta de esta apuesta
        /// </summary>
        public ApuestaPrototype Clonar()
        {
            // Prototype: Crear nueva instancia con los mismos valores
            return new ApuestaPrototype(ObtenerJugador(), ObtenerMonto());
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear apuesta clonada con nuevo monto
        /// </summary>
        public ApuestaPrototype ClonarConNuevoMonto(double nuevoMonto)
        {
            return new ApuestaPrototype(ObtenerJugador(), nuevoMonto);
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear apuesta clonada con nuevo jugador
        /// </summary>
        public ApuestaPrototype ClonarConNuevoJugador(IJugador nuevoJugador)
        {
            return new ApuestaPrototype(nuevoJugador, ObtenerMonto());
        }
    }

    /// <summary>
    /// PROTOTYPE PATTERN - Factory de prototipos
    /// Centraliza la creación y gestión de prototipos para el juego.
    /// </summary>
    public sealed class PrototypeFactory
    {
        private readonly Dictionary<string, CartaPrototype> _prototiposCartas = new();
        private readonly Dictionary<string, ApuestaPrototype> _prototiposApuestas = new();

        /// <summary>
        /// PROTOTYPE PATTERN: Registrar prototipo de carta
        /// </summary>
        public void RegistrarPrototipoCarta(string nombre, CartaPrototype prototipo)
        {
            _prototiposCartas[nombre] = prototipo;
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear carta desde prototipo registrado
        /// </summary>
        public CartaPrototype CrearCartaDesdePrototipo(string nombre)
        {
            if (_prototiposCartas.TryGetValue(nombre, out var prototipo))
            {
                return prototipo.Clonar();
            }
            throw new ArgumentException($"Prototipo de carta '{nombre}' no encontrado");
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear baraja completa desde prototipos
        /// </summary>
        public List<CartaPrototype> CrearBarajaCompletaDesdePrototipos()
        {
            var baraja = new List<CartaPrototype>();
            
            // Crear prototipos base si no existen
            if (!_prototiposCartas.ContainsKey("As_Corazones"))
            {
                InicializarPrototiposBase();
            }

            // Clonar cada prototipo para crear la baraja completa
            foreach (var prototipo in _prototiposCartas.Values)
            {
                baraja.Add(prototipo.Clonar());
            }

            return baraja;
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Inicializar prototipos base de cartas
        /// </summary>
        private void InicializarPrototiposBase()
        {
            foreach (Palo palo in Enum.GetValues<Palo>())
            {
                foreach (Cara cara in Enum.GetValues<Cara>())
                {
                    var nombre = $"{cara}_{palo}";
                    var prototipo = new CartaPrototype(palo, cara);
                    _prototiposCartas[nombre] = prototipo;
                }
            }
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Registrar prototipo de apuesta
        /// </summary>
        public void RegistrarPrototipoApuesta(string nombre, ApuestaPrototype prototipo)
        {
            _prototiposApuestas[nombre] = prototipo;
        }

        /// <summary>
        /// PROTOTYPE PATTERN: Crear apuesta desde prototipo registrado
        /// </summary>
        public ApuestaPrototype CrearApuestaDesdePrototipo(string nombre, IJugador jugador, double monto)
        {
            if (_prototiposApuestas.TryGetValue(nombre, out var prototipo))
            {
                var clon = prototipo.Clonar();
                // Ajustar jugador y monto del clon
                return new ApuestaPrototype(jugador, monto);
            }
            throw new ArgumentException($"Prototipo de apuesta '{nombre}' no encontrado");
        }
    }
}
