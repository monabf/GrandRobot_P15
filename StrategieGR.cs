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
        private bool SortirZoneDepart()
        {
//IL FAUT UN CONSTRUCTEUR QUELQUE PART QUI CRÉE TOUTES LES INSTANCES des classes CPince etc
#if HOMOLOGATION
            Tracage.Ecrire("Homologation");

            Tourner(12);
            AllerEn(341, Equipe == Couleur.Bleu ? 946 : 2054, sens.avancer);
            Tracage.Ecrire("Recuperation du cylindre");
            pince.ouvrir(Equipe); //À CODER : doit rentrer la pince et ouvrir en intermédiaire pour que le bras vienne récupérer le cylindre (le début du robot est à 5.8cm du cylindre)
            Tourner(Equipe==Couleur.Bleu ? 78 : -78);
            pince.fermer(Equipe); //À CODER : doit sortir et fermer
            pince.ouvrir(Equipe);
            bras.attraper(Equipe); //À CODER : doit descendre, attraper le cylindre laissé par la pince, remonter
            bool SortieOK = AllerEn(205, Equipe == Couleur.Bleu ? 870 : 2120, sens.avancer) == etat.arrive;
            bras.reposer(Equipe); //À CODER : doit redescendre et lâcher le cylindre qu'il tient
            //il faudra voir expérimentalement si les emplacements pour redéposer le cylindre conviennent, sinon les bouger un peu

//Homologation : récupérer le 1er cylindre et retourner dans la zone bleue pour l'y reposer, sans le mettre dans le réservoir
//entre les deux (le bras le garde simplement), s'arrêter si le capteur ultrasons réagit (l'arrêt est codé dans la classe capteur ultrasons)

#else
     //       Tracage.Ecrire("Sortie de la zone de depart");
        //    robotGoToXY((ushort)1200, (ushort)920, sens.avancer);
            for (int i = 0; i < 4; i++)
            {
                pince.sortir(Equipe);
                pince.entrer(Equipe);
                bras.semidescendre(Equipe);
                pince.deserrer(Equipe);
                bras.descendre(Equipe);
                bras.Monter(Equipe);
                bras.lacher(Equipe);
            }
            //robotGoToXY(400, 974, sens.avancer);
           // bool SortieOK = robotGoToXY((ushort) 341, (ushort) (Equipe == Couleur.Bleu ? 946 : 2054), sens.avancer) == etatBR.arrive;
            //on prend la convention Couleur.Bleu == zone de départ bleue, sinon zone de départ jaune ; en cm pour l'instant, à voir ! Attention aussi aux conventions de repère, ici on a pris y vers le bas et angle positif en sens horaire mais pas forcément vrai
            //rappeler à PED de coder etat car permet savoir si arrive/perdu... et adapter le nom en fonction de ce qu'il choisit
            pince.entrer(Equipe);
            Debug.Print("Sortie");
            //si SortieOk == etat.arrive alors on écrit Sortie reussie, sinon on écrit Sortie echouee

#endif

            return SortieOK;
            //return true;
        }



        private bool RecupererCylindre1()
        {
          cylindresRecup++;
          Debug.Print("RecupererCylindre1");

   //       Tracage.Ecrire("Recuperation du 1er cylindre");
        //  pince.ouvrir(Equipe);
          //Thread.Sleep(10000);

          //BaseRoulante.m_kangaroo.allerEn(1000,1,unite.mm);
         // Thread.Sleep(100000);
          robotRotate(Equipe==Couleur.Bleu ? 78 : -78);
         // pince.fermer(Equipe);
         // bras.attraper(Equipe);
         // bras.lacher(Equipe);
          //reservoir.tourner(Equipe);

          return true;
        }

        private bool RecupererCylindresFusee()
        {
          cylindresRecup+=4; //int défini dans GrandRobot.cs

   //       Tracage.Ecrire("Positionnement devant la fusee et recuperation des 4 cylindres");

          robotGoToXY((ushort) 341, (ushort) (Equipe==Couleur.Bleu ? 1150 : 1850), sens.avancer);
          Thread.Sleep(50);
          robotRotate(Equipe == Couleur.Bleu ? 90 : -90);
          Thread.Sleep(50);
          robotGoToXY((ushort)310, (ushort)(Equipe == Couleur.Bleu ? 1150 : 1850), sens.avancer);
         
        
           Debug.Print("RecupererCylindreFusee");

          return true;
        }

        private bool RecupererCylindre2()
        {
          cylindresRecup++; //int défini dans GrandRobot.cs

   //       Tracage.Ecrire("Recuperation du 2eme cylindre");

          robotGoToXY(850,1150, Equipe==Couleur.Bleu ? sens.avancer : sens.reculer);
          Thread.Sleep(50);
          robotRotate(180);
          Thread.Sleep(50);
          robotRotate(Equipe == Couleur.Bleu ? -45 : 45);
          robotGoToXY((ushort) 1100, (ushort) (Equipe == Couleur.Bleu ? 900 : 2100), sens.avancer);
          Thread.Sleep(50);
          robotRotate(Equipe==Couleur.Bleu ? 45 : -45);
          Thread.Sleep(50);
          robotGoToXY((ushort) 1136, (ushort)(Equipe == Couleur.Bleu ? 900 : 2100), sens.avancer);

          Debug.Print("RecupererCylindre2");

          return true;
        }

        private bool DeposerCylindres()
        {

   //       Tracage.Ecrire("Depot des cylindres");

          robotGoToXY((ushort)1125, (ushort) (Equipe == Couleur.Bleu ? 900 : 2100), sens.reculer);
          Thread.Sleep(50);
          robotRotate(Equipe==Couleur.Bleu ? -45 : 45);
          robotGoToXY((ushort)1275, (ushort) (Equipe == Couleur.Bleu ? 750 : 2250), sens.avancer);
          Thread.Sleep(50);
          robotRotate(Equipe==Couleur.Bleu ? -90 : 90);
          robotGoToXY((ushort)1427, (ushort) (Equipe == Couleur.Bleu ? 927 : 2073), sens.reculer);
          Thread.Sleep(50);
      //    reservoir.sortir(Equipe); //À CODER : doit utiliser le petit poussoir pour faire sortir les cylindres du réservoir
      //    reservoir.sortir(Equipe);
      //    reservoir.sortir(Equipe);
      //    reservoir.sortir(Equipe);

          bras.lacher(Equipe); //on laisse tomber l'avant-dernier cylindre dans le réservoir et on le sort
        //  reservoir.sortir(Equipe);

          bras.lacher(Equipe);
        //  reservoir.tourner(Equipe);
       //   reservoir.sortir(Equipe);
          Debug.Print("DeposerCylindre");

          return true;
        }

  /*private bool DeployerParachute()  NE PAS OUBLIER LA FUNNY ACTION
        {
            Tracage.Ecrire("Attente de la fin du temps imparti");
            while ((DateTime.Now - InstantDebut).Ticks < new TimeSpan(0, 1, 30).Ticks)
                Thread.Sleep(1);

            Tracage.Ecrire("Deploiement du parasol");
            Parasol.deployer();

            return true;
        }*/

        private void InitialiserStrategie()
        {
            Debug.Print("stratégie active");

            Strategie.Ajouter(new ActionRobot(SortirZoneDepart, ()=>true, ()=> 100, executionUnique: true));
            /*Permet de gérer l'enchaînement des actions : on ajoute une par une les actions qu'on veut exécuter, sous la forme
            new ActionRobot(nom, condition, priorité, exécution unique (tous ces param sauf le nom sont optionnels)). La condition
            est soit une fonction booléenne du type de celles au-dessus  (la suivante n'est exécutée que si la précédente a été appelée
            avant), soit une condition temporelle du type ci-dessous dans le code P14*/

        #if HOMOLOGATION
              return;
        #endif
            Strategie.Ajouter(new ActionRobot(RecupererCylindre1, ()=>true, () => 100 - cylindresRecup, true));
            Strategie.Ajouter(new ActionRobot(RecupererCylindresFusee, () => true, () => 100 - cylindresRecup, true));
            Strategie.Ajouter(new ActionRobot(RecupererCylindre2, () => true, () => 100 - cylindresRecup, true));
            Strategie.Ajouter(new ActionRobot(DeposerCylindres, () => true, () => 100 - cylindresRecup - 1, true));


        }












        //CODE POUR FAIRE UNE DÉMO!!!!!!!!!!! (pas d'action des servos moteurs, seulement des mouvements)
