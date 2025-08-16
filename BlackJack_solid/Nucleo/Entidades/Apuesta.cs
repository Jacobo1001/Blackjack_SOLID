using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Apuesta : IApuesta
    {
        private bool _activa = true;
        public IJugador Jugador { get; }
        public double Monto { get; }
        public DateTime Fecha { get; }

        public Apuesta(IJugador jugador, double monto, DateTime? fecha = null)
        {
            Jugador = jugador; Monto = monto; Fecha = fecha ?? DateTime.UtcNow;
        }

        public IJugador ObtenerJugador() => Jugador;
        public double ObtenerMonto() => Monto;
        public DateTime ObtenerFecha() => Fecha;
        public bool EstaActiva() => _activa;
        public void Cerrar() => _activa = false;
    }
}
