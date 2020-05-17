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
using Microsoft.Office.Interop.Word;
using ColorLib;
using ColorizationControls;
using System.Windows.Forms;

namespace ColorizationWord
{
    public class MSWText : TheText
    {

        private static RGB[] hilightColors = new RGB[HilightForm.nrColors]
        {
            new RGB(255,255,0),    // wdYellow
            new RGB(0,255,0),      // wdBrightGreen 
            new RGB(0,255,255),    // wdTurquoise 
            new RGB(255,0,255),    // wdPink 
            new RGB(0,0,255),      // wdBlue 
            new RGB(255,0,0),      // wdRed 
            new RGB(255,255,255),  // wdWhite 
            new RGB(0,0,128),      // wdDarkBlue 
            new RGB(0,128,128),    // wdTeal
            new RGB(0,128,0),      // wdGreen 
            new RGB(128,0,128),    // wdViolet 
            new RGB(128,0,0),      // wdDarkRed 
            new RGB(128,128,0),    // wdDarkYellow 
            new RGB(128,128,128),  // wdGray50 
            new RGB(192,192,192),  // wdGray25 
            new RGB(0,0,0),        // wdBlack
        };

        private static Dictionary<RGB, WdColorIndex> mapRGBtoColI = new Dictionary<RGB, WdColorIndex>(HilightForm.nrColors)
        {
            { hilightColors[0] , WdColorIndex.wdYellow },
            { hilightColors[1] , WdColorIndex.wdBrightGreen },
            { hilightColors[2] , WdColorIndex.wdTurquoise },
            { hilightColors[3] , WdColorIndex.wdPink },
            { hilightColors[4] , WdColorIndex.wdBlue },
            { hilightColors[5] , WdColorIndex.wdRed },
            { hilightColors[6] , WdColorIndex.wdWhite },
            { hilightColors[7] , WdColorIndex.wdDarkBlue },
            { hilightColors[8] , WdColorIndex.wdTeal },
            { hilightColors[9] , WdColorIndex.wdGreen },
            { hilightColors[10] , WdColorIndex.wdViolet },
            { hilightColors[11] , WdColorIndex.wdDarkRed },
            { hilightColors[12] , WdColorIndex.wdDarkYellow },
            { hilightColors[13] , WdColorIndex.wdGray50 },
            { hilightColors[14] , WdColorIndex.wdGray25 },
            { hilightColors[15] , WdColorIndex.wdBlack },
        };

        private Range range;
        private int rgStart;
        private Range rgeWork; // je ne sais pas quel est le coût en terme de performance de l'accès 
        // à range.Duplicate et range.Start. Dans le doute ne le faisons qu'une fois...

        public static void Initialize()
        {
            TheText.Init();
            ConfigControl.Init();
            HilightForm.hiliColors = hilightColors;
        }

        public static void ApplyCFToRange(CharFormatting cf, Range toR, Config inConf)
        {
            if (cf.bold)
                toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
            else if (cf.ForceNonBold(inConf))
                toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoFalse;

            if (cf.italic)
                toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
            else if (cf.ForceNonItalic(inConf))
                toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoFalse;

            if (cf.underline)
                toR.Underline = WdUnderline.wdUnderlineSingle;
            else if (cf.ForceNonUnderline(inConf))
                toR.Underline = WdUnderline.wdUnderlineNone;

            // if (cte.cf.caps) // capitalize
            // TBD

            if (cf.changeColor) // set new color
                toR.Font.Fill.ForeColor.RGB = cf.color;
            else if (cf.ForceBlackColor(inConf))
                toR.Font.Fill.ForeColor.RGB = ColConfWin.predefinedColors[(int)PredefCols.black];

            if (cf.changeHilight)
            {
                WdColorIndex wdCi;
                if (mapRGBtoColI.TryGetValue(cf.hilightColor, out wdCi))
                    toR.HighlightColorIndex = wdCi;
            }
            else if (cf.ForceHilightClear(inConf))
                toR.HighlightColorIndex = WdColorIndex.wdNoHighlight;
        }

        public MSWText(Range rge, Config inConf)
            : base(GetStringFor(rge), inConf)
        {
            this.range = rge;
            rgStart = rge.Start;
            rgeWork = range.Duplicate;

            // Debugging help
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(String.Format("this.ToString(): {0}", this.ToString()));
            //sb.AppendLine(String.Format("rgeSize (end - start): {0}", rge.End - rge.Start));
            //sb.AppendLine(String.Format("range.Text.Length: {0}", rge.Text.Length));
            //sb.AppendLine(String.Format("this.S.Length: {0}", this.S.Length));
            //MessageBox.Show(sb.ToString());
        }

        protected override void SetChars(FormattedTextEl fte)
        {
            rgeWork.SetRange(rgStart + fte.First, rgStart + fte.Last + 1); // End pointe sur le caractère qui suit le range...
            ApplyCFToRange(fte.cf, rgeWork, this.GetConfig());
        } 

        private static string GetStringFor(Range rge)
            // dans le cas où il a un objet attaché au texte, il y a un caractère pour marquer l'ancrage.
            // ce caractère n'est pas dans "Text". On le construit donc ici avec un caractère "vide" à la 
            // place du caractère spécial.
        {           
            const string empty = " ";
            string chText;
            Range itR = rge.Duplicate;
            int length = rge.End - rge.Start;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < rge.End - rge.Start; i++)
            {
                int start = rge.Start + i;
                itR.SetRange(start, start + 1);
                chText = itR.Text;
                if ((String.IsNullOrEmpty(chText)) || (chText.Length > 1))
                    chText = empty;
                sb.Append(chText);
            }
            return sb.ToString();
        }
    }
}
