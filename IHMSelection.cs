using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace GR
{
    /// <summary>
    /// Interface de sélection de la couleur de l'équipe et de la disposition des coquillages
    /// </summary>
    class IHMSelection : IIHM
    {
        private Window Fenetre;
        private Couleur _Equipe;
        private int _Disposition;
        private TextBlock TexteCouleur, TexteDispo;

        /// <summary>
        /// Couleur séléctionnée par l'utilisateur
        /// </summary>
        public Couleur Equipe
        {
            get { return _Equipe; }
            private set
            {
                switch (_Equipe = value)
                {
                    case Couleur.Vert:
                        TexteCouleur.Text = "Equipe verte.";
                        TexteCouleur.BackColor = (Color)0x008000;
                        break;
                    case Couleur.Violet:
                        TexteCouleur.Text = "Equipe violette.";
                        TexteCouleur.BackColor = (Color)0x800080;
                        break;
                }

                Fenetre.FillRect(TexteCouleur.Rect);
                TexteCouleur.Invalidate();
            }
        }

        /// <summary>
        /// Disposition séléctionnée par l'utilisateur
        /// </summary>
        public int Disposition
        {
            get { return _Disposition; }
            private set
            {
                TexteDispo.Text = "Disposition: no. " + (_Disposition = value);

                Fenetre.FillRect(TexteDispo.Rect);
                TexteDispo.Invalidate();
            }
        }

        /// <summary>
        /// Evénement déclenché lors de la validation de la sélection par l'utilisateur
        /// </summary>
        public EventHandler Validation;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public IHMSelection()
        {
            // initialisation de la fenêtre

            Fenetre = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.FenetreSelection));

            TexteCouleur = (TextBlock)Fenetre.GetChildByName("TexteCouleur");

            TexteDispo = (TextBlock)Fenetre.GetChildByName("TexteDispo");

            // valeurs par défaut

            Equipe = Couleur.Vert;
            Disposition = 1;

            // événements déclenchés par les boutons (selections)

            ((Button)Fenetre.GetChildByName("BoutonVert")).TapEvent += sender =>
            { Equipe = Couleur.Vert; };
            ((Button)Fenetre.GetChildByName("BoutonViolet")).TapEvent += sender =>
            { Equipe = Couleur.Violet; };

            ((Button)Fenetre.GetChildByName("BoutonDispo1")).TapEvent += sender =>
            { Disposition = 1; };
            ((Button)Fenetre.GetChildByName("BoutonDispo2")).TapEvent += sender =>
            { Disposition = 2; };
            ((Button)Fenetre.GetChildByName("BoutonDispo3")).TapEvent += sender =>
            { Disposition = 3; };
            ((Button)Fenetre.GetChildByName("BoutonDispo4")).TapEvent += sender =>
            { Disposition = 4; };
            ((Button)Fenetre.GetChildByName("BoutonDispo5")).TapEvent += sender =>
            { Disposition = 5; };

            ((Button)Fenetre.GetChildByName("BoutonValider")).TapEvent += sender =>
            { Validation(this, EventArgs.Empty); };

            Glide.FitToScreen = true;
            Glide.MainWindow = Fenetre;
        }

        /// <summary>
        /// Affiche l'IHM
        /// </summary>
        public void Afficher()
        {
            Glide.MainWindow = Fenetre;
        }

        /// <summary>
        /// Ferme l'IHM
        /// </summary>
        public void Fermer()
        {
            Glide.MainWindow = null;
            Glide.Screen.Clear();
            Glide.Screen.Flush();
        }
    }
}
