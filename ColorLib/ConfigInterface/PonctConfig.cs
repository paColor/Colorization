using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{

    public class PonctModifiedEventArgs : EventArgs
    {
        public Ponctuation p;

        public PonctModifiedEventArgs(Ponctuation inPonct)
        {
            p = inPonct;
        }
    }



    /// <summary>
    /// Contient la configuration pour chacune des familles de symboles de ponctuation définies
    /// dans <see cref="PunctInT"/>.
    /// <para>On considère qu'il existe un bouton pour chacune des familles et un bouton général qui permet
    /// de les définir tous. Ce dernier peut être dans les états master ou off.</para>
    /// </summary>
    [Serializable]
    class PonctConfig : ConfigBase
    {
        [Serializable]
        public enum State { master, off, undef}

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ****************************************************************************************
        // **************************************** INSTANTIATED **********************************
        // ****************************************************************************************

        // ----------------------------------------------------------------------------------------
        // ------------------------------------  Event Handlers -----------------------------------
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand le <see cref="CharFormatting"/> d'un signe de ponctuation 
        /// est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PonctModifiedEventArgs> PonctFormattingModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand la checkbox d'un signe de ponctuation est modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PonctModifiedEventArgs> PonctCBModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand le CF maître est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler MasterCFModified;

        /// <summary>
        /// Evènement déclenché quand l'état du maître est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler MasterStateModified;

        /// <summary>
        /// Evènement déclenché quand la checkbox maître est modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler MasterCBModified;



        private Dictionary<Ponctuation, CharFormatting> charFormats
            = new Dictionary<Ponctuation, CharFormatting>((int)Ponctuation.lastP);
        private Dictionary<Ponctuation, bool> checkBoxes
            = new Dictionary<Ponctuation, bool>((int)Ponctuation.lastP);

        private CharFormatting masterCF;
        private State masterState;
        private bool masterCheckBox;

    }
}
