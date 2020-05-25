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
    public partial class DuoConfForm : Form
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private ConfigControl confContr1;
        private ConfigControl confContr2;
        Config theConf;

        public DuoConfForm(Config inConf)
        {
            InitializeComponent();
            theConf = inConf;
            this.SuspendLayout();

            confContr1 = new ConfigControl(theConf.subConfig1);
            panelConfig1.Controls.Add(confContr1);
            confContr2 = new ConfigControl(theConf.subConfig2);
            panelConfig2.Controls.Add(confContr2);
            theConf.updateAlternance = UpdateAlternance;
            theConf.updateColorisFunction = UpdateColorisFunction;
            UpdateAlternance();
            UpdateColorisFunction();

            this.ResumeLayout();
        }

        private void UpdateAlternance()
        {
            logger.ConditionalTrace("UpdateAlternance");
            if (theConf.alternance == Config.Alternance.lignes)
            {
                rbtnLignes.Checked = true;
                rbtnMots.Checked = false;
            }
            else if (theConf.alternance == Config.Alternance.mots)
            {
                rbtnLignes.Checked = false;
                rbtnMots.Checked = true;
            }
            else
            {
                logger.Error("Valeur d'alternance inattendue: {0}", theConf.alternance);
                // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                // Même si on a un peu de récusion dans cette affaire.
                theConf.SetAlternance(Config.Alternance.mots);
            }
        }

        private void UpdateColorisFunction()
        {
            logger.ConditionalTrace("UpdateColorisFunction");
            switch (theConf.colorisFunction)
            {
                case Config.ColorisFunction.syllabes:
                    rbtnSyylabes.Checked = true;
                    break;
                case Config.ColorisFunction.mots:
                    rbtnColorMots.Checked = true;
                    break;
                case Config.ColorisFunction.lettres:
                    rbtnLettres.Checked = true;
                    break;
                case Config.ColorisFunction.voyCons:
                    rbtnVoyCons.Checked = true;
                    break;
                case Config.ColorisFunction.phonemes:
                    rbtnPhonemes.Checked = true;
                    break;
                case Config.ColorisFunction.muettes:
                    rbtnMuettes.Checked = true;
                    break;
                case Config.ColorisFunction.undefined:
                default:
                    logger.Error("Valeur de \"colorisFunction\" inattendue: {0}", theConf.colorisFunction);
                    // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                    theConf.SetColorisFunction(Config.ColorisFunction.syllabes);
                    break;
            }
        }
    }
}
