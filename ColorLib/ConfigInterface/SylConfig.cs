/********************************************************************************
 *  Copyright 2020 - 2021, Pierre-Alain Etique                                  *
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

using NLog;
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
    /// contient les informations pour la fenêtre d'édition des exceptions.
    /// </summary>
    [Serializable]
    public class ExceptionMots
    {
        /// <summary>
        /// Le texte dans la fenêtre d'édition.
        /// </summary>
        public string texte;

        /// <summary>
        /// Les mots contenus dans <c>texte</c>.
        /// </summary>
        public HashSet<string> exceptMots;

        /// <summary>
        /// Les checkboxes
        /// </summary>
        public bool syllabes;
        public bool mots;
        public bool arcs;
        public bool phonemes;
        public bool lettres;
        public bool voyCons;
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

        /// <summary>
        /// Nombre de boutons gérés. 
        /// </summary>
        public const int NrButtons = 6;

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

        /// <summary>
        /// Evènement déclenché la recherche de diérèseest modifiée.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ChercherDiereseModified;

        /// <summary>
        /// Evènement déclenché quand le nombre de pieds à considérer est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler NbrPiedsModified;

        /// <summary>
        /// Evènement déclenché quand les exceptions sont modifiées.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ExcMotsModified;

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
                if (_doubleConsStd != value) // pour ne déclencher l'évènement que s'il y a vraiement un changement.
                {
                    UndoFactory.ExceutingAction(new SylAction("Double consonne", SylAction.SylActionType.doubleCons,
                        this, DoubleConsStd, value));
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
                Mode prevMode = mode;
                switch (value)
                {
                    case Mode.ecrit:
                        _modeEcrit = true;
                        _modeOral = false;
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
                // pour ne pas interférer avec l'initialisation de 'mode', on choisit cette approche
                // pour éviter les boucles d'événements
                if (value != prevMode)
                {
                    UndoFactory.ExceutingAction(new SylAction("Mode syllabes", this, prevMode, 
                        value));
                    OnModeModified();
                }
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
                    UndoFactory.ExceutingAction(new SylAction("Marquer muettes", 
                        SylAction.SylActionType.marquerMuettes,
                        this, marquerMuettes, value));
                    _marquerMuettes = value;
                    OnMarquerMuettesModified();
                }
            }
        }

        public bool chercherDierese
        {
            get
            {
                return _chercherDierese;
            }
            set
            {
                if (value != _chercherDierese)
                {
                    UndoFactory.ExceutingAction(new SylAction("Chercher diérèse", SylAction.SylActionType.dierese,
                        this, chercherDierese, value));
                    _chercherDierese = value;
                    OnChercherDiereseModified();
                }
            }
        }

        public int nbrPieds
        {
            get
            {
                return _nbrPieds;
            }
            set
            {
                if (value != _nbrPieds)
                {
                    UndoFactory.ExceutingAction(new SylAction("Nombre de pieds", this, nbrPieds,
                        value));
                    _nbrPieds = value;
                    OnNbrPiedsModified();
                }
            }
        }

        public ExceptionMots ExcMots
        {
            get { return _excMots; }
            set
            {
                UndoFactory.ExceutingAction(new SylAction("Exceptions", this, ExcMots, value));
                _excMots = value;
                OnExcMotsModified();
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

        // ------------------------------  Traitement de la diérèse -------------------------------

        /// <summary>
        /// Indique s'il y a lieu de chercher les diérèses en mode poésie
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        private bool _chercherDierese;

        /// <summary>
        /// Indique le nombre de pieds que devraient contenir les vers. '0' veut dire recherche
        /// automatique.
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        private int _nbrPieds;

        // ---------------------------------------- Exceptions --------------------------------------

        [OptionalField(VersionAdded = 9)]
        private ExceptionMots _excMots; // null si aucune exception n'a jamais été créée.


        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Public Methods ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public SylConfig()
        {
            UndoFactory.DisableUndoRegistration();
            sylButtons = new SylButtonConf[NrButtons];
            for (int i = 0; i < NrButtons; i++)
            {
                sylButtons[i].buttonClickable = false;
                sylButtons[i].cf = CharFormatting.NeutralCF;
            }
            sylButtons[0].buttonClickable = true;
            nrSetButtons = 0;
            _excMots = null;
            Reset();
            UndoFactory.EnableUndoRegistration();
        }

        /// <summary>
        /// Indique s'il s'agit du bouton qui n'est pas encore actif, mais qu'on peut configurer.
        /// </summary>
        /// <param name="butNr"></param>
        /// <returns></returns>
        public bool ButtonIsActivableOne(int butNr) => butNr == nrSetButtons;

        /// <summary>
        /// Indique si le bouton donné est le dernier bouton actif, c.à.d. si c'est celui qui peut
        /// être effacé.
        /// </summary>
        /// <param name="butNr">Le bouton à analyser.</param>
        /// <returns><c>true</c>: le bouton est le dernier actif.</returns>
        public bool ButtonIsLastActive(int butNr) => butNr == nrSetButtons - 1;

        /// <summary>
        /// Donne la <c>SylButtonConf</c> pour <c>butNr</c>.
        /// </summary>
        /// <param name="butNr">Le numéro du boutonpour lequel on veut connaître la conf.</param>
        /// <returns>La <c>SylButtonConf</c> cherchée.</returns>
        public SylButtonConf GetSylButtonConfFor(int butNr) => sylButtons[butNr];

        /// <summary>
        /// Indique que le le boouton <paramref name="butNr"/> (commence à 0) doit être formatté avec 
        /// <paramref name="inCf"/>.
        /// La fonction <c>updateSylButton</c> est appelée pour le bouton <paramref name="butNr"/> (l'événement
        /// correspondant est généré...)
        /// </summary>
        /// <param name="butNr">Le numéro du bouton dont il faut changer le formatage.</param>
        /// <param name="inCf">Le nouveau formatage.</param>
        public void SetSylButtonCF(int butNr, CharFormatting inCf)
        {
            logger.ConditionalDebug("SylButtonModified butNr: {0}", butNr);
            if (butNr > nrSetButtons)
            {
                logger.Fatal("Modification d'un bouton non actif butNr: {0}", butNr);
                throw new ArgumentException("Modification d'un bouton non actif.", nameof(butNr));
            }
            UndoFactory.ExceutingAction(new SylAction("Formatage bout. syll.", this, butNr,
                nrSetButtons, sylButtons[butNr].cf, inCf));
            sylButtons[butNr].cf = inCf;
            if (butNr == nrSetButtons)
            {
                nrSetButtons++;
                if (nrSetButtons < NrButtons)
                {
                    sylButtons[nrSetButtons].buttonClickable = true;
                    OnSylButtonModified(nrSetButtons);
                }
                if (inCf.changeColor == false)
                {
                    // c'est un problème. Il faut une couleur, sinon l'expérience utilisateur n'est pas consistante.
                    // mettons le bouton à noir.
                    sylButtons[butNr].cf = new CharFormatting(inCf, ColConfWin.predefinedColors[(int)PredefCol.black]);
                }
            }
            OnSylButtonModified(butNr);
        }

        /// <summary>
        /// Définit un nouveau <c>CharFormatting</c> pour le bouton indiqué.
        /// </summary>
        /// <param name="butNrTxt">Le numéro du boutton.</param>
        /// <param name="inCf">Le nouveau <c>CharFormatting</c> pour le bouton.</param>
        public void SetSylButtonCF(string butNrTxt, CharFormatting inCf)
        {
            SetSylButtonCF(int.Parse(butNrTxt), inCf);
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
                UndoFactory.ExceutingAction(new SylAction("Effacer bout. syll.", this, butNr, sylButtons[butNr].cf));
                if (nrSetButtons < NrButtons)
                {
                    sylButtons[nrSetButtons].buttonClickable = false;
                    OnSylButtonModified(nrSetButtons);
                }
                nrSetButtons--;
                sylButtons[nrSetButtons].cf = CharFormatting.NeutralCF;
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
        /// <remarks><c>ExcMots</c> n'est pas réinitialisé. Il faut appeler une autre méthode pour
        /// cela.</remarks>
        public void ResetCounter() => counter = 0;

        /// <summary>
        /// Réinitialise à la configuration par défaut (rouge et noir)
        /// </summary>
        public override void Reset()
        {
            UndoFactory.StartRecording("Réinitialiser syllabes");
            for (int i = nrSetButtons - 1; i >= 2; i--)
            {
                ClearButton(i);
            }
            SetSylButtonCF(0, ColConfWin.coloredCF[(int)PredefCol.pureBlue]);
            SetSylButtonCF(1, ColConfWin.coloredCF[(int)PredefCol.red]);
            ResetCounter();
            DoubleConsStd = true; // mode std de LireCouleur
            mode = Mode.ecrit;
            marquerMuettes = true;
            chercherDierese = true;
            nbrPieds = 0; // Par défaut, mode automatique.
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Réinitialise les exceptions à "aucune exception".
        /// </summary>
        public void ResetExceptionMots()
        {
            ExcMots = null;
        }

        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            mode = Mode.ecrit;
            marquerMuettes = true;
            chercherDierese = true;
            nbrPieds = 0;
            _excMots = null;
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
            logger.ConditionalDebug(ConfigBase.cultF, "OnSylButtonModified bouton: \'{0}\'", buttonNr);
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

        protected virtual void OnChercherDiereseModified()
        {
            logger.ConditionalDebug("OnChercherDiereseModified");
            EventHandler eventHandler = ChercherDiereseModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnNbrPiedsModified()
        {
            logger.ConditionalDebug("OnNbrPiedsModified");
            EventHandler eventHandler = NbrPiedsModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExcMotsModified()
        {
            logger.ConditionalDebug("OnExcMotsModified");
            EventHandler eventHandler = ExcMotsModified;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }

    }
}
