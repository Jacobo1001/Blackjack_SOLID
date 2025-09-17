using BlackJack_solid.Blackjack.Reglas;
using BlackJack_solid.Blackjack.Servicios;
using BlackJack_solid.Nucleo.Controladores;
using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;

namespace BlackJack_solid.Nucleo.Factories
{
    public interface IGameFactory
    {
        IGameController CrearGameController();
        IJugador CrearJugador(int id, string nombre, double saldo);
        IReglasJuego CrearReglasBlackjack();
        IServicioMesa CrearServicioMesa();
        IServicioApuestas CrearServicioApuestas(IReglasJuego reglas);
        IDealer CrearDealer(IReglasJuego reglas);
    }

    public sealed class JuegoFabrica : IGameFactory
    {
        public IGameController CrearGameController()
        {
            var reglas = CrearReglasBlackjack();
            var servicioMesa = CrearServicioMesa();
            var servicioApuestas = CrearServicioApuestas(reglas);
            var dealer = CrearDealer(reglas);
            var jugador = CrearJugador(1, "Tú", 1000.0);

            return new GameController(reglas, servicioMesa, servicioApuestas, dealer, jugador);
        }

        public IJugador CrearJugador(int id, string nombre, double saldo)
        {
            return new Jugador(id, nombre, saldo);
        }

        public IReglasJuego CrearReglasBlackjack()
        {
            return new ReglasBlackjack();
        }

        public IServicioMesa CrearServicioMesa()
        {
            return new ServicioMesa();
        }

        public IServicioApuestas CrearServicioApuestas(IReglasJuego reglas)
        {
            return new ServicioApuestas(reglas);
        }

        public IDealer CrearDealer(IReglasJuego reglas)
        {
            return new Dealer(reglas);
        }
    }
}
