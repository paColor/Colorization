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
    /// The idea is that the inheriting class is created with a <c>string</c> and a <c>Config</c> (see the constructor
    /// <see cref="TheText(string, Config)"/>). Once this is done, 
    /// different methods make it possible to apply some kind of formatting to the text. This formatting is applied
    /// to the inheriting class by calling the method <c>SetChars</c> see <see cref="SetChars(FormattedTextEl)"/>.
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
    /// </remarks>
    public class TheText
    {
        /// <summary>
        /// The text that was entered when the object was constructed. Cannot be null.
        /// </summary>
        public string S { get; private set; }

        /// <summary>
        /// List of the elements whose formatting must be changed. The result of the operation. 
        /// </summary>
        public List<FormattedTextEl> Formats { get; private set; }


        private List<Word> words; // List of the words in TheText (computed when needed)
        private List<PhonWord> phonWords; // The corresponding PhonWords (computed when needed).
        private Config theConf; // the Config to apply to TheText. Defined at creation time.

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

        /// <summary>
        /// For test purposes: creates a <c>TheText</c> with the passed <c>string</c> and a default <c>Config</c>.
        /// </summary>
        /// <param name="s">The text that must be handled.</param>
        /// <param name="conf">The <c>Config</c> that will be applied for <c>TheText</c>.</param>
        /// <returns>A new <c>TheText</c>.</returns>
        public static TheText NewTestTheText(string s, out Config conf)
            // retourne un nouvel objet "TheText" avec une config par défaut
        {
            conf = new Config();
            return new TheText(s, conf);
        }

        /// <summary>
        /// For test purposes: creates a <c>TheText</c> with the passed <c>string</c> and a default <c>Config</c>.
        /// </summary>
        /// <param name="s">The text that must be handled.</param>
        /// <returns>A new <c>TheText</c>.</returns>
        public static TheText NewTestTheText(string s)
        // retourne un nouvel objet "TheText" avec une config par défaut
        {
            Config conf = new Config();
            return new TheText(s, conf);
        }

        /// <summary>
        /// Creates a <c>TheText</c> with the passed <c>string</c> and the passed <c>Config</c>. The <c>Config</c>
        /// will be used when applying formattings. 
        /// </summary>
        /// <param name="s">The text that will be worked on. Cannot be null.</param>
        /// <param name="inConf">The <c>Config</c> that will be used when applying formats to the text.</param>
        public TheText(string s, Config inConf)
        {
            Debug.Assert(s != null);
            this.S = s;
            Formats = new List<FormattedTextEl>((s.Length * 3)/4);
            phonWords = null;
            theConf = inConf;
            words = null;
        }

        /// <summary>
        /// Returns the text.
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
        /// <remarks>Is not needed by a normal consumer of the class and should only be used fror testing.</remarks>
        /// <returns>List of <c>PhonWords</c> contained in the text.</returns>
        public List<PhonWord> GetPhonWords()
        // public for test reasons
        {
            return GetPhonWords(theConf);
        }

        /// <summary>
        /// Returns the list of <c>PhonWords</c> contained in the text.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to use for the detection of the "phonèmes".</param>
        /// <returns>List of <c>PhonWords</c> contained in the text.</returns>
        private List<PhonWord> GetPhonWords(Config conf)
            // public for test reasons
        {
            if (phonWords == null)
            {
                List<Word> theWords = GetWords();
                phonWords = new List<PhonWord>(theWords.Count);
                foreach (Word w in theWords)
                    phonWords.Add(new PhonWord(w, conf));
            }
            return phonWords;
        }

        /// <summary>
        /// Applies the formattings defined in the <c>ColConfWin</c> identified by <paramref name="pct"/> to the 
        /// "phonèmes" in the text. I.e. fills <c>Formats</c> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="pct">Identifies the <c>ColConfWin</c> (see <see cref="ColorLib.ColConfWin"/>) that msut
        /// be used when coloring the "phonèmes".</param>
        public void ColorizePhons(PhonConfType pct)
        {
            ColorizePhons(theConf, pct);
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
            List<PhonWord> pws = GetPhonWords(conf);
            foreach (PhonWord pw in pws)
                pw.ColorPhons(conf, pct);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the letters in the text, according to the <see cref="PBDQConfig"/> attached to the <see cref="Config"/>
        /// passed at creation time. I.e. fills <see cref="Formats"/>and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkLetters()
        {
            MarkLetters(theConf);
        }

        /// <summary>
        /// Colors the letters in the text, according to the <see cref="PBDQConfig"/> <c>conf</c>,
        ///i.e. fills <see cref="Formats"/>and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> that must be used for marking the letters.</param>
        public void MarkLetters(Config conf)
        {
            for (int i = 0; i < S.Length; i++)
            {
                CharFormatting cf = conf.pBDQ.GetCfForPBDQLetter(S[i]);
                if (cf != null)
                    Formats.Add(new FormattedTextEl(this, i, i, cf));
            }
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "syllabes" in the text, according to the <see cref="SylConfig"/> attached to the <see cref="Config"/>
        /// passed at creation time. I.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkSyls()
        {
            MarkSyls(theConf);
        }

        /// <summary>
        /// Colors the "syllabes" in the text, according to the <see cref="SylConfig"/> attached to <c>conf</c>,
        /// i.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the "syllabes".</param>
        public void MarkSyls(Config conf)
        {
            conf.sylConf.ResetCounter();
            List<PhonWord> pws = GetPhonWords(conf);
            foreach (PhonWord pw in pws)
                pw.ComputeAndColorSyls(conf);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "words" in the text, according to the <see cref="SylConfig"/> attached to the <see cref="Config"/>
        /// passed at creation time. I.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkWords()
        {
            MarkWords(theConf);
        }

        /// <summary>
        /// Colors the "words" in the text, according to the <see cref="SylConfig"/> attached to <c>conf</c>, 
        /// i.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to be used for marking the words.</param>
        public void MarkWords(Config conf)
        {
            conf.sylConf.ResetCounter();
            List<Word> theWords = GetWords();
            foreach (Word w in theWords)
                w.PutColor(conf);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "unspoken letters" (muettes :-)) in the text, according to the <see cref="ColConfWin"/> 
        /// corresponding to <c>pct == PhonConfType.muettes</c> attached to the <see cref="Config"/> 
        /// passed at creation time. I.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkMuettes()
        {
            MarkMuettes(theConf);
        }

        /// <summary>
        /// Colors the "unspoken letters" (muettes :-)) in the text, according to the <see cref="ColConfWin"/> 
        /// corresponding to <c>pct == PhonConfType.muettes</c> attached to <c>conf</c>, i.e. fills
        /// <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        /// <param name="conf"></param>
        public void MarkMuettes(Config conf)
        {
            ColorizePhons(conf, PhonConfType.muettes);
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Colors the "voyelles" and "consonnes" in the text, according to the alternate colors defined in the <see cref="SylConfig"/>
        /// attached to the <see cref="Config"/> passed at creation time. I.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkVoyCons()
        {
            MarkVoyCons(theConf);
        }


        /// <summary>
        /// Colors the "voyelles" and "consonnes" in the text, according to the alternate colors defined in the <see cref="SylConfig"/>
        /// attached to <c>conf</c>, i.e. fills <see cref="Formats"/> and makes sure that 
        /// <see cref="SetChars(FormattedTextEl)"/> is called for each <c>FormattedTextEl</c>.
        /// </summary>
        public void MarkVoyCons(Config conf)
        {
            conf.sylConf.ResetCounter();
            CharFormatting voyCF = conf.sylConf.NextCF();
            CharFormatting consCF = conf.sylConf.NextCF();
            int start, end; 
            int i = 0;
            string smallCapsS = S.ToLower(BaseConfig.cultF);
            while (i< smallCapsS.Length)
            {
                if (TextEl.EstVoyelle(smallCapsS[i]))
                {
                    start = i;
                    i++;
                    while ((i < smallCapsS.Length) && (TextEl.EstVoyelle(smallCapsS[i])))
                        i++;
                    end = i - 1;
                    Formats.Add(new FormattedTextEl(this, start, end, voyCF));
                }
                else if (TextEl.EstConsonne(smallCapsS[i]))
                {
                    start = i;
                    i++;
                    while ((i < smallCapsS.Length) && (TextEl.EstConsonne(smallCapsS[i])))
                        i++;
                    end = i - 1;
                    Formats.Add(new FormattedTextEl(this, start, end, consCF));
                }
                else
                    i++;
            }
            ApplyFormatting(conf);
        }

        /// <summary>
        /// Forces the text to black color and no bold, italic, underline, ... formatting.
        /// </summary>
        public void MarkNoir() 
        {
            MarkNoir(theConf);
        }

        /// <summary>
        /// Forces the text to black color and no bold, italic, underline, ... formatting.
        /// </summary>
        /// <param name="conf">The <see cref="Config"/> to use for the formatting.</param>
        public void MarkNoir(Config conf)
        {
            CFForceBlack cfFB = new CFForceBlack();
            Formats.Add(new FormattedTextEl(this, 0, S.Length - 1, cfFB));
            ApplyFormatting(conf);
        }

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
            Debug.Assert(false);
        }
        
        protected void ApplyFormatting(Config conf) {
            foreach (FormattedTextEl fte in Formats)
                SetChars(fte, conf);
        }
    }
}
