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

using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib.Dierese
{
    public static class AnalyseDierese
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cherche les diérèses dans le texte <c>tt</c>. Les syllabes des mots concernés sont
        /// modifiées en conséquence.
        /// </summary>
        /// <param name="tt">Le texte du poème. Non <c>null</c>.</param>
        /// <param name="pwL">La liste des <c>PhonWord</c> du poème. Les syllabes ont déjà été
        /// calculées.</param>
        /// <param name="nbrPieds">Le nombre de pieds des vers du poème. 0 si on se contente de la 
        /// détection automatique du nombre de peids voulu.</param>
        /// <returns>La liste des <see cref="ZonePoeme"/> du poème. Peut être utile en cas de test.
        /// </returns>
        public static List<ZonePoeme> ChercheDierese(TheText tt, List<PhonWord> pwL, int nbrPieds)
        {
            logger.ConditionalDebug("ChercheDierese, nbrPieds: {0}", nbrPieds);
            if (tt == null)
            {
                const string Message = "ChercheDierese: tt ne peut pas être null";
                logger.Fatal(Message);
                throw new ArgumentNullException(nameof(tt), Message);
            }
            // créer les zones
            List<ZonePoeme> zL = new List<ZonePoeme>();
            ZonePoeme zpCourante = new ZonePoeme(tt);
            zL.Add(zpCourante);
            int pos = 0;
            while (pos < tt.S.Length)
            {
                Vers v = new Vers(tt, pos, pwL);
                if (!zpCourante.AddVers(v))
                {
                    zpCourante = new ZonePoeme(tt);
                    zL.Add(zpCourante);
                    if (!zpCourante.AddVers(v))
                    {
                        logger.Error("Une zone ne doit pas refuser le premier vers! {0}", v.ToString());
                        throw new InvalidOperationException("Une zone ne doit pas refuser le premier vers!");
                    }
                }
                pos = v.Last + 2; // on saute le caractère de fin de ligne
            }

            // chercher d'éventuelles diérèses
            foreach (ZonePoeme zp in zL)
                zp.ChercheDierese(nbrPieds);
            return zL;
        }
    }
}
