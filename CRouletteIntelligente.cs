using System;
using Microsoft.SPOT;

namespace GR.Membres;
{
  class CRouletteIntelligente
  {

    CAX_12 m_ax12Roulette;
    CCapteurCouleur m_capteurCouleur;

    public CRoulette(CCapteurCouleur capteurCouleur, CAX_12 ax12Roulette)
    {
      m_capteurCouleur = capteurCouleur;
      m_ax12Roulette = ax12Roulette;
      m_ax12Roulette.setMode(CAX_12.AX12Mode.wheel);
    }

    public void mettreBonneCouleur(Couleur equipe) {
      m_ax12Roulette.setMovingSpeed(speed.forward);
      while (m_capteurCouleur.continuerRotation())
      {
        // Mettre un Thread.sleep?
        Thread.sleep(100);
      }
      m_ax12Roulette.setMovingSpeed(speed.stop);
    }
  }
}
