using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Nucleo.Entidades
{
    public sealed class Ronda
    {
        public int Numero { get; }
        public DateTime Inicio { get; } = DateTime.UtcNow;
        public DateTime? Fin { get; private set; }

        public Ronda(int numero)
        {
            Numero = numero;
        }

        // Función pura para obtener el número de la ronda
        public int ObtenerNumero() => Numero;

        // Función pura para obtener la fecha de inicio de la ronda
        public DateTime ObtenerInicio() => Inicio;

        // Función pura para obtener la fecha de finalización de la ronda
        public DateTime? ObtenerFin() => Fin;

        // Método para finalizar la ronda (devuelve una nueva instancia con la fecha de finalización actualizada)
        public Ronda Finalizar()
        {
            return new Ronda(Numero)
            {
                Fin = DateTime.UtcNow
            };
        }
        //  Este bloque es funcional porque devuelve una nueva instancia de `Ronda` con la fecha de finalización actualizada, en lugar de modificar el estado interno.
    }
}
