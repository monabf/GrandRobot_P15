using System;
using Microsoft.SPOT;
using GR;
using PR;

namespace GR.Membres
{
    class CFunnyBras
    {

        enum positionFunnyBras
        {
            verouille = 10,
            deverouille = 10
        };

        CAX12 m_ax12FunnyBras;
        public byte m_idAx12FunnyBras;

        public CFunnyBras(ControleurAX12 controleur, byte idAx12FunnyBras)
        {
            m_idAx12FunnyBras = idAx12FunnyBras;
            m_ax12FunnyBras = new CAX12(idAx12FunnyBras, controleur.m_port, controleur.m_direction);
           // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
            //position initiale
            // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);

        }

        public void lancer(Couleur equipe)
        {
            m_ax12FunnyBras.move((int)positionFunnyBras.deverouille);
        }

    }
}
