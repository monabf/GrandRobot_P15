using Gadgeteer.Modules.GHIElectronics;
using System;
using System.Collections;
using System.Threading;
using GR.BR;
using GR.Membres;
using GR.Vision;
using GHI.Processor;
using GHI.Pins;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace GR
{
    /// <summary>
    /// Classe représentant le grand robot
    /// </summary>
    partial class GrandRobot
    {
        public enum Axe { Null = 0, X, Y }

        public  GestionnaireStrategie Strategie;
       // public  IHMTracage Tracage;

        public  Couleur Equipe;
        public  ConfigurationPorts Ports;
        public  Jack JackDemarrage;
        public  CBaseRoulante BaseRoulante;
        public  ControleurAX12 controleurAX12;
        public  CPince pince;
        public  CReservoir reservoir;
        public  CBras bras;
        public  CCapteurCouleur CapteurCouleur;
       // public  OutputPort m_direction;
        public  GroupeInfrarouge IR;
        public  CCapteurUltrason CapteurUltrason;
       
        public positionBaseRoulante Position = new positionBaseRoulante();
        public bool SortieOK = false;
        public static bool obstacle = false;

        public DateTime InstantDebut;
        public int cylindresRecup;
        /// <summary>
        /// Initialise le robot
        /// </summary>
        /// <param name="ports">Configuration des ports</param>
        /// <param name="equipe">Couleur de l'équipe</param>
        /// <param name="table">Configuration de la table</param>
        public GrandRobot(ConfigurationPorts ports, Couleur equipe)
        {
            Ports = ports;
            Equipe = equipe;
            Debug.Print("grand robot ligne 59");
            Strategie = new GestionnaireStrategie();
            Debug.Print("Gestionnaire actif");
           // Tracage = new IHMTracage();
            Debug.Print(Ports.bras.idAx12BrasModule + "");

            JackDemarrage = new Jack(Ports.IO, Ports.Jack);
            Debug.Print("Jack actif");
            Debug.Print(Ports.Plateforme+"");

            controleurAX12 = new ControleurAX12(Ports.contAX12);
            Debug.Print("Controleur actif");
            pince = new CPince(controleurAX12, Ports.pince);
            funnyBras = new FunnyBras(controleurAX12, Ports.funnyBras);
            Debug.Print("Pince actif");
            bras = new CBras(controleurAX12, Ports.bras);
            Debug.Print("Bras actif");
            BaseRoulante = new CBaseRoulante(Ports.Plateforme);
            Debug.Print("Base roulante actif");
            cylindresRecup = 0;

            //Ports.ConfigCanne.direction = m_direction;
           // m_direction = new OutputPort((Cpu.Pin)EMX.IO46, false);  //IO26 si 11
            /* m_RS485 = new RS485(9);
              m_RS485.Configure(500000, GT.SocketInterfaces.SerialParity.None, GT.SocketInterfaces.SerialStopBits.One, 8, GT.SocketInterfaces.HardwareFlowControl.NotRequired);
              m_RS485.Port.Open();*/


            IR = new GroupeInfrarouge(Ports.IO, Ports.InfrarougeAVD, Ports.InfrarougeAVG, Ports.InfrarougeARD, ports.InfrarougeARG);
            //TelemetreLaser = new CTelemetreLaser(Ports.TelemetreLaser, 9600);
//            CapteurUltrason = new CCapteurUltrason(Ports.CapteurUltrason);
            Debug.Print("infrarouge actif");

            BaseRoulante.setCouleur(equipe);
            BaseRoulante.getPosition(ref Position);

           // Tracage.Afficher();
            //Tracage.Ecrire("Equipe " + (equipe == Couleur.Vert ? "verte" : "violette"));

        }

        /// <summary>
        /// Attend le Jack
        /// </summary>
        public void AttendreJack()
        {
         //   Tracage.Ecrire("Veuillez retirer le jack pour demarrer.");

            while (!JackDemarrage.Etat) Thread.Sleep(1);
        }

        /// <summary>
        /// Démarre asynchronement le robot
        /// </summary>
        /// <param name="tempsImparti">Temps en secondes au bout du quel l'arrêt du robot est forcé</param>
        public void Demarrer(double tempsImparti)
        {


            Timer timeout;
            DateTime fin = new DateTime();
            var thDecompte = new Thread(() =>
            {
                while (Strategie.ExecutionPossible() && DateTime.Now < fin)
                {
              //      Tracage.Ecrire("Temps restant: " + (fin - DateTime.Now).ToString().Substring(3, 5) + ".");
                    Thread.Sleep(10000);
                }
            });
            InitialiserStrategie();

            var thStrat = new Thread(() => EffectuerStrategie());

            InstantDebut = DateTime.Now;
            fin = InstantDebut.AddSeconds(tempsImparti);


            thDecompte.Start();
            thStrat.Start();

            timeout = new Timer(state =>
            {
            //    Tracage.Ecrire("Fin du temps imparti.");
                if (thStrat.IsAlive) thStrat.Abort();
                BaseRoulante.stop();
                funnyBras.lancer();

            }, null, (int)(tempsImparti * 1000), -1);
        }

        public void EffectuerStrategie()
        {
         //   Tracage.Ecrire("Debut de l'execution de la strategie.");
            Debug.Print("Debut de l'execution de la strategie.");
            Debug.Print(Strategie.NombreAction+" nombre d'actions");
            while (Strategie.ExecutionPossible())
            {
                Debug.Print("execution possible");
           //     Tracage.Ecrire("Execution de l'action suivante.");
                Strategie.ExecuterSuivante();
            }

          //  Tracage.Ecrire("Fin de l'execution de la strategie.");
          //  Tracage.Ecrire("Nombre de cylindres " + cylindresRecup);
        }
        etatBR robotGoToXY(ushort x, ushort y, sens s, bool boolDetection = false, int speed = 10)
        {
            etatBR retour;
            if (boolDetection)
            {
                // on passe le sens "dir" au timer via la variable "state"
                // analogue au timeout-callback pour les amoureux du js
                //Timer t = new Timer(new TimerCallback(Detecter), s, 0, 1000);
                obstacle = false; // paramètre pour savoir si il y'a bien un obstacle
                var thDetection = new Thread(() =>
                {
                    while (true)
                    {
                        Detecter(s);
                        //Thread.Sleep(20);
                    }
                });
                thDetection.Start();
                retour = BaseRoulante.allerDect(y, x, s, speed);// x,y,s
                thDetection.Suspend();
                obstacle = false;
                //t.Dispose();
            }
            else
            {
                retour = BaseRoulante.allerEn(y, x, s, speed);
            }
            return retour;
        }


        public void Detecter(object o)
        {
            sens dir = (sens)o;
            // si on avance, les ultrasons sont utiles

            if (dir == sens.avancer)
            {
                // on teste les capteurs IR avants puis le capteur laser en appui
                double distance = 0d;
                bool obstacleUS = false;
                // mesure une distance moyenne avec 5 mesures rapides
                //Ultrason désactivé pour l'instant, ils prennent beaucoup trop de temps pour acquérir l'information.
                /*
                m_ultrason.getDistance(1, ref distance);
                if (distance < 30 && distance != -1)
                    obstacleUS = true;
                */

                if ((!IR.AVG.Read() || !IR.AVD.Read()))// infrarouge OK.. et c'est une condition et && obstacleUS
                {
                    //m_baseRoulante.stop();
                    obstacle = true;


                }
                else obstacle = false;

            }
            // si on recule, les ultrasons ne sont plus utiles
            else
            {
                // on teste les capteurs IR arrières
                if (!IR.ARG.Read() || !IR.ARD.Read())
                {
                    //Debug.Print("Détection obstacle après");
                    //m_baseRoulante.stop();
                    obstacle = true;


                }
                else obstacle = false;

            }

        }

        etatBR robotRotate(int alpha)
        {
            etatBR retour;
            retour = BaseRoulante.tourner(alpha);
            return retour;
        }

        public bool DetecterObstacle(BR.sens s, int distanceMax = 30, int nbMesuresUS = 5, int angleLaser = 60)
        {
            var obstacle = false;

            if (s == BR.sens.avancer)
            {
                double distance = 0d;

                obstacle = (!IR.AVG.Read() || !IR.AVD.Read()) ||
                    (CapteurUltrason.getDistance(nbMesuresUS, ref distance) &&
                    distance >= 0 && distance <= distanceMax);
            }
            else if (s == BR.sens.reculer)
                obstacle = !IR.ARG.Read() || !IR.ARD.Read();

            return obstacle;
        }

        /**
               * il faut recoder cette méthode
               * */
        public void Recaler(Axe axe = Axe.Null)
        {
              
            /*
            var posBR = new positionBaseRoulante();
            etatBR retour;

            BaseRoulante.getPosition(ref posBR);
            switch (axe)
            {
                case Axe.X:
                    Tracage.Ecrire("Recalage sur l'axe X.");
                    retour = AllerEn(posBR.x > Table.Taille.X / 2 ? Table.Taille.X : 0, posBR.y, BR.sens.avancer);
                    break;
                case Axe.Y:
                    Tracage.Ecrire("Recalage sur l'axe Y.");
                    retour = AllerEn(posBR.x, posBR.y > Table.Taille.Y / 2 ? Table.Taille.Y : 0, BR.sens.avancer);
                    break;
                default:
                    var dx = posBR.x > Table.Taille.X / 2 ? Table.Taille.X - posBR.x : posBR.x;
                    var dy = posBR.y > Table.Taille.Y / 2 ? Table.Taille.Y - posBR.y : posBR.y;

                    if (dx < dy) goto case Axe.X;
                    else goto case Axe.Y;
            }

            Tracage.Ecrire(retour == etatBR.arrive ? "Succes" : "Echec");
        */}
    }
}
