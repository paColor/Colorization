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
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Microsoft.Office.Tools;
using Microsoft.Office.Interop.Word;
using ColorLib;
using ColorizationControls;
using System.Threading;
using System.Diagnostics;

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
        private static void MarkArcs(MSWText t, Config conf) => t.MarkArcs(conf);
        private static void RemoveArcs(MSWText t, Config conf) => t.RemoveArcs(conf);
        private static void MarkPonct(MSWText t, Config conf) => t.MarkPonct(conf);
        private static void MarKMajDebut(MSWText t, Config conf) => t.MarkMajDebut(conf);
        private static void AddSpace(MSWText t, Config conf) => t.AddSpace(conf);
        private static void ShrinkSpace(MSWText t, Config conf) => t.ShrinkSpace(conf);


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
            ConfigControl.drawArcs = WordRibbon.ColorSelectedArcs;
            ConfigControl.removeArcs = WordRibbon.RemoveSelectedArcs;
            ConfigControl.colPonctuation = WordRibbon.ColorPonctuation;
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

        public static void ColorSelectedArcs(Config conf)
        {
            logger.Info("ColorSelectedArcs");
            ActOnSelectedText(MarkArcs, "Arcs", conf);
        }

        public static void RemoveSelectedArcs(Config conf)
        {
            logger.Info("RemoveSelectedArcs");
            ActOnSelectedText(RemoveArcs, "Effacer arcs", conf);
        }

        public static void ColorPonctuation(Config conf)
        {
            logger.Info("ColorPonctuation");
            ActOnSelectedText(MarkPonct, "Ponctuation", conf);
            if (conf.ponctConf.MajDebCB)
            {
                ActOnSelectedText(MarKMajDebut, "Majuscules début", conf);
            }
        }

        /// <summary>
        /// Ajoute un espace entre chaque mot du texte sélectionné.
        /// </summary>
        /// <remarks>
        /// A priori, la config n'est pas utilisée, mais ça permet de réutiliser le même "pattern"
        /// </remarks>
        /// <param name="conf">La <see cref="Config"/> à utiliser.</param>
        public static void Ecarter(Config conf)
        {
            logger.Info("Ecarter");
            ActOnSelectedText(AddSpace, "Écarter", conf);
        }

        /// <summary>
        /// Enlève un espace entre chaque mot du texte sélectionné. Ne fait rien s'il n'y a
        /// qu'un seul espace.
        /// </summary>
        /// <remarks>
        /// A priori, la config n'est pas utilisée, mais ça permet de réutiliser le même "pattern"
        /// </remarks>
        /// <param name="conf">La <see cref="Config"/> à utiliser.</param>
        public static void Resserrer(Config conf)
        {
            logger.Info("Resserrer");
            ActOnSelectedText(ShrinkSpace, "Resserrer", conf);
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
            try
            {
                ProgressNotifier.thePN.Start();
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
                Debug.Assert(false);
            }
            logger.ConditionalDebug("EXIT ActOnSelectedText");
        }

        private static void DocClosed(Document inDoc, ref bool cancel)
        {
            logger.ConditionalDebug("DocClosed");
            ConfigPane.DocClosed(inDoc);
        }

        /// <summary>
        /// Initialise les "handlers" (comment on dit ça en français?) pour le traitement du changement
        /// de sélection et la fermeture de document. 
        /// </summary>
        /// <param name="app">L'application Word dans laquelle ceci est exécuté.</param>
        public void InitHandlers (Microsoft.Office.Interop.Word.Application app)
        {
            app.DocumentBeforeClose += new ApplicationEvents4_DocumentBeforeCloseEventHandler(DocClosed);
            app.WindowSelectionChange += new ApplicationEvents4_WindowSelectionChangeEventHandler(SelChanged_Event);
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            logger.ConditionalDebug("Ribbon1_Load");
            if (ColorizationMSW.thisAddIn != null)
            {
                // Il semblerait que l'ordre de création de thisAddIn et du ruban ne soit pas fixe ??
                // Pour parer à toute éventualité, on effectue donc le test. Je ne suis pas sûr qu'il
                // n'y ait qu'une instance du ruban, même si je ne comprendrais pas comment ça marche
                // autrement... 
                InitHandlers(ColorizationMSW.thisAddIn.Application);
            }
            else
            {
                ColorizationMSW.RegisterRibbon(this);
            }
            EnableButtons(false);
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
                btnArcs.Enabled = enable;
                btnNettoyageArcs.Enabled = enable;
                btnPonct.Enabled = enable;
                btnEcarter.Enabled = enable;
                btnResserrer.Enabled = enable;
            }
        }

        private static Config GetConfigForActiveWindow()
        {
            Window activeWin = ColorizationMSW.thisAddIn.Application.ActiveWindow;
            string errMsg;
            Config toReturn = Config.GetConfigFor(activeWin, activeWin.Document, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
                MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return toReturn;
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

        private void btnArcs_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnArcs_Click");
            ColorSelectedArcs(GetConfigForActiveWindow());
        }

        private void btnNettoyageArcs_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnNettoyageArcs_Click");
            RemoveSelectedArcs(GetConfigForActiveWindow());
        }

        private void btnPonct_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnPonct_Click");
            ColorPonctuation(GetConfigForActiveWindow());
        }

        private void btnEcarter_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnEcarter_Click");
            Ecarter(GetConfigForActiveWindow());
        }

        private void btnResserrer_Click(object sender, RibbonControlEventArgs e)
        {
            logger.ConditionalDebug("btnResserrer_Click");
            Resserrer(GetConfigForActiveWindow());
        }
    }
}
