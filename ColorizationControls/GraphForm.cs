using ColorLib;
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
    public partial class GraphForm : Form
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void SetGraphFormResult(string son, Dictionary<string, bool> graphemesConfig);

        /// <summary>
        /// Le sonpour lequel le <c>GraphForm</c> est ouvert.
        /// </summary>
        private string son;
        
        /// <summary>
        /// La fonction à appeler quand la nouvelle configuration de graphèmes est validée.
        /// </summary>
        private SetGraphFormResult setGFR;

        public GraphForm(string theSon, Dictionary<string, bool> grs, SetGraphFormResult inSetGFR)
        {
            logger.ConditionalDebug("CTOR GraphForm");
            InitializeComponent();
            son = theSon;

            // Titre de la fenêtre
            StringBuilder sb = new StringBuilder();
            sb.Append("Son ");
            sb.Append(ColConfWin.DisplayText(theSon));
            sb.Append(" ");
            sb.Append(ColConfWin.ExampleText(theSon));
            this.Text = sb.ToString();

            setGFR = inSetGFR;

            checkedListBox1.Items.Clear();
            int i = 0;
            foreach (KeyValuePair<string, bool> gr in grs)
            {
                checkedListBox1.Items.Add(gr.Key, gr.Value);
                i++;
            }
        }

        private void btnValider_Click(object sender, EventArgs e)
        {
            Dictionary<string, bool> grs = new Dictionary<string, bool>(checkedListBox1.Items.Count);
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                grs.Add(checkedListBox1.Items[i].ToString(), checkedListBox1.GetItemChecked(i));
            }
            setGFR(son, grs);
        }
    }
}
