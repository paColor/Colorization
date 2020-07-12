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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ColorLib
{
    public class PhonWord : Word
    {
        private enum ComportementMotSuivant { voyelle, consonne, fin, undef }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private List<PhonInW> phons;
        private List<SylInW> syls;
        
        /// <summary>
        /// Crée un <c>PhonWord</c>.
        /// </summary>
        /// <param name="inT">Le <see cref="TheText"/> sur le quel est construit le <c>PhonWord</c>.</param>
        /// <param name="inFirst">La position de la première lettre (zero based) du mot dans le texte.</param>
        /// <param name="inLast">La position de la dernière lettre du mot dans le texte.</param>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour la détection des phonèmes.</param>
        public PhonWord(TheText inT, int inFirst, int inLast, Config conf)
            : base(inT, inFirst, inLast)
        {
            syls = null;
            phons = new List<PhonInW>((inLast - inFirst) + 1);
            AutomAutomat.autom.FindPhons(this, conf);
        }

        /// <summary>
        /// Crée un <c>PhonWord</c> sur la base d'un <c>Word</c>.
        /// </summary>
        /// <param name="w">Le <c>Word</c> sur la base duquel le <c>PhonWord</c> doit être créé.</param>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour la détection des phonèmes.</param>
        public PhonWord(Word w, Config conf)
            :base(w)
        {
            syls = null;
            phons = new List<PhonInW>((Last - First) + 1);
            AutomAutomat.autom.FindPhons(this, conf);
        }

        /// <summary>
        /// Ajout un phonème (<see cref="PhonInW"/>) au <c>PhonWord</c>.
        /// </summary>
        /// <param name="piw">Le <see cref="PhonInW"/> à ajouter.</param>
        public void AddPhon(PhonInW piw)
        {
            phons.Add(piw);
            Debug.Assert(phons.Count <= GetWord().Length);
        }
          
        /// <summary>
        /// Applique le formatage de phonèmes défini dans le <see cref="PhonConfType"/> <c>pct</c> de 
        /// la <see cref="Config"/> <c>conf</c>. 
        /// </summary>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour le formatage des phonèmes.</param>
        /// <param name="pct">Le <see cref="PhonConfType"/> définissant s'il s'agit de la configuration pour
        /// les phonèmes, les muettes, ...</param>
        public void ColorPhons(Config conf, PhonConfType pct)
        {
            foreach (PhonInW piw in phons)
                piw.PutColor(conf, pct);
        }

        /// <summary>
        /// Calcule les syllabes avec la <see cref="Config"/> donnée. La liste 'syls' est remplie.
        /// </summary>
        /// <param name="conf">La <c>Config</c> à utiliser pour savoir quelles options sont choisies 
        /// et le formatage à appliquer pour les syllabes.</param>
        public void ComputeSyls(Config conf)
        {
            logger.ConditionalTrace(BaseConfig.cultF, "ComputeAndColorSyls {0}", GetWord());
            SylInW siw;
            int i, j;
            SylConfig sylConfig = conf.sylConf;

            // Algorithme de Marie-Pierre
            if (syls == null)
            {
                syls = new List<SylInW>((Last - First) / 2);

                // créons une syllabe pour chaque phonème
                for (i = 0; i < phons.Count; i++)
                    syls.Add(new SylInW(phons[i]));

                logger.ConditionalTrace("Etape 1 {0} --> {1}, {2}", GetWord(), Syllabes(), GetPhonSyllabes());

                if (syls.Count > 1)
                {
                    // Si le décodage est standard dupliquer les phonèmes qui comportent des consonnes doubles
                    if (sylConfig.DoubleConsStd)
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
                    logger.ConditionalTrace("Etape 2 {0} --> {1}, {2}", GetWord(), Syllabes(), GetPhonSyllabes());

                    // Il y a une série de cas spéciaux où deux sons ne forment qu'une syllabe
                    // par exemple [bkptgdfv][lR] ou [y][i] ou [u]([i]|[e_tilda]|[o_tilda])
                    // (la notation est un peu libre :-)
                    for (i = 0; i < syls.Count - 1; i++)
                    {
                        if (FusionnerSyllabes(syls[i], syls[i + 1]))
                        {
                            // mixer les deux phonèmes puis raccourcir la chaîne
                            syls[i].AbsorbeSuivant(syls[i + 1]);
                            syls.RemoveAt(i + 1);
                            logger.ConditionalTrace("Etape 3-{0} {1} --> {2}, {3}", i, GetWord(), Syllabes(), GetPhonSyllabes());
                            i--; // faire en sorte que la prochaine itération considère le nouveau
                                 // phonème fusionné et son successeur
                        }
                    }

                    // construire les syllabes par association de phonèmes consonnes et voyelles
                    // Les syllabes sont constituées de tout ce qui précède un phonème voyelle 
                    // jusqu'à la syllabe précédente ou le début du mot.
                    // De plus si le phonème voyelle est suivi de deux consonnes, la première fait
                    // partie de la première syllabe.

                    i = 0;
                    j = 0; // début de la syllabe
                    while (i < syls.Count)
                    {
                        logger.ConditionalTrace("Etape fusion consonnes et voyelles i:{0}, j:{1} {2} --> {3}, {4}"
                            , i, j, GetWord(), Syllabes(), GetPhonSyllabes());
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
                            logger.ConditionalTrace("Etape 4A i:{0}, j:{1} {2} --> {3}, {4}", 
                                i, j, GetWord(), Syllabes(), GetPhonSyllabes());

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
                            logger.ConditionalTrace("Etape 4B i:{0}, j:{1} {2} --> {3}, {4}",
                                i, j, GetWord(), Syllabes(), GetPhonSyllabes());
                        }
                        i++;
                    } // while

                    logger.ConditionalTrace("Etape 5 i:{0}, j:{1} {2} --> {3}, {4}",
                        i, j, GetWord(), Syllabes(), GetPhonSyllabes());

                    // précaution de base : si pas de syllabes reconnues, on concatène simplement les phonèmes
                    if (j == 0)
                    {
                        // le mot ne comprend pas de voyelles --> une seule syllabe
                        syls.Clear();
                        siw = new SylInW(this, this.First, this.Last, Phonemes.firstPhon);
                        syls.Add(siw);
                        logger.ConditionalTrace("Etape 6A i:{0}, j:{1} {2} --> {3}, {4}",
                            i, j, GetWord(), Syllabes(), GetPhonSyllabes());
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
                        logger.ConditionalTrace("Etape 6B i:{0}, j:{1} {2} --> {3}, {4}",
                            i, j, GetWord(), Syllabes(), GetPhonSyllabes());
                    }
                    // ###############################################################################
                    // # Traitement spécial de de la dernière syllabe dans les modes oral et poésie. #
                    // ###############################################################################

                    if ((syls.Count > 1) && (syls[syls.Count - 1].P == Phonemes.q_caduc)) 
                    {
                        // s'il y a plus d'une syllabe, il y a aussi plus d'un phonème
                        if (sylConfig.mode == SylConfig.Mode.oral)
                        {
                            // si nous sommes en mode oral, les e caducs des dernières syllabes
                            // doivent être concaténés avec la syllabe précédente
                            syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                            syls.RemoveAt(syls.Count - 1);
                            logger.ConditionalTrace("Etape 7A {0} --> {1}, {2}", 
                                GetWord(), Syllabes(), GetPhonSyllabes());
                        }
                        else if (sylConfig.mode == SylConfig.Mode.poesie)
                        {
                            logger.ConditionalTrace("Mode poésie. Syllabes avant le traitement: {0}",
                                Syllabes());
                            // voir http://mamiehiou.over-blog.com/article-versification-comment-compter-les-pieds-syllabes-d-un-vers-97149081.html
                            // dont nous nous inspirons ici. Il faut toutefois noter que quand le 
                            // "e" ne compte pas pour un pied, nous le relions simplement avec la
                            // syllabe précédente, ce qui n'est pas tout à fait correct au niveau
                            // de la prononciation.

                            // En gros on peut dire que si le mot suivant commence par une voyelle
                            // (ou équivalent), le e-caduc ne se prononce pas, sauf s'il y a une laison.
                            // Si le mot suivant commence par une consonne (ou équivalent) le e-caduc
                            // se prononce.
                            
                            string txt = T.ToLowerString();
                            string wrd = ToLowerString();
                            ComportementMotSuivant cms = ComportementMotSuivant.undef;

                            int startNextWord = Last + 1;
                            // cherchons le début du prochain mot (ou la ponctuation ou la fin de ligne...)
                            while (startNextWord < txt.Length 
                                && (txt[startNextWord] == ' ' 
                                || txt[startNextWord] == '\t'
                                || txt[startNextWord] == ',' // la virgule n'empêche pas l'influence du mot suivant.
                                || txt[startNextWord] == '!' // ça pourrait dépendre des situations...
                                || txt[startNextWord] == '"'
                                || txt[startNextWord] == '«'
                                || txt[startNextWord] == '»'
                                || txt[startNextWord] == '“'
                                || txt[startNextWord] == '”'
                                || txt[startNextWord] == '‘'
                                || txt[startNextWord] == '’'
                                || txt[startNextWord] == ':' // ça pourrait dépendre des situations...
                                ))
                            {
                                startNextWord++;
                            }

                            // cherchons la fin du mot suivant
                            int endNextWord = startNextWord;
                            while (endNextWord < txt.Length 
                                && (EstConsonne(txt[endNextWord]) || EstVoyelle(txt[endNextWord])))
                            {
                                endNextWord++;
                            }
                            // startNextWord est l'index du début du mot suivant. S'il y a des 
                            // lettres, endNextWord est celui de la lettre qui suit le mot.
                            // S'il n'y a pas de lettres, endNextWord == startNextWord
                            string nextWord = null;
                            if (endNextWord > startNextWord)
                            {
                                nextWord = txt.Substring(startNextWord, endNextWord - startNextWord);
                                logger.ConditionalTrace("nextWord: {0}", nextWord);
                            }

                            if (startNextWord < txt.Length)
                            {
                                // il peut y avaoir un mot suivant.
                                if (Disjonction(nextWord))
                                {
                                    cms = ComportementMotSuivant.consonne;
                                }
                                else if (Liaison(nextWord))
                                {
                                    cms = ComportementMotSuivant.voyelle;
                                }
                                else if (txt[startNextWord] == 'y')
                                {
                                    // Le cas normal est que le y se comporte comme une consonne
                                    // et le e-caduc forme une syllabe). Les exceptions sont 
                                    // interceptées par "Liaison"
                                    cms = ComportementMotSuivant.consonne;
                                }
                                else if (TextEl.EstVoyelle(txt[startNextWord])) 
                                {
                                    cms = ComportementMotSuivant.voyelle;
                                }
                                else if (txt[startNextWord] == 'h')
                                {
                                    // Le 'h' mérite un dictionnaire à lui tout seul
                                    if (HAspire(nextWord))
                                    {
                                        cms = ComportementMotSuivant.consonne;
                                    }
                                    else
                                    {
                                        // h muet
                                        cms = ComportementMotSuivant.voyelle;
                                    }
                                }
                                else if (TextEl.EstConsonne(txt[startNextWord])) 
                                {
                                    cms = ComportementMotSuivant.consonne;
                                }
                                else
                                {
                                    // Il ne s'agit pas d'un lettre. Donc soit de la ponctuation,
                                    // une fin de ligne ou autre chose... On traite ce cas comme
                                    // une fin de vers.
                                    cms = ComportementMotSuivant.fin;
                                }
                            }
                            else
                            {
                                // C'est la fin du texte.
                                cms = ComportementMotSuivant.fin;
                            }
                            logger.ConditionalTrace("cms: {0}", cms.ToString());
                            switch (cms)
                            {
                                case ComportementMotSuivant.consonne:
                                    // la syllabe est prononcée, on la laisse.
                                    break;
                                case ComportementMotSuivant.voyelle:
                                    if (wrd[wrd.Length - 1] == 's' || wrd[wrd.Length - 1] == 't')
                                    {
                                        // il y a une liaison, la syllabe se prononce.
                                        // L'existence d'un eliaison est probablement plus compliquée
                                        // à identifier (il y certainement une foule d'exceptions)
                                        // :-) Commençons quand même comme ça...
                                    }
                                    else
                                    {
                                        // la syllabe ne se prononce pas.
                                        syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                        syls.RemoveAt(syls.Count - 1);
                                    }
                                    break;
                                case ComportementMotSuivant.fin:
                                    // la syllabe ne se prononce pas.
                                    syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                    syls.RemoveAt(syls.Count - 1);
                                    break;
                                default:
                                    logger.Error("ComportementMotSuivant {0} non traité", cms);
                                    break;
                            }
                            logger.ConditionalTrace("Etape 7A {0} --> {1}, {2}",
                                GetWord(), Syllabes(), GetPhonSyllabes());
                        }
                    }
                } // if (syls.Count > 1)
            } // if (syls == null)

            logger.ConditionalTrace("Résultat {0} --> {1}, {2}",
                GetWord(), Syllabes(), GetPhonSyllabes());
        }

        /// <summary>
        /// Colorizes the syllabes in syls, according to <c>conf</c>. 
        /// </summary>
        /// <param name="conf">The <c>Config</c> to apply. Cannot be null.</param>
        public void ColorizeSyls(Config conf)
        {
            logger.ConditionalDebug("ColorizeSyls");
            if (syls != null)
            {
                // Mettre les syllabes en couleur
                foreach (SylInW s in syls)
                    s.PutColor(conf);

                // s'il le faut, marquer par-dessus les phonemes muets.
                if (conf.sylConf.marquerMuettes)
                    foreach (PhonInW piw in phons)
                        if (piw.EstMuet())
                            piw.PutColor(conf, PhonConfType.muettes); 
            }
            else
            {
                logger.Error("ColorizeSyls called when syls is null.");
            }
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

        /// <summary>
        /// Retourne le mot avec des tirets '-' entre les syllabes
        /// </summary>
        /// <returns>Le <c>string</c> contenant le mot découpé en syllabes.</returns>
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

        /// <summary>
        /// Donne le nombre de syllabes dans le mot.
        /// </summary>
        /// <returns>Nombre de syllabes.</returns>
        public int GetNbreSyllabes()
        {
            return syls.Count;
        }


        /// <summary>
        /// Retourne pour chaque syllabe, le phonème rattaché. Ils sont séparés par des tirets.
        /// </summary>
        /// <returns>Les phonèmes pour chaque syllabe.</returns>
        private string GetPhonSyllabes()
        {
            StringBuilder sb = new StringBuilder(GetWord().Length + 4);
            for (int i = 0; i < syls.Count; i++)
            {
                sb.Append(syls[i].P.ToString());
                if (i < syls.Count - 1)
                    sb.Append("-");
            }
            return sb.ToString();
        }

        /// <summary>
        /// For test purpose, clear all phons
        /// </summary>
        public void ClearPhons() => phons.Clear();

        /// <summary>
        /// Vérifie si une syllabe et son successeur (voir http://www.academie-francaise.fr/la-successeur)
        /// doivent être fusionnées.
        /// <remarks>
        /// Marie-Pierre parle de "mixer" et documente les cas suivants:
        /// <para>
        /// mixer les doubles phonèmes de consonnes qui incluent [l] et [r] ; ex. : bl, tr, cr, chr, pl
        /// </para>
        /// <para>
        /// mixer les doubles phonèmes [y] et [i], [u] et [i,e_tilda,o_tilda]
        /// </para>
        /// <para>
        /// accrocher les lettres muettes aux lettres qui précèdent
        /// </para>
        /// <para>
        /// j'ai rajouté des cas comme le ¨[u] suivi de [a] dans certaines situations...
        /// </para>
        /// <para>
        /// D'une manière générale, on peut dire que
        /// la langue n'aime pas tellement le voyelles qui se suivent. On appelle
        /// ça l'hiatus. Pour l'éviter, deux voyelles qui se suivent sont parfois
        /// prononcées ensemble. La diérèse en poésie consiste justement à quand
        /// même prononcer ces deux sons.
        /// On essaye ici de traîter les cas évidents. Il existe certainement plein
        /// d'exceptions...
        /// </para>
        /// </remarks>
        /// <remarks>
        /// Ces tests sont regroupés dans une méthode séparée pour faciliter la lecture du code.
        /// </remarks>
        /// </summary>
        /// <param name="syl">La syllabe.</param>
        /// <param name="succ">Son successeur dans le mot.</param>
        /// <param name="forceDierese">indique si la diérèse doit être forcée. Dans ce cas, la
        /// méthode retourne false si on avait pu fusionner deux voyelles.</param>
        /// <returns></returns>
        private bool FusionnerSyllabes(SylInW syl, SylInW succ, bool forceDierese = false)
        {
            bool dieresePoss = false;
            bool toReturn = false;
            if (syl.EstBkptgdfv() && (succ.P == Phonemes.l || succ.P == Phonemes.R))  // [bkptgdfv][lR]
                toReturn = true;
            else if (syl.P == Phonemes.y && succ.P == Phonemes.i)  // ui
                toReturn = true;
            else if (syl.P == Phonemes.u
                         &&
                         (succ.P == Phonemes.i
                         || succ.P == Phonemes.e_tilda
                         || succ.P == Phonemes.o_tilda)
                         &&
                         !forceDierese) // u(i|e_tilda|o_tilda)
            {
                toReturn = true;
                dieresePoss = true;
            }
            else if (syl.P == Phonemes.u && succ.P == Phonemes.a && !forceDierese)
            {
                if (succ.Last < this.Last)
                {
                    // Pas la fin du mot. Quelle est la lettre suivante?
                    char nextC = succ.T.ToLowerString()[succ.Last + 1];
                    switch (nextC)
                    {
                        case 'c':
                        case 'd':
                        case 'n':
                        case 'p':
                        case 'q':
                        case 't':
                            toReturn = true;
                            dieresePoss = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
                toReturn = succ.EstMuet();
                return toReturn;
        }

        // ****************************************************************************************
        // ***                           Liaisons et disjonctions                               ***
        // ****************************************************************************************

        // Les mots avec trait d'union ont été enlevés de la liste car on considère que le proogramme 
        // copue les mots au trait d'union... En tenir compte si cela devait changer...
        // A prt ça, on a un problème avec 'huis-clos' qui doit dans la liste mais pas 'huis':
        // le huis-clos mais l'huis...

        static string[] hAspire =
        {
            "hâblerie", "hâbleur", "hableuse", "hachage", "hache", "haché", "hachée",
            "hachement", "hacher", "hachette", "hacheur",
            "hachis", "hachisch", "hachoir", "hachure", "hachurer", "hackle", "hadal",
            "hadale", "hadaux", "haddock", "haflinger", "hafnium", "hagard", "haggis", "haie",
            "haillon", "haillonneux", "haillonneuse", "haine", "haineusement", "haineux", "haineuse",
            "haïr", "haire", "haïssable", "haïtien", "haïtienne", "halage", "hâlage", "halal", 
            "halbi", "halbran", "halbrené", "halbrenée", "halde", "hâle", "hâlé", 
            "hâlée", "hale", "halefis", "haler", "hâler", "haletant", "halètement", "haleter", 
            "haleur", "haleuse", "half", "hall", "halle", "hallebarde", "halo", "hallux", "halte", 
            "hamac", "hamada", "hamburger", "hameau", "hamman", "hampe", "hamster", "hanche", 
            "hanchement", "hancher", "hand", "handballeur", "handballeuse", "handicap", "handicapant",
            "handicapante", "handicapé", "handicapée", "handicaper", "handicapeur", "handisport", 
            "hangar", "hanneton", "hanon", "hanse", "hanter", "hantise", "happement", "happening", 
            "happer", "happy", "haque", "haquenée", "haquet", "hara", "harangue", "haranguer", 
            "harangueur", "harangueuse", "haras", "harassant", "harassante", "harassé", "harassée",
            "harassement", "harasser", "harcelant", "harcelante", "harcèlement", "harceler",
            "hard", "harde", "hardé", "harder", "hardes", "hardi", "hardie",
            "hardiesse", "hardiment", "hardware", "harem", "hareng", "harengade",
            "harengaison", "harengère", "harenguet", "harengueux", "harenguier", "harenguière",
            "haret", "harfang", "hargne", "hargneusement", "hargneux", "hargneuse", "haricot",
            "haridelle", "harissa", "harka", "harki", "harnachement", "harnacher", "harnais",
            "harnat", "harnois", "haro", "harouelle", "harpail", "harpaye", "harpe", "harpette",
            "harpie", "harpiste", "harpocéras", "harpodon", "harpoise", "harpon", "harponnage",
            "harponnement", "harponner", "harponneur", "harpye", "harrier", "hart", "hasard",
            "hasarder", "hasardeusement", "hasardeux", "hasardeuse", "has", "hasch", "haschisch",
            "hase", "hâte", "hâter", "hatha", "hâtier", "hâtif", "hâtive", "hattéria", "hauban",
            "haubanage", "haubaner", "haubert", "hausse", "haussement", "hausser", "haussier",
            "haussière", "haut", "haute", "hautain", "hautaine", "hautbois", "hautboïste",
            "hautement", "hauteur", "hautin", "hauturier", "hauturière", "havage", "havane", "hâve",
            "havée", "haveneau", "havenet", "haver", "haveur", "haveuse", "havre", "havresac",
            "havrit", "haylage", "hayon", "hé", "heat", "heaume", "hein", "héler", "hem", "henné",
            "hennin", "hennir", "hennissant", "hennissante", "hennissement", "hennuyer", "hep", 
            "héraut", "hère", "hérissé", "hérissée", "hérissement", "hérisser", "hérisson",
            "hérissonne", "herniaire", "hernie", "hernié", "herniée", "hernieux", "hernieuse",
            "héron", "héronneau", "héronnier", "héronnière", "héros", "hersage",
            "herse", "herser", "hersillon", "hêtraie", "hêtre", "heu", "heurt", "heurter", "heurtoir",
            "hibou", "hic", "hickory", "hideur", "hideusement", "hideux", "hideuse", "hiement",
            "hiérarchie", "hiérarchique", "hiérarchiquement", "hiérarchisation", "hiérarchiser",
            "hiérarque", "hi", "high", "hilaire", "hile", "hindi", "hip", "hippie", "hippy", "hidjab",
            "hissage", "hit", "ho", "hobby", "hobbyste", "hobereau", "hochement", "hochepot", 
            "hochequeue", "hocher", "hochet", "hockey", "hockeyeur", "hockeyeuse", "holding",
            "holà", "hold", "hollandite", "hollywoodien", "hollywoodienne", "homard", 
            "homarderie", "homardier", "home", "hongre", "hongrer", "hongreur", "hongreuse", 
            "hongroierie", "hongroyage", "hongroyer", "hongroyeur", "honning", "honnir", "honoris",
            "honte", "honteusement", "honteux", "honteuse", "hooligan", "hooliganisme", "hop",
            "hoquet", "hoqueter", "hoqueton", "horde", "horion", "hormis", "hornblende", "hors",
            "horsain", "horsin", "horse", "horst", "hot", "hotdog", "hotinus", "hotte", "hottée",
            "hotu", "hou", "houache", "houaiche", "houage", "houblon", "houe", "houer", "houille",
            "houiller", "houillère", "houle", "houlette", "houleux", "houleuse",
            "houligan", "houliganisme", "houlque", "houp", "houppe",
            "houppelande", "houppette", "houppier", "hourd", "hourdage", "hourder", "hourdir",
            "hourdis", "houri", "hourque", "hourra", "hourri", "hourrite", "hourvari", "housche",
            "houseau", "house", "houspiller", "houssage", "housse", "housser", "housset",
            "houssière", "houst", "houx", "hovéa", "hoyau", "hoyé", "hoyée", "huard", "huart",
            "hublot", "huche", "hucher", "huchier", "hue", "huée", "huer", "huerta", "huguenot", 
            "huipil", "huir", "huis-clos", "huit", "huitain", "huitaine", "huitante", "huitième", 
            "huitièmement", "hulotte", "hululation", "hululement", "hululer", "hum", "humantin", 
            "humer", "hune", "hunier", "hunter", "huppe", "huppé", "huppée", "huque", "hurdler", 
            "hure", "hurlant", "hurlement", "hurler", "hurleur", "hurleuse", "huron", "huronne", 
            "hurrah", "hurricane", "husky", "hussard", "hussarde", "hutinet", "hutte", "hutteau",
            "hyène"
        };

        private static StringDictionary hAspire_hashed = null;

        /// <summary>
        /// Analyse si le mot comence par un h aspiré.
        /// </summary>
        /// <param name="wrd">Le mot à analyser en minuscules. Peut-être <c>null</c>.</param>
        /// <returns>Le mot commence par un h aspiré</returns>
        private bool HAspire(string wrd)
        {
            bool toReturn = false;
            if (!String.IsNullOrEmpty(wrd))
            {
                if (hAspire_hashed == null)
                {
                    hAspire_hashed = new StringDictionary();
                    for (int i = 0; i < hAspire.Length; i++)
                        hAspire_hashed.Add(hAspire[i], null);
                }
                toReturn = hAspire_hashed.ContainsKey(wrd);
                if (!toReturn && wrd[wrd.Length - 1] == 's')
                {
                    string sing = wrd.Substring(0, wrd.Length - 1);
                    toReturn = hAspire_hashed.ContainsKey(sing);
                }
            }
            return toReturn;
        }

        private static string[] disjonctions =
        {
            "onze", "oui", "uhlan", "ululement", "iodler", "jodler", "ionesco", "ouagadougou",
            "jahvé"
             //, "un" on dit le un quand il s'agit du chiffre, mais l'un et l'autre... Impossible
             // de distinguer sans connaître le sens. Donc on choisit l'article et le pronom qui 
             // sont quand même plus fréquents et qui se comportent comme une voyelle.
        };

        private static StringDictionary disjonctions_hashed = null;

        /// <summary>
        /// Analyse si le mot est une exception et se comporte comme s'il commençait pas une
        /// consonne en ce qui concerne les liaisons et le pronnonciation des syllabes
        /// qui précèdent. Typiquement on aura un 'le' ou un 'la' devant.
        /// </summary>
        /// <param name="wrd">Le mot à analyser en minuscules. Peut-être <c>null</c>.</param>
        /// <returns>Le mot fait partie de la liste des exceptions (sans les h aspirés).</returns>
        private bool Disjonction(string wrd)
        {
            bool toReturn = false;
            if (!String.IsNullOrEmpty(wrd))
            {
                if (disjonctions_hashed == null)
                {
                    disjonctions_hashed = new StringDictionary();
                    for (int i = 0; i < disjonctions.Length; i++)
                        disjonctions_hashed.Add(disjonctions[i], null);
                }
                toReturn = disjonctions_hashed.ContainsKey(wrd);
                if (!toReturn && wrd[wrd.Length - 1] == 's')
                {
                    string sing = wrd.Substring(0, wrd.Length - 1);
                    toReturn = disjonctions_hashed.ContainsKey(sing);
                }
            }
            return toReturn;
        }

        static string[] liaisons =
        {
            "yeux", "yeuse", "ypérite", "yèble", "york", "yourcenar"
        };

        private static StringDictionary liaisons_hashed = null;

        /// <summary>
        /// Analyse si le mot est une exception et se comporte comme s'il commençait pas une
        /// voyelle en ce qui concerne les liaisons et le pronnonciation des syllabes
        /// qui précèdent. Typiquement on aura un "l'" devant.
        /// </summary>
        /// <param name="wrd">Le mot à analyser en minuscules. Peut-être <c>null</c>.</param>
        /// <returns>Le mot fait partie de la liste des exceptions.</returns>
        private bool Liaison(string wrd)
        {
            bool toReturn = false;
            if (!String.IsNullOrEmpty(wrd))
            {
                if (liaisons_hashed == null)
                {
                    liaisons_hashed = new StringDictionary();
                    for (int i = 0; i < liaisons.Length; i++)
                        liaisons_hashed.Add(liaisons[i], null);
                }
                toReturn = liaisons_hashed.ContainsKey(wrd);
                if (!toReturn && wrd[wrd.Length - 1] == 's')
                {
                    string sing = wrd.Substring(0, wrd.Length - 1);
                    toReturn = liaisons_hashed.ContainsKey(sing);
                }
            }
            return toReturn;
        }


    }
}
