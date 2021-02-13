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

using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib.Dierese
{
    /// <summary>
    /// Représente une zone de plusieurs vers consécutifs dans un poème. Ces vers ont à peu près
    /// le même nombre de pieds. L'hypothèse est qu'ils devraient tous en compter exactement le
    /// même nombre et que les pieds qui manquent sont des diérèses qu'il reste à trouver...
    /// </summary>
    public class ZonePoeme : TextEl
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Au fur et à mesure de la construction de la zone, un algorithme estime le nombre de 
        /// pieds que devraient contenir les vers de la zone. 
        /// </summary>
        /// <value>Nombre de pieds calculé au fur et à mesure de l'ajout de vers.</value>
        public int nrPiedsVoulu { get; private set; }
        
        /// <summary>
        /// Liste (ordonnée) des vers de la zone.
        /// </summary>
        public List<Vers> vList { get; private set; }

        private const float DeltaMoins = 2.7f;
        private const float DeltaPlus = 2.1f;
        private float nrPiedsMoyen;

        /// <summary>
        /// Crée une <see cref="ZonePoeme"/> vide pour le texte <paramref name="tt"/>.
        /// </summary>
        /// <param name="tt"><see cref="TheText"/> sur lequel est construit la 
        /// <see cref="ZonePoeme"/>.</param>
        /// <exception cref="ArgumentNullException"> si <paramref name="tt"/> est <c>null</c>
        /// </exception>
        public ZonePoeme(TheText tt)
            : base(tt, 0, -1) // élément vide
        {
            nrPiedsMoyen = 0.0f;
            nrPiedsVoulu = 0;
            vList = new List<Vers>();
        }

        /// <summary>
        /// Ajout le vers <c>v</c> à la zone. 
        /// </summary>
        /// <param name="v">Le vers à ajouter à la zone. Non <c>null</c>.</param>
        /// <returns><c>true</c>: Le vers a été ajouté à la zone. <c>false</c>: le vers a un nombre
        /// de pieds trop éloigné des autres vers de la zone. Il fait partie d'une autre zone.
        /// Il n'a pas été ajouté.</returns>
        /// <exception cref="ArgumentNullException"> si <paramref name="v"/> est <c>null</c>.</exception>
        public bool AddVers(Vers v)
        {
            if (v == null)
            {
                logger.Fatal("Vers null ajouté à la zone.");
                throw new ArgumentNullException(nameof(v), "Vers null ajouté à la zone.");
            }
            bool toReturn = false;
            if (vList.Count == 0)
            {
                vList.Add(v);
                nrPiedsMoyen = v.nrPieds;
                nrPiedsVoulu = v.nrPieds;
                First = v.First;
                Last = v.Last;
                toReturn = true;
            }
            else if ((nrPiedsMoyen >= v.nrPieds && nrPiedsMoyen - v.nrPieds < DeltaMoins)
                || (nrPiedsMoyen < v.nrPieds && v.nrPieds - nrPiedsMoyen < DeltaPlus))
            {
                nrPiedsMoyen =  ((nrPiedsMoyen * vList.Count) + v.nrPieds) / (vList.Count + 1);
                nrPiedsVoulu = (int)nrPiedsMoyen;
                if (nrPiedsVoulu < (nrPiedsMoyen - (0.3f)))
                    nrPiedsVoulu++;
                vList.Add(v);
                Last = v.Last;
                toReturn = true;
            }
            return toReturn;
        }

        /// <summary>
        /// Pour tous les vers de la zone dont le nombre de pieds est plus petit que le nombre de
        /// pieds voulu, on cherche s'il n'y aurait pas une diérèse qui pourrait augmenter le
        /// nombre de pieds du vers.
        /// </summary>
        /// <param name="nrPieds">Nombre de pieds à rechercher. 0 s'il faut prendre le nombre de
        /// pieds estimé par l'algorithme.</param>
        /// /// <exception cref="ArgumentOutOfRangeException">Si <c>nbrPiedsVoulu</c> est plus
        /// petit que zéro.</exception>
        public void ChercheDierese(int nrPieds)
        {
            if (nrPieds > 0)
                nrPiedsVoulu = nrPieds;
            else if (nrPieds < 0)
            {
                logger.Fatal("Nombre de pieds voulu négatif: {0}", nrPieds);
                throw new ArgumentOutOfRangeException(nameof(nrPieds), 
                    "Nombre de pieds voulu négatif: " + nrPieds.ToString());
            }

            if (nrPiedsVoulu <= Vers.MaxNrPieds)
            {
                foreach (Vers v in vList)
                {
                    if (v.nrPieds < nrPiedsVoulu)
                    {
                        v.ChercheDierese(nrPiedsVoulu);
                    }
                }
            }
            else
            {
                logger.Info("Nombre de pieds demandé({0}) trop grand pour chercher les diérèses",
                    nrPiedsVoulu);
            }
        }

        /// <summary>
        /// Retourne la zone sous forme de texte où les mots sont écrits en syllabes séparées
        /// par des tirets.
        /// </summary>
        /// <returns>Le texte en syllabes.</returns>
        public string Syllabes()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vList.Count; i++)
            {
                sb.Append(vList[i].Syllabes());
                if (i < vList.Count - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
