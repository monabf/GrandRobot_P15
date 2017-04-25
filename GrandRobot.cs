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
using GadgeteerApp4;
using Grand_Robot;
using PR;

namespace GR
{
    /// <summary>
    /// Classe représentant le grand robot
    /// </summary>
    partial class GrandRobot
    {
        public enum Axe { Null = 0, X, Y }

        public  GestionnaireStrategie Strategie;
        public  IHMTracage Tracage;

        public  Couleur Equipe;
        public  ConfigurationPorts Ports;
        public  Jack JackDemarrage;
        public  CBaseRoulante BaseRoulante;
        public  ControleurAX12 controleurAX12;
        public  CPince pince;
        public  CReservoir reservoir;
        public CBras bras;
        public  CCapteurCouleur CapteurCouleur;
        public  OutputPort m_direction;
        public  GroupeInfrarouge IR;
        public  CCapteurUltrason CapteurUltrason;
       
        public positionBaseRoulante Position = new positionBaseRoulante();
        public bool SortieOK = false;

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

            Strategie = new GestionnaireStrategie();
            Tracage = new IHMTracage();
            bras = new CBras(controleurAX12, Ports.bras);
            JackDemarrage = new Jack(Ports.IO, Ports.Jack);
            BaseRoulante = new CBaseRoulante(Ports.Plateforme);
            controleurAX12 = new ControleurAX12(Ports.AX12);
            pince = new CPince(controleurAX12, Ports.pince);
            cylindresRecup = 0;

            //Ports.ConfigCanne.direction = m_direction;
            m_direction = new OutputPort((Cpu.Pin)EMX.IO46, false);  //IO26 si 11
            /* m_RS485 = new RS485(9);
              m_RS485.Configure(500000, GT.SocketInterfaces.SerialParity.None, GT.SocketInterfaces.SerialStopBits.One, 8, GT.SocketInterfaces.HardwareFlowControl.NotRequired);
              m_RS485.Port.Open();*/


            IR = new GroupeInfrarouge(Ports.IO, Ports.InfrarougeAVD, Ports.InfrarougeAVG, Ports.InfrarougeARD, ports.InfrarougeARG);
            //TelemetreLaser = new CTelemetreLaser(Ports.TelemetreLaser, 9600);
            CapteurUltrason = new CCapteurUltrason(Ports.CapteurUltrason);

            BaseRoulante.setCouleur(equipe);
            BaseRoulante.getPosition(ref Position);

            Tracage.Afficher();
            Tracage.Ecrire("Equipe " + (equipe == Couleur.Vert ? "verte" : "violette"));

        }

        /// <summary>
        /// Attend le Jack
        /// </summary>
        public void AttendreJack()
        {
            Tracage.Ecrire("Veuillez retirer le jack pour demarrer.");

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
                while (Strategie.ExecutionPossible && DateTime.Now < fin)
                {
                    Tracage.Ecrire("Temps restant: " + (fin - DateTime.Now).ToString().Substring(3, 5) + ".");
                    Thread.Sleep(10000);
                }
            });
            var thStrat = new Thread(() => EffectuerStrategie());

            InstantDebut = DateTime.Now;
            fin = InstantDebut.AddSeconds(tempsImparti);

            InitialiserStrategie();

            thDecompte.Start();
            thStrat.Start();

            timeout = new Timer(state =>
            {
                Tracage.Ecrire("Fin du temps imparti.");
                if (thStrat.IsAlive) thStrat.Abort();
                BaseRoulante.stop();

            }, null, (int)(tempsImparti * 1000), -1);
        }

        public void EffectuerStrategie()
        {
            Tracage.Ecrire("Debut de l'execution de la strategie.");

            while (Strategie.ExecutionPossible)
            {
                Tracage.Ecrire("Execution de l'action suivante.");
                Strategie.ExecuterSuivante();
            }

            Tracage.Ecrire("Fin de l'execution de la strategie.");
            Tracage.Ecrire("Nombre de cylindres " + cylindresRecup);
        }

        public etatBR AllerEn(double x, double y, BR.sens s, vitesse speedDistance = vitesse.premiere, bool reculSiBlocage = true,
            bool detection = false, vitesse speedRotation = vitesse.vitesseRotationMin, int distanceMax = 30, int delaiDetection = 100, int tempsMax = 5)
        {
            var obstacle = false;
            var thDetection = new Thread(() =>
            {
                while (true)
                {
                    var diff = obstacle;

                    if (obstacle = DetecterObstacle(s))
                    {
                        if (obstacle != diff) Tracage.Ecrire("Obstacle detecte");
                        BaseRoulante.stop();
                    }

                    Thread.Sleep(delaiDetection);
                }
            });
            etatBR retour = etatBR.stope;
            DateTime debut;

            debut = DateTime.Now;

            if(detection) thDetection.Start();
            while ((DateTime.Now - debut).Seconds < tempsMax &&
                (retour = BaseRoulante.allerEn(x, y, s, speedDistance, speedRotation)) == etatBR.stope)
                while ((DateTime.Now - debut).Seconds < tempsMax && obstacle)
                    Thread.Sleep(1);
            if (thDetection.IsAlive) thDetection.Abort();

            if (reculSiBlocage && retour != etatBR.arrive)
                BaseRoulante.allerEn(Position.x, Position.y, s == BR.sens.avancer ? BR.sens.reculer : BR.sens.avancer);

            BaseRoulante.getPosition(ref Position);

            return retour;
        }

        public etatBR Tourner(double angle, vitesse v = vitesse.vitesseRotationMin)
        {
            int alphaReel = 0;
            var retour = BaseRoulante.tourner(angle, v, ref alphaReel);

            BaseRoulante.getPosition(ref Position);

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
