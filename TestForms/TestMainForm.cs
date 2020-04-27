using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorizationControls;

namespace TestForms
{
    public partial class TestMainForm : Form
    {
        MyColorDialog cd;

        public TestMainForm()
        {
            InitializeComponent();
            cd = new MyColorDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // _ = cd.ShowDialog();



            //RGB theCol = new RGB(161, 34, 231);
            //CharFormatForm form = new CharFormatForm();
            //form.SetBtnCouleur(theCol);
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    MessageBox.Show("OK");
            //} else
            //{
            //    MessageBox.Show("Not OK");
            //}
            //form.Dispose();
        }
    }
}
