using System;
using Microsoft.SPOT;

namespace RobotWallE
{
    class CReservoir
    {
        const int PAS_DE_ROTATION = 10; 

        struct configReservoir
        {
            public SerialPort portSerie;
            public byte idAx12Poussoir;
            public byte idAx12Rotateur;
            public OutputPort direction;
            public byte idCapteurReservoir;
        };

        enum positionReservoir
        {
          rentre = ,
          deploye = ,
        };

        CAX_12 m_ax12Poussoir;
        CAX_12 m_ax12Rotateur;
        CCapteurCouleur m_capteurCouleur;

        public CReservoir(ControleurAX12 controleur, configReservoir config)
        {
            m_capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir);
            m_ax12Poussoir = new CAX_12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
            m_ax12Rotateur = new CAX_12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
           // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
            //position initiale
            // m_ax12Pince.setMode(CAX_12.AX12Mode.joint);
        }

        public void rentrer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.rentree);
        }

        public void deployer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.deployee);
        }

        public void sortir(Couleur equipe) 
        {
            m_ax12Poussoir.deployer(equipe);
            m_ax12Poussoir.rentrer(equipe);
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
