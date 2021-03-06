﻿/********************************************************************************
 *  Copyright 2020 - 2021, Pierre-Alain Etique                                  *
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using ColorLib;
using ColorizationControls;
using System.Windows.Forms;
using NLog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ColorizationWord
{
    public class MSWText : TheText
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static RGB[] hilightColors = new RGB[HilightForm.nrColors]
        {
            new RGB(255,255,0),    // wdYellow
            new RGB(0,255,0),      // wdBrightGreen 
            new RGB(0,255,255),    // wdTurquoise 
            new RGB(255,0,255),    // wdPink 
            new RGB(0,0,255),      // wdBlue 
            new RGB(255,0,0),      // wdRed 
            new RGB(255,255,255),  // wdWhite 
            new RGB(0,0,128),      // wdDarkBlue 
            new RGB(0,128,128),    // wdTeal
            new RGB(0,128,0),      // wdGreen 
            new RGB(128,0,128),    // wdViolet 
            new RGB(128,0,0),      // wdDarkRed 
            new RGB(128,128,0),    // wdDarkYellow 
            new RGB(128,128,128),  // wdGray50 
            new RGB(192,192,192),  // wdGray25 
            new RGB(0,0,0),        // wdBlack
        };

        private static Dictionary<RGB, WdColorIndex> mapRGBtoColI = new Dictionary<RGB, WdColorIndex>(HilightForm.nrColors)
        {
            { hilightColors[0] , WdColorIndex.wdYellow },
            { hilightColors[1] , WdColorIndex.wdBrightGreen },
            { hilightColors[2] , WdColorIndex.wdTurquoise },
            { hilightColors[3] , WdColorIndex.wdPink },
            { hilightColors[4] , WdColorIndex.wdBlue },
            { hilightColors[5] , WdColorIndex.wdRed },
            { hilightColors[6] , WdColorIndex.wdWhite },
            { hilightColors[7] , WdColorIndex.wdDarkBlue },
            { hilightColors[8] , WdColorIndex.wdTeal },
            { hilightColors[9] , WdColorIndex.wdGreen },
            { hilightColors[10] , WdColorIndex.wdViolet },
            { hilightColors[11] , WdColorIndex.wdDarkRed },
            { hilightColors[12] , WdColorIndex.wdDarkYellow },
            { hilightColors[13] , WdColorIndex.wdGray50 },
            { hilightColors[14] , WdColorIndex.wdGray25 },
            { hilightColors[15] , WdColorIndex.wdBlack },
        };

        /// <summary>
        /// pour le stockage intermédiaire de la liste, 
        /// lors de la construction du MSWText. Assez bizarre. Je me demande s'il y a une façon plus
        /// élégante de faire ça.
        /// </summary>
        private static List<MultipleCharInfo> tmpMultipleChars;


        private struct MultipleCharInfo
        {
            public int pos; // position dans le texte traité par TheText, cad dans S.
            public int nrChars; // nombre de caractères du caractère spécial.
        }

        private Range range;
        private int rgStart;
        private Range rgeWork; // je ne sais pas quel est le coût en terme de performance de l'accès 
                               // à range.Duplicate et range.Start. Dans le doute ne le faisons qu'une fois...
        /// <summary>
        /// Liste des positions de fin de ligne (dans S) à retourner dans <see cref="GetLastLinesPos"/>.
        /// </summary>
        private List<int> finDeLignes;

        /// <summary>
        /// La liste des charactères spéciaux dans <c>range</c> qui peuvent perturber le comptage
        /// des charactères...
        /// </summary>
        private List<MultipleCharInfo> multipleChars;

        /// <summary>
        /// Nombre d'arcs dessinés jusqu'à maintenant. A condition qu'ils soient dessinés 
        /// séquentiellement, le texte est décalé de ce nombre de caractères...
        /// </summary>
        private int nrArcs;
        private int lastPage;

        public static void Initialize()
        {
            logger.ConditionalDebug("Initialize");
            List<string> errMsgs = new List<string>();
            TheText.Init(errMsgs);
            foreach (string errMsg in errMsgs)
                MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ConfigControl.Init();
            HilightForm.hiliColors = hilightColors;
        }

        private static void ErrorMsgACTR(CharFormatting cf, Range toR, string msg, Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Ouups, désolé. Un problème vient de se produire avec la mise en ");
            sb.Append(msg);
            sb.AppendLine(".");
            sb.Append("Si vous utilisez un ancien format de fichier (par ex. \'.doc\"), il est ");
            sb.AppendLine("possible que la fonctionalité ne soit pas disponible.");
            sb.Append("Texte: \'");
            sb.Append(toR.Text);
            sb.Append("\' cf: ");
            sb.AppendLine(cf.ToString());
            sb.Append("N'hésitez pas à nous ");
            sb.AppendLine("envoyer une description de votre problème à info@colorization.ch.");
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);
            logger.Error(sb.ToString());
            MessageBox.Show(sb.ToString(), ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.Assert(false);
        }

        /// <summary>
        /// Applique le formatage <paramref name="cf"/> aux caractères dans le <see cref="Range"/>
        /// <paramref name="toR"/> en utilisant la <see cref="Config"/> <paramref name="inConf"/>.
        /// </summary>
        /// <param name="cf">Le <see cref="CharFormatting"/> à appliquer. Attention: ne devrait pas
        /// l'être, mais il y a eu des cas où <paramref name="cf"/> était <c>null</c>.</param>
        /// <param name="toR">Le <see cref="Range"/> à formater.</param>
        /// <param name="inConf">La <see cref="Config"/> à utiliser le cas échéant.</param>
        private void ApplyCFToRange(CharFormatting cf, Range toR, Config inConf)
        {
            logger.ConditionalTrace("ApplyCFToRange");
            // 24.01.2021: J'ai reçu une copie d'écran d'une erreur qui provenait visiblement de 
            // l'incapacité de Word à appliquer une couleur. J'ai donc décidé d'intercepter de
            // telles erreurs directement ici.
            // 31.01.2021: Entre temps, j'ai une explication possible et j'ai changé la façon de
            // mettre en couleur. Je laisse quand même les try / catch. 
            if (cf != null)
            {
                // **************************** GRAS ********************************
                try
                {
                    if (cf.bold)
                        toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
                    else if (cf.ForceNonBold(inConf))
                        toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoFalse;
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "gras", e);
                }

                // **************************** ITALIQUE ********************************
                try
                {
                    if (cf.italic)
                        toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
                    else if (cf.ForceNonItalic(inConf))
                        toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoFalse;
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "italique", e);
                }

                // **************************** SOULIGNÉ ********************************
                try
                {
                    if (cf.underline)
                        toR.Underline = WdUnderline.wdUnderlineSingle;
                    else if (cf.ForceNonUnderline(inConf))
                        toR.Underline = WdUnderline.wdUnderlineNone;
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "souligné", e);
                }

                // if (cte.cf.caps) // capitalize
                // TBD

                // **************************** COULEUR ********************************
                try
                {
                    if (cf.changeColor) // set new 
                    {
                        toR.Font.Color = (WdColor)(int)cf.color;
                        // On peut aussi coloriser en utilisant Font.Fill:
                        // toR.Font.Fill.ForeColor.RGB = cf.color;
                        // L'avantage c'est qu'il s'agit d'un remplissage des caractères et qu'on peut 
                        // par exemple faire des fondus. L'inconvénient c'est que ça plante avec les anciens
                        // fichiers .doc et que c'est plus lent.
                        // J'ai d'abord fait une distinction entre les deux modes, en interceptant
                        // l'exception, mais ça ne vaut pas la peine tant qu'on n'offre pas plus de 
                        // possibilités de formater.

                        
                    }
                    else if (cf.ForceBlackColor(inConf))
                    {
                        toR.Font.Color = (WdColor)(int)ColConfWin.predefinedColors[(int)PredefCol.black];
                    }
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "mise en couleur", e);
                }

                // **************************** SURLIGNAGE ********************************
                try
                {
                    if (cf.changeHilight)
                    {
                        WdColorIndex wdCi;
                        if (mapRGBtoColI.TryGetValue(cf.hilightColor, out wdCi))
                            toR.HighlightColorIndex = wdCi;
                    }
                    else if (cf.ForceHilightClear(inConf))
                    {
                        // il y a un bug sur les espaces, on met donc d'abord du blanc partout
                        // avant d'effacer. 
                        toR.HighlightColorIndex = WdColorIndex.wdWhite;
                        toR.HighlightColorIndex = WdColorIndex.wdNoHighlight;
                    }
                        
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "surlignage", e);
                }

                // **************************** ARC ********************************
                try
                {
                    if (cf.drawArc)
                    {
                        // Let's get rid of any special char before and after the effective text.
                        // We could possibly do that for each case, but arcs seems to be the only
                        // one requesting it, in the other cases, setting a color to an undisplayed
                        // character does not really matter.

                        Range ran = toR.Duplicate;
                        MatchCollection matches = TheText.rxWords.Matches(ran.Text);
                        if (matches.Count > 0)
                        {
                            // beg and end are zero based in ran.Text
                            // beg is the first character in a word, end - 1 is the last character
                            // in a word.
                            string texte = ran.Text;
                            int beg = matches[0].Index;
                            Match m = matches[matches.Count - 1];
                            int end = m.Index + m.Length; //char after the match so that end-beg == length

                            // Il peut semble-t-il arriver que des caractères spéciaux se trouvent au début de Text et
                            // qu'ils soient comptés différemment pour Start que pour Text... Probablement qqch à voir
                            // avec UTF-8 vs. UTF-16 ou un truc du genre...

                            // On corrige donc beg pour qu'il corresponde bien au premier caractère voulu. end doit
                            // aussi être corrigé, le cas échéant. 

                            string wishedChar = texte.Substring(beg, 1);
                            int i = 0;
                            foreach(Range c in ran.Characters)
                            {
                                if (c.Text == wishedChar)
                                {
                                    break;
                                }
                                i++;
                            }
                            Debug.Assert(i < ran.Characters.Count);
                            int delta = beg - i;
                            beg = i;
                            end = end - delta;

                            ran.SetRange(ran.Start + beg, ran.Start + end);

                            int pageNr = ran.Information[WdInformation.wdActiveEndPageNumber];
                            logger.ConditionalTrace("Page Number {0}", pageNr);
                            if (lastPage == 0)
                            {
                                lastPage = pageNr;
                            }
                            else if (pageNr > lastPage)
                            {
                                lastPage = pageNr;
                                ran.Select();
                            }

                            _ = ran.Information[WdInformation.wdHorizontalPositionRelativeToPage];
                            // Ne me demandez pas pourquoi il faut le faire deux fois... Mais seul le
                            // 2e appel donne le bon résultat... PAE 06.02.2021

                            float x0 = ran.Information[WdInformation.wdHorizontalPositionRelativeToPage];
                            float y0 = ran.Information[WdInformation.wdVerticalPositionRelativeToPage];
                            float fontHeight = ran.Font.Size;
                            y0 += fontHeight;
                            y0 += inConf.arcConf.Decalage;
                            float lineheight = ran.ParagraphFormat.LineSpacing;
                            float h = (inConf.arcConf.Hauteur / 100.0f) *
                                (float)Math.Sqrt(20.0f * (Math.Max(1, lineheight - fontHeight)));


                            Range endPosition = ran.Duplicate;
                            endPosition.Collapse(WdCollapseDirection.wdCollapseEnd);
                            float x1 = endPosition.Information[WdInformation.wdHorizontalPositionRelativeToPage];
                            float w = x1 - x0; // width

                            float[,] thePoints0 = new float[4, 2]
                            {
                            { x0, y0 },
                            { x0 + (((float)(100 - inConf.arcConf.Ecartement) / 200.0f) * w), y0 + h },
                            { x0 + (((float)(100 + inConf.arcConf.Ecartement) / 200.0f) * w), y0 + h },
                            { x1, y0 },
                            };

                            Shape s =
                                ColorizationMSW.thisAddIn.Application.ActiveDocument.Shapes.AddCurve(
                                thePoints0);
                            s.Line.ForeColor.RGB = cf.arcColor;
                            s.Line.Weight = inConf.arcConf.Epaisseur;
                            s.Name = "arc";

                            if (s.Anchor.Start <= ran.End)
                            {
                                nrArcs++;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "arcs", e);
                }

                // **************************** EEFACER ARCS ********************************
                try
                {
                    if (cf.removeArcs)
                    {
                        List<Shape> toRemoveShapes = new List<Shape>(toR.ShapeRange.Count);
                        foreach (Shape s in toR.ShapeRange)
                        {
                            if (s.Name == "arc") {
                                toRemoveShapes.Add(s);
                            }
                        }
                        int i = 0;
                        foreach (Shape s in toRemoveShapes)
                        {
                            if (i%30 == 0)
                            {
                                ProgressNotifier.thePN.InProgress((int)(((float)i / (float)toRemoveShapes.Count) * 100.0f));
                            }
                            i++;
                            s.Delete();
                        }
                    }
                   
                }
                catch (Exception e)
                {
                    ErrorMsgACTR(cf, toR, "effecer arcs", e);
                }

            }
            else
            {
                logger.Error("ApplyCFToRange with cf == null");
                Debug.Assert(false);
            }
        }

        public MSWText(Range rge)
            : base(GetStringFor(rge, out tmpMultipleChars))
        {
            multipleChars = tmpMultipleChars;
            this.range = rge;
            rgStart = rge.Start;
            rgeWork = range.Duplicate;
            finDeLignes = null;
            lastPage = 0;

#if DEBUG
            // Debugging help
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("this.ToString(): {0}", this.ToString()));
            sb.AppendLine(String.Format("rgeSize (end - start): {0}", rge.End - rge.Start));
            sb.AppendLine(String.Format("range.Text.Length: {0}", rge.Text.Length));
            sb.AppendLine(String.Format("this.S.Length: {0}", this.ToString().Length));
            logger.ConditionalTrace(sb.ToString());
#endif
        }

        /// <summary>
        /// Ajoute un espace entre chaque mot.
        /// </summary>
        /// <remarks>
        /// A priori, la config n'est pas utilisée, mais ça permet de réutiliser le même "pattern"
        /// </remarks>
        /// <param name="conf">La <see cref="Config"/> à utiliser.</param>
        public void AddSpace(Config conf)
        {
            logger.ConditionalDebug("AddSpace");
            bool previousIsSpace = false;
            foreach (Range r in range.Characters)
            {
                if (r.Text == " ")
                {
                    if (!previousIsSpace)
                    {
                        r.InsertBefore(" ");
                    }
                    previousIsSpace = true;
                }
                else
                {
                    previousIsSpace = false;
                }
            }
        }

        /// <summary>
        /// Enlève un espace entre chaque mot du texte sélectionné. Ne fait rien s'il n'y a
        /// qu'un seul espace.
        /// </summary>
        /// <remarks>
        /// A priori, la config n'est pas utilisée, mais ça permet de réutiliser le même "pattern"
        /// </remarks>
        /// <param name="conf">La <see cref="Config"/> à utiliser.</param>
        public void ShrinkSpace(Config conf)
        {
            logger.ConditionalDebug("ShrinkSpace");
            int countSpace = 0;
            Range lastSpace = null;
            foreach (Range r in range.Characters)
            {
                if (r.Text == " ")
                {
                    countSpace++;
                    lastSpace = r;
                }
                else
                {
                    if (countSpace > 1)
                    {
                        lastSpace.Delete();
                    }
                    countSpace = 0;
                    lastSpace = null;
                }
            }
            // Si le dernièr caractère est un espace...
            if (countSpace > 1)
            {
                lastSpace.Delete();
            }
        }

        /// <summary>
        /// Applique le formatage voulu au <see cref="FormattedTextEl"/> sur l'affichage.
        /// </summary>
        /// <exception cref="ArgumentNullException"> si <paramref name="fte"/> est <c>null</c>.
        /// </exception>
        /// <param name="fte">Le <see cref="FormattedTextEl"/> qui doit être formaté.</param>
        /// <param name="conf">La <see cref="Config"/> à prendre en compte pour l'application du
        /// formatage.</param>
        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            if (fte == null)
            {
                logger.Error("fte est null.");
                throw new ArgumentNullException(nameof(fte));
            }
            rgeWork.SetRange(rgStart + nrArcs + fte.First, 
                rgStart + nrArcs + fte.Last + 1); // End doit pointer sur le caractère qui suit le range...
            if (fte.ToString() != empty || rgeWork.Text == empty)
                ApplyCFToRange(fte.cf, rgeWork, conf);
        }

        /// <summary>
        /// Retourne la liste des positions des derniers caractères de chaque ligne (dans S).
        /// </summary>
        /// <remarks>Utilise <c>finDeLignes</c> comme cache.</remarks>
        /// <returns>La liste des positions des derniers caractères de chaque ligne (dans S)</returns>
        protected override List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            if (finDeLignes == null)
            {
                finDeLignes = new List<int>(7); // imaginons 7 lignes. Aucun moyen de savoir ce qu'il en est vraiment...
                if (ColorizationMSW.thisAddIn.Application.ActiveWindow.View.Type == WdViewType.wdPrintView)
                {
                    // Cherchons tous les Rectangles de la feneêtre active et travaillons sur toutes les lignes
                    // qui se trouvent dans la sélection
                    foreach (Page p in ColorizationMSW.thisAddIn.Application.ActiveWindow.ActivePane.Pages)
                    {
                        foreach (Rectangle r in p.Rectangles)
                        {
                            try
                            {
                                if (r.RectangleType == WdRectangleType.wdTextRectangle)
                                {
                                    foreach (Line l in r.Lines)
                                    {
                                        Range lineRange = l.Range;
                                        if (lineRange.InRange(range))
                                        {
                                            // linerange est dans la région sélectionnée.
                                            // linerange.End est toujours sur le caractère qui suit le range
                                            finDeLignes.Add(GetSPosForRangePos(lineRange.End - 1));
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.AppendLine("Il semblerait qu'il y a ait un nouveau bug dans Word et ");
                                sb.AppendLine("qu'il est parfois impossioble d'accéder à r.RectangleType ");
                                sb.AppendLine("sans qu'il y ait un moyen de tester avant l'appel si ce ");
                                sb.AppendLine("sera possible ou non... ");
                                sb.AppendLine("Voici donc du contrôle de flux par try / catch.");
                                sb.AppendLine("Tout ce qu'il faudrait éviter - Mais je ne vois pas comment faire autrement...");
                                sb.AppendLine(e.Message);
                                sb.AppendLine(e.StackTrace);
                                logger.Error(sb.ToString());
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("La mise en couleur de lignes ne fonctionne que dans le mode \'Page\'.",
                        ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            return finDeLignes;
        }

        /// <summary>
        /// Retourne le string correspondant au texte dans le range.
        /// </summary>
        /// <param name="rge">Le <c>Range</c> à transformer en <c>string</c>.</param>
        /// <param name="mChL">Out: La liste des cas spéciaux rencontrés.</param>
        /// <returns></returns>
        private static string GetStringFor(Range rge, out List<MultipleCharInfo> mChL)
            // dans le cas où il y a un objet attaché au texte, il y a un caractère pour marquer l'ancrage.
            // ce caractère n'est pas dans "Text". On le construit donc ici avec un caractère "vide" à la 
            // place du caractère spécial.
        {         
            mChL = new List<MultipleCharInfo>(5); // pourquoi pas 5? ça doit suffire pour la plupart des cas...
            string chText;
            Range itR = rge.Duplicate;
            int length = rge.End - rge.Start;
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < rge.End - rge.Start; i++)
            {
                int start = rge.Start + i;
                itR.SetRange(start, start + 1);
                chText = itR.Text;
                if (String.IsNullOrEmpty(chText))
                {
                    chText = empty;
                }
                else if (chText.Length > 1)
                {
                    chText = empty; // empty comes from TheText
                    MultipleCharInfo mci;
                    mci.pos = i;
                    mci.nrChars = chText.Length;
                    mChL.Add(mci);
                }
                sb.Append(chText);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Calcule la position dans <c>S</c> de la position <paramref name="rangePos"/> dans <c>range</c>.
        /// Condition: <c>rangePos</c> se situe dans <c>range</c>.
        /// </summary>
        /// <param name="rangePos">La position dans <c>range</c>.</param>
        /// <returns>La position </returns>
        private int GetSPosForRangePos(int rangePos)
        {
            logger.ConditionalDebug("GetSPosForRangePos \'{0}\'", rangePos);
            int toReturn = rangePos - range.Start;
            Debug.Assert(toReturn >= 0);
            int i = 0; // itérateur sur les caractères spéciaux.
            int thePos = 0; // La position dans S où se trouve le caractère spécial.
            while ((i < multipleChars.Count) && (thePos < toReturn))
            {
                thePos = multipleChars[i].pos;
                if (thePos < toReturn)
                {
                    toReturn -= (multipleChars[i].nrChars - 1);
                }
                i++;
            }
            return toReturn;
        } 
    }
}
