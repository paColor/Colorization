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
    readonly public struct RGB
    {
        private readonly Int32 color;
        public static implicit operator Int32(RGB r) => r.color;
        public static implicit operator System.Drawing.Color(RGB r) => System.Drawing.Color.FromArgb(r.R(), r.G(), r.B());
        public static implicit operator RGB(System.Drawing.Color col) => new RGB(col.R, col.G, col.B);

        public RGB(byte red, byte green, byte blue) => color = red + (256 * green) + 65536 * blue;
        public int R() => color % 256;
        public int G() => (color % 65536) / 256;
        public int B() => (color / 65536);

        public bool Dark()
        {
            return (R() + G() + (0.8*B())) < 400;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(15);
            sb.Append("(");
            sb.Append(R().ToString());
            sb.Append(", ");
            sb.Append(G().ToString());
            sb.Append(", ");
            sb.Append(B().ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }

    public class CharFormatting
    {
        public bool bold { get; private set; } // true means set it bold. false no change
        public bool italic { get; private set; }  // true means set it italic. false no change
        public bool underline { get; private set; }  // true means underline it. false no change

        // Note: we have given up on inversing fromatting, since applying two times the colorization delivers a
        // result that the user does not mandatorily master...

        public bool caps { get; private set; } // true means capital letter formatiing
        public bool changeColor { get; private set; } // if true the color is newly defined by the color member
        public RGB color { get; private set; }  // RGB color
        public bool changeHilight { get; private set; } // true means that the hilight must be set
        public RGB hilightColor { get; private set; }

        // réservé pour un usage futur.
        public bool contour { get; private set; } // true means that the character is outlined
        public bool serif { get; private set; }  // font is changed to a serif font

        public bool changeFontSize { get; private set; } // if true change the font size according to incrFontSize
        public int percIncrFontSize { get; private set; } // percentage value, pos oder neg, defining a change in the font size. newSize = (1 + incrFontSize) * currentSize


        public virtual bool ForceNonBold(Config conf) => conf.unsetBeh.CbuVal(Ucbx.bold);
        public virtual bool ForceNonItalic(Config conf) => conf.unsetBeh.CbuVal(Ucbx.italic);
        public virtual bool ForceNonUnderline(Config conf) => conf.unsetBeh.CbuVal(Ucbx.underline);
        public virtual bool ForceBlackColor(Config conf) => conf.unsetBeh.CbuVal(Ucbx.color);
        public virtual bool ForceHilightClear(Config conf) => conf.unsetBeh.CbuVal(Ucbx.hilight);
        public virtual bool ForceNonCaps(Config conf) => false;
        public virtual bool ForceNonContour(Config conf) => false;
        public virtual bool ForceNonSerif(Config conf) => false;

        private void InitNeutral ()
        {
            // default values
            bold = false;
            italic = false;
            underline = false;
            caps = false;
            changeColor = false;
            color = ColConfWin.predefinedColors[(int)PredefCols.black];
            changeHilight = false;
            hilightColor = ColConfWin.predefinedColors[(int)PredefCols.neutral];
            contour = false;
            serif = false;
            changeFontSize = false;
            percIncrFontSize = 0;
        }

        public CharFormatting ()
        {
            InitNeutral();
        } // constructor

        public CharFormatting(
                                bool inBold,
                                bool inItalic,
                                bool inUnderline,
                                bool inCaps,
                                bool inChangeColor,
                                RGB inColor,
                                bool inChangeHilight,
                                RGB inHilightColor,
                                bool inContour,
                                bool inSerif,
                                bool inChangeFontSize,
                                int inPercIncrFontSize
                             )
        {
            // default values
            bold = inBold;
            italic = inItalic;
            underline = inUnderline;
            caps = inCaps;
            changeColor = inChangeColor;
            color = inColor;
            hilightColor = inHilightColor;
            changeHilight = inChangeHilight;
            contour = inContour;
            serif = inSerif;
            changeFontSize = inChangeFontSize;
            percIncrFontSize = inPercIncrFontSize;
        } // constructor

        public CharFormatting(RGB inColor)
        {
            InitNeutral();
            changeColor = true;
            color = inColor;
        } // CharFormatting(RGB inColor)

        public CharFormatting(
                                bool inBold,
                                bool inItalic,
                                bool inUnderline,
                                bool inCaps,
                                bool inChangeColor,
                                RGB inColor,
                                bool inChangeHilight,
                                RGB inHilightColor
                             )
        {
            InitNeutral();
            bold = inBold;
            italic = inItalic;
            underline = inUnderline;
            caps = inCaps;
            changeColor = inChangeColor;
            color = inColor;
            hilightColor = inHilightColor;
            changeHilight = inChangeHilight;
        } // constructor

        public CharFormatting(
                                bool inBold,
                                bool inItalic,
                                bool inUnderline
                             )
        {
            InitNeutral();
            bold = inBold;
            italic = inItalic;
            underline = inUnderline;
        } // constructor
    }
}
