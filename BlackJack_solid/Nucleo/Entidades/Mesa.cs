using System;
using System.Collections.Generic;
using System.Linq;
using BlackJack_solid.Compartido;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Entidades
{
    public enum EstadoMesa
    {
        Cerrada,
        Abierta,
        EnRonda,
        Finalizada
    }

    public sealed class Mesa : IMesa
    {
        private Dealer? _dealer;
        private readonly List<Jugador> _jugadores = new List<Jugador>();
        private readonly List<Apuesta> _apuestas = new List<Apuesta>();
        private Ronda? _rondaActual;

        public int Id { get; }
        public EstadoMesa Estado { get; private set; } = EstadoMesa.Cerrada;
        public IReglasJuego Reglas { get; }

        public Mesa(int id, IReglasJuego reglas)
        {
            Id = id;
            Reglas = reglas;
        }

        // Función pura para obtener el ID de la mesa
        public int ObtenerId() => Id;

        // Función pura para obtener el estado de la mesa
        public EstadoMesa ObtenerEstado() => Estado;

        // Función pura para mostrar el estado de la mesa
        public string ObtenerEstadoMesa()
        {
            return $"Estado de la mesa: {Estado}\n" +
                   $"Jugadores en la mesa: {string.Join(", ", _jugadores.Select(j => j.Nombre))}\n" +
                   $"Apuestas registradas: {_apuestas.Count}";
        }
        //  Este bloque es funcional porque no modifica el estado interno de la clase y utiliza `LINQ` para transformar datos en una cadena legible.

        // Función pura para calcular el total de apuestas
        public double CalcularTotalApuestas() => _apuestas.Sum(a => a.ObtenerMonto());
        //  Este bloque es funcional porque utiliza `LINQ` para calcular el total de las apuestas sin modificar el estado interno.

        // Función pura para obtener jugadores con saldo positivo
        public IReadOnlyList<Jugador> ObtenerJugadoresConSaldoPositivo() =>
            _jugadores.Where(j => j.ObtenerSaldo() > 0).ToList();
        //  Este bloque es funcional porque utiliza `LINQ` para filtrar jugadores y devuelve una nueva lista sin modificar el estado interno.

        // Función pura para validar un movimiento
        public bool ValidarMovimiento(Jugador jugador, Movimiento movimiento) =>
            Reglas.ValidarMovimiento(jugador, movimiento);
        // Este bloque es funcional porque delega la validación a las reglas del juego y no modifica el estado interno.

        // Función pura para calcular resultados de la ronda
        public ResultadoRonda CalcularResultados()
        {
            if (_rondaActual == null)
                throw new InvalidOperationException("No hay una ronda activa para calcular resultados.");
            return Reglas.CalcularResultados(_rondaActual);
        }
        // Este bloque es funcional porque utiliza las reglas del juego para calcular resultados sin modificar el estado interno.

        public void AsignarDealer(Dealer dealer)
        {
            if (Estado != EstadoMesa.Abierta)
                throw new InvalidOperationException("La mesa debe estar abierta para asignar un dealer.");
            _dealer = dealer;
        }

        public Dealer ObtenerDealer()
        {
            if (_dealer == null)
                throw new InvalidOperationException("No se ha asignado un dealer a la mesa.");
            return _dealer;
        }

        public void AbrirMesa()
        {
            if (Estado != EstadoMesa.Cerrada)
                throw new InvalidOperationException("La mesa debe estar cerrada para abrirse.");
            Estado = EstadoMesa.Abierta;
        }

        public void IniciarNuevaRonda()
        {
            if (Estado != EstadoMesa.Abierta)
                throw new InvalidOperationException("La mesa debe estar abierta para iniciar una nueva ronda.");
            _rondaActual = new Ronda(_jugadores.Count + 1); // Número de ronda basado en jugadores.
            Estado = EstadoMesa.EnRonda;
        }

        public Ronda ObtenerRondaActual()
        {
            if (_rondaActual == null)
                throw new InvalidOperationException("No hay una ronda activa en la mesa.");
            return _rondaActual;
        }

        public void FinalizarRondaActual()
        {
            if (_rondaActual == null)
                throw new InvalidOperationException("No hay una ronda activa para finalizar.");
            _rondaActual.Finalizar();
            _rondaActual = null;
            Estado = EstadoMesa.Finalizada;
        }

        public void CerrarMesa()
        {
            if (Estado != EstadoMesa.Finalizada)
                throw new InvalidOperationException("La mesa debe estar finalizada para cerrarse.");
            Estado = EstadoMesa.Cerrada;
        }

        // Función pura para mostrar el estado de la mesa (usando LINQ y declaratividad)
        public void MostrarEstadoMesa()
        {
            Console.WriteLine(ObtenerEstadoMesa());
        }
        //  Este bloque utiliza la función pura `ObtenerEstadoMesa` para mostrar el estado de la mesa, manteniendo la lógica declarativa y funcional.
    }
}
