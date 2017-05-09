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
       public struct configPince
        {
            public byte idAx12PinceSupport;
            public byte idAx12PinceModule;
        };

        // A COMPLETER

        enum positionPince
        {
          avancee = 200,
          rangee = 0,
          ouverte = 200,
          intermediaire = 100,
          fermee = 0
        };

        CAX12 m_ax12PinceSupport;
        CAX12 m_ax12PinceModule;

        public CPince(ControleurAX12 controleur, configPince config)
        {
            m_ax12PinceSupport = new CAX12(config.idAx12PinceSupport, controleur.m_port, controleur.m_direction);
            m_ax12PinceModule = new CAX12(config.idAx12PinceModule, controleur.m_port, controleur.m_direction);
           // m_ax12Pince.setMode(CAX12.AX12Mode.joint);
            //position initiale
            // m_ax12Pince.setMode(CAX12.AX12Mode.joint);

        }

        public void ouvrir(Couleur equipe)
        {
            m_ax12PinceSupport.move((int)positionPince.rangee);
            Thread.Sleep(1000);
            m_ax12PinceModule.move((int)positionPince.intermediaire);
            Thread.Sleep(1000);
        }

        public void fermer(Couleur equipe)
        {
            m_ax12PinceSupport.move((int)positionPince.avancee);
            Thread.Sleep(1000);
            m_ax12PinceModule.move((int)positionPince.fermee);
            Thread.Sleep(1000);
        }
    }
}
