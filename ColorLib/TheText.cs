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
            private List<Word> wordList1;
            private List<Word> wordList2;
            private List<PhonWord> phonWordList1;
            private List<PhonWord> phonWordList2;
            private DuoConfig.Alternance alt;
            private DuoConfig.ColorisFunction colF;

            public DuoCache()
            {
                wordList1 = null;
                wordList2 = null;
                phonWordList1 = null;
                phonWordList2 = null;
                alt = DuoConfig.Alternance.undefined;
                colF = DuoConfig.ColorisFunction.undefined;
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
                if ((wordList1 == null) || (alt != dConf.alternance)) 
                {
                    wordList1 = new List<Word>((wL.Count / 2) + 1);
                    wordList2 = new List<Word>((wL.Count / 2) + 1);
                    alt = dConf.alternance;
                    switch (alt)
                    {
                        case DuoConfig.Alternance.mots:
                            for (int i = 0; i < wL.Count; i++)
                            {
                                if (i % 2 == 1)
                                {
                                    // odd
                                    wordList1.Add(wL[i]);
                                }
                                else
                                {
                                    // even
                                    wordList2.Add(wL[i]);
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
                                    wordList1.Add(wL[wordIndex]);
                                    wordIndex++;
                                }
                                lineIndex++;
                                if (lineIndex < eolPos.Count)
                                {
                                    while ((wordIndex < wL.Count) && (wL[wordIndex].Last <= eolPos[lineIndex]))
                                    {
                                        wordList2.Add(wL[wordIndex]);
                                        wordIndex++;
                                    }
                                    lineIndex++;
                                }
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
                wL1 = wordList1;
                wL2 = wordList2;
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
        /// <summary>
        /// List of the elements whose formatting must be changed. The result of the operation. 
        /// </summary>
        private List<FormattedTextEl> formats;
        private List<Word> words; // List of the words in TheText (computed when needed)
        private List<PhonWord> phonWords; // The corresponding PhonWords (computed when needed).
        private DuoCache dc;

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
            Debug.Assert(txt != null);
            this.S = txt;
            formats = null;
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
            if (conf != null)
            { 
                ClearFormats();
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
            if (conf != null)
            {
                ClearFormats();
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
            if (conf != null)
            {
                ClearFormats();
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
            if (conf != null)
            {
                ClearFormats();
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
            ClearFormats();
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
            if (conf != null)
            {
                ClearFormats();
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
            if (conf != null)
            {
                ClearFormats();
                CFForceBlack cfFB = new CFForceBlack();
                formats.Add(new FormattedTextEl(this, 0, S.Length - 1, cfFB));
                ApplyFormatting(conf);
            }
            else
            {
                logger.Error("conf == null. Impossible de mettre le texte en noir sans une configuration vallable.");
                throw new ArgumentException("conf == null. Impossible de mettre le texte en noir sans une configuration valable.");
            }
        }

        /// <summary>
        /// Formats the text to the duo formatting defined in <c>conf</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> defining how the "duo" formatting should
        /// be applied.</param>
        public void MarkDuo(Config conf)
        {
            if (conf != null)
            {
                ClearFormats();
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
                    case DuoConfig.ColorisFunction.muettes:
                    case DuoConfig.ColorisFunction.phonemes:
                    case DuoConfig.ColorisFunction.syllabes:
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
        }

        public void AddFTE(FormattedTextEl fte) => formats.Add(fte);

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
            throw new NotImplementedException("SetChars doit être implémenté par une classe descendante de TheText.");
        }

        /// <summary>
        /// Returns the list of the positions of the last characters of each line. Must be overridden
        /// by the inheriting classes. Default behaviour is to consider the text as one single line.
        /// </summary>
        /// <returns>The list of the zero based positions of the last characters of each line. Empty
        /// list if the text is empty.</returns>
        protected virtual List<int> GetLastLinesPos()
        {
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
            foreach (FormattedTextEl fte in formats)
                SetChars(fte, conf);
        }

        private void ClearFormats()
        {
            if (formats == null)
            {
                formats = new List<FormattedTextEl>((S.Length * 3) / 4);
            }
            else
            {
                formats.Clear();
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
        private List<PhonWord> GetPhonWords(List<Word> wordList, Config conf)
        {
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
            Debug.Assert(last < S.Length);
            for (int i = first; i <= last; i++)
            {
                CharFormatting cf = conf.pBDQ.GetCfForPBDQLetter(S[i]);
                if (cf != null)
                    formats.Add(new FormattedTextEl(this, i, i, cf));
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
                    formats.Add(new FormattedTextEl(this, start, end, voyCF));
                }
                else if (TextEl.EstConsonne(smallCapsS[i]))
                {
                    start = i;
                    i++;
                    while ((i < smallCapsS.Length) && (TextEl.EstConsonne(smallCapsS[i])))
                        i++;
                    end = i - 1;
                    formats.Add(new FormattedTextEl(this, start, end, consCF));
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
            foreach (Word w in wL)
            {
                FormatVoyCons(w.First, w.Last, conf);
            }
        }
    }
}
