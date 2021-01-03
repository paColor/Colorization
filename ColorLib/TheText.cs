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
using System.Diagnostics;
using System.Text.RegularExpressions;
using NLog;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using ColorLib.Dierese;

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
            private ConcurrentBag<PhonWord> cachedPWBag1;
            private ConcurrentBag<PhonWord> cachedPWBag2;
            private List<PhonWord> cachedPWL1;
            private List<PhonWord> cachedPWL2;
            private List<PhonWord> cachedComplList;

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
            SylConfig.Mode sylMode1;
            SylConfig.Mode sylMode2;
            bool marquerMuettes1;
            bool marquerMuettes2;
            bool cachedMergeApostrophe;



            public DuoCache()
            {
                logger.ConditionalDebug("DuoCache");
                cachedWordList1 = null;
                cachedWordList2 = null;
                cachedPWBag1 = null;
                cachedPWBag2 = null;
                cachedPWL1 = null;
                cachedPWL2 = null;
                cachedComplList = null;

                // Il n'est pas nécessaire d'initialiser les champs de gestion de l'actualité
                // du cache. Pour avoir un état clair, initialisons quand même ceux où on peut 
                // marquer clairement qu'il s'agit d'un état indéfini.
                alt = DuoConfig.Alternance.undefined;
                nbreAlternance = 999;
                illRule1Muettes = ColConfWin.IllRule.undefined;
                illRule2Muettes = ColConfWin.IllRule.undefined;
                illRule1Phon = ColConfWin.IllRule.undefined;
                illRule2Phon = ColConfWin.IllRule.undefined;
                sylMode1 = SylConfig.Mode.undefined;
                sylMode2 = SylConfig.Mode.undefined;
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
                out List<Word> wL1, out List<Word> wL2, bool mergeApostrophe)
            {
                // We measured 9 ms duration for a very long text (a fat book). So this is not the place to
                // put more sophisticated parallelism.
                logger.ConditionalDebug("GetWordLists");
                CheckCacheValidity(dConf, mergeApostrophe);
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
                            throw new ArgumentException(String.Format(ConfigBase.cultF,
                                "Type d'alternance non traité par la commande \'Duo\'." +
                                "alternance: \'{0}\'", alt));
                            break;
                    }
                }
                wL1 = cachedWordList1;
                wL2 = cachedWordList2;
            }

            /// <summary>
            /// Returns the two bags of <see cref="PhonWord"/>(s) that correspond to the passed List of
            /// <c>Word</c> and the passed <see cref="DuoConfig"/>.
            /// </summary>
            /// <param name="wL">List of <see cref="Word"/>(s) to split between the two lists of 
            /// <see cref="PhonWord"/>(s).</param>
            /// <param name="dConf">The <see cref="DuoConfig"/> to apply.</param>
            /// <param name="getEolPos">The method giving the positions of the last characters on
            /// each line.</param>
            /// <param name="pwB1">Out: the first bag of <see cref="PhonWord"/></param>
            /// <param name="pwB2">Out: the second bag of <see cref="PhonWord"/></param>
            public void GetPhonWordBags(List<Word> wL, DuoConfig dConf, GetTextPos getEolPos,
                out ConcurrentBag<PhonWord> pwB1, out ConcurrentBag<PhonWord> pwB2, bool mergeApostrophe)
            {
                logger.ConditionalDebug("GetPhonWordBags");
                CheckCacheValidity(dConf, mergeApostrophe);
                if (cachedPWBag1 == null)
                {
                    List<Word> wL1;
                    List<Word> wL2;
                    GetWordLists(wL, dConf, getEolPos, out wL1, out wL2, mergeApostrophe);

                    // Parallel way
                    ComputePhonWords cpw = ComputePW;
                    IAsyncResult asr = cpw.BeginInvoke(wL1, dConf.subConfig1, ref cachedPWBag1, null, null);
                    ComputePW(wL2, dConf.subConfig2, ref cachedPWBag2);
                    cpw.EndInvoke(ref cachedPWBag1, asr);

                    // Sequential way
                    //cachedPWL1 = GetPhonWords(wL1, dConf.subConfig1);
                    //cachedPWL2 = GetPhonWords(wL2, dConf.subConfig2);
                }
                pwB1 = cachedPWBag1;
                pwB2 = cachedPWBag2;
            }

            /// <summary>
            /// Returns the two sorted lists of <see cref="PhonWord"/> that correspond to the 
            /// passed list of <see cref="Word"/> and <c>dConf</c>.
            /// </summary>
            /// <param name="wL">List of <see cref="Word"/>(s) to split between the two lists of 
            /// <see cref="PhonWord"/>(s).</param>
            /// <param name="dConf">The <see cref="DuoConfig"/> to apply.</param>
            /// <param name="getEolPos">The method giving the positions of the last characters on
            /// each line.</param>
            /// <param name="pwL1">Out: the first list of <see cref="PhonWord"/></param>
            /// <param name="pwL2">Out: the second list of <see cref="PhonWord"/></param>
            /// <param name="compL"> Out: List of all <c>PhonWord</c> in both other lists.</param>
            /// <param name="mergeApostrophe">Inidcates whether the apostrophy and the shortened
            /// max 2 letters word should be merged with the following word. Makes sense when
            /// syllabes are looked for.</param>
            public void GetPhonWordLists(List<Word> wL, DuoConfig dConf, GetTextPos getEolPos,
                out List<PhonWord> pwL1, out List<PhonWord> pwL2, out List<PhonWord> compL,
                bool mergeApostrophe)
            {
                logger.ConditionalDebug("GetPhonWordLists");
                CheckCacheValidity(dConf, mergeApostrophe);
                if (cachedPWL1 == null)
                {
                    ConcurrentBag<PhonWord> pwB1;
                    ConcurrentBag<PhonWord> pwB2;
                    GetPhonWordBags(wL, dConf, getEolPos, out pwB1, out pwB2, mergeApostrophe);
                    cachedPWL1 = new List<PhonWord>(pwB1);
                    cachedPWL2 = new List<PhonWord>(pwB2);
                    cachedPWL1.Sort(TextEl.CompareTextElByPosition);
                    cachedPWL2.Sort(TextEl.CompareTextElByPosition);
                    cachedComplList = new List<PhonWord>(cachedPWL1.Count + cachedPWL2.Count);
                    foreach (PhonWord pw in cachedPWL1)
                        cachedComplList.Add(pw);
                    foreach (PhonWord pw in cachedPWL2)
                        cachedComplList.Add(pw);
                    cachedComplList.Sort(TextEl.CompareTextElByPosition);
                }
                pwL1 = cachedPWL1;
                pwL2 = cachedPWL2;
                compL = cachedComplList;
            }

            delegate void ComputePhonWords(List<Word> wL, Config subConf, ref ConcurrentBag<PhonWord> pwL);

            private void ComputePW(List<Word> wL, Config subConf, ref ConcurrentBag<PhonWord> pwL)
            {
                pwL = GetPhonWords(wL, subConf);
            }

            private void CheckCacheValidity(DuoConfig dConf, bool mergeApostrophe)
            {
                logger.ConditionalDebug("CheckCacheValidity");
                if ((alt != dConf.alternance)
                    || (nbreAlternance != dConf.nbreAlt)
                    || (doubleConsStd1 != dConf.subConfig1.sylConf.DoubleConsStd)
                    || (doubleConsStd2 != dConf.subConfig2.sylConf.DoubleConsStd)
                    || (sylMode1 != dConf.subConfig1.sylConf.mode)
                    || (sylMode2 != dConf.subConfig2.sylConf.mode)
                    || (marquerMuettes1 != dConf.subConfig1.sylConf.marquerMuettes)
                    || (marquerMuettes2 != dConf.subConfig2.sylConf.marquerMuettes)
                    || (illRule1Muettes != dConf.subConfig1.colors[PhonConfType.muettes].IllRuleToUse)
                    || (illRule2Muettes != dConf.subConfig2.colors[PhonConfType.muettes].IllRuleToUse) 
                    || (illRule1Phon != dConf.subConfig1.colors[PhonConfType.phonemes].IllRuleToUse)
                    || (illRule2Phon != dConf.subConfig2.colors[PhonConfType.phonemes].IllRuleToUse)
                    || (cachedMergeApostrophe != mergeApostrophe)
                   )
                {
                    logger.ConditionalTrace("Invalidate cache");
                    cachedWordList1 = null;
                    cachedWordList2 = null;
                    cachedPWBag1 = null;
                    cachedPWBag2 = null;
                    cachedPWL1 = null;
                    cachedPWL2 = null;
                    alt = dConf.alternance;
                    nbreAlternance = dConf.nbreAlt;
                    doubleConsStd1 = dConf.subConfig1.sylConf.DoubleConsStd;
                    doubleConsStd2 = dConf.subConfig2.sylConf.DoubleConsStd;
                    sylMode1 = dConf.subConfig1.sylConf.mode;
                    sylMode2 = dConf.subConfig2.sylConf.mode;
                    marquerMuettes1 = dConf.subConfig1.sylConf.marquerMuettes;
                    marquerMuettes2 = dConf.subConfig2.sylConf.marquerMuettes;
                    illRule1Muettes = dConf.subConfig1.colors[PhonConfType.muettes].IllRuleToUse;
                    illRule2Muettes = dConf.subConfig2.colors[PhonConfType.muettes].IllRuleToUse;
                    illRule1Phon = dConf.subConfig1.colors[PhonConfType.phonemes].IllRuleToUse;
                    illRule2Phon = dConf.subConfig2.colors[PhonConfType.phonemes].IllRuleToUse;
                    cachedMergeApostrophe = mergeApostrophe;
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
        // *                               public static methods                                  *
        // ****************************************************************************************

        /// <summary>
        /// Initializes the static elements of the whole <c>ColorLib</c> library. Must be called
        /// before an usage of the library.
        /// </summary>
        /// <param name="errMsgs">Si une erreur se produit, un message est ajouté à la liste. 
        /// La liste n'est pas touchée si tout se passe bien. <c>null</c> indique que le message
        /// n'est pas souhaité par l'appelant.</param>
        /// <example>
        /// Il est vivement conseillé d'informer l'utilisateur des erreurs rencontrées. En utilisant
        /// <c>Windows.Forms</c> on peut par exemple faire ceci:
        /// <code>
        /// List&lt;string&gt; errMsgs = new List&lt;string&gt;();
        /// TheText.Init(errMsgs);
        /// foreach(string errMsg in errMsgs)
        ///     MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        /// </code>
        /// </example>
        public static void Init(List<string> errMsgs = null)
        {
            logger.ConditionalDebug("Init");
            ConfigBase.Init(errMsgs);
            AutomAutomat.InitAutomat();
            SylInW.Init();
            Config.Init(errMsgs);
        }

        // ****************************************************************************************
        // *                                    public members                                   *
        // ****************************************************************************************

        /// <summary>
        /// The text that was entered when the object was constructed. Cannot be null, but can be
        /// empty.
        /// </summary>
        public string S { get; private set; }


        // ****************************************************************************************
        // *                                    private members                                   *
        // ****************************************************************************************

        private string lowerCaseS;
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
            logger.ConditionalDebug(ConfigBase.cultF, "TheText");
            logger.ConditionalTrace(ConfigBase.cultF, "TheText Constructor, txt: \'{0}\'.", txt);
            Debug.Assert(txt != null);
            this.S = txt;
            formatsMgmt = new FormatsMgmt(S.Length);
            dc = null;
            lowerCaseS = null;
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
        /// Returns the lower case version of the text.
        /// </summary>
        /// <returns><c>string</c> conatining the lower case version of the text.</returns>
        public string ToLowerString()
        {
            if (lowerCaseS == null)
            {
                lowerCaseS = S.ToLower(ConfigBase.cultF);
            }
            return lowerCaseS;
        }

        /// <summary>
        /// Retrurns the list of <c>Words</c> contained in the text.
        /// </summary>
        /// <remarks>Is not needed by a normal consumer of the class.</remarks>
        /// <param name="mergeApostrophe">If <c>true</c>, words ending with an apostrophe are merged
        /// with their successor</param>
        /// <returns>List of <c>Words</c> contained in the text</returns>
        private List<Word> GetWords(bool mergeApostrophe = false)
            // public for test reasons
        {
            logger.ConditionalDebug("GetWords");
            List<Word> words = new List<Word>(S.Length / 5); // longueur moyenne d'un mot avec l'espace : 5 charactères...
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(S);
            int i = 0;
            while (i < matches.Count)
            {
                Match match = matches[i];
                int beg = match.Index;
                int end = beg + match.Length - 1;

                // Apostrophes: On considère comme apostrophes, les caractères ' ou ’ placé après une ou deux lettres.
                // Cela couvre les formes élidées de le, que, ce, te... Y en  a-t-il d'autres?
                // Il peut y avoir confusion avec le guillemet simple. Tant pis!
                // Le mot est allongé pour contenir l'apostrophe comme dernière lettre.
                // [09.07.2020] On considère le trait d'union comme une apostrophe s'il suit 
                // un 't' seul, comme dans arriva-t-il.
                if ((match.Length <= 2) 
                    && (end + 1 < S.Length) 
                    && ((S[end + 1] == '\'') 
                        || (S[end + 1] == '’') 
                        || (match.Value == "t" && S[end + 1] == '-')))
                {
                    if (mergeApostrophe && i < matches.Count - 1)
                    {
                        Match nextMatch = matches[i + 1];
                        end = nextMatch.Index + nextMatch.Length - 1;
                        i++;
                    }
                    else
                    {
                        end++;
                    }
                }

                Word w = new Word(this, beg, end);
                words.Add(w);
                i++;
            }
            return words;
        }

        /// <summary>
        /// Returns the list of <c>PhonWords</c> contained in the text.
        /// </summary>
        /// <remarks>Is not needed by a normal consumer of the class and should only be used for testing.</remarks>
        /// <param name="conf">The <see cref="Config"/> to use for the detection of the "phonèmes".</param>
        /// <returns>List of <c>PhonWords</c> contained in the text.</returns>
        public List<PhonWord> GetPhonWordList(Config conf, bool mergeApostrophe = false)
            // public for test reasons
        {
            logger.ConditionalDebug("GetPhonWordList");
            ConcurrentBag<PhonWord> pws = GetPhonWords(conf, mergeApostrophe);
            List<PhonWord> toReturn = new List<PhonWord>(pws);
            toReturn.Sort(TextEl.CompareTextElByPosition);
            return toReturn;
        }

        /// <summary>
        /// Applies the formattings defined in the <c>ColConfWin</c> identified by <paramref name="conf"/> and 
        /// <paramref name="pct"/> to the 
        /// "phonèmes" in the text. I.e. fills <c>Formats</c> and makes sure that 
        /// <c>SetChars</c> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/> is not called.
        /// </remarks>
        /// <param name="pct">Identifies the <c>ColConfWin</c> (see <see cref="ColorLib.ColConfWin"/>) that must
        /// be used when coloring the "phonèmes".</param>
        /// <param name="conf">The <c>Config</c> to use for the colorization.</param>
        public void ColorizePhons(Config conf, PhonConfType pct)
        {
            logger.ConditionalDebug("ColorizePhons");
            if (conf != null)
            { 
                formatsMgmt.ClearFormats();
                ConcurrentBag<PhonWord> pws = GetPhonWords(conf);
                FormatPhons(pws, conf, pct);
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/> is not called.
        /// </remarks>
        /// <param name="conf">The <see cref="Config"/> that must be used for marking the letters.</param>
        public void MarkLetters(Config conf)
        {
            logger.ConditionalDebug("MarkLetters");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                FormatLetters(0, S.Length - 1, conf);
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/> is not called.
        /// </remarks>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the "syllabes".</param>
        public void MarkSyls(Config conf)
        {
            logger.ConditionalDebug("MarkSyls");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                // on prend une liste ordonnée, car l'alternance de couleur pour les syllabes s'étend
                // au-delà de la frontière du mot.
                List<PhonWord> pws = GetPhonWordList(conf, true);
                ComputeSyls(pws);
                if (conf.sylConf.mode == SylConfig.Mode.poesie && conf.sylConf.chercherDierese)
                {
                    _ = AnalyseDierese.ChercheDierese(this, pws, conf.sylConf.nbrPieds);
                }
                ColorizeSyls(pws, conf);
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/>
        /// is not called.</remarks>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the words.</param>
        public void MarkWords(Config conf)
        {
            logger.ConditionalDebug("MarkWords");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                FormatWords(GetWords(), conf);
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/>
        /// is not called. </remarks>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the words.</param>
        public void MarkMuettes(Config conf)
        {
            logger.ConditionalDebug("MarkMuettes");
            formatsMgmt.ClearFormats();
            ColorizePhons(conf, PhonConfType.muettes);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "voyelles" and "consonnes" in the text, according to the alternate colors
        /// defined in the <see cref="SylConfig"/>
        /// attached to <c>conf</c>, i.e. fills <see cref="formatsMgmt"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl, Config)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/> is not called.
        /// </remarks>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the words.</param>
        public void MarkVoyCons(Config conf)
        {
            logger.ConditionalDebug("MarkVoyCons");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                conf.sylConf.ResetCounter();
                FormatVoyCons(0, S.Length - 1, conf);
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/>
        /// is not called. </remarks>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        public void MarkNoir(Config conf)
        {
            logger.ConditionalDebug("MarkNoir");
            if (conf != null)
            {
                if (S.Length > 0)
                {
                    formatsMgmt.ClearFormats();
                    CFForceBlack cfFB = new CFForceBlack();
                    formatsMgmt.Add(new FormattedTextEl(this, 0, S.Length - 1, cfFB));
                    ApplyFormatting(conf);
                }
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/>
        /// is not called. </remarks>
        /// <param name="conf">The <see cref="Config"/> to use.</param>
        public void MarkLignes(Config conf)
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
                ApplyFormatting(conf);
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
        /// <remarks> <c>ProgressNotifier.thePN</c> must be started. Progress will be signalled
        /// from 1% to 99%. <c>ProgressNotifier.thePN</c> is not 'completed', 
        /// i.e. <see cref="ProgressNotifier.Completed"/> method of <see cref="ProgressNotifier"/>
        /// is not called.</remarks>
        /// <param name="conf">The <see cref="Config"/> defining how the "duo" formatting should
        /// be applied.</param>
        public void MarkDuo(Config conf)
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
                ConcurrentBag<PhonWord> pwBag1, pwBag2;

                DuoConfig dConf = conf.duoConf;
                switch (dConf.colorisFunction)
                {
                    case DuoConfig.ColorisFunction.lettres:
                        dc.GetWordLists(GetWords(false), dConf, GetLastLinesPos, out wL1, out wL2, false);
                        FormatLetters(wL1, dConf.subConfig1);
                        FormatLetters(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.mots:
                        dc.GetWordLists(GetWords(false), dConf, GetLastLinesPos, out wL1, out wL2, false);
                        FormatWords(wL1, dConf.subConfig1);
                        FormatWords(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.voyCons:
                        dc.GetWordLists(GetWords(false), dConf, GetLastLinesPos, out wL1, out wL2, false);
                        FormatVoyCons(wL1, dConf.subConfig1);
                        FormatVoyCons(wL2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.syllabes:
                        List<PhonWord> pwList1, pwList2, completeList;
                        dc.GetPhonWordLists(GetWords(true), dConf, GetLastLinesPos, out pwList1, 
                            out pwList2, out completeList, true);
                        ComputeSyls(completeList);
                        if (dConf.subConfig1.sylConf.mode == SylConfig.Mode.poesie
                            && dConf.subConfig2.sylConf.mode == SylConfig.Mode.poesie
                            && dConf.subConfig1.sylConf.chercherDierese
                            && dConf.subConfig2.sylConf.chercherDierese)
                        {
                            int nbrPieds = 0;
                            if (dConf.subConfig1.sylConf.nbrPieds == dConf.subConfig2.sylConf.nbrPieds)
                            {
                                nbrPieds = dConf.subConfig1.sylConf.nbrPieds;
                            }
                            _ = AnalyseDierese.ChercheDierese(this, completeList, nbrPieds);
                        }
                        ColorizeSyls(pwList1, dConf.subConfig1);
                        ColorizeSyls(pwList2, dConf.subConfig2);
                        break;
                    case DuoConfig.ColorisFunction.muettes:
                        dc.GetPhonWordBags(GetWords(false), dConf, GetLastLinesPos, out pwBag1, out pwBag2, false);
                        FormatPhons(pwBag1, dConf.subConfig1, PhonConfType.muettes);
                        FormatPhons(pwBag2, dConf.subConfig2, PhonConfType.muettes);
                        break;
                    case DuoConfig.ColorisFunction.phonemes:
                        dc.GetPhonWordBags(GetWords(false), dConf, GetLastLinesPos, out pwBag1, out pwBag2, false);
                        FormatPhons(pwBag1, dConf.subConfig1, PhonConfType.phonemes);
                        FormatPhons(pwBag2, dConf.subConfig2, PhonConfType.phonemes);
                        break;

                    default:
                        logger.Error("Fonction à exécuter inconnue: \'{0}\'.", dConf.colorisFunction);
                        break;
                }
                ApplyFormatting(conf);
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

        private void ApplyFormatting(Config conf)
        {
            logger.ConditionalDebug("ApplyFormatting");
            // Progress notification principles: We consider that BeginPercent of the work was done
            // before we start here. The job here represents 100% - BeginPercent. We inform about
            // progress every ProgressIncrement.
            const float BeginPercent = 10.0f; 
            // PAE 03.01.21 - chagé de 5 à 10 en raison de 
            // l'augmentation du temps de traitement dans le coeur avec les cas Morphalou.
            const float ProgressIncrement = 2.0f;

            float stepIncr = ((100 - BeginPercent) / formatsMgmt.formats.Count) - 0.001f;
            float progression = BeginPercent;
            float lastNotif = progression;
            ProgressNotifier.thePN.InProgress((int)lastNotif);

            foreach (FormattedTextEl fte in formatsMgmt.formats)
            {
                SetChars(fte, conf);
                progression += stepIncr;
                if ((progression - lastNotif) > ProgressIncrement)
                {
                    lastNotif = progression;
                    ProgressNotifier.thePN.InProgress((int)lastNotif);
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

        private ConcurrentBag<PhonWord> GetPhonWords(Config conf, bool mergeApostrophe = false)
        {
            logger.ConditionalDebug("GetPhonWords(conf)");
            List<Word> theWords = GetWords(mergeApostrophe);
            return GetPhonWords(theWords, conf);
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

        private void ComputeSyls(List<PhonWord> pws)
        {
            logger.ConditionalDebug("ComputeSyls");
            foreach (PhonWord pw in pws)
                pw.ComputeSyls();
        }

        private void ColorizeSyls(List<PhonWord> pws, Config conf)
        {
            logger.ConditionalDebug("ColorizeSyls");
            conf.sylConf.ResetCounter();
            foreach (PhonWord pw in pws)
                pw.ColorizeSyls(conf);
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
            if (lowerCaseS == null)
            {
                lowerCaseS = S.ToLower(ConfigBase.cultF);
            }
            while (i <= last)
            {
                if (TextEl.EstVoyelle(lowerCaseS[i]))
                {
                    start = i;
                    i++;
                    while ((i < lowerCaseS.Length) && (TextEl.EstVoyelle(lowerCaseS[i])))
                        i++;
                    end = i - 1;
                    formatsMgmt.Add(new FormattedTextEl(this, start, end, voyCF));
                }
                else if (TextEl.EstConsonne(lowerCaseS[i]))
                {
                    start = i;
                    i++;
                    while ((i < lowerCaseS.Length) && (TextEl.EstConsonne(lowerCaseS[i])))
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
