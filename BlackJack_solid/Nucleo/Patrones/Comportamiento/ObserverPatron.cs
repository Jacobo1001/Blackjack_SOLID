using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Nucleo.Patrones.Comportamiento
{
    // Define una dependencia uno-a-muchos entre objetos, de manera que cuando un objeto cambia de estado,
    // todos sus dependientes son notificados y actualizados automáticamente.
    // En Blackjack, permite notificar eventos del juego a diferentes observadores.

    public enum TipoEvento
    {
        RondaIniciada,
        ApuestaRealizada,
        CartasRepartidas,
        JugadorPidioCarta,
        JugadorSePlanto,
        JugadorDobloApuesta,
        JugadorSeRindio,
        JugadorSePaso,
        JugadorBlackjack,
        DealerPidioCarta,
        DealerSePlanto,
        DealerSePaso,
        DealerBlackjack,
        ResultadoCalculado,
        RondaFinalizada,
        MesaAbierta,
        MesaCerrada
    }


    public sealed class EventoJuego
    {
        public TipoEvento Tipo { get; }
        public DateTime Timestamp { get; }
        public string Descripcion { get; }
        public object? Datos { get; }

        public EventoJuego(TipoEvento tipo, string descripcion, object? datos = null)
        {
            Tipo = tipo;
            Timestamp = DateTime.UtcNow;
            Descripcion = descripcion;
            Datos = datos;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Tipo}: {Descripcion}";
        }
    }


    public interface IObservadorJuego
    {
        void NotificarEvento(EventoJuego evento);
    }


    public interface ISujetoObservable
    {

        void RegistrarObservador(IObservadorJuego observador);


        void DesregistrarObservador(IObservadorJuego observador);


        void NotificarObservadores(EventoJuego evento);
    }

    public sealed class SujetoObservable : ISujetoObservable
    {
        private readonly List<IObservadorJuego> _observadores = new List<IObservadorJuego>();

        public void RegistrarObservador(IObservadorJuego observador)
        {
            if (observador == null) throw new ArgumentNullException(nameof(observador));
            
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
                Console.WriteLine($"OBSERVER PATTERN: Observador '{observador.GetType().Name}' registrado. Total: {_observadores.Count}");
            }
        }

        //Desregistrar un observador
        public void DesregistrarObservador(IObservadorJuego observador)
        {
            if (observador == null) throw new ArgumentNullException(nameof(observador));
            
            if (_observadores.Remove(observador))
            {
                Console.WriteLine($"OBSERVER PATTERN: Observador '{observador.GetType().Name}' desregistrado. Total: {_observadores.Count}");
            }
        }

        //Notificar a todos los observadores
        public void NotificarObservadores(EventoJuego evento)
        {
            Console.WriteLine($"OBSERVER PATTERN: Notificando evento '{evento.Tipo}' a {_observadores.Count} observadores");
            
            foreach (var observador in _observadores.ToList())
            {
                try
                {
                    observador.NotificarEvento(evento);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error notificando a observador '{observador.GetType().Name}': {ex.Message}");
                }
            }
        }

        //Obtener número de observadores registrados
        public int ObtenerNumeroObservadores() => _observadores.Count;
    }

    //Observador para logging de eventos
    public sealed class ObservadorLogger : IObservadorJuego
    {
        private readonly string _nombre;

        public ObservadorLogger(string nombre = "Logger")
        {
            _nombre = nombre;
        }


        public void NotificarEvento(EventoJuego evento)
        {
            Console.WriteLine($" [{_nombre}] {evento}");
        }
    }

    //Observador para estadísticas del juego
    public sealed class ObservadorEstadisticas : IObservadorJuego
    {
        private readonly Dictionary<TipoEvento, int> _contadores = new Dictionary<TipoEvento, int>();
        private readonly List<EventoJuego> _eventos = new List<EventoJuego>();

        public void NotificarEvento(EventoJuego evento)
        {
            // Usa operadores ternarios para contar eventos por tipo y guardar el evento
            _contadores[evento.Tipo] = _contadores.ContainsKey(evento.Tipo) ? _contadores[evento.Tipo] + 1 : 1;
            _eventos.Add(evento);
            Console.WriteLine($"[Estadísticas] Evento registrado: {evento.Tipo} (Total: {_contadores[evento.Tipo]})");
        }

        //Obtener estadísticas
        public Dictionary<TipoEvento, int> ObtenerEstadisticas()
        {
            return new Dictionary<TipoEvento, int>(_contadores);
        }

        //Obtener eventos por tipo
        public IEnumerable<EventoJuego> ObtenerEventosPorTipo(TipoEvento tipo)
        {
            return _eventos.Where(e => e.Tipo == tipo);
        }

        public string ObtenerResumenEstadisticas()
        {
            var resumen = "📊 === RESUMEN DE ESTADÍSTICAS ===\n";
            
            foreach (var kvp in _contadores.OrderByDescending(x => x.Value))
            {
                resumen += $"• {kvp.Key}: {kvp.Value} veces\n";
            }

            resumen += $"• Total de eventos: {_eventos.Count}\n";
            resumen += $"• Tipos de eventos únicos: {_contadores.Count}";

            return resumen;
        }
    }

    //Observador para la interfaz de usuario
    public sealed class ObservadorUI : IObservadorJuego
    {
        private readonly string _nombre;

        public ObservadorUI(string nombre = "UI")
        {
            _nombre = nombre;
        }

        public void NotificarEvento(EventoJuego evento)
        {
            // OBSERVER PATTERN: Actualizar UI según el tipo de evento
            switch (evento.Tipo)
            {
                case TipoEvento.RondaIniciada:
                    Console.WriteLine($"🖥️ [{_nombre}] Actualizando pantalla de juego...");
                    break;

                case TipoEvento.ApuestaRealizada:
                    Console.WriteLine($"🖥️ [{_nombre}] Actualizando saldo del jugador...");
                    break;

                case TipoEvento.CartasRepartidas:
                    Console.WriteLine($"🖥️ [{_nombre}] Mostrando cartas en pantalla...");
                    break;

                case TipoEvento.JugadorPidioCarta:
                    Console.WriteLine($"🖥️ [{_nombre}] Animando nueva carta del jugador...");
                    break;

                case TipoEvento.JugadorSePlanto:
                    Console.WriteLine($"🖥️ [{_nombre}] Mostrando mensaje 'Jugador se plantó'...");
                    break;

                case TipoEvento.ResultadoCalculado:
                    Console.WriteLine($"🖥️ [{_nombre}] Mostrando resultado final...");
                    break;

                case TipoEvento.RondaFinalizada:
                    Console.WriteLine($"🖥️ [{_nombre}] Mostrando pantalla de resumen...");
                    break;

                default:
                    Console.WriteLine($"🖥️ [{_nombre}] Actualizando UI para evento: {evento.Tipo}");
                    break;
            }
        }
    }

    //Observador para notificaciones de seguridad
    public sealed class ObservadorSeguridad : IObservadorJuego
    {
        private readonly string _nombre;

        public ObservadorSeguridad(string nombre = "Seguridad")
        {
            _nombre = nombre;
        }

        //Monitorear eventos de seguridad
        public void NotificarEvento(EventoJuego evento)
        {
            switch (evento.Tipo)
            {
                case TipoEvento.ApuestaRealizada:
                    if (evento.Datos is double monto && monto > 1000)
                    {
                        Console.WriteLine($"🚨 [{_nombre}] ALERTA: Apuesta alta detectada: ${monto}");
                    }
                    break;

                case TipoEvento.JugadorBlackjack:
                    Console.WriteLine($"🚨 [{_nombre}] Monitoreando: Jugador obtuvo Blackjack");
                    break;

                case TipoEvento.DealerBlackjack:
                    Console.WriteLine($"🚨 [{_nombre}] Monitoreando: Dealer obtuvo Blackjack");
                    break;

                case TipoEvento.MesaAbierta:
                    Console.WriteLine($"🚨 [{_nombre}] Mesa abierta - iniciando monitoreo");
                    break;

                case TipoEvento.MesaCerrada:
                    Console.WriteLine($"🚨 [{_nombre}] Mesa cerrada - finalizando monitoreo");
                    break;

                default:
                    break;
            }
        }
    }

    //Observador para análisis de patrones de juego
    public sealed class ObservadorAnalisis : IObservadorJuego
    {
        private readonly List<EventoJuego> _eventosJugador = new List<EventoJuego>();
        private readonly List<EventoJuego> _eventosDealer = new List<EventoJuego>();


        public void NotificarEvento(EventoJuego evento)
        {
            switch (evento.Tipo)
            {
                case TipoEvento.JugadorPidioCarta:
                case TipoEvento.JugadorSePlanto:
                case TipoEvento.JugadorDobloApuesta:
                case TipoEvento.JugadorSeRindio:
                case TipoEvento.JugadorSePaso:
                case TipoEvento.JugadorBlackjack:
                    _eventosJugador.Add(evento);
                    AnalizarPatronJugador();
                    break;

                case TipoEvento.DealerPidioCarta:
                case TipoEvento.DealerSePlanto:
                case TipoEvento.DealerSePaso:
                case TipoEvento.DealerBlackjack:
                    _eventosDealer.Add(evento);
                    AnalizarPatronDealer();
                    break;

                default:
                    break;
            }
        }

        //Analizar patrones del jugador

        private void AnalizarPatronJugador()
        {
            var cartasPedidas = _eventosJugador.Count(e => e.Tipo == TipoEvento.JugadorPidioCarta);
            var vecesSePlanto = _eventosJugador.Count(e => e.Tipo == TipoEvento.JugadorSePlanto);

            if (cartasPedidas > 3)
            {
                Console.WriteLine($"🔍 [Análisis] Jugador agresivo detectado: {cartasPedidas} cartas pedidas");
            }

            if (vecesSePlanto > 5)
            {
                Console.WriteLine($"🔍 [Análisis] Jugador conservador detectado: {vecesSePlanto} veces se plantó");
            }
        }

        // Analiza los patrones del dealer usando los eventos registrados
        private void AnalizarPatronDealer()
        {
            // Cuenta cuántas veces el dealer pidió carta
            var cartasPedidas = _eventosDealer.Count(e => e.Tipo == TipoEvento.DealerPidioCarta);
            // Cuenta cuántas veces el dealer se plantó
            var vecesSePlanto = _eventosDealer.Count(e => e.Tipo == TipoEvento.DealerSePlanto);

            // Muestra un resumen simple en consola
            Console.WriteLine($"[Análisis] Dealer: {cartasPedidas} cartas pedidas, {vecesSePlanto} veces se plantó");
        }

        //Obtener resumen de análisis
        public string ObtenerResumenAnalisis()
        {
            return $"🔍 === ANÁLISIS DE PATRONES ===\n" +
                   $"• Eventos del jugador: {_eventosJugador.Count}\n" +
                   $"• Eventos del dealer: {_eventosDealer.Count}\n" +
                   $"• Cartas pedidas por jugador: {_eventosJugador.Count(e => e.Tipo == TipoEvento.JugadorPidioCarta)}\n" +
                   $"• Cartas pedidas por dealer: {_eventosDealer.Count(e => e.Tipo == TipoEvento.DealerPidioCarta)}";
        }
    }

    //Factory para crear observadores
    public static class ObservadorFactory
    {

        public static IObservadorJuego CrearLogger(string nombre = "Logger")
        {
            return new ObservadorLogger(nombre);
        }


        public static IObservadorJuego CrearEstadisticas()
        {
            return new ObservadorEstadisticas();
        }

        public static IObservadorJuego CrearUI(string nombre = "UI")
        {
            return new ObservadorUI(nombre);
        }


        public static IObservadorJuego CrearSeguridad(string nombre = "Seguridad")
        {
            return new ObservadorSeguridad(nombre);
        }


        public static IObservadorJuego CrearAnalisis()
        {
            return new ObservadorAnalisis();
        }


        public static List<IObservadorJuego> CrearConjuntoCompleto()
        {
            return new List<IObservadorJuego>
            {
                CrearLogger("Sistema"),
                CrearEstadisticas(),
                CrearUI("Pantalla Principal"),
                CrearSeguridad("Monitoreo"),
                CrearAnalisis()
            };
        }
    }
}
