using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    public static class UndoFactory
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static CLActionStack undoStack = new CLActionStack();
        private static CLActionStack redoStack = new CLActionStack();
        private static CLActionList actList = null;
        /// <summary>
        /// Indique si on est en train d'exécuter un undo. Dans ce cas, il ne faut pas
        /// enregistrer les actions.
        /// </summary>
        private static bool undoing = false; 

        public static int UndoCount => undoStack.Count;
        public static int RedoCount => redoStack.Count;

        public static event EventHandler UndoCountModified;

        public static event EventHandler RedoCountModified;



        /// <summary>
        /// Enregistre le fait que l'action <paramref name="act"/> est exécutée.
        /// </summary>
        /// <param name="act">L'action exécutée.</param>
        public static void ExceutingAction(CLAction act)
        {
            logger.ConditionalTrace("ExceutingAction {0}", act.Name);
            if (!undoing)
            {
                if (actList != null)
                {
                    actList.Add(act);
                }
                else
                {
                    undoStack.Push(act);
                    OnUndoCountModified();
                }
            }
        }

        /// <summary>
        /// Exécute l'annulation de la dernière action enregistrée. Ne fait rien s'il n'y a pas de
        /// dernière action à annuler.
        /// </summary>
        public static void UndoLastAction()
        {
            logger.ConditionalDebug("UndoLastAction");
            CLAction act = undoStack.Pop();
            if (act != null)
            {
                undoing = true;
                act.Undo();
                OnUndoCountModified();
                redoStack.Push(act);
                OnRedoCountModified();
                undoing = false;
            }
        }

        public static void RedoLastCanceledAction()
        {
            logger.ConditionalDebug("RedoLastCanceledAction");
            CLAction act = redoStack.Pop();
            if (act != null)
            {
                act.Redo();
                OnRedoCountModified();
            }
        }

        /// <summary>
        /// Les actions qui seront enregistrées jusqu'à l'appel de <see cref="EndRecording"/> 
        /// seront regroupées dans une seule action qui pourra être annulée en une fois.
        /// </summary>
        /// <param name="name">Le nom de l'action groupée.</param>
        public static void StartRecording(string name)
        {
            if (actList != null)
            {
                EndRecording();
            }
            actList = new CLActionList(name);
        }

        public static void EndRecording()
        {
            if (actList != null)
            {
                undoStack.Push(actList);
                actList = null;
                OnUndoCountModified();
            }
            else
            {
                throw new InvalidOperationException("EndRecording alors qu'il semble ne pas y avoir eu de Start correspondant.");
            }
        }

        private static void OnUndoCountModified()
        {
            EventHandler eventHandler = UndoCountModified;
            eventHandler?.Invoke(undoStack, EventArgs.Empty);
        }

        private static void OnRedoCountModified()
        {
            EventHandler eventHandler = RedoCountModified;
            eventHandler?.Invoke(redoStack, EventArgs.Empty);
        }
    }
}
