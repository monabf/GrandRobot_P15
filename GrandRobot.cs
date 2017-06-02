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
    #region structures

    struct EtatRobot
    {
        public Couleur couleurEquipe;
        public int disposition;
        public int coefPos;
        public int coefAngle;
        public bool stop;
        public sens s;
    }

    #endregion
    

    partial class GrandRobot
    {
       const int Y_TABLE = 3000;
       const int X_TABLE = 2000;
       public enum Axe { Null = 0, X, Y }

        public  GestionnaireStrategie Strategie;

        EtatRobot m_etatRobot;

        public IHM m_ihm;
        public  Jack JackDemarrage;
        public  CBaseRoulante BaseRoulante;
        public  ControleurAX12 controleurAX12;
        public  CPince pince;
        public  CFunnyBras funnyBras;
        public  CReservoir reservoir;
        public  CBras bras;
        public  GroupeInfrarouge IR;
    //    public  CCapteurUltrason CapteurUltrason;
       
        public positionBaseRoulante Position = new positionBaseRoulante();
        public static bool obstacle = false;

        public DateTime InstantDebut;
        public int cylindresRecup = 0;

        public int deltafusee = 10;


        public GrandRobot(ConfigurationPorts ports, Couleur equipe, int disposition, IHM ihm)
        {
            m_ihm = ihm;
            m_etatRobot = new EtatRobot();
            m_etatRobot.couleurEquipe = equipe;
            m_etatRobot.disposition = disposition;
            Strategie = new GestionnaireStrategie();

            JackDemarrage = new Jack(ports.IO, ports.Jack);
            Debug.Print("Ceration de la plateforme "+ports.Plateforme);
            BaseRoulante = new CBaseRoulante(ports.Plateforme);
            Debug.Print("plateforme créée");
            BaseRoulante.setCouleur(m_etatRobot.couleurEquipe);
            BaseRoulante.getPosition(ref Position);

            controleurAX12 = new ControleurAX12(ports.contAX12);
            Debug.Print("Controleur actif");
            pince = new CPince(controleurAX12, ports.pince);
            //funnyBras = new CFunnyBras(controleurAX12, ports.funnyBras);
            Debug.Print("funny bras actif");
            bras = new CBras(controleurAX12, ports.bras);
            reservoir = new CReservoir(m_etatRobot.couleurEquipe, controleurAX12, ports.reservoir);
            Debug.Print("reservoir actif");

            // à la date d'aujourd'hui, veille de la coupe, l'infrarouge avant gauche ne marche pas
            IR = new GroupeInfrarouge(ports.IO, ports.InfrarougeAVD, ports.InfrarougeAVG, ports.InfrarougeARD, ports.InfrarougeARG);
            Debug.Print("infrarouge actif");
            // NB : il n'y a pas de capteur ultrason sur notre grand robot
        }
        void recalageX(int angle, int x, sens s, int speed, int temps)
        {
            BaseRoulante.recalagePosX(angle, x, speed, s, temps);
        }

        void recalageY(int angle, int y, sens s, int speed, int temps)
        {
            BaseRoulante.recalagePosY(angle, y, speed, s, temps);
        }

        /// <summary>
        /// Attend le Jack
        /// </summary>
        public void AttendreJack()
        {
            while (!JackDemarrage.Etat) Thread.Sleep(1);
            Debug.Print(JackDemarrage.Etat+" plop jzck");
            m_ihm.retourPhase(Couleurs.vert);

        }

        /// <param name="tempsImparti">Temps en secondes au bout du quel l'arrêt du robot est forcé</param>
        public void Demarrer(double tempsImparti)
        {
            
            Timer timeout;
            DateTime fin = new DateTime();
            /*
            var thDecompte = new Thread(() =>
            {
                while (Strategie.ExecutionPossible() && DateTime.Now < fin)
                {
              //      Tracage.Ecrire("Temps restant: " + (fin - DateTime.Now).ToString().Substring(3, 5) + ".");
                    Thread.Sleep(10000);
                }
            });
           */
            InitialiserStrategie();

            Thread thStrat = new Thread(() => EffectuerStrategie());


            InstantDebut = DateTime.Now;
            fin = InstantDebut.AddSeconds(tempsImparti);


//            thDecompte.Start();
            thStrat.Start();
         //   if (thStrat.IsAlive) m_ihm.retourPhase(Couleurs.bleu); 

            timeout = new Timer(state =>
            {
                m_ihm.retourPhase(Couleurs.orange);

            //    Tracage.Ecrire("Fin du temps imparti.");

                /*if (thRanger.IsAlive)
                {
                    Debug.Print("plop ran");
                    thRanger.Suspend();
                }

                if (thReservoir.IsAlive)
                {
                    Debug.Print("plop res 1");
                    thReservoir.Suspend();
                }
                 * */
               

                

               // reservoir.arretUrgenceRoulette();
/*
                Thread thFunny = new Thread(() =>
                {
                    Thread.Sleep(3000);
                    funnyBras.lancer();
                });

                thFunny.Start();
 */

                funnyBras = new CFunnyBras(controleurAX12, 7);
                Thread.Sleep(2000);
                funnyBras.lancer();

                m_ihm.retourPhase(Couleurs.rouge);

                BaseRoulante.stop();
                m_ihm.retourPhase(Couleurs.vert);

                pince.killPince();
                bras.killBras();
                reservoir.killReservoir();

                if (thStrat.IsAlive)
                {
                    Debug.Print("kill main thread");
                    thStrat.Abort();
                }
                

                m_ihm.retourPhase(Couleurs.bleu);


                //reservoir.m_rouletteIntelligente.m_ax12Roulette.setMode(AX12Mode.joint);
                //reservoir.m_rouletteIntelligente.m_ax12Roulette.
                    /*  var thBase = new Thread(() =>
                {
                });
                thBase.Start();
                var thReservoir = new Thread(() =>
                {
                });
                thReservoir.Start();
                */

            }, null, (int)(tempsImparti * 1000), -1);
        }

        public void EffectuerStrategie()
        {
         //   Tracage.Ecrire("Debut de l'execution de la strategie.");
          //  Debug.Print("Debut de l'execution de la strategie.");
        //    Debug.Print(Strategie.NombreAction+" nombre d'actions");
            while (Strategie.ExecutionPossible())
            {
            //    Debug.Print("execution possible");
           //     Tracage.Ecrire("Execution de l'action suivante.");
                Strategie.ExecuterSuivante();
            }

        }

        public Couleur robotGetCouleur()
        {
            return m_etatRobot.couleurEquipe;
        }

        public int robotGetDisposition()
        {
            return m_etatRobot.disposition;
        }  

        etatBR robotGoToXY(ushort x, ushort y, sens s, bool boolDetection = false, int speed = 2)
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

                retour = BaseRoulante.allerDect(x, y, s, speed);// x,y,s
                thDetection.Suspend();
                obstacle = false;
                //t.Dispose();
            }
            else
            {
                retour = BaseRoulante.allerEn(x, y, s, speed);
            }
            Debug.Print("pos_visee " + x + " " + y);

            return retour;
        }


        public void Detecter(object o)
        {
            sens dir = (sens)o;
            // si on avance, les ultrasons sont utiles

            if (dir == sens.avancer)
            {
                if ((!IR.AVG.Read() || !IR.AVD.Read()))// infrarouge OK.. et c'est une condition et && obstacleUS
                {
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
                    obstacle = true;
                }
                else obstacle = false;

            }

        }

        public etatBR robotRotate(int alpha)
        {
            etatBR retour;
            Debug.Print("avant rotate " + alpha);
            retour = BaseRoulante.Tourner(alpha);
            Debug.Print("après rotate " + alpha);

            return retour;
        }

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
