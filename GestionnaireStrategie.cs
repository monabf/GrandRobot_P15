using System.Collections;
using System.Threading;
using System;
using System.IO;
using System.IO.Ports;
using Microsoft.SPOT;

namespace GR
{
    /// <summary>
    /// Gestionnaire de stratégie contenant des tâches à éxecuter sous certaines conditons, par ordre de priorité
    /// </summary>
    class GestionnaireStrategie
    {
        private ArrayList Actions;

        private ActionRobot ActionOptimale
        {
            get
            {
                ActionRobot optimale = null;

                foreach (ActionRobot action in Actions)
                    if (action.ExecutionPossible && action > optimale)
                        optimale = action;

                return optimale;
            }
        }

        /// <summary>
        /// Possibilité de continuer l'éxecution
        /// </summary>
        public bool ExecutionPossible
        {
            get
            {
                return ActionOptimale != null;
            }
        }

        /// <summary>
        /// Nombre d'actions éxistantes dans la stratégie
        /// </summary>
        public int NombreAction { get { return Actions.Count; } }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public GestionnaireStrategie()
        {
            Actions = new ArrayList();
        }

        /// <summary>
        /// Ajoute une action à la stratégie
        /// </summary>
        /// <param name="action">Action à ajouter</param>
        public void Ajouter(ActionRobot action)
        {
            Actions.Add(action);
        }

        /// <summary>
        /// Supprime une action de la stratégie
        /// </summary>
        /// <param name="action">Action à supprimer</param>
        public void Supprimer(ActionRobot action)
        {
            Actions.Remove(action);
            while (Actions.Count == 0)
            {
                Thread.Sleep(20);
            }
        }
        
        /// <summary>
        /// Recherche un élément existant dans la liste des actions
        /// </summary>
        /// <param name="action">Action à rechercher</param>
        /// <returns>Retourne si la stratégie contient l'action spécifiée</returns>
        public bool Contient(ActionRobot action)
        {
            return Actions.Contains(action);
        }

        /// <summary>
        /// Tente d'éxecuter l'action optimale suivante
        /// </summary>
        /// <returns>Résultat de l'éxecution de l'action</returns>
        public bool ExecuterSuivante()
        {
            var optimale = ActionOptimale;
            var resultat = optimale != null ? optimale.Executer() : false;

            if (resultat && optimale.ExecutionUnique)
                Actions.Remove(optimale);
            Debug.Print("executer suivante");
            return resultat;
        }
    }
}
