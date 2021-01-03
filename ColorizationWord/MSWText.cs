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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using ColorLib;
using ColorizationControls;
using System.Windows.Forms;
using NLog;
using System.Diagnostics;

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
        /// pour le stockage intermédiaire de la liste, 
        /// lors de la construction du MSWText. Assez bizarre. Je me demande s'il y a une façon plus
        /// élégante de faire ça.
        /// </summary>
        private static List<MultipleCharInfo> tmpMultipleChars;

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

        /// <summary>
        /// Applique le formatage <paramref name="cf"/> aux caractères dans le <see cref="Range"/>
        /// <paramref name="toR"/> en utilisant la <see cref="Config"/> <paramref name="inConf"/>.
        /// </summary>
        /// <param name="cf">Le <see cref="CharFormatting"/> à appliquer. Attention: ne devrait pas
        /// l'être, mais il y a eu des cas où <paramref name="cf"/> était <c>null</c>.</param>
        /// <param name="toR">Le <see cref="Range"/> à formater.</param>
        /// <param name="inConf">La <see cref="Config"/> à utiliser le cas échéant.</param>
        private static void ApplyCFToRange(CharFormatting cf, Range toR, Config inConf)
        {
            if (cf != null)
            {
                if (cf.bold)
                    toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
                else if (cf.ForceNonBold(inConf))
                    toR.Bold = (int)Microsoft.Office.Core.MsoTriState.msoFalse;

                if (cf.italic)
                    toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoTrue;
                else if (cf.ForceNonItalic(inConf))
                    toR.Italic = (int)Microsoft.Office.Core.MsoTriState.msoFalse;

                if (cf.underline)
                    toR.Underline = WdUnderline.wdUnderlineSingle;
                else if (cf.ForceNonUnderline(inConf))
                    toR.Underline = WdUnderline.wdUnderlineNone;

                // if (cte.cf.caps) // capitalize
                // TBD

                if (cf.changeColor) // set new color
                    toR.Font.Fill.ForeColor.RGB = cf.color;
                else if (cf.ForceBlackColor(inConf))
                    toR.Font.Fill.ForeColor.RGB = ColConfWin.predefinedColors[(int)PredefCol.black];

                if (cf.changeHilight)
                {
                    WdColorIndex wdCi;
                    if (mapRGBtoColI.TryGetValue(cf.hilightColor, out wdCi))
                        toR.HighlightColorIndex = wdCi;
                }
                else if (cf.ForceHilightClear(inConf))
                    toR.HighlightColorIndex = WdColorIndex.wdNoHighlight;
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
        /// Applique le formatage voulu au <see cref="FormattedTextEl"/> sur l'affichage.
        /// </summary>
        /// <param name="fte">Le <see cref="FormattedTextEl"/> qui doit être formaté.</param>
        /// <param name="conf">La <see cref="Config"/> à prendre en compte pour l'application du formatage.</param>
        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            rgeWork.SetRange(rgStart + fte.First, rgStart + fte.Last + 1); // End pointe sur le caractère qui suit le range...
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
            // dans le cas où il a un objet attaché au texte, il y a un caractère pour marquer l'ancrage.
            // ce caractère n'est pas dans "Text". On le construit donc ici avec un caractère "vide" à la 
            // place du caractère spécial.
        {         
            const string empty = " ";
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
                    chText = empty;
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
        /// Calcule la position dasn <c>S</c> de la position <paramref name="rangePos"/> dans <c>range</c>.
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
