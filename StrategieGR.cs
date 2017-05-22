using System;
using Microsoft.SPOT;
using System.Threading;
using System.Collections;
using GR.Membres;
using GR.BR;

namespace GR
{
    partial class GrandRobot
    {
        // ATTENTION POUR LA COUPE METTRE HOMOLATION = TRUE POUR TOUTES LES DISPOSITIONS
        private bool SortirZoneDepart()
        {
//Homologation : récupérer le 1er cylindre et reTourner dans la zone bleue pour l'y reposer, sans le mettre dans le réservoir
//entre les deux (le bras le garde simplement), s'arrêter si le capteur ultrasons réagit (l'arrêt est codé dans la classe capteur ultrasons)
            bool homologation = (robotGetDisposition() == 1);
            m_ihm.retourPhase(Couleurs.violet);
            reservoir.Rentrer(m_etatRobot.couleurEquipe);
            pince.Sortir(m_etatRobot.couleurEquipe);
            if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                robotGoToXY(400, 974, sens.avancer, homologation);
                pince.Entrer(m_etatRobot.couleurEquipe);
                robotGoToXY(400, 1130, sens.avancer, false);
            }
            else
            {
                robotGoToXY(X_TABLE - 400, 974, sens.avancer, homologation);
                pince.Entrer(m_etatRobot.couleurEquipe);
                robotGoToXY(X_TABLE - 400, 1130, sens.avancer, false);
            }

            return true;
        }



        private bool RecupererCylindre1()
        {
            cylindresRecup++;
            Debug.Print("RecupererCylindre1");
            m_ihm.retourPhase(Couleurs.indigo);
            bras.SemiDescendre(m_etatRobot.couleurEquipe);
            pince.Deserrer(m_etatRobot.couleurEquipe);
            bras.Descendre(m_etatRobot.couleurEquipe);
            bras.Monter(m_etatRobot.couleurEquipe);
            bras.Lacher(m_etatRobot.couleurEquipe);
            return true;
        }

        private bool RecupererCylindresFusee()
        {
            // dans toute cette partie la detection est coupee
            m_ihm.retourPhase(Couleurs.bleu);
            cylindresRecup+=4; //int défini dans GrandRobot.cs
            var thReservoir = new Thread(() =>
            {
                reservoir.Tourner(m_etatRobot.couleurEquipe);
            }
            );
            thReservoir.Start();
            Thread.Sleep(200); //ne pas raccourcir
            if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                robotGoToXY(240, 1130, sens.avancer);
            }
            else
            {
                robotGoToXY(X_TABLE - 240, 1130, sens.avancer);
            }

            for (int i = 0; i < 4; i++) //Une solution pour raccourcir la stratégie si on arrive pas en 90s : ignorer le dernier cylindre 
            //for (int i = 0; i < 3; i++)
            {
                pince.Sortir(m_etatRobot.couleurEquipe);
                pince.Entrer(m_etatRobot.couleurEquipe);

                if (i < 3) 
                {
                    if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                    {
                        robotGoToXY(300, 1130, sens.reculer);
                    }
                    else
                    {
                        robotGoToXY(X_TABLE - 300, 1130, sens.reculer);
                    }
                }

                bras.SemiDescendre(m_etatRobot.couleurEquipe);
                pince.Deserrer(m_etatRobot.couleurEquipe);
                bras.Descendre(m_etatRobot.couleurEquipe);
                bras.Monter(m_etatRobot.couleurEquipe);
                if (i < 3)
                {
                    bras.Lacher(m_etatRobot.couleurEquipe);
                    {
                        if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                        {
                            robotGoToXY(240, 1130, sens.reculer);
                        }
                        else
                        {
                            robotGoToXY(X_TABLE - 240, 1130, sens.reculer);
                        }
                    }
                }
            }
        
            Debug.Print("RecupererCylindreFusee");

