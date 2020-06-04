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
        private TextRange txtRange;

        public static void Initialize()
        {
            TheText.Init();
            ConfigControl.Init();
        }



        public PPTText(TextRange txtRange, Config conf)
            :base(txtRange.Text, conf)
        {
            this.txtRange = txtRange;
        }

        public static void ApplyCFToRange(CharFormatting cf, TextRange tRange, Config inConf)
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

        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            TextRange theChars = txtRange.Characters(fte.First + 1, fte.Last - fte.First + 1);
            ApplyCFToRange(fte.cf, theChars, conf);
        }

    } // class PPTText
} // namespace Colorization
