using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColorLib;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    class TestTheText : TheText
    {
        public readonly static RGB black = new RGB(0, 0, 0);
        public readonly static RGB red = new RGB(255, 0, 0);
        public readonly static RGB green = new RGB(0, 255, 0);
        public readonly static RGB blue = new RGB(0, 0, 255);
        public readonly static RGB white = new RGB(255, 255, 255);

        public readonly static CharFormatting redCF = new CharFormatting(red);
        public readonly static CharFormatting blueCF = new CharFormatting(blue);
        public readonly static CharFormatting greenCF = new CharFormatting(green);
        public readonly static CharFormatting whiteCF = new CharFormatting(white);

        /// <summary>
        /// Longuer de ligne pour les strings représentant le texte. Au delà, --> à la ligne
        /// </summary>
        private const int LineLength = 100;

        public class FormattedChar
        {
            
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

        /// <summary>
        /// Extraits les mots des deux strings et effectue une Assert.AreEqual sur chaque mot
        /// avec ^la même position dans les deux trings.
        /// </summary>
        /// <param name="expected">Mots attendus.</param>
        /// <param name="real">Mots effectifs.</param>
        public static void CompareWordByWord(string expected, string real)
        {
            Regex rx = new Regex(@"\b[\w-]+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matchesExpected = rx.Matches(expected);
            MatchCollection matchesReal = rx.Matches(real);
            Assert.AreEqual(matchesExpected.Count, matchesReal.Count);
            for (int i = 0; i < matchesExpected.Count; i++)
                Assert.AreEqual(matchesExpected[i].Value, matchesReal[i].Value);
        }

        /// <summary>
        /// retourne la liste de mots <paramref name="pws"/> sous forme de représentation par
        /// syllabes: espace entre les mots, tiret entre les syllabes. tous les 120 caractères
        /// environ, une nouvelle ligne est commencée. 
        /// </summary>
        /// <param name="pws">La liste de mots à représenter sous forme de syllabes.</param>
        /// <returns>Les mots de <paramref name="pws"/> en syllabes.</returns>
        public static string ToSyllabes(List<PhonWord> pws)
        {
            StringBuilder sb = new StringBuilder(pws.Count * 9);
            int lastNL = 0;
            for (int i = 0; i < pws.Count; i++)
            {
                sb.Append(pws[i].Syllabes());
                if (i < pws.Count - 1)
                {
                    sb.Append(" ");
                }
                if (pws[i].Last - lastNL > LineLength)
                {
                    lastNL = pws[i].Last;
                    sb.AppendLine();
                }
            }
            return sb.ToString();
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
        /// Excécute <c>MarkSyls</c> sur le texte avec la <c>Config passée</c>. Vérifie que le 
        /// résultat correspond au texte passé dans <c>syllabes</c>.
        /// </summary>
        /// <param name="conf">La config à utiliser.</param>
        /// <param name="syllabes">Un string correspondant au texte, où la ponctuation a été enlrvée,
        /// et les mots sont coupés en syllabes avec un tiret comme séparateur.
        /// Exemple: "Cha-que fois qu'on ti-rait de ce sol sou-ve-rain"</param>
        /// <remarks>
        /// <para>
        /// La configuration de couleurs est réinitialisée. Le marquage des muettes est
        /// désactivé. La méthode est donc utile pour vérifier que le marquage de couleurs identifie
        /// les bonnes syllabes. Elle ne permet pas de jouer avec les couleurs.
        /// </para>
        /// <para>
        /// Le mode de traitement des syllabes doit être défini par l'appelant.
        /// </para>
        /// </remarks>
        public void AssertSyls (Config conf, string syllabes)
        {
            for (int i = SylConfig.NrButtons - 1; i >= 0; i--)
                if (i > 1 && conf.sylConf.ButtonIsLastActive(i))
                    conf.sylConf.ClearButton(i);
            conf.sylConf.SylButtonModified(0, blueCF);
            conf.sylConf.SylButtonModified(1, redCF);
            conf.sylConf.marquerMuettes = false;

            MarkSyls(conf);

            List<PhonWord> pws = GetPhonWordList(conf, true);
            StringBuilder sb = new StringBuilder((int)(S.Length * 1.35f));
            RGB currentColor;
            RGB nextColor = blue;
            int lastNL = 0;
            for (int i = 0; i < pws.Count; i++)
            {
                AssertColor(pws[i].First, nextColor);
                currentColor = nextColor;
                for (int j = pws[i].First; j <= pws[i].Last; j++)
                {
                    if (formattedText[j].cf.color != currentColor)
                    {
                        // switch currrent color
                        currentColor = currentColor == red ? blue : red;
                        AssertColor(j, currentColor);
                        sb.Append("-");
                    }
                    sb.Append(formattedText[j].C);
                }
                if (i < pws.Count - 1)
                {
                    sb.Append(" ");
                }

                if (pws[i].Last - lastNL > LineLength)
                {
                    lastNL = pws[i].Last;
                    sb.AppendLine();
                }

                nextColor = currentColor == red ? blue : red;
            }
            CompareWordByWord(syllabes, sb.ToString());
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
