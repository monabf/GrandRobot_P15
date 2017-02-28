using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using System.IO.Ports;
using System.Text;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GR.Vision
{
    class CTelemetreLaser
    {
        #region Attributs

        private RS232 m_portSerie;
        private bool m_portIsOpen;

        #endregion

        #region Constructeur/Destructeur

        /// <summary>
        /// Constructeur avec paramètres d'initialisation (seulement les paramètres qui ont une valeur variable ici)
        /// </summary>
        /// <param name="socketNumber">Selection port sur la spider</param>
        /// <param name="baudrate">Vitesse</param>
        public CTelemetreLaser(int socketNumber,int baudrate)
        {
            m_portIsOpen = false;
            m_portSerie = new RS232(socketNumber);//Création port
            m_portSerie.Configure(baudrate, GT.SocketInterfaces.SerialParity.None, GT.SocketInterfaces.SerialStopBits.One, 8, GT.SocketInterfaces.HardwareFlowControl.NotRequired);//configuration avec les paramètres
            m_portSerie.Port.Open();//ouverture du port
            m_portIsOpen=m_portSerie.Port.IsOpen;
            
        }
         ~CTelemetreLaser()
        { 
            
        }

        #endregion

        #region Méthodes Infos Télémètre

         /// <summary>
        /// Récuperer : version du firmware, num série,...
        /// </summary>
        public bool takeSensorVersion(ref char[] information)//Fonctionne
        {
            bool check = false;
            char[] reponse = new char[10];
            int i = 0;
            if (m_portIsOpen == true)
            {
                m_portSerie.Port.WriteLine("VV\n");
                i = 0;
                //Récuperer echo commande 
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1]!= '\n');
                //Récuperer 00P
                i = 0;
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1] != '\n');
                //Récuperer informations utiles
                i = 0;
                information = new char[1000];
                do
                {
                    do
                    {
                        information[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (information[i - 1] != '\n');
                    i = i - 2;
                } while (information[i + 2] != '\n');
                check = true;
            }
            return check;
        }
        /// <summary>
        /// Récupérer les spécification du capteur laser
        /// </summary>
        public bool takeSensorSpecs(ref char[] specifications)//Fonctionne
        {
            char[] reponse = new char[10];
            int i = 0;
            bool check = false;
            if (m_portIsOpen == true)
            {
                m_portSerie.Port.WriteLine("PP\n");
                //Récuperer echo commande 
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1] != '\n');
                //Récuperer 00P
                i = 0;
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1] != '\n');
                //Récuperer informations utiles
                i = 0;
                specifications = new char[1000];
                do
                {
                    do
                    {
                        specifications[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (specifications[i - 1] != '\n');
                    i = i - 2;
                } while (specifications[i + 2] != '\n');
                check = true;
            }
            return check;
        }
        /// <summary>
        /// Récupérer état du capteur laser
        /// </summary>
        public bool takeSensorStatut(ref char[] statutCapteur)//Fonctionne
        {
            char[] reponse = new char[10];
            int i = 0;
            bool check = false;
            if (m_portIsOpen == true)
            {
                m_portSerie.Port.WriteLine("II\n");
                //Récuperer echo commande 
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1] != '\n');
                //Récuperer 00P
                i = 0;
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();

                } while (reponse[i - 1] != '\n');
                //Récuperer informations utiles
                i = 0;
                statutCapteur = new char[1000];
                do
                {
                    do
                    {
                        statutCapteur[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (statutCapteur[i - 1] != '\n');
                    i = i - 2;
                } while (statutCapteur[i + 2] != '\n');
                check = true;
            }
            return check;
        }

        #endregion 

        #region Vitesse Moteur/Port Série
        /// <summary>
        /// Changer valeur vitesse de transfert du RS232(defaut : 19.2Kbps)
        /// </summary>
        /// <param name="baudrate">Vitesse capteur : 19.2Kbps, 57,2Kbps, 115,2Kbps, 250Kbps, 500Kbps, 750Kbps</param>
        public bool updateBaudRateValue(long baudrate, ref char[] statut) //Fonctionne mais à améliorer
        {
            bool check = false;
            string trame = "0";
            char[] reponse = new char[20];
            int i = 0;
            if (m_portIsOpen == true)
            {
                if (baudrate == 019200 || baudrate == 057600 || baudrate == 115200 || baudrate == 250000 || baudrate == 500000 || baudrate == 750000)
                {
                    trame = "SS" + baudrate.ToString("D6") + "\n";
                    m_portSerie.Port.WriteLine(trame);
                    //Récuperation echo commande + baudrate
                    do
                    {
                        reponse[i++] = (char)m_portSerie.Port.ReadByte();
                    } while (reponse[i-1] != '\n');
                    //Récupération statut
                    i = 0;
                    statut = new char[15];
                    do
                    {
                        do
                        {
                            statut[i] = (char)m_portSerie.Port.ReadByte();
                            i++;
                        } while (statut[i - 1] != '\n');
                        i = i - 2;
                    } while (statut[i + 2] != '\n');
                    check = true;
                }
            }
            return check;
        }
        /// <summary>
        /// Changer la vitesse du motor du capteur laser
        /// </summary>
        /// <param name="vitesse">permet de d'augmenter ou de baisser la vitesse de rotation du moteur</param>
        /// <param name="status">permet de savoir s'il y a eu un probleme avec la commande</param>
        /// <returns></returns>
        public bool updateSpeedMotor(short vitesse, ref char[] status)//Fonctionne
        {
            bool check = false;
            string trame = "0";
            int i = 0;
            char[] reponse = new char [10];
            if (m_portIsOpen == true)
            {
                trame = "CR" + vitesse.ToString("D2") + "\n";
                m_portSerie.Port.WriteLine(trame);
                //Reception echo commande + ratio vitesse
                do
                {
                    reponse[i] = (char)m_portSerie.Port.ReadByte();
                    i++;
                } while (reponse[i - 1] != '\n');
                //Reception status
                status = new char[15];
                i = 0;
                do
                {
                    do
                    {
                        status[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (status[i - 1] != '\n');//on check le caractère avant pour voir s'il est égale à \n
                    i = i - 2;
                } while (status[i + 2] != '\n');
                check = true;
            }
            
            return check;
        }

        #endregion

        #region Activer/Désactiver Capteur

        /// <summary>
        /// Allumer le capteur laser
        /// </summary>
        public bool enableMeasurement(ref char[] status)//Fonctionne
        {
            bool check = false;
            int i = 0;
            char[] reponse = new char[10]; 
            if (m_portIsOpen)
            {
                m_portSerie.Port.WriteLine("BM\n");
                //Récupération echo commande 
                do
                {
                    reponse[i] = (char)m_portSerie.Port.ReadByte();
                    i++;
                } while (reponse[i - 1] != '\n');
                //Récupération status
                i = 0;
                status = new char[15];
                do
                {
                    do
                    {
                        status[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (status[i - 1] != '\n');
                    i = i-2; 
                } while (status[i + 2] != '\n');
                check = true;
            }
            return check;
        }
        /// <summary>
        /// Eteindre le capteur laser
        /// </summary>
        public bool disableMeasurement(ref char[] status)//Fonctionne
        {
            bool check = false;
            int i = 0;
            char[] reponse = new char[10];
            if (m_portIsOpen)
            {
                m_portSerie.Port.WriteLine("QT\n");
                //Récupération echo commande 
                do
                {
                    reponse[i] = (char)m_portSerie.Port.ReadByte();
                    i++;
                } while (reponse[i - 1] != '\n');
                //Récupération status
                i = 0;
                status = new char[15];
                do
                {
                    do
                    {
                        status[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    } while (status[i - 1] != '\n');
                    i = i - 2;
                } while (status[i + 2] != '\n');
                check = true;
            }
            return check;
        }

        #endregion

        #region Acquisition Données

        /// <summary>
        /// Lancer l'acquisition des données
        /// </summary>
        /// <param name="startingStep">début plage de vision</param>
        /// <param name="endStep">fin plage de vision</param>
        /// <param name="distance">tableau des distances</param>
        /// <param name="clusterCount">nombre de données fusionnées en une</param>
        /// <param name="interval"></param>
        /// <param name="scans">nombre de scans</param>
        /// <returns></returns>
        public bool getDistance(int startingStep, int endStep, ref int [] distance, ref char[] erreurStatus,int clusterCount = 1, int interval = 1,int scans = 1)
        {
            string commande;
            bool check = false;
            char[] reponse = new char [2000];
            char[] status = new char [5];
            char recu;
            int i=0;
            if (m_portIsOpen ==  true)
            {
                commande = "MS" + startingStep.ToString("D4") + endStep.ToString("D4") + clusterCount.ToString("D2") + interval.ToString("D1") + scans.ToString("D2") + "\n";
                m_portSerie.Port.WriteLine(commande);
                //boucle de reception l'echo de la commande 
                i = 0;
                do
                {
                    reponse[i++]=(char)m_portSerie.Port.ReadByte();
                } while (reponse[i-1]!='\n');

                //reception de "00P\n"
                i = 0;
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();
                } while (reponse[i - 1] != '\n');

                //reception de '\n'
                recu = (char)m_portSerie.Port.ReadByte();

                //boucle de reception de l'echo de la commande 
                i = 0;
                do
                {
                    reponse[i++] = (char)m_portSerie.Port.ReadByte();
                } while (reponse[i - 1] != '\n');

                //reception du status
                i=0;
                do
                {
                    status[i++] = (char)m_portSerie.Port.ReadByte();
                } while (status[i-1] != '\n');

                //si statut == 99
                if(status[0]=='9' && status[1]=='9')
                {
                    char[] timeStamp=new char[4];
                    //reception du timestamp
                    for (int j = 0; j < 4; j++)
                    {
                        timeStamp[j] = (char)m_portSerie.Port.ReadByte();
                    }
                    m_portSerie.Port.ReadByte(); //ignore le checksum
                    m_portSerie.Port.ReadByte(); //ignore le \n

                    //recuperations des données utiles
                    i = 0;
                    do
                    {
                        do
                        {
                            reponse[i] = (char)m_portSerie.Port.ReadByte();
                            i++;
                        } while (reponse[i-1] != '\n');
                        i = i - 2;
                    }while(reponse[i+2]!= '\n');
                    reponse[i + 1] = '\0';
                    
                    //conversion des données utiles en int
                    i=0;
                    int value1 = 0; 
                    int value2 = 0;
                    for (int j = 0; j < endStep - startingStep; j++)
                    {
                        distance[j] = 0;
                    }
                    for (int j = 0; j < endStep-startingStep; j++)
                    {
                        value1 = reponse[i++] - 0x30;
                        value2 = reponse[i++] - 0x30;
                        distance[j] = value1 << 6;
                        distance[j] |= value2;                     
                    }
                }
                else  //erreurs de status
                {
                    erreurStatus = new char[5];
                    erreurStatus = status;
                }
                check = true;
            }
            return check;
        }

        #endregion

        #region Reset

        public bool resetTelemetre()
        { 
            bool check = false;
            int i = 0;
            char [] reponse = new char[10];

            if (m_portIsOpen)
            {
                m_portSerie.Port.WriteLine("RS\n");
                //Récupération echo de la commande
                do
                {
                  reponse[i] = (char)m_portSerie.Port.ReadByte();
                  i++;
                }while(reponse[i-1] != '\n');
                //Récupération 00P
                i=0;
                do
                {
                    do
                    {
                        reponse[i] = (char)m_portSerie.Port.ReadByte();
                        i++;
                    }while(reponse[i-1] != '\n');
                    i = i - 2;
                }while(reponse[i+2] != '\n');
                check = true;
            }
            return check;
        }

        #endregion
    }
}
