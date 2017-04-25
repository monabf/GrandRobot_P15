using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using System;
using System.Collections;
using Grand_Robot;
namespace GR
{
    /// <summary>
    /// IHM de journalisation
    /// </summary>
    class IHMTracage : IIHM
    {
        private readonly Window Fenetre;
        private readonly TextBlock TexteJournal;
        private readonly Button BoutonEffacer;
        private readonly Button BoutonHaut;
        private readonly Button BoutonBas;
        private readonly Button BoutonDebut;
        private readonly Button BoutonFin;

        private readonly ArrayList Tampon;
        private readonly int LignesMax;
        private readonly int HauteurLigne;


        private bool DefilementAuto = true;

        private int _Index = 0;
        private int Index
        {
            get { return _Index; }
            set
            {
                int max = Tampon.Count - LignesMax;

                if (max < 0) max = 0;

                if (value < 0) _Index = 0;
                else if (value > max) _Index = max;
                else _Index = value;

                DefilementAuto = _Index == max;
                Rafraichir();
            }
        }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public IHMTracage()
        {
            int largeurLigne;

            Tampon = new ArrayList();

            // initialisation de la fenêtre
            Fenetre = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.FenetreTracage));
            TexteJournal = Fenetre.GetChildByName("TexteJournal") as TextBlock;
            BoutonEffacer = Fenetre.GetChildByName("BoutonEffacer") as Button;
            BoutonHaut = Fenetre.GetChildByName("BoutonHaut") as Button;
            BoutonBas = Fenetre.GetChildByName("BoutonBas") as Button;
            BoutonDebut = Fenetre.GetChildByName("BoutonDebut") as Button;
            BoutonFin = Fenetre.GetChildByName("BoutonFin") as Button;

            BoutonEffacer.TapEvent += sender =>
            {
                Tampon.Clear();
                DefilementAuto = true;
                Rafraichir();
            };
            BoutonHaut.TapEvent += sender => Index--;
            BoutonBas.TapEvent += sender => Index++;
            BoutonDebut.TapEvent += sender => Index = 0;
            BoutonFin.TapEvent += sender => Index = Tampon.Count;

            Glide.FitToScreen = true;
            Glide.MainWindow = Fenetre;

            TexteJournal.Font.ComputeTextInRect("bob", out largeurLigne, out HauteurLigne);
            LignesMax = TexteJournal.Height / HauteurLigne;
        }

        /// <summary>
        /// 
        /// Affiche l'IHM
        /// </summary>
        public void Afficher()
        {
            Glide.MainWindow = Fenetre;
        }

        public void Ecrire(string texte)
        {
            Tampon.Add("<" + DateTime.Now.ToString("mm:ss") + "> " + texte + '\n');
            if (DefilementAuto) Index++;
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

        private void Rafraichir()
        {
            TexteJournal.Text = string.Empty;

            for (int i = Index; i < Tampon.Count; i++)
                TexteJournal.Text += Tampon[i];

            Fenetre.FillRect(TexteJournal.Rect);
            TexteJournal.Invalidate();
        }
    }
}
