using System;
using Microsoft.SPOT;
using System.IO.Ports;
using GT = Gadgeteer;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace GR.Membres
{
    class CPince
    {
        struct configPince
        {
            public byte idAx12PinceSupport;
            public byte idAx12PinceModule;
        };

        enum positionPince
        {
          avancee = ,
          rangee = ,
          ouverte = ,
          intermediaire = ,
          fermee = ,
        };

        CAX_12 m_ax12PinceSupport;
        CAX_12 m_ax12PinceModule;

        public CPince(ControleurAX12 controleur, configPince config)
        {
            m_ax12PinceSupport = new CAX_12(config.idAx12PinceSupport, controleur.m_port, controleur.m_direction);
            m_ax12PinceModule = new CAX_12(config.idAx12PinceModule, controleur.m_port, controleur.m_direction);
           // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
            //position initiale
            // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);

        }

        public void ouvrir(Couleur equipe)
        {
            m_ax12PinceSupport.move((int)positionPince.rangee);
            m_ax12PinceModule.move((int)positionPince.intermediaire);
        }

        public void fermer(Couleur equipe)
        {
            m_ax12PinceSupport.move((int)positionPince.avancee);
            m_ax12PinceModule.move((int)positionPince.fermee);
        }
    }
}
