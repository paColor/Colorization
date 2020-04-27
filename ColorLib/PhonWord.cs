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
using System.Diagnostics;
using System.Text;

namespace ColorLib
{
    public class PhonWord : Word
    {
        private List<PhonInW> phons;
        private List<SylInW> syls;
        
        public PhonWord(TheText inT, int inFirst, int inLast)
            : base(inT, inFirst, inLast)
        {
            syls = null;
            phons = new List<PhonInW>((inLast - inFirst) + 1);
            AutomAutomat.autom.FindPhons(this);
        }

        public PhonWord(Word w)
            :base(w)
        {
            syls = null;
            phons = new List<PhonInW>((Last - First) + 1);
            AutomAutomat.autom.FindPhons(this);
        }

        public void AddPhon(PhonInW piw)
        {
            phons.Add(piw);
            Debug.Assert(phons.Count <= GetWord().Length);
        }
            
        public void ColorPhons(PhonConfType pct)
        {
            foreach (PhonInW piw in phons)
                piw.PutColor(pct);
        }

        public void ComputeAndColorSyls()
        {
            SylInW siw;
            int i, j;
            SylConfig sylConfig = this.T.GetConfig().sylConf;

            // Algorithme de Marie-Pierre
            if (syls == null)
            {
                syls = new List<SylInW>((Last - First) / 2);

                // créons une syllabe pour chaque phonème
                for (i = 0; i < phons.Count; i++)
                    syls.Add(new SylInW(phons[i]));

                if (syls.Count > 1)
                {
                    // Si le décodage est standard dupliquer les phonèmes qui comportent des consonnes doubles
                    if (sylConfig.DoubCons())
                    {
                        for (i = 0; i < syls.Count; i++)
                        {
                            if (syls[i].EstConsonneRedoublee())
                            {
                                siw = new SylInW(syls[i]);
                                syls[i].ReduitADerniereLettre();
                                siw.ReduitAPremiereLettre();
                                syls.Insert(i, siw);
                            }
                        }
                    }

                    // mixer les doubles phonèmes de consonnes qui incluent [l] et [r] ; ex. : bl, tr, cr, chr, pl
                    // mixer les doubles phonèmes [y] et [i], [u] et [i,e_tilda,o_tilda]
                    // accrocher les lettres muettes aux lettres qui précèdent
                    for (i = 0; i < syls.Count - 1; i++)
                    {
                        if ((syls[i].EstBkptgdfv() && ((syls[i+1].P == Phonemes.l) || (syls[i + 1].P == Phonemes.R)))  // [bkptgdfv][lR]
                            ||
                            (((syls[i].P == Phonemes.y) && (syls[i + 1].P == Phonemes.i))  // ui
                             ||
                             ((syls[i].P == Phonemes.u) && ((syls[i+1].P == Phonemes.i) || (syls[i+1].P == Phonemes.e_tilda) || (syls[i+1].P == Phonemes.o_tilda))) // u(i|e_tilda|o_tilda)
                            )
                            ||
                            syls[i + 1].EstMuet()
                            )
                        {
                            // mixer les deux phonèmes puis raccourcir la chaîne
                            syls[i].AbsorbeSuivant(syls[i + 1]);
                            syls.RemoveAt(i + 1);
                            i--; // faire en sorte que la prochaine itération considère le nouveau phonème fusionné et son successeur
                        } 
                    }

                    // construire les syllabes par association de phonèmes consonnes et voyelles
                    // Les syllabes sont constituées de tout ce qui précède un phonème voyelle jusqu'à la syllabe précédente ou le début du mot.
                    // De plus si le phonème voyelle est suivi de deux consonnes, la première fait partie de la première syllabe.

                    i = 0;
                    j = 0; // début de la syllabe
                    while (i < syls.Count)
                    {
                        if(syls[i].EstVoyelle())
                        {
                            // fusionner les syllabes de j à i
                            for (int k = 0; k<(i-j); k++)
                            {
                                syls[j].AbsorbeSuivant(syls[j + 1]);
                                syls.RemoveAt(j + 1);
                            }
                            i = j;
                            j++;

                            // si les deux lettres qui suivent sont des consonnes, la première fait partie de la syllabe que nous venons de créer
                            // A condition qu'elles ne soient pas toutes les deux dans la même syllabe.
                            if (j < syls.Count)
                            {
                                int pos = syls[j].First; // position de la lettre suivante dans le texte sous-jacent
                                if ((syls[j].Last == syls[j].First) & (pos < this.Last) && EstConsonne(GetChar(pos)) && EstConsonne(GetChar(pos+1))) 
                                {
                                    syls[j - 1].EtendDroite(1);
                                    if (!syls[j].ReduitGauche(1))
                                        syls.RemoveAt(j);
                                }
                            }
                        }
                        i++;
                    } // while

                    // précaution de base : si pas de syllabes reconnues, on concatène simplement les phonèmes
                    if (j == 0)
                    {
                        // le mot ne comprend pas de voyelles --> une seule syllabe
                        syls.Clear();
                        siw = new SylInW(this, this.First, this.Last, Phonemes.firstPhon);
                        syls.Add(siw);
                    } 
                    else 
                    {
                        // il ne doit rester à la fin que les lettres muettes ou des consonnes qu'on ajoute à la dernière syllabe
                        while (j < syls.Count)
                        {
                            syls[j-1].AbsorbeSuivant(syls[j]);
                            syls.RemoveAt(j);
                            j++;
                        }
                    }

                    // si nous sommes en mode oral, les e caducs des dernières syllabes doivent être concaténés avec la syllabe précédente
                    if ((!sylConfig.ModeEcrit()) && (syls.Count > 1) && (phons[phons.Count-1].P == Phonemes.q_caduc)) // s'il y a plus d'une syllabe, il y a aussi plus d'un phonème
                    {
                        syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                        syls.RemoveAt(syls.Count - 1);
                    }
                } // if (syls.Count > 1)
            } // if (syls == null)

            // Mettre les syllabes en couleur
            foreach(SylInW s in syls)
                s.PutColor();

            // si on est en mode écrit, marquer par-dessus les phonemes muets.
            if (sylConfig.ModeEcrit())
                foreach(PhonInW piw in phons)
                    if (piw.EstMuet())
                        piw.PutColor(PhonConfType.muettes);
        }

        // returns the phonetical representation of the PhonWord (notation from lexique.org)
        public string Phonetique()
        {
            StringBuilder sb = new StringBuilder(GetWord().Length - 1);
            foreach (PhonInW piw in phons)
            {
                sb.Append(piw.Phon2String());
            }
            return sb.ToString();
        }

        public override string AllStringInfo ()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.AllStringInfo());

            sb.Append("phons:");
            if (phons != null)
            {
                sb.AppendLine("");
                foreach (PhonInW piw in phons)
                    sb.AppendLine(piw.AllStringInfo());
            } else
                sb.AppendLine(" null");

            sb.Append("syls:");
            if (syls != null)
            {
                sb.AppendLine("");
                foreach (SylInW siw in syls)
                    sb.AppendLine(siw.AllStringInfo());
            } else
                sb.AppendLine(" null");

            return sb.ToString();
        }

        // for debugging returns the word with hyphens '-' betheen the syllabes
        public string Syllabes()
        {
            StringBuilder sb = new StringBuilder(GetWord().Length+4);
            for (int i = 0; i<syls.Count; i++) 
            {
                sb.Append(syls[i].ToString());
                if (i < syls.Count-1)
                    sb.Append("-");
            }
            return sb.ToString();
        }

        // For test purpose, clear all phons
        public void ClearPhons() => phons.Clear();

    }
}
