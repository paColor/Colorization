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
using System.Text;

namespace ColorLib
{
    public class LetterButtonModifiedEventArgs : EventArgs
    {
        public int buttonNr { get; private set; }

        public LetterButtonModifiedEventArgs(int inBNr)
        {
            buttonNr = inBNr;
        }
    }

    [Serializable]
    public class PBDQConfig : ConfigBase
    {
        public const char inactiveLetter = ' '; // letter used to determinde that the button is inactive.
        public const int nrButtons = 8;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Event Handlers ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand un des boutons est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<LetterButtonModifiedEventArgs> LetterButtonModifiedEvent;

        /// <summary>
        /// Evènement déclenché <c>markAsBlack</c> est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler MarkAsBlackModifiedEvent;

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------- Public Members  ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Indique si les lettres qui ne doivent pas être marquées doivent être laissées telles
        /// quelles <c>false</c> ou mises en noir <c>true</c>.
        /// </summary>
        public bool markAsBlack { get; private set; }

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------- Private Members  ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private Dictionary<char, CharFormatting> bpdqCF; // the mapping letter - CharFormmating
        private char[] selLetters; // the letters that are associated to each button. inactiveLetter means no letter
        private CharFormatting defaultCF; // the CF that is returned for a non selected letter.

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------------  Methods -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public PBDQConfig()
        {
            bpdqCF = new Dictionary<char, CharFormatting>(nrButtons);
            selLetters = new char[nrButtons];
            Reset();
        }

        /// <summary>
        /// <c><see cref="CharFormatting"/> pour la lettre <paramref name="c"/>.</c>
        /// </summary>
        /// <param name="c">La lettre pour laquelle on cherche le <see cref="CharFormatting"/></param>
        /// <returns>Le <see cref="CharFormatting"/> recherché ou <c>null</c> si la lettre n'est pas
        /// trouvée.</returns>
        public CharFormatting GetCfForPBDQLetter(char c)
        {
            CharFormatting toReturn = null;
            if (!bpdqCF.TryGetValue(c, out toReturn))
                toReturn = defaultCF; 
            return toReturn;
        }

        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> et la lettre choisie pour le bouton <paramref name="i"/>. 
        /// </summary>
        /// <param name="i">Numéro du bouton dont on veut le <see cref="CharFormatting"/> et la lettre.
        /// 0 &lt= <c>i</c> &lt <c>nrButtons</c></param>
        /// <param name="c">Retourne la lettre traitée par le bouton <paramref name="i"/>.</param>
        /// <returns>Le <see cref="CharFormatting"/> attribué au bouton <paramref name="i"/> ou <c>null</c>
        /// s'il n'existe pas pour le bouton en question.</returns>
        public CharFormatting GetCfForPBDQButton(int i, out char c)
        {
            CharFormatting toReturn = null;
            c = selLetters[i];
            return GetCfForPBDQLetter(c);
        }

        /// <summary>
        /// Retourne la lettre pour le bouton <paramref name="butNr"/>.
        /// </summary>
        /// <param name="butNr">Le numéro du bouton dont on veut connaître la lettre.
        /// 0 &lt= <c>butNr</c> &lt <c>nrButtons</c></param>
        /// <returns></returns>
        public char GetLetterForButtonNr(int butNr) => selLetters[butNr];

        /// <summary>
        /// Met à jour la configuration du bouton <c>buttonNr</c> avec <c>cf</c>.
        /// </summary>
        /// <param name="buttonNr">Le nr du bouton pour lequel il y a un nouveau <c>cf</c></param>
        /// <param name="cf">Le nouveau <c>CharFormatting</c> pour le bouton.</param>
        /// <returns><c>false</c> si la lettre du bouton en question est égale à la lettre inactive (' '). A ce 
        /// moment-là, rien n'est fait. <c>true</c> si la modification a été effectuée avec succès.</returns>
        public bool UpdateLetter(int buttonNr, CharFormatting cf)
        {
            logger.ConditionalTrace("UpdateLetter bouton no {0}", buttonNr);
            bool toReturn = false;
            char c = selLetters[buttonNr];
            if (c != inactiveLetter)
            {
                toReturn = true;
                bpdqCF[c] = cf;
                OnLetterButtonModifed(new LetterButtonModifiedEventArgs(buttonNr));
            }
            return toReturn;
        }

