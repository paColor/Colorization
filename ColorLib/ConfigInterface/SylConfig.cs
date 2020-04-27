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
using System.Text;

namespace ColorLib
{
    public delegate void ExecTaskOnSylButton(int buttonNr);  

    public class SylConfig
    {
        public struct SylButtonConf
        {
            public bool buttonClickable;
            public CharFormatting cf;
        }

        public const int nrButtons = 6;

        public ExecuteTask updateSylButtons { set; private get; }
        public ExecTaskOnSylButton updateSylButton { set; private get; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool doubleConsStd; // définit si les syllabes sont coupées entre deux consonnes doublées.
        private bool modeEcrit; // définit si une syllabe muette en fin de mot doit être considérée comme une syllabe
                                // Dans LireCouleur, on parle de mode oral (la syllabe finale est fusionnée avec la précédente) ou écrit.

        private SylButtonConf[] sylButtons;
        private int nrSetButtons;
        private int counter;

        public SylConfig()
        {
            doubleConsStd = true; // mode std de LireCouleur
            modeEcrit = true; // mode écrit de LireCouleur
            sylButtons = new SylButtonConf[nrButtons];
            ResetCounter();
            InitStandardColors();
        }

        public bool DoubCons() => doubleConsStd;
        public bool ModeEcrit() => modeEcrit;

        // appelé quand la checkbox correspondante est modifiée. La nouvelle valeur pour "doubleConsStd" est dans val
        public void DoubleConsModified(bool val) => doubleConsStd = val;
        public void ModeEcritModified(bool val) => modeEcrit = val;

        public bool ButtonIsActivableOne(int butNr) => butNr == nrSetButtons;

        public bool ButtonIsLastActive(int butNr) => butNr == nrSetButtons - 1;

        public SylButtonConf GetSylButtonConfFor(int butNr) => sylButtons[butNr];

        public void SylButtonModified(int butNr, CharFormatting inCf)
        {
            sylButtons[butNr].cf = inCf;
            updateSylButton(butNr);
            if (butNr == nrSetButtons)
            {
                nrSetButtons++;
                if (nrSetButtons < nrButtons)
                {
                    sylButtons[nrSetButtons].buttonClickable = true;
                    updateSylButton(nrSetButtons);
                }
            }
        }

        public void SylButtonModified(string butNrTxt, CharFormatting inCf)
        {
            SylButtonModified(int.Parse(butNrTxt), inCf);
        }

        public void ClearButton(int butNr)
        {
            logger.ConditionalTrace("ClearButton butNr: {0}", butNr);
            if ((butNr == (nrSetButtons - 1)) && (nrSetButtons > 0))
            {
                sylButtons[nrSetButtons].buttonClickable = false;
                updateSylButton(nrSetButtons);
                nrSetButtons--;
                sylButtons[nrSetButtons].cf = ColConfWin.predefCF[(int)PredefCols.neutral];
                updateSylButton(nrSetButtons);
            }
            else
            {
                Exception e = new ArgumentException("The Button Number cannot be cleared", "butNr");
                logger.Error(e, "The Button number {0} cannot be cleared. nrSetButtons: {1}", butNr, nrSetButtons);
                throw e;
            }
        }
        
        public CharFormatting NextCF()
        {
            CharFormatting toReturn = sylButtons[counter].cf;
            counter = (counter + 1) % nrSetButtons;
            return toReturn;
        }

        public void ResetCounter() => counter = 0;

        
        public void ResetStandardColors()
        {
            InitStandardColors();
            updateSylButtons();
        }


        private void InitStandardColors()
        {
            sylButtons[0].buttonClickable = true;
            sylButtons[0].cf = ColConfWin.predefCF[(int)PredefCols.pureBlue];
            sylButtons[1].buttonClickable = true;
            sylButtons[1].cf = ColConfWin.predefCF[(int)PredefCols.red];
            sylButtons[2].buttonClickable = true;
            sylButtons[2].cf = ColConfWin.predefCF[(int)PredefCols.neutral];
            for (int i = 3; i < nrButtons; i++)
            {
                sylButtons[i].buttonClickable = false;
                sylButtons[i].cf = ColConfWin.predefCF[(int)PredefCols.neutral];
            }
            nrSetButtons = 2;
        }

    }
}
