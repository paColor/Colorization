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
        private delegate void ActOnPPTText(PPTText t, Config conf);

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static void ColorizePhons(PPTText t, Config conf) => t.ColorizePhons(conf, PhonConfType.phonemes);
        private static void MarkLetters(PPTText t, Config conf) => t.MarkLetters(conf);
        private static void MarkSyls(PPTText t, Config conf) => t.MarkSyls(conf);
        private static void MarkWords(PPTText t, Config conf) => t.MarkWords(conf);
        private static void MarkMuettes(PPTText t, Config conf) => t.MarkMuettes(conf);
        private static void MarkNoir(PPTText t, Config conf) => t.MarkNoir(conf);
        private static void MarkVoyCons(PPTText t, Config conf) => t.MarkVoyCons(conf);
        private static void MarkLignes(PPTText t, Config conf) => t.MarkLignes(conf);
        private static void MarkDuo(PPTText t, Config conf) => t.MarkDuo(conf);
        

        public static void Init()
        {
            logger.ConditionalDebug("Init");
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

        public static void ColorizeSelectedPhons(Config conf)
        {
            logger.ConditionalDebug("ColorizeSelectedPhons");
            ActOnSelectedText(ColorizePhons, conf);
        }

        public static void ColorSelectedLetters(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedLetters");
            ActOnSelectedText(MarkLetters, conf);
        }

        public static void ColorSelectedSyls(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedSyls");
            ActOnSelectedText(MarkSyls, conf);
        }

        public static void ColorSelectedWords(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedWords");
            ActOnSelectedText(MarkWords, conf);
        }

        public static void ColorSelectedMuettes(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedMuettes");
            ActOnSelectedText(MarkMuettes, conf);
        }

        public static void ColorSelectedNoir(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedNoir");
            ActOnSelectedText(MarkNoir, conf);
        }

        public static void ColorSelectedVoyCons(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedVoyCons");
            ActOnSelectedText(MarkVoyCons, conf);
        }

        public static void ColorSelectedLignes(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedLignes");
            ActOnSelectedText(MarkLignes, conf);
        }

        public static void ColorSelectedDuo(Config conf)
        {
            logger.ConditionalDebug("ColorSelectedDuo");
            ActOnSelectedText(MarkDuo, conf);
        }

        private static void ActOnShape(Shape sh, ActOnPPTText act, int nrObjSelected, Config conf)
        {
            logger.ConditionalDebug("ActOnShape");
            Debug.Assert(sh != null);
            if(sh.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue){
                if (sh.TextFrame.HasText == Microsoft.Office.Core.MsoTriState.msoTrue)
                    act(new PPTText(sh.TextFrame.TextRange), conf);
            } else if (sh.Type == Microsoft.Office.Core.MsoShapeType.msoGroup)
                foreach (Shape descSh in sh.GroupItems)
                    ActOnShape(descSh, act, nrObjSelected, conf);
            if (sh.HasTable == Microsoft.Office.Core.MsoTriState.msoTrue)
                foreach (Row r in sh.Table.Rows)
                    foreach (Cell c in r.Cells)
                        if ((nrObjSelected > 1) || (c.Selected))
                            ActOnShape(c.Shape, act, nrObjSelected, conf);
            // il y a visiblement un problème avec la sélection de tableaux. Les cellules ne sont pas sélectionnées
            // si plusieurs objects sont sélectionnés dont le tableau...
            // rendons donc le comportement dépendant du nombre d'objets dans la sélection... Y a-t-il un piège?
            // Powerpoint lui-même n'utilise pas ce truc. Hypothèse il s'agit d'un bug de Powerpoint dans la version
            // que j'utilise. Le comportement pourrait donc changer. Avec un peu de chance, le code devrait continuer 
            // à se comporter correctement.
        } // private void ActOnShape
   
        private static void ActOnSelectedText(ActOnPPTText act, Config conf)
        {
            logger.ConditionalDebug("ActOnSelectedText");
            try
            {
                ProgressNotifier.thePN.Start();
                if (ColorizationPPT.thisAddIn.Application.Presentations.Count > 0)
                {
                    ColorizationPPT.thisAddIn.Application.StartNewUndoEntry();
                    var sel = ColorizationPPT.thisAddIn.Application.ActiveWindow.Selection;
                    if (sel.Type == PpSelectionType.ppSelectionText)
                    {
                        act(new PPTText(ColorizationPPT.thisAddIn.Application.ActiveWindow.Selection.TextRange), conf);
                    }
                    else if (sel.Type == PpSelectionType.ppSelectionShapes)
                    {
                        // bool textFound = false;
                        foreach (Shape sh in sel.ShapeRange)
                        {
                            ActOnShape(sh, act, sel.ShapeRange.Count, conf);
                        } // foreach
                    } // else no text selected
                    else if (sel.Type == PpSelectionType.ppSelectionSlides)
                    {
                        foreach (Slide s in sel.SlideRange)
                        {
                            foreach (Shape sh in s.Shapes)
                            {
                                ActOnShape(sh, act, s.Shapes.Count, conf);
                            }
                        }
                    }
                }
                ProgressNotifier.thePN.Completed();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Ouups. Une vilaine erreur s'est produite. Désolé. N'hésitez pas à nous ");
                sb.AppendLine("envoyer une description de votre problème à info@colorization.ch.");
                sb.AppendLine(e.Message);
                sb.AppendLine(e.StackTrace);
                logger.Error(sb.ToString());
                MessageBox.Show(sb.ToString(), ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            logger.ConditionalDebug("EXIT ActOnSelectedText");
        } // void ColorizeSelectedPhons()

        private static void PresentationClosed(Presentation inPres) => ConfigPane.DocClosed(inPres);

        private static Config GetConfigForActiveWindow()
        {
            DocumentWindow activeWin = ColorizationPPT.thisAddIn.Application.ActiveWindow;
            string errMsg;
            Config toReturn = Config.GetConfigFor(activeWin, activeWin.Presentation, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
                MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return toReturn;
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            logger.ConditionalDebug("Ribbon1_Load");
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
            logger.ConditionalDebug("grpDialogLauncher_Click");
            if (ColorizationPPT.thisAddIn.Application.Presentations.Count > 0)
            {
                DocumentWindow activeWin = ColorizationPPT.thisAddIn.Application.ActiveWindow;
                ConfigPane.MakePaneVisibleInWin(activeWin, activeWin.Presentation, ColorizationPPT.thisAddIn.CustomTaskPanes,
                    typeof(ColorizationPPT).Assembly.GetName().Version.ToString());
            }
        }

        private void btnColoriser_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnColoriser_Click");
            ColorizeSelectedPhons(GetConfigForActiveWindow());
        } // private void btnColoriser_Click

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

        private void btnVoyCons_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnVoyCons_Click");
            ColorSelectedVoyCons(GetConfigForActiveWindow());
        }

        private void btnLignes_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnLignes_Click");
            ColorSelectedLignes(GetConfigForActiveWindow());
        }

        private void btnDuo_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnDuo_Click");
            ColorSelectedDuo(GetConfigForActiveWindow());
        }
    }
}
