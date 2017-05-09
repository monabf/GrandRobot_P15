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
    /// T�che � effectuer par le robot
    /// </summary>
    class ActionRobot : IComparable
    {
        private readonly FonctionBool Tache;
        private readonly FonctionBool Condition;
        private readonly FonctionInt CalculPriorite;

        /// <summary>
        /// Si true, l'action sera supprim�e de la stat�gie si son �xecution r�ussi
        /// </summary>
        public readonly bool ExecutionUnique;

        /// <summary>
        /// Vrai s'il n'existe pas de condition pr�alable � l'�xecution, sinon cette derni�re
        /// </summary>
        public bool ExecutionPossible { get { return Condition == null || Condition(); } }

        /// <summary>
        /// Priorit� de l'action si elle a �t� d�finie, sinon z�ro
        /// </summary>
        public int Priorite { get { return CalculPriorite != null ? CalculPriorite() : 0; } }

        /// <summary>
        /// Constructeur d'ActionRobot
        /// </summary>
        /// <param name="tache">T�che � �xecuter</param>
        /// <param name="condition">Condition n�cessaire � l'�xecution</param>
        /// <param name="calculPriorite">Calcul de la priorit� de la t�che</param>
        /// <param name="executionUnique">Si true, l'action sera supprim�e de la stat�gie si son �xecution r�ussi</param>
        public ActionRobot(FonctionBool tache, FonctionBool condition = null, FonctionInt calculPriorite = null, bool executionUnique = false)
        {
            Debug.Print("action robot cr��e");
            Tache = tache;
            Condition = condition;
            CalculPriorite = calculPriorite;
            ExecutionUnique = executionUnique;
        }

        /// <summary>
        /// Execute la t�che
        /// </summary>
        /// <returns>R�sultat de l'�xecution</returns>
        public bool Executer()
        {
            return Tache();
        }

        /// <summary>
        /// Impl�mentation de IComparable, voir surcharge int CompareTo(ActionRobot)
        /// </summary>
        public int CompareTo(object obj)
        {
            return CompareTo(obj as ActionRobot);
        }
        
        /// <summary>
        /// Compare la priorit� de l'action � celle d'une autre action en tenant compte des condition d'�xecution
        /// </summary>
        /// <param name="autre">Action � comparer</param>
        /// <returns>R�sultat de la comparaison ({ -1; 0; 1 })</returns>
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
