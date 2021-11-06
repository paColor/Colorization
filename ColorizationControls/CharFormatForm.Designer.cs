namespace ColorizationControls
{
    partial class CharFormatForm
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
            this.btnCouleur = new System.Windows.Forms.Button();
            this.btnValider = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.btnSurl = new System.Windows.Forms.Button();
            this.btnGraphemes = new System.Windows.Forms.Button();
            this.pbxBold = new System.Windows.Forms.PictureBox();
            this.pbxItalic = new System.Windows.Forms.PictureBox();
            this.pbxUnderscore = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbxBold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxItalic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxUnderscore)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCouleur
            // 
            this.btnCouleur.CausesValidation = false;
            this.btnCouleur.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCouleur.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCouleur.Location = new System.Drawing.Point(84, 10);
            this.btnCouleur.Name = "btnCouleur";
            this.btnCouleur.Size = new System.Drawing.Size(55, 23);
            this.btnCouleur.TabIndex = 20;
            this.btnCouleur.TabStop = false;
            this.btnCouleur.Text = "Texte";
            this.toolTip1.SetToolTip(this.btnCouleur, "Choisissez la couleur à \r\nutiliser pour le texte.");
            this.btnCouleur.UseVisualStyleBackColor = true;
            this.btnCouleur.Click += new System.EventHandler(this.btnCouleur_Click);
            // 
            // btnValider
            // 
            this.btnValider.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnValider.Location = new System.Drawing.Point(10, 43);
            this.btnValider.Name = "btnValider";
            this.btnValider.Size = new System.Drawing.Size(75, 23);
            this.btnValider.TabIndex = 15;
            this.btnValider.Text = "Valider";
            this.btnValider.UseVisualStyleBackColor = true;
            this.btnValider.Click += new System.EventHandler(this.btnValider_Click);
            // 
            // btnAnnuler
            // 
            this.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnuler.Location = new System.Drawing.Point(91, 43);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new System.Drawing.Size(75, 23);
            this.btnAnnuler.TabIndex = 16;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.UseVisualStyleBackColor = true;
            // 
            // btnSurl
            // 
            this.btnSurl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSurl.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSurl.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnSurl.Location = new System.Drawing.Point(145, 10);
            this.btnSurl.Margin = new System.Windows.Forms.Padding(1);
            this.btnSurl.Name = "btnSurl";
            this.btnSurl.Size = new System.Drawing.Size(55, 23);
            this.btnSurl.TabIndex = 21;
            this.btnSurl.Text = "S\'lignage";
            this.toolTip1.SetToolTip(this.btnSurl, "Coicissez une couleur\r\nde surlignage.");
            this.btnSurl.UseVisualStyleBackColor = true;
            this.btnSurl.Click += new System.EventHandler(this.btnSurl_Click);
            // 
            // btnGraphemes
            // 
            this.btnGraphemes.Location = new System.Drawing.Point(172, 43);
            this.btnGraphemes.Name = "btnGraphemes";
            this.btnGraphemes.Size = new System.Drawing.Size(27, 23);
            this.btnGraphemes.TabIndex = 22;
            this.btnGraphemes.Text = "gr";
            this.toolTip1.SetToolTip(this.btnGraphemes, "Sélectionnez les graphèmes\r\nà coloriser.");
            this.btnGraphemes.UseVisualStyleBackColor = true;
            this.btnGraphemes.Click += new System.EventHandler(this.btnGraphemes_Click);
            // 
            // pbxBold
            // 
            this.pbxBold.Image = global::ColorizationControls.Properties.Resources.Bold;
            this.pbxBold.InitialImage = global::ColorizationControls.Properties.Resources.BoldSet;
            this.pbxBold.Location = new System.Drawing.Point(9, 10);
            this.pbxBold.Name = "pbxBold";
            this.pbxBold.Size = new System.Drawing.Size(22, 23);
            this.pbxBold.TabIndex = 3;
            this.pbxBold.TabStop = false;
            this.toolTip1.SetToolTip(this.pbxBold, "Gras");
            // 
            // pbxItalic
            // 
            this.pbxItalic.Image = global::ColorizationControls.Properties.Resources.Italic;
            this.pbxItalic.InitialImage = global::ColorizationControls.Properties.Resources.BoldSet;
            this.pbxItalic.Location = new System.Drawing.Point(31, 10);
            this.pbxItalic.Name = "pbxItalic";
            this.pbxItalic.Size = new System.Drawing.Size(22, 23);
            this.pbxItalic.TabIndex = 7;
            this.pbxItalic.TabStop = false;
            this.toolTip1.SetToolTip(this.pbxItalic, "Italique");
            // 
            // pbxUnderscore
            // 
            this.pbxUnderscore.Image = global::ColorizationControls.Properties.Resources.Underscore;
            this.pbxUnderscore.InitialImage = global::ColorizationControls.Properties.Resources.BoldSet;
            this.pbxUnderscore.Location = new System.Drawing.Point(53, 10);
            this.pbxUnderscore.Name = "pbxUnderscore";
            this.pbxUnderscore.Size = new System.Drawing.Size(22, 23);
            this.pbxUnderscore.TabIndex = 11;
            this.pbxUnderscore.TabStop = false;
            this.toolTip1.SetToolTip(this.pbxUnderscore, "Souligné");
            // 
            // CharFormatForm
            // 
            this.AcceptButton = this.btnValider;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnuler;
            this.ClientSize = new System.Drawing.Size(215, 76);
            this.Controls.Add(this.btnGraphemes);
            this.Controls.Add(this.btnCouleur);
            this.Controls.Add(this.btnSurl);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnValider);
            this.Controls.Add(this.pbxBold);
            this.Controls.Add(this.pbxItalic);
            this.Controls.Add(this.pbxUnderscore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CharFormatForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " ";
            this.Load += new System.EventHandler(this.CharFormatForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxBold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxItalic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxUnderscore)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCouleur;
        private System.Windows.Forms.PictureBox pbxBold;
        private System.Windows.Forms.PictureBox pbxItalic;
        private System.Windows.Forms.PictureBox pbxUnderscore;
        private System.Windows.Forms.Button btnValider;
        private System.Windows.Forms.Button btnAnnuler;
        private System.Windows.Forms.Button btnSurl;
        private System.Windows.Forms.Button btnGraphemes;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}