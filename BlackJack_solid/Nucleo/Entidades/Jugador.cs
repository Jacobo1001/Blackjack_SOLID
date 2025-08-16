using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Jugador : IJugador
    {
        public int Id { get; }
        public string Nombre { get; }
        public double Saldo { get; private set; }

        public Jugador(int id, string nombre, double saldo)
        {
            Id = id; Nombre = nombre; Saldo = saldo;
        }

        public int ObtenerId() => Id;
        public string ObtenerNombre() => Nombre;
        public double ObtenerSaldo() => Saldo;
        public void ActualizarSaldo(double monto) => Saldo += monto;
    }
}
