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

    public class TheText
        /*
         * The class TheText represents the text that is analysed and whose formatting is adapted by the 
         * analysis algorithm.
         * 
         * It is not supposed to be intiantiated directly. However, for test reasons, we do not declare it as abstract.
         * 
         * Memebers:
         *      string S --> containing the text.
         *      CharFormatting[] Formats --> an array of Charformats for each character in S.
         *      
         *      static ColorConfig Colors --> the color and formatting definition to use when coloring the text.
         */
    

    {
        public string S { get; private set; } //the text to colorize
        public List<FormattedTextEl> Formats { get; private set; }
        // list of the elemnts whose formatting must changed

        private List<Word> words;
        private List<PhonWord> phonWords;
        private Config theConf;

        public static void Init()
        {
            AutomAutomat.InitAutomat();
            PhonInW.Init();
            SylInW.Init();
            Config.Init();
        }

        public static TheText NewTestTheText(string s)
            // retourne un nouvel objet "TheText" avec une config par défaut
        {
            Config c = new Config();
            return new TheText(s, c);
        }

        public TheText(string s, Config inConf)
        {
            Debug.Assert(s != null);
            this.S = s;
            Formats = new List<FormattedTextEl>((s.Length * 3)/4);
            phonWords = null;
            theConf = inConf;
            words = null;
        }

        public override string ToString()
        {
            return S;
        }

        public List<Word> GetWords()
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

                    // Apostrophes: On considère comm apostrophes, les caractères ' ou ’ placé après une ou deux lettres.
                    // Cela couvre les formes élidées de le, que, ce, te... Y en  a-t-il d'autres?
                    // Il peut y avaoir confusion avec le guillemet simple. Tant pis!
                    if ((match.Length <= 2) && (end + 1 < S.Length) && ((S[end + 1] == '\'') || (S[end + 1] == '’')))
                    {
                        end++;
                    }
                    Word w = new Word(this, beg, end);
                    words.Add(w);
                }
            }
            return words;
        }

        public List<PhonWord> GetPhonWords()
            // public for test reasons
        {
            if (phonWords == null)
            {
                List<Word> theWords = GetWords();
                phonWords = new List<PhonWord>(theWords.Count);
                foreach (Word w in theWords)
                    phonWords.Add(new PhonWord(w));
            }
            return phonWords;
        }

        public Config GetConfig() => theConf;

        public void ColorizePhons(PhonConfType pct)
        {
            List<PhonWord> pws = GetPhonWords();
            foreach (PhonWord pw in pws)
                pw.ColorPhons(pct);
            ApplyFormatting();
        }

        public void MarkLetters()
        {
            for (int i = 0; i < S.Length; i++)
            {
                CharFormatting cf = theConf.pBDQ.GetCfForPBDQLetter(S[i]);
                if (cf != null)
                    Formats.Add(new FormattedTextEl(this, i, i, cf));
            }
            ApplyFormatting();
        }

        public void MarkSyls()
        {
            theConf.sylConf.ResetCounter();
            List<PhonWord> pws = GetPhonWords();
            foreach (PhonWord pw in pws)
                pw.ComputeAndColorSyls();
            ApplyFormatting();
        }

        public void MarkWords()
        {
            theConf.sylConf.ResetCounter();
            List<Word> theWords = GetWords();
            foreach (Word w in theWords)
                w.PutColor();
            ApplyFormatting();
        }

        public void MarkMuettes()
        {
            ColorizePhons(PhonConfType.muettes);
            ApplyFormatting();
        }

        public void MarkVoyCons()
        {
            theConf.sylConf.ResetCounter();
            CharFormatting voyCF = theConf.sylConf.NextCF();
            CharFormatting consCF = theConf.sylConf.NextCF();
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
            ApplyFormatting();
        }

        public void MarkNoir() 
        {
            CFForceBlack cfFB = new CFForceBlack();
            Formats.Add(new FormattedTextEl(this, 0, S.Length-1, cfFB));
            ApplyFormatting();
        } 

        protected virtual void SetChars(FormattedTextEl fte) { }
        // Formatte les caractères identifiés par cte au format voulu
        // N'est pas "abstract" pour simplifier le test.

        protected void ApplyFormatting() {
            foreach (FormattedTextEl fte in Formats)
                SetChars(fte);
        }
    }
}
