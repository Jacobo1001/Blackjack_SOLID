using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;

namespace BlackJack_solid.Nucleo.Entidades
{
    public class Apuesta : IApuesta
    {
        private bool _activa = true;
        public IJugador Jugador { get; }
        public double Monto { get; }
        public DateTime Fecha { get; }

        private readonly List<Apuesta> _apuestas = new();

        public Apuesta(IJugador jugador, double monto, DateTime? fecha = null)
        {
            Jugador = jugador;
            Monto = monto;
            Fecha = fecha ?? DateTime.UtcNow;
        }

        //Métodos como ObtenerJugador, ObtenerMonto, ObtenerFecha, y EstaActiva son funciones puras que no tienen efectos secundarios ni dependen de estados externos.
        // Función pura para obtener el jugador
        public IJugador ObtenerJugador() => Jugador;

        // Función pura para obtener el monto
        public double ObtenerMonto() => Monto;

        // Función pura para obtener la fecha
        public DateTime ObtenerFecha() => Fecha;

        // Función pura para verificar si la apuesta está activa
        public bool EstaActiva() => _activa;

        // Método para cerrar la apuesta (devuelve una nueva instancia con el estado actualizado)
        public Apuesta Cerrar()
        {
            _activa = false;
            return this;
        }

        // Función pura para registrar una apuesta (devuelve una nueva lista con la apuesta agregada)
        public IReadOnlyList<Apuesta> RegistrarApuesta(Apuesta apuesta, EstadoMesa estado)
        {
            if (estado != EstadoMesa.EnRonda)
                throw new InvalidOperationException("Solo se pueden registrar apuestas durante una ronda.");

            var nuevasApuestas = new List<Apuesta>(_apuestas) { apuesta };
            return nuevasApuestas.AsReadOnly();
        }

        // Función pura para obtener todas las apuestas
        public IReadOnlyList<Apuesta> ObtenerApuestas() => _apuestas.AsReadOnly();
    }
}
