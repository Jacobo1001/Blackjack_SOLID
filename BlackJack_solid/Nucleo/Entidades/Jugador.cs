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
        public IJugador ActualizarSaldo(double monto)
        {
            return new Jugador(Id, Nombre, Saldo + monto);
        }
        // Este bloque es funcional porque devuelve una nueva instancia del jugador con el saldo actualizado, en lugar de modificar el estado interno.
    }
}
