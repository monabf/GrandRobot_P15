using System;
using Microsoft.SPOT;

namespace GR
{
    // ce fichier contient la logique des tests pour la base roulante

    partial class GrandRobot
    {
        public void DemarrerTestBR()
        {
            throw new NotImplementedException();
            /*IHMTestBR ihm;
            var posInit = new positionBaseRoulante();

            var test = BaseRoulante.getPosition(ref posInit);
            ihm = new IHMTestBR();

            ihm.ChangementVitesse += vitesse => BaseRoulante.setSpeed(vitesse);
            ihm.Deplacement += (x, y, alpha, signe, dir) =>
            {
                var posBRDebut = new positionBaseRoulante();
                var posBRFin = new positionBaseRoulante();
                
                var err1 = BaseRoulante.getPosition(ref posBRDebut);
                BaseRoulante.goToXY(x, y, alpha, signe, dir);
                var err2 = BaseRoulante.getPosition(ref posBRFin);
                Debug.Print("Debut: " + posBRDebut.posx + ", " + posBRDebut.posy + " - " + err1);
                Debug.Print("Fin: " + posBRFin.posx + ", " + posBRFin.posy + " - " + err2);
                ihm.Position = new IHMTestBR.PositionTestBR(posBRFin.posx, posBRFin.posy, posBRFin.alpha, 
                    (int)System.Math.Sqrt(System.Math.Pow(posBRFin.posx - posBRDebut.posx, 2) + System.Math.Pow(posBRFin.posy - posBRDebut.posy, 2)));
            };

            Tracage.Fermer();
            ihm.Afficher(posInit);
            ihm.Position = new IHMTestBR.PositionTestBR(posInit.posx, posInit.posy, posInit.alpha, 0);*/
        }
    }
}
