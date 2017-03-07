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
            IHMSelection selection;

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
                Plateforme = 10,
                IO = 5,
                Jack = 4,
                InfrarougeAVG = 6,
                InfrarougeAVD = 8,
                InfrarougeARG = 7,
                InfrarougeARD = 9,
                CapteurUltrason = 6,
                AX12 = 11,
                IdFunnyBras = 3,
            };

            ports.bras.idAx12BrasSupport = 1;
            ports.bras.idAx12BrasModule = 2;

            ports.pince.idAx12PinceSupport = 3;
            ports.pince.idAx12PinceModule = 4;

            ports.reservoir.idAx12Poussoir = 12;
            ports.reservoir.idAx12Rotateur = 13;
            
            // initialisation de GHI Glide pour les IHMs
            GlideTouch.Initialize();
            Glide.FitToScreen = true;

            // initialisation de l'IHM de sélection
            selection = new IHMSelection();
            selection.Validation += SelectionEffectuee;

            // affiche l'IHM de sélection et attende de la validation
            selection.Afficher();
            while (!SelectionValidee) Thread.Sleep(1);
            selection.Fermer();
          
            // initialisation du robot
            robot = new GrandRobot(
                ports,
                selection.Equipe
            );

            // attente du jack
            robot.AttendreJack();
            // démarre le robot
            robot.Demarrer(90d);
        }

        private void SelectionEffectuee(object sender, EventArgs e)
        {
            var selection = sender as IHMSelection;

            SelectionValidee = selection.Equipe != Couleur.Null && selection.Disposition > 0;
        }
    }
}
