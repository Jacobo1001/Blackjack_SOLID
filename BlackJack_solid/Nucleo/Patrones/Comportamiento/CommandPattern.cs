using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Nucleo.Patrones.Comportamiento
{
    /// <summary>
    /// COMMAND PATTERN
    /// Encapsula una solicitud como un objeto, permitiendo parametrizar clientes con diferentes solicitudes,
    /// encolar o registrar solicitudes, y soportar operaciones de deshacer.
    /// En Blackjack, permite encapsular las acciones del jugador como comandos.
    /// </summary>

    /// <summary>
    /// COMMAND PATTERN - Interfaz base para comandos
    /// </summary>
    public interface IComando
    {
        /// <summary>
        /// COMMAND PATTERN: Ejecutar el comando
        /// </summary>
        void Ejecutar();

        /// <summary>
        /// COMMAND PATTERN: Deshacer el comando (si es posible)
        /// </summary>
        void Deshacer();

        /// <summary>
        /// COMMAND PATTERN: Verificar si el comando se puede deshacer
        /// </summary>
        bool SePuedeDeshacer();

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción del comando
        /// </summary>
        string ObtenerDescripcion();
    }

    /// <summary>
    /// COMMAND PATTERN - Interfaz para comandos con parámetros
    /// </summary>
    public interface IComandoConParametros<T> : IComando
    {
        /// <summary>
        /// COMMAND PATTERN: Establecer parámetros del comando
        /// </summary>
        void EstablecerParametros(T parametros);
    }

    /// <summary>
    /// COMMAND PATTERN - Comando para pedir carta
    /// </summary>
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

        /// <summary>
        /// COMMAND PATTERN: Establecer parámetros del comando
        /// </summary>
        public void EstablecerParametros(ParametrosPedirCarta parametros)
        {
            _parametros = parametros ?? throw new ArgumentNullException(nameof(parametros));
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar el comando de pedir carta
        /// </summary>
        public void Ejecutar()
        {
            if (_parametros == null)
                throw new InvalidOperationException("Parámetros no establecidos");

            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Pedir Carta' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Simular la acción (en una implementación real se llamaría al dealer)
            Console.WriteLine("🃏 Simulando entrega de carta al jugador...");
            
            // COMMAND PATTERN: Simular carta entregada para demostración
            _cartaEntregada = new Carta(Palo.Corazones, Cara.C10);

            Console.WriteLine($"✅ Carta entregada: {_cartaEntregada.Cara} de {_cartaEntregada.Palo}");
        }

        /// <summary>
        /// COMMAND PATTERN: Deshacer el comando (remover la última carta)
        /// </summary>
        public void Deshacer()
        {
            if (!SePuedeDeshacer())
                throw new InvalidOperationException("No se puede deshacer este comando");

            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando 'Pedir Carta' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Simular remoción de carta
            Console.WriteLine("🃏 Simulando remoción de carta del jugador...");
            
            Console.WriteLine($"↩️ Carta removida: {_cartaEntregada?.Cara} de {_cartaEntregada?.Palo}");
        }

        /// <summary>
        /// COMMAND PATTERN: Verificar si se puede deshacer
        /// </summary>
        public bool SePuedeDeshacer()
        {
            return _cartaEntregada != null;
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Pedir carta para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Comando para plantarse
    /// </summary>
    public sealed class ComandoPlantarse : IComando
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private bool _ejecutado = false;

        public ComandoPlantarse(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar el comando de plantarse
        /// </summary>
        public void Ejecutar()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Plantarse' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Marcar como ejecutado
            _ejecutado = true;
            
            Console.WriteLine($"✅ {_jugador.ObtenerNombre()} se plantó");
        }

        /// <summary>
        /// COMMAND PATTERN: Deshacer el comando (no es posible deshacer plantarse)
        /// </summary>
        public void Deshacer()
        {
            throw new InvalidOperationException("No se puede deshacer 'Plantarse'");
        }

        /// <summary>
        /// COMMAND PATTERN: Verificar si se puede deshacer
        /// </summary>
        public bool SePuedeDeshacer()
        {
            return false; // No se puede deshacer plantarse
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Plantarse para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Comando para doblar apuesta
    /// </summary>
    public sealed class ComandoDoblarApuesta : IComandoConParametros<ParametrosDoblarApuesta>
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private ParametrosDoblarApuesta? _parametros;
        private double _apuestaOriginal;

        public ComandoDoblarApuesta(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <summary>
        /// COMMAND PATTERN: Establecer parámetros del comando
        /// </summary>
        public void EstablecerParametros(ParametrosDoblarApuesta parametros)
        {
            _parametros = parametros ?? throw new ArgumentNullException(nameof(parametros));
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar el comando de doblar apuesta
        /// </summary>
        public void Ejecutar()
        {
            if (_parametros == null)
                throw new InvalidOperationException("Parámetros no establecidos");

            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Doblar Apuesta' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Guardar apuesta original para posible deshacer
            _apuestaOriginal = _parametros.ApuestaOriginal;
            
            // COMMAND PATTERN: Doblar la apuesta
            var nuevaApuesta = _apuestaOriginal * 2;
            
            Console.WriteLine($"✅ Apuesta doblada de ${_apuestaOriginal} a ${nuevaApuesta}");
        }

        /// <summary>
        /// COMMAND PATTERN: Deshacer el comando (restaurar apuesta original)
        /// </summary>
        public void Deshacer()
        {
            if (!SePuedeDeshacer())
                throw new InvalidOperationException("No se puede deshacer este comando");

            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando 'Doblar Apuesta' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Restaurar apuesta original
            Console.WriteLine($"↩️ Apuesta restaurada a ${_apuestaOriginal}");
        }

        /// <summary>
        /// COMMAND PATTERN: Verificar si se puede deshacer
        /// </summary>
        public bool SePuedeDeshacer()
        {
            return _parametros != null && _apuestaOriginal > 0;
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Doblar apuesta para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Comando para rendirse
    /// </summary>
    public sealed class ComandoRendirse : IComando
    {
        private readonly IJugador _jugador;
        private readonly IDealer _dealer;
        private bool _ejecutado = false;

        public ComandoRendirse(IJugador jugador, IDealer dealer)
        {
            _jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            _dealer = dealer ?? throw new ArgumentNullException(nameof(dealer));
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar el comando de rendirse
        /// </summary>
        public void Ejecutar()
        {
            Console.WriteLine($"COMMAND PATTERN: Ejecutando comando 'Rendirse' para {_jugador.ObtenerNombre()}");
            
            // COMMAND PATTERN: Marcar como ejecutado
            _ejecutado = true;
            
            Console.WriteLine($"✅ {_jugador.ObtenerNombre()} se rindió");
        }

        /// <summary>
        /// COMMAND PATTERN: Deshacer el comando (no es posible deshacer rendirse)
        /// </summary>
        public void Deshacer()
        {
            throw new InvalidOperationException("No se puede deshacer 'Rendirse'");
        }

        /// <summary>
        /// COMMAND PATTERN: Verificar si se puede deshacer
        /// </summary>
        public bool SePuedeDeshacer()
        {
            return false; // No se puede deshacer rendirse
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Rendirse para {_jugador.ObtenerNombre()}";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Comando compuesto (macro comando)
    /// </summary>
    public sealed class ComandoCompuesto : IComando
    {
        private readonly List<IComando> _comandos = new List<IComando>();
        private readonly string _nombre;

        public ComandoCompuesto(string nombre)
        {
            _nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        /// <summary>
        /// COMMAND PATTERN: Agregar comando al macro comando
        /// </summary>
        public void AgregarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));
            _comandos.Add(comando);
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar todos los comandos
        /// </summary>
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
                    Console.WriteLine($"❌ Error ejecutando comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                    throw; // Re-lanzar para que el macro comando falle
                }
            }
            
            Console.WriteLine($"✅ Comando compuesto '{_nombre}' ejecutado exitosamente");
        }

        /// <summary>
        /// COMMAND PATTERN: Deshacer todos los comandos en orden inverso
        /// </summary>
        public void Deshacer()
        {
            Console.WriteLine($"COMMAND PATTERN: Deshaciendo comando compuesto '{_nombre}'");
            
            // COMMAND PATTERN: Deshacer en orden inverso
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
                        Console.WriteLine($"❌ Error deshaciendo comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                    }
                }
            }
            
            Console.WriteLine($"↩️ Comando compuesto '{_nombre}' deshecho");
        }

        /// <summary>
        /// COMMAND PATTERN: Verificar si se puede deshacer
        /// </summary>
        public bool SePuedeDeshacer()
        {
            return _comandos.Any(c => c.SePuedeDeshacer());
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener descripción
        /// </summary>
        public string ObtenerDescripcion()
        {
            return $"Comando compuesto '{_nombre}' ({_comandos.Count} comandos)";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Invocador (quien ejecuta los comandos)
    /// </summary>
    public sealed class InvocadorComandos
    {
        private readonly Stack<IComando> _historialComandos = new Stack<IComando>();
        private readonly Queue<IComando> _colaComandos = new Queue<IComando>();

        /// <summary>
        /// COMMAND PATTERN: Ejecutar comando inmediatamente
        /// </summary>
        public void EjecutarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));

            Console.WriteLine($"COMMAND PATTERN: Invocador ejecutando comando '{comando.ObtenerDescripcion()}'");
            
            try
            {
                comando.Ejecutar();
                _historialComandos.Push(comando);
                Console.WriteLine($"✅ Comando '{comando.ObtenerDescripcion()}' ejecutado y guardado en historial");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error ejecutando comando '{comando.ObtenerDescripcion()}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// COMMAND PATTERN: Encolar comando para ejecución posterior
        /// </summary>
        public void EncolarComando(IComando comando)
        {
            if (comando == null) throw new ArgumentNullException(nameof(comando));

            _colaComandos.Enqueue(comando);
            Console.WriteLine($"COMMAND PATTERN: Comando '{comando.ObtenerDescripcion()}' encolado. Cola: {_colaComandos.Count}");
        }

        /// <summary>
        /// COMMAND PATTERN: Ejecutar todos los comandos encolados
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
        /// COMMAND PATTERN: Deshacer último comando
        /// </summary>
        public void DeshacerUltimoComando()
        {
            if (_historialComandos.Count == 0)
            {
                Console.WriteLine("❌ No hay comandos para deshacer");
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
                Console.WriteLine($"❌ Comando '{comando.ObtenerDescripcion()}' no se puede deshacer");
            }
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener historial de comandos
        /// </summary>
        public IEnumerable<IComando> ObtenerHistorial()
        {
            return _historialComandos.ToArray();
        }

        /// <summary>
        /// COMMAND PATTERN: Limpiar historial
        /// </summary>
        public void LimpiarHistorial()
        {
            _historialComandos.Clear();
            Console.WriteLine("COMMAND PATTERN: Historial de comandos limpiado");
        }

        /// <summary>
        /// COMMAND PATTERN: Obtener información del invocador
        /// </summary>
        public string ObtenerInformacion()
        {
            return $"Invocador: {_historialComandos.Count} comandos en historial, {_colaComandos.Count} comandos encolados";
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Parámetros para comando de pedir carta
    /// </summary>
    public sealed class ParametrosPedirCarta
    {
        public IJugador Jugador { get; }
        public DateTime Timestamp { get; }

        public ParametrosPedirCarta(IJugador jugador)
        {
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Parámetros para comando de doblar apuesta
    /// </summary>
    public sealed class ParametrosDoblarApuesta
    {
        public double ApuestaOriginal { get; }
        public IJugador Jugador { get; }
        public DateTime Timestamp { get; }

        public ParametrosDoblarApuesta(double apuestaOriginal, IJugador jugador)
        {
            ApuestaOriginal = apuestaOriginal;
            Jugador = jugador ?? throw new ArgumentNullException(nameof(jugador));
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// COMMAND PATTERN - Factory para crear comandos
    /// </summary>
    public static class ComandoFactory
    {
        /// <summary>
        /// COMMAND PATTERN: Crear comando de pedir carta
        /// </summary>
        public static IComandoConParametros<ParametrosPedirCarta> CrearComandoPedirCarta(IDealer dealer, IJugador jugador)
        {
            return new ComandoPedirCarta(dealer, jugador);
        }

        /// <summary>
        /// COMMAND PATTERN: Crear comando de plantarse
        /// </summary>
        public static IComando CrearComandoPlantarse(IJugador jugador, IDealer dealer)
        {
            return new ComandoPlantarse(jugador, dealer);
        }

        /// <summary>
        /// COMMAND PATTERN: Crear comando de doblar apuesta
        /// </summary>
        public static IComandoConParametros<ParametrosDoblarApuesta> CrearComandoDoblarApuesta(IJugador jugador, IDealer dealer)
        {
            return new ComandoDoblarApuesta(jugador, dealer);
        }

        /// <summary>
        /// COMMAND PATTERN: Crear comando de rendirse
        /// </summary>
        public static IComando CrearComandoRendirse(IJugador jugador, IDealer dealer)
        {
            return new ComandoRendirse(jugador, dealer);
        }

        /// <summary>
        /// COMMAND PATTERN: Crear comando compuesto
        /// </summary>
        public static ComandoCompuesto CrearComandoCompuesto(string nombre)
        {
            return new ComandoCompuesto(nombre);
        }
    }
}
