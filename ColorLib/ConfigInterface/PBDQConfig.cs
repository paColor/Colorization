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
    public delegate void ExecTaskOnLetterButton(int buttonNr);

    [Serializable]
    public class PBDQConfig
    {
        public const char inactiveLetter = ' '; // letter used to determinde that the button is inactive.

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [NonSerialized] public ExecuteTask updateLetterButtons; // { set; private get; }
        [NonSerialized] public ExecTaskOnLetterButton updateLetterButton; // { set; private get; }
        
        public bool markAsBlack { get; private set; }
        // indicates whether non selected letters should be left as they are (false) or marked black (true)

        private Dictionary<char, CharFormatting> bpdqCF; // the mapping letter - CharFormmating
        private char[] selLetters; // the letters that are associated to each button. inactiveLetter means no letter
        private CharFormatting defaultCF; // the CF that is returned for a non selected letter.
        

        public PBDQConfig()
        {
            bpdqCF = new Dictionary<char, CharFormatting>(8);
            selLetters = new char[8];
            defaultCF = ColConfWin.predefCF[(int)PredefCols.neutral];
                // Ne peut pas être null, sinon les options avancées ne fonctionneraient plus.
                // On peut se demander si elles sont vraiment nécessaires :-)
            markAsBlack = false;
            InitStandardColors();
        }

        public CharFormatting GetCfForPBDQLetter(char c)
            // returns null if no formatting should be applied.
        {
            CharFormatting toReturn = null;
            if (!bpdqCF.TryGetValue(c, out toReturn))
                toReturn = defaultCF; 
            return toReturn;
        }

        public CharFormatting GetCfForPBDQButton(int i, out char c)
        // returns null if no formatting is applied to the corresponding button.
        {
            CharFormatting toReturn = null;
            c = selLetters[i];
            return GetCfForPBDQLetter(c);
        }

        public char GetLetterForButtonNr(int butNr) => selLetters[butNr];

        public bool UpdateLetter (int buttonNr, char c, CharFormatting cf)
            // returns false if the update could not be executed because the letter is already handled.
        {
            logger.ConditionalTrace("UodateLetter buttonNr: {0}, c: \'{1}\'", buttonNr, c);
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
            {
                updateLetterButton(buttonNr);
                //markSelLetters();
            }
            logger.ConditionalTrace("END UodateLetter toReturn: {0}", toReturn.ToString());
            return toReturn;
        } // UpdateLetter

        public void BlackSettingCheckChangedTo(bool val)
        {
            markAsBlack = val;
            if (markAsBlack)
                defaultCF = ColConfWin.predefCF[(int)PredefCols.black];
            else
                defaultCF = ColConfWin.predefCF[(int)PredefCols.neutral];
            //markSelLetters();
        }

        public void ResetStandardColors()
        {
            InitStandardColors();
            updateLetterButtons();
            //markSelLetters();
        }

        private void InitStandardColors()
        {
            bpdqCF.Clear();
            bpdqCF.Add('b', ColConfWin.predefCF[(int)PredefCols.red]);
            bpdqCF.Add('p', ColConfWin.predefCF[(int)PredefCols.darkGreen]);
            bpdqCF.Add('d', ColConfWin.predefCF[(int)PredefCols.pureBlue]);
            bpdqCF.Add('q', ColConfWin.predefCF[(int)PredefCols.brown]);
            bpdqCF.Add(inactiveLetter, ColConfWin.predefCF[(int)PredefCols.neutral]);
            selLetters[0] = 'b';
            selLetters[1] = 'p';
            selLetters[2] = 'd';
            selLetters[3] = 'q';
            selLetters[4] = inactiveLetter;
            selLetters[5] = inactiveLetter;
            selLetters[6] = inactiveLetter;
            selLetters[7] = inactiveLetter;
        }
    }
}