/*
        private bool SortirZoneDepart()
        {
          Tracage.Ecrire("Sortie de la zone de depart");

          Tourner(12);
          bool SortieOK = AllerEn(341, Equipe == Couleur.Bleu ? 946 : 2054, sens.avancer) == etat.arrive;
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
          Tourner(Equipe==Couleur.Bleu ? 78 : -78);
          return true;
        }

        private bool RecupererCylindresFusee()
        {
          cylindresRecup+=4; //int à créer pour en faire un indice

          Tracage.Ecrire("Positionnement devant la fusee et recuperation des 4 cylindres");

          AllerEn(341,Equipe==Couleur.Bleu ? 1150 : 1850, sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? 90 : -90);
          AllerEn(310, Equipe==Couleur.Bleu ? 1150 : 1850, sens.avancer);

          return true;
        }

        private bool RecupererCylindre2()
        {
          cylindresRecup++; //int à créer pour en faire un indice

          Tracage.Ecrire("Recuperation du 2eme cylindre");

          AllerEn(850,1150, Equipe==Couleur.Bleu ? sens.avancer : sens.reculer);
          Tourner(180);
          Tourner(Equipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1100, Equipe==Couleur.Bleu ? 900 : 2100, sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? 45 : -45);
          AllerEn(1136, Equipe==Couleur.Bleu ? 900 : 2100);

          return true;
        }

        private bool DeposerCylindres()
        {


          Tracage.Ecrire("Depot des cylindres");

          AllerEn(1125, Equipe==Couleur.Equipe ? 900 : 2100, sens.reculer);
          Tourner(Equipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1275, Equipe==Couleur.Bleu ? 750 : 2250, sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? -90 : 90);
          AllerEn(1427, Equipe==Couleur.Bleu ? 927 : 2073, sens.reculer);


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
