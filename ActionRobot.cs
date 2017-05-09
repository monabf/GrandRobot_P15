using System;
using System.IO;
using System.IO.Ports;
using Microsoft.SPOT;

namespace GR
{
    /// <summary>
    /// Fonction retournant un System.Boolean
    /// </summary>
    /// <returns></returns>
    public delegate bool FonctionBool();
    /// <summary>
    /// Fonction retournant un System.Int32
    /// </summary>
    /// <returns></returns>
    public delegate int FonctionInt();

    /// <summary>
    /// Tâche à effectuer par le robot
    /// </summary>
    class ActionRobot : IComparable
    {
        private readonly FonctionBool Tache;
        private readonly FonctionBool Condition;
        private readonly FonctionInt CalculPriorite;

        /// <summary>
        /// Si true, l'action sera supprimée de la statégie si son éxecution réussi
        /// </summary>
        public readonly bool ExecutionUnique;

        /// <summary>
        /// Vrai s'il n'existe pas de condition préalable à l'éxecution, sinon cette dernière
        /// </summary>
        public bool ExecutionPossible { get { return Condition == null || Condition(); } }

        /// <summary>
        /// Priorité de l'action si elle a été définie, sinon zéro
        /// </summary>
        public int Priorite { get { return CalculPriorite != null ? CalculPriorite() : 0; } }

        /// <summary>
        /// Constructeur d'ActionRobot
        /// </summary>
        /// <param name="tache">Tâche à éxecuter</param>
        /// <param name="condition">Condition nécessaire à l'éxecution</param>
        /// <param name="calculPriorite">Calcul de la priorité de la tâche</param>
        /// <param name="executionUnique">Si true, l'action sera supprimée de la statégie si son éxecution réussi</param>
        public ActionRobot(FonctionBool tache, FonctionBool condition = null, FonctionInt calculPriorite = null, bool executionUnique = false)
        {
            Debug.Print("action robot créée");
            Tache = tache;
            Condition = condition;
            CalculPriorite = calculPriorite;
            ExecutionUnique = executionUnique;
        }

        /// <summary>
        /// Execute la tâche
        /// </summary>
        /// <returns>Résultat de l'éxecution</returns>
        public bool Executer()
        {
            return Tache();
        }

        /// <summary>
        /// Implémentation de IComparable, voir surcharge int CompareTo(ActionRobot)
        /// </summary>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as ActionRobot);
        }
        
        /// <summary>
        /// Compare la priorité de l'action à celle d'une autre action en tenant compte des condition d'éxecution
        /// </summary>
        /// <param name="autre">Action à comparer</param>
        /// <returns>Résultat de la comparaison ({ -1; 0; 1 })</returns>
        public int CompareTo(ActionRobot autre)
        {
            int priorite, autrePriorite;
            bool possible, autrePossible;

            if (autre == null) return 1;

            priorite = Priorite;
            autrePriorite = autre.Priorite;

            possible = ExecutionPossible;
            autrePossible = autre.ExecutionPossible;

            return !possible && !autrePossible ? 0 :
                possible && !autrePossible ? 1 :
                !possible && autrePossible ? -1 :
                priorite > autrePriorite ? 1 :
                priorite < autrePriorite ? -1 : 0;
        }

        public static bool operator <(ActionRobot a, ActionRobot b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(ActionRobot a, ActionRobot b)
        {
            return a.CompareTo(b) > 0;
        }
    }
}
