using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Ronda
    {
        public int Numero { get; }
        public DateTime Inicio { get; } = DateTime.UtcNow;
        public DateTime? Fin { get; private set; }
        public Ronda(int numero) => Numero = numero;
        public void Finalizar() => Fin = DateTime.UtcNow;
    }
}
