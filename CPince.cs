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

        

        enum positionPince
        {
          avancee = 570,
          semiavancee = 470,
          rangee = 400,
          ouverte = 535,
          intermediaire = 475,
            petiteavancee = 430,
          fermee = 439
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

        public void sortir(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.ouverte);
            Thread.Sleep(50);
            m_ax12PinceSupport.move((int)positionPince.avancee);
            Thread.Sleep(50);
            
        }

        public void entrer(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.fermee);
            Thread.Sleep(50);
            m_ax12PinceSupport.move((int)positionPince.rangee);
            Thread.Sleep(50);
            
        }
        public void deserrer(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.intermediaire);
            Thread.Sleep(50);
        }
    }
}
