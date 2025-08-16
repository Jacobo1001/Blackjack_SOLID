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
        void AsignarCroupier(ICroupier croupier);
        ICroupier ObtenerCroupier();
        IReglasJuego ObtenerReglas();
    }
}
