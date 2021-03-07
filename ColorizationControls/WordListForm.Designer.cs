
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxSyllabes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxMots)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(427, 426);
            this.textBox1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(500, 413);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAnnuler
            // 
            this.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnuler.Location = new System.Drawing.Point(500, 384);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new System.Drawing.Size(75, 23);
            this.btnAnnuler.TabIndex = 2;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(450, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 39);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choicissez ci-dessous quels\r\noutils doivent ne pas coloriser\r\nles mots de la list" +
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
    "e, espace, \r\nsaut de ligne, ...\r\nL\'utilisation de minuscules\r\nsuffit";
            // 
            // cbxSyllabes
            // 
            this.cbxSyllabes.AutoSize = true;
            this.cbxSyllabes.Location = new System.Drawing.Point(487, 156);
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
            this.cbxMots.Location = new System.Drawing.Point(487, 180);
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
            this.cbxArcs.Location = new System.Drawing.Point(487, 204);
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
            this.pictBoxSyllabes.Location = new System.Drawing.Point(465, 155);
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
            this.pictBoxMots.Location = new System.Drawing.Point(465, 179);
            this.pictBoxMots.Name = "pictBoxMots";
            this.pictBoxMots.Size = new System.Drawing.Size(16, 16);
            this.pictBoxMots.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictBoxMots.TabIndex = 9;
            this.pictBoxMots.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ColorizationControls.Properties.Resources.syll_26;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(465, 203);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // WordListForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnnuler;
            this.ClientSize = new System.Drawing.Size(623, 450);
            this.Controls.Add(this.pictureBox1);
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
            this.Text = "Liste des mots à ne pas coloriƨer";
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxSyllabes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBoxMots)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}