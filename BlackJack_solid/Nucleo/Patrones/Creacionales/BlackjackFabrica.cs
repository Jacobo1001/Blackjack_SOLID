using BlackJack_solid.Nucleo.Entidades;
using BlackJack_solid.Nucleo.Interfaces;
using BlackJack_solid.Compartido;
using BlackJack_solid.Blackjack.Servicios;

namespace BlackJack_solid.Nucleo.Patrones.Creacionales
{
    // FACTORY METHOD PATTERN
    // Define la interfaz para crear objetos del juego sin especificar sus clases concretas.
    // Permite que las subclases decidan qué clase instanciar.
    public interface IBlackjackFactory
    {
        IJugador CrearJugador(int id, string nombre, double saldo);
        IDealer CrearDealer(IReglasJuego reglas);
        IMesa CrearMesa(int id, IReglasJuego reglas);
        Carta CrearCarta(Palo palo, Cara cara);
        IApuesta CrearApuesta(IJugador jugador, double monto);
        Mano CrearMano();
    }

    // FACTORY METHOD PATTERN - Implementación Estándar
    // Crea objetos básicos del juego Blackjack con configuración estándar.
    public sealed class BlackjackFactoryEstandar : IBlackjackFactory
    {
        public IJugador CrearJugador(int id, string nombre, double saldo)
        {
            // Factory Method: Crear jugador con configuración estándar
            return new Jugador(id, nombre, saldo);
        }

        public IDealer CrearDealer(IReglasJuego reglas)
        {
            // Factory Method: Crear dealer con reglas específicas
            return new Dealer(reglas);
        }

        public IMesa CrearMesa(int id, IReglasJuego reglas)
        {
            // Factory Method: Crear mesa con configuración estándar
            return new Mesa(id, reglas);
        }

        public Carta CrearCarta(Palo palo, Cara cara)
        {
            // Factory Method: Crear carta individual
            return new Carta(palo, cara);
        }

        public IApuesta CrearApuesta(IJugador jugador, double monto)
        {
            // Factory Method: Crear apuesta básica
            return new Apuesta(jugador, monto);
        }

        public Mano CrearMano()
        {
            // Factory Method: Crear mano vacía
            return new Mano();
        }
    }

}
