﻿/********************************************************************************
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
    public class FormattedTextEl : TextEl
    {
        public CharFormatting cf { get; private set; }

        public FormattedTextEl(TextEl te, CharFormatting inCf)
            :base(te)
        {
            cf = inCf;
        }

        public FormattedTextEl(TheText inT, int inFirst, int inLast, CharFormatting inCf)
            : base(inT, inFirst, inLast)
        {
            cf = inCf;
        }

    }
}
