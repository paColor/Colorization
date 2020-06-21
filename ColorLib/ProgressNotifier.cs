using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ColorLib
{

    /// <summary>
    /// Argument passé lors de l'évènement <c>ConfigResetttedEvent</c>.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public enum ProgressType { start, inProgress, finished, undefined};
        
        /// <value>L'état en % de la progression</value>
        public int progress { get; private set; }

        /// <value>Le temps écoulé depuis le début de l'exécution de la tâche.</value>
        public long elapsedMilliseconds { get; private set; }

        /// <value>Le type d'évènement. Il est à noter que la valeur de <c>progress</c> n'est
        /// intéressante que dans le cas où <c>progressType</c> == <c>inProgress</c>.</value>
        public ProgressType progressType { get; private set; }

        /// <summary>
        /// Crée une instance d'arguments pour l'évènement.
        /// </summary>
        /// <param name="p">L'état en % de la progression</param>
        /// <param name="e">Le temps écoulé depuis le début de l'exécution de la tâche.</param>
        public ProgressEventArgs(ProgressType pt, int p, long e)
        {
            progress = p;
            elapsedMilliseconds = e;
            progressType = pt;
        }
    }

    /// <summary>
    /// Sert d'interface entre la couche de présentation et <c>ColorLib</c> en ce qui concerne le 
    /// progrès fait par l'exécution d'une opération.
    /// </summary>
    /// <remarks>
    /// Le flux logique est le suivant:
    /// <list type="number">
    /// <item>
    /// <description> La partie responsable du UI, définit une classe qui s'occupe de traîter l'affichage de 
    /// la progression. Par exemple une fenêtre avec une barre de progression.</description>
    /// </item>
    /// <item>
    /// <description>
    /// Cette entité crée un <c>ProgressNotifier</c> et elle se connecte à ses évènements.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Le <c>ProgressNotifier</c> est passé à la méthode qui exécute la tâche.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// La méthode appelle <see cref="Start"/>, <see cref="InProgress(int)"/> et <see cref="Finished"/>
    /// sur le <c>ProgressNotifier</c> qu ilui a été passé.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Le UI traite les évènements et informe l'utilsateur du progrès.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ProgressNotifier
    {

        // ****************************************************************************************
        // ****************************************************************************************
        // *                                                                                                                   *
        // * --------------------------------------- STATIC ------------------------------------- *
        // *                                                                                                                   *
        // ****************************************************************************************
        // ****************************************************************************************


        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        // ****************************************************************************************
        // ****************************************************************************************
        // *                                                                                                                   *
        // * ------------------------------------ INSTANTIATED ---------------------------------- *
        // *                                                                                                                   *
        // ****************************************************************************************
        // ****************************************************************************************

        // --------------------------------- EventHandlers ----------------------------------

        /// <summary>
        /// Evènement déclenché quand une évolution de la progression est connue.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        // ----------------------------------- Members -------------------------------------

        private Stopwatch stopwatch;

        // ----------------------------------- Methods -------------------------------------

        /// <summary>
        /// Crée un <c>ProgressNotifier</c>
        /// </summary>
        public ProgressNotifier()
        {
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Démarrage de la tâche.
        /// </summary>
        public void Start()
        {
            logger.ConditionalDebug("Start");
            stopwatch.Restart();
            OnProgressEvent(ProgressEventArgs.ProgressType.start, 0);
        }

        /// <summary>
        /// Indique la progression de la tâche.
        /// </summary>
        /// <remarks>Autant que possible, ne pas appeler cette méthode plus d'une fois par pourcent.</remarks>
        /// <param name="progression">en pourcents (0 - 100) l'état de progression de la tâche.</param>
        public void InProgress(int progression)
        {
            logger.ConditionalTrace(BaseConfig.cultF, "InProgress {0}%", progression);
            OnProgressEvent(ProgressEventArgs.ProgressType.inProgress, progression);
        }

        /// <summary>
        /// Tâche terminée
        /// </summary>
        public void Finished()
        {
            logger.ConditionalDebug("Finished");
            stopwatch.Stop();
            OnProgressEvent(ProgressEventArgs.ProgressType.finished, 100);
        }

        protected virtual void OnProgressEvent(ProgressEventArgs.ProgressType pt, int progress)
        {
            EventHandler<ProgressEventArgs> eventHandler = ProgressEvent;
            eventHandler?.Invoke(this, new ProgressEventArgs(pt, progress, stopwatch.ElapsedMilliseconds));
        }
    }
}
