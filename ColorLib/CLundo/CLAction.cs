using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// L'enregistrement d'une action qui a été exécutée. Elle peut être annulée (Undo) ou
    /// réexécutée (Redo).
    /// </summary>
    public abstract class CLAction
    {
        public string Name { get; private set; }

        public CLAction(string inName)
        {
            Name = inName;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Annule l'action.
        /// </summary>
        public virtual void Undo()
        {

        }

        /// <summary>
        /// Réexécute l'action.
        /// </summary>
        public virtual void Redo()
        {

        }

    }
}
