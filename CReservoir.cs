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

        public struct configReservoir
        {
            public byte idAx12Poussoir;
            public byte idAx12Rotateur;
            public byte idCapteurReservoir;
        };

        enum positionReservoir
        {
          rentre = 345,
          deploye = 830
        };

        CAX12 m_ax12Poussoir;
        CRouletteIntelligente m_rouletteIntelligente;
        Couleur couleurEquipe;

        public CReservoir(Couleur equipe, ControleurAX12 controleur, configReservoir config)
        {
            couleurEquipe = equipe;
            CCapteurCouleur capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir, couleurEquipe);
            CAX12 ax12Rotateur = new CAX12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
            
            m_rouletteIntelligente = new CRouletteIntelligente(capteurCouleur, ax12Rotateur);
            m_ax12Poussoir = new CAX12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
            m_ax12Poussoir.setMode(AX12Mode.joint);
        }

        public void rentrer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.rentre);
            Thread.Sleep(500);
        }

        public void deployer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.deploye);
            Thread.Sleep(500);
        }

        public void sortir(Couleur equipe) 
        {
            deployer(equipe);
            Thread.Sleep(500);
            rentrer(equipe);
            Thread.Sleep(500);
        }

        public void tourner(Couleur equipe)
        {
            m_rouletteIntelligente.mettreBonneCouleur(equipe);
        }

    }
}
