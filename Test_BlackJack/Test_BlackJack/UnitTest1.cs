using System;
using System.Linq;
using Xunit;

// Referencias a tu biblioteca
using BlackJack_solid.Blackjack.Reglas;
using BlackJack_solid.Blackjack.Servicios;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJackTest
{
    public class BlackjackTests
    {
        [Fact]
        public void Dealer_FlujoCompleto_Reparte_Pedir_Finaliza_Evalua()
        {
            // Arrange
            IReglasJuego reglas = new ReglasBlackjack();
            var dealer = new Dealer(reglas);

            var j1 = new Jugador(id: 1, nombre: "Jacobo", saldo: 100);
            var j2 = new Jugador(id: 2, nombre: "CPU", saldo: 100);

            dealer.ConfigurarJugadores(new[] { j1, j2 });

            // Act
            dealer.IniciarRonda();
            dealer.RepartirElementos();

            // Assert inicial: dos cartas para cada uno y para el dealer
            var manoJ1 = dealer.ObtenerManoJugador(j1);
            var manoJ2 = dealer.ObtenerManoJugador(j2);
            var manoDealer = dealer.ObtenerManoDealer();

            Assert.Equal(2, manoJ1.Cartas.Count);
            Assert.Equal(2, manoJ2.Cartas.Count);
            Assert.Equal(2, manoDealer.Cartas.Count);

            // Jacobo pide una carta
            dealer.PedirCarta(j1);
            manoJ1 = dealer.ObtenerManoJugador(j1);
            Assert.True(manoJ1.Cartas.Count >= 3);

            // Dealer juega y finaliza la ronda
            dealer.FinalizarRonda();

            // Hay resultados para ambos jugadores
            var resultados = dealer.EvaluarResultados();
            Assert.True(resultados.ContainsKey(j1));
            Assert.True(resultados.ContainsKey(j2));

            // Puntos calculados son coherentes (0 < puntos <= 31 aprox. después de ajustes de A)
            var puntosJ1 = dealer.CalcularPuntos(manoJ1);
            var puntosJ2 = dealer.CalcularPuntos(manoJ2);
            var puntosD = dealer.CalcularPuntos(manoDealer);

            Assert.InRange(puntosJ1, 2, 31);
            Assert.InRange(puntosJ2, 2, 31);
            Assert.InRange(puntosD, 2, 31);
        }

        [Fact]
        public void ReglasBlackjack_ValorCartas_EsCorrecto()
        {
            // Arrange
            IReglasJuego reglas = new ReglasBlackjack();

            // Act / Assert
            Assert.Equal(11, reglas.ValorCarta("A"));
            Assert.Equal(10, reglas.ValorCarta("K"));
            Assert.Equal(10, reglas.ValorCarta("Q"));
            Assert.Equal(10, reglas.ValorCarta("J"));
            Assert.Equal(2, reglas.ValorCarta("2"));
            Assert.Equal(10, reglas.ValorCarta("10"));
        }

        [Fact]
        public void ServicioApuestas_RegistrarYCalcularGanancias_ActualizaSaldo()
        {
            // Arrange
            IReglasJuego reglas = new ReglasBlackjack();
            var srvApuestas = new ServicioApuestas(reglas);

            var jugador = new Jugador(id: 1, nombre: "Jacobo", saldo: 100);
            var apuesta = new Apuesta(jugador, monto: 20);

            // Act: registrar apuesta descuenta del saldo
            var registrada = srvApuestas.RegistrarApuesta(apuesta);
            Assert.True(registrada);
            Assert.Equal(80, jugador.ObtenerSaldo());

            // Calcular ganancias (regla 1:1 simplificada en tu servicio)
            var pagos = srvApuestas.CalcularGanancias();

            // Assert: se devuelve apuesta + premio (quedando saldo +20 respecto al inicio)
            Assert.True(pagos.ContainsKey(jugador));
            Assert.Equal(20, pagos[jugador]);     // premio
            Assert.Equal(120, jugador.ObtenerSaldo()); // 100 - 20 + (20*2) = 120
        }
    }
}
