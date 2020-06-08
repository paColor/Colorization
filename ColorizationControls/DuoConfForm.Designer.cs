namespace ColorizationControls
{
    partial class DuoConfForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DuoConfForm));
            this.lblConfig1 = new System.Windows.Forms.Label();
            this.panelConfig1 = new System.Windows.Forms.Panel();
            this.panelConfig2 = new System.Windows.Forms.Panel();
            this.lblConfig2 = new System.Windows.Forms.Label();
            this.rbtnMots = new System.Windows.Forms.RadioButton();
            this.lblAlternance = new System.Windows.Forms.Label();
            this.rbtnLignes = new System.Windows.Forms.RadioButton();
            this.lblColoriser = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbtnMuettes = new System.Windows.Forms.RadioButton();
            this.rbtnPhonemes = new System.Windows.Forms.RadioButton();
            this.rbtnVoyCons = new System.Windows.Forms.RadioButton();
            this.rbtnLettres = new System.Windows.Forms.RadioButton();
            this.rbtnSyylabes = new System.Windows.Forms.RadioButton();
            this.rbtnColorMots = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDefaut = new System.Windows.Forms.Button();
            this.btnDefConf1 = new System.Windows.Forms.Button();
            this.btnDefConf2 = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.btnValider = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConfig1
            // 
            this.lblConfig1.AutoSize = true;
            this.lblConfig1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfig1.Location = new System.Drawing.Point(89, 13);
            this.lblConfig1.Name = "lblConfig1";
            this.lblConfig1.Size = new System.Drawing.Size(111, 16);
            this.lblConfig1.TabIndex = 1;
            this.lblConfig1.Text = "Configuration 1";
            // 
            // panelConfig1
            // 
            this.panelConfig1.Location = new System.Drawing.Point(12, 45);
            this.panelConfig1.Name = "panelConfig1";
            this.panelConfig1.Size = new System.Drawing.Size(348, 533);
            this.panelConfig1.TabIndex = 2;
            // 
            // panelConfig2
            // 
            this.panelConfig2.Location = new System.Drawing.Point(520, 45);
            this.panelConfig2.Name = "panelConfig2";
            this.panelConfig2.Size = new System.Drawing.Size(348, 533);
            this.panelConfig2.TabIndex = 3;
            // 
            // lblConfig2
            // 
            this.lblConfig2.AutoSize = true;
            this.lblConfig2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfig2.Location = new System.Drawing.Point(590, 13);
            this.lblConfig2.Name = "lblConfig2";
            this.lblConfig2.Size = new System.Drawing.Size(111, 16);
            this.lblConfig2.TabIndex = 4;
            this.lblConfig2.Text = "Configuration 2";
            // 
            // rbtnMots
            // 
            this.rbtnMots.AutoSize = true;
            this.rbtnMots.Location = new System.Drawing.Point(3, 3);
            this.rbtnMots.Name = "rbtnMots";
            this.rbtnMots.Size = new System.Drawing.Size(48, 17);
            this.rbtnMots.TabIndex = 5;
            this.rbtnMots.TabStop = true;
            this.rbtnMots.Text = "Mots";
            this.toolTip1.SetToolTip(this.rbtnMots, "Le formatage de la configuration 1 sera\r\nappliqué aux mots impairs et le formatag" +
        "e\r\nde la configuration 2 aux mots pairs.");
            this.rbtnMots.UseVisualStyleBackColor = true;
            this.rbtnMots.CheckedChanged += new System.EventHandler(this.rbtnMots_CheckedChanged);
            // 
            // lblAlternance
            // 
            this.lblAlternance.AutoSize = true;
            this.lblAlternance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlternance.Location = new System.Drawing.Point(400, 64);
            this.lblAlternance.Name = "lblAlternance";
            this.lblAlternance.Size = new System.Drawing.Size(82, 16);
            this.lblAlternance.TabIndex = 6;
            this.lblAlternance.Text = "Alternance";
            // 
            // rbtnLignes
            // 
            this.rbtnLignes.AutoSize = true;
            this.rbtnLignes.Location = new System.Drawing.Point(3, 26);
            this.rbtnLignes.Name = "rbtnLignes";
            this.rbtnLignes.Size = new System.Drawing.Size(56, 17);
            this.rbtnLignes.TabIndex = 6;
            this.rbtnLignes.TabStop = true;
            this.rbtnLignes.Text = "Lignes";
            this.toolTip1.SetToolTip(this.rbtnLignes, "Le formatage de la configuration 1 sera\r\nappliqué aux lignes  impaires (1, 3 ,5, " +
        "... )\r\net le formatagede la configuration 2 \r\naux lignes paires (2, 4, 6, ...).\r" +
        "\n");
            this.rbtnLignes.UseVisualStyleBackColor = true;
            this.rbtnLignes.CheckedChanged += new System.EventHandler(this.rbtnLignes_CheckedChanged);
            // 
            // lblColoriser
            // 
            this.lblColoriser.AutoSize = true;
            this.lblColoriser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColoriser.Location = new System.Drawing.Point(405, 154);
            this.lblColoriser.Name = "lblColoriser";
            this.lblColoriser.Size = new System.Drawing.Size(71, 16);
            this.lblColoriser.TabIndex = 8;
            this.lblColoriser.Text = "Coloriser";
            this.lblColoriser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbtnMuettes);
            this.panel3.Controls.Add(this.rbtnPhonemes);
            this.panel3.Controls.Add(this.rbtnVoyCons);
            this.panel3.Controls.Add(this.rbtnLettres);
            this.panel3.Controls.Add(this.rbtnSyylabes);
            this.panel3.Controls.Add(this.rbtnColorMots);
            this.panel3.Location = new System.Drawing.Point(387, 173);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(112, 259);
            this.panel3.TabIndex = 11;
            // 
            // rbtnMuettes
            // 
            this.rbtnMuettes.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnMuettes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnMuettes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnMuettes.Image = global::ColorizationControls.Properties.Resources.l_muettes_26;
            this.rbtnMuettes.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnMuettes.Location = new System.Drawing.Point(12, 214);
            this.rbtnMuettes.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnMuettes.Name = "rbtnMuettes";
            this.rbtnMuettes.Size = new System.Drawing.Size(88, 37);
            this.rbtnMuettes.TabIndex = 14;
            this.rbtnMuettes.TabStop = true;
            this.rbtnMuettes.Text = "Muettes";
            this.rbtnMuettes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnMuettes, "La commande \"2\" colorisera les muettes\r\navec les formatages choisis appliqués en\r" +
        "\nalternance.\r\n");
            this.rbtnMuettes.UseVisualStyleBackColor = true;
            this.rbtnMuettes.CheckedChanged += new System.EventHandler(this.rbtnMuettes_CheckedChanged);
            // 
            // rbtnPhonemes
            // 
            this.rbtnPhonemes.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnPhonemes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnPhonemes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnPhonemes.Image = global::ColorizationControls.Properties.Resources.phon_carré_26;
            this.rbtnPhonemes.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnPhonemes.Location = new System.Drawing.Point(12, 172);
            this.rbtnPhonemes.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnPhonemes.Name = "rbtnPhonemes";
            this.rbtnPhonemes.Size = new System.Drawing.Size(88, 37);
            this.rbtnPhonemes.TabIndex = 13;
            this.rbtnPhonemes.TabStop = true;
            this.rbtnPhonemes.Text = "Pho-\r\nnèmes";
            this.rbtnPhonemes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnPhonemes, "La commande \"2\" colorisera les phonèmes\r\navec les formatages choisis appliqués en" +
        "\r\nalternance.");
            this.rbtnPhonemes.UseVisualStyleBackColor = true;
            this.rbtnPhonemes.CheckedChanged += new System.EventHandler(this.rbtnPhonemes_CheckedChanged);
            // 
            // rbtnVoyCons
            // 
            this.rbtnVoyCons.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnVoyCons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnVoyCons.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnVoyCons.Image = global::ColorizationControls.Properties.Resources.voycons_26;
            this.rbtnVoyCons.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnVoyCons.Location = new System.Drawing.Point(12, 130);
            this.rbtnVoyCons.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnVoyCons.Name = "rbtnVoyCons";
            this.rbtnVoyCons.Size = new System.Drawing.Size(88, 37);
            this.rbtnVoyCons.TabIndex = 12;
            this.rbtnVoyCons.TabStop = true;
            this.rbtnVoyCons.Text = "Voy. /\r\nCons.";
            this.rbtnVoyCons.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnVoyCons, "La commande \"2\" colorisera les voyelles\r\net le consonnes avec les formatages\r\ncho" +
        "isis appliqués en alternance.\r\n");
            this.rbtnVoyCons.UseVisualStyleBackColor = true;
            this.rbtnVoyCons.CheckedChanged += new System.EventHandler(this.rbtnVoyCons_CheckedChanged);
            // 
            // rbtnLettres
            // 
            this.rbtnLettres.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnLettres.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnLettres.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnLettres.Image = global::ColorizationControls.Properties.Resources.bdpq_26;
            this.rbtnLettres.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnLettres.Location = new System.Drawing.Point(12, 88);
            this.rbtnLettres.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnLettres.Name = "rbtnLettres";
            this.rbtnLettres.Size = new System.Drawing.Size(88, 37);
            this.rbtnLettres.TabIndex = 11;
            this.rbtnLettres.TabStop = true;
            this.rbtnLettres.Text = "Lettres";
            this.rbtnLettres.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbtnLettres.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnLettres, "La commande \"2\" colorisera les lettres\r\navec les formatages choisis appliqués en\r" +
        "\nalternance.\r\n");
            this.rbtnLettres.UseVisualStyleBackColor = true;
            this.rbtnLettres.CheckedChanged += new System.EventHandler(this.rbtnLettres_CheckedChanged);
            // 
            // rbtnSyylabes
            // 
            this.rbtnSyylabes.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnSyylabes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnSyylabes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnSyylabes.Image = global::ColorizationControls.Properties.Resources.syll_dys_26;
            this.rbtnSyylabes.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnSyylabes.Location = new System.Drawing.Point(12, 4);
            this.rbtnSyylabes.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnSyylabes.Name = "rbtnSyylabes";
            this.rbtnSyylabes.Size = new System.Drawing.Size(88, 37);
            this.rbtnSyylabes.TabIndex = 9;
            this.rbtnSyylabes.TabStop = true;
            this.rbtnSyylabes.Text = "Syllabes";
            this.rbtnSyylabes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbtnSyylabes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnSyylabes, "La commande \"2\" colorisera les syllabes\r\navec les formatages choisis appliqués en" +
        "\r\nalternance.");
            this.rbtnSyylabes.UseVisualStyleBackColor = true;
            this.rbtnSyylabes.CheckedChanged += new System.EventHandler(this.rbtnSyylabes_CheckedChanged);
            // 
            // rbtnColorMots
            // 
            this.rbtnColorMots.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtnColorMots.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rbtnColorMots.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.rbtnColorMots.Image = global::ColorizationControls.Properties.Resources.mots_26;
            this.rbtnColorMots.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbtnColorMots.Location = new System.Drawing.Point(12, 46);
            this.rbtnColorMots.Margin = new System.Windows.Forms.Padding(0);
            this.rbtnColorMots.Name = "rbtnColorMots";
            this.rbtnColorMots.Size = new System.Drawing.Size(88, 37);
            this.rbtnColorMots.TabIndex = 10;
            this.rbtnColorMots.TabStop = true;
            this.rbtnColorMots.Text = "Mots";
            this.rbtnColorMots.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rbtnColorMots.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.rbtnColorMots, "La commande \"2\" colorisera les mots\r\navec les formatages choisis appliqués en\r\nal" +
        "ternance.\r\n");
            this.rbtnColorMots.UseVisualStyleBackColor = true;
            this.rbtnColorMots.CheckedChanged += new System.EventHandler(this.rbtnColorMots_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtnLignes);
            this.panel2.Controls.Add(this.rbtnMots);
            this.panel2.Location = new System.Drawing.Point(408, 82);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(68, 52);
            this.panel2.TabIndex = 7;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(279, 601);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(326, 79);
            this.richTextBox1.TabIndex = 12;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "Heureux qui, comme Ulysse, a fait un beau voyage,\nOu comme cestui-là qui conquit " +
    "la toison,\nEt puis est retourné, plein d\'usage et raison,\nVivre entre ses parent" +
    "s le reste de son âge!";
            // 
            // btnDefaut
            // 
            this.btnDefaut.Image = global::ColorizationControls.Properties.Resources.Défaut_bleu_16;
            this.btnDefaut.Location = new System.Drawing.Point(111, 627);
            this.btnDefaut.Name = "btnDefaut";
            this.btnDefaut.Size = new System.Drawing.Size(75, 23);
            this.btnDefaut.TabIndex = 15;
            this.btnDefaut.Text = "Défaut";
            this.btnDefaut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnDefaut, "Réinitialise les deux configurations\r\nà leur valeur par défaut.");
            this.btnDefaut.UseVisualStyleBackColor = true;
            this.btnDefaut.Click += new System.EventHandler(this.btnDefaut_Click);
            // 
            // btnDefConf1
            // 
            this.btnDefConf1.Image = global::ColorizationControls.Properties.Resources.Défaut_bleu_16;
            this.btnDefConf1.Location = new System.Drawing.Point(214, 10);
            this.btnDefConf1.Name = "btnDefConf1";
            this.btnDefConf1.Size = new System.Drawing.Size(75, 23);
            this.btnDefConf1.TabIndex = 17;
            this.btnDefConf1.Text = "Défaut 1";
            this.btnDefConf1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnDefConf1, "Réinitialise la \'configuration 1\'\r\nà sa valeur par défaut.");
            this.btnDefConf1.UseVisualStyleBackColor = true;
            this.btnDefConf1.Click += new System.EventHandler(this.btnDefConf1_Click);
            // 
            // btnDefConf2
            // 
            this.btnDefConf2.Image = global::ColorizationControls.Properties.Resources.Défaut_bleu_16;
            this.btnDefConf2.Location = new System.Drawing.Point(726, 10);
            this.btnDefConf2.Name = "btnDefConf2";
            this.btnDefConf2.Size = new System.Drawing.Size(75, 23);
            this.btnDefConf2.TabIndex = 18;
            this.btnDefConf2.Text = "Défaut 2";
            this.btnDefConf2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnDefConf2, "Réinitialise la \'configuration 2\'\r\nà sa valeur par défaut.");
            this.btnDefConf2.UseVisualStyleBackColor = true;
            this.btnDefConf2.Click += new System.EventHandler(this.btnDefConf2_Click);
            // 
            // btnAnnuler
            // 
            this.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnuler.Image = global::ColorizationControls.Properties.Resources.Effacer15;
            this.btnAnnuler.Location = new System.Drawing.Point(706, 643);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new System.Drawing.Size(75, 23);
            this.btnAnnuler.TabIndex = 14;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAnnuler.UseVisualStyleBackColor = true;
            this.btnAnnuler.Click += new System.EventHandler(this.btnAnnuler_Click);
            // 
            // btnValider
            // 
            this.btnValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnValider.Image = global::ColorizationControls.Properties.Resources.OK_b_16;
            this.btnValider.Location = new System.Drawing.Point(706, 614);
            this.btnValider.Name = "btnValider";
            this.btnValider.Size = new System.Drawing.Size(75, 23);
            this.btnValider.TabIndex = 13;
            this.btnValider.Text = "Valider";
            this.btnValider.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnValider.UseVisualStyleBackColor = true;
            this.btnValider.Click += new System.EventHandler(this.btnValider_Click);
            // 
            // DuoConfForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnuler;
            this.ClientSize = new System.Drawing.Size(880, 694);
            this.Controls.Add(this.btnDefConf2);
            this.Controls.Add(this.btnDefConf1);
            this.Controls.Add(this.btnDefaut);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnValider);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.lblColoriser);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblAlternance);
            this.Controls.Add(this.lblConfig2);
            this.Controls.Add(this.panelConfig2);
            this.Controls.Add(this.panelConfig1);
            this.Controls.Add(this.lblConfig1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DuoConfForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Double action";
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblConfig1;
        private System.Windows.Forms.Panel panelConfig1;
        private System.Windows.Forms.Panel panelConfig2;
        private System.Windows.Forms.Label lblConfig2;
        private System.Windows.Forms.RadioButton rbtnMots;
        private System.Windows.Forms.Label lblAlternance;
        private System.Windows.Forms.RadioButton rbtnLignes;
        private System.Windows.Forms.Label lblColoriser;
        private System.Windows.Forms.RadioButton rbtnSyylabes;
        private System.Windows.Forms.RadioButton rbtnColorMots;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbtnLettres;
        private System.Windows.Forms.RadioButton rbtnMuettes;
        private System.Windows.Forms.RadioButton rbtnPhonemes;
        private System.Windows.Forms.RadioButton rbtnVoyCons;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnValider;
        private System.Windows.Forms.Button btnAnnuler;
        private System.Windows.Forms.Button btnDefaut;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDefConf1;
        private System.Windows.Forms.Button btnDefConf2;
    }
}