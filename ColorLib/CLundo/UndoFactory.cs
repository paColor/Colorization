using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// nombre de listes d'action imbriquées.
        /// </summary>
        private static int recDepth = 0;

        /// <summary>
        /// si > 0 aucune action n'est enregistrée pour un possible undo. 
        /// </summary>
        private static int disableRegistering;

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
        /// Empêche que les actions soient enregistrées pour pouvoir ensuite être annulées.
        /// Fonctionne en paire avec <see cref="EnableUndoRegistration"/> qui doit être appelé
        /// quand les actions peuvent à nouveau être enregistrées.
        /// </summary>
        /// <remarks>Les appels de cette méthode peuvent être imbriqués. Mais un
        /// <see cref="EnableUndoRegistration"/> doit correspondre à un <c>Disable</c></remarks>
        public static void DisableUndoRegistration()
        {
            disableRegistering++;
        }

        /// <summary>
        /// Réactive l'enregistrement des actions. Un appel doit obligatoirement avoir été précédé
        /// d'un appel à <see cref="DisableUndoRegistration"/>.
        /// </summary>
        public static void EnableUndoRegistration()
        {
            disableRegistering--;
            Debug.Assert(disableRegistering >= 0);
        }

        /// <summary>
        /// Enregistre le fait que l'action <paramref name="act"/> est exécutée.
        /// </summary>
        /// <param name="act">L'action exécutée.</param>
        public static void ExceutingAction(CLAction act)
        {
            logger.ConditionalTrace("ExceutingAction {0}, undoing: {1}", act.Name, undoing);
            if (!undoing && disableRegistering == 0)
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
            logger.ConditionalTrace(undoStack.ToString());
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
        /// <remarks>Attention à ce que le couple <see cref="StartRecording"/> 
        /// <see cref="EndRecording"/> se trouve à l'intérieur d'un couple 
        /// <see cref="DisableUndoRegistration"/> <see cref="EnableUndoRegistration"/></remarks>
        /// <param name="name">Le nom de l'action groupée.</param>
        public static void StartRecording(string name)
        {
            logger.ConditionalTrace("StartRecording {0}, undoing: {1}", name, undoing);
            if (!undoing && disableRegistering == 0)
            {
                if (actList != null)
                {
                    recDepth++;
                }
                else
                {
                    actList = new CLActionList(name);
                }
            }
        }

        /// <summary>
        /// Termine l'enregistrement d'une liste d'actions. Le pendant à 
        /// <see cref="StartRecording"/>.
        /// </summary>
        public static void EndRecording()
        {
            logger.ConditionalTrace("EndRecording depth: {0} undoing: {1}", recDepth, undoing);
            if (!undoing && disableRegistering == 0)
            {
                if (actList != null)
                {
                    if (recDepth > 0)
                    {
                        recDepth--;
                    }
                    else
                    {
                        undoStack.Push(actList);
                        actList = null;
                        OnUndoCountModified();
                    }
                }
                else
                {
                    throw new InvalidOperationException("EndRecording alors qu'il semble ne pas y avoir eu de Start correspondant.");
                }
            }
        }

        /// <summary>
        /// Efface la mémoire des actions effectuées.
        /// </summary>
        public static void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
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
