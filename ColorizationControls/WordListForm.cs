using ColorLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorizationControls
{
    public partial class WordListForm : Form
    {
        /// <summary>
        /// Le texte affiché dans la fenêtre et les checkboxes. Peut être lu à la fermeture de la
        /// fenêtre si fermé par OK. Dans toutes les autres situations est égal à <c>null</c>.
        /// </summary>
        /// <remarks>Il est conseillé de conserver ces données et de les utiliser à la 
        /// prochaine ouverture de la fenêtre. De cette manière, l'utilisateur retrouve
        /// ce qu'il a entré.</remarks>
        public ExceptionMots ExcMots { get; private set; }

        /// <summary>
        /// Crée la <see cref="WordListForm"/>
        /// </summary>
        /// <param name="gMots">Les informations à afficehr à l'ouverture. <c>null</c> s'il n'y
        /// a pas de données à afficher.</param>
        public WordListForm(ExceptionMots gMots)
        {
            InitializeComponent();
            ExcMots = null;
            if (gMots != null)
            {
                textBox1.Text = gMots.texte;
                cbxArcs.Checked = gMots.arcs;
                cbxMots.Checked = gMots.mots;
                cbxSyllabes.Checked = gMots.syllabes;
                cbxPhonemes.Checked = gMots.phonemes;
                cbxLettres.Checked = gMots.lettres;
                cbxVoyCons.Checked = gMots.voyCons;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ExcMots = new ExceptionMots();
            ExcMots.texte = textBox1.Text;
            ExcMots.arcs = cbxArcs.Checked;
            ExcMots.mots = cbxMots.Checked;
            ExcMots.syllabes = cbxSyllabes.Checked;
            ExcMots.phonemes = cbxPhonemes.Checked;
            ExcMots.lettres = cbxLettres.Checked;
            ExcMots.voyCons = cbxVoyCons.Checked;

            ExcMots.exceptMots = new HashSet<string>();
            MatchCollection matches = TheText.rxWords.Matches(ExcMots.texte);
            foreach (Match m in matches)
            {
                ExcMots.exceptMots.Add(ExcMots.texte.Substring(m.Index, m.Length));
            }
            DialogResult = DialogResult.OK;
        }

        private void btnTrier_Click(object sender, EventArgs e)
        {
            string texte = textBox1.Text;
            SortedSet<string> mots = new SortedSet<string>();
            MatchCollection matches = TheText.rxWords.Matches(texte);
            foreach (Match m in matches)
            {
                string mot = texte.Substring(m.Index, m.Length);
                if (!mots.Contains(mot))
                {
                    mots.Add(mot);
                }
            }
            StringBuilder sb = new StringBuilder(texte.Length);
            foreach (string mot in mots)
            {
                sb.Append(mot);
                sb.Append(" ");
            }
            textBox1.Text = sb.ToString();
        }
    }
}
