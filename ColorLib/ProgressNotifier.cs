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
        /// <value>L'état en % de la progression</value>
        public int progress { get; private set; }

        /// <value>Le temps écoulé depuis le début de l'exécution de la tâche.</value>
        public long elapsedMilliseconds { get; private set; }

        public long remainingMilliseconds { get; private set; }

        /// <summary>
        /// Crée une instance d'arguments pour l'évènement.
        /// </summary>
        /// <param name="p">L'état en % de la progression</param>
        /// <param name="e">Le temps écoulé depuis le début de l'exécution de la tâche (en 
        /// millisecondes).</param>
        /// <param name="r">Estimation du temps restant (en millisecondes).</param>
        public ProgressEventArgs(int p, long e, long r)
        {
            progress = p;
            elapsedMilliseconds = e;
            remainingMilliseconds = r;
        }
    }

    /// <summary>
    /// Crée une instance d'arguments pour l'évènement.
    /// </summary>
    /// <param name="e">Le temps écoulé depuis le début de l'exécution de la tâche.</param>
    public class CompletedEventArgs : EventArgs
    {
        /// <value>Le temps écoulé depuis le début de l'exécution de la tâche.</value>
        public long elapsedMilliseconds { get; private set; }

        public CompletedEventArgs(long e)
        {
            elapsedMilliseconds = e;
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
        /// Evènement déclenché quand la tâche commence.
        /// </summary>
        public event EventHandler<EventArgs> StartEvent;
        
        /// <summary>
        /// Evènement déclenché quand une évolution de la progression est connue.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressEvent;

        /// <summary>
        /// /// Evènement déclenché quand la tâche est terminée.                                                        /// </summary>
        public event EventHandler<CompletedEventArgs> CompletedEvent;


        // ----------------------------------- Members -------------------------------------

        private Stopwatch stopwatch;
        private long invSpeed; // miliseconds per percent
        private int previousProgress;
        private long previousTime;

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
            invSpeed = 10; // ça nous met la tâche standard à une seconde...
            previousProgress = 0;
            previousTime = 0;
            OnStartEvent();
        }

        /// <summary>
        /// Indique la progression de la tâche.
        /// </summary>
        /// <remarks>Autant que possible, ne pas appeler cette méthode plus d'une fois par pourcent.</remarks>
        /// <param name="progression">en pourcents (0 - 100) l'état de progression de la tâche.</param>
        public void InProgress(int progression)
        {
            logger.ConditionalTrace(BaseConfig.cultF, "InProgress {0}%", progression);
            OnProgressEvent(progression);
        }

        /// <summary>
        /// Tâche terminée
        /// </summary>
        public void Completed()
        {
            logger.ConditionalDebug("Completed in {0} milliseconds", stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();
            OnCompletedEvent();
        }

        protected virtual void OnStartEvent()
        {
            EventHandler<EventArgs> eventHandler = StartEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnProgressEvent(int progress)
        {
            long instantInvSpeed;
            int effectiveProgress = progress - previousProgress;
            long e = stopwatch.ElapsedMilliseconds;
            if (effectiveProgress > 0)
            {
                instantInvSpeed = (e - previousTime) / effectiveProgress;
                invSpeed = (invSpeed + instantInvSpeed + instantInvSpeed) / 3;
            }
            previousTime = e;
            previousProgress = progress;
            long estRemainingT = invSpeed * (100 - progress);
            logger.ConditionalTrace("OnProgressEvent)
            EventHandler <ProgressEventArgs> eventHandler = ProgressEvent;
            eventHandler?.Invoke(this, new ProgressEventArgs(progress, e, estRemainingT));
        }

        protected virtual void OnCompletedEvent()
        {
            EventHandler<CompletedEventArgs> eventHandler = CompletedEvent;
            eventHandler?.Invoke(this, new CompletedEventArgs(stopwatch.ElapsedMilliseconds));
        }
    }
}
