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
            private List<PhonWord> cachedPWL1;
            private List<PhonWord> cachedPWL2;

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
                out List<PhonWord> pwL1, out List<PhonWord> pwL2)
            {
                logger.ConditionalDebug("GetPhonWordLists");
                CheckCacheValidity(dConf);
                if (cachedPWL1 == null)
                {
                    List<Word> wL1;
                    List<Word> wL2;
                    GetWordLists(wL, dConf, getEolPos, out wL1, out wL2);
                    cachedPWL1 = GetPhonWords(wL1, dConf.subConfig1);
                    cachedPWL2 = GetPhonWords(wL2, dConf.subConfig2);
                }
                pwL1 = cachedPWL1;
                pwL2 = cachedPWL2;
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
            public Dictionary<CharFormatting, List<FormattedTextEl>> formatsPerCF { get; private set; }

            /// <summary>
            /// Crée un manager de formats
            /// </summary>
            /// <param name="sLength">Longueur du texte dont il faut s^'occuper des formats.</param>
            public FormatsMgmt(int inSLength)
            {
                logger.ConditionalDebug("FormatsMgmt sLength: {0}", inSLength);
                sLength = inSLength;
                formatsPerCF = new Dictionary<CharFormatting, List<FormattedTextEl>>(16); 
                // 16 au pif. Il semble peu fréquent d'avoir plus de 16 formatages différents 
                // dans un texte. Il y en a 13 dans la config CERAS.
            }

            /// <summary>
            /// Réinitialise la gestion des formats. Efface tout ce qui pourrait être présent.
            /// </summary>
            public void ClearFormats()
            {
                logger.ConditionalDebug("ClearFormats");
                formatsPerCF.Clear();
            }

            public void Add(FormattedTextEl fte)
            {
                List<FormattedTextEl> ftes;
                if (!formatsPerCF.TryGetValue(fte.cf, out ftes))
                {
                    ftes = new List<FormattedTextEl>(sLength/10);
                    formatsPerCF.Add(fte.cf, ftes);
                }
                ftes.Add(fte);
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
        /// Initializes the static elements of the whole <c>ColorLib</c> library. Must be called befor any
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
        private List<PhonWord> phonWords; // The corresponding PhonWords (computed when needed).
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
        /// <param name="inConf">The <c>Config</c> that will be used when applying formats to the text.</param>
        public TheText(string txt)
        {
            logger.ConditionalDebug(BaseConfig.cultF, "TheText, txt: \'{0}\'.", txt);
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
        public List<PhonWord> GetPhonWords(Config conf)
            // public for test reasons
        {
            logger.ConditionalDebug("GetPhonWords");
            if (phonWords == null)
            {
                List<Word> theWords = GetWords();
                phonWords = GetPhonWords(theWords, conf);
            }
            return phonWords;
        }

        /// <summary>
        /// Applies the formattings defined in the <c>ColConfWin</c> identified by <paramref name="conf"/> and 
        /// <paramref name="pct"/> to the 
        /// "phonèmes" in the text. I.e. fills <c>Formats</c> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="pct">Identifies the <c>ColConfWin</c> (see <see cref="ColorLib.ColConfWin"/>) that msut
        /// be used when coloring the "phonèmes".</param>
        public void ColorizePhons(Config conf, PhonConfType pct)
        {
            logger.ConditionalDebug("ColorizePhons");
            if (conf != null)
            { 
                formatsMgmt.ClearFormats();
                List<PhonWord> pws = GetPhonWords(conf);
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
        ///i.e. fills <see cref="formats"/>and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
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
        /// i.e. fills <see cref="formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the "syllabes".</param>
        public void MarkSyls(Config conf)
        {
            logger.ConditionalDebug("MarkSyls");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                List<PhonWord> pws = GetPhonWords(conf);
                FormatSyls(pws, conf);
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
        /// i.e. fills <see cref="formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
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
        /// <see cref="formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf"></param>
        public void MarkMuettes(Config conf)
        {
            logger.ConditionalDebug("MarkMuettes");
            formatsMgmt.ClearFormats();
            ColorizePhons(conf, PhonConfType.muettes);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "voyelles" and "consonnes" in the text, according to the alternate colors defined in the <see cref="SylConfig"/>
        /// attached to <c>conf</c>, i.e. fills <see cref="formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
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
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        public void MarkNoir(Config conf)
        {
            logger.ConditionalDebug("MarkNoir");
            if (conf != null)
            {
                formatsMgmt.ClearFormats();
                CFForceBlack cfFB = new CFForceBlack();
                formatsMgmt.Add(new FormattedTextEl(this, 0, S.Length - 1, cfFB));
                ApplyFormatting(conf);
            }
            else
            {
                logger.Error("conf == null. Impossible de mettre le texte en noir sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de mettre le texte en noir sans une configuration valable.");
            }
        }

        /// <summary>
        /// Formats the lines of the text, according to the given <see cref="Config"/>. It fills 
        /// <see cref="formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
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
                List<PhonWord> pws1, pws2;

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
            logger.Error("SetChars ou DisplayFormat doit être implémentée par une classe descendante de TheText.");
            throw new NotImplementedException(
                "SetChars ou DisplayFormat doit être implémenté par une classe descendante de TheText.");
        }

        /// <summary>
        /// Applique le formatage <paramref name="cf"/> à tous les <see cref="FormattedTextEl"/> de
        /// la liste.
        /// </summary>
        /// <param name="cf">Le formatage à appliquer.</param>
        /// <param name="ftes">La liste des <see cref="FormattedTextEl"/> qui doivent être affichés.
        /// Notez que tous les membres de <c>ftes</c> ont le <see cref="CharFormatting"/> <c>cf</c>.
        /// </param>
        /// <param name="conf">La <see cref="Config"/> à utiliser.</param>
        /// <remarks>
        /// <para>
        /// Cette méthode est appelée au moment ou le résultat d'une commande doit être affiché.
        /// Elle est exécutée pour chaque <see cref="CharFormatting"/> utilisé par la commande.
        /// Par défaut elle utilise <c>SetChars</c> pour chaque élément de <c>ftes</c>. 
        /// </para>
        /// <para>
        /// l'hypothèse est que certains héritiers seront plus performants s'ils peuvent créer un
        /// ensemble d'éléments de texte disjoints devant recevoir le même formatage que s'ils 
        /// reformatent chaque élément de texte l'un après l'autre. Chaque héritier peut donc choisir
        /// de réaliser soit <see cref="SetChars(FormattedTextEl, Config)"/>, soit 
        /// <c>DisplayFormat</c> en fonction du gain de performance que leui donnerait 
        /// <c>DisplayFormat</c>. Inutile de réaliser les deux méthodes.
        /// </para>
        /// </remarks>
        protected virtual void DisplayFormat(CharFormatting cf, List<FormattedTextEl> ftes, Config conf)
        {
            foreach (FormattedTextEl fte in ftes)
                SetChars(fte, conf);
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
            //foreach (FormattedTextEl fte in formats)
            //    SetChars(fte, conf);
            foreach(KeyValuePair<CharFormatting, List<FormattedTextEl>> kvp in formatsMgmt.formatsPerCF)
            {
                DisplayFormat(kvp.Key, kvp.Value, conf);
            }
        }

        /// <summary>
        /// Returns a list of <c>PhonWord</c> corresponding to <paramref name="wordList"/> and
        /// computet according to <paramref name="conf"/>;
        /// </summary>
        /// <param name="wordList">The <c>Word</c>(s) whose corresponding <c>PhonWord</c>(s) 
        /// must be computed.</param>
        /// <param name="conf">The <c>Config</c> that must be used for the computation.</param>
        /// <returns></returns>
        private static List<PhonWord> GetPhonWords(List<Word> wordList, Config conf)
        {
            logger.ConditionalDebug("GetPhonWords");
            List<PhonWord> toReturn = new List<PhonWord>(wordList.Count);
            foreach (Word w in wordList)
                toReturn.Add(new PhonWord(w, conf));
            return toReturn;
        }

        /// <summary>
        /// Fills <c>formats</c> in order to format the phonèmes for the <see cref="PhonWord"/>(s) in
        /// <paramref name="pws"/>, using the
        /// <c>ColConfWin</c> defined by <paramref name="conf"/> and <paramref name="pct"/>.
        /// </summary>
        /// <param name="pws">The list of words to format.</param>
        /// <param name="conf">The <c>Config</c> to use for the fromatting.</param>
        /// <param name="pct">The <c>ColConfWin</c> within <c>conf</c> to use for the fromatting.</param>
        private void FormatPhons(List<PhonWord> pws, Config conf, PhonConfType pct)
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
        /// <see cref="FormattedTextEl"/>(s) to <see cref="formats"/>.
        /// </summary>
        /// <param name="pws">The list of <see cref="PhonWord"/>(s) to format.</param>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        private void FormatSyls(List<PhonWord> pws, Config conf)
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
