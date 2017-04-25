using System;
using Microsoft.SPOT;
using GadgeteerApp4;
using Grand_Robot;
using PR;
using System.Threading;

namespace GR.Membres
{
  class CRouletteIntelligente
  {

    CAX12 m_ax12Roulette;
    CCapteurCouleur m_capteurCouleur;

    public void CRoulette(CCapteurCouleur capteurCouleur, CAX12 ax12Roulette)
    {
      m_capteurCouleur = capteurCouleur;
      m_ax12Roulette = ax12Roulette;
      m_ax12Roulette.setMode(AX12Mode.wheel);
    }

    public void mettreBonneCouleur(Couleur equipe) {
      m_ax12Roulette.setMovingSpeed(speed.forward);
      while (m_capteurCouleur.ContinuerRotation(equipe))
      {
        // Mettre un Thread.sleep?
        Thread.sleep(100);
      }
      m_ax12Roulette.setMovingSpeed(speed.stop);
    }
  }
}