            return true;
        }

        private bool RecupererCylindre2()
        {
            bool homologation = (robotGetDisposition() == 1);
              cylindresRecup++; //int défini dans GrandRobot.cs
              m_ihm.retourPhase(Couleurs.vert);
              if (m_etatRobot.couleurEquipe == Couleur.Bleu)
              {
                  robotGoToXY(350, 1130, sens.reculer);
                  robotGoToXY(1200, 720, sens.avancer, homologation);
                  robotGoToXY(1240, 755, sens.avancer, homologation);
                  pince.Sortir(m_etatRobot.couleurEquipe);
                  pince.Entrer(m_etatRobot.couleurEquipe);
                  robotGoToXY(1200, 725, sens.reculer, homologation);
                  robotGoToXY(1355, 878, sens.reculer, homologation);
              }
              else
              {
                  robotGoToXY(X_TABLE - 350, 1130, sens.reculer, homologation);
                  robotGoToXY(X_TABLE - 1200, 720, sens.avancer, homologation);
                  robotGoToXY(X_TABLE - 1240, 755, sens.avancer, homologation);
                  pince.Sortir(m_etatRobot.couleurEquipe);
                  pince.Entrer(m_etatRobot.couleurEquipe);
                  robotGoToXY(X_TABLE - 1200, 725, sens.reculer, homologation);
                  robotGoToXY(X_TABLE - 1355, 878, sens.reculer, homologation);
              }
              Debug.Print("RecupererCylindre2");
              return true;
        }

        private bool DeposerCylindres()
        {
            m_ihm.retourPhase(Couleurs.jaune);
            for (int i = 1; i < 5; i++)
            {
                reservoir.Sortir(m_etatRobot.couleurEquipe);
                Thread.Sleep(500);
            }
            bras.Lacher(m_etatRobot.couleurEquipe);
            Thread.Sleep(500);
            var thReservoir2 = new Thread(() =>
            {
                reservoir.Sortir(m_etatRobot.couleurEquipe);
                bras.SemiDescendre(m_etatRobot.couleurEquipe);
                pince.Deserrer(m_etatRobot.couleurEquipe);
                bras.Descendre(m_etatRobot.couleurEquipe);
                bras.Monter(m_etatRobot.couleurEquipe);
                bras.Lacher(m_etatRobot.couleurEquipe);
            });
            thReservoir2.Start();
            reservoir.Tourner(m_etatRobot.couleurEquipe);
            reservoir.Sortir(m_etatRobot.couleurEquipe);
            // et deux sorties de plus au cas où
            Thread.Sleep(500);
            reservoir.Sortir(m_etatRobot.couleurEquipe);
            Thread.Sleep(500);
            reservoir.Sortir(m_etatRobot.couleurEquipe);

            Debug.Print("DeposerCylindre");
            m_ihm.retourPhase(Couleurs.orange);

            return true;
        }

        private void InitialiserStrategie()
        {
            Debug.Print("stratégie active");

            Strategie.Ajouter(new ActionRobot(SortirZoneDepart, ()=>true, ()=> 100, executionUnique: true));
            Strategie.Ajouter(new ActionRobot(RecupererCylindre1, ()=>true, () => 100, true));
            Strategie.Ajouter(new ActionRobot(RecupererCylindresFusee, () => true, () => 99, true));
            Strategie.Ajouter(new ActionRobot(RecupererCylindre2, () => true, () => 95, true));
            Strategie.Ajouter(new ActionRobot(DeposerCylindres, () => true, () => 94, true));

        }


        //CODE POUR FAIRE UNE DÉMO!!!!!!!!!!! (pas d'action des servos moteurs, seulement des mouvements)
