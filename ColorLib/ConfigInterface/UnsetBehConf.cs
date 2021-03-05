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
    public class CheckboxUnsetModifiedEventArgs : EventArgs
    {
        public Ucbx unsetCBX { get; private set; }
        public string unsetCBName { get; private set; }

        public CheckboxUnsetModifiedEventArgs(Ucbx inUnsetCBX, string inUnsetCBName)
        {
            unsetCBX = inUnsetCBX;
            unsetCBName = inUnsetCBName;   
        }
    }

    /// <summary>
    /// Les différentes checkboxes qui existent
    /// </summary>
    [Serializable]
    public enum Ucbx { bold, italic, underline, color, hilight, all, last }
    // all avant-dernier, last dernier

    /// <summary>
    /// <para>
    /// UnsetBehaviourConfiguration:
    /// </para>
    /// <para>
    /// Configuration pour les flags avancés déterminant comment doit se comporter le formatage
    /// quand un attribut d'un <see cref="CharFormatting"/> est à la valeur <c>false</c>. Faut-il
    /// laisser le formatage dans l'état actuel ou forcer à la valeur <c>false</c>?
    /// </para>
    /// <para>
    /// Chacun des flags indique comment traiter un des attributs de <see cref="CharFormatting"/>.
    /// La liste des attributs gérés par la classe est donnée par l'enum <see cref="Ucbx"/>. On
    /// peut également accéder à chaque flag par son nom sous forme de <c>string</c>. Les valeurs
    /// admises sont:
    /// <code>
    /// { "Bold",       Ucbx.bold },
    /// { "Italic",     Ucbx.},
    /// { "Underline",  Ucbx.underline },
    /// { "Color",      Ucbx.color },
    /// { "Hilight",    Ucbx.hilight },
    /// { "All",        Ucbx.all }
    /// </code>
    /// </para>
    /// </summary>
    [Serializable]
    public class UnsetBehConf : ConfigBase
    {
        // ************************************************* STATIC *************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Les différents <c>string</c> qui peuvent être utilisés pour identifier une des checkboxes. Les 
        /// positions dans le tableau correspondent à l'ordre dans l'énum <see cref="Ucbx"/>
        /// </summary>
        private static string[] cbuNames { get; set; } 
            = new string[] { "Bold", "Italic", "Underline", "Color", "Hilight", "All" };

        /// <summary>
        /// permet de faire la conversion checkbox name --> index dans <see cref="Ucbx"/> en utilsant la
        /// forme <code>int i = cbuMap[name];</code>
        /// </summary>
        private static Dictionary<string, Ucbx> cbuMap { get; set; } = new Dictionary<string, Ucbx>((int)Ucbx.last)
        {
            { "Bold",       Ucbx.bold },
            { "Italic",     Ucbx.italic },
            { "Underline",  Ucbx.underline },
            { "Color",      Ucbx.color },
            { "Hilight",    Ucbx.hilight },
            { "All",        Ucbx.all }
        };

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

        /// <summary>
        /// Le tableau des valeurs des flags gérés. À indexer avec <see cref="Ucbx"/>.
        /// </summary>
        private bool[] act;

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------------  Methods -------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Crée un <see cref="UnsetBehConf"/> avec une configuration par défaut (celle obtenue
        /// après <see cref="Reset"/>.
        /// </summary>
        public UnsetBehConf()
        {
            UndoFactory.DisableUndoRegistration();
            act = new bool[(int)Ucbx.last];
            Reset();
            UndoFactory.EnableUndoRegistration();
        }

        /// <summary>
        /// Modifie la valeur du flag identifié par <paramref name="cbuName"/> pour la
        /// metter à <paramref name="val"/>.
        /// </summary>
        /// <param name="cbuName">Le nom de la checkbox à modifier.</param>
        /// <param name="val">La nouvelle valeur.</param>
        /// <exception cref="ArgumentNullException"> si <paramref name="cbuName"/> est null.
        /// </exception>
        /// <exception cref="KeyNotFoundException"> si <paramref name="cbuName"/> n'est pas
        /// l'identifiant valable d'un flag.</exception>
        public void SetCbuFlag(string cbuName, bool val)
        {
            SetCbuFlag(cbuMap[cbuName], val);
        }

        /// <summary>
        /// Donne la valeur du flag identifié par <paramref name="cbuName"/>.
        /// </summary>
        /// <param name="cbuName">Le nom du flag dont on veut connaître la valeur.</param>
        /// <returns>La valeur du flag.</returns>
        /// <exception cref="ArgumentNullException"> si <paramref name="cbuName"/> est null.
        /// </exception>
        /// <exception cref="KeyNotFoundException"> si <paramref name="cbuName"/> n'est pas
        /// l'identifiant valable d'un flag.</exception>
        public bool GetCbuFlag(string cbuName) => act[(int)cbuMap[cbuName]];

        /// <summary>
        /// Donne la valeur du flag identifié par <paramref name="u"/>.
        /// </summary>
        /// <param name="u">Le flag dont on veut connaître la valeur.</param>
        /// <returns>La valeur du flag.</returns>
        /// <exception cref="IndexOutOfRangeException"> si <paramref name="u"/> est égal à
        /// <c>Ucbx.last</c></exception>
        public bool GetCbuFlag(Ucbx u) => act[(int)u];

        /// <summary>
        /// Réinitialise tous les flags à <c>false</c>.
        /// </summary>
        public override void Reset()
        {
            logger.ConditionalDebug("Reset");
            for (int i = 0; i < (int)Ucbx.last; i++)
            {
                SetCbuFlag((Ucbx)i, false);
            }
        }

        protected virtual void OnCheckboxUnsetModified(Ucbx u)
        {
            logger.ConditionalDebug(ConfigBase.cultF, "OnCheckboxUnsetModified, checkbox \'{0}\'", cbuNames[(int)u]);
            EventHandler<CheckboxUnsetModifiedEventArgs> eventHandler = CheckboxUnsetModifiedEvent;
            eventHandler?.Invoke(this, new CheckboxUnsetModifiedEventArgs(u, cbuNames[(int)u]));
        }

        /// <summary>
        /// N'est <c>public</c> que pour les annulations. 
        /// </summary>
        /// <param name="flag">Le flag à modifier.</param>
        /// <param name="val">La nouvelle valeur.</param>
        public void SetCbuFlag(Ucbx flag, bool val)
        {
            logger.ConditionalDebug("SetCbuFlag flag: \'{0}\', val: {1}", flag, val);
            int btuIndex = (int)flag;

            if (act[btuIndex] != val) // Pour éviter un évènement si rien ne change
            {
                UndoFactory.ExceutingAction(new UnsetBehAction("Modifier flag avancé", this, flag, act[btuIndex], val));
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
    }
}
