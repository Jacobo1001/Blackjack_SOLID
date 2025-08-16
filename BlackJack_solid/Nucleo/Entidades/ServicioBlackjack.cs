using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Croupier : IDealer
    {
        private readonly IReglasJuego _reglas;
        private Ronda? _rondaActual;

        public Croupier(IReglasJuego reglas) => _reglas = reglas;

        public void IniciarRonda() => _rondaActual = new Ronda(numero: 1);
        public void FinalizarRonda() => _rondaActual?.Finalizar();
        public void RepartirElementos() {}

        public bool ValidarJugada(string jugada) =>
            jugada is "PedirCarta" or "Plantarse"; 
    }
}
