using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GR
{
    class CCapteurCouleur
    {
		Couleur couleurInitiale;
        ColorSense myColorSense;
        static int White = 2, Yellow = 1, Blue = 0;
        int c1, c2, c3, c4;
        Couleur ourColor = Couleur.Bleu;

        int config = -1; 
            // sens de rotation du cylindre  = direct
            int pricisionAngle = (int) (360 * System.Math.PI / 10);
            int defaultAngle = (int) (360 * System.Math.PI / 4); //Toujours Pi/4

        public static int getHue(int red, int green, int blue)
        {

            float min = System.Math.Min(System.Math.Min(red, green), blue);
            float max = System.Math.Max(System.Math.Max(red, green), blue);

            float hue = 0f;
            
                if (max == red)
                {
                    hue = (green - blue) / (max - min+1);

                }
                else if (max == green)
                {
                    hue = 2f + (blue - red) / (max - min+1);

                }
                else
                {
                    hue = 4f + (red - green) / (max - min+1);
                }

                hue = hue * 60;
            
            if (hue < 0) hue = hue + 360;

            return (int)System.Math.Round(hue);

        }
      

        
        //Constructeur
        public CCapteurCouleur(int id, Couleur ourColor)
        {
            couleurInitiale = ourColor;
            myColorSense = new ColorSense(id);
            myColorSense.LedEnabled = true;
            Debug.Print("ColorSense opérationnel");


            if (ourColor == Couleur.Bleu)
            {
                c1 = 5; c2 = 1; c3 = 7; c4 = 3;
            }
            else
            {
                c1 = 1; c2 = 5; c3 = 3; c4 = 7;
            }
          
        }
        public bool ContinuerRotation()
        {
            //Lire les couleurs 
            //***********
            ColorSense.ColorData colors = myColorSense.ReadColor();
            int HUE = getHue(colors.Red, colors.Green, colors.Blue);
            bool white = colors.Red + colors.Blue + colors.Green > 420;
            //**********

            //Debug.Print("prog continuerRotation" + HUE + "-" + colors.Red + " " + colors.Blue + " " + colors.Green);
            //Attention !!!!!! il faut mettre le parametre couleurInitiale à -1 pour chaque cylindre
            if (white) return true;
                
            else
                if (couleurInitiale == Couleur.Jaune)
                {
                    if (HUE > 150) return false; //Blue
                    else return true;
                }
                else
                {
                    if (HUE < 100) return false; //Jaune
                    else return true;
                }

        }
        /*
        //Continuer la rotation
        public bool ContinuerRotation()
        {
            //Lire les couleurs 
            //***********
            ColorSense.ColorData colors = myColorSense.ReadColor();
            int HUE = getHue(colors.Red, colors.Blue, colors.Green);
            bool white = colors.Red + colors.Blue + colors.Green > 420;
            //**********

            Debug.Print("prog continuerRotation" + HUE + "-" + colors.Red +" "+ colors.Blue+" "+ colors.Green);
            //Attention !!!!!! il faut mettre le parametre couleurInitiale à -1 pour chaque cylindre
            if (couleurInitiale == -1)
            {
                if (white) couleurInitiale = 2;
                else if (HUE > 150) couleurInitiale = 0; //Blue
                else if (HUE < 100) couleurInitiale = 1; //Yellow
            }

			/////////////////////////////////////////////////
            if (HUE > 150)
            {
                if (couleurInitiale != 0)
                {
                    config = 1;
                    return false;
                }
                return true;

            }
            else if (HUE < 100)
            {
                if (couleurInitiale != 1)
                {
                    config = 2;
                    return false;
                }
                return true;
            }
            else if (white)
            {
                if (couleurInitiale != 2) { 
						if (couleurInitiale == 1){
							config = 3;
							return false;
						}else if (couleurInitiale == 0){
							config = 4;
							return false;
						}
                   return true; //en cas d'erreur (couleurInitiale = -1) !
                 }
                return true;
            }
            else { return true; } //la couleur n'est pas claire
        }
        */

        public int CompleterRotation()
        {
            //
            switch (config)
            {
                case 1:
                    return (int) (c1 * defaultAngle);
                case 2:
                    return (int) (c2 * defaultAngle);
                case 3:
                    return (int) (c3 * defaultAngle);
                case 4:
                    return (int) (c4 * defaultAngle);
                default:
                    Debug.Print("Error 'config cylinder' !");
					return 0;
            }
        }
    }
}
