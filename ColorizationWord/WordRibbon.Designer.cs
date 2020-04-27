﻿namespace ColorizationWord
{
    partial class WordRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public WordRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            logger.ConditionalTrace("WordRibbon - Constructor");
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.buttonGroup3 = this.Factory.CreateRibbonButtonGroup();
            this.btnPhonemes = this.Factory.CreateRibbonButton();
            this.btnMuettes = this.Factory.CreateRibbonButton();
            this.btnSyls = this.Factory.CreateRibbonButton();
            this.buttonGroup1 = this.Factory.CreateRibbonButtonGroup();
            this.btnMots = this.Factory.CreateRibbonButton();
            this.btnLignes = this.Factory.CreateRibbonButton();
            this.btnBPDQ = this.Factory.CreateRibbonButton();
            this.buttonGroup2 = this.Factory.CreateRibbonButtonGroup();
            this.btnVoyCons = this.Factory.CreateRibbonButton();
            this.btnNoir = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group1.SuspendLayout();
            this.buttonGroup3.SuspendLayout();
            this.buttonGroup1.SuspendLayout();
            this.buttonGroup2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.ControlId.OfficeId = "TabHome";
            this.tab1.Groups.Add(this.group1);
            this.tab1.Label = "TabHome";
            this.tab1.Name = "tab1";
            // 
            // group1
            // 
            ribbonDialogLauncherImpl1.ScreenTip = "Configuration";
            ribbonDialogLauncherImpl1.SuperTip = "Ouvre le panneau de configuration";
            this.group1.DialogLauncher = ribbonDialogLauncherImpl1;
            this.group1.Items.Add(this.buttonGroup3);
            this.group1.Items.Add(this.buttonGroup1);
            this.group1.Items.Add(this.buttonGroup2);
            this.group1.Label = "Coloriƨation";
            this.group1.Name = "group1";
            this.group1.DialogLauncherClick += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.grpDialogLauncher_Click);
            // 
            // buttonGroup3
            // 
            this.buttonGroup3.Items.Add(this.btnPhonemes);
            this.buttonGroup3.Items.Add(this.btnMuettes);
            this.buttonGroup3.Items.Add(this.btnSyls);
            this.buttonGroup3.Name = "buttonGroup3";
            // 
            // btnPhonemes
            // 
            this.btnPhonemes.Image = global::ColorizationWord.Properties.Resources.phon_carré_30;
            this.btnPhonemes.Label = "Phonèmes";
            this.btnPhonemes.Name = "btnPhonemes";
            this.btnPhonemes.ScreenTip = "Phonèmes";
            this.btnPhonemes.ShowImage = true;
            this.btnPhonemes.ShowLabel = false;
            this.btnPhonemes.SuperTip = "Colorise les phonèmes dans le texte sélectionné.";
            this.btnPhonemes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnPhonemes_Click);
            // 
            // btnMuettes
            // 
            this.btnMuettes.Image = global::ColorizationWord.Properties.Resources.l_muettes_26;
            this.btnMuettes.Label = "Muettes";
            this.btnMuettes.Name = "btnMuettes";
            this.btnMuettes.ScreenTip = "Muettes";
            this.btnMuettes.ShowImage = true;
            this.btnMuettes.ShowLabel = false;
            this.btnMuettes.SuperTip = "Colorise les lettres muettes dans le texte sélectionné.";
            this.btnMuettes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMuettes_Click);
            // 
            // btnSyls
            // 
            this.btnSyls.Image = global::ColorizationWord.Properties.Resources.syll_dys_35;
            this.btnSyls.Label = "Syllabes";
            this.btnSyls.Name = "btnSyls";
            this.btnSyls.ScreenTip = "Syllabes";
            this.btnSyls.ShowImage = true;
            this.btnSyls.ShowLabel = false;
            this.btnSyls.SuperTip = "Colorise les syllabes dans le texte sélectionné.";
            this.btnSyls.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyl_Click);
            // 
            // buttonGroup1
            // 
            this.buttonGroup1.Items.Add(this.btnMots);
            this.buttonGroup1.Items.Add(this.btnLignes);
            this.buttonGroup1.Items.Add(this.btnBPDQ);
            this.buttonGroup1.Name = "buttonGroup1";
            // 
            // btnMots
            // 
            this.btnMots.Image = global::ColorizationWord.Properties.Resources.mots_30;
            this.btnMots.Label = "Mots";
            this.btnMots.Name = "btnMots";
            this.btnMots.ScreenTip = "Mots";
            this.btnMots.ShowImage = true;
            this.btnMots.ShowLabel = false;
            this.btnMots.SuperTip = "Colorise les mots dans le texte sélectionné.";
            this.btnMots.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMots_Click);
            // 
            // btnLignes
            // 
            this.btnLignes.Image = global::ColorizationWord.Properties.Resources.lines_26;
            this.btnLignes.Label = "Lignes";
            this.btnLignes.Name = "btnLignes";
            this.btnLignes.ScreenTip = "Lignes";
            this.btnLignes.ShowImage = true;
            this.btnLignes.ShowLabel = false;
            this.btnLignes.SuperTip = "Colorise les lignes dans le texte sélectionné.";
            this.btnLignes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLignes_Click);
            // 
            // btnBPDQ
            // 
            this.btnBPDQ.Image = global::ColorizationWord.Properties.Resources.bdpq1;
            this.btnBPDQ.Label = "BPDQ";
            this.btnBPDQ.Name = "btnBPDQ";
            this.btnBPDQ.ScreenTip = "Lettres (bpdq)";
            this.btnBPDQ.ShowImage = true;
            this.btnBPDQ.ShowLabel = false;
            this.btnBPDQ.SuperTip = "Colorise les lettres choisies dans le texte sélectionné.";
            this.btnBPDQ.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnBDPQ_Click);
            // 
            // buttonGroup2
            // 
            this.buttonGroup2.Items.Add(this.btnVoyCons);
            this.buttonGroup2.Items.Add(this.btnNoir);
            this.buttonGroup2.Name = "buttonGroup2";
            // 
            // btnVoyCons
            // 
            this.btnVoyCons.Image = global::ColorizationWord.Properties.Resources.voycons_26;
            this.btnVoyCons.Label = "button1";
            this.btnVoyCons.Name = "btnVoyCons";
            this.btnVoyCons.ScreenTip = "Voyelles-Consonnes";
            this.btnVoyCons.ShowImage = true;
            this.btnVoyCons.ShowLabel = false;
            this.btnVoyCons.SuperTip = "Colorise les voyelles et les consonnes dans le texte sélectionné";
            this.btnVoyCons.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnVoyCons_Click);
            // 
            // btnNoir
            // 
            this.btnNoir.Image = global::ColorizationWord.Properties.Resources.black_26;
            this.btnNoir.Label = "Noir";
            this.btnNoir.Name = "btnNoir";
            this.btnNoir.ScreenTip = "Noir";
            this.btnNoir.ShowImage = true;
            this.btnNoir.ShowLabel = false;
            this.btnNoir.SuperTip = "Met le texte sélectionné en noir, sans gras, italique ou souligné.";
            this.btnNoir.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnNoir_Click);
            // 
            // WordRibbon
            // 
            this.Name = "WordRibbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.buttonGroup3.ResumeLayout(false);
            this.buttonGroup3.PerformLayout();
            this.buttonGroup1.ResumeLayout(false);
            this.buttonGroup1.PerformLayout();
            this.buttonGroup2.ResumeLayout(false);
            this.buttonGroup2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPhonemes;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnBPDQ;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSyls;
        internal Microsoft.Office.Tools.Ribbon.RibbonButtonGroup buttonGroup1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMots;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMuettes;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnNoir;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLignes;
        internal Microsoft.Office.Tools.Ribbon.RibbonButtonGroup buttonGroup2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnVoyCons;
        internal Microsoft.Office.Tools.Ribbon.RibbonButtonGroup buttonGroup3;
    }

    partial class ThisRibbonCollection
    {
        internal WordRibbon Ribbon1
        {
            get { return this.GetRibbon<WordRibbon>(); }
        }
    }
}
