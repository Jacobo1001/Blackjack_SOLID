using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Blackjack.Reglas
{
    public sealed class ReglasBlackjack : IReglasJuego
    {
        public string NombreReglas => "Reglas Blackjack estándar";
        public int MaximoPuntos => 21;
        public bool ValidarApuesta(double monto) => monto > 0;

        //CalcularPuntos: Calcula el total de puntos de una mano de cartas sin modificar el estado interno.
        //EsManoValida: Determina si una mano es válida basándose en los puntos calculados.
        public int ValorCarta(string cara) => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            "C2" => 2,
            "C3" => 3,
            "C4" => 4,
            "C5" => 5,
            "C6" => 6,
            "C7" => 7,
            "C8" => 8,
            "C9" => 9,
            "C10" => 10,
            _ => 0
        };

        //Se utiliza LINQ (Sum, Count) para calcular el total de puntos y contar ases en una mano de cartas.
        // Función pura para calcular el total de puntos de una mano
        public int CalcularPuntos(IEnumerable<string> cartas)
        {
            var total = cartas.Sum(carta => ValorCarta(carta));
            var ases = cartas.Count(carta => carta == "A");

            while (total > MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }

            return total;
        }
        //Se utiliza LINQ (Sum, Count) para calcular el total de puntos y contar ases en una mano de cartas.
        // Función pura para determinar si una mano es válida
        public bool EsManoValida(IEnumerable<string> cartas)
        {
            var puntos = CalcularPuntos(cartas);
            return puntos <= MaximoPuntos;
        }
    }
}
