using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Blackjack.Servicios
{
    public sealed class ServicioMesa : IServicioMesa
    {
        private sealed class MesaSimple : IMesa
        {
            private readonly int _id;
            private string _estado = "CERRADA";
            private IDealer? _Dealer;
            private readonly IReglasJuego _reglas;

            public MesaSimple(int id, IReglasJuego reglas)
            { _id = id; _reglas = reglas; }

            public int ObtenerId() => _id;
            public string ObtenerEstado() => _estado;
            public void AsignarDealer(IDealer c) => _Dealer = c;
            public IDealer ObtenerDealer() => _Dealer!;
            public IReglasJuego ObtenerReglas() => _reglas;

            public void Abrir() => _estado = "ABIERTA";
            public void Cerrar() => _estado = "CERRADA";
        }

        private int _contador = 1;

        public IMesa AbrirMesa(IReglasJuego reglas)
        {
            var mesa = new MesaSimple(_contador++, reglas);
            (mesa as MesaSimple)!.Abrir();
            return mesa;
        }

        public void CerrarMesa(IMesa mesa) =>
            (mesa as MesaSimple)!.Cerrar();
    }
}
