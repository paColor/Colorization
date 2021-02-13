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

namespace ColorLib
{


    public class SylInW : PhonInW
    {
        public static List<Phonemes> bkptgdfv = new List<Phonemes> { Phonemes.b, Phonemes.k, Phonemes.p, Phonemes.t, Phonemes.g,
        Phonemes.d, Phonemes.f, Phonemes.v, Phonemes.k_qu, Phonemes.g_u };

        public static new void Init()
        {
            bkptgdfv.Sort();
        }

        public SylInW(PhonInW piw)
            : base(piw)
        {}

        public SylInW(PhonWord inW, int inBeg, int inEnd, Phonemes inP)
            : base(inW, inBeg, inEnd, inP)
        {}

        public bool EstBkptgdfv() => (bkptgdfv.BinarySearch(P) >= 0);

        public bool EstConsonneRedoublee()
        {
            return ((Last - First == 1) // le phoneme contient exactement deux lettres
                && PW.GetChar(First) == PW.GetChar(Last) // qui sont égales
                && EstConsonne()); // et il s'agit bien d'une consonne
        }

        // pour une syllabe de deux lettres, se réduit à la première lettre
        public void ReduitAPremiereLettre()
        {
            Debug.Assert(Last - First == 1); // exactement deux lettres
            Last = First;
        }

        public void ReduitADerniereLettre() // exactement deux lettres
        {
            Debug.Assert(Last - First == 1);
            First = Last;
        }

        public void AbsorbeSuivant(SylInW suivant)
        {
            Debug.Assert(suivant.First == Last + 1);
            Last = suivant.Last;
            if (!EstVoyelle() && suivant.EstVoyelle())
            {
                P = suivant.P;
            }
        }

        // enlève les n premières lettres à la syllabe
        // retourne false si la syllabe est vide (plus de lettres)
        public bool ReduitGauche (int n)
        {
            First = First + n;
            return (Last >= First);
        }

        // Etend la syllabe de n caractères vers la droite.
        public void EtendDroite(int n)
        {
            Last = Last + n;
            Debug.Assert (Last <= PW.Last);
        }

        public override void PutColor(Config conf)
        {
            SetCharFormat(conf.sylConf.NextCF());
        }

        public void PutArc(Config conf)
        {
            SetCharFormat(conf.arcConf.NextCF());
        }

        public override string AllStringInfo()
        {
            return base.AllStringInfo();
        }
    }
}
