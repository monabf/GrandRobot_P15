using System;
using Microsoft.SPOT;
using System.Threading;
using System.Collections;
using GR.Plateforme;
using GR.Membres;

namespace GR
{
    partial class GrandRobot
    {
        private bool SortirZoneDepart()
        {

#if HOMOLOGATION
            Tracage.Ecrire("Homologation");

            Tourner(12);
            AllerEn(341, Equipe == Couleur.Bleu ? 946 : 2054, Plateforme.sens.avancer);
            Tracage.Ecrire("Recuperation du cylindre");
            Pince.ouvrir(Equipe); //À CODER : doit rentrer la pince et ouvrir en intermédiaire pour que le bras vienne récupérer le cylindre (le début du robot est à 5.8cm du cylindre)
            Tourner(Equipe==Couleur.Bleu ? 78 : -78);
            Pince.fermer(Equipe); //À CODER : doit sortir et fermer
            Pince.ouvrir(Equipe);
            Bras.attraper(Equipe); //À CODER : doit descendre, attraper le cylindre laissé par la pince, remonter
            SortieOK = AllerEn(205, Equipe == Couleur.Bleu ? 870 : 2120, Plateforme.sens.avancer) == etat.arrive;
            Bras.reposer(Equipe); //À CODER : doit redescendre et lâcher le cylindre qu'il tient
            //il faudra voir expérimentalement si les emplacements pour redéposer le cylindre conviennent, sinon les bouger un peu

//Homologation : récupérer le 1er cylindre et retourner dans la zone bleue pour l'y reposer, sans le mettre dans le réservoir
//entre les deux (le bras le garde simplement), s'arrêter si le capteur ultrasons réagit (l'arrêt est codé dans la classe capteur ultrasons)

#else
            Tracage.Ecrire("Sortie de la zone de depart");

            Tourner(12);
            SortieOK = AllerEn(341, Equipe == Couleur.Bleu ? 946 : 2054, Plateforme.sens.avancer) == etat.arrive;
            //on prend la convention Couleur.Bleu == zone de départ bleue, sinon zone de départ jaune ; en cm pour l'instant, à voir ! Attention aussi aux conventions de repère, ici on a pris y vers le bas et angle positif en sens horaire mais pas forcément vrai
            //rappeler à PED de coder etat car permet savoir si arrive/perdu... et adapter le nom en fonction de ce qu'il choisit

            Tracage.Ecrire(SortieOK ? "Sortie reussie" : "Sortie echouee");
            //si SortieOk == etat.arrive alors on écrit Sortie reussie, sinon on écrit Sortie echouee

#endif

            return SortieOK;
        }

      /*  private bool PecherPoissons()
        {
            int posX = Table.Aquarium.Position.X - (Equipe == Couleur.Bleu ? 200 : 225);

            TentativesPeche++;

            Tracage.Ecrire("Deplacement vers l'aquarium");
            if (AllerEn(Table.Aquarium.Position.X - 250, Table.Aquarium.Position.Y,
                Plateforme.sens.avancer) != etat.arrive) return false;
            AllerEn(posX, Table.Aquarium.Position.Y + (Equipe == Couleur.Bleu ? 300 : -400),
                Equipe == Couleur.Bleu ? Plateforme.sens.reculer : Plateforme.sens.avancer);
            Tourner(-90 - Position.alpha);

            Tracage.Ecrire("Peche des poissons");
            CanneAPeche.baisser();
            AllerEn(posX, Table.Aquarium.Position.Y + (Equipe == Couleur.Bleu ? 100 : -200),
                Equipe == Couleur.Bleu ? Plateforme.sens.avancer : Plateforme.sens.reculer);
            AllerEn(posX, Table.Aquarium.Position.Y + (Equipe == Couleur.Bleu ? 300 : -450),
                Equipe == Couleur.Bleu ? Plateforme.sens.reculer : Plateforme.sens.avancer);
            CanneAPeche.ranger();

            Tracage.Ecrire("Depot des poissons");
            AllerEn(posX, Table.Filet.Position.Y + (Equipe == Couleur.Bleu ? 200 : -300),
                Equipe == Couleur.Bleu ? Plateforme.sens.reculer : Plateforme.sens.avancer);

            CanneAPeche.lacherPoisson();
            Thread.Sleep(1000);
            CanneAPeche.ranger();

            AllerEn(Table.Aquarium.Position.X - 285, Table.Aquarium.Position.Y + (Equipe == Couleur.Bleu ? -50 : 50),
                Equipe == Couleur.Bleu ? Plateforme.sens.avancer : Plateforme.sens.reculer);

            return true;
        }*/

        private bool RecupererCylindre1()
        {
          CylindresRecup++; //int à créer pour en faire un indice

          Tracage.Ecrire("Recuperation du 1er cylindre");
          Pince.ouvrir(Equipe); //À CODER : doit rentrer la pince et ouvrir en intermédiaire pour que le bras vienne récupérer le cylindre (le début du robot est à 5.8cm du cylindre)
          Tourner(Equipe==Couleur.Bleu ? 78 : -78);
          Pince.fermer(Equipe); //À CODER : doit sortir et fermer
          Pince.ouvrir(Equipe);
          Bras.attraper(Equipe); //À CODER : doit descendre, attraper le cylindre laissé par la pince, remonter
          Bras.lacher(Equipe); //À CODER : doit lâcher le cylindre attrapé dans le réservoir
          Reservoir.tourner(Equipe); //À CODER : doit utiliser le capteur de couleur et la petite roue pour mettre le cylindre dans le bon sens

          return true;
        }

