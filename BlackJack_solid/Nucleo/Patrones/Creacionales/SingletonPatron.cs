using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using System.Collections.Concurrent;

namespace BlackJack_solid.Nucleo.Patrones.Creacionales
{
    // SINGLETON PATTERN
    // Asegura que solo exista una instancia de GestorMesas en toda la aplicación.
    // Centraliza la creación y registro de todas las mesas del casino.
    public sealed class GestorMesas
    {
        private static volatile GestorMesas? _instancia;
        private static readonly object _lock = new object();
        
        private readonly ConcurrentDictionary<int, IMesa> _mesas = new();
        private readonly ConcurrentDictionary<int, IDealer> _dealers = new();
        private int _contadorMesas = 0;
        private int _contadorDealers = 0;

        // SINGLETON PATTERN: Constructor privado para prevenir instanciación externa
        private GestorMesas()
        {
            // Singleton: Constructor privado para controlar la instanciación
        }

        // SINGLETON PATTERN: Propiedad estática que devuelve la única instancia
        // Implementa lazy initialization con double-checked locking para thread-safety
        public static GestorMesas Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    lock (_lock)
                    {
                        if (_instancia == null)
                        {
                            // Singleton: Crear la única instancia
                            _instancia = new GestorMesas();
                        }
                    }
                }
                return _instancia;
            }
        }

        // SINGLETON PATTERN: Crear nueva mesa usando Factory Method
        public IMesa CrearMesa(IReglasJuego reglas, IBlackjackFactory factory)
        {
            var id = Interlocked.Increment(ref _contadorMesas);
            var mesa = factory.CrearMesa(id, reglas);
            
            // Singleton: Registrar mesa en el gestor central
            _mesas.TryAdd(id, mesa);
            
            return mesa;
        }

        // SINGLETON PATTERN: Crear nuevo dealer usando Factory Method
        public IDealer CrearDealer(IReglasJuego reglas, IBlackjackFactory factory)
        {
            var id = Interlocked.Increment(ref _contadorDealers);
            var dealer = factory.CrearDealer(reglas);
            
            // Singleton: Registrar dealer en el gestor central
            _dealers.TryAdd(id, dealer);
            
            return dealer;
        }

        // SINGLETON PATTERN: Obtener mesa por ID
        public IMesa? ObtenerMesa(int id)
        {
            _mesas.TryGetValue(id, out var mesa);
            return mesa;
        }

        // SINGLETON PATTERN: Obtener todas las mesas
        public IReadOnlyCollection<IMesa> ObtenerTodasLasMesas()
        {
            return _mesas.Values.ToList().AsReadOnly();
        }

        // SINGLETON PATTERN: Obtener mesas por estado
        public IEnumerable<IMesa> ObtenerMesasPorEstado(string estado)
        {
            return _mesas.Values.Where(m => m.ObtenerEstado() == estado);
        }

        // SINGLETON PATTERN: Asignar dealer a mesa
        public bool AsignarDealerAMesa(int idMesa, IDealer dealer)
        {
            if (_mesas.TryGetValue(idMesa, out var mesa))
            {
                mesa.AsignarDealer(dealer);
                return true;
            }
            return false;
        }

        // SINGLETON PATTERN: Cerrar mesa
        public bool CerrarMesa(int idMesa)
        {
            if (_mesas.TryGetValue(idMesa, out var mesa))
            {
                // Singleton: Remover mesa del gestor central
                _mesas.TryRemove(idMesa, out _);
                return true;
            }
            return false;
        }

        // SINGLETON PATTERN: Obtener estadísticas del gestor
        public EstadisticasGestor ObtenerEstadisticas()
        {
            return new EstadisticasGestor
            {
                TotalMesas = _mesas.Count,
                TotalDealers = _dealers.Count,
                MesasAbiertas = _mesas.Values.Count(m => m.ObtenerEstado() == "ABIERTA"),
                MesasEnRonda = _mesas.Values.Count(m => m.ObtenerEstado() == "EnRonda"),
                MesasCerradas = _mesas.Values.Count(m => m.ObtenerEstado() == "CERRADA")
            };
        }

        // SINGLETON PATTERN: Limpiar todas las mesas (para testing)
        public void LimpiarTodasLasMesas()
        {
            _mesas.Clear();
            _dealers.Clear();
            _contadorMesas = 0;
            _contadorDealers = 0;
        }
    }

    // SINGLETON PATTERN - Clase para estadísticas del gestor
    public sealed class EstadisticasGestor
    {
        public int TotalMesas { get; set; }
        public int TotalDealers { get; set; }
        public int MesasAbiertas { get; set; }
        public int MesasEnRonda { get; set; }
        public int MesasCerradas { get; set; }

        public override string ToString()
        {
            return $"Gestor Mesas - Total: {TotalMesas}, Abiertas: {MesasAbiertas}, En Ronda: {MesasEnRonda}, Cerradas: {MesasCerradas}";
        }
    }
}
