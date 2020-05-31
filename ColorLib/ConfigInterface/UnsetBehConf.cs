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
    public class CheckboxUnsetModifiedEventArgs : EventArgs
    {
        public Ucbx unsetCBX { get; private set; }
        public string unsetCBName { get; private set; }

        public CheckboxUnsetModifiedEventArgs(Ucbx inUnsetCBX)
        {
            unsetCBX = inUnsetCBX;
            unsetCBName = UnsetBehConf.cbuNames[(int)unsetCBX];
        }
    }

    /// <summary>
    /// Les différentes checkboxes qui existent
    /// </summary>
    [Serializable]
    public enum Ucbx { bold, italic, underline, color, hilight, all, last } // all avant-dernier, last dernier

    /// <summary>
    /// Configuration pour les flags avancés déterminant comment doit se comporter le formatage
    /// quand un attribut d'un <see cref="CharFormatting"/> sont à la valeur <c>false</c>. Faut-il
    /// laisser le formatage dans l'état actuel ou forcer à la valeur <c>false</c>?
    /// </summary>
    [Serializable]
    public class UnsetBehConf : ConfigBase
    {
        // ************************************************* STATIC *************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Les différents <c>string</c> qui peuvent être utilisés pour identifier une des checkboxes. Les 
        /// positions dans le tableau correspondent à l'order dans l'énum <see cref="Ucbx"/>
        /// </summary>
        public static string[] cbuNames { get; private set; } 
            = new string[] { "Bold", "Italic", "Underline", "Color", "Hilight", "All" };

        /// <summary>
        /// permet de faire la conversion checkbox name --> index dans <see cref="Ucbx"/> en utilsant la
        /// forme <c>int i = cbuMap[name];</c>
        /// </summary>
        public static Dictionary<string, int> cbuMap { get; private set; } = new Dictionary<string, int>((int)Ucbx.last);

        public static void Init()
        {
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                cbuMap[cbuNames[i]] = i;
            }
        }

        // ************************************************** INSTANTIATED ********************************************

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Event Handlers ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand un des flags est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<CheckboxUnsetModifiedEventArgs> CheckboxUnsetModifiedEvent;

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------------  Members -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private bool[] act;

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------------  Methods -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public UnsetBehConf()
        {
            act = new bool[(int)Ucbx.last];
            Reset();
        }

        /// <summary>
        /// Modifie la valeur du flag identifié par <paramref name="cbuName"/> pour la
        /// metter à <paramref name="val"/>.
        /// </summary>
        /// <param name="cbuName">Le nom de la checkbox à modifier.</param>
        /// <param name="val">La nouvelle valeur.</param>
        public void CbuChecked(string cbuName, bool val)
        {
            int btuIndex = cbuMap[cbuName];
            if (act[btuIndex] = val) // Pour éviter un évènement si rien ne change
            {
                act[btuIndex] = val;
                OnCheckboxUnsetModified((Ucbx)btuIndex);
                if (btuIndex == (int)Ucbx.all)
                {
                    for (int i = 0; i < (int)Ucbx.all; i++)
                    {
                        act[i] = val;
                        OnCheckboxUnsetModified((Ucbx)i);
                    }
                }
            }
        }

        /// <summary>
        /// Donne la valeur du flag identifié par <paramref name="cbuName"/>.
        /// </summary>
        /// <param name="cbuName">Le nom du flag dont on veut connaître la valeur.</param>
        /// <returns></returns>
        public bool CbuVal(string cbuName) => act[cbuMap[cbuName]];
        
        /// <summary>
        /// Donne la valeur du flag identifié par <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Le flag dont on veut connaître la valeur.</param>
        /// <returns></returns>
        public bool CbuVal(Ucbx u) => act[(int)u];

        /// <summary>
        /// Réinitialise tous les flags à <c>false</c>.
        /// </summary>
        public override void Reset()
        {
            logger.ConditionalTrace("Reset");
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                CbuChecked(cbuNames[i], false);
            }
        }

        protected virtual void OnCheckboxUnsetModified(Ucbx u)
        {
            logger.ConditionalTrace(BaseConfig.cultF, "OnCheckboxUnsetModified, checkbox \'{0}\'", cbuNames[(int)u]);
            EventHandler<CheckboxUnsetModifiedEventArgs> eventHandler = CheckboxUnsetModifiedEvent;
            eventHandler?.Invoke(this, new CheckboxUnsetModifiedEventArgs(u));
        }
    }
}
