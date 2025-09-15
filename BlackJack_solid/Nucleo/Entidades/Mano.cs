using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Entidades
{
    public class Mano
    {
        private readonly List<Carta> _cartas = new();
        public IReadOnlyList<Carta> Cartas => _cartas;
        public void Agregar(Carta c) => _cartas.Add(c);
        public void Limpiar() => _cartas.Clear();
    }
}
