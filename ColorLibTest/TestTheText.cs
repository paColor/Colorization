using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ColorLib;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    class TestTheText : TheText
    {
        public class FormattedChar
        {
            private static RGB black = new RGB(0, 0, 0);
            private static RGB red = new RGB(255, 0, 0);

            public char C { get; private set; }
            public CharFormatting cf { get; private set; }

            /// <summary>
            /// Creates a neutral <c>FormattedChar</c> , i.e. black, no other Formatting.
            /// </summary>
            /// <param name="inC">The character.</param>
            public FormattedChar(char inC)
            {
                C = inC;
                cf = new CharFormatting(false, false, false, false, false, black, false, red);
            }

            public void SetChar(CharFormatting theCF, Config inConf)
            {
                bool bold = cf.bold;
                bool italic = cf.italic;
                bool underline = cf.underline;
                bool caps = cf.caps;
                bool changeColor = cf.changeColor;
                RGB color = cf.color;
                bool changeHilight = cf.changeHilight;
                RGB hilightColor = cf.hilightColor;

                if (theCF.bold)
                    bold = true;
                else if (theCF.ForceNonBold(inConf))
                    bold = false;

                if (theCF.italic)
                    italic = true;
                else if (theCF.ForceNonItalic(inConf))
                    italic = false;

                if (theCF.underline)
                    underline = true;
                else if (theCF.ForceNonUnderline(inConf))
                    underline = false;

                if (theCF.caps)
                    caps = true;
                else if (theCF.ForceNonCaps(inConf))
                    caps = false;

                if (theCF.changeColor)
                {
                    color = theCF.color;
                    changeColor = true;
                }
                else if (theCF.ForceBlackColor(inConf))
                    color = ColConfWin.predefinedColors[(int)PredefCols.black];

                if (theCF.changeHilight)
                {
                    changeHilight = true;
                    hilightColor = theCF.hilightColor;
                }  
                else if (theCF.ForceHilightClear(inConf))
                {
                    changeHilight = false;
                }

                cf = new CharFormatting(bold, italic, underline, 
                    caps, changeColor, color, changeHilight, hilightColor);

            }
        }

        public List<FormattedChar> formattedText { get; private set; }

        private List<int> finsDeLigne;

        public TestTheText(string txt)
            :base(txt)
        {
            formattedText = new List<FormattedChar>(txt.Length);
            for (int i = 0; i < txt.Length; i++)
            {
                formattedText.Add(new FormattedChar(txt[i]));
                Assert.IsTrue(formattedText[i].C == txt[i]);
            }
        }

        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            for (int i = fte.First; i <= fte.Last; i++)
            {
                formattedText[i].SetChar(fte.cf, conf);
            }
        }

        protected override List<int> GetLastLinesPos()
        {
            if (finsDeLigne == null)
            {
                finsDeLigne = new List<int>();
                for (int i = 0; i < S.Length; i++)
                {
                    if (S[i] == '\r' || S[i] == '\v')
                        finsDeLigne.Add(i);
                }
            }
            return finsDeLigne;
        }

        public void AssertColor(int pos, RGB theCol)
        {
            Assert.IsTrue(pos >= 0 && pos < S.Length);
            Assert.IsTrue(formattedText[pos].cf.color == theCol);
        }

        public void AssertColor(int fromPos, int toPos, RGB theCol)
        {
            Assert.IsTrue(toPos >= fromPos);
            for (int i = fromPos; i <= toPos; i++)
            {
                AssertColor(i, theCol);
            }
        }

        /// <summary>
        /// retourne un string contenant tous le caractères ayant la couleur donnée.
        /// </summary>
        /// <param name="theCol">La couleur cherchée. </param>
        /// <returns>Un <c>string</c> contenant tous les caractères formattés avec la couleur
        /// <c>theCol</c>.</returns>
        public string GetCharsInCol(RGB theCol)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < formattedText.Count; i++)
            {
                if (formattedText[i].cf.color == theCol)
                {
                    sb.Append(formattedText[i].C);
                }
            }
            return sb.ToString();
        }
    }
}
