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
            m_ax12PinceModule.setMode(AX12Mode.joint);
            m_ax12PinceSupport.setMode(AX12Mode.joint);
        }

        public void killPince()
        {
            m_ax12PinceModule.setMode(AX12Mode.wheel);
            m_ax12PinceSupport.setMode(AX12Mode.wheel);
        }

        public void Sortir(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.ouverte);
            Thread.Sleep(150);
            m_ax12PinceSupport.move((int)positionPince.avancee);
            Thread.Sleep(150);
            
        }

        public void Sortir(Couleur equipe, int position_ideale)
        {
            m_ax12PinceModule.move((int)positionPince.ouverte);
            Thread.Sleep(150);
            m_ax12PinceSupport.move(position_ideale);
            Thread.Sleep(150);
        }

        public void Entrer(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.fermee);
            Thread.Sleep(150);
            m_ax12PinceSupport.move((int)positionPince.rangee);
            Thread.Sleep(150);
            
        }
        public void Deserrer(Couleur equipe)
        {
            m_ax12PinceModule.move((int)positionPince.intermediaire);
            Thread.Sleep(150);
        }
    }
}
