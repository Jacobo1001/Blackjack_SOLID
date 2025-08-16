using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Blackjack.Servicios
{
    public sealed class ServicioApuestas : IServicioApuestas
    {
        private readonly IReglasJuego _reglas;
        private readonly List<IApuesta> _apuestas = new();

        public ServicioApuestas(IReglasJuego reglas) => _reglas = reglas;

        public bool RegistrarApuesta(IApuesta apuesta)
        {
            if (!ValidarApuesta(apuesta)) return false;
            apuesta.ObtenerJugador().ActualizarSaldo(-apuesta.ObtenerMonto());
            _apuestas.Add(apuesta);
            return true;
        }

        public bool ValidarApuesta(IApuesta apuesta) =>
            apuesta is not null &&
            apuesta.EstaActiva() &&
            _reglas.ValidarApuesta(apuesta.ObtenerMonto()) &&
            apuesta.ObtenerJugador().ObtenerSaldo() >= apuesta.ObtenerMonto();

        public IReadOnlyList<IApuesta> ObtenerApuestasActivas() =>
            _apuestas.Where(a => a.EstaActiva()).ToList();

        // Ejemplo simplificado: se paga 1:1 a todos
        public IDictionary<IJugador, double> CalcularGanancias()
        {
            var pagos = new Dictionary<IJugador, double>();
            foreach (var a in _apuestas)
            {
                var premio = a.ObtenerMonto(); 
                a.ObtenerJugador().ActualizarSaldo(premio * 2); 
                pagos[a.ObtenerJugador()] = premio;
                (a as Apuesta)?.Cerrar();
            }
            return pagos;
        }
    }
}
