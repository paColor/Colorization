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
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Interop.PowerPoint;
using ColorizationControls;
using ColorLib;
using System.Diagnostics;

namespace Colorization
{
    public partial class Ribbon1
    {
        private delegate void ActOnPPTText(PPTText t);
        private delegate void ActOnRange(TextRange range, ActOnPPTText act);

        private static void ColorizePhons(PPTText t) => t.ColorizePhons(PhonConfType.phonemes);
        private static void MarkLetters(PPTText t) => t.MarkLetters();
        private static void MarkSyls(PPTText t) => t.MarkSyls();
        private static void MarkWords(PPTText t) => t.MarkWords();
        private static void MarkMuettes(PPTText t) => t.MarkMuettes();
        private static void MarkNoir(PPTText t) => t.MarkNoir();
        private static void MarkVoyCons(PPTText t) => t.MarkVoyCons();

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            logger.ConditionalTrace("Init");
            ConfigControl.markSelLetters = Ribbon1.ColorSelectedLetters;
            ConfigControl.colorizeAllSelPhons = Ribbon1.ColorizeSelectedPhons;
            ConfigControl.colSylSelLetters = Ribbon1.ColorSelectedSyls;
            ConfigControl.colMotsSelLetters = Ribbon1.ColorSelectedWords;
            ConfigControl.colVoyConsSelText = Ribbon1.ColorSelectedVoyCons;
            ConfigControl.colLignesSelText = Ribbon1.ColorSelectedLignes;
            ConfigControl.colNoirSelText = Ribbon1.ColorSelectedNoir;
            ConfigControl.colMuettesSelText = Ribbon1.ColorSelectedMuettes;
            ConfigControl.colDuoSelText = Ribbon1.ColorSelectedDuo;
        }

        public static void ColorizeSelectedPhons()
        {
            logger.ConditionalTrace("ColorizeSelectedPhons");
            ActOnSelectedText(ColorizePhons, ActoOnRangePPTText);
        }

        public static void ColorSelectedLetters()
        {
            logger.ConditionalTrace("ColorSelectedLetters");
            ActOnSelectedText(MarkLetters, ActoOnRangePPTText);
        }

        public static void ColorSelectedSyls()
        {
            logger.ConditionalTrace("ColorSelectedSyls");
            ActOnSelectedText(MarkSyls, ActoOnRangePPTText);
        }

        public static void ColorSelectedWords()
        {
            logger.ConditionalTrace("ColorSelectedWords");
            ActOnSelectedText(MarkWords, ActoOnRangePPTText);
        }

        public static void ColorSelectedMuettes()
        {
            logger.ConditionalTrace("ColorSelectedMuettes");
            ActOnSelectedText(MarkMuettes, ActoOnRangePPTText);
        }

        public static void ColorSelectedNoir()
        {
            logger.ConditionalTrace("ColorSelectedNoir");
            ActOnSelectedText(MarkNoir, ActoOnRangePPTText);
        }

        public static void ColorSelectedVoyCons()
        {
            logger.ConditionalTrace("ColorSelectedVoyCons");
            ActOnSelectedText(MarkVoyCons, ActoOnRangePPTText);
        }

        public static void ColorSelectedLignes()
        {
            logger.ConditionalTrace("ColorSelectedLignes");
            ActOnSelectedText(null, MarkLignes);
        }

        public static void ColorSelectedDuo()
        {
            logger.ConditionalTrace("ColorSelectedDuo");
            ActOnSelectedText(null, MarkLignes);
        }

        private static void MarkLignes(TextRange tRange, ActOnPPTText act)
        {
            DocumentWindow activeWin = ColorizationPPT.thisAddIn.Application.ActiveWindow;
            Config theConf = Config.GetConfigFor(activeWin, activeWin.Presentation);
            theConf.sylConf.ResetCounter();
            int i = 1;
            int startColoredLine = 0;
            TextRange theLine = tRange.Lines(i);
            while ((theLine.Start > startColoredLine) && (theLine.Start + theLine.Length) <= (tRange.Start + tRange.Length))
            {
                // color line
                PPTText.ApplyCFToRange(theConf.sylConf.NextCF(), theLine, theConf);
                startColoredLine = theLine.Start;
                i++;
                theLine = tRange.Lines(i);
            }
        }

        private static void ActoOnRangePPTText (TextRange tRange, ActOnPPTText actOn)
        {
            DocumentWindow activeWin = ColorizationPPT.thisAddIn.Application.ActiveWindow;
            actOn (new PPTText(tRange, Config.GetConfigFor(activeWin, activeWin.Presentation)));
        }

