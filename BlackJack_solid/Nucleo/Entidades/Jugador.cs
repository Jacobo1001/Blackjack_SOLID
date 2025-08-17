using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Jugador : IJugador
    {
        public int Id { get; }
        public string Nombre { get; }
        public double Saldo { get; private set; }

        private readonly List<Jugador> _jugadores = new List<Jugador>();

        public Jugador(int id, string nombre, double saldo)
        {
            Id = id;
            Nombre = nombre;
            Saldo = saldo;
        }

        // Función pura para obtener el ID del jugador
        public int ObtenerId() => Id;

        // Función pura para obtener el nombre del jugador
        public string ObtenerNombre() => Nombre;

        // Función pura para obtener el saldo del jugador
        public double ObtenerSaldo() => Saldo;

        // Método para actualizar el saldo (devuelve una nueva instancia con el saldo actualizado)
        public Jugador ActualizarSaldo(double monto)
        {
            return new Jugador(Id, Nombre, Saldo + monto);
        }
        //  Este bloque es funcional porque devuelve una nueva instancia del jugador con el saldo actualizado, en lugar de modificar el estado interno.

        // Función pura para agregar un jugador (devuelve una nueva lista con el jugador agregado)
        public IReadOnlyList<Jugador> AgregarJugador(Jugador jugador, EstadoMesa estado)
        {
            if (estado != EstadoMesa.Abierta)
                throw new InvalidOperationException("La mesa debe estar abierta para agregar jugadores.");

            var nuevosJugadores = new List<Jugador>(_jugadores) { jugador };
            return nuevosJugadores.AsReadOnly();
        }
        // Este bloque es funcional porque devuelve una nueva lista de jugadores en lugar de modificar directamente la lista interna `_jugadores`.

        // Función pura para eliminar un jugador (devuelve una nueva lista con el jugador eliminado)
        public IReadOnlyList<Jugador> EliminarJugador(Jugador jugador)
        {
            if (!_jugadores.Contains(jugador))
                throw new InvalidOperationException("El jugador no está en la mesa.");

            var nuevosJugadores = _jugadores.Where(j => j != jugador).ToList();
            return nuevosJugadores.AsReadOnly();
        }
        // Este bloque es funcional porque utiliza LINQ para filtrar la lista de jugadores y devuelve una nueva lista sin modificar el estado interno.

        // Función pura para obtener todos los jugadores
        public IReadOnlyList<Jugador> ObtenerJugadores() => _jugadores.AsReadOnly();
        // Comentario: Este bloque es funcional porque devuelve una lista de solo lectura, asegurando que no se modifique el estado interno.
    }
}
