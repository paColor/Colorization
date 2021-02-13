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
using System.Text;
using System.Diagnostics;
using NLog;

namespace ColorLib
{
    /// <summary>
    /// Un élément du texte référencé <c>T</c>. Commence à <c>First</c> et se termine à 
    /// <c>Last</c>. Si <c>Last</c> = <c>First</c>, l'élément contient exactement 1
    /// caractère.
    /// </summary>
    public class TextEl
    {
        // ****************************************************************************************
        // *                                       STATIC                                         *
        // ****************************************************************************************

        /// <summary>
        /// Permet de comparer des <c>textEl</c> pour, le cas échéant, les remettre dans l'ordre.
        /// N'a un sens que pour des éléments se rapportant au même <see cref="TheText"/> 
        /// </summary>
        /// <param name="te1"></param>
        /// <param name="te2"></param>
        /// <returns></returns>
        public static int CompareTextElByPosition(TextEl te1, TextEl te2)
        {
            Debug.Assert(te1.T == te2.T); // sinon ça n'a pas de sens.
            int toReturn = 0;
            if (te1.First < te2.First)
            {
                toReturn = -1;
            }
            else if (te1.First > te2.First)
            {
                toReturn = 1;
            }
            return toReturn;
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ****************************************************************************************
        // *                                    INSTANTIATED                                      *
        // ****************************************************************************************

        /// <summary>
        /// Le <see cref="TheText"/> auquel se rapporte le <c>TextEl</c>.
        /// </summary>
        public TheText T { get; private set; }
        
        /// <summary>
        /// La position (zero-based) dans <c>T</c> du premier caractère du <c>TextEl</c>. Doit
        /// être plus grand ou égal à zéro.
        /// </summary>
        public int First { get; protected set; } // start index of the word in T

        /// <summary>
        /// La position (zero-based) dans <c>T</c> du dernier caractère du <c>TextEl</c>.
        /// S'il est plus petit que <c>First</c>, l'élément est considéré comme vide.
        /// </summary>
        public int Last { get; protected set; }

        private const string consonnes = "bcdfghjklmnpqrstvwxzç";
        private const string voyelles = "aeiouyœéàèùäëïöüâêîôûœ";

        public static bool EstConsonne(char c)
        {
            return consonnes.IndexOf(c) > -1;
        }

        public static bool EstVoyelle(char c)
        {
            return voyelles.IndexOf(c) > -1;
        }

        /// <summary>
        /// Crée un <c>TextEl</c> qui correspond à une partie du texte <c>inT</c>. Ne
        /// peut pas être vide! 
        /// </summary>
        /// <param name="tt">Le texte dont le <c>TextEl</c> est une partie. Ne peut pas
        /// être vide, ni <c>null</c>! </param>
        /// <param name="inFirst">La position du premier caractère dans <c>inT</c>. Doit 
        /// être plus grand ou égal à 0. </param>
        /// <param name="inLast">La position du dernier caractère dans <c>inT</c>. S'il est plus 
        /// petit que <c>inFirst</c>, l'élément est considéré comme vide.</param>
        /// <exception cref="ArgumentNullException"> si <paramref name="tt"/> est <c>null</c>
        /// </exception>
        public TextEl(TheText tt, int inFirst, int inLast)
        {
            if (tt == null)
            {
                const string Message = "On ne peut pas créer un TextEl sur un TheText null.";
                logger.Error(Message);
                throw new ArgumentNullException(nameof(tt), Message);
            }
            if (inFirst < 0 || inLast >= tt.S.Length)
            {
                string Message = String.Format
                    ("TextEl: inFirst: {0} trop petir ou inLast {1} trop grand. tt: {2}",
                    inFirst, inLast, tt.S);
                logger.Error("TextEl: inFirst: {0} trop petir ou inLast {1} trop grand. tt: {2}",
                    inFirst, inLast, tt.S);
                throw new ArgumentException("TextEl: inFirst trop petit ou inLast trop grand");
            }
#if DEBUG
            if (inLast < inFirst)
            {
                // L'élément est vide...
                logger.ConditionalDebug("TextEl vide créé.");
            }
#endif
            T = tt;
            First = inFirst;
            Last = inLast;
        }

        protected TextEl(TextEl te)
        {
            T = te.T;
            First = te.First;
            Last = te.Last;
        }

        /// <summary>
        /// returns a string that contains the characters of the <c>TextEl</c>
        /// </summary>
        /// <returns>The characters of the <c>TextEl</c></returns>
        public override string ToString()
        {
            if (Last < First)
                return "";
            else
                return T.ToString().Substring(First, (Last - First) + 1);
        }

        /// <summary>
        /// Returns the characters of the <c>TextEl</c> in lower case.
        /// </summary>
        /// <returns>lower case string of the <c>TextEl</c>.</returns>
        public string ToLowerString()
        {
            if (Last < First)
                return "";
            else
                return T.ToLowerString().Substring(First, (Last - First) + 1);
        }

        /// <summary>
        /// Crée le <see cref="FormattedTextEl"/> correspondant au <c>TextEl</c> <c>this</c> et
        /// au <see cref="CharFormatting"/> <paramref name="cf"/>. Ajoute le <c>FormattedTextEl</c>
        /// à la liste des <c>FormattedTextEl</c> du texte.
        /// </summary>
        /// <param name="cf">Le <see cref="CharFormatting"/> à utiliser pour ce <c>TextEl</c></param>
        protected void SetCharFormat(CharFormatting cf)
        {
            if (Last >= First)
            {
                FormattedTextEl fte = new FormattedTextEl(this, cf);
                T.AddFTE(fte);
            }
        }

        /// <summary>
        /// Applique le formatage voulu par <paramref name="conf"/> à l'élément de texte.
        /// Par défaut le formatage est neutre, mais les héritiers sont invités à changer
        /// ce comportement.
        /// </summary>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour le formatage.</param>
        public virtual void PutColor(Config conf)
        {

            SetCharFormat(CharFormatting.NeutralCF);
        }

        /// <summary>
        /// Returns a dump of all members of the object
        /// </summary>
        /// <returns>A string containing a text representation of all members.</returns>
        public virtual string AllStringInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.ToString());
            sb.Append(" --> ");
            sb.Append(String.Format("First: {0}, Last: {1}", First, Last));
            return sb.ToString();
        }

    }
}
