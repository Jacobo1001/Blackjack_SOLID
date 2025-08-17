using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Blackjack.Servicios
{
    public sealed class ServicioMesa : IServicioMesa
    {
        private sealed class MesaSimple : IMesa
        {
            private readonly int _id;
            private readonly IReglasJuego _reglas;
            private string _estado = "CERRADA";
            private IDealer? _dealer;

            public MesaSimple(int id, IReglasJuego reglas)
            {
                _id = id;
                _reglas = reglas;
            }

            // Función pura para obtener el ID de la mesa
            public int ObtenerId() => _id;

            // Función pura para obtener el estado de la mesa
            public string ObtenerEstado() => _estado;

            // Función pura para asignar un dealer
            public void AsignarDealer(IDealer dealer) => _dealer = dealer;

            // Función pura para obtener el dealer
            public IDealer ObtenerDealer() => _dealer ?? throw new InvalidOperationException("No se ha asignado un dealer.");

            // Función pura para obtener las reglas
            public IReglasJuego ObtenerReglas() => _reglas;

            // Función para abrir la mesa (transición de estado)
            public void Abrir() => _estado = "ABIERTA";

            // Función para cerrar la mesa (transición de estado)
            public void Cerrar() => _estado = "CERRADA";
        }

        private int _contador = 1;
        
        //Las operaciones como abrir y cerrar mesas son más claras y fáciles de entender.
        // Función pura para abrir una nueva mesa
        public IMesa AbrirMesa(IReglasJuego reglas)
        {
            var mesa = new MesaSimple(_contador++, reglas);
            mesa.Abrir();
            return mesa;
        }

        // Función pura para cerrar una mesa
        public void CerrarMesa(IMesa mesa)
        {
            if (mesa is MesaSimple mesaSimple)
            {
                mesaSimple.Cerrar();
            }
            else
            {
                throw new InvalidOperationException("La mesa no es válida.");
            }
        }
        //Se agregó el método ObtenerMesasAbiertas, que utiliza LINQ para filtrar las mesas abiertas.
        // Función pura para obtener mesas abiertas (usando LINQ)
        public IEnumerable<IMesa> ObtenerMesasAbiertas(IEnumerable<IMesa> mesas) =>
            mesas.Where(m => m.ObtenerEstado() == "ABIERTA");
    }
}
