using System.Threading;
using Microsoft.SPOT;

namespace PR.Membres
{
  class CRouletteIntelligente
  {

        CAX12 m_ax12Roulette;
        CCapteurCouleur m_capteurCouleur;

        public CRouletteIntelligente(CCapteurCouleur capteurCouleur, CAX12 ax12Roulette)
        {
            m_capteurCouleur = capteurCouleur;
            m_ax12Roulette = ax12Roulette;
            // la ligne ci-dessous a été commentée pour les tests. La décommenter après
            //m_ax12Roulette.setMode(AX12Mode.wheel);
            Debug.Print("roulette opérationnelle");
        }

        public CAX12 getRoulette(){
            return m_ax12Roulette;
        }

        public void Tourner(Couleur equipe) {
            // les lignes ci-dessous a été commentées pour les tests. La décommenter après
            if (equipe == Couleur.Bleu)
            {
                m_ax12Roulette.setMovingSpeed(speed.forward);
            }
            else
            {
                m_ax12Roulette.setMovingSpeed(speed.reverse);
            }
            while (m_capteurCouleur.ContinuerRotation())
            {
                //Thread.Sleep(10);
                //Debug.Print(""+i++);
            }
            m_ax12Roulette.setMovingSpeed(speed.stop);
             
        }
      /*
        public void MettreBonneCouleurLoinDuCapteur(Couleur equipe)
        {
            // "opposé" de la fonction précédente !
            if (equipe == Couleur.Jaune)
            {
                MettreBonneCouleurPresDuCapteur(Couleur.Bleu);
            }
            else
            {
                MettreBonneCouleurPresDuCapteur(Couleur.Jaune);
            }
        }
       * */
    }
}
