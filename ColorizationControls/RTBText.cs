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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorLib;

namespace ColorizationControls
{
    /// <summary>
    /// Classe descendant de <see cref="TheText"/> pour gérer le formatage du texte dans une
    /// <c>RichTextBox</c>
    /// </summary>
    class RTBText : TheText
    {
        // ****************************************************************************************
        // *                               private static members                                 *
        // ****************************************************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static List<int> finDeLignesStatic;

        // ****************************************************************************************
        // *                                    private members                                   *
        // ****************************************************************************************

        /// <summary>
        /// La <see cref="RichTextBox"/> sur laquelle est construit le <c>RTBText</c>.
        /// </summary>
        private RichTextBox rtb;

        /// <summary>
        /// Liste des positions de fin de ligne (dans S) à retourner dans <see cref="GetLastLinesPos"/>.
        /// </summary>
        private List<int> finDeLignes;

        // ****************************************************************************************
        // *                                     public methods                                   *
        // ****************************************************************************************

        public RTBText(RichTextBox theRTB)
            :base(theRTB.Text)
        {
            logger.ConditionalDebug("RTBText");
            rtb = theRTB;
            logger.ConditionalTrace(rtb.Text);
            finDeLignes = new List<int>();
            if (theRTB.Multiline)
            {
                int lineStart = 0;
                string[] theLines = theRTB.Lines;
                for (int i = 0; i < theLines.Length; i++)
                {
                    int eol = lineStart + theLines[i].Length - 1;
                    finDeLignes.Add(eol);
                    lineStart = eol + 2;
                }
            }
#if DEBUG
            for (int i = 0; i < finDeLignes.Count; i++)
                logger.ConditionalTrace("finDeLignes[{0}] == {1}", i, finDeLignes[i]);
#endif
        }

        // ****************************************************************************************
        // *                                   protected methods                                  *
        // ****************************************************************************************

        protected override List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            return finDeLignes;
        }

        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            logger.ConditionalTrace("SetChars");
            rtb.Select(fte.First, fte.Last - fte.First + 1);
            ApplyCFToSelection(fte.cf, conf);
        }

        // ****************************************************************************************
        // *                                     private methods                                  *
        // ****************************************************************************************

        private void ApplyCFToSelection(CharFormatting cf, Config inConf)
        {
            Font currentFont = rtb.SelectionFont;
            bool bold, italic, underline;
            if (cf.bold)
                bold = true;
            else if (cf.ForceNonBold(inConf))
                bold = false;
            else
                bold = currentFont.Bold;

            if (cf.italic)
                italic = true;
            else if (cf.ForceNonItalic(inConf))
                italic = false;
            else
                italic = currentFont.Italic;

            if (cf.underline)
                underline = true;
            else if (cf.ForceNonUnderline(inConf))
                underline = false;
            else
                underline = currentFont.Underline;

            rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, 
                StaticColorizControls.GetFontStyle(bold, italic, underline));

            if (cf.changeColor) // set new color
                rtb.SelectionColor = cf.color;
            else if (cf.ForceBlackColor(inConf))
                rtb.SelectionColor = ColConfWin.predefinedColors[(int)PredefCol.black];
        }
    }
}