/*
        private bool SortirZoneDepart()
        {
          Tracage.Ecrire("Sortie de la zone de depart");

          Tourner(12);
          bool SortieOK = AllerEn(341, m_etatRobot.couleurEquipe == Couleur.Bleu ? 946 : 2054, sens.avancer) == etat.arrive;
          //on prend la convention Couleur.Bleu == zone de départ bleue, sinon zone de départ jaune ; en cm pour l'instant, à voir ! Attention aussi aux conventions de repère, ici on a pris y vers le bas et angle positif en sens horaire mais pas forcément vrai
          //rappeler à PED de coder etat car permet savoir si arrive/perdu... et adapter le nom en fonction de ce qu'il choisit

          Tracage.Ecrire(SortieOK ? "Sortie reussie" : "Sortie echouee");
          //si SortieOk == etat.arrive alors on écrit Sortie reussie, sinon on écrit Sortie echouee

          return SortieOK;
        }

        private bool RecupererCylindre1()
        {
          cylindresRecup++; //À CODER (pas essentiel) : un entier haut niveau qui compte le nombre de cylindres déjà récupérés

          Tracage.Ecrire("Recuperation du 1er cylindre");
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? 78 : -78);
          return true;
        }

        private bool RecupererCylindresFusee()
        {
          cylindresRecup+=4; //int à créer pour en faire un indice

          Tracage.Ecrire("Positionnement devant la fusee et recuperation des 4 cylindres");

          AllerEn(341,m_etatRobot.couleurEquipe==Couleur.Bleu ? 1150 : 1850, sens.avancer);
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? 90 : -90);
          AllerEn(310, m_etatRobot.couleurEquipe==Couleur.Bleu ? 1150 : 1850, sens.avancer);

          return true;
        }

        private bool RecupererCylindre2()
        {
          cylindresRecup++; //int à créer pour en faire un indice

          Tracage.Ecrire("Recuperation du 2eme cylindre");

          AllerEn(850,1150, m_etatRobot.couleurEquipe==Couleur.Bleu ? sens.avancer : sens.reculer);
          Tourner(180);
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1100, m_etatRobot.couleurEquipe==Couleur.Bleu ? 900 : 2100, sens.avancer);
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? 45 : -45);
          AllerEn(1136, m_etatRobot.couleurEquipe==Couleur.Bleu ? 900 : 2100);

          return true;
        }

        private bool DeposerCylindres()
        {


          Tracage.Ecrire("Depot des cylindres");

          AllerEn(1125, m_etatRobot.couleurEquipe==Couleur.m_etatRobot.couleurEquipe ? 900 : 2100, sens.reculer);
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1275, m_etatRobot.couleurEquipe==Couleur.Bleu ? 750 : 2250, sens.avancer);
          Tourner(m_etatRobot.couleurEquipe==Couleur.Bleu ? -90 : 90);
          AllerEn(1427, m_etatRobot.couleurEquipe==Couleur.Bleu ? 927 : 2073, sens.reculer);


          return true;
        }

 

        /*private void InitialiserStrategie()
        {
            // test unitaire: moyenne d'erreur sur x avances d'1m
            var nbEssais = 2;
            var historique = new ArrayList();
            var dir = -1;

            Strategie.Ajouter(new ActionRobot(() =>
            {
                var posBRDebut = new positionBaseRoulante();
                var posBRFin = new positionBaseRoulante();
                var diff = new positionBaseRoulante();
                float erreur;

                Tracage.Ecrire("Test no. " + (historique.Count + 1).ToString());
                Tracage.Ecrire("Deplacement de 1000mm sur l'axe y");
                BaseRoulante.getPosition(ref posBRDebut);

                Tracage.Ecrire("Position BR estimee (mm): [ " + posBRDebut.x + " ; " + posBRDebut.y + " ]");
                Tracage.Ecrire("Demarrage..");

                AllerEn(posBRDebut.x, posBRDebut.y + 1000 * (dir *= -1), dir > 0 ? BR.sens.reculer : BR.sens.avancer);
                BaseRoulante.getPosition(ref posBRFin);

                Tracage.Ecrire("Position BR estimee (mm): [ " + posBRFin.x + " ; " + posBRFin.y + " ]");

                diff.x = (ushort)System.Math.Abs(posBRFin.x - posBRDebut.x);
                diff.y = (ushort)System.Math.Abs(posBRFin.y - posBRDebut.y);
                erreur = ((float)(1000 - diff.y) / 10);

                Tracage.Ecrire("Difference estimee (mm): [ " + diff.x + " ; " + diff.y + " ]");
                Tracage.Ecrire("Erreur estimee: " + erreur.ToString() + "%");
                historique.Add(erreur);

                return true;
            }, condition: () => historique.Count < nbEssais));

            Strategie.Ajouter(new ActionRobot(
                () =>
                {
                    float somme = 0f;

                    for (int i = 0; i < historique.Count; i++)
                        somme += (float)historique[i];

                    Tracage.Ecrire("Moyenne d'erreur sur " + historique.Count.ToString() + " essais: " + (somme / historique.Count).ToString() + "%");

                    return true;
                }, condition: () => historique.Count >= nbEssais,
                executionUnique: true));
        }*/
    }
}
