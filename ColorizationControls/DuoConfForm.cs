using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorizationControls
{
    public partial class DuoConfForm : Form
    {
        private ConfigControl confContr1;

        public DuoConfForm()
        {
            InitializeComponent();
            this.SuspendLayout();
            confContr1 = new ConfigControl(this, this, "test");
            this.Controls.Add(confContr1);
            this.ResumeLayout();
        }
    }
}
