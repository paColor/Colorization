﻿/********************************************************************************
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
using System.Diagnostics;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ColorLib
{
    /// <summary>
    /// <para>
    /// <c>CharFormatting</c> contient un certain nombre de méthodes qui permettent de savoir comment se comporter avec
    /// une valeur <c>false</c> pour les différents membres de type <c>bool</c>. Faut-il ne rien faire ou forcer
    /// la caractéristique "gras" à "non gras" par exemple? 
    /// </para>
    /// <para>
    /// Une instance de <c>CFForceBlack</c> se comporte comme un <c>CharFormatting</c> mais retourne toutjours <c>true</c>
    /// pour les méthodes évoquées ci-dessus. Cela permet par exemple de créer un <c>CharFormatting</c> qui forcera
    /// la mise en noir et l'élimination des caractéristique <c>bold</c>, <c>italic</c>, <c>underline</c> et autres.
    /// </para>
    /// </summary>
    public class CFForceBlack : CharFormatting
    {
        public override bool ForceNonBold(Config conf) => true;
        public override bool ForceNonItalic(Config conf) => true;
        public override bool ForceNonUnderline(Config conf) => true;
        public override bool ForceBlackColor(Config conf) => true;
        public override bool ForceHilightClear(Config conf) => true;
        public override bool ForceNonCaps(Config conf) => true;
        public override bool ForceNonContour(Config conf) => true;
        public override bool ForceNonSerif(Config conf) => true;
    }


    /// <summary>
    /// <para>
    /// The class TheText represents the text that is analysed and whose formatting is adapted by the
    /// analysis algorithm.
    /// </para>
    /// <para>
    /// The idea is that the inheriting class is created with a <c>string</c>. Once this is done, 
    /// different methods make it possible to apply some kind of formatting to the text. This formatting is applied
    /// to the inheriting class by calling the method <c>SetChars</c> see <see cref="SetChars(FormattedTextEl, Config)"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para><c>Init</c> must be called before any usage of the class. This is especially important for testing,
    /// where this could easily be forgotten. </para>
    /// <para>
    /// The class is not supposed to be intiantiated directly. However, for test reasons, we do not declare it as abstract.
    /// Some members and methods of this class are public to simplify testing, although from a logical point of view, this
    /// would not be necessary.
    /// </para>
    /// <para>
    /// The class is constructed in such a way that several formatting operations can be applied to it sequentially. It 
    /// optimizes the computation, so that lengthy operations are only conducted once, even if two commands are excuted.
    /// Obviously this possibility will not be used often, but there are some cases where it can be useful.
    /// </para>
    /// </remarks>
    public class TheText
    {
        /// <summary>
        /// Returns a list of zero based text positions.
        /// </summary>
        /// <returns>List of zero based text positions. Empty if the text is an empty string.</returns>
        protected delegate List<int> GetTextPos();

        // ****************************************************************************************
        // *                                    class DuoCache                                    *
        // ****************************************************************************************

        /// <summary>
        /// <para>
        /// Stores computed lists of <see cref="Word"/>(s) and <see cref="PhonWord"/>(s) for the "Duo"
        /// command as well as the parameters that were used to compute them.
        /// </para>
        /// <para>
        /// This makes it possible to to recompute things only when it is not necessary.
        /// </para>
        /// </summary>
        private class DuoCache
        {
            private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            private List<Word> cachedWordList1;
            private List<Word> cachedWordList2;
            private ConcurrentBag<PhonWord> cachedPWL1;
            private ConcurrentBag<PhonWord> cachedPWL2;

            // Si une de ces données change, le cache est invalidé.
            // Les deux champs suivants ont une influence sur la façon de distribuer les mots
            // dans les deux listes. 
            private DuoConfig.Alternance alt;
            private int nbreAlternance;

            // Les paramètres suivants ont une influence sur le calcul des PhonWord(s). J'ai 
            // hésité à en tenir compte ici, car je n'aime pas l'idée d'expliciter cette
            // connaissance ici. ça nuit à l'indépendance des classes et donc à la robustesse
            // du programme! D'un autre côté, c'est le prix à payer pour l'utilisation d'un 
            // cache... Le risque est que le cache ne soit pas invalidé quand il le devrait.
            // :-( Saleté de caches :-(
            // Attention, les PhonWord(s) contiennent également les syllabes...
            private ColConfWin.IllRule illRule1Muettes;
            private ColConfWin.IllRule illRule2Muettes;
            private ColConfWin.IllRule illRule1Phon;
            private ColConfWin.IllRule illRule2Phon;
            bool doubleConsStd1;
            bool doubleConsStd2;
            bool modeEcrit1;
            bool modeEcrit2;



            public DuoCache()
            {
                logger.ConditionalDebug("DuoCache");
                cachedWordList1 = null;
                cachedWordList2 = null;
                cachedPWL1 = null;
                cachedPWL2 = null;

                // Il n'est pas nécessaire d'initialiser les champs de gestion de l'actualité
                // du cache. Pour avoir un état clair, initialisons quand mêm ceux où on peut 
                // marquer clairement qu'il s'agit d'un état indéfini.
                alt = DuoConfig.Alternance.undefined;
                nbreAlternance = 999;
                illRule1Muettes = ColConfWin.IllRule.undefined;
                illRule2Muettes = ColConfWin.IllRule.undefined;
                illRule1Phon = ColConfWin.IllRule.undefined;
                illRule2Phon = ColConfWin.IllRule.undefined;
            }

            /// <summary>
            /// Returns the two list of <see cref="Word"/>(s) that must be handled by the "Duo" command
            /// according to <paramref name="dConf"/>.
            /// </summary>
            /// <param name="wL">The complete list of <see cref="Word"/>(s), before it is split.</param>
            /// <param name="dConf">The <see cref="Config"/> to be used for splitting <paramref name="wL"/>.</param>
            /// <param name="getEolPos">The method to call in order to get the indexes for the last characters 
            /// of each line.</param>
            /// <param name="wL1">Out: listo of <see cref="Word"/>(s) nr 1</param>
            /// <param name="wL2">Out: listo of <see cref="Word"/>(s) nr 2</param>
            public void GetWordLists(List<Word> wL, DuoConfig dConf, GetTextPos getEolPos, 
                out List<Word> wL1, out List<Word> wL2)
            {
                // We measured 9 ms duration for a very long text (a fat book). So this is not the place to
                // put more sophisticated parallelism.
                logger.ConditionalDebug("GetWordLists");
                CheckCacheValidity(dConf);
                if (cachedWordList1 == null)
                {
                    cachedWordList1 = new List<Word>((wL.Count / 2) + 1);
                    cachedWordList2 = new List<Word>((wL.Count / 2) + 1);
                    switch (dConf.alternance)
                    {
                        case DuoConfig.Alternance.mots:
                            for (int i = 0; i < wL.Count; i++)
                            {
                                if ((i / dConf.nbreAlt) % 2 == 1)
                                {
                                    // odd
                                    cachedWordList2.Add(wL[i]);
                                }
                                else
                                {
                                    // even
                                    cachedWordList1.Add(wL[i]);
                                }
                            }
                            break;

                        case DuoConfig.Alternance.lignes:
                            List<int> eolPos = getEolPos();
                            int lineIndex = 0;
                            int wordIndex = 0;
                            while (lineIndex < eolPos.Count)
                            {
                                while ((wordIndex < wL.Count) && (wL[wordIndex].Last <= eolPos[lineIndex]))
                                {
                                    if ((lineIndex / dConf.nbreAlt) % 2 == 1)
                                    {
                                        // odd
                                        cachedWordList2.Add(wL[wordIndex]);
                                    }
                                    else
                                    {
                                        // even
                                        cachedWordList1.Add(wL[wordIndex]);
                                    }
                                    wordIndex++;
                                }
                                lineIndex++;
                            }
                            break;

                        default:
                            logger.Error("Type d'alternance non traité par la commande \'Duo\'. alternance: \'{0}\'",
                                alt);
                            throw new ArgumentException(String.Format(BaseConfig.cultF,
                                "Type d'alternance non traité par la commande \'Duo\'." +
                                "alternance: \'{0}\'", alt));
                            break;
                    }
                }
                wL1 = cachedWordList1;
                wL2 = cachedWordList2;
            }

            /// <summary>
            /// Returns the two lists of <see cref="PhonWord"/>(s) that correspond to the passed List of
            /// <c>Word</c> and the passed <see cref="DuoConfig"/>.
            /// </summary>
            /// <param name="wL">List of <see cref="Word"/>(s) to split between the two lists of 
            /// <see cref="PhonWord"/>(s).</param>
            /// <param name="dConf">The <see cref="DuoConfig"/> to apply.</param>
            /// <param name="getEolPos">The method giving the positions of the last characters on each line.</param>
            /// <param name="pwL1">Out: the first list of <see cref="PhonWord"/></param>
            /// <param name="pwL2">Out: the second list of <see cref="PhonWord"/></param>
            public void GetPhonWordLists(List<Word> wL, DuoConfig dConf, GetTextPos getEolPos,
                out ConcurrentBag<PhonWord> pwL1, out ConcurrentBag<PhonWord> pwL2)
            {
                logger.ConditionalDebug("GetPhonWordLists");
                CheckCacheValidity(dConf);
                if (cachedPWL1 == null)
                {
                    List<Word> wL1;
                    List<Word> wL2;
                    GetWordLists(wL, dConf, getEolPos, out wL1, out wL2);

                    // Parallel way
                    ComputePhonWords cpw = ComputePW;
                    IAsyncResult asr = cpw.BeginInvoke(wL1, dConf.subConfig1, ref cachedPWL1, null, null);
                    ComputePW(wL2, dConf.subConfig2, ref cachedPWL2);
                    cpw.EndInvoke(ref cachedPWL1, asr);

                    // Sequential way
                    //cachedPWL1 = GetPhonWords(wL1, dConf.subConfig1);
                    //cachedPWL2 = GetPhonWords(wL2, dConf.subConfig2);
                }
                pwL1 = cachedPWL1;
                pwL2 = cachedPWL2;
            }

            delegate void ComputePhonWords(List<Word> wL, Config subConf, ref ConcurrentBag<PhonWord> pwL);

            private void ComputePW(List<Word> wL, Config subConf, ref ConcurrentBag<PhonWord> pwL)
            {
                pwL = GetPhonWords(wL, subConf);
            }

            private void CheckCacheValidity(DuoConfig dConf)
            {
                logger.ConditionalDebug("CheckCacheValidity");
                if ((alt != dConf.alternance)
                    || (nbreAlternance != dConf.nbreAlt)
                    || (doubleConsStd1 != dConf.subConfig1.sylConf.DoubleConsStd)
                    || (doubleConsStd2 != dConf.subConfig2.sylConf.DoubleConsStd)
                    || (modeEcrit1 != dConf.subConfig1.sylConf.ModeEcrit)
                    || (modeEcrit2 != dConf.subConfig2.sylConf.ModeEcrit)
                    || (illRule1Muettes != dConf.subConfig1.colors[PhonConfType.muettes].IllRuleToUse)
                    || (illRule2Muettes != dConf.subConfig2.colors[PhonConfType.muettes].IllRuleToUse) 
                    || (illRule1Phon != dConf.subConfig1.colors[PhonConfType.phonemes].IllRuleToUse)
                    || (illRule2Phon != dConf.subConfig2.colors[PhonConfType.phonemes].IllRuleToUse)
                   )
                {
                    logger.ConditionalTrace("Invalidate cache");
                    cachedWordList1 = null;
                    cachedWordList2 = null;
                    cachedPWL1 = null;
                    cachedPWL2 = null;
                    alt = dConf.alternance;
                    nbreAlternance = dConf.nbreAlt;
                    doubleConsStd1 = dConf.subConfig1.sylConf.DoubleConsStd;
                    doubleConsStd2 = dConf.subConfig2.sylConf.DoubleConsStd;
                    modeEcrit1 = dConf.subConfig1.sylConf.ModeEcrit;
                    modeEcrit2 = dConf.subConfig2.sylConf.ModeEcrit;
                    illRule1Muettes = dConf.subConfig1.colors[PhonConfType.muettes].IllRuleToUse;
                    illRule2Muettes = dConf.subConfig2.colors[PhonConfType.muettes].IllRuleToUse;
                    illRule1Phon = dConf.subConfig1.colors[PhonConfType.phonemes].IllRuleToUse;
                    illRule2Phon = dConf.subConfig2.colors[PhonConfType.phonemes].IllRuleToUse;
                }
            }
        }


        // ****************************************************************************************
        // *                                  class FormatsMgmt                                   *
        // ****************************************************************************************

        /// <summary>
        /// Gestion de la liste des <see cref="FormattedTextEl"/> qui sont découverts au cous de 
        /// l'exécution des fonctions et qui sont ajoutés à la liste.
        /// </summary>
        private class FormatsMgmt
        {
            int sLength;
            public List<FormattedTextEl> formats;

            // Deprecated
            // public Dictionary<CharFormatting, List<FormattedTextEl>> formatsPerCF { get; private set; }

            /// <summary>
            /// Crée un manager de formats
            /// </summary>
            /// <param name="sLength">Longueur du texte dont il faut s'occuper des formats.</param>
            public FormatsMgmt(int inSLength)
            {
                logger.ConditionalDebug("FormatsMgmt sLength: {0}", inSLength);
                sLength = inSLength;
                formats = new List<FormattedTextEl>((inSLength / 3) * 2);

                // formatsPerCF = new Dictionary<CharFormatting, List<FormattedTextEl>>(16); 
                // 16 au pif. Il semble peu fréquent d'avoir plus de 16 formatages différents 
                // dans un texte. Il y en a 13 dans la config CERAS.
            }

            /// <summary>
            /// Réinitialise la gestion des formats. Efface tout ce qui pourrait être présent.
            /// </summary>
            public void ClearFormats()
            {
                logger.ConditionalDebug("ClearFormats");
                formats.Clear();
                // formatsPerCF.Clear();
            }

            public void Add(FormattedTextEl fte)
            {
                formats.Add(fte);
                //List<FormattedTextEl> ftes;
                //if (!formatsPerCF.TryGetValue(fte.cf, out ftes))
                //{
                //    ftes = new List<FormattedTextEl>(sLength/10);
                //    formatsPerCF.Add(fte.cf, ftes);
                //}
                //ftes.Add(fte);
            }
        }

        // ****************************************************************************************
        // *                               private static members                                 *
        // ****************************************************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ****************************************************************************************
        // *                               public static methods                                 *
        // ****************************************************************************************

        /// <summary>
        /// Initializes the static elements of the whole <c>ColorLib</c> library. Must be called before any
        /// usage of the library.
        /// </summary>
        public static void Init()
        {
            logger.ConditionalDebug("Init");
            AutomAutomat.InitAutomat();
            PhonInW.Init();
            SylInW.Init();
            Config.Init();
        }

        // ****************************************************************************************
        // *                                    private members                                   *
        // ****************************************************************************************

        /// <summary>
        /// The text that was entered when the object was constructed. Cannot be null.
        /// </summary>
        private string S;
        private string smallCapsS;
        private List<Word> words; // List of the words in TheText (computed when needed)
        private ConcurrentBag<PhonWord> phonWords; // The corresponding PhonWords (computed when needed).
        private DuoCache dc;
        private FormatsMgmt formatsMgmt;

        // ****************************************************************************************
        // *                                     public methods                                   *
        // ****************************************************************************************

        /// <summary>
        /// Creates a <c>TheText</c> with the passed <c>string</c> and the passed <c>Config</c>. The <c>Config</c>
        /// will be used when applying formattings. 
        /// </summary>
        /// <param name="txt">The text that will be worked on. Cannot be null.</param>
        /// <param name="inConf">The <c>Config</c> that will be used when applying formatsMgmt to the text.</param>
        public TheText(string txt)
        {
            logger.ConditionalDebug(BaseConfig.cultF, "TheText");
            logger.ConditionalTrace(BaseConfig.cultF, "TheText Constructor, txt: \'{0}\'.", txt);
            Debug.Assert(txt != null);
            this.S = txt;
            formatsMgmt = new FormatsMgmt(S.Length);
            phonWords = null;
            words = null;
            dc = null;
            smallCapsS = null;
        }

        /// <summary>
        /// Returns the text as string.
        /// </summary>
        /// <returns>The text.</returns>
        public override string ToString()
        {
            return S;
        }

        /// <summary>
        /// Retrurns the list of <c>Words</c> contained in the text.
        /// </summary>
        /// <remarks>Is not needed by a normal consumer of the class.</remarks>
        /// <returns>List of <c>Words</c> contained in the text</returns>
        private List<Word> GetWords()
            // public for test reasons
        {
            logger.ConditionalDebug("GetWords");
            if (words == null)
            {
                words = new List<Word>(S.Length / 5); // longueur moyenne d'un mot avec l'espace : 5 charactères...
                Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
                MatchCollection matches = rx.Matches(S);
                foreach (Match match in matches)
                {
                    int beg = match.Index;
                    int end = beg + match.Length - 1;

                    // Apostrophes: On considère comme apostrophes, les caractères ' ou ’ placé après une ou deux lettres.
                    // Cela couvre les formes élidées de le, que, ce, te... Y en  a-t-il d'autres?
                    // Il peut y avoir confusion avec le guillemet simple. Tant pis!
                    // Le mot est allongé pour contenir l'apostrophe comme dernière lettre.
                    if ((match.Length <= 2) && (end + 1 < S.Length) && ((S[end + 1] == '\'') || (S[end + 1] == '’')))
                    {
                        end++;
                    }

                    // Pour le traitement des syllabes, il peut y avoir un sens à fusionner le mot avec apostrophe
                    // avec son successeur. ("l'" ne forme pas vraiment une syllabe indépendante...). Si on décide 
                    // de le faire, ça pourrait assez facilement être ici, avant la création du nouveau mot pour
                    // le successeur.

                    Word w = new Word(this, beg, end);
                    words.Add(w);
                }
            }
            return words;
        }

        /// <summary>
        /// Returns the list of <c>PhonWords</c> contained in the text.
        /// </summary>
        /// <remarks>Is not needed by a normal consumer of the class and should only be used for testing.</remarks>
        /// <param name="conf">The <see cref="Config"/> to use for the detection of the "phonèmes".</param>
        /// <returns>List of <c>PhonWords</c> contained in the text.</returns>
        public List<PhonWord> GetPhonWordList(Config conf)
            // public for test reasons
        {
            logger.ConditionalDebug("GetPhonWordList");
            ConcurrentBag<PhonWord> pws = GetPhonWords(conf);
            List<PhonWord> toReturn = new List<PhonWord>(pws);
            toReturn.Sort(TextEl.CompareTextElByPosition);
            return toReturn;
        }

        /// <summary>
        /// Applies the formattings defined in the <c>ColConfWin</c> identified by <paramref name="conf"/> and 
        /// <paramref name="pct"/> to the 
        /// "phonèmes" in the text. I.e. fills <c>Formats</c> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="pct">Identifies the <c>ColConfWin</c> (see <see cref="ColorLib.ColConfWin"/>) that msut
        /// be used when coloring the "phonèmes".</param>
        /// <param name="conf">The <c>Config</c> to use for the colorization.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void ColorizePhons(Config conf, PhonConfType pct, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("ColorizePhons");
            if (conf != null)
            { 
                formatsMgmt.ClearFormats();
                ConcurrentBag<PhonWord> pws = GetPhonWords(conf);
                FormatPhons(pws, conf, pct);
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser les phonèmes sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser les phonèmes sans une configuration valable.");
            }
        }

        /// <summary>
        /// Colors the letters in the text, according to the <see cref="PBDQConfig"/> <c>conf</c>,
        ///i.e. fills <see cref="formatsMgmt"/>and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> that must be used for marking the letters.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkLetters(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkLetters");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                FormatLetters(0, S.Length - 1, conf);
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser les lettres sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser les lettres sans une configuration valable.");
            }
        }

        /// <summary>
        /// Colors the "syllabes" in the text, according to the <see cref="SylConfig"/> attached to <c>conf</c>,
        /// i.e. fills <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the "syllabes".</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkSyls(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkSyls");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                ConcurrentBag<PhonWord> pws = GetPhonWords(conf);
                FormatSyls(pws, conf);
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser les syllabes sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser les syllabes sans une configuration valable.");
            }
        }

        /// <summary>
        /// Colors the "words" in the text, according to the <see cref="SylConfig"/> attached to <c>conf</c>, 
        /// i.e. fills <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the words.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkWords(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkWords");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                FormatWords(GetWords(), conf);
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser les mots sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser les mots sans une configuration valable.");
            }
        }

        /// <summary>
        /// Colors the "unspoken letters" (muettes :-)) in the text, according to the <see cref="ColConfWin"/> 
        /// corresponding to <c>pct == PhonConfType.muettes</c> attached to <c>conf</c>, i.e. fills
        /// <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkMuettes(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkMuettes");
            formatsMgmt.ClearFormats();
            ColorizePhons(conf, PhonConfType.muettes, pn);
            ApplyFormatting(conf, pn);
        }

        /// <summary>
        /// Colors the "voyelles" and "consonnes" in the text, according to the alternate colors defined in the <see cref="SylConfig"/>
        /// attached to <c>conf</c>, i.e. fills <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkVoyCons(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkVoyCons");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                conf.sylConf.ResetCounter();
                FormatVoyCons(0, S.Length - 1, conf);
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser les voyelles et les consonnes sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser les voyelles et les consonnes sans une configuration valable.");
            }
        }

        /// <summary>
        /// Forces the text to black color and no bold, italic, underline, ... formatting.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkNoir(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkNoir");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                CFForceBlack cfFB = new CFForceBlack();
                formatsMgmt.Add(new FormattedTextEl(this, 0, S.Length - 1, cfFB));
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de mettre le texte en noir sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de mettre le texte en noir sans une configuration valable.");
            }
        }

        /// <summary>
        /// Formats the lines of the text, according to the given <see cref="Config"/>. It fills 
        /// <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to use.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkLignes(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkLignes");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                List<int> eolPos = GetLastLinesPos();
                conf.sylConf.ResetCounter();
                int beg = 0;
                for (int i = 0; i < eolPos.Count; i++)
                {
                    formatsMgmt.Add(new FormattedTextEl(this, beg, eolPos[i], conf.sylConf.NextCF()));
                    beg = eolPos[i] + 1;
                }
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de marquer les lignes sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de marquer les lignes sans une configuration vallable.");
            }
        }


        /// <summary>
        /// Formats the text to the duo formatting defined in <c>conf</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> defining how the "duo" formatting should
        /// be applied.</param>
        /// <param name="pn">Optional: The <see cref="ProgressNotifier"/> that should be used to
        /// notify progress of the colorization. If present, the <c>pn</c> is already started. 
        /// It will not be finished here either. Progress will be incremented from 1 to 99. 
        /// </param>
        public void MarkDuo(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("MarkDuo");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                // Let us create two collections of words that contain the words to be formatted with 
                // subConf1, respectively subConf2.

                if (dc == null)
                {
                    dc = new DuoCache();
                }

                List<Word> wL1, wL2;
                ConcurrentBag<PhonWord> pws1, pws2;

                DuoConfig dConf = conf.duoConf;
                switch (dConf.colorisFunction)
                {
                    case DuoConfig.ColorisFunction.lettres:
                        dc.GetWordLists(GetWords(), dConf, GetLastLinesPos, out wL1, out wL2);
                        FormatLetters(wL1, dConf.subConfig1);
                        FormatLetters(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.mots:
                        dc.GetWordLists(GetWords(), dConf, GetLastLinesPos, out wL1, out wL2);
                        FormatWords(wL1, dConf.subConfig1);
                        FormatWords(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.voyCons:
                        dc.GetWordLists(GetWords(), dConf, GetLastLinesPos, out wL1, out wL2);
                        FormatVoyCons(wL1, dConf.subConfig1);
                        FormatVoyCons(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.syllabes:
                        dc.GetPhonWordLists(GetWords(), dConf, GetLastLinesPos, out pws1, out pws2);
                        FormatSyls(pws1, dConf.subConfig1);
                        FormatSyls(pws2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.muettes:
                        dc.GetPhonWordLists(GetWords(), dConf, GetLastLinesPos, out pws1, out pws2);
                        FormatPhons(pws1, dConf.subConfig1, PhonConfType.muettes);
                        FormatPhons(pws2, dConf.subConfig2, PhonConfType.muettes);
                        break;
                    case DuoConfig.ColorisFunction.phonemes:
                        dc.GetPhonWordLists(GetWords(), dConf, GetLastLinesPos, out pws1, out pws2);
                        FormatPhons(pws1, dConf.subConfig1, PhonConfType.phonemes);
                        FormatPhons(pws2, dConf.subConfig2, PhonConfType.phonemes);
                        break;

                    default:
                        logger.Error("Fonction à exécuter inconnue: \'{0}\'.", dConf.colorisFunction);
                        break;
                }
                ApplyFormatting(conf, pn);
            }
            else
            {
                logger.Error("conf == null. Impossible de coloriser en \'Duo\' sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de coloriser en \'Duo\' sans une configuration valable.");
            }
            logger.ConditionalTrace("MarkDuo EXIT");
        }

        public void AddFTE(FormattedTextEl fte) => formatsMgmt.Add(fte);

        // ****************************************************************************************
        // *                                   protected methods                                  *
        // ****************************************************************************************

        /// <summary>
        /// <para>
        /// This is to be considered as an abstract method that must be implemented by the inheriting class.
        /// The method is called for each <see cref="FormattedTextEl"/> that results from the operation applied to
        /// <c>TheText</c>.
        /// </para>
        /// <para>
        /// The intent is that the inheriting class can then apply the requested formatting to the text in the user interface.
        /// </para>
        /// <remarks>
        /// Is not abstract in order to make it possible to instantiate <c>TheText</c> and hence simplify testing.
        /// </remarks>
        /// </summary>
        /// <param name="fte">The <see cref="FormattedTextEl"/> that should be formatted on the output device.</param>
        /// <param name="conf">The <see cref="Config"/> that must be used for the formating.</param>
        protected virtual void SetChars(FormattedTextEl fte, Config conf) 
        {
            logger.Error("SetChars doit être implémentée par une classe descendante de TheText.");
            throw new NotImplementedException(
                "SetChars doit être implémenté par une classe descendante de TheText.");
        }

        /// <summary>
        /// Returns the list of the positions of the last characters of each line. Must be overridden
        /// by the inheriting classes. Default behaviour is to consider the text as one single line.
        /// </summary>
        /// <returns>The list of the zero based positions of the last characters of each line. Empty
        /// list if the text is empty.</returns>
        protected virtual List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            List<int> toReturn = new List<int>(1);
            if (S.Length > 0)
                toReturn.Add(S.Length - 1);
            return toReturn;
        }

        // ****************************************************************************************
        // *                                     private methods                                  *
        // ****************************************************************************************

        private void ApplyFormatting(Config conf, ProgressNotifier pn = null)
        {
            logger.ConditionalDebug("ApplyFormatting");
            if (pn is null)
            {
                foreach (FormattedTextEl fte in formatsMgmt.formats)
                    SetChars(fte, conf);
            } 
            else
            {
                // Progress notification principles: We consider that BeginPercent of the work was done
                // before we start here. The job here represents 100% - BeginPercent. We inform about
                // progress every ProgressIncrement.
                const float BeginPercent = 2.0f; 
                const float ProgressIncrement = 3.0f;

                float stepIncr = ((100 - BeginPercent) / formatsMgmt.formats.Count) - 0.001f;
                float progression = BeginPercent;
                float lastNotif = progression;
                pn.InProgress((int)lastNotif);

                foreach (FormattedTextEl fte in formatsMgmt.formats)
                {
                    SetChars(fte, conf);
                    progression += stepIncr;
                    if ((progression - lastNotif) > ProgressIncrement)
                    {
                        lastNotif = progression;
                        pn.InProgress((int)lastNotif);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a ConcurrentBag of <c>PhonWord</c> corresponding to <paramref name="wordList"/> and
        /// computed according to <paramref name="conf"/>;
        /// </summary>
        /// <param name="wordList">The <c>Word</c>(s) whose corresponding <c>PhonWord</c>(s) 
        /// must be computed.</param>
        /// <param name="conf">The <c>Config</c> that must be used for the computation.</param>
        /// <returns></returns>
        private static ConcurrentBag<PhonWord> GetPhonWords(List<Word> wordList, Config conf)
        {
            // Limite du nombre de mots au-dessus de laquelle on travaille en parallèle.
            const int ParallelLimit = 50; 
            logger.ConditionalDebug("GetPhonWords (wordList, conf), nr mots: {0}", wordList.Count);
            ConcurrentBag<PhonWord> toReturn = new ConcurrentBag<PhonWord>();
            if (wordList.Count > ParallelLimit)
            {
                Parallel.ForEach(wordList, (w) => { toReturn.Add(new PhonWord(w, conf)); });
            }
            else
            {
                foreach (Word w in wordList)
                    toReturn.Add(new PhonWord(w, conf));
            }
            return toReturn;
        }

        private ConcurrentBag<PhonWord> GetPhonWords(Config conf)
        {
            logger.ConditionalDebug("GetPhonWords(conf)");
            if (phonWords == null)
            {
                List<Word> theWords = GetWords();
                phonWords = GetPhonWords(theWords, conf);
            }
            return phonWords;
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format the phonèmes for the <see cref="PhonWord"/>(s) in
        /// <paramref name="pws"/>, using the
        /// <c>ColConfWin</c> defined by <paramref name="conf"/> and <paramref name="pct"/>.
        /// </summary>
        /// <param name="pws">The list of words to format.</param>
        /// <param name="conf">The <c>Config</c> to use for the fromatting.</param>
        /// <param name="pct">The <c>ColConfWin</c> within <c>conf</c> to use for the fromatting.</param>
        private void FormatPhons(ConcurrentBag<PhonWord> pws, Config conf, PhonConfType pct)
        {
            logger.ConditionalDebug("FormatPhons");
            foreach (PhonWord pw in pws)
                pw.ColorPhons(conf, pct);
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format the letters according to <c>conf</c>, for all
        /// characters in <c>this</c> <c>TheText</c> between zero based positions <c>first</c> and
        /// <c>last</c>.
        /// </summary>
        /// <param name="first">Position of the first character in <c>S</c> to format.</param>
        /// <param name="last">Position of the last character in <c>S</c> to format.</param>
        /// <param name="conf">The <c>Config</c> to apply.</param>
        private void FormatLetters(int first, int last, Config conf)
        {
            logger.ConditionalDebug("FormatLetters(int first, int last, Config conf)");
            Debug.Assert(last < S.Length);
            for (int i = first; i <= last; i++)
            {
                CharFormatting cf = conf.pBDQ.GetCfForPBDQLetter(S[i]);
                if (cf != null)
                    formatsMgmt.Add(new FormattedTextEl(this, i, i, cf));
            }
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format the letters according to <c>conf</c>, for all
        /// characters in <paramref name="wL"/>.
        /// </summary>
        /// <param name="wL">List of the <see cref="Word"/>(s) to format.</param>
        /// <param name="conf"><see cref="Config"/>to use for the formatting.</param>
        private void FormatLetters(List<Word> wL, Config conf)
        {
            logger.ConditionalDebug("FormatLetters(List<Word> wL, Config conf)");
            foreach (Word w in wL)
            {
                FormatLetters(w.First, w.Last, conf);
            }
        }

        /// <summary>
        /// Formats the list of <see cref="PhonWord"/>(s) in order to highlight the syllabes,
        /// according to <paramref name="conf"/>. I.e. Adds the corresponding 
        /// <see cref="FormattedTextEl"/>(s) to <see cref="formatsMgmt"/>.
        /// </summary>
        /// <param name="pws">The list of <see cref="PhonWord"/>(s) to format.</param>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        private void FormatSyls(ConcurrentBag<PhonWord> pws, Config conf)
        {
            logger.ConditionalDebug("FormatSyls");
            conf.sylConf.ResetCounter();
            foreach (PhonWord pw in pws)
                pw.ComputeAndColorSyls(conf);
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format the words in <paramref name="wL"/> 
        /// according to <c>conf</c>.
        /// </summary>
        /// <param name="wL">List of <see cref="Word"/>(s) to format.</param>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        private void FormatWords(List<Word> wL, Config conf)
        {
            logger.ConditionalDebug("FormatWords");
            conf.sylConf.ResetCounter();
            foreach (Word w in wL)
                w.PutColor(conf);
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format vowels and consonants according to <c>conf</c>, for all
        /// characters in <c>this</c> <c>TheText</c> between zero based positions <c>first</c> and
        /// <c>last</c>.
        /// </summary>
        /// <param name="first">Position of the first character in <c>S</c> to format.</param>
        /// <param name="last">Position of the last character in <c>S</c> to format.</param>
        /// <param name="conf">The <c>Config</c> to apply.</param>
        private void FormatVoyCons(int first, int last, Config conf)
        {
            logger.ConditionalDebug("FormatVoyCons(int first, int last, Config conf)");
            CharFormatting voyCF = conf.sylConf.NextCF();
            CharFormatting consCF = conf.sylConf.NextCF();
            int start, end;
            int i = first;
            if (smallCapsS == null)
            {
                smallCapsS = S.ToLower(BaseConfig.cultF);
            }
            while (i <= last)
            {
                if (TextEl.EstVoyelle(smallCapsS[i]))
                {
                    start = i;
                    i++;
                    while ((i < smallCapsS.Length) && (TextEl.EstVoyelle(smallCapsS[i])))
                        i++;
                    end = i - 1;
                    formatsMgmt.Add(new FormattedTextEl(this, start, end, voyCF));
                }
                else if (TextEl.EstConsonne(smallCapsS[i]))
                {
                    start = i;
                    i++;
                    while ((i < smallCapsS.Length) && (TextEl.EstConsonne(smallCapsS[i])))
                        i++;
                    end = i - 1;
                    formatsMgmt.Add(new FormattedTextEl(this, start, end, consCF));
                }
                else
                    i++;
            }
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format vowels and consonants according to <c>conf</c>, for all
        /// characters in <paramref name="wL"/>.
        /// </summary>
        /// <param name="wL">List of the <see cref="Word"/>(s) to format.</param>
        /// <param name="conf"><see cref="Config"/>to use for the formatting.</param>
        private void FormatVoyCons(List<Word> wL, Config conf)
        {
            logger.ConditionalDebug("FormatVoyCons(List<Word> wL, Config conf)");
            foreach (Word w in wL)
            {
                FormatVoyCons(w.First, w.Last, conf);
            }
        }
    }
}
