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
    public delegate void ExecuteCommand(Config conf);

    public partial class ConfigControl : UserControl
    {
        // on aurait pu régler ça avec des évènements si on aviat maîtrisé le concept à cette époque...
        public static ExecuteCommand colSylSelLetters { set; private get; }
        public static ExecuteCommand colMotsSelLetters { set; private get; }
        public static ExecuteCommand colLignesSelText { set; private get; }
        public static ExecuteCommand colVoyConsSelText { set; private get; }
        public static ExecuteCommand colNoirSelText { set; private get; }
        public static ExecuteCommand colorizeAllSelPhons { set; private get; }
        public static ExecuteCommand colMuettesSelText { set; private get; }
        public static ExecuteCommand markSelLetters { set; private get; }
        public static ExecuteCommand colDuoSelText { set; private get; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static CharFormatting clipboard = null;

        private enum FontFormat {
            standard = 0,
            bold = 1,
            italic = 2,
            boldItalic = 3,
            underline = 4,
            boldUnderline = 5,
            italicUnderline = 6,
            boldItalicUnderline = 7,

            last = 8
        }

        private static Font[] fonts = new Font[(int)FontFormat.last]
        {
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))), // standard = 0
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),    // bold = 1
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(0))),  // italic = 2
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, ((byte)(0))), // boldItalic = 3,
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point, ((byte)(0))), // underline = 4
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, ((byte)(0))), // boldUnderline = 5
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point, ((byte)(0))), // italicUnderline = 6
            new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point, ((byte)(0))) // boldItalicUnderline = 7
        };

        public static void Init()
        {
            logger.ConditionalDebug("Init");
            StaticColorizControls.Init();
            WaitingForm.Init();
        }

        private class SonInfo
        {
            public CheckBox cbx { get; set; }
            public Button btn { get; set; }
            public RGB btnOrigColor { get; set; }

            // public SonInfo() {}
        }

        /// <summary>
        /// Facteur avec lequel il faut multiplier les grandeurs en pixels pour s'adapter à l'écran de l'utilisateur.
        /// </summary>
        public double ScaleFactor { get; private set; }

        private Object theWin; // La fenêtre dans laquelle le contrôle est placé.
        private Object theDoc; // le document ouvert dans la fenêtre.
        private Config theConf; // la configuration dont le contrôle est le GUI

        private Dictionary<string, CheckBox> formattingCheckBoxes; // les checkboxes pour 'unsetbehaviour' de l'onglet avancé.
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
        private CharFormatting cmsCF; // contient le CharFormatting du bouton cliqué droit.
        private int countPasteLetters; // pour itérer à traver lettersToPaste quand on colle sur une lettre vide.
        private const string lettersToPaste = @"ƨ$@#<>*%()?![]{},.;:/\-_§°~¦|";

        /// <summary>
        /// Ordonne au <c>ConfigControl</c> d'éditer une autre <c>Config</c>. Ajuste les affichages aux nouvelles valeurs.
        /// </summary>
        /// <param name="newConfig">La nouvelle <c>Config</c> qu'il s'agit d'éditer.</param>
        public void ResetConfig(Config newConfig)
        {
            logger.ConditionalDebug("ResetConfig");
            InitializeTheConf(newConfig);
            UpdateAll();
        }
        
        /// <summary>
        /// Crée un <c>ConfiControl</c> pour <c>subConf</c>. Il s'agit d'une "sub Config" qui n'est pas attachée
        /// à une fenêtre mais à une autre <c>Config</c> "mère".
        /// </summary>
        /// <param name="subConf">La <c>Config</c> pour laquelle le <c>ConfigControl</c> est créé. </param>
        public ConfigControl(Config subConf)
        {
            logger.ConditionalDebug("ConfigControl - constructeur avec subConf");
            theWin = null;
            theDoc = null;
            InitCtor("", subConf);
            // Ne pas afficher l'onglet "A propos"
            tabControl1.Controls.Remove(tabAPropos);

            // Ne pas afficher le bouton "button 1"
            butConfigDuo.Visible = false;
            butConfigDuo.Enabled = false;
            butExecuteDuo.Visible = false;
            butExecuteDuo.Enabled = false;

            // Les commandes fonctionnent depuis un ConfiControl qui correspond à une "subConfig".
            // Mais... Comme nous ouvrons la fenêtre "DuoConfForm" dans un "dialogue modal" le résultat 
            // n'est appliqué au texte qu'un fois la fenêtre "DuoConfForm" fermée. C'est trop perturbant
            // pour un utilisateur lambda. Il faut donc soit ouvrir "DuoConfForm" dans un dialogue non
            // modal, ce qui serait également très perturbant ou renoncer à ces commandes ici...

            btcPhons.Enabled = false;
            btcLNoir2.Enabled = false;
            btcLbpdq.Enabled = false;
            btSAppliquer.Enabled = false;
            btSMots.Enabled = false;
            btZeLignes.Enabled = false;
            btSVoyCons.Enabled = false;
            btcLNoir.Enabled = false;

            tabControl1.SelectTab(tabAutres);
            logger.ConditionalDebug("ConfigControl - EXIT constructeur avec subConf");
        }

        public ConfigControl(Object inWin, Object inDoc, string version)
        {
            logger.ConditionalDebug("ConfigControl - constructeur avec win et doc");
            // theConf
            theWin = inWin;
            theDoc = inDoc;
            string errMsg;
            InitCtor(version, Config.GetConfigFor(theWin, theDoc, out errMsg));
            if (!string.IsNullOrEmpty(errMsg))
                MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            logger.ConditionalDebug("ConfigControl - EXIT constructeur avec win et doc");
        }

        private void UpdateSonButton(string son)
        {
            logger.ConditionalTrace("UpdateSonButton son: {0}", son);

            SonInfo si = sonInfos[son];
            RGB btnColor = si.btnOrigColor;
            if (si.cbx.Checked)
            {
                CharFormatting cf = theConf.colors[pct].GetCF(son);
                SetButtonFont(si.btn, cf);
                if (cf.changeColor)
                    btnColor = cf.color;
            }
            else
            {
                SetButtonFontStandard(si.btn);
            }
            SetButtonColor(si.btn, btnColor);
        }

        private void UpdateCbxSon(string son)
        {
            logger.ConditionalTrace("UpdateCbxSon son: {0}", son);
            sonInfos[son].cbx.Checked = theConf.colors[pct].GetCheck(son);
            UpdateSonButton(son);
        }

        private void UpdateAllSoundCbxAndButtons()
        {
            logger.ConditionalDebug("UpdateAllSoundCbxAndButtons");
            SuspendLayout();

            foreach (string son in sonInfos.Keys)
            {
                // checkbox
                UpdateCbxSon(son);
                // button
                UpdateSonButton(son);
            }
            cbSBlackPhons.Checked = (theConf.colors[pct].defBeh == ColConfWin.DefBeh.noir);
            ResumeLayout();
        }

        private void UpdateLetterButtons()
        {
            logger.ConditionalDebug("UpdateLetterButtons");
            SuspendLayout();
            for (int i = 0; i < letterButtons.Count; i++)
                UpdateLetterButton(i);
            cbAlettresNoir.Checked = theConf.pBDQ.markAsBlack;
            ResumeLayout();
        }

        private void UpdateLetterButton(int buttonNr)
        {
            logger.ConditionalTrace("UpdateLetterButton buttonNr: {0}", buttonNr);
            char c;
            CharFormatting cf = theConf.pBDQ.GetCfForPBDQButton(buttonNr, out c);
            StringBuilder sb = new StringBuilder(1);
            sb.Append(c);
            letterButtons[buttonNr].Text = sb.ToString();
            RGB theButtonCol = defaultLetterButtonCol;
            if (cf != null)
            {
                SetButtonFont(letterButtons[buttonNr], cf);
                if (cf.changeColor)
                    theButtonCol = cf.color;
            }
            SetButtonColor(letterButtons[buttonNr], theButtonCol);
        }

        private void UpdateSylModeButtons()
        {
            rbnEcrit.Checked = theConf.sylConf.mode == SylConfig.Mode.ecrit;
            rbnOral.Checked = theConf.sylConf.mode == SylConfig.Mode.oral;
            rbnPoesie.Checked = theConf.sylConf.mode == SylConfig.Mode.poesie;
            UpdateDiereseControls();
        }

        private void UpdateMarquerMuettesButton()
        {
            cbMuettesSyl.Checked = theConf.sylConf.marquerMuettes;
        }

        private void UpdateNrPieds()
        {
            logger.ConditionalDebug("UpdateNrPieds: {0}", theConf.sylConf.nbrPieds);
            comboBoxNrPieds.SelectedIndex = theConf.sylConf.nbrPieds;
        }

        private void UpdateChercherDierese()
        {
            logger.ConditionalDebug("UpdateChercherDierese: {0}", theConf.sylConf.chercherDierese);
            checkBoxDierese.Checked = theConf.sylConf.chercherDierese;
            comboBoxNrPieds.Enabled = theConf.sylConf.chercherDierese;
            UpdateNrPieds();
        }

        private void UpdateDiereseControls()
        {
            logger.ConditionalDebug("UpdateDiereseControls");
            groupBoxPoesie.Enabled = theConf.sylConf.mode == SylConfig.Mode.poesie;
            UpdateChercherDierese();
        }

        private void UpdateSylButtons ()
        {
            logger.ConditionalDebug("UpdateLetterButtons");
            SuspendLayout();
            for (int i = 0; i < SylConfig.NrButtons; i++)
                UpdateSylButton(i);

            rbnAv2Cons.Checked = !theConf.sylConf.DoubleConsStd;
            rbnStandard.Checked = theConf.sylConf.DoubleConsStd;
            UpdateSylModeButtons();
            UpdateMarquerMuettesButton();
            ResumeLayout();
        }

        private void UpdateSylButton(int butNr)
        {
            logger.ConditionalTrace("UpdateSylButton buttonNr: {0}", butNr);
            const string filledBtnTxt = "Txt";
            const string emptyBtnTxt = "";
            SylConfig.SylButtonConf sbC = theConf.sylConf.GetSylButtonConfFor(butNr);
            Button theButton = sylButtons[butNr];
            RGB theButtonCol = defaultSylButtonCol;
            RGB thePbxCol = defaultSylButtonCol;
            string theBtnTxt = emptyBtnTxt;
            if ((sbC.cf != null))
            {
                if (sbC.cf.changeColor)
                {
                    theButtonCol = sbC.cf.color;
                    theBtnTxt = filledBtnTxt;
                }
                if (sbC.cf.changeHilight)
                    thePbxCol = sbC.cf.hilightColor;
                else
                    thePbxCol = theButtonCol;
                SetButtonFont(theButton, sbC.cf);
            }
            theButton.Text = theBtnTxt;
            SetButtonColor(theButton, theButtonCol);
            sylPictureBoxes[butNr].BackColor = thePbxCol;
            theButton.Enabled = sbC.buttonClickable;
            sylPictureBoxes[butNr].Enabled = sbC.buttonClickable;
        }


        private void UpdateUcheckBoxes()
        {
            logger.ConditionalDebug("UpdateUcheckBoxes");
            foreach (string cbUname in formattingCheckBoxes.Keys)
            {
                CheckBox fcb = formattingCheckBoxes[cbUname];
                fcb.Checked = theConf.unsetBeh.GetCbuFlag(cbUname);
            }
        }

        private void UpdateIllRadioB()
        {
            logger.ConditionalDebug("UpdateUcheckBoxes");
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

        private void IllConfigModified(object sender, PhonConfModifiedEventArgs e)
        {
            logger.ConditionalDebug("IllConfigModified");
            if (e.pct == this.pct)
            {
                Debug.Assert((ColConfWin)sender == theConf.colors[pct]);
                UpdateIllRadioB();
            }
        }

        public void UpdateAll()
        {
            logger.ConditionalDebug("UpdateAll");
            UpdateAllSoundCbxAndButtons();
            UpdateLetterButtons();
            UpdateUcheckBoxes();
            UpdateSylButtons();
            UpdateIllRadioB();
            UpdateConfigName();
        }

        /// <summary>
        /// établit le lien entre le contrôle et la config en capturant les évènements nécessaires. Définit
        /// également <c>theConf</c>.
        /// </summary>
        /// <param name="inConf">La <c>Config</c> pour laquelle il faut initialiser les évèenements</param>
        private void InitializeTheConf(Config inConf)
        {
            logger.ConditionalDebug("InitializeTheConf");
            theConf = inConf;
            theConf.ConfigReplacedEvent += ConfigReplaced;
            theConf.ConfigNameModifiedEvent += ConfigNameModified;
            theConf.colors[PhonConfType.phonemes].SonCharFormattingModifiedEvent += SonButtonCFModified;
            theConf.colors[PhonConfType.phonemes].SonCBModifiedEvent += SonCheckBoxModified;
            theConf.colors[PhonConfType.phonemes].IllModifiedEvent += IllConfigModified;
            theConf.colors[PhonConfType.phonemes].DefBehModifiedEvent += DefBehModified;
            theConf.colors[PhonConfType.muettes].SonCharFormattingModifiedEvent += SonButtonCFModified;
            theConf.colors[PhonConfType.muettes].SonCBModifiedEvent += SonCheckBoxModified;
            theConf.colors[PhonConfType.muettes].IllModifiedEvent += IllConfigModified;
            theConf.colors[PhonConfType.muettes].DefBehModifiedEvent += DefBehModified;
            theConf.pBDQ.LetterButtonModifiedEvent += LetterButtonModified;
            theConf.pBDQ.MarkAsBlackModifiedEvent += MarkAsBlackModified;
            theConf.sylConf.SylButtonModifiedEvent += this.SylButtonModified;
            theConf.sylConf.ModeModifiedEvent += SylModeModified;
            theConf.sylConf.DoubleConsStdModifiedEvent += DoubleConsStdModified;
            theConf.sylConf.MarquerMuettesModified += MarquerMuettesModified;
            theConf.sylConf.ChercherDiereseModified += HandleChercherDiereseModified;
            theConf.sylConf.NbrPiedsModified += HandleNbrPiedsModified;
            theConf.unsetBeh.CheckboxUnsetModifiedEvent += CheckboxUnsetModified;
        }

        private void ConfigReplaced(object sender, ConfigReplacedEventArgs e)
        {
            logger.ConditionalDebug("ConfigReplaced");
            // Se désabonner des anciens évènements. C'est peut-être inutile, je ne suis pas sûr...
            theConf.ConfigReplacedEvent -= ConfigReplaced;
            theConf.ConfigNameModifiedEvent -= ConfigNameModified;
            theConf.colors[PhonConfType.phonemes].SonCharFormattingModifiedEvent -= SonButtonCFModified;
            theConf.colors[PhonConfType.phonemes].SonCBModifiedEvent -= SonCheckBoxModified;
            theConf.colors[PhonConfType.phonemes].IllModifiedEvent -= IllConfigModified;
            theConf.colors[PhonConfType.phonemes].DefBehModifiedEvent -= DefBehModified;
            theConf.colors[PhonConfType.muettes].SonCharFormattingModifiedEvent -= SonButtonCFModified;
            theConf.colors[PhonConfType.muettes].SonCBModifiedEvent -= SonCheckBoxModified;
            theConf.colors[PhonConfType.muettes].IllModifiedEvent -= IllConfigModified;
            theConf.colors[PhonConfType.muettes].DefBehModifiedEvent -= DefBehModified;
            theConf.pBDQ.LetterButtonModifiedEvent -= LetterButtonModified;
            theConf.pBDQ.MarkAsBlackModifiedEvent -= MarkAsBlackModified;
            theConf.sylConf.SylButtonModifiedEvent -= SylButtonModified;
            theConf.sylConf.ModeModifiedEvent -= SylModeModified;
            theConf.sylConf.DoubleConsStdModifiedEvent -= DoubleConsStdModified;
            theConf.sylConf.MarquerMuettesModified -= MarquerMuettesModified;
            theConf.sylConf.ChercherDiereseModified -= HandleChercherDiereseModified;
            theConf.sylConf.NbrPiedsModified -= HandleNbrPiedsModified;
            theConf.unsetBeh.CheckboxUnsetModifiedEvent -= CheckboxUnsetModified;

            // Initialiser les handlers
            InitializeTheConf(e.newConfig);

            // Resynchroniser l'affichage
            UpdateAll();
        }

        /// <summary>
        /// Appelé par les constructeurs pour les initialisations communes aux différents cas. 
        /// <param name="version">Le numéro de version à utiliser pour l'affichage si la version ne
        /// peut pas être récupérée dans le fichier de déployement.</param>
        /// <param name="inConf">La <see cref="Config"/> pour laquelle le <c>ConfigControl</c>
        /// est créé.</param>
        private void InitCtor(string version, Config inConf)
        {
            logger.ConditionalDebug("InitCtor");
            InitializeComponent(); // calls the setup of the whole component
            InitializeTheConf(inConf);
            Config.ListSavedConfigsModified += ListSavedConfigsModifed;

            // Compute ScaleFacor
            double dimWidth;
            if (AutoScaleMode == AutoScaleMode.Dpi)
                dimWidth = 96; // value observed on the development machine
            else if (AutoScaleMode == AutoScaleMode.Font)
                dimWidth = 6; // value observed on the development machine
            else
            {
                dimWidth = AutoScaleDimensions.Width;
                logger.Warn("Unexpected AutoScaleMode encountered. Scaling may not work properly.");
            }
            ScaleFactor = CurrentAutoScaleDimensions.Width / dimWidth;
            logger.Info("CurrentAutoScaleDimensions.Width == {0}", CurrentAutoScaleDimensions.Width);
            logger.Info("AutoScaleDimensions.Width == {0}", AutoScaleDimensions.Width);
            logger.Info("factor == {0}", ScaleFactor);
            logger.Info("AutoScaleMode is {0}", AutoScaleMode.ToString());

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
            sylButtons = new Button[SylConfig.NrButtons];
            sylPictureBoxes = new PictureBox[SylConfig.NrButtons];
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
            logger.ConditionalTrace("SetButtonColor, bouton \'{0}\'", b.Name);
            b.BackColor = color;
            if (color.Dark())
                b.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.white];
            else
                b.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.black];
        }

        /// <summary>
        /// Assigne le font défini par <c>cf</c> au bouton <c>b</c>.
        /// </summary>
        /// <param name="b">Le bouton dont le font doit être adapté.</param>
        /// <param name="cf">Le <c>CharFormatting</c> définissant le font à utiliser.</param>
        private void SetButtonFont (Button b, CharFormatting cf)
        {
            logger.ConditionalTrace("SetButtonFont bouton \'{0}\'", b.Name);
            int fontIndex = 0;
            if (cf.bold)
                fontIndex += 1;
            if (cf.italic)
                fontIndex += 2;
            if (cf.underline)
                fontIndex += 4;
            b.Font = fonts[fontIndex];
        }

        /// <summary>
        /// Met le font standard pour le bouton <c>b</c>.
        /// </summary>
        /// <param name="b">Le bouton dont il faut modifier le font. </param>
        private void SetButtonFontStandard(Button b) => b.Font = fonts[(int)FontFormat.standard];

        //--------------------------------------------------------------------------------------------
        // -------------------------- Boutons généraux phonèmes---------------------------------------
        //--------------------------------------------------------------------------------------------

        private void btnCERAS_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnCERAS_Click");
            theConf.colors[pct].SetCeras();
        }

        private void btGCerasRose_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnCERAS_Click");
            theConf.colors[pct].SetCerasRose();
        }

        private void btnTout_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnTout_Click");
            theConf.colors[pct].SetAllCbxSons();
        }

        private void btnRien_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnRien_Click");
            theConf.colors[pct].ClearAllCbxSons();
        }

        private void btcPhons_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btcPhons_Click");
            if (pct == PhonConfType.phonemes)
            {
                colorizeAllSelPhons(theConf);
            }
            else
            {
                colMuettesSelText(theConf);
            }
        }

        private void SonButtonCFModified(object sender, SonConfigModifiedEventArgs e)
        {
            logger.ConditionalDebug("SonButtonCFModified");
            if (e.pct == this.pct)
            {
                Debug.Assert((ColConfWin)sender == theConf.colors[pct]);
                UpdateSonButton(e.son);
            }
        }

        private void SonCheckBoxModified(object sender, SonConfigModifiedEventArgs e)
        {
            logger.ConditionalDebug("SonCheckBoxModified");
            if (e.pct == this.pct)
            {
                Debug.Assert((ColConfWin)sender == theConf.colors[pct]);
                UpdateCbxSon(e.son);
            }
        }

        //--------------------------------------------------------------------------------------------
        // ----------------------------- CheckBoxes sons ---------------------------------------------
        //--------------------------------------------------------------------------------------------

        private void SonCheckBox_CheckedChanged(Object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;
            logger.ConditionalDebug("SonCheckBox_CheckedChanged {0}", cbx.Name);
            Debug.Assert(cbx.Name.StartsWith("cbx"));
            string son = cbx.Name.Substring(3, cbx.Name.Length - 3);
            theConf.colors[pct].SetChkSon(son, cbx.Checked);
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------------- Boutons sons ---------------------------------------------
        //--------------------------------------------------------------------------------------------

        private void SonButton_Click(object sender, EventArgs e)
        {
            Button theBtn = (Button)sender;
            logger.ConditionalDebug("SonButton_Click {0}", theBtn.Name);
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            Debug.Assert(theBtn.Name.StartsWith("btn"));
            string son = theBtn.Name.Substring(3, theBtn.Name.Length - 3);
            CharFormatForm form = new CharFormatForm(theConf.colors[pct].GetCF(son), son, 
                theConf.colors[pct].SetCFSon);
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
            logger.ConditionalDebug("SonCheckBox_CheckedChanged {0}", theBtn.Name);
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
            logger.ConditionalDebug("btCPBDQ_Click");
            theConf.pBDQ.Reset();
        }

        private void btcLbpdq_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btcLbpdq_Click");
            markSelLetters(theConf);
        }

        private void LetterButtonModified(object sender, LetterButtonModifiedEventArgs e)
        {
            UpdateLetterButton(e.buttonNr);
        }

        //--------------------------------------------------------------------------------------------
        // -------------------------------- advanced checkboxes --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void cbAlettresNoir_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("cbAlettresNoir_CheckedChanged");
            Debug.Assert(sender != null);
            CheckBox cbx = (CheckBox)sender;
            theConf.pBDQ.SetMarkAsBlackTo(cbx.Checked);
        }

        private void MarkAsBlackModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("MarkAsBlackModified");
            cbAlettresNoir.Checked = theConf.pBDQ.markAsBlack;
        }

        private void cbSBlackPhons_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("cbSBlackPhons_CheckedChanged");
            Debug.Assert(sender != null);
            CheckBox cbx = (CheckBox)sender;
            if (cbx.Checked)
                theConf.colors[pct].SetDefaultBehaviourTo(ColConfWin.DefBeh.noir);
            else
                theConf.colors[pct].SetDefaultBehaviourTo(ColConfWin.DefBeh.transparent);
        }

        private void DefBehModified(object sender, PhonConfModifiedEventArgs e)
        {
            logger.ConditionalDebug("DefBehModified e.pct: \'{0}\'", e.pct);
            if (e.pct == this.pct)
            {
                cbSBlackPhons.Checked = (theConf.colors[pct].defBeh == ColConfWin.DefBeh.noir);
            }
        }

        private void UcheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbu = (CheckBox)sender;
            logger.ConditionalDebug("UcheckBoxes_CheckedChanged {0}", cbu.Name);
            Debug.Assert(cbu.Name.StartsWith("cbu"));
            string cbuNameEnd = cbu.Name.Substring(3, cbu.Name.Length - 3);
            theConf.unsetBeh.SetCbuFlag(cbuNameEnd, cbu.Checked);
        }

        private void CheckboxUnsetModified (object sender, CheckboxUnsetModifiedEventArgs e)
        {
            logger.ConditionalDebug("CheckboxUnsetModified, checkbox \'{0}\'", e.unsetCBName);
            Debug.Assert(ReferenceEquals(sender, theConf.unsetBeh));
            CheckBox fcb = formattingCheckBoxes[e.unsetCBName];
            fcb.Checked = theConf.unsetBeh.GetCbuFlag(e.unsetCBX);
        }

        //--------------------------------------------------------------------------------------------
        // ---------------------------------- RadioButton "ill" --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void rbnIll_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbnIll_CheckedChanged, position \'{0}\'", rbnIllCeras.Checked);
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

        private void rbnEcrit_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbnEcrit_CheckedChanged");
            if(rbnEcrit.Checked)
            {
                theConf.sylConf.mode = SylConfig.Mode.ecrit;
            }
        }

        private void rbnOral_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbnOral_CheckedChanged");
            if (rbnOral.Checked)
            {
                theConf.sylConf.mode = SylConfig.Mode.oral;
            }
        }

        private void rbnPoesie_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbnPoesie_CheckedChanged");
            if (rbnPoesie.Checked)
            {
                theConf.sylConf.mode = SylConfig.Mode.poesie;
            }
        }

        private void SylModeModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("SylModeModified");
            Debug.Assert(ReferenceEquals(sender, theConf.sylConf));
            UpdateSylModeButtons();
        }

        private void rbnStandard_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbnStandard_CheckedChanged");
            theConf.sylConf.DoubleConsStd = rbnStandard.Checked;
        }

        private void DoubleConsStdModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("DoubleConsStdModified");
            Debug.Assert(ReferenceEquals(sender, theConf.sylConf));
            rbnStandard.Checked = theConf.sylConf.DoubleConsStd;
        }

        private void cbMuettesSyl_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("DoubleConsStdModified");
            theConf.sylConf.marquerMuettes = cbMuettesSyl.Checked;
        }

        private void MarquerMuettesModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("MarquerMuettesModified");
            UpdateMarquerMuettesButton();
        }


        //--------------------------------------------------------------------------------------------
        // ------------------------------------ Boutons Syllabes -------------------------------------
        //--------------------------------------------------------------------------------------------

        private void btSAppliquer_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSAppliquer_Click");
            colSylSelLetters(theConf);
        }

        private void btSMots_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSMots_Click");
            colMotsSelLetters(theConf);
        }

        private void btZeLignes_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btZeLignes_Click");
            colLignesSelText(theConf);
        }

        private void btSVoyCons_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSVoyCons_Click");
            colVoyConsSelText(theConf);
        }

        private void btcLNoir_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btcLNoir_Click");
            colNoirSelText(theConf);
        }

        private void btcInitSyls_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btcInitSyls_Click");
            theConf.sylConf.Reset();
        }

        private void SylButton_Click(object sender, EventArgs e)
            // can be called for the Buttons and for the PictureBoxes
        {
            logger.ConditionalDebug("SylButton_Click");
            Control theControl = (Control)sender;
            logger.ConditionalDebug("SylButton_Click {0}", theControl.Name);
            Point mousePos = theControl.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            Debug.Assert(theControl.Name.StartsWith("btSC") || theControl.Name.StartsWith("pbHL"));
            string cNrTxt = theControl.Name.Substring(4, theControl.Name.Length - 4);
            int cNr = int.Parse(cNrTxt);
            if (theConf.sylConf.GetSylButtonConfFor(cNr).buttonClickable)
            {
                CharFormatForm form = new SylFormatForm(theConf.sylConf.GetSylButtonConfFor(cNr).cf,
                cNrTxt, theConf.sylConf.SetSylButtonCF);
                mousePos.Offset(-form.Width, -(form.Height / 2));
                form.Location = mousePos;
                _ = form.ShowDialog();
                form.Dispose();
            }
        }

        private void SylButtonModified(object sender, SylButtonModifiedEventArgs e)
        {
            logger.ConditionalDebug("SylButtonModified, bouton \'{0}\'", e.buttonNr);
            Debug.Assert((SylConfig)sender == theConf.sylConf);
            UpdateSylButton(e.buttonNr);
        }

        private void checkBoxDierese_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("checkBoxDierese_CheckedChanged: {0}", checkBoxDierese.Checked);
            theConf.sylConf.chercherDierese = checkBoxDierese.Checked;
        }

        private void comboBoxNrPieds_SelectedIndexChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("checkBoxDierese_CheckedChanged: {0}", comboBoxNrPieds.SelectedIndex);
            theConf.sylConf.nbrPieds = comboBoxNrPieds.SelectedIndex;
        }

        private void HandleChercherDiereseModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("HandleChercherDiereseModified");
            UpdateChercherDierese();
        }

        private void HandleNbrPiedsModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("HandleNbrPiedsModified");
            UpdateNrPieds();
        }


        //--------------------------------------------------------------------------------------------
        // --------------------------------------- Boutons Duo ---------------------------------------
        //--------------------------------------------------------------------------------------------

        private void butConfigDuo_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("butConfigDuo_Click");
            DuoConfForm dcf = new DuoConfForm(theConf);
            dcf.ShowDialog();
        }

        private void butExecuteDuo_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("butExecuteDuo_Click");
            colDuoSelText(theConf);
        }

        //--------------------------------------------------------------------------------------------
        // --------------------------------------  Onglet Sauv. --------------------------------------
        //--------------------------------------------------------------------------------------------

        private void tabSauv_Enter(object sender, EventArgs e)
        {
            // Quand l'utilisateur rend l'onglet visible
            logger.ConditionalDebug("tabSauv_Enter");
            UpdateListeConfigs();
        }

        private void UpdateConfigName()
        {
            logger.ConditionalDebug("UpdateConfigName");
            txtBNomConfig.Text = theConf.GetConfigName();
            btSauvSauv.Enabled = (txtBNomConfig.Text.Length > 0);
        }

        private void ConfigNameModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("ConfigNameModified");
            UpdateConfigName();
        }

        private void UpdateListeConfigs()
        {
            logger.ConditionalDebug("UpdateListeConfigs");
            lbConfigs.DataSource = Config.GetSavedConfigNames();
            btSauvCharger.Enabled = (lbConfigs.Items.Count > 0);
            if (!String.IsNullOrEmpty(txtBNomConfig.Text))
            {
                int pos = lbConfigs.FindString(txtBNomConfig.Text);
                if (pos != ListBox.NoMatches)
                {
                    lbConfigs.SetSelected(pos, true);
                }
            }
        }

        private void ListSavedConfigsModifed(object sender, EventArgs e)
        {
            logger.ConditionalDebug("ListSavedConfigsModifed");
            UpdateListeConfigs();
        }

        // --------------- txtBNomConfig : Text Box contenant le nom de la config ------------------------
        
        private void txtBNomConfig_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalDebug("txtBNomConfig_KeyPress: \'{0}\'", e.KeyChar);
            const string forbiddenChars = @"<>:/\|?*" + "\"";
            if (forbiddenChars.Contains(e.KeyChar))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"Les caractères /\?%*:|<>");
                sb.Append("\" ne peuvent pas être utilisés dans le nom d'une configuration.");
                MessageBox.Show(sb.ToString(), ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            logger.ConditionalDebug("txtBNomConfig_KeyUp - nbre caractères: {0}", txtBNomConfig.Text.Length);
            // En fonction du nombre de caractères que contient le nom de la config, on peut activer ou désactiver 
            // le bouton de sauvegarde.
            btSauvSauv.Enabled = (txtBNomConfig.Text.Length > 0);
        }

        
        // --------------------------------- btSauvSauv : Bouton "Sauver" ------------------ ------------------------

        private void btSauvSauv_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSauvSauv_Click");
            if (txtBNomConfig.Text.Length > 0)
            {
                bool doIt = true;
                if (lbConfigs.FindStringExact(txtBNomConfig.Text) != ListBox.NoMatches)
                {
                    string message = String.Format(
                        "Un configuration poartant le nom \'{0}\' est déjà enregistrée. Souhaitez-vous le remplacer?",
                        txtBNomConfig.Text);
                    var result = MessageBox.Show(message, ConfigBase.ColorizationName, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    doIt = (result == DialogResult.Yes);
                }
                if (doIt)
                {
                    string msgTxt;
                    if (!theConf.SaveConfig(txtBNomConfig.Text, out msgTxt))
                    {
                        string message = String.Format("Impossible de sauvegarder la configuration \'{0}\'. Erreur: {1}", 
                            txtBNomConfig.Text, msgTxt);
                        MessageBox.Show(message, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            logger.ConditionalDebug("btSauvSauv_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvSauv.PerformClick();
                e.Handled = true;
            }
        }


        // --------------- lbConfigs : ListBox contenant la liste des configs sauvegardées ------------------------

        private void lbConfigs_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalDebug("lbConfigs_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvCharger.PerformClick();
                e.Handled = true;
            }
        }

        private void lbConfigs_DoubleClick(object sender, EventArgs e)
        {
            logger.ConditionalDebug("lbConfigs_DoubleClick");
            btSauvCharger.PerformClick();
        }

        // --------------------------------- btSauvCharger : Bouton "Charger" ------------------ ------------------------

        private void btSauvCharger_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSauvCharger_Click");
            string configName = lbConfigs.SelectedItem.ToString();
            string errMsg;
            if(!theConf.LoadConfig(configName, out errMsg))
            {
                string message = String.Format("Impossible de charger la configuration \'{0}\'. Erreur: {1}",
                    configName, errMsg);
                MessageBox.Show(message, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSauvCharger_KeyPress(object sender, KeyPressEventArgs e)
        {
            logger.ConditionalDebug("btSauvCharger_KeyPress: \'{0}\'", e.KeyChar);
            if (e.KeyChar == '\r')
            {
                btSauvCharger.PerformClick();
                e.Handled = true;
            }
        }

        // --------------------------------- btSauv : Bouton "Effacer" ------------------ ------------------------

        private void btSauvEffacer_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSauvCharger_Click");
            string configName = lbConfigs.SelectedItem.ToString();
            string message = String.Format(
                        "Voulez-vous vraiment effacer la configuration \'{0}\' ? Cette opération est " +
                        "irréversible",
                        configName);
            var result = MessageBox.Show(message, ConfigBase.ColorizationName, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string errTxt;
                if (!Config.DeleteSavedConfig(configName, out errTxt))
                {
                    string errMessages = String.Format("Impossible d'effacer la configuration \'{0}\'. Erreur: {1}", configName, errTxt);
                    MessageBox.Show(errMessages, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --------------------------------- btSauv : Bouton "Par défaut" ------------------ ------------------------

        private void btSauvDefaut_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btSauvDefaut_Click");
            theConf.Reset();
            btSauvSauv.Focus();
        }

        //--------------------------------------------------------------------------------------------
        // ------------------------------------  Onglet A propos -------------------------------------
        //--------------------------------------------------------------------------------------------

        private void linkLireCouleur_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalDebug("linkLireCouleur_LinkClicked");
            this.linkLireCouleur.LinkVisited = true;
            System.Diagnostics.Process.Start(this.linkLireCouleur.Text);
        }

        private void linkCERAS_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalDebug("linkCERAS_LinkClicked");
            this.linkCERAS.LinkVisited = true;
            System.Diagnostics.Process.Start(this.linkCERAS.Text);
        }

        private void linkInfoAtColorization_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalDebug("linkInfoAtColorization_LinkClicked");
            this.linkInfoAtColorization.LinkVisited = true;
            System.Diagnostics.Process.Start("mailto:info@colorization.ch");
        }

        private void linkCodeCouleurAPI_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalDebug("linkCodeCouleurAPI_LinkClicked");
            this.linkCodeCouleurAPI.LinkVisited = true;
            System.Diagnostics.Process.Start("https://colorization.ch/docs/Sons-couleurs-symboles-et-coloriseur-API.pdf");
        }

        private void linkCodeCouleurAPIRose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            logger.ConditionalDebug("linkCodeCouleurAPIRose_LinkClicked");
            this.linkCodeCouleurAPIRose.LinkVisited = true;
            System.Diagnostics.Process.Start("https://colorization.ch/docs/Sons_couleurs_COLORISEUR_API_rose_2020.pdf");
        }


        private void butLicense_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("butLicense_Click");
            Button theBtn = (Button)sender;
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location);
            LicenseForm lf = new LicenseForm();
            p.Offset(-lf.Width, -(lf.Height / 2));
            lf.Location = p;
            _ = lf.ShowDialog();
            lf.Dispose();
        }

        private void butAide_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("butAide_Click");
            System.Diagnostics.Process.Start("https://colorization.ch/docs/Manuel_Utilisateur_Colorization.pdf");
        }

        //--------------------------------------------------------------------------------------------
        // -------------------  Context Menu Strip - Clic droit EffacerCopier ------------------------
        //--------------------------------------------------------------------------------------------

        /// <summary>
        /// Fais en sorte que les éléments du menu, correspondent au <c>CharFormatting</c> reçu en paramètre.
        /// </summary>
        /// <param name="cf">Le <c>Charformatting</c> pour les éléments du menu.</param>
        /// <param name="txt">Le texte à utiliser pour le menu de la couleur du texte.</param>
        private void SetTsmiGISforCF(CharFormatting cf, string txt = "Texte")
        {
            logger.ConditionalDebug("SetTsmiGISforCF, txt: \'{0}\'", txt);
            tsmiGras.Enabled = true;
            tsmiItalique.Enabled = true;
            tsmiSouligne.Enabled = true;
            tsmiCouleur.Enabled = true;
            if (HilightForm.CanOperate())
            {
                tsmiSurlignage.Enabled = true;
                tsmiSurlignage.Visible = true;
                tsmiSurlignage.BackColor = cf.hilightColor;
                tsmiSurlignage.Checked = cf.changeHilight;
                if (cf.hilightColor.Dark())
                    tsmiSurlignage.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.white];
                else
                    tsmiSurlignage.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.black];
            }
            else
            {
                tsmiSurlignage.Enabled = false;
                tsmiSurlignage.Visible = false;
            }
            tsmiGras.Checked = cf.bold;
            tsmiItalique.Checked = cf.italic;
            tsmiSouligne.Checked = cf.underline;
            tsmiCouleur.Text = txt;
            tsmiCouleur.BackColor = cf.color;
            if (cf.color.Dark())
                tsmiCouleur.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.white];
            else
                tsmiCouleur.ForeColor = ColConfWin.predefinedColors[(int)PredefCol.black]; 
        }

        // cms => context menu strip
        private void cmsEffacerCopier_Opening(object sender, CancelEventArgs e)
        {
            string cName = cmsEffacerCopier.SourceControl.Name; 
            logger.ConditionalDebug("cmsEffacerCopier_Opening {0}", cName);
            tsmiCouper.Enabled = false;
            tsmiCopier.Enabled = false;
            tsmiEffacer.Enabled = false;
            tsmiColler.Enabled = (clipboard != null);
            tsmiGras.Enabled = false;
            tsmiItalique.Enabled = false;
            tsmiSouligne.Enabled = false;
            tsmiCouleur.Enabled = false;
            tsmiSurlignage.Enabled = false;
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
                    cmsCF = theConf.pBDQ.GetCfForPBDQLetter(c);
                    SetTsmiGISforCF(cmsCF);
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
                    cmsCF = theConf.sylConf.GetSylButtonConfFor(cmsButNr).cf;
                    SetTsmiGISforCF(cmsCF);
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
                cmsCF = theConf.colors[pct].GetCF(cmsButSon);
                SetTsmiGISforCF(cmsCF,ColConfWin.ExampleText(cmsButSon));
            }
        }

        // tsmi => ToolStripMenuItem

        private void tsmiCouper_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiCouper_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            tsmiCopier_Click(sender, e);
            tsmiEffacer_Click(sender, e);
        }

        private void tsmiCopier_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiCopier_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
            switch (cmsButType)
            {
                case "btL":
                    clipboard = theConf.pBDQ.GetCfForPBDQButton(cmsButNr, out _);
                    break;
                case "btSC":
                    clipboard = theConf.sylConf.GetSylButtonConfFor(cmsButNr).cf;
                    break;
                case "btn":
                    clipboard = theConf.colors[pct].GetCF(cmsButSon);
                    break;
            }
        }

        private void tsmiColler_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiColler_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
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
                    theConf.sylConf.SetSylButtonCF(cmsButNr, clipboard);
                    break;
                case "btn":
                    theConf.colors[pct].SetCbxAndCF(cmsButSon, clipboard);
                    break;
            }
        }

        private void tsmiEffacer_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiEffacer_Click cmsButType == {0}, cmsButNr == {1}, cmsButSon == {2}", cmsButType, cmsButNr, cmsButSon);
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

        private void APplyCFToClickedButton(CharFormatting cf)
        {
            logger.ConditionalDebug("APplyCFToClickedButton");
            switch (cmsButType)
            {
                case "btL":
                    _ = theConf.pBDQ.UpdateLetter(cmsButNr, cf);
                    break;
                case "btSC":
                    theConf.sylConf.SetSylButtonCF(cmsButNr, cf);
                    break;
                case "btn":
                    theConf.colors[pct].SetCbxAndCF(cmsButSon, cf);
                    break;
            }
        }

        private void tsmiGras_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("APplyCFToClickedButton");
            APplyCFToClickedButton(new CharFormatting(cmsCF, !cmsCF.bold, cmsCF.italic, cmsCF.underline));
        }

        private void tsmiItalique_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("APplyCFToClickedButton");
            APplyCFToClickedButton(new CharFormatting(cmsCF, cmsCF.bold, !cmsCF.italic, cmsCF.underline));
        }

        private void tsmiSouligne_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("APplyCFToClickedButton");
            APplyCFToClickedButton(new CharFormatting(cmsCF, cmsCF.bold, cmsCF.italic, !cmsCF.underline));
        }

        private void tsmiCouleur_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiCouleur_Click");
            Point p = cmsEffacerCopier.PointToScreen(tsmiCouleur.Bounds.Location); // position relative à l'écran
            p.Offset((int)(-450 * ScaleFactor), (int)(-100 * ScaleFactor));
            var mcd = new MyColorDialog();
            mcd.CustomColors = StaticColorizControls.customColors;
            mcd.AnyColor = true;
            mcd.FullOpen = true;
            mcd.Color = cmsCF.color;
            mcd.SetPos(p);
            if (mcd.ShowDialog() == DialogResult.OK)
            {
                APplyCFToClickedButton(new CharFormatting(cmsCF, mcd.Color));
                StaticColorizControls.customColors = mcd.CustomColors;
            }
            mcd.Dispose();
        }

        private void tsmiSurlignage_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("tsmiSurlignage_Click");
            Point p = cmsEffacerCopier.PointToScreen(tsmiCouleur.Bounds.Location); // position relative à l'écran
            HilightForm hiForm = new HilightForm(cmsCF.hilightColor);
            p.Offset((int)(ScaleFactor * (-hiForm.Width)), (int)(ScaleFactor * (-(hiForm.Height / 2))));
            hiForm.Location = p;
            if (hiForm.ShowDialog() == DialogResult.OK)
            {
                APplyCFToClickedButton(new CharFormatting(cmsCF.bold, cmsCF.italic, cmsCF.underline, cmsCF.caps, cmsCF.changeColor, cmsCF.color,
                           true, hiForm.GetSelectedColor()));
            }
            hiForm.Dispose();
        }

        //--------------------------------------------------------------------------------------------
        // -------------------  Context Menu Strip - Clic droit configMuettes ------------------------
        //--------------------------------------------------------------------------------------------

        private void configMuettesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("configMuettesToolStripMenuItem_Click pct == {0}", pct.ToString());
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
