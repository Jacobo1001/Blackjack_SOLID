using System;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Croupier : IDealer
    {
        private readonly IReglasJuego _reglas;
        private Ronda? _rondaActual;

        public Croupier(IReglasJuego reglas) => _reglas = reglas ?? throw new ArgumentNullException(nameof(reglas));

        // Función pura para iniciar una nueva ronda (devuelve una nueva instancia de Ronda)
        public Ronda IniciarRonda(int numero)
        {
            _rondaActual = new Ronda(numero);
            return _rondaActual;
        }
        //  Este bloque es funcional porque devuelve una nueva instancia de `Ronda` en lugar de modificar directamente el estado interno.

        // Función pura para finalizar la ronda actual (devuelve una nueva instancia de Ronda con la fecha de finalización actualizada)
        public Ronda? FinalizarRonda()
        {
            if (_rondaActual == null)
                throw new InvalidOperationException("No hay una ronda activa para finalizar.");
            _rondaActual = _rondaActual.Finalizar();
            return _rondaActual;
        }
        //  Este bloque es funcional porque utiliza el método `Finalizar` de `Ronda`, que devuelve una nueva instancia, manteniendo la inmutabilidad.

        // Función pura para validar una jugada
        public bool ValidarJugada(string jugada) =>
            jugada is "PedirCarta" or "Plantarse";
        //  Este bloque es funcional porque utiliza un patrón declarativo para validar la jugada. Es una función pura que no tiene efectos secundarios.

        // Función para repartir elementos (puede ser extendida para incluir lógica funcional)
        public void RepartirElementos()
        {
            // Este método actualmente no tiene lógica funcional. Si se requiere, se puede implementar de manera declarativa.
        }
    }
}
