using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IServicioMesa
    {
        IMesa AbrirMesa(IReglasJuego reglas);
        IMesa MesaCerrada(IMesa mesa);
        IMesa RondaFinalizada(IMesa mesa);
    }
}
