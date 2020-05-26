/********************************************************************************
 *  Copyright 2020, Pierre-Alain Etique                                         *
 *                                                                              *
 *  This file is part of Coloriƨation.                                          *
 *                                                                              *
 *  Coloriƨation is free software: you can redistribute it and/or modify        *
 *  it under the terms of the GNU General Public License as published by        *
 *  the Free Software Foundation, either version 3 of the License, or           *
 *  (at your option) any later version.                                         *
 *                                                                              *
 *  Coloriƨation is distributed in the hope that it will be useful,             *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *  GNU General Public License for more details.                                *
 *                                                                              *
 *  You should have received a copy of the GNU General Public License           *
 *  along with Coloriƨation.  If not, see <https://www.gnu.org/licenses/>.      *
 *                                                                              *
 ********************************************************************************/

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

            confContr1 = new ConfigControl(theConf.duoConf.subConfig1);
            panelConfig1.Controls.Add(confContr1);
            confContr2 = new ConfigControl(theConf.duoConf.subConfig2);
            panelConfig2.Controls.Add(confContr2);
            theConf.duoConf.AlternanceChanged += UpdateAlternance;
            theConf.duoConf.ColorisFunctionChanged += UpdateColorisFunction;
            UpdateAlternance(this, EventArgs.Empty);
            UpdateColorisFunction(this, EventArgs.Empty);

            this.ResumeLayout();
        }

        private void UpdateAlternance(object sender, EventArgs e)
        {
            logger.ConditionalTrace("UpdateAlternance");
            if (theConf.duoConf.alternance == DuoConfig.Alternance.lignes)
            {
                rbtnLignes.Checked = true;
                rbtnMots.Checked = false;
            }
            else if (theConf.duoConf.alternance == DuoConfig.Alternance.mots)
            {
                rbtnLignes.Checked = false;
                rbtnMots.Checked = true;
            }
            else
            {
                logger.Error("Valeur d'alternance inattendue: {0}", theConf.duoConf.alternance);
                // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                // Même si on a un peu de récusion dans cette affaire.
                theConf.duoConf.alternance = DuoConfig.Alternance.mots;
            }
        }

        private void UpdateColorisFunction(object sender, EventArgs e)
        {
            logger.ConditionalTrace("UpdateColorisFunction");
            switch (theConf.duoConf.colorisFunction)
            {
                case DuoConfig.ColorisFunction.syllabes:
                    rbtnSyylabes.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.mots:
                    rbtnColorMots.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.lettres:
                    rbtnLettres.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.voyCons:
                    rbtnVoyCons.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.phonemes:
                    rbtnPhonemes.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.muettes:
                    rbtnMuettes.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.undefined:
                default:
                    logger.Error("Valeur de \"colorisFunction\" inattendue: {0}", theConf.duoConf.colorisFunction);
                    // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                    theConf.duoConf.colorisFunction = DuoConfig.ColorisFunction.syllabes;
                    break;
            }
        }

        private void btnValider_Click(object sender, EventArgs e)
        {

        }
    }
}
