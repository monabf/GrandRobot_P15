using System;
using Microsoft.SPOT;
using System.IO.Ports;
using GT = Gadgeteer;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace GR.Membres
{
    class CReservoir
    {
        const int PAS_DE_ROTATION = 10; 

        public struct configReservoir
        {
            public byte idAx12Poussoir;
            public byte idAx12Rotateur;
            public byte idCapteurReservoir;
        };

        enum positionReservoir
        {
          rentre = 10,
          deploye = 10
        };

        CAX_12 m_ax12Poussoir;
        CAX_12 m_ax12Rotateur;
        CCapteurCouleur m_capteurCouleur;

        public CReservoir(ControleurAX12 controleur, configReservoir config)
        {
            m_capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir);
            m_ax12Poussoir = new CAX_12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
            m_ax12Rotateur = new CAX_12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
           //m_ax12Pince.setMode(CAX_12.AX12Mode.wheel);
            //position initiale
            // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
        }

        public void rentrer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.rentre);
        }

        public void deployer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.deploye);
        }

        public void sortir(Couleur equipe) 
        {
            deployer(equipe);
            rentrer(equipe);
        }

        public void tourner(Couleur equipe)
        {
            while (m_capteurCouleur.continuerRotation()) 
            {
                m_ax12Rotateur.move(PAS_DE_ROTATION);
            }
        }

    }
}
