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
using Microsoft.Office.Tools;
using Microsoft.Office.Interop.Word;
using ColorLib;
using ColorizationControls;

namespace ColorizationWord
{
    public partial class WordRibbon
    {
        private delegate void ActOnMSWText(MSWText t);
        private delegate void ActOnRange(Range range, ActOnMSWText act);

        private static void DummyActOnMSWText(MSWText t) {}
        private static void MarkPhons(MSWText t) => t.ColorizePhons(PhonConfType.phonemes);
        private static void MarkLetters(MSWText t) => t.MarkLetters();
        private static void MarkSyls(MSWText t) => t.MarkSyls();
        private static void MarkWords(MSWText t) => t.MarkWords();
        private static void MarkMuettes(MSWText t) => t.MarkMuettes();
        private static void MarkNoir(MSWText t) => t.MarkNoir();
        private static void MarkVoyCons(MSWText t) => t.MarkVoyCons();

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Init()
        {
            logger.ConditionalTrace("Init");
            ConfigControl.markSelLetters = WordRibbon.ColorSelectedLetters;
            ConfigControl.colorizeAllSelPhons = WordRibbon.ColorSelectedPhons;
            ConfigControl.colSylSelLetters = WordRibbon.ColorSelectedSyls;
            ConfigControl.colMotsSelLetters = WordRibbon.ColorSelectedWords;
            ConfigControl.colLignesSelText = WordRibbon.ColorSelectedLignes;
            ConfigControl.colVoyConsSelText = WordRibbon.ColorSelectedVoyCons;
            ConfigControl.colNoirSelText = WordRibbon.ColorSelectedNoir;
        }

        public static void ColorSelectedPhons()
        {
            ActOnSelectedText(MarkPhons, "Phonèmes", ActoOnRangeMSWText);
        }

        public static void ColorSelectedLetters()
        {
            ActOnSelectedText(MarkLetters, "bpdq", ActoOnRangeMSWText);
        }

        public static void ColorSelectedSyls()
        {
            ActOnSelectedText(MarkSyls, "Syllabes", ActoOnRangeMSWText);
        }

        public static void ColorSelectedWords()
        {
            ActOnSelectedText(MarkWords, "Mots", ActoOnRangeMSWText);
        }

        public static void ColorSelectedMuettes()
        {
            ActOnSelectedText(MarkMuettes, "Muettes", ActoOnRangeMSWText);
        }

        public static void ColorSelectedNoir()
        {
            ActOnSelectedText(MarkNoir, "Noir", ActoOnRangeMSWText);
        }

        public static void ColorSelectedLignes()
        {
            ActOnSelectedText(null, "Lignes", MarkLignes);
        }

        public static void ColorSelectedVoyCons()
        {
            ActOnSelectedText(MarkVoyCons, "Voy-Cons", ActoOnRangeMSWText);
        }


        private static void MarkLignes(Range range, ActOnMSWText act)
        {
            Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
            Config theConf = Config.GetConfigFor(activeWin, activeWin.Document);
            theConf.sylConf.ResetCounter();
            if (ColorizationMSW.thisAddIn.Application.ActiveWindow.View.Type == WdViewType.wdPrintView)
            {
                // Cherchons tous les Rectangles de la feneêtre active et travaillons sur toutes les lignes
                // qui se trouvent dans la sélection
                foreach (Page p in ColorizationMSW.thisAddIn.Application.ActiveWindow.ActivePane.Pages)
                {
                    foreach (Rectangle r in p.Rectangles)
                    {
                        if (r.RectangleType == WdRectangleType.wdTextRectangle)
                        {
                            foreach (Line l in r.Lines)
                            {
                                Range lineRange = l.Range;
                                if (lineRange.InRange(range))
                                {
                                    // the line is in the selected region
                                    MSWText.ApplyCFToRange(theConf.sylConf.NextCF(), lineRange, theConf);
                                }
                            }
                        }
                    }
                }

            } else
            {
                MessageBox.Show("La mise en couleur de lignes ne fonctionne que dans le mode \'Page\'.");
            }
                
        }

        private static void ActoOnRangeMSWText (Range range, ActOnMSWText actOn)
        {
            Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
            actOn(new MSWText(range, Config.GetConfigFor(activeWin, activeWin.Document)));
        }

