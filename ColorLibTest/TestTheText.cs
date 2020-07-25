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
        // ****************************************************************************************
        // *                                       STATIC                                         *
        // ****************************************************************************************

        public readonly static RGB black = new RGB(0, 0, 0);
        public readonly static RGB red = new RGB(255, 0, 0);
        public readonly static RGB green = new RGB(0, 255, 0);
        public readonly static RGB blue = new RGB(0, 0, 255);
        public readonly static RGB white = new RGB(255, 255, 255);

        public readonly static RGB hilight1 = new RGB(255, 255, 000); // wdYellow
        public readonly static RGB hilight2 = new RGB(000, 255, 000); // wdBrightGreen
        public readonly static RGB hilight3 = new RGB(000, 255, 255); // wdTurquoise
        public readonly static RGB hilight4 = new RGB(255, 000, 255); // wdPink
        public readonly static RGB hilight5 = new RGB(000, 000, 255); // wdBlue

        public readonly static RGB col00 = new RGB(010, 010, 010); // on évite la couleure noire
        public readonly static RGB col01 = new RGB(010, 010, 060);
        public readonly static RGB col02 = new RGB(010, 010, 120);
        public readonly static RGB col03 = new RGB(010, 010, 180);
        public readonly static RGB col04 = new RGB(010, 010, 240);
        public readonly static RGB col05 = new RGB(010, 060, 010);
        public readonly static RGB col06 = new RGB(010, 060, 060);
        public readonly static RGB col07 = new RGB(010, 060, 120);
        public readonly static RGB col08 = new RGB(010, 060, 180);
        public readonly static RGB col09 = new RGB(010, 060, 240);
        public readonly static RGB col10 = new RGB(010, 120, 010);
        public readonly static RGB col11 = new RGB(010, 120, 060);
        public readonly static RGB col12 = new RGB(010, 120, 120);
        public readonly static RGB col13 = new RGB(010, 120, 180);
        public readonly static RGB col14 = new RGB(010, 120, 240);
        public readonly static RGB col15 = new RGB(010, 180, 010);
        public readonly static RGB col16 = new RGB(010, 180, 060);
        public readonly static RGB col17 = new RGB(010, 180, 120);
        public readonly static RGB col18 = new RGB(010, 180, 180);
        public readonly static RGB col19 = new RGB(010, 180, 240);
        public readonly static RGB col20 = new RGB(010, 240, 010);
        public readonly static RGB col21 = new RGB(010, 240, 060);
        public readonly static RGB col22 = new RGB(010, 240, 120);
        public readonly static RGB col23 = new RGB(010, 240, 180);
        public readonly static RGB col24 = new RGB(010, 240, 240);
        public readonly static RGB col25 = new RGB(060, 010, 010);
        public readonly static RGB col26 = new RGB(060, 010, 060);
        public readonly static RGB col27 = new RGB(060, 010, 120);
        public readonly static RGB col28 = new RGB(060, 010, 180);
        public readonly static RGB col29 = new RGB(060, 010, 240);
        public readonly static RGB col30 = new RGB(060, 060, 010);
        public readonly static RGB col31 = new RGB(060, 060, 060);
        public readonly static RGB col32 = new RGB(060, 060, 120);
        public readonly static RGB col33 = new RGB(060, 060, 180);
        public readonly static RGB col34 = new RGB(060, 060, 240);
        public readonly static RGB col35 = new RGB(060, 120, 010);
        public readonly static RGB col36 = new RGB(060, 120, 060);
        public readonly static RGB col37 = new RGB(060, 120, 120);
        public readonly static RGB col38 = new RGB(060, 120, 180);
        public readonly static RGB col39 = new RGB(060, 120, 240);
        public readonly static RGB col40 = new RGB(060, 180, 010);
        public readonly static RGB col41 = new RGB(060, 180, 060);
        public readonly static RGB col42 = new RGB(060, 180, 120);
        public readonly static RGB col43 = new RGB(060, 180, 180);
        public readonly static RGB col44 = new RGB(060, 180, 240);
        public readonly static RGB col45 = new RGB(060, 240, 010);
        public readonly static RGB col46 = new RGB(060, 240, 060);
        public readonly static RGB col47 = new RGB(060, 240, 120);
        public readonly static RGB col48 = new RGB(060, 240, 180);
        public readonly static RGB col49 = new RGB(060, 240, 240);

        public readonly static RGB[] fixCols =
            new RGB[] { col00, col01, col02, col03, col04, col05, col06, col07, col08, col09,
                        col10, col11, col12, col13, col14, col15, col16, col17, col18, col19,
                        col20, col21, col22, col23, col24, col25, col26, col27, col28, col29,
                        col30, col31, col32, col33, col34, col35, col36, col37, col38, col39,
                        col40, col41, col42, col43, col44, col45, col46, col47, col48, col49};

        /// <summary>
        /// Liste de CFs différents
        /// </summary>
        public static readonly CharFormatting[] fixCFs = new CharFormatting[50]
        {
            //  CharFormatting(bold,  itali, under, caps,  chCol,        chHig       )
            new CharFormatting(false, false, false, false, true,  col00, false, black),
            new CharFormatting(false, false, true,  false, true,  col01, false, black),
            new CharFormatting(false, true,  false, false, true,  col02, false, black),
            new CharFormatting(false, true,  true,  false, true,  col03, false, black),
            new CharFormatting(true,  false, false, false, true,  col04, false, black),
            new CharFormatting(true,  false, true,  false, true,  col05, false, black),
            new CharFormatting(true,  true,  false, false, true,  col06, false, black),
            new CharFormatting(true,  true,  true,  false, true,  col07, false, black),

            new CharFormatting(false, false, false, false, true,  col08, true, hilight1),
            new CharFormatting(false, false, true,  false, true,  col09, true, hilight2),
            new CharFormatting(false, true,  false, false, true,  col10, true, hilight3),
            new CharFormatting(false, true,  true,  false, true,  col11, true, hilight4),
            new CharFormatting(true,  false, false, false, true,  col12, true, hilight5),
            new CharFormatting(true,  false, true,  false, true,  col13, true, hilight1),
            new CharFormatting(true,  true,  false, false, true,  col14, true, hilight2),
            new CharFormatting(true,  true,  true,  false, true,  col15, true, hilight3),

            new CharFormatting(false, false, false, false, true,  col16, false, black),
            new CharFormatting(false, false, false, false, true,  col17, false, black),
            new CharFormatting(false, false, false, false, true,  col18, false, black),
            new CharFormatting(false, false, false, false, true,  col19, false, black),
            new CharFormatting(false, false, false, false, true,  col20, false, black),
            new CharFormatting(false, false, false, false, true,  col21, false, black),
            new CharFormatting(false, false, false, false, true,  col22, false, black),
            new CharFormatting(false, false, false, false, true,  col23, false, black),

            new CharFormatting(false, false, false, false, true,  col24, true,  hilight1),
            new CharFormatting(false, false, false, false, true,  col25, true,  hilight2),
            new CharFormatting(false, false, false, false, true,  col26, true,  hilight3),
            new CharFormatting(false, false, false, false, true,  col27, true,  hilight4),
            new CharFormatting(false, false, false, false, true,  col28, true,  hilight5),
            new CharFormatting(false, false, false, false, true,  col29, false, black),
            new CharFormatting(false, false, false, false, true,  col30, false, black),
            new CharFormatting(false, false, false, false, true,  col31, false, black),

            new CharFormatting(false, false, false, false, true,  col32, false, black),
            new CharFormatting(false, false, true,  false, true,  col33, false, black),
            new CharFormatting(false, true,  false, false, true,  col34, false, black),
            new CharFormatting(false, true,  true,  false, true,  col35, false, black),
            new CharFormatting(true,  false, false, false, true,  col36, false, black),
            new CharFormatting(true,  false, true,  false, true,  col37, false, black),
            new CharFormatting(true,  true,  false, false, true,  col38, false, black),
            new CharFormatting(true,  true,  true,  false, true,  col39, false, black),

            new CharFormatting(false, false, false, false, true,  col40, false, black),
            new CharFormatting(false, false, false, false, true,  col41, false, black),
            new CharFormatting(false, false, false, false, true,  col42, false, black),
            new CharFormatting(false, false, false, false, true,  col43, false, black),
            new CharFormatting(false, false, false, false, true,  col44, false, black),
            new CharFormatting(false, false, false, false, true,  col45, false, black),
            new CharFormatting(false, false, false, false, true,  col46, false, black),
            new CharFormatting(false, false, false, false, true,  col47, false, black),
            new CharFormatting(false, false, false, false, true,  col48, false, black),
            new CharFormatting(false, false, false, false, true,  col49, false, black)

        };

        /// <summary>
        /// Liste des <see cref="CharFormatting"/> dans <c>fixCFs</c> qui ont le flag <c>bold</c>.
        /// </summary>
        public static List<int> Bolds { get; private set; } = new List<int>() 
            { 4, 5, 6, 7, 12, 13, 14, 15, 36, 37, 38, 39 };

        /// <summary>
        /// Liste des <see cref="CharFormatting"/> dans <c>fixCFs</c> qui ont le flag <c>italic</c>.
        /// </summary>
        public static List<int> Italics { get; private set; } = new List<int>()
            { 2, 3, 6, 7, 10, 11, 14, 15, 34, 35, 38, 39 };

        /// <summary>
        /// Liste des <see cref="CharFormatting"/> dans <c>fixCFs</c> qui ont le flag <c>underline</c>.
        /// </summary>
        public static List<int> Underlines { get; private set; } = new List<int>()
            { 1, 3, 5, 7, 9, 11, 13, 15, 33, 35, 37, 39 };

        /// <summary>
        /// Liste des <see cref="CharFormatting"/> dans <c>fixCFs</c> qui ont le flag <c>changeHilight</c>.
        /// </summary>
        public static List<int> Hilighted { get; private set; } = new List<int>()
            { 8, 9, 10, 11, 12, 13, 14, 15, 24, 25, 26, 27, 28 };

        public readonly static CharFormatting redCF = new CharFormatting(red);
        public readonly static CharFormatting blueCF = new CharFormatting(blue);
        public readonly static CharFormatting greenCF = new CharFormatting(green);
        public readonly static CharFormatting whiteCF = new CharFormatting(white);
        public readonly static CharFormatting blackCF = new CharFormatting(black);

        /// <summary>
        /// Longueur de ligne pour les strings représentant le texte. Au delà, --> à la ligne
        /// </summary>
        private const int LineLength = 100;

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

        // ****************************************************************************************
        // *                             public class FormattedChar                               *
        // ****************************************************************************************

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

        // ****************************************************************************************
        // *                                   public Members                                     *
        // ****************************************************************************************

        public List<FormattedChar> formattedText { get; private set; }

        // ****************************************************************************************
        // *                                   private Members                                    *
        // ****************************************************************************************

        private List<int> finsDeLigne;

        // ****************************************************************************************
        // *                                   TheText Methods                                    *
        // ****************************************************************************************

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

        // ****************************************************************************************
        // *                                     TEST SUPPORT                                     *
        // ****************************************************************************************

        // --------------------- On Position -----------------------------

        public void AssertColor(int pos, RGB theCol)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(theCol, formattedText[pos].cf.color);
        }

        public void AssertNotColor(int pos, RGB theCol)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreNotEqual(theCol, formattedText[pos].cf.color);
        }

        public void AssertBold(int pos, bool val)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(val, formattedText[pos].cf.bold);
        }

        public void AssertItalic(int pos, bool val)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(val, formattedText[pos].cf.italic);
        }

        public void AssertUnderline(int pos, bool val)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(val, formattedText[pos].cf.underline);
        }

        public void AssertChangeColor(int pos, bool val)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(val, formattedText[pos].cf.changeColor);
        }

        public void AssertChangeHilight(int pos, bool val)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(val, formattedText[pos].cf.changeHilight);
        }

        public void AssertHilightColor(int pos, RGB theCol)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            Assert.AreEqual(theCol, formattedText[pos].cf.hilightColor);
        }

        public CharFormatting GetCF (int pos)
        {
            Assert.IsTrue(pos >= 0 && pos < formattedText.Count);
            return formattedText[pos].cf;
        }

        // --------------------- On Range -----------------------------

        public void AssertColor(int fromPos, int len, RGB theCol)
        {
            for (int i = fromPos; i <= fromPos + len - 1; i++)
            {
                AssertColor(i, theCol);
            }
        }

        public void AssertCF(int pos, CharFormatting theCF)
        {
            Assert.IsTrue(pos >= 0 && pos < S.Length);
            Assert.AreEqual(theCF, formattedText[pos].cf);
        }

        public void AssertCF(int fromPos, int len, CharFormatting theCF)
        {
            for (int i = fromPos; i <= fromPos + len - 1; i++)
            {
                AssertCF(i, theCF);
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
            conf.sylConf.SetSylButtonCF(0, blueCF);
            conf.sylConf.SetSylButtonCF(1, redCF);
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
