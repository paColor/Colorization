/********************************************************************************
 *  Copyright 2020, Pierre-Alain Etique                                         *
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
    public class SylButtonModifiedEventArgs : EventArgs
    {
        public int buttonNr { get; private set; }

        public SylButtonModifiedEventArgs(int inBNr)
        {
            buttonNr = inBNr;
        }
    }

    /// <summary>
    /// Configuration pour les commandes demandant un formatage alterné comme le marquage des syllabes, des mots
    /// des lignes...
    /// </summary>
    [Serializable]
    public class SylConfig: ConfigBase
    {
        public enum Mode { ecrit, oral, poesie, undefined }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Serializable]
        public struct SylButtonConf
        {
            public bool buttonClickable;
            public CharFormatting cf;
        }

        public const int nrButtons = 6;

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Event Handlers ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand le bouton pour une syllabe est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SylButtonModifiedEventArgs> SylButtonModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand le mode doubleConsStd est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler DoubleConsStdModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand le mode écrit, oral ou poesie est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ModeModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand le marquage des muettes est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler MarquerMuettesModified;

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Public Members ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <value>Indique si les </value>
        public bool DoubleConsStd
        {
            get
            {
                return _doubleConsStd;
            }
            set
            {
                if (_doubleConsStd != value) // pour ne déclencher l'évèenement que s'il y a vraiement un changement.
                {
                    _doubleConsStd = value;
                    OnDoubleConsStdModified();
                }
            }
        }

        /// <value>Le mode à utiliser pour le marquage des syllabes.</value>
        public Mode mode
        {
            get
            {
                if (_modeEcrit)
                    return Mode.ecrit;
                else if (_modeOral)
                    return Mode.oral;
                else if (_modePoesie)
                    return Mode.poesie;
                else
                    return Mode.undefined;
            }
            set
            {
                switch (value)
                {
                    case Mode.ecrit:
                        _modeEcrit = true;
                        _modeOral   = false;
                        _modePoesie = false;
                        break;
                    case Mode.oral:
                        _modeEcrit = false;
                        _modeOral = true;
                        _modePoesie = false;
                        break;
                    case Mode.poesie:
                        _modeEcrit = false;
                        _modeOral = false;
                        _modePoesie = true;
                        break;
                    case Mode.undefined:
                        _modeEcrit = false;
                        _modeOral = false;
                        _modePoesie = false;
                        break;
                    default:
                        logger.Error("Mode de traitement des syllabes impossible.");
                        throw new ArgumentException("Mode de traitement des syllabes impossible.");
                        break;
                }
                OnModeModified();
            }
        }

        public bool marquerMuettes
        {
            get
            {
                return _marquerMuettes;
            }
            set
            {
                if (value != _marquerMuettes)
                {
                    _marquerMuettes = value;
                    OnMarquerMuettesModified();
                }
            }
        }


        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Private Members ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private bool _doubleConsStd; // définit si les syllabes sont coupées entre deux consonnes doublées.
        private bool _modeEcrit; // définit si une syllabe muette en fin de mot doit être considérée comme une syllabe
                                // Dans LireCouleur, on parle de mode oral (la syllabe finale est fusionnée avec la précédente) ou écrit.

        private SylButtonConf[] sylButtons;
        private int nrSetButtons;
        private int counter;

        /// <summary>
        /// indique si le mode oral est activé. Ne peut pas être vrai en même temps que _modeEcrit
        /// ou modePoesie.
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        private bool _modeOral;

        /// <summary>
        /// indique si le mode poésie est activé. Ne peut pas être vrai en même temps que _modeEcrit
        /// ou modePoesie.
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        private bool _modePoesie;

        /// <summary>
        /// indique si les muettes doivent être marquées.
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        private bool _marquerMuettes;


        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Public Methods ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public SylConfig()
        {
            sylButtons = new SylButtonConf[nrButtons];
            Reset();
        }

        public bool ButtonIsActivableOne(int butNr) => butNr == nrSetButtons;

        public bool ButtonIsLastActive(int butNr) => butNr == nrSetButtons - 1;

        public SylButtonConf GetSylButtonConfFor(int butNr) => sylButtons[butNr];

        /// <summary>
        /// Indique que le le boouton <paramref name="butNr"/> (commence à 0) doit être formatté avec 
        /// <paramref name="inCf"/>.
        /// La fonction <c>updateSylButton</c> est appelée pour le bouton <paramref name="butNr"/> (l'événement
        /// correspondant est généré...)
        /// </summary>
        /// <param name="butNr">Le numéro du bouton dont il faut changer le formatage.</param>
        /// <param name="inCf">Le nouveau formatage.</param>
        public void SylButtonModified(int butNr, CharFormatting inCf)
        {
            logger.ConditionalDebug("SylButtonModified butNr: {0}", butNr);
            Debug.Assert(butNr <= nrSetButtons);
            sylButtons[butNr].cf = inCf;
            if (butNr == nrSetButtons)
            {
                nrSetButtons++;
                if (nrSetButtons < nrButtons)
                {
                    sylButtons[nrSetButtons].buttonClickable = true;
                    OnSylButtonModified(nrSetButtons);
                }
                if (inCf.changeColor == false)
                {
                    // c'est un problème. Il faut une couleur, sinon l'expérience utilisateur n'est pas consistante.
                    // mettons le bouton à noir.
                    sylButtons[butNr].cf = new CharFormatting(inCf, ColConfWin.predefinedColors[(int)PredefCols.black]);
                }
            }
            OnSylButtonModified(butNr);
        }

        public void SylButtonModified(string butNrTxt, CharFormatting inCf)
        {
            SylButtonModified(int.Parse(butNrTxt), inCf);
        }

        /// <summary>
        /// Le bouton <paramref name="butNr"/> est effacé. N'est possible que pour le dernier bouton
        /// formaté de la série.
        /// </summary>
        /// <exception cref="ArgumentException"> est levée si le bouton ne peut pas être effacé.</exception>
        /// <param name="butNr">Le numéro du bouton à effacer.</param>
        public void ClearButton(int butNr)
        {
            logger.ConditionalDebug("ClearButton butNr: {0}", butNr);
            if ((butNr == (nrSetButtons - 1)) && (nrSetButtons > 0))
            {
                sylButtons[nrSetButtons].buttonClickable = false;
                OnSylButtonModified(nrSetButtons);
                nrSetButtons--;
                sylButtons[nrSetButtons].cf = ColConfWin.predefCF[(int)PredefCols.neutral];
                OnSylButtonModified(nrSetButtons);
            }
            else
            {
                Exception e = new ArgumentException("The Button Number cannot be cleared", "butNr");
                logger.Error(e, "The Button number {0} cannot be cleared. nrSetButtons: {1}", butNr, nrSetButtons);
                throw e;
            }
        }
        
        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> à utiliser en alternance suivant la configuration actuelle.
        /// Assure de passer à travers les différents <c>CharFormatting</c> voulus.
        /// </summary>
        /// <remarks>
        /// Si aucun bouton n'a été défini, retourne le <c>CharFormatting</c> neutre.
        /// </remarks>
        /// <returns>Le <see cref="CharFormatting"/> à utiliser pour un formatage alterné.</returns>
        public CharFormatting NextCF()
        {
            CharFormatting toReturn = sylButtons[counter].cf;
            if (nrSetButtons > 0)
                counter = (counter + 1) % nrSetButtons;
            else
                counter = 0;
            return toReturn;
        }

        /// <summary>
        /// Réinitialise le compteur utilisé par <see cref="NextCF"/> pour retourner les formatages aletrnés. 
        /// Permet de s'assurer que la série retournée commence par le formatage du premier bouton.
        /// </summary>
        public void ResetCounter() => counter = 0;

        /// <summary>
        /// Réinitialise à la configuration par défaut (rouge et noir)
        /// </summary>
        public override void Reset()
        {
            for (int i = 2; i < nrButtons; i++)
            {
                sylButtons[i].buttonClickable = false;
                sylButtons[i].cf = ColConfWin.predefCF[(int)PredefCols.neutral];
                OnSylButtonModified(i);
            }
            sylButtons[0].buttonClickable = true;
            nrSetButtons = 0;
            SylButtonModified(0, ColConfWin.predefCF[(int)PredefCols.pureBlue]);
            SylButtonModified(1, ColConfWin.predefCF[(int)PredefCols.red]);
            ResetCounter();
            DoubleConsStd = true; // mode std de LireCouleur
            mode = Mode.ecrit;
            marquerMuettes = true;
        }

        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            mode = Mode.ecrit;
            marquerMuettes = true;
        }

        internal override void PostLoadInitOptionalFields()
        {
            logger.ConditionalDebug("PostLoadInitOptionalFields");
            if (mode == Mode.undefined) // legacy: _modeEcrit == false
            {
                mode = Mode.oral;
                marquerMuettes = false;
            }
        }

        // ------------------------------------------ Events --------------------------------------

        protected virtual void OnSylButtonModified(int buttonNr)
        {
            logger.ConditionalDebug(BaseConfig.cultF, "OnSylButtonModified bouton: \'{0}\'", buttonNr);
            EventHandler<SylButtonModifiedEventArgs> eventHandler = SylButtonModifiedEvent;
            eventHandler?.Invoke(this, new SylButtonModifiedEventArgs(buttonNr));
        }

        protected virtual void OnDoubleConsStdModified()
        {
            logger.ConditionalDebug("OnDoubleConsStdModified");
            EventHandler eventHandler = DoubleConsStdModifiedEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnModeModified()
        {
            logger.ConditionalDebug("OnModeModified");
            EventHandler eventHandler = ModeModifiedEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMarquerMuettesModified()
        {
            logger.ConditionalDebug("OnMarqueMuettesModified");
            EventHandler eventHandler = MarquerMuettesModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

    }
}
