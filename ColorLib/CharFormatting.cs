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
    /// <summary>
    /// Représentation d'une couleur au format RGB (Red, Green, Blue). 
    /// </summary>
    [Serializable]
    readonly public struct RGB
    {
        private readonly Int32 color;
        public static implicit operator Int32(RGB r) => r.color;
        public static implicit operator System.Drawing.Color(RGB r) => System.Drawing.Color.FromArgb(r.R, r.G, r.B);
        public static implicit operator RGB(System.Drawing.Color col) => new RGB(col.R, col.G, col.B);

        /// <summary>
        /// Crée un objet RGB représentant la couleur définie par les trois paramêtres
        /// </summary>
        /// <param name="red">la valuer pour "rouge" entre 0 et 255</param>
        /// <param name="green">la valeur pour "vert" entre 0 et 255</param>
        /// <param name="blue">la valeur pour "bleu" entre 0 et 255</param>
        public RGB(byte red, byte green, byte blue) => color = red + (256 * green) + 65536 * blue;

        /// <summary>
        /// La valeur "rouge" de la couleur (entre 0 et 255)
        /// </summary>
        public int R { get{return color % 256;} }

        /// <summary>
        /// La valeur "verte" de la couleur (entre 0 et 255)
        /// </summary>
        public int G { get { return (color % 65536) / 256; } }

        /// <summary>
        /// La valeur "bleue" de la couleur (entre 0 et 255)
        /// </summary>
        public int B { get { return (color / 65536); } }

        /// <summary>
        /// Indique si la couleur doit être considérée comme foncée. Concrètement, il serait logique
        /// d'utiliser du blanc pour écrire sur une couleur foncée, et du noir pour écrire sur une couleur
        /// qui n'est pas foncée.
        /// </summary>
        /// <returns><c>true</c> si la couleur est foncée, <c>false</c> si elle est claire.</returns>
        public bool Dark()
        {
            return ((0.9*R) + (1.5*G) + (0.5*B)) < 380;
        }

        /// <summary>
        /// Génère une représentation textuelle de la couleur. Par exemple (255, 255, 255) pour blanc.
        /// </summary>
        /// <returns>La représentation textuelle de la couleur.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(15);
            sb.Append("(");
            sb.Append(R.ToString());
            sb.Append(", ");
            sb.Append(G.ToString());
            sb.Append(", ");
            sb.Append(B.ToString());
            sb.Append(")");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Un objet de cette classe correspond à des instruction de formatage. Les différents membres de l'objet
    /// indiquent comment les différents aspects du formatage doivent être traités.
    /// </summary>
    [Serializable]
    public class CharFormatting
    {
        /// <summary>
        /// true means set it bold. false no change
        /// </summary>
        public bool bold { get; private set; }

        /// <summary>
        /// true means set it italic. false no change
        /// </summary>
        public bool italic { get; private set; }

        /// <summary>
        /// true means underline it. false no change
        /// </summary>
        public bool underline { get; private set; }

        // Note: we have given up on inverting fromatting, since applying two times the colorization delivers a
        // result that the user does not mandatorily master...

        /// <summary>
        /// true means capital letter formatiing. Not used yet.
        /// </summary>
        public bool caps { get; private set; }

        /// <summary>
        /// if true the color is newly defined by the color member
        /// </summary>
        public bool changeColor { get; private set; }

        /// <summary>
        /// RGB color
        /// </summary>
        public RGB color { get; private set; }

        /// <summary>
        /// true means that the hilight must be set to the value given in <c>hilightColor</c>
        /// </summary>
        public bool changeHilight { get; private set; }

        /// <summary>
        /// The color the hilight has to be set to if <c>changeHilight</c> is true.
        /// </summary>
        public RGB hilightColor { get; private set; }

        // réservé pour un usage futur.
        public bool contour { get; private set; } // true means that the character is outlined
        public bool serif { get; private set; }  // font is changed to a serif font
        public bool changeFontSize { get; private set; } // if true change the font size according to incrFontSize
        public int percIncrFontSize { get; private set; } // percentage value, pos oder neg, defining a change in the font size. newSize = (1 + incrFontSize) * currentSize

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>bold</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non bold' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>bold</c> == <c>false</c> means that the character should be set to no 
        /// 'bold'</returns>
        public virtual bool ForceNonBold(Config conf) => conf.unsetBeh.CbuVal(Ucbx.bold);

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>italic</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non italic' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>italic</c> == <c>false</c> means that the character should be set to no 
        /// 'italic'</returns>
        public virtual bool ForceNonItalic(Config conf) => conf.unsetBeh.CbuVal(Ucbx.italic);

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>underline</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non underline' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>underline</c> == <c>false</c> means that the character should be set to no 
        /// 'underline'</returns>
        public virtual bool ForceNonUnderline(Config conf) => conf.unsetBeh.CbuVal(Ucbx.underline);

        /// <summary>
        /// Indicates how to react when <c>changeColor</c> is <c>false</c>. If the response is <c>true</c>, the 
        /// color black should be applied.
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if the color black (RGB(0,0,0)) must be applied when <c>changeColor</c>
        /// is <c>false</c></returns>
        public virtual bool ForceBlackColor(Config conf) => conf.unsetBeh.CbuVal(Ucbx.color);

        /// <summary>
        /// Indicates how to react when <c>changeHilight</c> is <c>false</c>. If the response is <c>true</c>, the 
        /// hilighting should be turned off.
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if the hilighting must be turned off when <c>changeHilight</c>
        /// is <c>false</c></returns>
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

        /// <summary>
        /// Creates a neutral <c>CharFormatting</c>
        /// </summary>
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

        /// <summary>
        /// Create a <c>CharFormatting</c> setting the given color. <c>changeColor</c> is set to <c>true</c>.
        /// </summary>
        /// <param name="inColor">The color that the <c>CharFormatting</c> must request to apply.</param>
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

        /// <summary>
        /// Crée un <c>CharFormatting</c> comme une copie de <c>cfToCopy</c> avec trois aspects qui peuvent
        /// changer: <c>bold</c>, <c>italic</c> et <c>underline</c>.
        /// </summary>
        /// <param name="cfToCopy">Le <c>CharFormatting</c> qui doit être recopié.</param>
        /// <param name="inBold">La nouvelle valeur de <c>bold</c>.</param>
        /// <param name="inItalic">La nouvelle valeur de <c>italic</c>.</param>
        /// <param name="inUnderline">La nouvelle valeur de <c>underline</c>.</param>
        public CharFormatting(
                                CharFormatting cfToCopy,
                                bool inBold,
                                bool inItalic,
                                bool inUnderline
                             )
        {
            bold = inBold;
            italic = inItalic;
            underline = inUnderline;
            caps = cfToCopy.caps;
            changeColor = cfToCopy.changeColor;
            color = cfToCopy.color;
            changeHilight = cfToCopy.changeHilight;
            hilightColor = cfToCopy.hilightColor;
            contour = cfToCopy.contour;
            serif = cfToCopy.serif;
            changeFontSize = cfToCopy.changeFontSize;
            percIncrFontSize = cfToCopy.percIncrFontSize;
        }

        /// <summary>
        /// Crée un <c>CharFormatting</c> comme une copie de <c>cfToCopy</c> et en mettant <c>color</c> à
        /// <c>inColor</c> et en forçant <c>changeCOlor</c> à <c>true</c>.
        /// </summary>
        /// <param name="cfToCopy">Le <c>CharFormatting</c> qui doit être recopié.</param>
        /// <param name="inColor">La nouvelle couleur.</param>
        public CharFormatting(
                                CharFormatting cfToCopy,
                                RGB inColor
                              )
        {
            bold = cfToCopy.bold;
            italic = cfToCopy.italic;
            underline = cfToCopy.underline;
            caps = cfToCopy.caps;
            changeColor = true;
            color = inColor;
            changeHilight = cfToCopy.changeHilight;
            hilightColor = cfToCopy.hilightColor;
            contour = cfToCopy.contour;
            serif = cfToCopy.serif;
            changeFontSize = cfToCopy.changeFontSize;
            percIncrFontSize = cfToCopy.percIncrFontSize;
        }
    }
}
