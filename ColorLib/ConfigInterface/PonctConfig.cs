/********************************************************************************
 *  Copyright 2020-2021, Pierre-Alain Etique                                    *
 *                                                                              *
 *  This file is part of Coloriƨation.                                          *
 *                                                                              *
 *  Coloriƨation is free software: you can redistribute it and/or modify        *
 *  it under the terms of the GNU General Public License as published by        *
 *  the Free Software Foundation, either version 3 of the License, or           *
 *  (at your option) any later version.                                         *
 *                                                                              *
 *  Coloriƨation is distributed in the hope that it will be useful,             *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *  GNU General Public License for more details.                                *
 *                                                                              *
 *  You should have received a copy of the GNU General Public License           *
 *  along with Coloriƨation.  If not, see <https://www.gnu.org/licenses/>.      *
 *                                                                              *
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        // -----------------------------------  Public  Members  ----------------------------------
        // ----------------------------------------------------------------------------------------

        public CharFormatting MasterCF
        {
            get
            {
                return _masterCF;
            }

            set
            {
                if (_masterCF != value)
                {
                    _masterCF = value;
                    for (Ponctuation p = Ponctuation.firstP; p < Ponctuation.lastP; p++)
                    {
                        SetCF(p, value);
                    }
                    MasterState = State.master;
                    OnMasterCFModified();
                }
            }
        }

        public State MasterState
        {
            get
            {
                return _masterState;
            }

            set
            {
                if (_masterState != value)
                {
                    _masterState = value;
                    if (value == State.master)
                    {
                        for (Ponctuation p = Ponctuation.firstP; p < Ponctuation.lastP; p++)
                        {
                            SetCF(p, MasterCF);
                        }
                        MasterCheckBox = true;
                    }
                    else if (value == State.off)
                    {
                        MasterCheckBox = false;
                        OnMasterCBModified();
                        for (Ponctuation p = Ponctuation.firstP; p < Ponctuation.lastP; p++)
                        {
                            SetCB(p, false);
                        }
                    }
                    OnMasterStateModified();
                }
            }
        }

        public bool MasterCheckBox { 
            get
            {
                return _masterCheckBox;
            } 

            set
            {
                if (MasterCheckBox != value)
                {
                    _masterCheckBox = value;
                    for (Ponctuation p = Ponctuation.firstP; p < Ponctuation.lastP; p++)
                    {
                        SetCB(p, value);
                    }
                    MasterState = State.master;
                    OnMasterCBModified();
                }
            }
        }






        // ----------------------------------------------------------------------------------------
        // -----------------------------------  Private Members  ----------------------------------
        // ----------------------------------------------------------------------------------------

        private Dictionary<Ponctuation, CharFormatting> charFormats
            = new Dictionary<Ponctuation, CharFormatting>((int)Ponctuation.lastP);
        private Dictionary<Ponctuation, bool> checkBoxes
            = new Dictionary<Ponctuation, bool>((int)Ponctuation.lastP);

        private CharFormatting _masterCF;
        private State _masterState;
        private bool _masterCheckBox;

        // ----------------------------------------------------------------------------------------
        // ------------------------------------  Event Handlers -----------------------------------
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand le <see cref="CharFormatting"/> d'un signe de ponctuation 
        /// est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PonctModifiedEventArgs> PonctFormattingModified;

        /// <summary>
        /// Evènement déclenché quand la checkbox d'un signe de ponctuation est modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PonctModifiedEventArgs> PonctCBModified;

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

        // ----------------------------------------------------------------------------------------
        // ------------------------------------  Public Methods -----------------------------------
        // ----------------------------------------------------------------------------------------

        public PonctConfig()
        {
            Reset();
        }

        // <summary>
        /// Réinitialise à la configuration par défaut
        /// </summary>
        public override void Reset()
        {
            for (Ponctuation p = Ponctuation.firstP; p < Ponctuation.lastP; p++)
            {
                charFormats[p] = CharFormatting.NeutralCF;
                checkBoxes[p] = false;
            }
            _masterCF = CharFormatting.NeutralCF;
            _masterState = State.master;
            _masterCheckBox = false;
        }

        public CharFormatting GetCF(Ponctuation p) => charFormats[p];

        public void SetCF(Ponctuation p, CharFormatting toCF)
        {
            if (toCF != charFormats[p])
            {
                charFormats[p] = toCF;
                OnPonctFormattingModified(p);
                if (MasterState == State.master && MasterCF != toCF)
                {
                    MasterState = State.off;
                }
            }
        }

        public bool GetCB(Ponctuation p) => checkBoxes[p];

        public void SetCB(Ponctuation p, bool toCB)
        {
            if (toCB != checkBoxes[p])
            {
                checkBoxes[p] = toCB;
                OnPonctCBModified(p);
                if (MasterState == State.master && MasterCheckBox != toCB)
                {
                    MasterState = State.off;
                }
            }
        }


        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
        }

        // ------------------------------------------ Events --------------------------------------

        protected virtual void OnPonctFormattingModified(Ponctuation p)
        {
            logger.ConditionalDebug(ConfigBase.cultF, "OnPonctFormattingModified ponctuation: \'{0}\'", p);
            EventHandler<PonctModifiedEventArgs> eventHandler = PonctFormattingModified;
            eventHandler?.Invoke(this, new PonctModifiedEventArgs(p));
        }

        protected virtual void OnPonctCBModified(Ponctuation p)
        {
            logger.ConditionalDebug(ConfigBase.cultF, "OnPonctCBModified ponctuation: \'{0}\'", p);
            EventHandler<PonctModifiedEventArgs> eventHandler = PonctCBModified;
            eventHandler?.Invoke(this, new PonctModifiedEventArgs(p));
        }

        protected virtual void OnMasterCFModified()
        {
            logger.ConditionalDebug("OnMasterCFModified");
            EventHandler eventHandler = MasterCFModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMasterStateModified()
        {
            logger.ConditionalDebug("OnMasterStateModified");
            EventHandler eventHandler = MasterStateModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMasterCBModified()
        {
            logger.ConditionalDebug("OnMasterCBModified");
            EventHandler eventHandler = MasterCBModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
