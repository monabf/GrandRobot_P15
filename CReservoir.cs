using System;
using Microsoft.SPOT;

namespace RobotWallE
{
    class CReservoir
    {
        struct configReservoir
        {
            public byte idAx12Poussoir;
            public byte idAx12Rotateur;
            public byte idCapteurReservoir;
        };

        enum positionReservoir
        {
          rentre = 10,
          deploye = 10,
        };

        CAX_12 m_ax12Poussoir;
        CRouletteIntelligente m_rouletteIntelligente;

        public CReservoir(ControleurAX12 controleur, configReservoir config)
        {
            CCapteurCouleur capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir);
            CAX_12 ax12Rotateur = new CAX_12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
            m_rouletteIntelligente = CRouletteIntelligente(capteurCouleur, ax12Rotateur);
            m_ax12Poussoir = new CAX_12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
            m_ax12Poussoir.setMode(CAX_12.AX12Mode.joint);
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
            m_rouletteIntelligente.mettreBonneCouleur(equipe);
        }

    }
}
