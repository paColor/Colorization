﻿/********************************************************************************
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
        /// <summary>
        /// APplique un formatage au texte donné par <c>t</c> en utilisant la <c>Config</c> <c>conf</c>.
        /// </summary>
        /// <param name="t">Le texte à formater.</param>
        /// <param name="conf">La <c>Config</c> à utiliser.</param>
        private delegate void ActOnMSWText(MSWText t, Config conf);

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static void MarkPhons(MSWText t, Config conf) => t.ColorizePhons(conf, PhonConfType.phonemes);
        private static void MarkLetters(MSWText t, Config conf) => t.MarkLetters(conf);
        private static void MarkSyls(MSWText t, Config conf) => t.MarkSyls(conf);
        private static void MarkWords(MSWText t, Config conf) => t.MarkWords(conf);
        private static void MarkMuettes(MSWText t, Config conf) => t.MarkMuettes(conf);
        private static void MarkNoir(MSWText t, Config conf) => t.MarkNoir(conf);
        private static void MarkVoyCons(MSWText t, Config conf) => t.MarkVoyCons(conf);
        private static void MarkLignes(MSWText t, Config conf) => t.MarkLignes(conf);
        private static void MarkDuo(MSWText t, Config conf) => t.MarkDuo(conf);


        public static void Init()
        {
            logger.ConditionalDebug("Init");
            ConfigControl.markSelLetters = WordRibbon.ColorSelectedLetters;
            ConfigControl.colorizeAllSelPhons = WordRibbon.ColorSelectedPhons;
            ConfigControl.colSylSelLetters = WordRibbon.ColorSelectedSyls;
            ConfigControl.colMotsSelLetters = WordRibbon.ColorSelectedWords;
            ConfigControl.colLignesSelText = WordRibbon.ColorSelectedLignes;
            ConfigControl.colVoyConsSelText = WordRibbon.ColorSelectedVoyCons;
            ConfigControl.colNoirSelText = WordRibbon.ColorSelectedNoir;
            ConfigControl.colMuettesSelText = WordRibbon.ColorSelectedMuettes;
            ConfigControl.colDuoSelText = WordRibbon.ColorSelectedDuo;
        }

        public static void ColorSelectedPhons(Config conf)
        {
            logger.Info("ColorSelectedPhons");
            ActOnSelectedText(MarkPhons, "Phonèmes", conf);
        }

        public static void ColorSelectedLetters(Config conf)
        {
            logger.Info("ColorSelectedLetters");
            ActOnSelectedText(MarkLetters, "bpdq", conf);
        }

        public static void ColorSelectedSyls(Config conf)
        {
            logger.Info("ColorSelectedSyls");
            ActOnSelectedText(MarkSyls, "Syllabes", conf);
        }

        public static void ColorSelectedWords(Config conf)
        {
            logger.Info("ColorSelectedWords");
            ActOnSelectedText(MarkWords, "Mots", conf);
        }

        public static void ColorSelectedMuettes(Config conf)
        {
            logger.Info("ColorSelectedMuettes");
            ActOnSelectedText(MarkMuettes, "Muettes", conf);
        }

        public static void ColorSelectedNoir(Config conf)
        {
            logger.Info("ColorSelectedNoir");
            ActOnSelectedText(MarkNoir, "Noir", conf);
        }

        public static void ColorSelectedLignes(Config conf)
        {
            logger.Info("ColorSelectedLignes");
            ActOnSelectedText(MarkLignes, "Lignes", conf);
        }

        public static void ColorSelectedVoyCons(Config conf)
        {
            logger.Info("ColorSelectedVoyCons");
            ActOnSelectedText(MarkVoyCons, "Voy-Cons", conf);
        }

        public static void ColorSelectedDuo(Config conf)
        {
            logger.Info("ColorSelectedDuo");
            ActOnSelectedText(MarkDuo, "Duo", conf);
        }

        private static void ActOnShape(Shape sh, ActOnMSWText act, Config conf)
        {
            logger.ConditionalDebug("ActOnShape");
            if (sh.TextFrame.HasText == (int)Microsoft.Office.Core.MsoTriState.msoTrue)
                act(new MSWText(sh.TextFrame.TextRange), conf); 

            if (sh.Type == Microsoft.Office.Core.MsoShapeType.msoGroup)
                foreach (Shape descSh in sh.GroupItems)
                    ActOnShape(descSh, act, conf);
        }

        /// <summary>
        /// Fais en sorte que l'action <c>aor</c> soit exécutée sur tous les "ranges" sélectionnés.
        /// </summary>
        /// <param name="act">L'action à effectuer sur un texte. Par exemple <c>MarkSyls</c> ou <c>MarkNoir</c></param>
        /// <param name="undoTxt">Le texte qui est inscrit dans le <c>UndoRecord</c> et que l'utilisateur voit s'il va
        /// sur la lsite des actions qu'il peut annuler.</param>
        private static void ActOnSelectedText(ActOnMSWText act, string undoTxt, Config conf)
        {
            logger.ConditionalDebug("ActOnSelectedText");
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
                            act(new MSWText(f.Range), conf);
                        break;
                    case WdSelectionType.wdSelectionShape:
                        foreach (Shape sh in sel.ShapeRange)
                            ActOnShape(sh, act, conf);
                        break;
                    case WdSelectionType.wdSelectionColumn:
                    case WdSelectionType.wdSelectionRow:
                    case WdSelectionType.wdSelectionBlock:
                    case WdSelectionType.wdSelectionNormal:
                        act(new MSWText(sel.Range), conf);
                        foreach (Shape sh in sel.Range.ShapeRange)
                            ActOnShape(sh, act, conf);
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
            logger.ConditionalDebug("DocClosed");
            ConfigPane.DocClosed(inDoc);
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            logger.ConditionalDebug("Ribbon1_Load");
            ColorizationMSW.thisAddIn.Application.DocumentBeforeClose 
                += new ApplicationEvents4_DocumentBeforeCloseEventHandler(DocClosed);
            ColorizationMSW.thisAddIn.Application.WindowSelectionChange
                += new ApplicationEvents4_WindowSelectionChangeEventHandler(SelChanged_Event);
        }

        private void SelChanged_Event(Selection sel)
        {
            logger.ConditionalDebug("SelChanged_Event");
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
            logger.ConditionalDebug("EnableButtons to \'{0}\'", enable);
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
                btnDuo.Enabled = enable;
            }
        }

        private Config GetConfigForActiveWindow()
        {
            Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
            return Config.GetConfigFor(activeWin, activeWin.Document);
        }

        private void btnPhonemes_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnPhonemes_Click");
            ColorSelectedPhons(GetConfigForActiveWindow()); ;
        }

        private void btnBDPQ_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnBDPQ_Click");
            ColorSelectedLetters(GetConfigForActiveWindow());
        }

        private void btnSyl_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnSyl_Click");
            ColorSelectedSyls(GetConfigForActiveWindow());
        }

        private void btnMots_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnMots_Click");
            ColorSelectedWords(GetConfigForActiveWindow());
        }

        private void btnMuettes_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnMuettes_Click");
            ColorSelectedMuettes(GetConfigForActiveWindow());
        }

        private void btnNoir_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnNoir_Click");
            ColorSelectedNoir(GetConfigForActiveWindow());
        }

        private void btnLignes_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnLignes_Click");
            ColorSelectedLignes(GetConfigForActiveWindow());
        }

        private void btnVoyCons_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnVoyCons_Click");
            ColorSelectedVoyCons(GetConfigForActiveWindow());
        }

        private void btnDuo_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnDuo_Click");
            ColorSelectedDuo(GetConfigForActiveWindow());
        }

        private void grpDialogLauncher_Click(object sender, RibbonControlEventArgs e)
        {
            logger.Info("grpDialogLauncher_Click");
            if (ColorizationMSW.thisAddIn.Application.Documents.Count > 0)
            {
                Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
                ConfigPane.MakePaneVisibleInWin(activeWin, activeWin.Document, ColorizationMSW.thisAddIn.CustomTaskPanes,
                    typeof(ColorizationMSW).Assembly.GetName().Version.ToString());
            }
        }
    }
}
