using System;
using Microsoft.SPOT;
using System.Threading;

namespace GR.Membres
{
  class CRouletteIntelligente
  {

    CAX12 m_ax12Roulette;
    CCapteurCouleur m_capteurCouleur;

    public CRouletteIntelligente(CCapteurCouleur capteurCouleur, CAX12 ax12Roulette)
    {
      m_capteurCouleur = capteurCouleur;
      m_ax12Roulette = ax12Roulette;
      m_ax12Roulette.setMode(AX12Mode.wheel);
    }

    public void mettreBonneCouleur(Couleur equipe) {
      m_ax12Roulette.setMovingSpeed(speed.forward);
      while (m_capteurCouleur.ContinuerRotation())
      {
        // Mettre un Thread.sleep?
      }
      m_ax12Roulette.setMovingSpeed(speed.stop);
    }
  }
}
