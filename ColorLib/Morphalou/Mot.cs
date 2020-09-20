using ColorLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace ColorLib.Morphalou
{
    public class Mot
    {
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        // * * * * * * * * * * * * * * * * * * *   S T A T I C   * * * * * * * * * * * * * * * * *
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *

        public static List<Mot> mots = new List<Mot>(396000);

        private static int progressCount;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            logger.Debug("Init");
            TheText.Init();
        }


        /// <summary>
        /// S'assure que les champs <c>sampa1</c>, <c>sampa2</c> et <c>col</c> sont calculés pour
        /// chaque <c>Mot</c> dans <c>mots</c>.
        /// </summary>
        /// <param name="conf">Configuration à utiliser pour le cas où il faut calculer
        /// <c>col</c>.</param>
        /// <param name="recompute">Indique si <c>col</c> doit être recalculé dans tous les
        /// cas.</param>
        public static void EnsureCompleteness(Config conf, bool recompute, bool writeprogress = false)
        {
            logger.Debug("EnsureCompleteness");
            progressCount = 0;
            Parallel.ForEach(mots, (m) => { m.EnsureComplete(conf, recompute, writeprogress); });
        }

        public static void DumpMotsFiltered(StreamWriter matchS, StreamWriter unMatchS)
        {
            logger.Debug("DumpMotsFiltered");
            WriteHeader(matchS);
            WriteHeader(unMatchS);
            foreach (Mot m in mots)
            {
                m.WriteToFile(matchS, unMatchS);
            }
        }

        public static void DumpPourTests(StreamWriter sw)
        {
            const int MaxCperLine = 90;
            int posLine = 20;

            sw.Write("const string txt = @\"");
            foreach (Mot m in mots)
            {
                sw.Write(m.graphie);
                sw.Write(" ");
                posLine += m.graphie.Length + 1;
                if (posLine > MaxCperLine)
                {
                    sw.WriteLine();
                    posLine = 0;
                }

            }
            sw.Write("\";");
            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine(@"string[] phonetique = new string[]");
            sw.WriteLine(@"{");
            posLine = 0;
            foreach (Mot m in mots)
            {
                sw.Write("\"");
                sw.Write(m.col);
                sw.Write("\", ");
                posLine += m.col.Length + 4;
                if (posLine > MaxCperLine)
                {
                    sw.WriteLine();
                    posLine = 0;
                }
            }
            sw.WriteLine(@"};");
        }

        /// <summary>
        /// Écrit l'en-tête du fichier csv.
        /// </summary>
        /// <param name="sw"></param>
        public static void WriteHeader(StreamWriter sw)
        {
            logger.Debug("WriteHeader");
            sw.WriteLine("GRAPHIE;ID;MORPHALOU1;MORPHALOU2;SAMPA1;SAMPA2;COLORIZATION");
        }

        /// <summary>
        /// Compare les deux strings. retourne dans <paramref name="diffs"/> un flag 
        /// <c>true</c> pour chaque position différente.
        /// </summary>
        /// <param name="ph">premier <c>string</c> à comparer.</param>
        /// <param name="col">deuxième <c>string</c> à comparer.</param>
        /// <param name="diffs">Liste des positions où les deux <c>string</c>(s)
        /// diffèrent.</param>
        /// <returns></returns>
        private static bool AreEqualPhon(string ph, string col, out List<int> diffs)
        {
            logger.Trace("AreEqualPhon \'{0}\', \'{1}\'", ph, col);
            bool toReturn = true;
            diffs = new List<int>();
            for (int i = 0; i < Math.Max(ph.Length, col.Length); i++)
            {
                char cPh, cCol;
                if (i < ph.Length)
                {
                    cPh = ph[i];
                }
                else
                {
                    cPh = '$';
                }
                if (i < col.Length)
                {
                    cCol = col[i];
                }
                else
                {
                    cCol = '$';
                }
                if (cPh != cCol 
                    && !(cPh == 'ë' && (cCol == 'e' || cCol == 'E'))
                    && !(cPh == 'ü' && (cCol == 'u' || cCol == 'y'))
                    )
                {
                    toReturn = false;
                    diffs.Add(i);
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Retourne la liste des versions de <paramref name="s"/> où il manque un '°'
        /// </summary>
        /// <param name="s">Le <c>string</c> à raccourcir.</param>
        /// <returns>Un eliste de strings correspondant à <paramref name="s"/> raccourci
        /// d'un '°'. La liste est vide s'il n'en contient pas.</returns>
        private static List<string> RemoveSchwas(string s)
        {
            List<string> toReturn = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '°')
                {
                    string subS = s.Remove(i, 1);
                    toReturn.Add(subS);
                }
            }
            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphie"></param>
        /// <param name="ph1">La représentation phonétique morphalou</param>
        /// <param name="col">La représentation phonétique de colorization</param>
        /// <returns></returns>
        private static bool AreEqualButSchwa(string graphie, string ph1, string col)
        {
            List<string> sL = RemoveSchwas(ph1);
            foreach (string sampa in sL)
            {
                if (AreMatch(graphie, sampa, col))
                    return true;
            }

            sL = RemoveSchwas(col);
            foreach (string c in sL)
            {
                if (AreMatch(graphie, ph1, c))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphie"></param>
        /// <param name="ph1">La représentation phonétique morphalou</param>
        /// <param name="col">La représentation phonétique de colorization</param>
        /// <returns></returns>
        private static bool AreMatch(string graphie, string ph1, string col)
        {
            if (String.IsNullOrEmpty(ph1))
                return false; // col vide ne nous intéresse de toute façon pas.

            if (ph1 == col)
                return true;

            // s'il s'agit d'un 'b' qu'on peut prononcer 'p'
            List<int> diffs;
            bool equality = AreEqualPhon(ph1, col, out diffs);
            if (equality)
                return true; // il peut s'agir de ü ou ë

            int pos = diffs[0];

            if (pos < col.Length)
            {
                if (pos > 0 && col[pos] == col[pos - 1])
                {
                    // La version colorization double parfois certains sons, ce qui n'est 
                    // pas un problème quand on colorise. 
                    if (AreMatch(graphie, ph1, col.Remove(pos, 1)))
                        return true;
                }

                if (pos < ph1.Length)
                {
                    if (pos < graphie.Length
                        && graphie[pos] == 'b' // il est clair que l'index dans grpahie peut vite diverger
                        && ph1[pos] == 'p'
                        && col[pos] == 'b')
                    {
                        // lettre 'b' dans le texte, son [p] morphalou et [b] colorization
                        return true;
                    }

                    if (pos > 2
                        && ph1[pos] == 'j'
                        && ph1[pos - 1] == 'a'
                        && ph1[pos - 2] == 'w'
                        )
                    {
                        // Il es possible qu'on ait un mot comme "aboya" que Morphalou voit
                        // comme "abwaja" mais où Colorization ne voit pas le [j]. Morphalou
                        // a raison, mais ça n'a aucune importance pour la fonctionalité de
                        // Colorization puisque Colorization ne peut associer qu'un son (une
                        // couleur) à une lettre.
                        // On pourrait en théorie introduire un nouveau 'phonème' pour oy
                        // qui correspondrait à la phonétique [waj]. Ce serait correct mais
                        // inutile pour coloriser correctement.
                        string newPh1 = ph1.Remove(pos, 1);
                        if (AreMatch(graphie, newPh1, col))
                            return true;
                    }

                    if (pos > 3
                        && ph1[pos] == 'i'
                        && ph1[pos - 1] == 'j'
                        && ph1[pos - 2] == 'a'
                        && ph1[pos - 3] == 'w')
                    {
                        // pour aboyiez... Morphalou met [ji] pour le 'yi', Colorization conna'it
                        // le son [oi] et met [j] pour le 'y'.
                        string newPh1 = ph1.Remove(pos - 1, 1);
                        if (AreMatch(graphie, newPh1, col))
                            return true;
                    }

                    if (((col[pos] == 'i' && ph1[pos] == 'j')
                        || (col[pos] == 'j' && ph1[pos] == 'i'))
                        && pos < ph1.Length - 1
                        && pos < col.Length - 1
                        && col[pos + 1] == ph1[pos + 1]
                        && PhonInW.IsPhonVoyelle(PhonInW.ColSE2phon(col[pos + 1]))
                       )
                    {
                        // Col [ij], Morph [ji] mais suivi de le même voyelle
                        return true;
                    }

                    if (pos < col.Length - 1 && pos < ph1.Length - 1
                        && col[pos] == 'u' && ph1[pos] == 'w'
                        && (col[pos + 1] == 'a' && ph1[pos + 1] == 'a'
                        || col[pos + 1] == 'i' && ph1[pos + 1] == 'i'))
                    {
                        // col [ua] et morph [wa], ou alors col [ui] et morph [wi].
                        StringBuilder sb = new StringBuilder(col.Length);
                        sb.Append(col.Substring(0, pos));
                        sb.Append('w'); // à la place du 'u'
                        sb.Append(col.Substring(pos + 1, col.Length - (pos + 1)));
                        if (AreMatch(graphie, ph1, sb.ToString()))
                            return true;
                    }

                    if (pos < col.Length - 2
                        && col[pos] == 'i'
                        && col[pos + 1] == 'j'
                        && ph1[pos] == 'j'
                        && PhonInW.IsPhonVoyelle(PhonInW.ColSE2phon(col[pos + 2])))
                    {
                        // Morphalou a peut-être seulement le [j]. 
                        // S'il y a une voyelle derrière, c'est équivalent.
                        // Enlevons le [i]
                        string newCol = col.Remove(pos, 1);
                        if (AreMatch(graphie, ph1, newCol))
                            return true;
                    }

                    if (col[pos] == 'N' && ph1[pos] == 'n'
                        && pos < col.Length - 1 && pos < ph1.Length - 1
                        && col[pos+1] == 'j' && ph1[pos + 1] == 'j')
                    {
                        // [nj] est équivalent à [Nj]
                        if (AreMatch(graphie, ph1.Remove(pos, 1), col.Remove(pos, 1)))
                            return true;
                    }
                    
                    if (col[pos] == 'j' && ph1[pos] == 'i'
                        && pos < col.Length - 1 && pos < ph1.Length - 1
                        && col[pos + 1] == '°' && ph1[pos + 1] == '°')
                    {
                        // [nj] est équivalent à [Nj]
                        if (AreMatch(graphie, ph1.Remove(pos, 1), col.Remove(pos, 1)))
                            return true;
                    }

                    if (// pos == col.Length - 1 && pos == ph1.Length - 1 && 
                        col[pos] == 'i' && ph1[pos] == 'j')
                    {
                        // Morphalou a [j] et Colorization [i]... Pas seulement en fin de mot
                        if (AreMatch(graphie, ph1.Remove(pos, 1), col.Remove(pos, 1)))
                            return true;
                    }

                    if ((col[pos] == 'u' && ph1[pos] == 'w')
                        || (col[pos] == 'j' && ph1[pos] == 'i')
                        )
                    {
                        // 'w' et 'u' sont équivalents
                        if (AreMatch(graphie, ph1.Remove(pos, 1), col.Remove(pos, 1)))
                            return true;
                    }

                } // if (pos < ph1.Length)

                if (pos > 2 && pos - 1 < ph1.Length
                    && col[pos] == 'j'
                    && col[pos - 1] == 'i'
                    && ph1[pos - 1] == 'i'
                    && PhonInW.IsPhonConsonne(PhonInW.ColSE2phon(col[pos - 2]))
                    && PhonInW.IsPhonConsonne(PhonInW.ColSE2phon(col[pos - 3])))
                {
                    // [ij] dans colorization et Morphalou a évtlmt seulement le [i
                    // ça pourrait être la fameuse règle 'prec_2cons' que nous considérons
                    // juste, mais où Morphalou omet le j.
                    // recalculons 'col' sans le son [j]
                    string newCol = col.Remove(pos, 1);
                    if (AreMatch(graphie, ph1, newCol))
                        return true;
                }

                if (col[pos] == 'j' && pos > 0 && col[pos - 1] == 'N')
                {
                    // gnions - N§ - Nj§
                    if (AreMatch(graphie, ph1, col.Remove(pos, 1)))
                        return true;
                }

            } // if (pos < col.Length)

            if (pos > 0 && pos < ph1.Length - 1 
                && ph1[pos] == 'i' && ph1[pos - 1] == 'j'
                && PhonInW.IsPhonVoyelle(PhonInW.ColSE2phon(ph1[pos + 1])))
            {
                // Morphalou a [ji] suivi d'une voyelle, Col n'a pas le [i].
                if (AreMatch(graphie, ph1.Remove(pos, 1), col))
                    return true;
            }

            if (pos > 0 && pos < ph1.Length - 2
                && ph1[pos] == 'i' && ph1[pos + 1] == 'j'
                && PhonInW.IsPhonVoyelle(PhonInW.ColSE2phon(ph1[pos + 2])))
            {
                // Morphalou a [ji] suivi d'une voyelle, Col n'a pas le [i].
                if (AreMatch(graphie, ph1.Remove(pos, 1), col))
                    return true;
            }

            if (pos < ph1.Length)
            {
                if (pos > 0 && ph1[pos - 1] == ph1[pos])
                {
                    // Morphalou a doublé un son qui n'apparaît qu'une fois pour colorization.
                    // Si le reste est équivalent, cela n'a pas d'importance pour coloriser.
                    if (AreMatch(graphie, ph1.Remove(pos, 1), col))
                        return true;
                }
            }

            if (AreEqualButSchwa(graphie, ph1, col))
                return true;
            // La récursion ne doit pas se faire plus d'une fois

            return false;
        }

        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        // * * * * * * * * * * * * * * *   I N S T A N T I A T E D   * * * * * * * * * * * * * * *
        // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *

        private string id;
        private string graphie;
        private string morph; // format morphalou peut contenir "OU"
        private string morph1; // format morphalou sans "OU"
        private string morph2; // format morphalou sans "OU"
        private string sampa1; // format ColSimpl
        private string sampa2; // format ColSimpl
        private string col; // format ColSimpl
        public bool match;
        public bool matchSet;

        public Mot(string[] fields)
        {
            if (fields.Length == 3)
            {
                // headerLine = "GRAPHIE;ID;MORPHALOU"
                graphie = fields[0];
                id = fields[1];
                morph = fields[2];

                morph1 = null;
                morph2 = null;
                sampa1 = null;
                sampa2 = null;
                col = null;
            }
            else if (fields.Length == 5)
            {
                // headerLine = "GRAPHIE;ID;MORPHALOU1;MORPHALOU2;COLORIZATION";
                graphie = fields[0];
                id = fields[1];
                morph = null;
                morph1 = fields[2];
                morph2 = fields[3];
                sampa1 = null;
                sampa2 = null;
                col = fields[4];
            }
            else if (fields.Length == 7)
            {
                // headerLine = "GRAPHIE;ID;MORPHALOU1;MORPHALOU2;SAMPA1;SAMPA2;COLORIZATION";
                graphie = fields[0];
                id = fields[1];
                morph = null;
                morph1 = fields[2];
                morph2 = fields[3];
                sampa1 = fields[4];
                sampa2 = fields[5];
                col = fields[6];
            }
            else
            {
                throw new ArgumentException("Ne fonctionne qu'avec des lignes de 3, 5 ou 7 éléments.");
            }
            matchSet = false;
            mots.Add(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("graphie:  " + graphie);
            sb.AppendLine("id:       " + id);
            sb.AppendLine("morph:    " + morph != null ? morph : "null");
            sb.AppendLine("morph1:   " + morph1 != null ? morph1 : "null");
            sb.AppendLine("morph2:   " + morph2 != null ? morph2 : "null");
            sb.AppendLine("sampa1:   " + sampa1 != null ? sampa1 : "null");
            sb.AppendLine("sampa2:   " + sampa2 != null ? sampa2 : "null");
            sb.AppendLine("col:      " + col != null ? col : "null");
            sb.AppendLine("matchSet: " + matchSet.ToString());
            sb.AppendLine("match:    " + match.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Retourne les champs du <c>Mot</c> sous forme d'une ligne de valeurs séparées par des
        /// point-virgules.
        /// </summary>
        /// <returns>Le texte d'une ligne de fichier csv.</returns>
        public string GetFileString()
        {
            return String.Format("{0};{1};{2};{3};{4};{5};{6}", graphie, id, morph1, morph2, sampa1, sampa2, col);
        }

        /// <summary>
        /// Écrit le mot dans 5 champs séparés par des ';'. Si col == sampa1 ou 2 dans
        /// <paramref name="matchS"/> sinon dans <paramref name="unMatchS"/>.
        /// </summary>
        /// <param name="matchS">Fichier (en fait stream) pour les mots qui "matchent".</param>
        /// <param name="unMatchS">Fichier (stream) pour les mots qui ne "matchent" pas.</param>
        public void WriteToFile(StreamWriter matchS, StreamWriter unMatchS)
        {
            StreamWriter sw;
            Debug.Assert(matchSet);
            if (match)
            {
                sw = matchS;
            }
            else
            {
                sw = unMatchS;
            }
            sw.WriteLine(GetFileString());
        }

        /// <summary>
        /// S'assure que les champs <c>sampa1</c>, <c>sampa2</c> et <c>col</c> sont calculés pour
        /// le <c>Mot</c>.
        /// </summary>
        /// <param name="conf">Configuration à utiliser pour le cas où il faut calculer
        /// <c>col</c>.</param>
        /// <param name="recompute">Indique si <c>col</c> doit être recalculé dans tous les
        /// cas.</param>
        /// <param name="writeProgress">Indique s'il faut afficher une info de progression
        /// sur la console.</param>
        public void EnsureComplete(Config conf, bool recompute = false, bool writeProgress = false)
        {
            if (recompute || col == null)
            {
                TheText tt = new TheText(graphie);
                List<PhonWord> pws = tt.GetPhonWordList(conf);
                col = NotationsPhon.C2CS(pws[0].Phonetique());
            }
            if (morph1 == null)
            {
                int posOU = morph.IndexOf("OU");
                if (posOU > 0)
                {
                    morph1 = morph.Substring(0, posOU);
                    morph2 = morph.Substring(posOU + 3, morph.Length - (posOU + 3));
                }
                else
                {
                    morph1 = morph;
                    morph2 = "";
                }
            }
            if (sampa1 == null)
            {
                sampa1 = NotationsPhon.S2CS(morph1);
                sampa2 = NotationsPhon.S2CS(morph2);
            }
            if (!matchSet)
            {
                match = Match();
                matchSet = true;
            }

            if (writeProgress)
            {
                if (progressCount % 5000 == 0)
                {
                    Console.WriteLine(progressCount);
                }
                Interlocked.Increment(ref progressCount);
            }
        }

        private bool Match()
        {
            if (graphie.Contains(" ")
                || graphie.Contains("-")
                || graphie.Contains(@"'")
                || graphie.Contains(@".")
               )
            {
                return true;
            }
            return AreMatch(graphie, sampa1, col) || AreMatch(graphie, sampa2, col);
        }

    }
}
