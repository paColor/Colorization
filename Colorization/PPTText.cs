/********************************************************************************
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
using Microsoft.Office.Interop.PowerPoint;
using System.Windows.Forms;
using ColorLib;
using ColorizationControls;
using System.Diagnostics;

namespace Colorization
{
    public class PPTText : TheText
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Initialize()
        {
            List<string> errMsgs = new List<string>();
            TheText.Init(errMsgs);
            foreach(string errMsg in errMsgs)
                MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ConfigControl.Init();
        }

        private TextRange txtRange;
        private bool withinTable;
        private float tableIncrX;
        private float tableIncrY;

        /// <summary>
        /// Liste des positions de fin de ligne (dans S) à retourner dans <see cref="GetLastLinesPos"/>.
        /// </summary>
        private List<int> finsDeLigne;

        /// <summary>
        /// Crée le PPTText.
        /// </summary>
        /// <param name="inTxtRange">Le <c>TextRange</c> sur lequel on travaille.</param>
        /// <param name="inWithinTable">Indique si le texte se situe à l'intérieur d'une table.
        /// </param>
        /// <param name="incrX">L'incrément en x pour trouver la position des lettres du texte. 
        /// N'est différent de zéro que pour le cas où <c>inWithinTable</c> est <c>true</c>."/>
        /// </param>
        /// <param name="incrY">L'incrément en y pour trouver la position des lettres du texte. 
        /// N'est différent de zéro que pour le cas où <c>inWithinTable</c> est <c>true</c>."/>
        /// </param>
        public PPTText(TextRange inTxtRange, bool inWithinTable = false, float incrX = 0.0f, float incrY = 0.0f )
            :base(inTxtRange.Text)
        {
            txtRange = inTxtRange;
            withinTable = inWithinTable;
            tableIncrX = incrX;
            tableIncrY = incrY;
            finsDeLigne = null;
        }

        /// <summary>
        /// Applique le formatage <paramref name="cf"/> aux caractères dans le <see cref="TextRange"/>
        /// <paramref name="tRange"/> en utilisant la <see cref="Config"/> <paramref name="inConf"/>.
        /// </summary>
        /// <param name="cf">Le <see cref="CharFormatting"/> à appliquer. Attention: peut être 
        /// <c>null</c>.</param>
        /// <param name="tRange">Le <see cref="TextRange"/> à formater.</param>
        /// <param name="inConf">La <see cref="Config"/> à utiliser le cas échéant.</param>
        private void ApplyCFToRange(CharFormatting cf, TextRange tRange, Config inConf)
        {
            if (cf != null)
            {
                if (cf.bold)
                    tRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoTrue;
                else if (cf.ForceNonBold(inConf))
                    tRange.Font.Bold = Microsoft.Office.Core.MsoTriState.msoFalse;

                if (cf.italic)
                    tRange.Font.Italic = Microsoft.Office.Core.MsoTriState.msoTrue;
                else if (cf.ForceNonItalic(inConf))
                    tRange.Font.Italic = Microsoft.Office.Core.MsoTriState.msoFalse;

                if (cf.underline)
                    tRange.Font.Underline = Microsoft.Office.Core.MsoTriState.msoTrue;
                else if (cf.ForceNonUnderline(inConf))
                    tRange.Font.Underline = Microsoft.Office.Core.MsoTriState.msoFalse;

                //if (cf.caps) // capitalize
                //    tRange.Text = tRange.Text.ToUpper(BaseConfig.cultF);
                //else if (Config.ActiveConf().unsetBeh.CbuVal(Ucbx.caps))
                //    tRange.Text = tRange.Text.ToLower(BaseConfig.cultF);

                if (cf.changeColor) // set new color
                    tRange.Font.Color.RGB = cf.color;
                else if (cf.ForceBlackColor(inConf))
                    tRange.Font.Color.RGB = ColConfWin.predefinedColors[(int)PredefCol.black];

                if (cf.drawArc)
                {
                    float x0 = tRange.BoundLeft;
                    float y0 = tRange.BoundTop;
                    float fontHeight = tRange.Font.Size;
                    // Chercher l'interligne
                    if (tRange.ParagraphFormat.LineRuleWithin == Microsoft.Office.Core.MsoTriState.msoTrue)
                    {
                        // il s'agit de lignes
                        y0 += (fontHeight * tRange.ParagraphFormat.SpaceWithin);
                    }
                    else if (tRange.ParagraphFormat.LineRuleWithin == Microsoft.Office.Core.MsoTriState.msoFalse)
                    {
                        // il s'agit de points...
                        y0 += fontHeight;
                        float delta = tRange.ParagraphFormat.SpaceWithin - fontHeight;
                        if (delta > 0.0f)
                        {
                            y0 += (float)Math.Pow(Math.Abs(delta), 1.7d) / (1.2f * fontHeight); // empirique
                        }
                        else
                        {
                            y0 -= 4 + (Math.Abs(delta) / 2); // empirique
                        }
                    }
                    else
                    {
                        // ça ne peut pas arriver (en théorie :-))
                        logger.Error("Interligne ni en points ni en lignes...");
                    }

                    y0 += inConf.arcConf.Decalage;
                    float h = 3.0f;
                    float w = tRange.BoundWidth; // width
                    float x1 = x0 + w;

                    if (withinTable)
                    {
                        x0 += tableIncrX;
                        x1 += tableIncrX;
                        y0 += tableIncrY;
                    }

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
                    float[,] thePoints0 = new float[4, 2]
                       {
                            { x0, y0 },
                            { x0 + (((float)(100 - inConf.arcConf.Ecartement) / 200.0f) * w), y0 + h },
                            { x0 + (((float)(100 + inConf.arcConf.Ecartement) / 200.0f) * w), y0 + h },
                            { x1, y0 },
                       };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional

                    TextFrame tFrame = tRange.Parent;
                    if (tFrame != null)
                    {
                        Shape tShape = tFrame.Parent;
                        if (tShape != null)
                        {
                            Debug.Assert(tShape is Shape);
                            Slide tSlide = tShape.Parent;
                            if (tSlide != null)
                            {
                                Debug.Assert(tSlide is Slide);
                                Shape s = tSlide.Shapes.AddCurve(thePoints0);
                                s.Line.ForeColor.RGB = cf.arcColor;
                                s.Line.Weight = inConf.arcConf.Epaisseur;
                                s.Name = "arc";
                            } 
                            else
                            {
                                logger.Error("Le Shape n'a pas de parent... (?)");
                            }
                            
                        }
                        else
                        {
                            logger.Error("Le TextFrame n'a pas de parent... (?)");
                        }
                        
                    }
                    else
                    {
                        logger.Error("Le TextRange n'a pas de parent... (?)");
                    }
                    
                }
            }
            else
            {
                logger.Error("ApplyCFToRange with cf == null");
                Debug.Assert(false);
            }
        }

        /// <summary>
        /// Applique le formatage voulu au <see cref="FormattedTextEl"/> sur l'affichage.
        /// </summary>
        /// <param name="fte">Le <see cref="FormattedTextEl"/> qui doit être formaté.</param>
        /// <param name="conf">La <see cref="Config"/> à prendre en compte pour l'application du formatage.</param>
        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            TextRange theChars = txtRange.Characters(fte.First + 1, fte.Last - fte.First + 1);
            ApplyCFToRange(fte.cf, theChars, conf);
        }

        /// <summary>
        /// Retourne la liste des positions des derniers caractères de chaque ligne (dans S).
        /// </summary>
        /// <remarks>Utilise <c>finDeLignes</c> comme cache.</remarks>
        /// <returns>La liste des positions des derniers caractères de chaque ligne (dans S)</returns>
        protected override List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            if (finsDeLigne == null)
            {
                logger.ConditionalTrace("txtRange.Start: {0}, txtRange.Length: {1}", txtRange.Start, txtRange.Length);
                finsDeLigne = new List<int>(7);
                int i = 1;
                int startColoredLine = 0;
                TextRange theLine = txtRange.Lines(i);
                while ((theLine.Start > startColoredLine) && (theLine.Start + theLine.Length) <= (txtRange.Start + txtRange.Length))
                {
                    logger.ConditionalTrace("i: {0}, theLine - Start: {1}, Length: {2}", i, theLine.Start, theLine.Length);
                    if (theLine.Length > 0)
                    {
                        int endFormattedLine = theLine.Start + theLine.Length - 1;
                        finsDeLigne.Add(endFormattedLine - txtRange.Start);
                    }
                    startColoredLine = theLine.Start;
                    i++;
                    theLine = txtRange.Lines(i);
                }
            }
            logger.ConditionalTrace("EXIT GetLastLinesPos");
            return finsDeLigne;
        }

    } // class PPTText
} // namespace Colorization
