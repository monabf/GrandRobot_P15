using System;
using Microsoft.SPOT;
using System.IO.Ports;
using GT = Gadgeteer;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace GR.Membres
{
    class CParasol
    {
        CAX_12 m_ax12Parasol;

        public CParasol(ControleurAX12 controleur, byte id)
        {
            m_ax12Parasol = new CAX_12(id, controleur.m_port, controleur.m_direction);
        //    m_ax12Parasol.setMode(CAX_12.AX12Mode.joint);
            m_ax12Parasol.move(641); //position initiale        
        }

        public void deployer()
        {
            m_ax12Parasol.move(512);
        }
    }
}
