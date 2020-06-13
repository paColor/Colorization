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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;
using System.Windows.Forms;
using ColorLib;
using ColorizationControls;
using System.Diagnostics;

namespace Colorization
{
    public class PPTText : TheText
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Initialize()
        {
            TheText.Init();
            ConfigControl.Init();
        }

        private TextRange txtRange;

        /// <summary>
        /// Liste des positions de fin de ligne (dans S) à retourner dans <see cref="GetLastLinesPos"/>.
        /// </summary>
        private List<int> finDeLignes;

        public PPTText(TextRange txtRange)
            :base(txtRange.Text)
        {
            this.txtRange = txtRange;
            finDeLignes = null;
        }

        private static void ApplyCFToRange(CharFormatting cf, TextRange tRange, Config inConf)
        {
            if (cf.bold)
                tRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
            else if (cf.ForceNonBold(inConf))
                tRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoFalse;

            if (cf.italic)
                tRange.Font.Italic = Microsoft.Office.Core.MsoTriState.msoTrue;
            else if (cf.ForceNonItalic(inConf))
                tRange.Font.Italic = Microsoft.Office.Core.MsoTriState.msoFalse;

            if (cf.underline)
                tRange.Font.Underline = Microsoft.Office.Core.MsoTriState.msoTrue;
            else if (cf.ForceNonUnderline(inConf))
                tRange.Font.Underline = Microsoft.Office.Core.MsoTriState.msoFalse;

            //if (cf.caps) // capitalize
            //    tRange.Text = tRange.Text.ToUpper(BaseConfig.cultF);
            //else if (Config.ActiveConf().unsetBeh.CbuVal(Ucbx.caps))
            //    tRange.Text = tRange.Text.ToLower(BaseConfig.cultF);

            if (cf.changeColor) // set new color
                tRange.Font.Color.RGB = cf.color;
            else if (cf.ForceBlackColor(inConf))
                tRange.Font.Color.RGB = ColConfWin.predefinedColors[(int)PredefCols.black];
        }

        /// <summary>
        /// Applique le formatage voulu au <see cref="FormattedTextEl"/> sur l'affichage.
        /// </summary>
        /// <param name="fte">Le <see cref="FormattedTextEl"/> qui doit être formaté.</param>
        /// <param name="conf">La <see cref=">Config"/> à prendre en compte pour l'application du formatage.</param>
        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            TextRange theChars = txtRange.Characters(fte.First + 1, fte.Last - fte.First + 1);
            ApplyCFToRange(fte.cf, theChars, conf);
        }

        /// <summary>
        /// Retourne la liste des positions des derniers caractères de chaque ligne (dans S).
        /// </summary>
        /// <remarks>Utilise <c>finDeLignes</c> comme cache.</remarks>
        /// <returns>La liste des positions des derniers caractères de chaque ligne (dans S)</returns>
        protected override List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            if (finDeLignes == null)
            {
                logger.ConditionalTrace("txtRange.Start: {0}, txtRange.Length: {1}", txtRange.Start, txtRange.Length);
                finDeLignes = new List<int>(7);
                int i = 1;
                int startColoredLine = 0;
                TextRange theLine = txtRange.Lines(i);
                while ((theLine.Start > startColoredLine) && (theLine.Start + theLine.Length) <= (txtRange.Start + txtRange.Length))
                {
                    logger.ConditionalTrace("i: {0}, theLine - Start: {1}, Length: {2}", i, theLine.Start, theLine.Length);
                    if (theLine.Length > 0)
                    {
                        int endFormattedLine = theLine.Start + theLine.Length - 1;
                        finDeLignes.Add(endFormattedLine - txtRange.Start);
                    }
                    startColoredLine = theLine.Start;
                    i++;
                    theLine = txtRange.Lines(i);
                }
            }
            logger.ConditionalTrace("EXIT GetLastLinesPos");
            return finDeLignes;
        }

    } // class PPTText
} // namespace Colorization
