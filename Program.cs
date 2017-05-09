using GHI.Glide;
using Microsoft.SPOT;
using System.Threading;

namespace GR
{
    public partial class Program
    {
        private bool SelectionValidee;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            GrandRobot robot;
            //IHMSelection selection;

            /*
            ports.numIO= 5;
            ports.pinJack = 8;
            ports.pinAVG = 4;
            ports.pinAVD = 5;
            ports.pinARG = 6;
            ports.pinARD = 7;
            ports.numSocketUltrason = 6;
            ports.numSerialPortBaseRoulante = 8;
            ports.numSerialPortMembres = 11;
            ports.confChasseNeige.idAX12BDroit = 3;
            ports.confChasseNeige.idAX12BGauche = 2;
            ports.confBras.idAX12Coude = 6;
            ports.confBras.idAX12Poignet = 5;*/
            // initialisation des ports

            // numéros de ports à corriger

            

            var ports = new ConfigurationPorts
            {
                Plateforme = 11,
                IO = 5,
                Jack = 3, // pin 1 de l'extendeur
                InfrarougeAVG = 7,
                InfrarougeAVD = 6,
                InfrarougeARG = 5,
                InfrarougeARD = 4,
                contAX12 = 9,
            };

            ports.bras.idAx12BrasSupport = 1; // num servo
            ports.bras.idAx12BrasModule = 2; // idem
            ports.pince.idAx12PinceSupport = 3;
            ports.pince.idAx12PinceModule = 4;
            ports.reservoir.idAx12Rotateur = 5;
            ports.reservoir.idAx12Poussoir = 6;
            ports.idFunnyBras = 7; // idem

        //    ports.reservoir.idCapteurCouleur = ; // pin de la spider



            
            // initialisation de GHI Glide pour les IHMs
            GlideTouch.Initialize();
            Glide.FitToScreen = true;


            // initialisation de l'IHM de sélection
            //selection = new IHMSelection();
           // selection.Validation += SelectionEffectuee;

            // affiche l'IHM de sélection et attende de la validation
            //selection.Afficher();
            //while (!SelectionValidee) Thread.Sleep(1);
            //selection.Fermer();
            Debug.Print("bienvenue les gens");
            // initialisation du robot
            robot = new GrandRobot(
                ports,
                //selection.Equipe
                Couleur.Bleu
            );
            Debug.Print("Grand robot construit !");

            // attente du jack
            robot.AttendreJack();
            Debug.Print("on n'attend plus !");
            // démarre le robot
            robot.Demarrer(90d);
            Debug.Print("demarrage ok");
        }

        private void SelectionEffectuee(object sender, EventArgs e)
        {
            var selection = sender as IHMSelection;

            SelectionValidee = selection.Equipe != Couleur.Null && selection.Disposition > 0;
        }
    }
}
