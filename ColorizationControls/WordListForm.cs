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

            MatchCollection matches = TheText.rxWords.Matches(ExcMots.texte);

            ExcMots.exceptMots = new HashSet<string>(matches.Count);
            foreach (Match m in matches)
            {
                ExcMots.exceptMots.Add(m.Value);
            }

            ExcMots.exceptMotsSyls = new HashSet<string>(matches.Count);
            int i = 0;
            while (i < matches.Count)
            {
                Match m = matches[i];
                int beg = m.Index;
                int end = beg + m.Length - 1;

                //Voir commentaire su le traitement de l'apostrophe dans TheText.
                if ((m.Length <= 2)
                    && (end + 1 < ExcMots.texte.Length)
                    && ((ExcMots.texte[end + 1] == '\'')
                        || (ExcMots.texte[end + 1] == '’')
                        || (m.Value == "t" && ExcMots.texte[end + 1] == '-')))
                {
                    if (i < matches.Count - 1)
                    {
                        Match nextMatch = matches[i + 1];
                        end = nextMatch.Index + nextMatch.Length - 1;
                        i++;
                    }
                    else
                    {
                        end++;
                    }
                }
                string mot = ExcMots.texte.Substring(beg, end - beg + 1);
                ExcMots.exceptMotsSyls.Add(mot);
                i++;
            }
            DialogResult = DialogResult.OK;
        }

    }
}