        private bool RecupererCylindresFusee()
        {
          CylindresRecup+=4; //int à créer pour en faire un indice

          Tracage.Ecrire("Positionnement devant la fusee et recuperation des 4 cylindres");

          AllerEn(341,Equipe==Couleur.Bleu ? 1150 : 1850, Plateforme.sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? 90 : -90);
          AllerEn(310, Equipe==Couleur.Bleu ? 1150 : 1850, Plateforme.sens.avancer);
          Pince.ouvrir(Equipe);
          Pince.fermer(Equipe);
          Pince.ouvrir(Equipe);
          Bras.attraper(Equipe);
          Bras.lacher(Equipe);

          Pince.ouvrir(Equipe);
          Pince.fermer(Equipe);
          Pince.ouvrir(Equipe);
          Bras.attraper(Equipe);
          Bras.lacher(Equipe);

          Pince.ouvrir(Equipe);
          Pince.fermer(Equipe);
          Pince.ouvrir(Equipe);
          Bras.attraper(Equipe);
          Bras.lacher(Equipe);

          Pince.ouvrir(Equipe);
          Pince.fermer(Equipe);
          Pince.ouvrir(Equipe);
          Bras.attraper(Equipe); //On garde le 5eme cylindre dans le grand bras jusqu'à avoir fait de la place dans le réservoir !

          return true;
        }

        private bool RecupererCylindre2()
        {
          CylindresRecup++; //int à créer pour en faire un indice

          Tracage.Ecrire("Recuperation du 2eme cylindre");

          AllerEn(850,1150, Equipe==Couleur.Bleu ? Plateforme.sens.avancer : Plateforme.sens.reculer);
          Tourner(180);
          Tourner(Equipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1100, Equipe==Couleur.Bleu ? 900 : 2100, Plateforme.sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? 45 : -45);
          AllerEn(1136, Equipe==Couleur.Bleu ? 900 : 2100);

          Pince.ouvrir(Equipe);
          Pince.fermer(Equipe); //On garde le 6eme cylindre dans le petit bras jusqu'à avoir fait de la place dans le réservoir !

          return true;
        }

        private bool DeposerCylindres()
        {
          CylindresRecup++; //int à créer pour en faire un indice

          Tracage.Ecrire("Depot des cylindres");

          AllerEn(1125, Equipe==Couleur.Equipe ? 900 : 2100, Plateforme.sens.reculer);
          Tourner(Equipe==Couleur.Bleu ? -45 : 45);
          AllerEn(1275, Equipe==Couleur.Bleu ? 750 : 2250, Plateforme.sens.avancer);
          Tourner(Equipe==Couleur.Bleu ? -90 : 90);
          AllerEn(1427, Equipe==Couleur.Bleu ? 927 : 2073, Plateforme.sens.reculer);
          Reservoir.sortir(Equipe); //À CODER : doit utiliser le petit poussoir pour faire sortir les cylindres du réservoir
          Reservoir.sortir(Equipe);
          Reservoir.sortir(Equipe);
          Reservoir.sortir(Equipe);

          Bras.lacher(Equipe); //on laisse tomber l'avant-dernier cylindre dans le réservoir et on le sort
          Reservoir.sortir(Equipe);

          Pince.ouvrir(Equipe); //on récupère le dernier cylindre avec le grand bras, puis on le lâche dans le réservoir, on le tourne avec la bonne oculeur dessus et on le sort
          Bras.attraper(Equipe);
          Bras.lacher(Equipe);
          Reservoir.tourner(Equipe);
          Reservoir.sortir(Equipe);

          return true;
        }

  private bool DeployerParasol()
        {
            Tracage.Ecrire("Attente de la fin du temps imparti");
            while ((DateTime.Now - InstantDebut).Ticks < new TimeSpan(0, 1, 30).Ticks)
                Thread.Sleep(1);

            Tracage.Ecrire("Deploiement du parasol");
            Parasol.deployer();

            return true;
        }

        private void InitialiserStrategie()
        {

            /*  Strategie.Ajouter(new ActionRobot(() =>
              {
                  Tracage.Ecrire("getPosition");
                  BaseRoulante.getPosition(ref Position);
                  Tracage.Ecrire("TOurner");
               //   AllerEn(Position.x, Position.y + (Equipe == Couleur.Bleu ? 1000 : -1000), BR.sens.avancer);
                  int tempo = 0;
                  BaseRoulante.tourner(90, vitesse.vitesseRotationMax, ref tempo);
                  Tracage.Ecrire("ok");


                  return true;
              }, executionUnique: true));
              return;*/

            Strategie.Ajouter(new ActionRobot(SortirZoneDepart, executionUnique: true));
        #if HOMOLOGATION
              return;
        #endif
            Strategie.Ajouter(new ActionRobot(PecherPoissons,
            () => SortieOK && (DateTime.Now - InstantDebut).Ticks < new TimeSpan(0, 1, 30).Ticks,
            () => 100 - TentativesPeche * 2, true));
            Strategie.Ajouter(new ActionRobot(DeplacerChateaux,
            () => SortieOK && (DateTime.Now - InstantDebut).Ticks < new TimeSpan(0, 1, 30).Ticks,
            () => 99 - TentativesChateaux * 2, true));
            /*Strategie.Ajouter(new ActionRobot(DeployerParasol,
            () => Strategie.NombreAction == 1 || (DateTime.Now - InstantDebut).Ticks >= new TimeSpan(0, 1, 30).Ticks,
            executionUnique: true));*/ // déplacé
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
