using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack_solid.Compartido
{
    public enum Palo { Corazones, Diamantes, Treboles, Picas }
    public enum Cara { A = 1, C2, C3, C4, C5, C6, C7, C8, C9, C10, J, Q, K }
    public enum AccionJugada { Pedir, Plantarse }
    public enum EstadoMesa { Cerrada, Abierta, EnRonda }
}