        private static void ActOnShape(Shape sh, ActOnPPTText act, ActOnRange aor, int nrObjSelected)
        {
            Debug.Assert(sh != null);
            if(sh.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue){
                if (sh.TextFrame.HasText == Microsoft.Office.Core.MsoTriState.msoTrue)
                    aor(sh.TextFrame.TextRange, act);
            } else if (sh.Type == Microsoft.Office.Core.MsoShapeType.msoGroup)
                foreach (Shape descSh in sh.GroupItems)
                    ActOnShape(descSh, act, aor, nrObjSelected);
            if (sh.HasTable == Microsoft.Office.Core.MsoTriState.msoTrue)
                foreach (Row r in sh.Table.Rows)
                    foreach (Cell c in r.Cells)
                        if ((nrObjSelected > 1) || (c.Selected))
                            ActOnShape(c.Shape, act, aor, nrObjSelected);
            // il y a visiblement un problème avec la sélection de tableaux. Les cellules ne sont pas sélectionnées
            // si plusieurs objects sont sélectionnés dont le tableau...
            // rendons donc le comportement dépendant du nombre d'objets dans la sélection... Y a-t-il un piège?
            // Powerpoint lui-même n'utilise pas ce truc. Hypothèse il s'agit d'un bug de Powerpoint dans la version
            // que j'utilise. Le comportement pourrait donc changer. Avec un peu de chance, le code devrait continuer 
            // à se comporter correctement.
        } // private void ActOnShape
   
        private static void ActOnSelectedText(ActOnPPTText act, ActOnRange aor)
        {
            if (ColorizationPPT.thisAddIn.Application.Presentations.Count > 0)
            {
                ColorizationPPT.thisAddIn.Application.StartNewUndoEntry();
                var sel = ColorizationPPT.thisAddIn.Application.ActiveWindow.Selection;
                if (sel.Type == PpSelectionType.ppSelectionText)
                {
                    aor(ColorizationPPT.thisAddIn.Application.ActiveWindow.Selection.TextRange, act);
                }
                else if (sel.Type == PpSelectionType.ppSelectionShapes)
                {
                    // bool textFound = false;
                    foreach (Shape sh in sel.ShapeRange)
                    {
                        ActOnShape(sh, act, aor, sel.ShapeRange.Count);
                    } // foreach
                } // else no text selected
                else if (sel.Type == PpSelectionType.ppSelectionSlides)
                {
                    foreach (Slide s in sel.SlideRange)
                    {
                        foreach (Shape sh in s.Shapes)
                        {
                            ActOnShape(sh, act, aor, s.Shapes.Count);
                        }
                    }
                }
            }
        } // void ColorizeSelectedPhons()

        private static void PresentationClosed(Presentation inPres) => ConfigPane.DocClosed(inPres);

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            ColorizationPPT.thisAddIn.Application.PresentationClose
                += new EApplication_PresentationCloseEventHandler(PresentationClosed);
            ColorizationPPT.thisAddIn.Application.WindowSelectionChange
                += new EApplication_WindowSelectionChangeEventHandler(SelChanged_Event);
        }

        private void SelChanged_Event(Selection sel)
        {
            EnableButtons(sel.Type != PpSelectionType.ppSelectionNone);
        }

        private void EnableButtons(bool enable)
        {
            if (btnBPDQ.Enabled != enable)
            {
                btnBPDQ.Enabled = enable;
                btnLignes.Enabled = enable;
                btnMots.Enabled = enable;
                btnMuettes.Enabled = enable;
                btnNoir.Enabled = enable;
                btnSyl.Enabled = enable;
                btnPhon.Enabled = enable;
                btnVoyCons.Enabled = enable;
                btnDuo.Enabled = enable;
            }
        }

        private void grpDialogLauncher_Click(object sender, RibbonControlEventArgs e)
        {
            if (ColorizationPPT.thisAddIn.Application.Presentations.Count > 0)
            {
                DocumentWindow activeWin = ColorizationPPT.thisAddIn.Application.ActiveWindow;
                ConfigPane.MakePaneVisibleInWin(activeWin, activeWin.Presentation, ColorizationPPT.thisAddIn.CustomTaskPanes,
                    typeof(ColorizationPPT).Assembly.GetName().Version.ToString());
            }
        }

        private void btnColoriser_Click(object sender, RibbonControlEventArgs e)
        {
            ColorizeSelectedPhons();
        } // private void btnColoriser_Click

        private void btnBDPQ_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedLetters();
        }

        private void btnSyl_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedSyls();
        }

        private void btnMots_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedWords();
        }

        private void btnMuettes_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedMuettes();
        }

        private void btnNoir_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedNoir();
        }

        private void btnVoyCons_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedVoyCons();
        }

        private void btnLignes_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedLignes();
        }

        private void btnDuo_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedDuo();
        }
    }
}
