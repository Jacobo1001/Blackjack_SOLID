using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IServicioApuestas
    {
        IApuesta CrearApuesta(IJugador jugador, double monto);
        IApuesta ApuestaFinalizada(IApuesta apuesta);

        /*bool RegistrarApuesta(IApuesta apuesta);
        bool ValidarApuesta(IApuesta apuesta);
        IReadOnlyList<IApuesta> ObtenerApuestasActivas();
        IDictionary<IJugador, double> CalcularGanancias();
        */
    }
}
