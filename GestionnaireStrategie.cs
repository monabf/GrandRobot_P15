using System.Collections;
using System.Threading;
using System;
using System.IO;
using System.IO.Ports;
using Microsoft.SPOT;

namespace GR
{
    /// <summary>
    /// Gestionnaire de strat�gie contenant des t�ches � �xecuter sous certaines conditons, par ordre de priorit�
    /// </summary>
    class GestionnaireStrategie
    {
        private ArrayList Actions;

        private ActionRobot ActionOptimale()
        {

            ActionRobot optimale = null;
            // attention le coup de l'indice c'est assez moche
            // mais apparemment le foreach bugue en parcourant Actions, et renvoie null pour chaque action removed
            int j = 0;
            foreach (ActionRobot action in Actions) {
                if (j < Actions.Count)
                {
                    j++;
                    if (action.ExecutionPossible() && action > optimale)
          //              Debug.Print("if pass�e pour une certaine action");
                    optimale = action;
                }

            }
        //    Debug.Print("action optimale boucle for ok");

            return optimale;

        }

        /// <summary>
        /// Possibilit� de continuer l'�xecution
        /// </summary>
        public bool ExecutionPossible()
        {
            return (ActionOptimale() != null);
        }

        /// <summary>
        /// Nombre d'actions �xistantes dans la strat�gie
        /// </summary>
        public int NombreAction { get { return Actions.Count; } }

        /// <summary>
        /// Constructeur par d�faut
        /// </summary>
        public GestionnaireStrategie()
        {
            Actions = new ArrayList();
        }

        /// <summary>
        /// Ajoute une action � la strat�gie
        /// </summary>
        /// <param name="action">Action � ajouter</param>
        public void Ajouter(ActionRobot action)
        {
            Actions.Add(action);
        }

        /// <summary>
        /// Supprime une action de la strat�gie
        /// </summary>
        /// <param name="action">Action � supprimer</param>
        public void Supprimer(ActionRobot action)
        {
            Actions.Remove(action);
            while (Actions.Count == 0)
            {
                Thread.Sleep(20);
            }
        }
        
        /// <summary>
        /// Recherche un �l�ment existant dans la liste des actions
        /// </summary>
        /// <param name="action">Action � rechercher</param>
        /// <returns>Retourne si la strat�gie contient l'action sp�cifi�e</returns>
        public bool Contient(ActionRobot action)
        {
            return Actions.Contains(action);
        }

        /// <summary>
        /// Tente d'�xecuter l'action optimale suivante
        /// </summary>
        /// <returns>R�sultat de l'�xecution de l'action</returns>
        public bool ExecuterSuivante()
        {
            var optimale = ActionOptimale();
            var resultat = false;
            if (optimale != null)
            {
                Debug.Print("opt " + (optimale != null));
                optimale.Executer();
                if (optimale.ExecutionUnique)
                {
                    Supprimer(optimale);
                    resultat = true;
                }
            }

          //  if (resultat && optimale.ExecutionUnique)

          //  Debug.Print(Actions+"");
         //   Debug.Print("executer suivante");
            return resultat;
        }
    }
}
