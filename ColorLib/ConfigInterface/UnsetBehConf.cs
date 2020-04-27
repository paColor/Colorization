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
    public enum Ucbx { bold, italic, underline, color, hilight, all, last } // all avant-dernier, last dernier

    public class UnsetBehConf
    {
        public ExecuteTask updateUCheckBoxes { private get; set; }

        private static string[] cbuNames = new string[] { "Bold", "Italic", "Underline", "Color", "Hilight", "All" };

        private bool[] act;
        private Dictionary<string, int> cbuMap;

        public UnsetBehConf()
        {
            act = new bool[(int)Ucbx.last];
            cbuMap = new Dictionary<string, int>((int)Ucbx.last);
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                act[i] = false;
                cbuMap[cbuNames[i]] = i;
            }
        }

        public void CbuChecked(string cbuName, bool val)
        {
            int btuIndex = cbuMap[cbuName];
            act[btuIndex] = val;
            if (btuIndex == (int)Ucbx.all)
            {
                for (int i = 0; i < (int)Ucbx.all; i++)
                    act[i] = val;
                updateUCheckBoxes();
            }
        }

        public bool CbuVal(string cbuName) => act[cbuMap[cbuName]];
        public bool CbuVal(Ucbx u) => act[(int)u];
    }
}
