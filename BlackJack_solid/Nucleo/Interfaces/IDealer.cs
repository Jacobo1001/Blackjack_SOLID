using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IDealer
    {
        void IniciarRonda();
        void FinalizarRonda();
        void RepartirElementos();
        bool ValidarJugada(string jugada);
    }
}
