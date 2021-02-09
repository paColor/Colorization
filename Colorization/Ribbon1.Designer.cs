using Microsoft.Office.Tools.Ribbon;
using System;

namespace Colorization
{
    partial class Ribbon1 : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon1()
            : base(Globals.Factory.GetRibbonFactory())
        {
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
            this.grpConfiguration = this.Factory.CreateRibbonGroup();
            this.buttonGroup1 = this.Factory.CreateRibbonButtonGroup();
            this.buttonGroup2 = this.Factory.CreateRibbonButtonGroup();
            this.buttonGroup3 = this.Factory.CreateRibbonButtonGroup();
            this.serviceController1 = new System.ServiceProcess.ServiceController();
            this.btnPhon = this.Factory.CreateRibbonButton();
            this.btnMuettes = this.Factory.CreateRibbonButton();
            this.btnSyl = this.Factory.CreateRibbonButton();
            this.btnArcs = this.Factory.CreateRibbonButton();
            this.btnMots = this.Factory.CreateRibbonButton();
            this.btnLignes = this.Factory.CreateRibbonButton();
            this.btnBPDQ = this.Factory.CreateRibbonButton();
            this.btnRemoveArcs = this.Factory.CreateRibbonButton();
            this.btnVoyCons = this.Factory.CreateRibbonButton();
            this.btnDuo = this.Factory.CreateRibbonButton();
            this.btnNoir = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.grpConfiguration.SuspendLayout();
            this.buttonGroup1.SuspendLayout();
            this.buttonGroup2.SuspendLayout();
            this.buttonGroup3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.ControlId.OfficeId = "TabHome";
            this.tab1.Groups.Add(this.grpConfiguration);
            this.tab1.Label = "TabHome";
            this.tab1.Name = "tab1";
            // 
            // grpConfiguration
            // 
            ribbonDialogLauncherImpl1.ScreenTip = "Configuration";
            ribbonDialogLauncherImpl1.SuperTip = "Ouvre le panneau de configuration";
            this.grpConfiguration.DialogLauncher = ribbonDialogLauncherImpl1;
            this.grpConfiguration.Items.Add(this.buttonGroup1);
            this.grpConfiguration.Items.Add(this.buttonGroup2);
            this.grpConfiguration.Items.Add(this.buttonGroup3);
            this.grpConfiguration.Label = "Coloriƨation";
            this.grpConfiguration.Name = "grpConfiguration";
            this.grpConfiguration.DialogLauncherClick += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.grpDialogLauncher_Click);
            // 
            // buttonGroup1
            // 
            this.buttonGroup1.Items.Add(this.btnPhon);
            this.buttonGroup1.Items.Add(this.btnMuettes);
            this.buttonGroup1.Items.Add(this.btnSyl);
            this.buttonGroup1.Items.Add(this.btnArcs);
            this.buttonGroup1.Name = "buttonGroup1";
            // 
            // buttonGroup2
            // 
            this.buttonGroup2.Items.Add(this.btnMots);
            this.buttonGroup2.Items.Add(this.btnLignes);
            this.buttonGroup2.Items.Add(this.btnBPDQ);
            this.buttonGroup2.Items.Add(this.btnRemoveArcs);
            this.buttonGroup2.Name = "buttonGroup2";
            // 
            // buttonGroup3
            // 
            this.buttonGroup3.Items.Add(this.btnVoyCons);
            this.buttonGroup3.Items.Add(this.btnDuo);
            this.buttonGroup3.Items.Add(this.btnNoir);
            this.buttonGroup3.Name = "buttonGroup3";
            // 
            // btnPhon
            // 
            this.btnPhon.Description = "Phonèmes";
            this.btnPhon.Enabled = false;
            this.btnPhon.Image = global::Colorization.Properties.Resources.phon;
            this.btnPhon.KeyTip = "P";
            this.btnPhon.Label = "Phonèmes";
            this.btnPhon.Name = "btnPhon";
            this.btnPhon.ScreenTip = "Phonèmes";
            this.btnPhon.ShowImage = true;
            this.btnPhon.ShowLabel = false;
            this.btnPhon.SuperTip = "Coloriser les phonèmes du texte sélectionné";
            this.btnPhon.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnColoriser_Click);
            // 
            // btnMuettes
            // 
            this.btnMuettes.Enabled = false;
            this.btnMuettes.Image = global::Colorization.Properties.Resources.l_muettes_26;
            this.btnMuettes.KeyTip = "-";
            this.btnMuettes.Label = "Muettes";
            this.btnMuettes.Name = "btnMuettes";
            this.btnMuettes.ScreenTip = "Lettres muettes";
            this.btnMuettes.ShowImage = true;
            this.btnMuettes.ShowLabel = false;
            this.btnMuettes.SuperTip = "Coloriser les lettres muettes en gris dans le texte sélectionné";
            this.btnMuettes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMuettes_Click);
            // 
            // btnSyl
            // 
            this.btnSyl.Enabled = false;
            this.btnSyl.Image = global::Colorization.Properties.Resources.syll_dys_35;
            this.btnSyl.KeyTip = "S";
            this.btnSyl.Label = "Syllabes";
            this.btnSyl.Name = "btnSyl";
            this.btnSyl.ScreenTip = "Syllabes";
            this.btnSyl.ShowImage = true;
            this.btnSyl.ShowLabel = false;
            this.btnSyl.SuperTip = "Coloriser les syllabes sélectionnées";
            this.btnSyl.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSyl_Click);
            // 
            // btnArcs
            // 
            this.btnArcs.Enabled = false;
            this.btnArcs.Image = global::Colorization.Properties.Resources.syll_26;
            this.btnArcs.Label = "Arcs";
            this.btnArcs.Name = "btnArcs";
            this.btnArcs.ScreenTip = "Arcs";
            this.btnArcs.ShowImage = true;
            this.btnArcs.ShowLabel = false;
            this.btnArcs.SuperTip = "Dessine des arcs sous les syllabes du texte sélectionné.";
            this.btnArcs.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnArcs_Click);
            // 
            // btnMots
            // 
            this.btnMots.Enabled = false;
            this.btnMots.Image = global::Colorization.Properties.Resources.mots_30;
            this.btnMots.KeyTip = "M";
            this.btnMots.Label = "Mots";
            this.btnMots.Name = "btnMots";
            this.btnMots.ScreenTip = "Mots";
            this.btnMots.ShowImage = true;
            this.btnMots.ShowLabel = false;
            this.btnMots.SuperTip = "Coloriser les mots sélectionnés";
            this.btnMots.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMots_Click);
            // 
            // btnLignes
            // 
            this.btnLignes.Enabled = false;
            this.btnLignes.Image = global::Colorization.Properties.Resources.lines_30;
            this.btnLignes.Label = "Lignes";
            this.btnLignes.Name = "btnLignes";
            this.btnLignes.ScreenTip = "Lignes";
            this.btnLignes.ShowImage = true;
            this.btnLignes.ShowLabel = false;
            this.btnLignes.SuperTip = "Colorise les lignes du texte sélectionné";
            this.btnLignes.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLignes_Click);
            // 
            // btnBPDQ
            // 
            this.btnBPDQ.Enabled = false;
            this.btnBPDQ.Image = global::Colorization.Properties.Resources.bdpq;
            this.btnBPDQ.KeyTip = "L";
            this.btnBPDQ.Label = "button1";
            this.btnBPDQ.Name = "btnBPDQ";
            this.btnBPDQ.ScreenTip = "Lettres (bdpq)";
            this.btnBPDQ.ShowImage = true;
            this.btnBPDQ.ShowLabel = false;
            this.btnBPDQ.SuperTip = "Coloriser les lettres choisies (par exemple bdpq) dans le texte sélectionné";
            this.btnBPDQ.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnBDPQ_Click);
            // 
            // btnRemoveArcs
            // 
            this.btnRemoveArcs.Enabled = false;
            this.btnRemoveArcs.Image = global::Colorization.Properties.Resources.cleaner_26;
            this.btnRemoveArcs.Label = "RemoveArcs";
            this.btnRemoveArcs.Name = "btnRemoveArcs";
            this.btnRemoveArcs.ScreenTip = "Effacer les arcs";
            this.btnRemoveArcs.ShowImage = true;
            this.btnRemoveArcs.ShowLabel = false;
            this.btnRemoveArcs.SuperTip = "Efface les arcs qui se trouvent dans la sélection.";
            this.btnRemoveArcs.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRemoveArcs_Click);
            // 
            // btnVoyCons
            // 
            this.btnVoyCons.Enabled = false;
            this.btnVoyCons.Image = global::Colorization.Properties.Resources.voycons_26;
            this.btnVoyCons.Label = "VoyCons";
            this.btnVoyCons.Name = "btnVoyCons";
            this.btnVoyCons.ScreenTip = "Voyelles-Consonnes";
            this.btnVoyCons.ShowImage = true;
            this.btnVoyCons.ShowLabel = false;
            this.btnVoyCons.SuperTip = "Colorise les voyelles et les consonnes dans le texte sélectionné";
            this.btnVoyCons.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnVoyCons_Click);
            // 
            // btnDuo
            // 
            this.btnDuo.Enabled = false;
            this.btnDuo.Image = global::Colorization.Properties.Resources._2_16;
            this.btnDuo.KeyTip = "D";
            this.btnDuo.Label = "Noir";
            this.btnDuo.Name = "btnDuo";
            this.btnDuo.ScreenTip = "Duo";
            this.btnDuo.ShowImage = true;
            this.btnDuo.ShowLabel = false;
            this.btnDuo.SuperTip = "Le texte sélectionné est formaté en alternance pour deux lecteurs.";
            this.btnDuo.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDuo_Click);
            // 
            // btnNoir
            // 
            this.btnNoir.Enabled = false;
            this.btnNoir.Image = global::Colorization.Properties.Resources.black_26;
            this.btnNoir.KeyTip = "N";
            this.btnNoir.Label = "Noir";
            this.btnNoir.Name = "btnNoir";
            this.btnNoir.ScreenTip = "Sans formattage";
            this.btnNoir.ShowImage = true;
            this.btnNoir.ShowLabel = false;
            this.btnNoir.SuperTip = "Le texte sélectionné est mis en noir sans autre formattage (gras, italique et sou" +
    "ligné sont annulés)";
            this.btnNoir.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnNoir_Click);
            // 
            // Ribbon1
            // 
            this.Name = "Ribbon1";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.grpConfiguration.ResumeLayout(false);
            this.grpConfiguration.PerformLayout();
            this.buttonGroup1.ResumeLayout(false);
            this.buttonGroup1.PerformLayout();
            this.buttonGroup2.ResumeLayout(false);
            this.buttonGroup2.PerformLayout();
            this.buttonGroup3.ResumeLayout(false);
            this.buttonGroup3.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpConfiguration;
        private System.ServiceProcess.ServiceController serviceController1;
        internal RibbonButton btnPhon;
        internal RibbonButtonGroup buttonGroup1;
        internal RibbonButton btnSyl;
        internal RibbonButton btnBPDQ;
        internal RibbonButton btnMots;
        internal RibbonButton btnMuettes;
        internal RibbonButtonGroup buttonGroup2;
        internal RibbonButton btnNoir;
        internal RibbonButton btnVoyCons;
        internal RibbonButton btnLignes;
        internal RibbonButtonGroup buttonGroup3;
        internal RibbonButton btnDuo;
        internal RibbonButton btnArcs;
        internal RibbonButton btnRemoveArcs;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon1 Ribbon1
        {
            get { return this.GetRibbon<Ribbon1>(); }
        }
    }
}
