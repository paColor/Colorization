
namespace ColorizationControls
{
    partial class WordListForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxSyllabes = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbxMots = new System.Windows.Forms.CheckBox();
            this.cbxArcs = new System.Windows.Forms.CheckBox();
            this.pictBoxSyllabes = new System.Windows.Forms.PictureBox();
            this.pictBoxMots = new System.Windows.Forms.PictureBox();
            this.pictBoxArcs = new System.Windows.Forms.PictureBox();
            this.pictBoxPhonemes = new System.Windows.Forms.PictureBox();
            this.cbxPhonemes = new System.Windows.Forms.CheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.cbxLettres = new System.Windows.Forms.CheckBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.cbxVoyCons = new System.Windows.Forms.CheckBox();
            this.btnTrier = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxSyllabes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxMots)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxArcs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxPhonemes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(427, 426);
            this.textBox1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Image = global::ColorizationControls.Properties.Resources.OK_b_16;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(500, 413);
            this.btnOK.Name = "btnOK";
            this.btnOK.Padding = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Valider";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAnnuler
            // 
            this.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnuler.Image = global::ColorizationControls.Properties.Resources.Effacer15;
            this.btnAnnuler.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAnnuler.Location = new System.Drawing.Point(500, 384);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.btnAnnuler.Size = new System.Drawing.Size(75, 23);
            this.btnAnnuler.TabIndex = 2;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAnnuler.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(450, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 39);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choicissez ci-dessous quels\r\noutils ne doivent pas coloriser\r\nles mots de la list" +
    "e.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(450, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 78);
            this.label2.TabIndex = 4;
            this.label2.Text = "Les mots peuvent être séparés\r\npar n\'importe quel caractère. \r\nPar exemple virgul" +
    "e, espace, \r\npoint-virgule, ...\r\nL\'utilisation de minuscules\r\nsuffit";
            // 
            // cbxSyllabes
            // 
            this.cbxSyllabes.AutoSize = true;
            this.cbxSyllabes.Location = new System.Drawing.Point(487, 185);
            this.cbxSyllabes.Name = "cbxSyllabes";
            this.cbxSyllabes.Size = new System.Drawing.Size(65, 17);
            this.cbxSyllabes.TabIndex = 5;
            this.cbxSyllabes.Text = "Syllabes";
            this.toolTip1.SetToolTip(this.cbxSyllabes, "Cocher pour que les mots de \r\nla liste ne soient pas colorisés par\r\nl\'outil \"Syll" +
        "abes\".");
            this.cbxSyllabes.UseVisualStyleBackColor = true;
            // 
            // cbxMots
            // 
            this.cbxMots.AutoSize = true;
            this.cbxMots.Location = new System.Drawing.Point(487, 231);
            this.cbxMots.Name = "cbxMots";
            this.cbxMots.Size = new System.Drawing.Size(49, 17);
            this.cbxMots.TabIndex = 6;
            this.cbxMots.Text = "Mots";
            this.toolTip1.SetToolTip(this.cbxMots, "Cocher pour que les mots de \r\nla liste ne soient pas colorisés par\r\nl\'outil \"Mots" +
        "\".");
            this.cbxMots.UseVisualStyleBackColor = true;
            // 
            // cbxArcs
            // 
            this.cbxArcs.AutoSize = true;
            this.cbxArcs.Location = new System.Drawing.Point(487, 208);
            this.cbxArcs.Name = "cbxArcs";
            this.cbxArcs.Size = new System.Drawing.Size(47, 17);
            this.cbxArcs.TabIndex = 7;
            this.cbxArcs.Text = "Arcs";
            this.toolTip1.SetToolTip(this.cbxArcs, "Cocher pour que les mots de \r\nla liste ne soient pas traités par\r\nl\'outil \"Arcs\"." +
        "\r\n");
            this.cbxArcs.UseVisualStyleBackColor = true;
            // 
            // pictBoxSyllabes
            // 
            this.pictBoxSyllabes.Image = global::ColorizationControls.Properties.Resources.syll_dys_16;
            this.pictBoxSyllabes.InitialImage = null;
            this.pictBoxSyllabes.Location = new System.Drawing.Point(463, 184);
            this.pictBoxSyllabes.Name = "pictBoxSyllabes";
            this.pictBoxSyllabes.Size = new System.Drawing.Size(16, 16);
            this.pictBoxSyllabes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictBoxSyllabes.TabIndex = 8;
            this.pictBoxSyllabes.TabStop = false;
            // 
            // pictBoxMots
            // 
            this.pictBoxMots.Image = global::ColorizationControls.Properties.Resources.mots_26;
            this.pictBoxMots.InitialImage = null;
            this.pictBoxMots.Location = new System.Drawing.Point(463, 230);
            this.pictBoxMots.Name = "pictBoxMots";
            this.pictBoxMots.Size = new System.Drawing.Size(16, 16);
            this.pictBoxMots.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBoxMots.TabIndex = 9;
            this.pictBoxMots.TabStop = false;
            // 
            // pictBoxArcs
            // 
            this.pictBoxArcs.Image = global::ColorizationControls.Properties.Resources.syll_26;
            this.pictBoxArcs.InitialImage = null;
            this.pictBoxArcs.Location = new System.Drawing.Point(463, 207);
            this.pictBoxArcs.Name = "pictBoxArcs";
            this.pictBoxArcs.Size = new System.Drawing.Size(16, 16);
            this.pictBoxArcs.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBoxArcs.TabIndex = 10;
            this.pictBoxArcs.TabStop = false;
            // 
            // pictBoxPhonemes
            // 
            this.pictBoxPhonemes.Image = global::ColorizationControls.Properties.Resources.phon_carré_26;
            this.pictBoxPhonemes.InitialImage = null;
            this.pictBoxPhonemes.Location = new System.Drawing.Point(463, 161);
            this.pictBoxPhonemes.Name = "pictBoxPhonemes";
            this.pictBoxPhonemes.Size = new System.Drawing.Size(16, 16);
            this.pictBoxPhonemes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBoxPhonemes.TabIndex = 12;
            this.pictBoxPhonemes.TabStop = false;
            // 
            // cbxPhonemes
            // 
            this.cbxPhonemes.AutoSize = true;
            this.cbxPhonemes.Location = new System.Drawing.Point(487, 162);
            this.cbxPhonemes.Name = "cbxPhonemes";
            this.cbxPhonemes.Size = new System.Drawing.Size(76, 17);
            this.cbxPhonemes.TabIndex = 11;
            this.cbxPhonemes.Text = "Phonèmes";
            this.toolTip1.SetToolTip(this.cbxPhonemes, "Cocher pour que les mots de \r\nla liste ne soient pas colorisés par\r\nl\'outil \"Phon" +
        "èmes\".");
            this.cbxPhonemes.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ColorizationControls.Properties.Resources.bdpq_26;
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(463, 253);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(16, 16);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 14;
            this.pictureBox3.TabStop = false;
            // 
            // cbxLettres
            // 
            this.cbxLettres.AutoSize = true;
            this.cbxLettres.Location = new System.Drawing.Point(487, 254);
            this.cbxLettres.Name = "cbxLettres";
            this.cbxLettres.Size = new System.Drawing.Size(58, 17);
            this.cbxLettres.TabIndex = 13;
            this.cbxLettres.Text = "Lettres";
            this.toolTip1.SetToolTip(this.cbxLettres, "Cocher pour que les mots de \r\nla liste ne soient pas traités par\r\nl\'outil \"Lettre" +
        "s\".");
            this.cbxLettres.UseVisualStyleBackColor = true;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::ColorizationControls.Properties.Resources.voycons_26;
            this.pictureBox4.InitialImage = null;
            this.pictureBox4.Location = new System.Drawing.Point(463, 276);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(16, 16);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 16;
            this.pictureBox4.TabStop = false;
            // 
            // cbxVoyCons
            // 
            this.cbxVoyCons.AutoSize = true;
            this.cbxVoyCons.Location = new System.Drawing.Point(487, 277);
            this.cbxVoyCons.Name = "cbxVoyCons";
            this.cbxVoyCons.Size = new System.Drawing.Size(129, 17);
            this.cbxVoyCons.TabIndex = 15;
            this.cbxVoyCons.Text = "Voyelles / Consonnes";
            this.toolTip1.SetToolTip(this.cbxVoyCons, "Cocher pour que les mots de \r\nla liste ne soient pas colorisés par\r\nl\'outil \"Voye" +
        "lles / Consonnes\".\r\n");
            this.cbxVoyCons.UseVisualStyleBackColor = true;
            // 
            // btnTrier
            // 
            this.btnTrier.Location = new System.Drawing.Point(500, 329);
            this.btnTrier.Name = "btnTrier";
            this.btnTrier.Size = new System.Drawing.Size(75, 23);
            this.btnTrier.TabIndex = 17;
            this.btnTrier.Text = "Trier";
            this.btnTrier.UseVisualStyleBackColor = true;
            this.btnTrier.Click += new System.EventHandler(this.btnTrier_Click);
            // 
            // WordListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnuler;
            this.ClientSize = new System.Drawing.Size(623, 450);
            this.Controls.Add(this.btnTrier);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.cbxVoyCons);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.cbxLettres);
            this.Controls.Add(this.pictBoxPhonemes);
            this.Controls.Add(this.cbxPhonemes);
            this.Controls.Add(this.pictBoxArcs);
            this.Controls.Add(this.pictBoxMots);
            this.Controls.Add(this.pictBoxSyllabes);
            this.Controls.Add(this.cbxArcs);
            this.Controls.Add(this.cbxMots);
            this.Controls.Add(this.cbxSyllabes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WordListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Liste des mots à ne pas coloriƨer";
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxSyllabes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxMots)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxArcs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxPhonemes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnAnnuler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbxSyllabes;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbxMots;
        private System.Windows.Forms.CheckBox cbxArcs;
        private System.Windows.Forms.PictureBox pictBoxSyllabes;
        private System.Windows.Forms.PictureBox pictBoxMots;
        private System.Windows.Forms.PictureBox pictBoxArcs;
        private System.Windows.Forms.PictureBox pictBoxPhonemes;
        private System.Windows.Forms.CheckBox cbxPhonemes;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.CheckBox cbxLettres;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.CheckBox cbxVoyCons;
        private System.Windows.Forms.Button btnTrier;
    }
}