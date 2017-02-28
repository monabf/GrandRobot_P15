using System;
using Microsoft.SPOT;

namespace RobotWallE
{
    class CFunnyBras
    {
        struct configFunnyBras
        {
            public byte idAx12FunnyBras;
        };

        enum positionFunnyBras
        {
          verouille = ,
          deverouille = ,
        };

        CAX_12 m_ax12FunnyBras;

        public CFunnyBras(ControleurAX12 controleur, configFunnyBras config)
        {
            m_ax12FunnyBras = new CAX_12(config.idAx12FunnyBras, controleur.m_port, controleur.m_direction);
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
