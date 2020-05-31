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
    /// Classe de base pour les différentes classes de configuration.
    /// </summary>
    [Serializable]
    public abstract class ConfigBase
    {

        /// <summary>
        /// Est appelé à la fin de la désérialisation pour permettre d'initilaiser les membres optionnels
        /// qui n'ont pas pu être traités par les mécanismes classiques. Il y a en effet des problèmes avec les 
        /// collections qui semblent être désérialisées après l'appel de la méthode [OnDeserialized]
        /// </summary>
        /// <remarks>
        /// <para>
        /// Attention: Tout classe qui "contient" des classses basées sur <c>ConfigBase</c> (des enfants) doit  
        /// s'assurer que cette méthode est appelée pour tous les enfants.
        /// </para>
        /// <para>
        /// En cas de désérialisation il faut explicitement appeler cette méthode après la désérialisation, 
        /// puisque [OnDeserialized] ne permet pas de couvrir tous les cas.
        /// </para>
        /// <para>
        /// Les évèenements ne sont pas enregistrés dans la sérialisation. Il faut donc absolument s'abonner
        /// ici aux évènements des "enfants" qui seraient nécessaires. 
        /// </para>
        /// </remarks>
        internal virtual void PostLoadInitOptionalFields()
        {
            // par défaut on ne fait rien.
        }

        /// <summary>
        /// Réinitialise l'objet entièrement à ses valeurs par défaut.
        /// </summary>
        public abstract void Reset();
    }
}
