using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IMesa
    {
        int ObtenerId();
        string ObtenerEstado();
        void AsignarDealer(IDealer Dealer);
        IDealer ObtenerDealer();
        IReglasJuego ObtenerReglas();
    }
}
