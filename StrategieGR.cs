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
        public Thread thRanger;
        public Thread thReservoir;
        public Thread thReservoir2;

        // ATTENTION POUR LA COUPE METTRE HOMOLATION = TRUE POUR TOUTES LES DISPOSITIONS
        private bool SortirZoneDepart()
        {
//Homologation : récupérer le 1er cylindre et reTourner dans la zone bleue pour l'y reposer, sans le mettre dans le réservoir
//entre les deux (le bras le garde simplement), s'arrêter si le capteur ultrasons réagit (l'arrêt est codé dans la classe capteur ultrasons)
           // bool homologation = (robotGetDisposition() == 1);
            bool homologation = false;

            //m_ihm.retourPhase(Couleurs.violet);
            var thRanger = new Thread(() =>
            {
                pince.Sortir(m_etatRobot.couleurEquipe);
                reservoir.Rentrer(m_etatRobot.couleurEquipe);
            });
            thRanger.Start();
            // attention la XXX de pince n'est pas au centre du robot lorsque celui-ci est déployé


            return true;
        }

        private bool Homologation()
        {
            if (robotGetDisposition() == 1)
            {
                /*
                pince.Sortir(m_etatRobot.couleurEquipe);
                if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                {
                    robotGoToXY(405, 960, sens.avancer, false);
                    pince.Entrer(m_etatRobot.couleurEquipe);
                    robotGoToXY(440, 1060, sens.avancer, true);
                }
                else
                {
                    robotGoToXY(400, Y_TABLE - 950, sens.avancer, false);
                    Thread.Sleep(200);
                    pince.Entrer(m_etatRobot.couleurEquipe);
                    Thread.Sleep(200);
                    robotGoToXY(440, Y_TABLE - 1050, sens.avancer, true);
                }
                Thread.Sleep(200);


                if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                {
                    robotGoToXY(180, 920, sens.reculer, true);
                }
                else
                {
                    robotGoToXY(180, Y_TABLE - 920, sens.reculer, true);
                }
                Thread.Sleep(200);
                pince.Sortir(m_etatRobot.couleurEquipe);

                Thread.Sleep(100000);
                **/
            }
                 
            return true;
        }

        private bool RecupererCylindre1()
        {
            bool homologation = false;
            if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                robotGoToXY(460, (ushort)(1000 - deltafusee), sens.avancer, homologation);

            }
            else
            {
                // trouver position exacte
                robotGoToXY(462, (ushort)(Y_TABLE - 990 + deltafusee), sens.avancer, homologation);
            }
            cylindresRecup++;
            Debug.Print("RecupererCylindre1");
            m_ihm.retourPhase(Couleurs.indigo);

            return true;
        }

        private bool RecupererCylindresFusee()
        {
            bool homologation = false;

            // dans toute cette partie la detection est coupee
            m_ihm.retourPhase(Couleurs.bleu);
            cylindresRecup+=4; //int défini dans GrandRobot.cs
            thReservoir = new Thread(() =>
            {
                pince.Entrer(m_etatRobot.couleurEquipe);
                bras.SemiDescendre(m_etatRobot.couleurEquipe);
                pince.Deserrer(m_etatRobot.couleurEquipe);
                bras.Descendre(m_etatRobot.couleurEquipe);
                bras.Monter(m_etatRobot.couleurEquipe);
                bras.Lacher(m_etatRobot.couleurEquipe);
                Thread.Sleep(300);
                reservoir.Tourner(m_etatRobot.couleurEquipe);
            }
            );
            thReservoir.Start();
            Thread.Sleep(200); //ne pas raccourcir

            if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                // ligne pour l'évitement désespéré !
                //robotGoToXY(480, (ushort)(1160 - deltafusee), sens.avancer, homologation);
                robotGoToXY(400, (ushort) (1150-deltafusee), sens.avancer, false);
                robotGoToXY(74 + 153, (ushort) (1150-deltafusee), sens.avancer);
              //  recalageX(180, 220, sens.avancer, 4, 1000);
              //  robotGoToXY(250, 1130, sens.reculer);
            }
            else
            {
                robotGoToXY(400, (ushort) (Y_TABLE - 1153 + deltafusee), sens.avancer, false);
                robotGoToXY(74 + 153, (ushort) (Y_TABLE - 1153 + deltafusee), sens.avancer);
              //  recalageX(180, Y_TABLE - 220, sens.avancer, 4, 1000);
              //  robotGoToXY(250, Y_TABLE - 1130, sens.reculer);
            }

            for (int i = 0; i < 4; i++) //Une solution pour raccourcir la stratégie si on arrive pas en 90s : ignorer le dernier cylindre 
            //for (int i = 0; i < 3; i++)
            {
                pince.Sortir(m_etatRobot.couleurEquipe, 550);
                pince.Entrer(m_etatRobot.couleurEquipe);

                if (i < 4) 
                {
                    if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                    {
                        robotGoToXY(74 + 153 + 50, (ushort)(1150 - deltafusee), sens.reculer, homologation);
                    }
                    else
                    {
                        robotGoToXY(74 + 153 + 50, (ushort)(Y_TABLE - 1153 + deltafusee), sens.reculer, homologation);
                    }
                }

                bras.SemiDescendre(m_etatRobot.couleurEquipe);
                pince.Deserrer(m_etatRobot.couleurEquipe);
                bras.Descendre(m_etatRobot.couleurEquipe);
                bras.Monter(m_etatRobot.couleurEquipe);
                if (i < 3)
                {
                    bras.Lacher(m_etatRobot.couleurEquipe);                    
                    if (m_etatRobot.couleurEquipe == Couleur.Bleu)
                    {
                        robotGoToXY(74 + 153, (ushort) (1150 - deltafusee), sens.avancer);
                    }
                    else
                    {
                            robotGoToXY(74 + 153, (ushort) (Y_TABLE - 1153 + deltafusee), sens.avancer);
                    }
                    BaseRoulante.m_posBR.alpha = 180;
                    BaseRoulante.m_posBR.x = 74 + 153;
                    
                }
            }
            if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                robotGoToXY(74 + 153 + 80, (ushort)(1150 - deltafusee), sens.reculer, homologation);
            }
            else
            {
                robotGoToXY(74 + 153 + 80, (ushort)(Y_TABLE - 1153 + deltafusee), sens.reculer, homologation);
            }
            Debug.Print("RecupererCylindreFusee");

            return true;
        }

        private bool RecupererCylindre2()
        {
            bool homologation = false;
              cylindresRecup++; //int défini dans GrandRobot.cs
              m_ihm.retourPhase(Couleurs.vert);
              if (m_etatRobot.couleurEquipe == Couleur.Bleu)
              {
                  robotGoToXY(350, (ushort) (1140 - deltafusee), sens.reculer, homologation);
                  robotGoToXY(1210 + 4, (ushort) (700 + 4 - deltafusee), sens.avancer, homologation);
                  // laisser 183/1.4 de marge en x et en y
                  // 183/1.4 = 130
                  robotGoToXY(1265, (ushort) (755 - deltafusee), sens.avancer, homologation);
                  pince.Sortir(m_etatRobot.couleurEquipe);
                  pince.Entrer(m_etatRobot.couleurEquipe);
                  BaseRoulante.m_kangaroo.allerEn(-30, 3, unite.mm);
                  robotRotate(-180);
                  BaseRoulante.m_kangaroo.allerEn(-90, 3, unite.mm);
                //  robotGoToXY(1200, 725, sens.reculer, homologation);
                //  robotGoToXY(1335, 858, sens.reculer, homologation);
              }
              else
              {
               /*   robotGoToXY(350, (ushort) (Y_TABLE - 1130 + deltafusee), sens.reculer, homologation);
                  robotGoToXY(1200, (ushort) (Y_TABLE - 710 + deltafusee), sens.avancer, homologation);
                  robotGoToXY(1270, (ushort) (Y_TABLE - 770 + deltafusee), sens.avancer, homologation);
                * */
                  robotGoToXY(350, (ushort)(Y_TABLE - 1130 + deltafusee), sens.reculer, homologation);
                  robotGoToXY(1200 - 10, (ushort)(Y_TABLE - 720 - 10 + deltafusee), sens.avancer, homologation);
                  robotGoToXY(1300 - 20, (ushort)(Y_TABLE - 835 -20 + deltafusee), sens.avancer, homologation);

                  pince.Sortir(m_etatRobot.couleurEquipe);
                  pince.Entrer(m_etatRobot.couleurEquipe);
                //  robotGoToXY(1200, Y_TABLE - 725, sens.reculer, homologation);
                  BaseRoulante.m_kangaroo.allerEn(-30, 3, unite.mm);
                  robotRotate(180);
                  // TROP VENERE A CORRIGER
                  BaseRoulante.m_kangaroo.allerEn(-80, 3, unite.mm);
              //    robotGoToXY(1360, Y_TABLE - 815, sens.reculer, homologation);
              }
              Debug.Print("RecupererCylindre2");
              return true;
        }

        private bool DeposerCylindres()
        {
            bool homologation = false;
           /* if (m_etatRobot.couleurEquipe == Couleur.Bleu)
            {
                robotGoToXY(1200, 725, sens.reculer, homologation);
                robotGoToXY(1355, 870, sens.reculer, homologation);
            }
            else
            {
                robotGoToXY(1200, Y_TABLE - 725, sens.reculer, homologation);
                robotGoToXY(1355, Y_TABLE - 870, sens.reculer, homologation);
            }*/
            m_ihm.retourPhase(Couleurs.jaune);
            // threservoir2 : sixième cylindre
            thReservoir2 = new Thread(() =>
            {
                bras.SemiDescendre(m_etatRobot.couleurEquipe);
                pince.Deserrer(m_etatRobot.couleurEquipe);
                bras.Descendre(m_etatRobot.couleurEquipe);

                bras.Monter(m_etatRobot.couleurEquipe);
                bras.Lacher(m_etatRobot.couleurEquipe);
              //  Thread.Sleep(300);
              //  bras.SemiDescendre(m_etatRobot.couleurEquipe);
              //  bras.Monter(m_etatRobot.couleurEquipe);
            });
            // deux coups pour assurer la sortie du troisième
            Thread.Sleep(500);
            reservoir.Sortir(m_etatRobot.couleurEquipe);
            reservoir.Sortir(m_etatRobot.couleurEquipe);


            BaseRoulante.m_kangaroo.allerEn(40, 10, unite.mm);
            Thread.Sleep(300);
            BaseRoulante.m_kangaroo.allerEn(-48, 10, unite.mm);
            Thread.Sleep(300);
            reservoir.Sortir(m_etatRobot.couleurEquipe);
            bras.Lacher(m_etatRobot.couleurEquipe);
            Thread.Sleep(300);
            thReservoir2.Start();

            // un coup violent pour assurer le troisième
            reservoir.Sortir(m_etatRobot.couleurEquipe);
            BaseRoulante.m_kangaroo.allerEn(70, 10, unite.mm);
            Thread.Sleep(1000);
            BaseRoulante.m_kangaroo.allerEn(-78, 10, unite.mm);
            Thread.Sleep(1000);


            // une boucle pour assurer les arrières
            for (int k = 5; k < 9; k++)
            {
                reservoir.Sortir(m_etatRobot.couleurEquipe);
                
                //BaseRoulante.m_kangaroo.allerEn(30, 10, unite.mm);
                // Thread.Sleep(300);
                //  BaseRoulante.m_kangaroo.allerEn(-30, 10, unite.mm);
                // Thread.Sleep(300);
            }
            BaseRoulante.m_kangaroo.allerEn(30, 10, unite.mm);


            /*
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 reservoir.Sortir(m_etatRobot.couleurEquipe);
                 Thread.Sleep(100);
                 BaseRoulante.m_kangaroo.allerEn(20, 3, unite.mm);

            */
            /*
                 thPousse = new Thread(() =>
                 {
                     reservoir.Sortir(m_etatRobot.couleurEquipe);
                 });

                 for (int i = 0; i < 10; i++)
                 {
                     thPousse.Start();
                     BaseRoulante.m_kangaroo.allerEn(25, 7, unite.mm);
                     Thread.Sleep(150);
                     BaseRoulante.m_kangaroo.allerEn(-25, 4, unite.mm);
                     Thread.Sleep(150);
                 }
             * */


          
            //thReservoir2.Start();

            // FUCK IT LE TOURNER RESERVOIR

            //reservoir.Tourner(m_etatRobot.couleurEquipe);

            //Thread.Sleep(300);

            // et deux sorties de plus au cas où
            //Thread.Sleep(500);
            //reservoir.Sortir(m_etatRobot.couleurEquipe);
            //Thread.Sleep(500);
            //reservoir.Sortir(m_etatRobot.couleurEquipe);

            Debug.Print("DeposerCylindre");
            m_ihm.retourPhase(Couleurs.orange);

            return true;
        }

        private void InitialiserStrategie()
        {
            Debug.Print("stratégie active");
            Strategie.Ajouter(new ActionRobot(Homologation, () => true, () => 101, executionUnique: true));
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
          AllerEn(1140, m_etatRobot.couleurEquipe==Couleur.Bleu ? 900 : 2100);

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

 

        /*private void 
 * ()
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
