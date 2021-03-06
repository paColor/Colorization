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

 // cette classe n'est pas sensée s'ouvrir proprement dans l'éditeur de formulaire 
 // de VisualStudio

using ColorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorizationControls
{
    /// <summary>
    /// <c>SylFormatForm</c> se comporte comme <c>CharFormatForm</c> dont elle est une descendante, sauf pour la
    /// définition du titre de la fenêtre qui s'appelle ici "Formatage syllabe no "
    /// </summary>
    public class SylFormatForm : CharFormatForm
    {
        /// <summary>
        /// Crée la fenêtre pour la définition du formatage pour une syllabe. Permet de déterminer 
        /// </summary>
        /// <param name="cf"></param>
        /// <param name="theSylButton"></param>
        /// <param name="inCharFormResult"></param>
        public SylFormatForm(CharFormatting cf, string theSylButton, SetCharFormResult inCharFormResult)
            : base(cf, theSylButton, inCharFormResult)
        { }

        protected override string FormName(string theSylButton)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Formatage syllabe no ");
            sb.Append(int.Parse(theSylButton) + 1);
            return sb.ToString();
        }
    }
}
