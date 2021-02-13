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
        public static implicit operator RGB(Int32 i) => new RGB(i);

        public override bool Equals(object obj)
        {
            return this.Equals((RGB)obj);
        }

        public bool Equals(RGB rgb)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(rgb, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, rgb))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != rgb.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (color == rgb.color);
        }

        public override int GetHashCode()
        {
            return color;
        }

        public static bool operator ==(RGB lhs, RGB rhs)
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

        public static bool operator !=(RGB lhs, RGB rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Crée un objet RGB représentant la couleur définie par les trois paramêtres
        /// </summary>
        /// <param name="red">la valuer pour "rouge" entre 0 et 255</param>
        /// <param name="green">la valeur pour "vert" entre 0 et 255</param>
        /// <param name="blue">la valeur pour "bleu" entre 0 et 255</param>
        public RGB(byte red, byte green, byte blue) => color = red + (256 * green) + 65536 * blue;

        /// <summary>
        /// Créé un objet RGB sur la base d'un nombre entier représentant les 3 codes RGB
        /// </summary>
        /// <param name="inColor">La couleur</param>
        public RGB(Int32 inColor)
        {
            color = inColor;
        }

        /// <summary>
        /// La valeur "rouge" de la couleur (entre 0 et 255)
        /// </summary>
        public int R { get { return color % 256; } }

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
            return ((0.9 * R) + (1.5 * G) + (0.5 * B)) < 380;
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

}
