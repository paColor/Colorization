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
using ColorLib;
using NLog;

namespace ColorizationControls
{
    public static class StaticColorizControls
    {
        private static FontStyle[] fontStyles = new FontStyle[8]
        {
            FontStyle.Regular,                                          // standard = 0
            FontStyle.Bold,                                             // bold = 1
            FontStyle.Italic,                                           // italic = 2
            FontStyle.Bold | FontStyle.Italic,                          // boldItalic = 3,
            FontStyle.Underline,                                        // underline = 4
            FontStyle.Bold | FontStyle.Underline,                       // boldUnderline = 5
            FontStyle.Italic | FontStyle.Underline,                     // italicUnderline = 6
            FontStyle.Bold | FontStyle.Italic | FontStyle.Underline     // boldItalicUnderline = 7
        };

        public const int NrCustomColors = 16;
        public static int[] customColors = new int[NrCustomColors];

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            logger.ConditionalDebug("Init");
            for (int i = 0; i < Math.Min(ColConfWin.predefinedColors.Length, NrCustomColors); i++)
                customColors[i] = ColConfWin.predefinedColors[i];
            for (int i = ColConfWin.predefinedColors.Length; i < NrCustomColors; i++)
                customColors[i] = 255 + (255 * 256) + (255 * 65536); // white
        }

        public static FontStyle GetFontStyle(CharFormatting cf)
        {
            int fontIndex = 0;
            if (cf.bold)
                fontIndex += 1;
            if (cf.italic)
                fontIndex += 2;
            if (cf.underline)
                fontIndex += 4;
            return fontStyles[fontIndex];
        }

        public static FontStyle GetFontStyle(bool bold, bool italic, bool underline)
        {
            int fontIndex = 0;
            if (bold)
                fontIndex += 1;
            if (italic)
                fontIndex += 2;
            if (underline)
                fontIndex += 4;
            return fontStyles[fontIndex];
        }
    }
}
