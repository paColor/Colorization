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
        /// <summary>
        /// Lettre que traite un bouton inactif, c'est-à-dire pour lequel aucun
        /// <see cref="CharFormatting"/> ne sera retourné.
        /// </summary>
        public const char inactiveLetter = ' '; // letter used to determinde that the button is inactive.

        /// <summary>
        /// Nombre de boutons géré par un objet <c>PBDQConfig</c>.
        /// </summary>
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
        /// <remarks>
        /// <para>
        /// Peut être modifié par la méthode <see cref="SetMarkAsBlackTo"/>.
        /// On n'a pas modifié le 'setter' pour ne pas se poser de questions sur la compatibilité
        /// avec les anciennes versions des fichiers de sauvegarde. 
        /// </para>
        /// <para>
        /// Seule la culeur est mise à noir. Les autres caractéristiques comme 'bold' ne sont
        /// pas touchées.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Crée un <c>PBDQConfig</c>. <see cref="Reset"/> est appliqué.
        /// </summary>
        public PBDQConfig()
        {
            bpdqCF = new Dictionary<char, CharFormatting>(nrButtons);
            selLetters = new char[nrButtons];
            markAsBlack = false;
            defaultCF = CharFormatting.NeutralCF;
            Reset();
        }

        /// <summary>
        /// <see cref="CharFormatting"/> pour la lettre <paramref name="c"/>.
        /// </summary>
        /// <param name="c">La lettre pour laquelle on cherche le <see cref="CharFormatting"/></param>
        /// <returns>Le <see cref="CharFormatting"/> recherché. Si aucun <c>CharFormatting</c>
        /// n'est géré pour <c>c</c>, retourne le <c>CharFormatting</c> à appliquer pour
        /// les autres lettres. Celui-ci dépend du flag <see cref="markAsBlack"/>; ls sera soit
        /// neutre ou impliquera un formattage noir.</returns>
        public CharFormatting GetCfForPBDQLetter(char c)
        {
            logger.ConditionalDebug("GetCfForPBDQLetter bouton with letter {0}", c);
            CharFormatting toReturn = null;
            if (!bpdqCF.TryGetValue(c, out toReturn))
                toReturn = defaultCF; 
            return toReturn;
        }

        /// <summary>
        /// Retourne le <see cref="CharFormatting"/> et la lettre choisie pour le bouton <paramref name="butNr"/>. 
        /// </summary>
        /// <param name="butNr">Numéro du bouton dont on veut le <see cref="CharFormatting"/> et la lettre.</param>
        /// <param name="c">Retourne la lettre traitée par le bouton <paramref name="butNr"/>. Si le 
        /// bouton est inutilisé, retourne <see cref="inactiveLetter"/>.</param>
        /// <returns>Le <see cref="CharFormatting"/> attribué au bouton <paramref name="butNr"/> ou <c>null</c>
        /// s'il n'existe pas pour le bouton en question.</returns>
        /// <exception cref="ArgumentException">Si <c>i</c> n'a pas une valeur entre <c>0</c> et
        /// <c>nrButtons - 1</c>.</exception>
        public CharFormatting GetCfForPBDQButton(int butNr, out char c)
        {
            logger.ConditionalDebug("GetCfForPBDQButton bouton no {0}", butNr);
            if (butNr < 0 || butNr > nrButtons - 1)
            {
                logger.Fatal("GetCfForPBDQButton - le bouton demandé n'existe pas: {0}", butNr);
                throw new ArgumentException("GetCfForPBDQButton - le bouton demandé n'existe pas", nameof(butNr));
            }
            c = selLetters[butNr];
            return GetCfForPBDQLetter(c);
        }

        /// <summary>
        /// Retourne la lettre traitée par le bouton <paramref name="butNr"/>.
        /// </summary>
        /// <param name="butNr">Le numéro du bouton dont on veut connaître la lettre.
        /// </param>
        /// <returns>La lettre pour le bouton <c>butNr</c></returns>
        /// <exception cref="ArgumentException">Si <c>butNr</c> n'a pas une valeur entre <c>0</c> et
        /// <c>nrButtons - 1</c>.</exception>
        public char GetLetterForButtonNr(int butNr)
        {
            logger.ConditionalDebug("GetLetterForButtonNr bouton no {0}", butNr);
            if (butNr < 0 || butNr > nrButtons - 1)
            {
                logger.Fatal("GetLetterForButtonNr - le bouton demandé n'existe pas: {0}", butNr);
                throw new ArgumentException("GetLetterForButtonNr - le bouton demandé n'existe pas", nameof(butNr));
            }
            return selLetters[butNr];
        }

        /// <summary>
        /// Met à jour la configuration du bouton <c>buttonNr</c> avec <c>cf</c>.
        /// </summary>
        /// <param name="butNr">Le nr du bouton pour lequel il y a un nouveau <c>cf</c></param>
        /// <param name="cf">Le nouveau <c>CharFormatting</c> pour le bouton.</param>
        /// <returns><c>false</c> si la lettre du bouton en question est égale à la lettre inactive (' '). A ce 
        /// moment-là, rien n'est fait. <c>true</c> si la modification a été effectuée avec succès.</returns>
        /// <exception cref="ArgumentException">Si <c>buttonNr</c> n'a pas une valeur entre <c>0</c> et
        /// <c>nrButtons - 1</c>.</exception>
        public bool UpdateLetter(int butNr, CharFormatting cf)
        {
            logger.ConditionalDebug("UpdateLetter bouton no {0}", butNr);
            if (butNr < 0 || butNr > nrButtons - 1)
            {
                logger.Fatal("UpdateLetter - le bouton indiqué n'existe pas: {0}", butNr);
                throw new ArgumentException("UpdateLetter - le bouton indiqué n'existe pas", nameof(butNr));
            }
            bool toReturn = false;
            char c = selLetters[butNr];
            if (c != inactiveLetter)
            {
                toReturn = true;
                bpdqCF[c] = cf;
                OnLetterButtonModifed(new LetterButtonModifiedEventArgs(butNr));
            }
            return toReturn;
        }

        /// <summary>
        /// Met à jour la configuration pour le bouton <c>buttonNr</c>. Si le caractère <c>c</c> est déjà utilisé
        /// pour un autre bouton, la modification est refusée et la méthode retourne <c>false</c>. Si <c>c</c> est 
        /// le caractère inactif ' ', le bouton est "effacé". 
        /// </summary>
        /// <param name="butNr">Identifie le bouton à modifier par son numéro.</param>
        /// <param name="c">Le caractère pour le bouton. <see cref="inactiveLetter"/> correspond
        /// à un effacement du bouton.</param>
        /// <param name="cf">Le nouveau <c>CharFormatting</c> pour le bouton</param>
        /// <returns><c>false</c> si la lettre n'a pas pu être mise à jour, par exemple parce qu'elle est
        /// déjà traitée.</returns>
        /// <exception cref="ArgumentException">Si <c>buttonNr</c> n'a pas une valeur entre <c>0</c> et
        /// <c>nrButtons - 1</c>.</exception>
        public bool UpdateLetter (int butNr, char c, CharFormatting cf)
        {
            logger.ConditionalDebug("UpdateLetter buttonNr: {0}, c: \'{1}\'", butNr, c);
            if (butNr < 0 || butNr > nrButtons - 1)
            {
                logger.Fatal("UpdateLetter - le bouton demandé n'existe pas: {0}, lettre \'{1}\'", butNr, c);
                throw new ArgumentException("UpdateLetter - le bouton demandé n'existe pas", nameof(butNr));
            }
            bool toReturn = true;
            char previousC = selLetters[butNr];

            if (c != inactiveLetter)
            {
                if (previousC != c)
                {
                    if (!bpdqCF.ContainsKey(c))
                    {
                        if (previousC != inactiveLetter)
                            bpdqCF.Remove(previousC);
                        bpdqCF[c] = cf;
                        selLetters[butNr] = c;
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
                selLetters[butNr] = inactiveLetter; // neutral character inactiveLetter
                if (previousC != inactiveLetter)
                    bpdqCF.Remove(previousC);
            }
            if (toReturn)
                OnLetterButtonModifed(new LetterButtonModifiedEventArgs(butNr));
            logger.ConditionalDebug("END UodateLetter toReturn: {0}", toReturn.ToString());
            return toReturn;
        } // UpdateLetter

        /// <summary>
        /// Modifie la valeur du flag <c>markAsBlack</c>.
        /// </summary>
        /// <param name="val">Nouvelle valeur du flag.</param>
        public void SetMarkAsBlackTo(bool val)
        {
            if (markAsBlack != val) { // on s'assure qu'il ne peut y avoir de boucle pour toujours remettre la même valeur.
                markAsBlack = val;
                OnMarkAsBlackModified();
                if (markAsBlack)
                    defaultCF = CharFormatting.BlackCF;
                else
                    defaultCF = CharFormatting.NeutralCF;
                bpdqCF[inactiveLetter] = defaultCF;
            }
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            
            bpdqCF.Clear();
            // bpdqCF.Add(inactiveLetter, defaultCF); - not needed since done in SetMarkAsBlackTo
            SetMarkAsBlackTo(false);
            UpdateLetter(0, 'b', ColConfWin.coloredCF[(int)PredefCol.red]);
            UpdateLetter(1, 'p', ColConfWin.coloredCF[(int)PredefCol.darkGreen]);
            UpdateLetter(2, 'd', ColConfWin.coloredCF[(int)PredefCol.pureBlue]);
            UpdateLetter(3, 'q', ColConfWin.coloredCF[(int)PredefCol.brown]);
            UpdateLetter(4, inactiveLetter, defaultCF);
            UpdateLetter(5, inactiveLetter, defaultCF);
            UpdateLetter(6, inactiveLetter, defaultCF);
            UpdateLetter(7, inactiveLetter, defaultCF); ;
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ------------------------------------------------  On Events -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Méthode appelée quand un des boutons est modifié. Elle déclenche l'évènement
        /// <see cref="LetterButtonModifiedEvent"/>.
        /// </summary>
        /// <param name="e">Les paramètres à joindre à l'évènement déclenché.</param>
        protected virtual void OnLetterButtonModifed (LetterButtonModifiedEventArgs e)
        {
            logger.ConditionalDebug(ConfigBase.cultF, "OnLetterButtonModifed, buttonNr: {0}", e.buttonNr);
            EventHandler<LetterButtonModifiedEventArgs> eventHandler = LetterButtonModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        /// <summary>
        /// Méthode appelée quand le flag <see cref="markAsBlack"/> est modifié. Déclenche
        /// l'évènement <see cref="MarkAsBlackModifiedEvent"/>.
        /// </summary>
        protected virtual void OnMarkAsBlackModified()
        {
            logger.ConditionalDebug("OnMarkAsBlackModified");
            EventHandler eventHandler = MarkAsBlackModifiedEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
