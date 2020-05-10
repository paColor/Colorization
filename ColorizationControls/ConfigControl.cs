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
using System.Deployment.Application;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorLib;
using System.Diagnostics;

namespace ColorizationControls
{
    public delegate bool ReturnCheckBoxValue(string cbuName);
    public delegate void CheckBoxChecked(string cbuName, bool val);

    public partial class ConfigControl : UserControl
    {
        public static ExecuteTask colSylSelLetters { set; private get; }
        public static ExecuteTask colMotsSelLetters { set; private get; }
        public static ExecuteTask colLignesSelText { set; private get; }
        public static ExecuteTask colVoyConsSelText { set; private get; }
        public static ExecuteTask colNoirSelText { set; private get; }
        public static ExecuteTask colorizeAllSelPhons { set; private get; }
        public static ExecuteTask colMuettesSelText { set; private get; }
        public static ExecuteTask markSelLetters { set; private get; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static CharFormatting clipboard = null;

        private Dictionary <string, CheckBox> formattingCheckBoxes;

        private class SonInfo
        {
            public CheckBox cbx { get; set; }
            public Button btn { get; set; }
            public RGB btnOrigColor { get; set; }

            // public SonInfo() {}
        }

        private Object theWin; // La fenêtre dans laquelle le contrôle est placé.
        private Object theDoc; // le document ouvert dans la fenêtre.
        private Config theConf; // la configuration dont le contrôle est le GUI

        private Dictionary<int, Button> letterButtons;
        private RGB defaultLetterButtonCol;
        private Button[] sylButtons;
        private PictureBox[] sylPictureBoxes;
        private RGB defaultSylButtonCol;
        private Dictionary<string, SonInfo> sonInfos;
        private MyColorDialog mcd4Syls;

        private PhonConfType pct;
        private string cmsButType; // type of button, that was right clicked e.g. "btSC", "btL", "btn", ...
        private int cmsButNr; // le numéro du bouton cliqué droit
        private string cmsButSon = ""; // contient le son du bouton cliqué droit
        private int countPasteLetters; // pour itérer à traver lettersToPaste quand on colle sur une lettre vide.
        private const string lettersToPaste = @"ƨ$@#<>*%()?![]{},.;:/\-_§°~¦|";

        
        public static void Init ()
        {
            StaticColorizControls.Init();
        }

        public ConfigControl(Object inWin, Object inDoc, string version)
        {
            InitializeComponent(); // calls the setup of the whole component
            theWin = inWin;
            theDoc = inDoc;
            theConf = Config.GetConfigFor(theWin, theDoc);
            InitializeTheConf();

            // letterButtons
            letterButtons = new Dictionary<int, Button>(8);
            defaultLetterButtonCol = btL0.BackColor;

            // son
            sonInfos = new Dictionary<string, SonInfo>(ColConfWin.nrSons);
            foreach (string theSon in ColConfWin.GetListOfSons())
                sonInfos.Add(theSon, new SonInfo());

            // UCheckBoxes
            formattingCheckBoxes = new Dictionary<string, CheckBox>(6); // 6 is just an estimation. Currently the correct number is 5

            // Syllabes
            sylButtons = new Button[SylConfig.nrButtons];
            sylPictureBoxes = new PictureBox[SylConfig.nrButtons];
            defaultSylButtonCol = btSC0.BackColor;
            mcd4Syls = new MyColorDialog();
            mcd4Syls.CustomColors = StaticColorizControls.customColors;
            mcd4Syls.AnyColor = true;
            mcd4Syls.FullOpen = true;

            // pct
            pct = PhonConfType.phonemes; //  par défaut on édite la configration des phonèmes

            countPasteLetters = 0;

            SetLocalTablesForControl(this);
            
            if (ApplicationDeployment.IsNetworkDeployed)
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                // we try to use the deployment version... Should work for distributed versions. Else we use the version
                // set by the higher level, which corresponds to the assembly version.

            lblVersion.Text = "Version: " + version;
        }

        public void UpdateSonButton(string son)
        {
            logger.ConditionalTrace("UpdateSonButton son: {0}", son);
            RGB btnColor;
            CharFormatting cf;
            SonInfo si;

            si = sonInfos[son];
            btnColor = si.btnOrigColor;
            if (si.cbx.Checked)
            {
                cf = theConf.colors[pct].Get(son);
                if (cf.changeColor)
                    btnColor = cf.color;
            }
            SetButtonColor(si.btn, btnColor);
        }

        public void UpdateCbxSon(string son)
        {
            logger.ConditionalTrace("UpdateCbxSon son: {0}", son);
            sonInfos[son].cbx.Checked = theConf.colors[pct].GetCheck(son);
        }

        public void UpdateAllSoundCbxAndButtons()
        {
            logger.ConditionalTrace("UpdateAllSoundCbxAndButtons");
            theConf.colors[pct].DisableCbxSonsEventHandling();
            SuspendLayout();

            foreach (string son in sonInfos.Keys)
            {
                // checkbox
                UpdateCbxSon(son);
                // button
                UpdateSonButton(son);
            }
            cbSBlackPhons.Checked = (ColConfWin.defBeh == ColConfWin.DefBeh.noir);
            ResumeLayout();
            theConf.colors[pct].EnableCbxSonsEventHandling();
        }

        public void UpdateLetterButtons()
        {
            logger.ConditionalTrace("UpdateLetterButtons");
            SuspendLayout();
            for (int i = 0; i < letterButtons.Count; i++)
                UpdateLetterButton(i);
            cbAlettresNoir.Checked = theConf.pBDQ.markAsBlack;
            ResumeLayout();
        }

        public void UpdateLetterButton(int buttonNr)
        {
            logger.ConditionalTrace("UpdateLetterButton buttonNr: {0}", buttonNr);
            char c;
            CharFormatting cf = theConf.pBDQ.GetCfForPBDQButton(buttonNr, out c);
            StringBuilder sb = new StringBuilder(1);
            sb.Append(c);
            letterButtons[buttonNr].Text = sb.ToString();
            RGB theButtonCol;
            if ((cf != null) && (cf.changeColor))
                theButtonCol = cf.color;
            else
                theButtonCol = defaultLetterButtonCol;
            SetButtonColor(letterButtons[buttonNr], theButtonCol);
        }

        public void UpdateSylButtons ()
        {
            logger.ConditionalTrace("UpdateLetterButtons");
            SuspendLayout();
            for (int i = 0; i < SylConfig.nrButtons; i++)
                UpdateSylButton(i);
            cbSStd.Checked = theConf.sylConf.DoubCons();
            cbSEcrit.Checked = theConf.sylConf.ModeEcrit();
            ResumeLayout();
        }

        public void UpdateSylButton(int butNr)
        {
            logger.ConditionalTrace("UpdateSylButton buttonNr: {0}", butNr);
            SylConfig.SylButtonConf sbC = theConf.sylConf.GetSylButtonConfFor(butNr);
            Button theButton = sylButtons[butNr];
            RGB theButtonCol = defaultSylButtonCol;
            RGB thePbxCol = defaultSylButtonCol;
            if ((sbC.cf != null))
            {
                if (sbC.cf.changeColor)
                    theButtonCol = sbC.cf.color;
                if (sbC.cf.changeHilight)
                    thePbxCol = sbC.cf.hilightColor;
                else
                    thePbxCol = theButtonCol;
            }
            SetButtonColor(theButton, theButtonCol);
            sylPictureBoxes[butNr].BackColor = thePbxCol;
            theButton.Enabled = sbC.buttonClickable;
        }


        public void UpdateUcheckBoxes()
        {
            logger.ConditionalTrace("UpdateUcheckBoxes");
            foreach (string cbUname in formattingCheckBoxes.Keys)
            {
                CheckBox fcb = formattingCheckBoxes[cbUname];
                fcb.Checked = theConf.unsetBeh.CbuVal(cbUname);
            }
        }

        internal void UpdateIllRadioB()
        {
            logger.ConditionalTrace("UpdateUcheckBoxes");
            if (theConf.colors[pct].IllRuleToUse == ColConfWin.IllRule.ceras)
            {
                rbnIllCeras.Checked = true;
                rbnIllLireCouleur.Checked = false;
            }
            else
            {
                rbnIllCeras.Checked = false;
                rbnIllLireCouleur.Checked = true;
            } 
        }

        public void UpdateAll()
        {
            logger.ConditionalTrace("UpdateAll");
            UpdateAllSoundCbxAndButtons();
            UpdateLetterButtons();
            UpdateUcheckBoxes();
            UpdateSylButtons();
            UpdateIllRadioB();
        }

        private void InitializeTheConf()
        // établit le lien entre le contrôle et la config en définissant les upcalls.
        {
            theConf.updateConfigName = this.UpdateConfigName;
            theConf.updateListeConfigs = this.UpdateListeConfigs;
            theConf.colors[PhonConfType.phonemes].updateAllSoundCbxAndButtons = this.UpdateAllSoundCbxAndButtons;
            theConf.colors[PhonConfType.phonemes].updateButton = this.UpdateSonButton;
            theConf.colors[PhonConfType.phonemes].updateCbx = this.UpdateCbxSon;
            theConf.colors[PhonConfType.phonemes].updateIllRule = this.UpdateIllRadioB;
            theConf.colors[PhonConfType.muettes].updateAllSoundCbxAndButtons = this.UpdateAllSoundCbxAndButtons;
            theConf.colors[PhonConfType.muettes].updateButton = this.UpdateSonButton;
            theConf.colors[PhonConfType.muettes].updateCbx = this.UpdateCbxSon;
            theConf.colors[PhonConfType.muettes].updateIllRule = this.UpdateIllRadioB;
            theConf.pBDQ.updateLetterButtons = this.UpdateLetterButtons;
            theConf.pBDQ.updateLetterButton = this.UpdateLetterButton;
            theConf.sylConf.updateSylButtons = this.UpdateSylButtons;
            theConf.sylConf.updateSylButton = this.UpdateSylButton;
            theConf.unsetBeh.updateUCheckBoxes = this.UpdateUcheckBoxes;
        }

        private void ConfigControl_Load(object sender, EventArgs e)
        {
            logger.Info("ConfigControl_Load");
            UpdateAll();
        }

        private void SetLocalTablesForControl(Control c)
        {
            logger.ConditionalTrace("SetLocalTablesForControl");
            string cNameFin;
            Button b;

            if (c.GetType().Equals(typeof(CheckBox)))
            {
                if (c.Name.StartsWith("cbx")) // Caution the name of other Checkboxes has to begin differently
                {
                    cNameFin = c.Name.Substring(3, c.Name.Length - 3);
                    sonInfos[cNameFin].cbx = (CheckBox)c;
                }
                else if(c.Name.StartsWith("cbu")) // Upcall buttons
                {
                    cNameFin = c.Name.Substring(3, c.Name.Length - 3);
                    formattingCheckBoxes[cNameFin] = (CheckBox)c;
                }
            }
            else if (c.GetType().Equals(typeof(Button)))
            {
                if (c.Name.StartsWith("btn")) // Caution the name of other Buttons has to begin differently
                {
                    cNameFin = c.Name.Substring(3, c.Name.Length - 3);
                    b = (Button)c;
                    sonInfos[cNameFin].btn = b;
                    sonInfos[cNameFin].btnOrigColor = b.BackColor;
                    b.ContextMenuStrip = this.cmsEffacerCopier; // ça nous évite de les mettre à la main
                } 
                else if (c.Name.StartsWith("btSC"))
                {
                    Button theBtn = (Button)c;
                    string butNrTxt = theBtn.Name.Substring(4, theBtn.Name.Length - 4);
                    int butNr = int.Parse(butNrTxt);
                    sylButtons[butNr] = theBtn;
                    theBtn.ContextMenuStrip = this.cmsEffacerCopier; // ça nous évite de les mettre à la main
                }
                else if (c.Name.StartsWith("btL"))
                {
                    Button theBtn = (Button)c;
                    string butNrTxt = theBtn.Name.Substring(3, theBtn.Name.Length - 3);
                    int butNr = int.Parse(butNrTxt);
                    letterButtons[butNr] = theBtn;
                    // Le ContextMenuStrip est mis dans le designer
                }
            }
            else if (c.GetType().Equals(typeof(PictureBox)))
            {
                if (c.Name.StartsWith("pbHL"))
                {
                    PictureBox thePB = (PictureBox)c;
                    string pbNrTxt = thePB.Name.Substring(4, thePB.Name.Length - 4);
                    int pbNr = int.Parse(pbNrTxt);
                    sylPictureBoxes[pbNr] = thePB;
                    thePB.ContextMenuStrip = this.cmsEffacerCopier; // ça nous évite de les mettre à la main
                }
            }
            foreach (Control subC in c.Controls)
                SetLocalTablesForControl(subC);
        }

        private void SetButtonColor (Button b, RGB color)
        {
            b.BackColor = color;
            if (color.Dark())
                b.ForeColor = ColConfWin.predefinedColors[(int)PredefCols.white];
            else
                b.ForeColor = ColConfWin.predefinedColors[(int)PredefCols.black];
        }


        //--------------------------------------------------------------------------------------------
        // -------------------------- Boutons généraux phonèmes---------------------------------------
        //--------------------------------------------------------------------------------------------

        private void btnCERAS_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btnCERAS_Click");
            theConf.colors[pct].SetCeras();
        }

