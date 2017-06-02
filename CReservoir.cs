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
        public CRouletteIntelligente m_rouletteIntelligente;
        Couleur couleurEquipe;

        public CReservoir(Couleur equipe, ControleurAX12 controleur, configReservoir config)
        {
            couleurEquipe = equipe;
            CCapteurCouleur capteurCouleur = new CCapteurCouleur(config.idCapteurReservoir, couleurEquipe);
            CAX12 ax12Rotateur = new CAX12(config.idAx12Rotateur, controleur.m_port, controleur.m_direction);
            ax12Rotateur.setMode(AX12Mode.wheel);
            
            m_rouletteIntelligente = new CRouletteIntelligente(capteurCouleur, ax12Rotateur);
            m_ax12Poussoir = new CAX12(config.idAx12Poussoir, controleur.m_port, controleur.m_direction);
            m_ax12Poussoir.setMode(AX12Mode.joint);
        }

        public void killReservoir()
        {
            m_ax12Poussoir.setMode(AX12Mode.wheel);
            m_rouletteIntelligente.getRoulette().setMode(AX12Mode.joint);
        }

        public void Rentrer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.rentre);
            Thread.Sleep(500);
        }

        public void Deployer(Couleur equipe)
        {
            m_ax12Poussoir.move((int)positionReservoir.deploye);
            Thread.Sleep(500);
        }

        public void Sortir(Couleur equipe) 
        {
            Deployer(equipe);
            Thread.Sleep(400);
            Rentrer(equipe);
            Thread.Sleep(400);
        }

        public void Tourner(Couleur equipe)
        {
            m_rouletteIntelligente.mettreBonneCouleur(equipe);
        }

        public void arretUrgenceRoulette()
        {
            m_rouletteIntelligente.getRoulette().setMovingSpeed(speed.stop);
        }

    }
}
