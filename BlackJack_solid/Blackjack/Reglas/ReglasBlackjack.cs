using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Blackjack.Reglas
{
    public sealed class ReglasBlackjack : IReglasJuego
    {
        public string NombreReglas => "Reglas Blackjack estándar";
        public int MaximoPuntos => 21;

        public bool ValidarApuesta(double monto) => monto > 0;

        public int ValorCarta(string cara) => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.TryParse(cara, out var v) ? v : 0
        };
    }
}
