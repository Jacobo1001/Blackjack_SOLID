using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Interfaces
{
    public interface IReglasJuego
    {
        string NombreReglas { get; }
        bool ValidarApuesta(double monto);
        int ValorCarta(string cara);
        int MaximoPuntos { get; }
        int CalcularPuntos(IEnumerable<string> cartas);
        bool EsManoValida(IEnumerable<string> cartas);
    }
}
