using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IJugador
    {
        int ObtenerId();
        string ObtenerNombre();
        double ObtenerSaldo();
        void ActualizarSaldo(double monto);
    }
}
