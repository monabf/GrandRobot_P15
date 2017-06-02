using Microsoft.SPOT;
using System.Threading;

namespace GR
{
    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            var ports = new ConfigurationPorts
            {
                Plateforme = 8,
                IO = 5,
                Jack = 3, // pin 1 de l'extendeur
                InfrarougeAVG = 7,
                InfrarougeAVD = 6,
                InfrarougeARG = 5,
                InfrarougeARD = 4,
                contAX12 = 11,
            };

            ports.bras.idAx12BrasSupport = 3; // num servo
            ports.bras.idAx12BrasModule = 4; // idem
            ports.pince.idAx12PinceSupport = 1;
            ports.pince.idAx12PinceModule = 2;
            ports.reservoir.idAx12Rotateur = 5;
            ports.reservoir.idAx12Poussoir = 6;
            ports.funnyBras = 7; // idem

            ports.reservoir.idCapteurReservoir = 4; // pin de la spider

            IHM ihm_principal;
            ihm_principal = new IHM();
            ihm_principal.Selection();

            GrandRobot robot;
            robot = new GrandRobot(ports, ihm_principal.getEquipe(), ihm_principal.getDisposition(), ihm_principal);

            
            Debug.Print("bienvenue les gens");

            Debug.Print("Grand robot construit !");

            // attente du jack
            robot.AttendreJack();

            Debug.Print("on n'attend plus !");
            // démarre le robot
            robot.Demarrer(90d);
            Debug.Print("demarrage ok");
        }

    }
}
