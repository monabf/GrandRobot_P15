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
        CAX_12 m_ax12Pince;

        public CPince(ControleurAX12 controleur, byte idPince)
        {
            m_ax12Pince = new CAX_12(idPince, controleur.m_port, controleur.m_direction);
           // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
            m_ax12Pince.move(818); //position initiale        
        }

        public void ouvrir(Couleur equipe)
        {
            //m_ax12Pince.move(894);
            m_ax12Pince.move(equipe == Couleur.Violet ? 866 : 770);
        }
        public void fermer(Couleur equipe)
        {
            m_ax12Pince.move(equipe == Couleur.Violet ? 790 : 846);
            
        }
    }
}
