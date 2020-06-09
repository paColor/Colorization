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
using ColorLib;
using NLog;

namespace ColorizationControls
{
    public static class StaticColorizControls
    {
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
    }
}
