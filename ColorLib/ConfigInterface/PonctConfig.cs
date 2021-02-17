﻿/********************************************************************************
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
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace ColorLib
{

    public class PonctModifiedEventArgs : EventArgs
    {
        public Ponctuation p;
        public string pName;

        public PonctModifiedEventArgs(Ponctuation inPonct)
        {
            p = inPonct;
            pName = p.ToString();
        }
    }

    /// <summary>
    /// Contient la configuration pour chacune des familles de symboles de ponctuation définies
    /// dans <see cref="PonctInT"/>.
    /// <para>On considère qu'il existe un bouton pour chacune des familles et un bouton général qui permet
    /// de les définir tous. Ce dernier peut être dans les états master ou off.</para>
    /// </summary>
    [Serializable]
    public class PonctConfig : ConfigBase
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
                if (_masterCF != value || MasterState == State.off)
                {
                    _masterCF = value;
                    MasterState = State.master;
                    for (Ponctuation p = Ponctuation.firstP + 1; p < Ponctuation.lastP; p++)
                    {
                        charFormats[p] = MasterCF;
                        OnPonctFormattingModified(p);
                        checkBoxes[p] = true;
                        OnPonctCBModified(p);
                    }
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
                    OnMasterStateModified();
                }
            }
        }

        public CharFormatting MajDebCF
        {
            get
            {
                return _majDebCF;
            }
            set
            {
                if (_majDebCF != value)
                {
                    _majDebCF = value;
                    OnMajDebCFModified();
                }
            }
        }

        public bool MajDebCB
        {
            get
            {
                return _majDebCB;
            }
            set
            {
                if (_majDebCB != value)
                {
                    _majDebCB = value;
                    OnMajDebCBModified();
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

        [OptionalField(VersionAdded = 7)]
        private CharFormatting _majDebCF;

        [OptionalField(VersionAdded = 7)]
        private bool _majDebCB;

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

        // ----------------------------------------------------------------------------------------
        // ------------------------------------  Public Methods -----------------------------------
        // ----------------------------------------------------------------------------------------

        public PonctConfig()
        {
            for (Ponctuation p = Ponctuation.firstP + 1; p < Ponctuation.lastP; p++)
            {
                charFormats[p] = CharFormatting.NeutralCF;
                OnPonctFormattingModified(p);
                checkBoxes[p] = false;
                OnPonctCBModified(p);
            }
            Reset();
        }

        // <summary>
        /// Réinitialise à la configuration par défaut
        /// </summary>
        public override void Reset()
        {
            logger.ConditionalDebug("Reset");
            MasterCF = new CharFormatting(ColConfWin.coloredCF[(int)PredefCol.pinky], true, false, false);
            MajDebCF = MasterCF;
            MajDebCB = false;
        }

        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> pour la famille de caractères
        /// <paramref name="p"/>.
        /// </summary>
        /// <param name="p">La famille de caractères de ponctuation dont on veut le CF.</param>
        /// <returns>Le <see cref="CharFormatting"/> souhaité.</returns>
        public CharFormatting GetCF(Ponctuation p) => charFormats[p];

        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> à utiliser pour le formatage des caractères.
        /// Tient compte de la checkbox pour déterminer le <see cref="CharFormatting"/> retourné.
        /// </summary>
        /// <remarks>Retourne <see cref="CharFormatting.NeutralCF"/> si la checkbox correspondante
        /// est à <c>false</c>.</remarks>
        /// <param name="p">La famille de signes pour laquelle on veut le 
        /// <see cref="CharFormatting"/></param>
        /// <returns>le <see cref="CharFormatting"/> à utiliser.</returns>
        public CharFormatting GetCFtoApply(Ponctuation p)
        {
            if (checkBoxes[p])
            {
                return charFormats[p];
            }
            else
            {
                return CharFormatting.NeutralCF;
            }
        }

        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> pour la famille de caractères identifiée par
        /// son nom <paramref name="ponct"/>. Le nom doit correspondre exactement à la valeur de
        /// l'énuméré dans <see cref="Ponctuation"/>.
        /// </summary>
        /// <param name="ponct">Le nom de la famille de caractères tel que défini dans 
        /// <see cref="Ponctuation"/>.</param>
        /// <returns></returns>
        public CharFormatting GetCF(string ponct) => GetCF(PonctInT.Ponct4String(ponct));

        public void SetCF(Ponctuation p, CharFormatting toCF)
        {
            logger.ConditionalDebug("SetCF {0} to {1}", p.ToString(), toCF.ToString());
            if (toCF != charFormats[p])
            {
                charFormats[p] = toCF;
                OnPonctFormattingModified(p);
                MasterState = State.off;
            }
        }

        public void SetCF(string ponct, CharFormatting toCF) => SetCF(PonctInT.Ponct4String(ponct), toCF);

        /// <summary>
        /// Définit le <see cref="CharFormatting"/> pour la famille de signes donnée et met
        /// également la Checkbox corrspondante à <c>true</c>.
        /// </summary>
        /// <param name="ponctS">La famille de signes visée.</param>
        /// <param name="toCF">Le nouveau <see cref="CharFormatting"/>.</param>
        public void SetCFandCB(string ponctS, CharFormatting toCF)
        {
            Ponctuation p = PonctInT.Ponct4String(ponctS);
            SetCF(p, toCF);
            SetCB(p, true);
        }

        public bool GetCB(Ponctuation p) => checkBoxes[p];

        public bool GetCB(string ponct) => GetCB(PonctInT.Ponct4String(ponct));

        public void SetCB(Ponctuation p, bool toCB)
        {
            logger.ConditionalDebug("SetCF {0} to {1}", p.ToString(), toCB);
            if (toCB != checkBoxes[p])
            {
                checkBoxes[p] = toCB;
                OnPonctCBModified(p);
                MasterState = State.off;
            }
        }

        public void SetCB(string ponct, bool toCB) => SetCB(PonctInT.Ponct4String(ponct), toCB);

        /// <summary>
        /// Est utilisé par <see cref="CharFormatForm"/> qui réclame une fonction (delegate) avec
        /// cette signature.
        /// </summary>
        /// <param name="dummy">N'est pas utilisé.</param>
        /// <param name="cf">le <see cref="CharFormatting"/> auquel <c>MasterCF</c> doit être
        /// mis.</param>
        public void SetMasterCF(string dummy, CharFormatting cf) => MasterCF = cf;

        /// <summary>
        /// Efface le CharFormatting pour la famille de signes indiquée. La checkbox correspondante
        /// est également mise à <c>false</c>.
        /// </summary>
        /// <param name="ponctS">La famille de signes à effacer.</param>
        public void ClearPonct(string ponctS)
        {
            Ponctuation p = PonctInT.Ponct4String(ponctS);
            SetCF(p, CharFormatting.NeutralCF);
            SetCB(p, false);
        }

        /// <summary>
        /// Efface le maître. Désactive tout formatage des signes.
        /// </summary>
        public void ClearMaster()
        {
            MasterCF = CharFormatting.NeutralCF;
            MasterState = State.off;
            for (Ponctuation p = Ponctuation.firstP + 1; p < Ponctuation.lastP; p++)
            {
                checkBoxes[p] = false;
                OnPonctCBModified(p);
            }
        }

        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            MajDebCF = MasterCF;
            MajDebCB = false;
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

    }
}
