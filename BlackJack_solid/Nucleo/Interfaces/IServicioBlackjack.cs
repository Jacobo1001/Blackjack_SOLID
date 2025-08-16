using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJack_solid.Nucleo.Interfaces;


namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IServicioBlackjack
    {
        IMesa AbrirMesa(IReglasJuego reglas);
        void IniciarRonda(IMesa mesa);
        void FinalizarRonda(IMesa mesa);
    }
}
