using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GR.Vision
{
    class CCapteurUltrason
    {
        private DistanceUS3 m_distanceUS3;
        private bool isOpen;

        public CCapteurUltrason(int socketNumber)
        {
            m_distanceUS3 = new DistanceUS3(socketNumber);
            isOpen = true;
        }
        ~CCapteurUltrason()
        { 
            
        }
        /// <summary>
        /// Permet de recuperer la distance entre le capteur et l'obstacle, en fonction d'un nombre de mesure et il retourne une moyenne
        /// </summary>
        /// <param name="nbrMesure">nombre de mesure que prends le capteur et renvoie une moyenne</param>
        /// <param name="distance">distance de l'obstacle (cm)</param>
        /// <returns></returns>
        public bool getDistance( int nbrMesure, ref double distance)
        {
            bool check = false;
            distance = 0;
            if (isOpen == true)
            {
                distance = m_distanceUS3.GetDistance(nbrMesure); 
                check = true;
            }
            return check;
        }
    }
}
