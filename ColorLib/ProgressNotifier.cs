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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("progress: ");
            sb.Append(progress.ToString(ConfigBase.cultF));
            sb.Append(", elapsedMilliseconds: ");
            sb.Append(elapsedMilliseconds.ToString(ConfigBase.cultF));
            sb.Append(", remainingMilliseconds: ");
            sb.Append(remainingMilliseconds.ToString(ConfigBase.cultF));
            return sb.ToString();
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("elapsedMilliseconds: ");
            sb.Append(elapsedMilliseconds.ToString(ConfigBase.cultF));
            return sb.ToString();
        }
    }

    /// <summary>
    /// Sert d'interface entre la couche de présentation et <c>ColorLib</c> en ce qui concerne le 
    /// progrès fait par l'exécution d'une opération.
    /// </summary>
    /// <remarks>
    /// <para>
    /// J'avais conçu ceci à l'origine avec l'idée qu'il pourrait y avoir plusieurs fenêtres dans
    /// lesquelles on pourrait exécuter une colorisation en parallèle. Cependant ce n'est pas
    /// possible dans Office et pas vraiment non plus sous Windows.Forms.
    /// </para>
    /// <para>
    /// On peut donc partir sur une vision simple avec un seul <c>ProgressNotifier</c> qu'on
    /// trouve sous la variable statique <c>ProgressNotifier.thePN</c>.
    /// </para>
    /// <para>
    /// La classe n'est pas statique car il y a des événements et qu'ils ont besoin d'un objet
    /// émetteur.
    /// </para>
    /// </remarks>
    public class ProgressNotifier
    {

        // ****************************************************************************************
        // ****************************************************************************************
        // *                                                                                      *
        // * --------------------------------------- STATIC ------------------------------------- *
        // *                                                                                      *
        // ****************************************************************************************
        // ****************************************************************************************

        public static ProgressNotifier thePN { get; private set; } = new ProgressNotifier();

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        // ****************************************************************************************
        // ****************************************************************************************
        // *                                                                                      *
        // * ------------------------------------ INSTANTIATED ---------------------------------- *
        // *                                                                                      *
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
        /// Evènement déclenché quand la tâche est terminée.                                                        
        /// </summary>
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
            logger.ConditionalTrace(ConfigBase.cultF, "InProgress {0}%", progression);
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
                invSpeed = (invSpeed + instantInvSpeed + instantInvSpeed + instantInvSpeed) / 4;
            }
            previousTime = e;
            previousProgress = progress;
            long estRemainingT = invSpeed * (100 - progress);
            logger.ConditionalTrace("OnProgressEvent, progress: {0}, elapsed: {1}, remaining: {2}",
                progress, e, estRemainingT);
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
