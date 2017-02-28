using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.SocketInterfaces;
using Microsoft.SPOT.Hardware;

namespace GR
{
    /// <summary>
    /// Jack sur entrée numérique
    /// </summary>
    class Jack
    {
        private readonly InputPort Entree;

        /// <summary>
        /// Etat de l'entrée numérique
        /// </summary>
        public bool Etat { get { return Entree.Read(); } }

        /// <summary>
        /// Initialise le jack
        /// </summary>
        /// <param name="numPort">Numéro de port</param>
        /// <param name="numPin">Numéro de pin</param>
        public Jack( int numPort, int numPin)
        {
            Entree = new InputPort(Socket.GetSocket(numPort, true, null, null).CpuPins[numPin], false, Port.ResistorMode.PullUp);
        }
    }
}