        /// <summary>
        /// Met à jour la configuration pour le bouton <c>buttonNr</c>. Si le caractère <c>c</c> est déjà utilisé
        /// pour un autre bouton, la modification est refusée et la méthode retourne <c>false</c>. Si <c>c</c> est 
        /// le caractère inactif ' ', le bouton est "effacé". 
        /// </summary>
        /// <param name="buttonNr">Identifie le bouton à modifier par son numéro.</param>
        /// <param name="c">Le caractère pour le bouton.</param>
        /// <param name="cf">Le nouveau <c>CharFormatting</c> pour le bouton</param>
        /// <returns><c>false</c> si la lettre n'a pas pu être mise à jour, par exemple parce qu'elle est
        /// déjà traitée.</returns>
        public bool UpdateLetter (int buttonNr, char c, CharFormatting cf)
            // returns false if the update could not be executed because the letter is already handled.
        {
            logger.ConditionalTrace("UpdateLetter buttonNr: {0}, c: \'{1}\'", buttonNr, c);
            bool toReturn = true;
            char previousC = selLetters[buttonNr];

            if (c != inactiveLetter)
            {
                if (previousC != c)
                {
                    if (!bpdqCF.ContainsKey(c))
                    {
                        if (previousC != inactiveLetter)
                            bpdqCF.Remove(previousC);
                        bpdqCF[c] = cf;
                        selLetters[buttonNr] = c;
                    }
                    else
                    {
                        // bpdqCF.ContainsKey(c) i.e. the letter is already present
                        toReturn = false;
                    }
                }
                else
                {
                    // previousC == c
                    bpdqCF[c] = cf;
                }
            }
            else
            {
                // c == inactiveLetter
                selLetters[buttonNr] = inactiveLetter; // neutral character inactiveLetter
                if (previousC != inactiveLetter)
                    bpdqCF.Remove(previousC);
            }
            if (toReturn)
                OnLetterButtonModifed(new LetterButtonModifiedEventArgs(buttonNr));
            logger.ConditionalTrace("END UodateLetter toReturn: {0}", toReturn.ToString());
            return toReturn;
        } // UpdateLetter

        /// <summary>
        /// Modifie la valeur du flag <c>markAsBlack</c>.
        /// </summary>
        /// <param name="val">Nouvelle valeur du flag.</param>
        public void BlackSettingCheckChangedTo(bool val)
        {
            if (markAsBlack != val) { // on s'assure qu'il ne peut y avoir de boucle pour toujours remettre la même valeur.
                markAsBlack = val;
                OnMarkAsBlackModified();
                if (markAsBlack)
                    defaultCF = ColConfWin.predefCF[(int)PredefCols.black];
                else
                    defaultCF = ColConfWin.predefCF[(int)PredefCols.neutral];
            }
        }

        public override void Reset()
        {
            BlackSettingCheckChangedTo(false);
            bpdqCF.Clear();
            bpdqCF.Add(inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]);
            UpdateLetter(0, 'b', ColConfWin.predefCF[(int)PredefCols.red]);
            UpdateLetter(1, 'p', ColConfWin.predefCF[(int)PredefCols.darkGreen]);
            UpdateLetter(2, 'd', ColConfWin.predefCF[(int)PredefCols.pureBlue]);
            UpdateLetter(3, 'q', ColConfWin.predefCF[(int)PredefCols.brown]);
            UpdateLetter(4, inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]);
            UpdateLetter(5, inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]);
            UpdateLetter(6, inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]);
            UpdateLetter(7, inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]); ;
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------  On Events -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        protected virtual void OnLetterButtonModifed (LetterButtonModifiedEventArgs e)
        {
            logger.ConditionalTrace(BaseConfig.cultF, "OnLetterButtonModifed, buttonNr: {0}", e.buttonNr);
            EventHandler<LetterButtonModifiedEventArgs> eventHandler = LetterButtonModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnMarkAsBlackModified()
        {
            logger.ConditionalTrace("OnMarkAsBlackModified");
            EventHandler eventHandler = MarkAsBlackModifiedEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
