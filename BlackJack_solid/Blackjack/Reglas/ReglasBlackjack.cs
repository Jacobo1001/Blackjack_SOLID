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
        //ValidarApuesta y ValorCarta ahora son funciones puras representadas como expresiones lambda (Func<T, TResult>). Esto hace que sean más declarativas y reutilizables.
        // Función pura para validar una apuesta
        public Func<double, bool> ValidarApuesta => monto => monto > 0;

        //CalcularPuntos: Calcula el total de puntos de una mano de cartas sin modificar el estado interno.
        //EsManoValida: Determina si una mano es válida basándose en los puntos calculados.
        // Función pura para calcular el valor de una carta
        public Func<string, int> ValorCarta => cara => cara switch
        {
            "A" => 11,
            "K" or "Q" or "J" => 10,
            _ => int.TryParse(cara, out var v) ? v : 0
        };

        //Se utiliza LINQ (Sum, Count) para calcular el total de puntos y contar ases en una mano de cartas.
        // Función pura para calcular el total de puntos de una mano
        public Func<IEnumerable<string>, int> CalcularPuntos => cartas =>
        {
            var total = cartas.Sum(carta => ValorCarta(carta));
            var ases = cartas.Count(carta => carta == "A");

            // Ajustar el valor de los ases si el total excede el máximo permitido
            while (total > MaximoPuntos && ases > 0)
            {
                total -= 10;
                ases--;
            }

            return total;
        };
        //Se utiliza LINQ (Sum, Count) para calcular el total de puntos y contar ases en una mano de cartas.
        // Función pura para determinar si una mano es válida
        public Func<IEnumerable<string>, bool> EsManoValida => cartas =>
        {
            var puntos = CalcularPuntos(cartas);
            return puntos <= MaximoPuntos;
        };
    }
}
