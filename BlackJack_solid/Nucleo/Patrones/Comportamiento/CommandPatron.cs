using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Nucleo.Patrones.Comportamiento
{
    // Implementación del patrón Command para encapsular acciones de Blackjack como objetos comando
    // Permite parametrizar, deshacer y componer acciones de juego
    public interface IComando
    {
        void Ejecutar();
        void Deshacer();
        bool SePuedeDeshacer();
        string ObtenerDescripcion();
    }

    // Interfaz para comandos que requieren parámetros, hacemos que los comandos sean genéricos y reutilizables
    public interface IComandoConParametros<T> : IComando
    {
        void EstablecerParametros(T parametros);
    }

    // Comando para pedir una carta al dealer.
    public sealed class ComandoPedirCarta : IComandoConParametros<ParametrosPedirCarta>
    {
        private readonly IDealer _dealer;
        private readonly IJugador _jugador;
        private ParametrosPedirCarta? _parametros;
        private Carta? _cartaEntregada;

        public ComandoPedirCarta(IDealer dealer, IJugador jugador)
        {
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
        }

        // Establece los parámetros necesarios para ejecutar el comando
        public void EstablecerParametros(ParametrosPedirCarta parametros)
        {
            _parametros = parametros ?? throw new ArgumentNullException(nameof(parametros));
        }

        // Ejecuta el comando de pedir carta
        public void Ejecutar()
        {
            if (_parametros == null)
                throw new InvalidOperationException("Parámetros no establecidos");

            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Pedir Carta' para {_jugador.ObtenerNombre()}");
            Console.WriteLine("Simulando entrega de carta al jugador...");
            _cartaEntregada = new Carta(Palo.Corazones, Cara.C10);
            Console.WriteLine($"Carta entregada: {ObtenerNombreCara(_cartaEntregada.Cara)} de {_cartaEntregada.Palo}");
        }

        // Deshace el comando de pedir carta
        public void Deshacer()
        {
            if (!SePuedeDeshacer())
                throw new InvalidOperationException("No se puede deshacer este comando");
            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando 'Pedir Carta' para {_jugador.ObtenerNombre()}");
            Console.WriteLine("Simulando remoción de carta del jugador...");
            Console.WriteLine($"Carta removida: {(_cartaEntregada != null ? ObtenerNombreCara(_cartaEntregada.Cara) : "?")} de {_cartaEntregada?.Palo}");
        }

        // Indica si el comando se puede deshacer
        public bool SePuedeDeshacer()
        {
            return _cartaEntregada != null;
        }

        // Proporciona una descripción del comando
        public string ObtenerDescripcion()
        {
            return $"Pedir carta para {_jugador.ObtenerNombre()}";
        }

        // Método auxiliar para mostrar la cara de la carta de forma amigable
        private static string ObtenerNombreCara(Cara cara)
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
    }

    
    // Comando para plantarse (no pedir más cartas).

    public sealed class ComandoPlantarse : IComando
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private bool _ejecutado = false;

        /// <summary>
        /// Inicializa el comando con el jugador y el dealer.
        /// </summary>
        public ComandoPlantarse(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <inheritdoc/>
        public void Ejecutar()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Plantarse' para {_jugador.ObtenerNombre()}");
            _ejecutado = true;
            Console.WriteLine($"{_jugador.ObtenerNombre()} se plantó");
        }

        /// <inheritdoc/>
        public void Deshacer()
        {
            throw new InvalidOperationException("No se puede deshacer 'Plantarse'");
        }

        /// <inheritdoc/>
        public bool SePuedeDeshacer()
        {
            return false;
        }

        /// <inheritdoc/>
        public string ObtenerDescripcion()
        {
            return $"Plantarse para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// Comando para doblar la apuesta.
    /// </summary>
    public sealed class ComandoDoblarApuesta : IComandoConParametros<ParametrosDoblarApuesta>
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private ParametrosDoblarApuesta? _parametros;
        private double _apuestaOriginal;

        /// <summary>
        /// Inicializa el comando con el jugador y el dealer.
        /// </summary>
        public ComandoDoblarApuesta(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <inheritdoc/>
        public void EstablecerParametros(ParametrosDoblarApuesta parametros)
        {
            _parametros = parametros ?? throw new ArgumentNullException(nameof(parametros));
        }

        /// <inheritdoc/>
        public void Ejecutar()
        {
            if (_parametros == null)
                throw new InvalidOperationException("Parámetros no establecidos");
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Doblar Apuesta' para {_jugador.ObtenerNombre()}");
            _apuestaOriginal = _parametros.ApuestaOriginal;
            var nuevaApuesta = _apuestaOriginal * 2;
            Console.WriteLine($"Apuesta doblada de ${_apuestaOriginal} a ${nuevaApuesta}");
        }

        /// <inheritdoc/>
        public void Deshacer()
        {
            if (!SePuedeDeshacer())
                throw new InvalidOperationException("No se puede deshacer este comando");
            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando 'Doblar Apuesta' para {_jugador.ObtenerNombre()}");
            Console.WriteLine($"↩️ Apuesta restaurada a ${_apuestaOriginal}");
        }

        /// <inheritdoc/>
        public bool SePuedeDeshacer()
        {
            return _parametros != null && _apuestaOriginal > 0;
        }

        /// <inheritdoc/>
        public string ObtenerDescripcion()
        {
            return $"Doblar apuesta para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// Comando para rendirse.
    /// </summary>
    public sealed class ComandoRendirse : IComando
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private bool _ejecutado = false;

        /// <summary>
        /// Inicializa el comando con el jugador y el dealer.
        /// </summary>
        public ComandoRendirse(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <inheritdoc/>
        public void Ejecutar()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Rendirse' para {_jugador.ObtenerNombre()}");
            _ejecutado = true;
            Console.WriteLine($"{_jugador.ObtenerNombre()} se rindió");
        }

        /// <inheritdoc/>
        public void Deshacer()
        {
            throw new InvalidOperationException("No se puede deshacer 'Rendirse'");
        }

        /// <inheritdoc/>
        public bool SePuedeDeshacer()
        {
            return false;
        }

        /// <inheritdoc/>
        public string ObtenerDescripcion()
        {
            return $"Rendirse para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// Comando compuesto (macro comando) que agrupa varios comandos.
    /// </summary>
    public sealed class ComandoCompuesto : IComando
    {
        private readonly List<IComando> _comandos = new List<IComando>();
        private readonly string _nombre;

        /// <summary>
        /// Inicializa el macro comando con un nombre descriptivo.
        /// </summary>
        public ComandoCompuesto(string nombre)
        {
            _nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        /// <summary>
        /// Agrega un comando al macro comando.
        /// </summary>
        public void AgregarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));
            _comandos.Add(comando);
        }

        /// <inheritdoc/>
        public void Ejecutar()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando compuesto '{_nombre}' con {_comandos.Count} comandos");
            foreach (var comando in _comandos)
            {
                try
                {
                    comando.Ejecutar();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ejecutando comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                    throw;
                }
            }
            Console.WriteLine($"Comando compuesto '{_nombre}' ejecutado exitosamente");
        }

        /// <inheritdoc/>
        public void Deshacer()
        {
            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando compuesto '{_nombre}'");
            for (int i = _comandos.Count - 1; i >= 0; i--)
            {
                var comando = _comandos[i];
                if (comando.SePuedeDeshacer())
                {
                    try
                    {
                        comando.Deshacer();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deshaciendo comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                    }
                }
            }
            Console.WriteLine($"Comando compuesto '{_nombre}' deshecho");
        }

        /// <inheritdoc/>
        public bool SePuedeDeshacer()
        {
            return _comandos.Any(c => c.SePuedeDeshacer());
        }

        /// <inheritdoc/>
        public string ObtenerDescripcion()
        {
            return $"Comando compuesto '{_nombre}' ({_comandos.Count} comandos)";
        }
    }

    /// <summary>
    /// Invocador de comandos: ejecuta, encola y deshace comandos.
    /// </summary>
    public sealed class InvocadorComandos
    {
        private readonly Stack<IComando> _historialComandos = new Stack<IComando>();
        private readonly Queue<IComando> _colaComandos = new Queue<IComando>();

        /// <summary>
        /// Ejecuta un comando inmediatamente y lo guarda en el historial.
        /// </summary>
        public void EjecutarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));
            Console.WriteLine($"COMMAND PATTERN: Invocador ejecutando comando '{comando.ObtenerDescripcion()}'");
            try
            {
                comando.Ejecutar();
                _historialComandos.Push(comando);
                Console.WriteLine($"Comando '{comando.ObtenerDescripcion()}' ejecutado y guardado en historial");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ejecutando comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Encola un comando para ejecución posterior.
        /// </summary>
        public void EncolarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));
            _colaComandos.Enqueue(comando);
            Console.WriteLine($"COMMAND PATTERN: Comando '{comando.ObtenerDescripcion()}' encolado. Cola: {_colaComandos.Count}");
        }

        /// <summary>
        /// Ejecuta todos los comandos encolados.
        /// </summary>
        public void EjecutarColaComandos()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando {_colaComandos.Count} comandos encolados");
            while (_colaComandos.Count > 0)
            {
                var comando = _colaComandos.Dequeue();
                EjecutarComando(comando);
            }
        }

        /// <summary>
        /// Deshace el último comando ejecutado si es posible.
        /// </summary>
        public void DeshacerUltimoComando()
        {
            if (_historialComandos.Count == 0)
            {
                Console.WriteLine("No hay comandos para deshacer");
                return;
            }
            var comando = _historialComandos.Pop();
            if (comando.SePuedeDeshacer())
            {
                Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando '{comando.ObtenerDescripcion()}'");
                comando.Deshacer();
            }
            else
            {
                Console.WriteLine($"Comando '{comando.ObtenerDescripcion()}' no se puede deshacer");
            }
        }

        /// <summary>
        /// Devuelve el historial de comandos ejecutados.
        /// </summary>
        public IEnumerable<IComando> ObtenerHistorial()
        {
            return _historialComandos.ToArray();
        }

        /// <summary>
        /// Limpia el historial de comandos.
        /// </summary>
        public void LimpiarHistorial()
        {
            _historialComandos.Clear();
            Console.WriteLine("COMMAND PATTERN: Historial de comandos limpiado");
        }

        /// <summary>
        /// Devuelve información del estado del invocador.
        /// </summary>
        public string ObtenerInformacion()
        {
            return $"Invocador: {_historialComandos.Count} comandos en historial, {_colaComandos.Count} comandos encolados";
        }
    }

    /// <summary>
    /// Parámetros para el comando de pedir carta.
    /// </summary>
    public sealed class ParametrosPedirCarta
    {
        public IJugador Jugador { get; }
        public DateTime Timestamp { get; }

        /// <summary>
        /// Inicializa los parámetros con el jugador.
        /// </summary>
        public ParametrosPedirCarta(IJugador jugador)
        {
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Timestamp = DateTime.UtcNow;
        }
    }

    /// Parámetros para el comando de doblar apuesta.
    public sealed class ParametrosDoblarApuesta
    {
        public double ApuestaOriginal { get; }
        public IJugador Jugador { get; }
        public DateTime Timestamp { get; }

        /// Inicializa los parámetros con la apuesta original y el jugador.
        public ParametrosDoblarApuesta(double apuestaOriginal, IJugador jugador)
        {
            ApuestaOriginal = apuestaOriginal;
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Timestamp = DateTime.UtcNow;
        }
    }

    /// Fábrica de comandos para crear instancias de comandos de Blackjack.
    public static class ComandoFactory
    {
        /// Crea un comando para pedir carta.
        public static IComandoConParametros<ParametrosPedirCarta> CrearComandoPedirCarta(IDealer dealer, IJugador jugador)
        {
            return new ComandoPedirCarta(dealer, jugador);
        }

        /// Crea un comando para plantarse.
        public static IComando CrearComandoPlantarse(IJugador jugador, IDealer dealer)
        {
            return new ComandoPlantarse(jugador, dealer);
        }

        /// Comando para doblar la apuesta
        public static IComandoConParametros<ParametrosDoblarApuesta> CrearComandoDoblarApuesta(IJugador jugador, IDealer dealer)
        {
            return new ComandoDoblarApuesta(jugador, dealer);
        }

        // Crea un comando para rendirse
        public static IComando CrearComandoRendirse(IJugador jugador, IDealer dealer)
        {
            return new ComandoRendirse(jugador, dealer);
        }

        // Nos ayuda a usar varios comandos a la vez, los agrupamos en un comando compuesto
        public static ComandoCompuesto CrearComandoCompuesto(string nombre)
        {
            return new ComandoCompuesto(nombre);
        }
    }
}
