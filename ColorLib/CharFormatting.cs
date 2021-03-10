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
using System.Diagnostics;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// Un objet de cette classe correspond à des instruction de formatage. Les différents membres de l'objet
    /// indiquent comment les différents aspects du formatage doivent être traités.
    /// </summary>
    [Serializable]
    public class CharFormatting
    {
        // ****************************************************************************************
        // *                                       STATIC                                         *
        // ****************************************************************************************

        /// <summary>
        /// A <c>CharFormatting</c> as generated by the constructor without parameters. No flag set.
        /// </summary>
        public static CharFormatting NeutralCF { get; private set; }

        /// <summary>
        /// A <see cref="CharFormatting"/> containing only the instruction to color the text in
        /// black.
        /// </summary>
        public static CharFormatting BlackCF { get; private set; }

        /// <summary>
        /// La couleur utilisée pour <c>arcColor</c> quand un arc ne doit pas être dessiné. 
        /// </summary>
        public static RGB neutralArcsCol { get; private set; }

        public static void Init()
        {
            neutralArcsCol = new RGB(255, 255, 254);
            NeutralCF = new CharFormatting();
            BlackCF = new CharFormatting(new RGB(0, 0, 0));
        }

        // ****************************************************************************************
        // *                                INSTANTIATED MEMBERS                                  *
        // ****************************************************************************************

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
        /// Couleur à utiliser pour la colorisation des lettres
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

        /// <summary>
        /// FOR FUTURE USE
        /// <c>true</c> means that the character is outlined.
        /// </summary>
        public bool contour { get; private set; }

        /// <summary>
        /// FOR FUTURE USE
        /// <c>true</c>: font is changed to a serif font
        /// </summary>
        public bool serif { get; private set; }

        /// <summary>
        /// FOR FUTURE USE
        /// <c>true</c>: change the font size according to <c>incrFontSize</c>.
        /// </summary>
        public bool changeFontSize { get; private set; }

        /// <summary>
        /// FOR FUTURE USE
        /// percentage value, pos or neg, defining a change in the font size.
        /// newSize = (1 + incrFontSize) * currentSize
        /// </summary>
        public int percIncrFontSize { get; private set; }

        /// <summary>
        /// Indique si un arc doit être tracé sous le groupe de letres. La couleur
        /// est donnée pas <see cref="arcColor"/>
        /// </summary>
        public bool drawArc { get; private set; }

        /// <summary>
        /// La couleur de l'arc.
        /// </summary>
        public RGB arcColor { get; private set; }

        /// <summary>
        /// Indique si les arcs liés au groupe de lettres doivent être effacés.
        /// </summary>
        public bool removeArcs { get; private set; }


        // ****************************************************************************************
        // *                                   FORCE METHODS                                      *
        // ****************************************************************************************

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>bold</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non bold' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>bold</c> == <c>false</c> means that the character should be set to no 
        /// 'bold'</returns>
        public virtual bool ForceNonBold(Config conf) => conf.unsetBeh.GetCbuFlag(Ucbx.bold);

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>italic</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non italic' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>italic</c> == <c>false</c> means that the character should be set to no 
        /// 'italic'</returns>
        public virtual bool ForceNonItalic(Config conf) => conf.unsetBeh.GetCbuFlag(Ucbx.italic);

        /// <summary>
        /// Indicates whether a <c>false</c> value for the member <c>underline</c> should be interpreted as 'do nothing' 
        /// (<c>false</c>) or 'apply non underline' (<c>true</c>
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if <c>underline</c> == <c>false</c> means that the character should be set to no 
        /// 'underline'</returns>
        public virtual bool ForceNonUnderline(Config conf) => conf.unsetBeh.GetCbuFlag(Ucbx.underline);

        /// <summary>
        /// Indicates how to react when <c>changeColor</c> is <c>false</c>. If the response is <c>true</c>, the 
        /// color black should be applied.
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if the color black (RGB(0,0,0)) must be applied when <c>changeColor</c>
        /// is <c>false</c></returns>
        public virtual bool ForceBlackColor(Config conf) => conf.unsetBeh.GetCbuFlag(Ucbx.color);

        /// <summary>
        /// Indicates how to react when <c>changeHilight</c> is <c>false</c>. If the response is <c>true</c>, the 
        /// hilighting should be turned off.
        /// </summary>
        /// <param name="conf">The <c>Config</c> that must be used to answer the question.</param>
        /// <returns><c>true</c> if the hilighting must be turned off when <c>changeHilight</c>
        /// is <c>false</c></returns>
        public virtual bool ForceHilightClear(Config conf) => conf.unsetBeh.GetCbuFlag(Ucbx.hilight);
        public virtual bool ForceNonCaps(Config conf) => false;
        public virtual bool ForceNonContour(Config conf) => false;
        public virtual bool ForceNonSerif(Config conf) => false;

        // ****************************************************************************************
        // *                             CONSTRUCTORS & ToString()                                *
        // ****************************************************************************************

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(40);
            if (bold)
                sb.Append('b');
            if (italic)
                sb.Append('i');
            if (underline)
                sb.Append('u');
            if (caps)
                sb.Append('C');
            if (contour)
                sb.Append('o');
            if (serif)
                sb.Append('f');
            if (changeColor)
            {
                sb.Append('c');
                sb.Append(color.ToString());
            }
            if (changeHilight)
            {
                sb.Append('h');
                sb.Append(hilightColor.ToString());
            }
            if (changeFontSize)
            {
                sb.Append("s-");
                sb.Append(percIncrFontSize);
                sb.Append("%-");
            }
            return sb.ToString();
        }


        /// <summary>
        /// Creates a neutral <c>CharFormatting</c>
        /// </summary>
        protected CharFormatting ()
        {
            InitNeutral();
        } // constructor

        /// <summary>
        /// Constructeur permettant de définir tous les champs d'un <see cref="CharFormatting"/>
        /// </summary>
        /// <param name="inBold"></param>
        /// <param name="inItalic"></param>
        /// <param name="inUnderline"></param>
        /// <param name="inCaps"></param>
        /// <param name="inChangeColor"></param>
        /// <param name="inColor"></param>
        /// <param name="inChangeHilight"></param>
        /// <param name="inHilightColor"></param>
        /// <param name="inContour"></param>
        /// <param name="inSerif"></param>
        /// <param name="inChangeFontSize"></param>
        /// <param name="inPercIncrFontSize"></param>
        /// <param name="inDrawArc"></param>
        /// <param name="inArcColor"></param>
        /// <param name="inRemoveArcs"></param>
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
                                int inPercIncrFontSize,
                                bool inDrawArc,
                                RGB inArcColor,
                                bool inRemoveArcs
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
            drawArc = inDrawArc;
            arcColor = inArcColor;
            removeArcs = inRemoveArcs;
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

        /// <summary>
        /// Crée un <see cref="CharFormatting"/> avec les paramètres que l'utilisateur peut configurer dans
        /// la fenêtre de définition du formatage.
        /// </summary>
        /// <param name="inBold"></param>
        /// <param name="inItalic"></param>
        /// <param name="inUnderline"></param>
        /// <param name="inCaps"></param>
        /// <param name="inChangeColor"></param>
        /// <param name="inColor"></param>
        /// <param name="inChangeHilight"></param>
        /// <param name="inHilightColor"></param>
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

        /// <summary>
        /// Création d'un <see cref="CharFormatting"/> avec les trois caractéristiques d'un caractère
        /// </summary>
        /// <param name="inBold"></param>
        /// <param name="inItalic"></param>
        /// <param name="inUnderline"></param>
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
            drawArc = cfToCopy.drawArc;
            arcColor = cfToCopy.arcColor;
        }

        /// <summary>
        /// Crée un <c>CharFormatting</c> comme une copie de <c>cfToCopy</c> et en mettant <c>color</c> à
        /// <c>inColor</c> et en forçant <c>changeCOlor</c> à <c>true</c>.
        /// </summary>
        /// <param name="cfToCopy">Le <c>CharFormatting</c> qui doit être recopié. Non null.</param>
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
            drawArc = cfToCopy.drawArc;
            arcColor = cfToCopy.arcColor;
        }

        /// <summary>
        /// Crée un <c>CharFormatting</c> comme une copie de <c>cfToCopy</c> et en mettant 
        /// <c>changeHilight</c> et <c>hilightColor</c> aux valeurs demandées.
        /// </summary>
        /// <param name="cfToCopy">Le <c>CharFormatting</c> qui doit être recopié. Non null.</param>
        /// <param name="inChangeHilight">La nouvelle valeur pour <c>changeHilight</c>.</param>
        /// <param name="inHilightColor">La nouvelle valeur pour <c>hilightColor</c>.</param>
        public CharFormatting(
                                CharFormatting cfToCopy,
                                bool inChangeHilight,
                                RGB inHilightColor
                              )
        {
            bold = cfToCopy.bold;
            italic = cfToCopy.italic;
            underline = cfToCopy.underline;
            caps = cfToCopy.caps;
            changeColor = cfToCopy.changeColor;
            color = cfToCopy.color;
            changeHilight = inChangeHilight;
            hilightColor = inHilightColor;
            contour = cfToCopy.contour;
            serif = cfToCopy.serif;
            changeFontSize = cfToCopy.changeFontSize;
            percIncrFontSize = cfToCopy.percIncrFontSize;
            drawArc = cfToCopy.drawArc;
            arcColor = cfToCopy.arcColor;
        }

        /// <summary>
        /// Créé un <c>CharFormatting</c> pour la colorisation d'arcs.
        /// </summary>
        /// <remarks><paramref name="inDrawArc"/> doit être <c>true</c>. La seule raison de sa
        /// présence est la nécessité d'avoir une signature de fonction différente.</remarks>
        /// <param name="inDrawArc">La valeur du champ <c>drawArc</c>. Doit être <c>true</c>.</param>
        /// <param name="inArcColor">La couleur de l'arc.</param>
        public CharFormatting(
                                bool inDrawArc,
                                RGB inArcColor  
                             )
        {
            Debug.Assert(inDrawArc);
            InitNeutral();
            drawArc = inDrawArc;
            arcColor = inArcColor;
        }

        /// <summary>
        /// Crée un <see cref="CharFormatting"/> pour l'effacement des arcs.
        /// </summary>
        /// <param name="inRemoveArcs">Doit être <c>true</c></param>
        public CharFormatting(bool inRemoveArcs)
        {
            Debug.Assert(inRemoveArcs);
            removeArcs = inRemoveArcs;
        }

        // ****************************************************************************************
        // *                                      COMPARISON                                      *
        // ****************************************************************************************

        public override bool Equals(object obj)
        {
            return this.Equals((CharFormatting)obj);
        }

        public bool Equals(CharFormatting cf)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(cf, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, cf))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != cf.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.

            bool toReturn = (
                bold == cf.bold
                && italic == cf.italic
                && underline == cf.underline
                && caps == cf.caps
                && changeColor == cf.changeColor
                && changeHilight == cf.changeHilight
                && changeFontSize == changeFontSize
                && contour == cf.contour
                && serif == cf.serif
                && drawArc == cf.drawArc
                && removeArcs == cf.removeArcs
                );
            if (changeColor)
                toReturn = toReturn && color == cf.color;
            if (changeHilight)
                toReturn = toReturn && hilightColor == cf.hilightColor;
            if (changeFontSize)
                toReturn = toReturn && percIncrFontSize == cf.percIncrFontSize;
            if (drawArc)
                toReturn = toReturn && arcColor == cf.arcColor;

            return toReturn;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode = hashCode + (256 * Convert.ToInt32(bold));
            hashCode = hashCode + (128 * Convert.ToInt32(italic));
            hashCode = hashCode + ( 64 * Convert.ToInt32(underline));
            hashCode = hashCode + ( 32 * Convert.ToInt32(caps));
            hashCode = hashCode + ( 16 * Convert.ToInt32(changeColor));
            hashCode = hashCode + (  8 * Convert.ToInt32(changeHilight));
            hashCode = hashCode + (  4 * Convert.ToInt32(changeFontSize));
            hashCode = hashCode + (  2 * Convert.ToInt32(contour));
            hashCode = hashCode + (  1 * Convert.ToInt32(serif));
            hashCode = hashCode << 20;

            if (changeColor)
                hashCode = hashCode ^ color.GetHashCode();
            hashCode = hashCode << 1;
            if (changeHilight)
                hashCode = hashCode ^ hilightColor.GetHashCode();
            hashCode = hashCode << 1;
            if (changeFontSize)
                hashCode = hashCode ^ percIncrFontSize.GetHashCode();

            return hashCode;
        }

        public static bool operator ==(CharFormatting lhs, CharFormatting rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(CharFormatting lhs, CharFormatting rhs)
        {
            return !(lhs == rhs);
        }

        // ****************************************************************************************
        // *                                    PRIVATE METHODS                                   *
        // ****************************************************************************************

        private void InitNeutral()
        {
            // default values
            bold = false;
            italic = false;
            underline = false;
            caps = false;
            changeColor = false;
            color = ColConfWin.predefinedColors[(int)PredefCol.black];
            changeHilight = false;
            hilightColor = ColConfWin.predefinedColors[(int)PredefCol.neutral];
            contour = false;
            serif = false;
            changeFontSize = false;
            percIncrFontSize = 0;
            drawArc = false;
            arcColor = neutralArcsCol;
            removeArcs = false;
        }

        private int ShiftAndWrap(int value, int positions)
        {
            positions = positions & 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer.
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded.
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits.
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }

    }
}
