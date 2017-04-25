using System;
using Microsoft.SPOT;
using System.IO.Ports;
using GT = Gadgeteer;
using System.Threading;
using Microsoft.SPOT.Hardware;
using PR;
using GadgeteerApp4;

namespace GR.Membres
{
    class CReservoir
    {

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

        CAX12 m_ax12Poussoir;
        CRouletteIntelligente m_rouletteIntelligente;

        public CReservoir(ControleurAX12 controleur, configReservoir config)
        {
            CCapteurCouleur capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir);
            CAX12 ax12Rotateur = new CAX12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
            m_ax12Poussoir.setMode(AX12Mode.joint);
            m_rouletteIntelligente = CRouletteIntelligente(capteurCouleur, ax12Rotateur);
            m_ax12Poussoir = new CAX12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
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
            m_rouletteIntelligente.mettreBonneCouleur(equipe);
        }

    }
}
