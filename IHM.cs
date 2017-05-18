using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Touch;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using System.Threading;
using PetitRobotP15;

enum Couleurs { rouge, orange, jaune, vert, bleu, indigo, violet };
namespace PR
{
    class IHM
    {
        #region Attributs

        GHI.Glide.Display.Window fenetreSelection, fenetreAffichage, fenetrePhase;

        Couleur m_equipe = Couleur.Null;
        int m_disposition = 0;
        public bool validation = false;

        #endregion

        #region Attributs selection
        
        GHI.Glide.UI.Button BoutonBleu;
        GHI.Glide.UI.Button BoutonJaune;
        GHI.Glide.UI.Button BoutonDispo1;
        GHI.Glide.UI.Button BoutonDispo2;
        GHI.Glide.UI.Button BoutonDispo3;
        GHI.Glide.UI.Button BoutonDispo4;
        GHI.Glide.UI.Button BoutonDispo5;
        GHI.Glide.UI.Button BoutonValider;
        TextBlock TexteCouleur;
        TextBlock TexteDispo;

        #endregion

        #region Attributs affichage

        TextBlock TexteAffichage;
        string[] liste = new string[150];
        int nbLignes;
        int hauteurLigne;
        int largeurLigne;
        
        #endregion


        public IHM()
        {
            GlideTouch.Initialize();
            Debug.Print("IHM created");
            fenetreSelection = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.fenetreSelection));
            Glide.MainWindow = fenetreSelection;
            //Charge le fichier XML pour la fenetre de selection
            fenetreAffichage = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.fenetreAffichage));     // -- -- -- -- -- -- -- -- -- -- -- -- -- d'affichage
            fenetrePhase = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.fenetrePhase)); 
            Debug.Print("IHM completed");
                        Glide.FitToScreen = true;       //Dimensionne la fenetre pour l'adapter à l'écran LCD
            
        }

        /// <summary>
        /// Selection de l'équipe et de la disposition du terrais parmi les 5 différentes
        /// </summary>
        /// 
        /// 
        public Couleur getEquipe()
        {
            return m_equipe;
        }

        public int getDisposition()
        {
            return m_disposition;
        }
        public void retourPhase(Couleurs couleur)
        {
            switch(couleur)
            {
                case Couleurs.rouge :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Red;
                    break;
                case Couleurs.orange :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Orange;
                    break;
                case Couleurs.jaune :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Yellow;
                    break;
                case Couleurs.vert :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Green;
                    break;
                case Couleurs.bleu :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Blue;
                    break;
                case Couleurs.indigo :
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Cyan;
                    break;
                case Couleurs.violet:
                    fenetrePhase.BackColor = Microsoft.SPOT.Presentation.Media.Colors.Purple;
                    break;
                
            }
            Glide.MainWindow = fenetrePhase;
        }
        public void Selection()
        {
            Glide.MainWindow = fenetreSelection;        //Affiche la fenetre de selection sur l'écran LCD
            
            BoutonBleu = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonBleu");            //Créer les différents boutons
            BoutonJaune = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonJaune");
            BoutonDispo1 = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonHomologation");
            BoutonDispo2 = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonDispo2");
            BoutonDispo3 = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonDispo3");
            BoutonDispo4 = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonDispo4");
            BoutonDispo5 = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonDispo5");
            BoutonValider = (GHI.Glide.UI.Button)fenetreSelection.GetChildByName("BoutonValider");

            TexteCouleur = (TextBlock)fenetreSelection.GetChildByName("TexteCouleur");        //Bloc de texte pour la selection (affiche la couleur et la disposition choisie)
            TexteDispo = (TextBlock)fenetreSelection.GetChildByName("TexteDispo");            //

            BoutonBleu.TapEvent += new OnTap(sender => { m_equipe = Couleur.Bleu;
                                                         TexteCouleur.Text = "Equipe bleue";
                                                         TexteCouleur.BackColor = (Color)0x1E7FCB;
                                                         fenetreSelection.FillRect(TexteCouleur.Rect);
                                                         TexteCouleur.Invalidate(); });

            BoutonJaune.TapEvent += new OnTap(sender => { m_equipe = Couleur.Jaune;
                                                           TexteCouleur.Text = "Equipe jaune";
                                                           TexteCouleur.BackColor = (Color)0xE8D630;
                                                           fenetreSelection.FillRect(TexteCouleur.Rect);
                                                           TexteCouleur.Invalidate(); });

            BoutonDispo1.TapEvent += new OnTap(sender => { m_disposition = 1;
                                                           TexteDispo.Text = "Homologation";
                                                           fenetreSelection.FillRect(TexteDispo.Rect);
                                                           TexteDispo.Invalidate(); });

            BoutonDispo2.TapEvent += new OnTap(sender => { m_disposition = 2;
                                                           TexteDispo.Text = "Dispo. no. 2";
                                                           fenetreSelection.FillRect(TexteDispo.Rect);
                                                           TexteDispo.Invalidate(); });

            BoutonDispo3.TapEvent += new OnTap(sender => { m_disposition = 3;
                                                           TexteDispo.Text = "Dispo. no. 3";
                                                           fenetreSelection.FillRect(TexteDispo.Rect);
                                                           TexteDispo.Invalidate(); });

            BoutonDispo4.TapEvent += new OnTap(sender => { m_disposition = 4;
                                                           TexteDispo.Text = "Dispo. no. 4";
                                                           fenetreSelection.FillRect(TexteDispo.Rect);
                                                           TexteDispo.Invalidate(); });

            BoutonDispo5.TapEvent += new OnTap(sender => { m_disposition = 5;
                                                           TexteDispo.Text = "Dispo. no. 5";
                                                           fenetreSelection.FillRect(TexteDispo.Rect);
                                                           TexteDispo.Invalidate(); });

            while (m_equipe == Couleur.Null || m_disposition == 0)      //Execution de la boucle tant que l'equipe et la disposition du terrain ne sont pas validés


     
            while (validation == false)
            {
                BoutonValider.TapEvent += new OnTap(sender => { validation = true; });

            }
            
            Fermer();   //ferme la fenetre de selection  

            Glide.MainWindow = fenetreAffichage;            //Affiche la fenetre d'affichage sur l'écran LCD
            TexteAffichage = (TextBlock)fenetreAffichage.GetChildByName("TexteAffichage");          //Bloc de texte pour l'affichage des instructions
        }

        /// <summary>
        /// Affiche un message sur l'IHM
        /// </summary>
        /// <param name="message">Message a afficher sur l'IHM</param>
        public void Afficher(string message)
        {         
            //TexteAffichage.Font.ComputeTextInRect("", out largeurLigne, out hauteurLigne);          //Retourne la largeur d'une ligne et la hauteur d'une ligne
            //nbLignes = TexteAffichage.Height / hauteurLigne;
            
            TexteAffichage.Text = message + '\n';
            fenetreAffichage.FillRect(TexteAffichage.Rect);
            TexteAffichage.Invalidate();

            Thread.Sleep(500);     //test
        }

        /// <summary>
        /// Ferme la fenetre
        /// </summary>
        public void Fermer()
        {
            Glide.MainWindow = null;
            Glide.Screen.Clear();       
            Glide.Screen.Flush();       
        }
    }
}
