using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack_solid.Blackjack.Servicios
{
    public sealed class ServicioApuestas : IServicioApuestas
    {
        private readonly IReglasJuego _reglas;
        private readonly List<IApuesta> _apuestas = new();

        public ServicioApuestas(IReglasJuego reglas) => _reglas = reglas;

        // Función pura para registrar una apuesta
        public IApuesta CrearApuesta(IJugador jugador, double monto)
        {
            if (!_reglas.ValidarApuesta(monto))
                throw new ArgumentException("Monto de apuesta inválido", nameof(monto));
            var apuesta = new Apuesta(jugador, monto);
            _apuestas.Add(apuesta);
            return apuesta;
        }

        public IApuesta ApuestaFinalizada(IApuesta apuesta)
        {
            if (apuesta is Apuesta a)
            {
                return a.Cerrar();
            }
            return apuesta;
        }

        /* public bool RegistrarApuesta(IApuesta apuesta)
        {
            if (!ValidarApuesta(apuesta)) return false;

            // Actualizar saldo del jugador y agregar la apuesta
            var jugador = apuesta.ObtenerJugador();
            jugador.ActualizarSaldo(-apuesta.ObtenerMonto());
            _apuestas.Add(apuesta);

            return true;
        } */
        //ValidarApuesta ahora es una función pura representada como una expresión lambda (Func<IApuesta, bool>). Esto hace que sea más declarativa y reutilizable.
        // Función pura para validar una apuesta
        /* public Func<IApuesta, bool> ValidarApuesta => apuesta =>
            apuesta is not null &&
            apuesta.EstaActiva() &&
            _reglas.ValidarApuesta(apuesta.ObtenerMonto()) &&
            apuesta.ObtenerJugador().ObtenerSaldo() >= apuesta.ObtenerMonto(); */

        //ObtenerApuestasActivas utiliza LINQ para filtrar las apuestas activas.
        //CalcularGanancias utiliza LINQ para calcular las ganancias y actualizar el saldo de los jugadores.
        // Función pura para obtener apuestas activas usando LINQ
        /* public IReadOnlyList<IApuesta> ObtenerApuestasActivas() =>
            _apuestas.Where(a => a.EstaActiva()).ToList(); */

        //Las operaciones como validar apuestas y calcular ganancias son más claras y fáciles de entender gracias al uso de funciones puras y expresiones funcionales.
        // Función pura para calcular ganancias usando LINQ
        /* public IDictionary<IJugador, double> CalcularGanancias()
        {
            return _apuestas
                .Where(a => a.EstaActiva())
                .ToDictionary(
                    a => a.ObtenerJugador(),
                    a =>
                    {
                        var premio = a.ObtenerMonto();
                        a.ObtenerJugador().ActualizarSaldo(premio * 2); // Actualizar saldo del jugador
                        (a as Apuesta)?.Cerrar(); // Cerrar la apuesta
                        return premio;
                    }
                );
        } */

        // Devuelve el monto de la última apuesta activa de un jugador
        public double ObtenerUltimaApuestaMonto(IJugador jugador)
        {
            var ultima = _apuestas.LastOrDefault(a => a.ObtenerJugador().ObtenerId() == jugador.ObtenerId());
            return ultima?.ObtenerMonto() ?? 0;
        }
    }
}
