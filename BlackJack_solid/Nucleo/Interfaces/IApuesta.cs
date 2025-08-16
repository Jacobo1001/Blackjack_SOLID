using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IApuesta
    {
        IJugador ObtenerJugador();
        double ObtenerMonto();
        DateTime ObtenerFecha();
        bool EstaActiva();
    }
}