        private void btGCerasRose_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btnCERAS_Click");
            theConf.colors[pct].SetCerasRose();
        }

        private void btnTout_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btnTout_Click");
            theConf.colors[pct].SetAllCbxSons();
        }

        private void btnRien_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btnRien_Click");
            theConf.colors[pct].ClearAllCbxSons();
        }

        private void btcPhons_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btcPhons_Click");
            if (pct == PhonConfType.phonemes)
            {
                colorizeAllSelPhons();
            }
            else
            {
                colMuettesSelText();
            }
        }


        //--------------------------------------------------------------------------------------------
        // ----------------------------- CheckBoxes sons ---------------------------------------------
        //--------------------------------------------------------------------------------------------

        private void SonCheckBox_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;
            logger.ConditionalTrace("SonCheckBox_CheckedChanged {0}", cbx.Name);
            Debug.Assert(cbx.Name.StartsWith("cbx"));
            string son = cbx.Name.Substring(3, cbx.Name.Length - 3);
            theConf.colors[pct].SonConfCheckedChanged(son, cbx.Checked);
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------------- Boutons sons ---------------------------------------------
        //--------------------------------------------------------------------------------------------

        private void SonButton_Click(object sender, EventArgs e)
        {
            Button theBtn = (Button)sender;
            logger.ConditionalTrace("SonButton_Click {0}", theBtn.Name);
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            Debug.Assert(theBtn.Name.StartsWith("btn"));
            string son = theBtn.Name.Substring(3, theBtn.Name.Length - 3);
            CharFormatForm form = new CharFormatForm(theConf.colors[pct].Get(son), son, 
                theConf.colors[pct].UpdateCF);
            p.Offset(-form.Width, -(form.Height / 2));
            form.Location = p;
            _ = form.ShowDialog();
            form.Dispose();
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------------- Boutons lettres ------------------------------------------
        //--------------------------------------------------------------------------------------------

        private void LetterButton_Click(object sender, EventArgs e)
        {
            Button theBtn = (Button)sender;
            logger.ConditionalTrace("SonCheckBox_CheckedChanged {0}", theBtn.Name);
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            Debug.Assert(theBtn.Name.StartsWith("btL"));
            string butNrTxt = theBtn.Name.Substring(3, theBtn.Name.Length - 3);
            int butNr = int.Parse(butNrTxt);           
            LetterFormatForm form = new LetterFormatForm(theBtn.Text[0], butNr, theConf.pBDQ);
            p.Offset(-form.Width, -(form.Height / 2));
            form.Location = p;
            _ = form.ShowDialog();
            form.Dispose();
        }

        private void btCPBDQ_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btCPBDQ_Click");
            theConf.pBDQ.ResetStandardColors();
        }

        private void btcLbpdq_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btcLbpdq_Click");
            markSelLetters();
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------------- advanced checkboxes --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void cbAlettresNoir_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalTrace("cbAlettresNoir_CheckedChanged");
            Debug.Assert(sender != null);
            CheckBox cbx = (CheckBox)sender;
            theConf.pBDQ.BlackSettingCheckChangedTo(cbx.Checked);
        }

        private void cbSBlackPhons_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalTrace("cbSBlackPhons_CheckedChanged");
            Debug.Assert(sender != null);
            CheckBox cbx = (CheckBox)sender;
            ColConfWin.DefaultBehaviourChangedTo(cbx.Checked);
        }

        private void UcheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbu = (CheckBox)sender;
            logger.ConditionalTrace("UcheckBoxes_CheckedChanged {0}", cbu.Name);
            Debug.Assert(cbu.Name.StartsWith("cbu"));
            string cbuNameEnd = cbu.Name.Substring(3, cbu.Name.Length - 3);
            theConf.unsetBeh.CbuChecked(cbuNameEnd, cbu.Checked);
        }

        //--------------------------------------------------------------------------------------------
        // ---------------------------------- RadioButton "ill" --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void rbnIll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnIllCeras.Checked)
            {
                theConf.colors[pct].IllRuleToUse = ColConfWin.IllRule.ceras;
            }
            else
            {
                theConf.colors[pct].IllRuleToUse = ColConfWin.IllRule.lirecouleur;
            }
        }

        //--------------------------------------------------------------------------------------------
        // ---------------------------------- Checkboxes Syllabes ------------------------------------
        //--------------------------------------------------------------------------------------------

        private void cbSStd_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalTrace("cbSStd_CheckedChanged");
            CheckBox cbSStd = (CheckBox)sender;
            theConf.sylConf.DoubleConsModified(cbSStd.Checked);
        }

        private void cbSEcrit_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalTrace("cbSEcrit_CheckedChanged");
            CheckBox cbSEcrit = (CheckBox)sender;
            theConf.sylConf.ModeEcritModified(cbSEcrit.Checked);
        }

        //--------------------------------------------------------------------------------------------
        // ------------------------------------ Boutons Syllabes -------------------------------------
        //--------------------------------------------------------------------------------------------

        private void btSAppliquer_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSAppliquer_Click");
            colSylSelLetters();
        }

        private void btSMots_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSMots_Click");
            colMotsSelLetters();
        }

        private void btZeLignes_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btZeLignes_Click");
            colLignesSelText();
        }

        private void btSVoyCons_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSVoyCons_Click");
            colVoyConsSelText();
        }

        private void btcLNoir_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btcLNoir_Click");
            colNoirSelText();
        }

        private void btcInitSyls_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btcInitSyls_Click");
            theConf.sylConf.ResetStandardColors();
        }

        private void SylButton_Click(object sender, EventArgs e)
            // can be called for the Buttons and for the PictureBoxes
        {
            Control theControl = (Control)sender;
            logger.ConditionalTrace("SylButton_Click {0}", theControl.Name);
            Point mousePos = theControl.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            Debug.Assert(theControl.Name.StartsWith("btSC") || theControl.Name.StartsWith("pbHL"));
            string cNrTxt = theControl.Name.Substring(4, theControl.Name.Length - 4);
            int cNr = int.Parse(cNrTxt);
            if (theConf.sylConf.GetSylButtonConfFor(cNr).buttonClickable)
            {
                CharFormatForm form = new SylFormatForm(theConf.sylConf.GetSylButtonConfFor(cNr).cf,
                cNrTxt, theConf.sylConf.SylButtonModified);
                mousePos.Offset(-form.Width, -(form.Height / 2));
                form.Location = mousePos;
                _ = form.ShowDialog();
                form.Dispose();
            }
        }

        //--------------------------------------------------------------------------------------------
        // --------------------------------------  Onglet Sauv. --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void tabSauv_Enter(object sender, EventArgs e)
        {
            // Quand l'utilisateur rend l'onglet visible
            logger.ConditionalTrace("tabSauv_Enter");
            UpdateSauvTab();
        }

        public void UpdateConfigName()
        {
            logger.ConditionalTrace("UpdateConfigName");
            txtBNomConfig.Text = theConf.GetConfigName();
            btSauvSauv.Enabled = (txtBNomConfig.Text.Length > 0);
        }

        public void UpdateListeConfigs()
        {
            logger.ConditionalTrace("UpdateListeConfigs");
            lbConfigs.DataSource = Config.GetSavedConfigNames();
            btSauvCharger.Enabled = (lbConfigs.Items.Count > 0);
        }

        public void UpdateSauvTab()
        {
            logger.ConditionalTrace("UpdateSauvTab");
            UpdateConfigName();
            UpdateListeConfigs();
        }

        // --------------- txtBNomConfig : Text Box contenant le nom de la config ------------------------
        
        private void txtBNomConfig_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalTrace("txtBNomConfig_KeyPress: \'{0}\'", e.KeyChar);
            const string forbiddenChars = @"<>:/\|?*" + "\"";
            if (forbiddenChars.Contains(e.KeyChar))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"Les caractères /\?%*:|<>");
                sb.Append("\" ne peuvent pas être utilisés dans le nom d'une configuration.");
                MessageBox.Show(sb.ToString(), BaseConfig.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Handled = true;
                logger.Info("Caractère refusé pour le nom de config: \'{0}\', code ASCII \'{1}\'", e.KeyChar, (int)e.KeyChar);
            }
            else if (e.KeyChar == '\r' && txtBNomConfig.Text.Length > 0)
            {
                btSauvSauv.PerformClick();
                e.Handled = true;
            }
        }

        private void txtBNomConfig_KeyUp(object sender, KeyEventArgs e)
        {
            logger.ConditionalTrace("txtBNomConfig_KeyUp - nbre caractères: {0}", txtBNomConfig.Text.Length);
            // En fonction du nombre de caractères que contient le nom de la config, on peut activer ou désactiver 
            // le bouton de sauvegarde.
            btSauvSauv.Enabled = (txtBNomConfig.Text.Length > 0);
        }

        
        // --------------------------------- btSauvSauv : Bouton "Sauver" ------------------ ------------------------

        private void btSauvSauv_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSauvSauv_Click");
            if (txtBNomConfig.Text.Length > 0)
            {
                bool doIt = true;
                if (lbConfigs.FindStringExact(txtBNomConfig.Text) != ListBox.NoMatches)
                {
                    string message = String.Format(
                        "Un configuration poartant le nom \'{0}\' est déjà enregistrée. Souhaitez-vous l'écraser?",
                        txtBNomConfig.Text);
                    var result = MessageBox.Show(message, BaseConfig.ColorizationName, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    doIt = (result == DialogResult.Yes);
                }
                if (doIt)
                {
                    if (!theConf.SaveConfig(txtBNomConfig.Text))
                    {
                        string message = String.Format("Impossible de sauvegarder la configuration \'{0}\'", txtBNomConfig.Text);
                        MessageBox.Show(message, BaseConfig.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                logger.Warn("btSauvSauv_Click a été exécuté alors que txtBNomConfig.Text est vide.");
            }
        }

        private void btSauvSauv_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalTrace("btSauvSauv_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvSauv.PerformClick();
                e.Handled = true;
            }
        }


        // --------------- lbConfigs : ListBox contenant la liste des configs sauvegardées ------------------------

        private void lbConfigs_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalTrace("lbConfigs_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvCharger.PerformClick();
                e.Handled = true;
            }
        }

        // --------------------------------- btSauvCharger : Bouton "Charger" ------------------ ------------------------

        private void btSauvCharger_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSauvCharger_Click");
            string configName = lbConfigs.SelectedItem.ToString();
            Config newConfig = Config.LoadConfig(configName, theWin, theDoc);
            if (newConfig != null)
            {
                theConf = newConfig;
                InitializeTheConf();
                UpdateAll();
                UpdateConfigName();
            }
            else
            {
                string message = String.Format("Impossible de charger la configuration \'{0}\'", configName);
                MessageBox.Show(message, BaseConfig.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSauvCharger_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalTrace("btSauvCharger_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvCharger.PerformClick();
                e.Handled = true;
            }
        }

        // --------------------------------- btSauvCharger : Bouton "Effacer" ------------------ ------------------------

        private void btSauvEffacer_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("btSauvCharger_Click");
            string configName = lbConfigs.SelectedItem.ToString();
            string message = String.Format(
                        "Voulez-vous vraiment effacer la configuration \'{0}\' ?",
                        configName);
            var result = MessageBox.Show(message, BaseConfig.ColorizationName, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (Config.DeleteSavedConfig(configName))
                {
                    UpdateListeConfigs();
                }
                else
                {
                    string errMessages = String.Format("Impossible d'effacer la configuration \'{0}\'", configName);
                    MessageBox.Show(errMessages, BaseConfig.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //--------------------------------------------------------------------------------------------
        // ------------------------------------  Onglet A propos -------------------------------------
        //--------------------------------------------------------------------------------------------

        private void linkLireCouleur_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalTrace("linkLireCouleur_LinkClicked");
            this.linkLireCouleur.LinkVisited = true;
            System.Diagnostics.Process.Start(this.linkLireCouleur.Text);
        }

        private void linkCERAS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalTrace("linkCERAS_LinkClicked");
            this.linkCERAS.LinkVisited = true;
            System.Diagnostics.Process.Start(this.linkCERAS.Text);
        }

        private void linkInfoAtColorization_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalTrace("linkInfoAtColorization_LinkClicked");
            this.linkInfoAtColorization.LinkVisited = true;
            System.Diagnostics.Process.Start("mailto:info@colorization.ch");
        }

        private void linkCodeCouleurAPI_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalTrace("linkCodeCouleurAPI_LinkClicked");
            this.linkCodeCouleurAPI.LinkVisited = true;
            System.Diagnostics.Process.Start("http://api.ceras.ch/wp-content/uploads/2020/01/Sons-couleurs-symboles-et-coloriseur-API.pdf");
        }


        private void butLicense_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("butLicense_Click");
            Button theBtn = (Button)sender;
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location);
            LicenseForm lf = new LicenseForm();
            p.Offset(-lf.Width, -(lf.Height / 2));
            lf.Location = p;
            _ = lf.ShowDialog();
            lf.Dispose();
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------  Context Menu Strip - Clic droit --------------------------------
        //--------------------------------------------------------------------------------------------

        // cms => context menu strip
        private void cmsEffacerCopier_Opening(object sender, CancelEventArgs e)
        {
            string cName = cmsEffacerCopier.SourceControl.Name;
            logger.ConditionalTrace("cmsEffacerCopier_Opening {0}", cName);
            tsmiCouper.Enabled = false;
            tsmiCopier.Enabled = false;
            tsmiEffacer.Enabled = false;
            tsmiColler.Enabled = (clipboard != null); 
            if (cName.StartsWith("btL"))
            {
                // Bouton Lettre
                cmsButType = "btL";
                cmsButNr = int.Parse(cName.Substring(3, cName.Length - 3));
                char c;
                _ = theConf.pBDQ.GetCfForPBDQButton(cmsButNr, out c);
                if (c != PBDQConfig.inactiveLetter)
                {
                    tsmiCouper.Enabled = true;
                    tsmiCopier.Enabled = true;
                    tsmiEffacer.Enabled = true;
                }
            }
            else if (cName.StartsWith("btSC") || cName.StartsWith("pbHL"))
            {
                cmsButType = "btSC";
                cmsButNr = int.Parse(cName.Substring(4, cName.Length - 4));
                if (theConf.sylConf.GetSylButtonConfFor(cmsButNr).buttonClickable)
                {
                    if (!theConf.sylConf.ButtonIsActivableOne(cmsButNr))
                    {
                        tsmiCopier.Enabled = true;
                        if (theConf.sylConf.ButtonIsLastActive(cmsButNr))
                        {
                            tsmiCouper.Enabled = true;
                            tsmiEffacer.Enabled = true;
                        }
                    }
                } 
                else
                {
                    tsmiColler.Enabled = false;
                }
            } else if (cName.StartsWith("btn"))
            {
                cmsButType = "btn";
                cmsButSon = cName.Substring(3, cName.Length - 3);
                if (theConf.colors[pct].GetCheck(cmsButSon))
                {
                    tsmiCouper.Enabled = true;
                    tsmiCopier.Enabled = true;
                    tsmiEffacer.Enabled = true;
                }
            }
        }

        // tsmi => ToolStripMenuItem

        private void tsmiCouper_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("tsmiCouper_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            tsmiCopier_Click(sender, e);
            tsmiEffacer_Click(sender, e);
        }

        private void tsmiCopier_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("tsmiCopier_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            switch (cmsButType)
            {
                case "btL":
                    clipboard = theConf.pBDQ.GetCfForPBDQButton(cmsButNr, out _);
                    break;
                case "btSC":
                    clipboard = theConf.sylConf.GetSylButtonConfFor(cmsButNr).cf;
                    break;
                case "btn":
                    clipboard = theConf.colors[pct].Get(cmsButSon);
                    break;
            }
        }

        private void tsmiColler_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("tsmiColler_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            switch (cmsButType)
            {
                case "btL":
                    char c;
                    _ = theConf.pBDQ.GetCfForPBDQButton(cmsButNr, out c);
                    if (c == PBDQConfig.inactiveLetter)
                    {
                        c = lettersToPaste[countPasteLetters];
                        countPasteLetters = (countPasteLetters + 1) % lettersToPaste.Length;
                    }
                    theConf.pBDQ.UpdateLetter(cmsButNr, c, clipboard);
                    break;
                case "btSC":
                    theConf.sylConf.SylButtonModified(cmsButNr, clipboard);
                    break;
                case "btn":
                    theConf.colors[pct].SetCbxAndCF(cmsButSon, clipboard);
                    break;
            }
        }

        private void tsmiEffacer_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("tsmiEffacer_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            switch (cmsButType)
            {
                case "btL":
                    _ = theConf.pBDQ.UpdateLetter(cmsButNr, PBDQConfig.inactiveLetter, clipboard);
                    break;
                case "btSC":
                    Debug.Assert(theConf.sylConf.GetSylButtonConfFor(cmsButNr).buttonClickable);
                    Debug.Assert(theConf.sylConf.ButtonIsLastActive(cmsButNr));
                    theConf.sylConf.ClearButton(cmsButNr);
                    break;
                case "btn":
                    theConf.colors[pct].ClearSon(cmsButSon);
                    break;
            }
        }
        
        private void configMuettesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.ConditionalTrace("configMuettesToolStripMenuItem_Click pct == {0}", pct.ToString());
            if (pct == PhonConfType.phonemes)
            {
                pct = PhonConfType.muettes;
                configMuettesToolStripMenuItem.Text = "Config Phonèmes";
                configMuettesToolStripMenuItem.Image = Properties.Resources.phon;
                btcPhons.Image = Properties.Resources.l_muettes_26;
                ttipLettreEnNoir.SetToolTip(btcPhons, "Coloriser les lettres muettes");
            }
            else
            {
                pct = PhonConfType.phonemes;
                configMuettesToolStripMenuItem.Text = "Config Muettes";
                configMuettesToolStripMenuItem.Image = Properties.Resources.l_muettes_16;
                btcPhons.Image = Properties.Resources.phon;
                ttipLettreEnNoir.SetToolTip(btcPhons, "Coloriser les phonèmes");
            }
            UpdateAllSoundCbxAndButtons();
        }

        
    }
}