        private static void ActOnShape(Shape sh, ActOnMSWText act, ActOnRange aor)
        {
            if (sh.TextFrame.HasText == (int)Microsoft.Office.Core.MsoTriState.msoTrue)
                aor(sh.TextFrame.TextRange, act);    

            if (sh.Type == Microsoft.Office.Core.MsoShapeType.msoGroup)
                foreach (Shape descSh in sh.GroupItems)
                    ActOnShape(descSh, act, aor);
        }

        private static void ActOnSelectedText(ActOnMSWText act, string undoTxt, ActOnRange aor)
        {
            if (ColorizationMSW.thisAddIn.Application.Documents.Count > 0)
            {
                UndoRecord objUndo = ColorizationMSW.thisAddIn.Application.UndoRecord;
                objUndo.StartCustomRecord(undoTxt);
                ColorizationMSW.thisAddIn.Application.ScreenUpdating = false;

                Selection sel = ColorizationMSW.thisAddIn.Application.ActiveWindow.Selection;

                switch (sel.Type)
                {
                    case WdSelectionType.wdSelectionInlineShape:
                    case WdSelectionType.wdNoSelection:
                    case WdSelectionType.wdSelectionIP: // IP: Insertion Point
                        // rien à faire ...
                        break;
                    case WdSelectionType.wdSelectionFrame:
                        foreach (Frame f in sel.Frames)
                            aor(f.Range, act);
                        break;
                    case WdSelectionType.wdSelectionShape:
                        foreach (Shape sh in sel.ShapeRange)
                            ActOnShape(sh, act, aor);
                        break;
                    case WdSelectionType.wdSelectionColumn:
                    case WdSelectionType.wdSelectionRow:
                    case WdSelectionType.wdSelectionBlock:
                    case WdSelectionType.wdSelectionNormal:
                        aor(sel.Range, act);
                        foreach (Shape sh in sel.Range.ShapeRange)
                            ActOnShape(sh, act, aor);
                        break;
                    default:
                        throw new ArgumentException(String.Format("Type de sélection non traitée: {0}", sel.Type.ToString()));
                        break;
                }

                ColorizationMSW.thisAddIn.Application.ScreenUpdating = true;
                objUndo.EndCustomRecord();
            }
        }

        private static void DocClosed(Document inDoc, ref bool cancel)
        {
            logger.ConditionalTrace("DocClosed");
            ConfigPane.DocClosed(inDoc);
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            logger.ConditionalTrace("Ribbon1_Load");
            ColorizationMSW.thisAddIn.Application.DocumentBeforeClose 
                += new ApplicationEvents4_DocumentBeforeCloseEventHandler(DocClosed);
            ColorizationMSW.thisAddIn.Application.WindowSelectionChange
                += new ApplicationEvents4_WindowSelectionChangeEventHandler(SelChanged_Event);
        }

        private void SelChanged_Event(Selection sel)
        {
            bool selected;
            switch (sel.Type)
            {
                case WdSelectionType.wdSelectionInlineShape:
                case WdSelectionType.wdNoSelection:
                case WdSelectionType.wdSelectionIP:
                    selected = false;
                    break;
                default:
                    selected = true; // par défaut on considère qu'il faut activer les boutons
                    break;
            }
            EnableButtons(selected);
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
                btnPhonemes.Enabled = enable;
                btnSyls.Enabled = enable;
                btnVoyCons.Enabled = enable;
            }
        }

        private void btnPhonemes_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedPhons(); ;
        }

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

        private void btnLignes_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedLignes();
        }

        private void btnVoyCons_Click(object sender, RibbonControlEventArgs e)
        {
            ColorSelectedVoyCons();
        }

        private void grpDialogLauncher_Click(object sender, RibbonControlEventArgs e)
        {
            if (ColorizationMSW.thisAddIn.Application.Documents.Count > 0)
            {
                Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
                ConfigPane.MakePaneVisibleInWin(activeWin, activeWin.Document, ColorizationMSW.thisAddIn.CustomTaskPanes,
                    typeof(ColorizationMSW).Assembly.GetName().Version.ToString());
            }
        }
    }
}
