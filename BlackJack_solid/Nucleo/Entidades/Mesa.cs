using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJack_solid.Compartido;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Mesa : IMesa
    {
        private Dealer? _dealer;
        public int Id { get; }
        public string Estado { get; private set; } = "Cerrada";
        public IReglasJuego Reglas { get; }

        public Mesa(int id, IReglasJuego reglas)
        {
            Id = id;
            Reglas = reglas;
        }

        public int ObtenerId() => Id;
        public string ObtenerEstado() => Estado;

        public void AsignarDealer(Dealer dealer) => _dealer = dealer;
        public Dealer ObtenerDealer() => _dealer!;

        public IReglasJuego ObtenerReglas() => Reglas;
    }
}
