using System;
using Microsoft.SPOT;
using GR;

namespace GR.Membres
{
    class CFunnyBras
    {

        enum positionFunnyBras
        {
            verouille = 490, // purement à titre indicatif
            deverouille = 570
        };

        CAX12 m_ax12FunnyBras;

        public CFunnyBras(ControleurAX12 controleur, byte idAx12FunnyBras)
        {
            m_ax12FunnyBras = new CAX12(idAx12FunnyBras, controleur.m_port, controleur.m_direction);

        }

        public void lancer()
        {
            m_ax12FunnyBras.move((int)positionFunnyBras.deverouille);
        }

    }
}
