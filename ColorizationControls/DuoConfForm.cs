/********************************************************************************
 *  Copyright 2020 - 2021, Pierre-Alain Etique                                  *
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
    /// <summary>
    /// Classe Windows.Forms pour la gestion de la fenêtre de configuration d'une <see cref="DuoConfig"/>.
    /// </summary>
    public partial class DuoConfForm : Form
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // les deux ConfigControl à afficher.
        private ConfigControl confContr1;
        private ConfigControl confContr2;

        // La config sur laquelle on travaille
        private Config theConf; // La Config dont le duoConf doit être édité.
        private Config theConfCopy; // une copie de theConf.
        // theConfCopy.duoConf qui sera éditée et qui remplacera theConf.duoConf si l'utilisateur
        // clique "Valider"
        private DuoConfig duoConfCopy; 

        // Le RTBText correspondant à la RichTextBox affichée dans le contrôle.
        private RTBText rTBText;

        // Un flag indiquant qu'un reset d'au moins une des deux configs affichées
        // est en cours. Permet d'éviter de remettre à jour rTBText pour chaque paramètre des
        // Configs qui sont réinitialisés.
        private bool _resetting;
        private bool resetting
        {
            get
            {
                return _resetting;
            }
            set
            {
                if (value != _resetting)
                {
                    _resetting = value;
                    if (_resetting == false)
                    {
                        UpdateRichTextBox(this, EventArgs.Empty);
                    }
                }
            }
        }

        public DuoConfForm(Config inConf)
        {
            InitializeComponent();
            theConf = inConf;

            // Faisons une copie de duoConf qui sera éditée. Si l'utilisateur clique "Valider" on 
            // pourra l'utiliser sinon on pourra la jeter.

            theConfCopy = theConf.DeepCopy();
            duoConfCopy = theConfCopy.duoConf;
            this.SuspendLayout();

            confContr1 = new ConfigControl(duoConfCopy.subConfig1);
            panelConfig1.Controls.Add(confContr1);
            confContr2 = new ConfigControl(duoConfCopy.subConfig2);
            panelConfig2.Controls.Add(confContr2);
            rTBText = new RTBText(rtbUlysse);
            resetting = false;

            duoConfCopy.AlternanceModifiedEvent += UpdateAlternance;
            duoConfCopy.AlternanceModifiedEvent += UpdateRichTextBox; 
            duoConfCopy.ColorisFunctionModifiedEvent += UpdateColorisFunction;
            duoConfCopy.ColorisFunctionModifiedEvent += UpdateRichTextBox;
            duoConfCopy.NbreAltModifiedEvent += UpdateNbreAlt;
            duoConfCopy.NbreAltModifiedEvent += UpdateRichTextBox;

            RegisterConfigEvents(duoConfCopy.subConfig1);
            RegisterConfigEvents(duoConfCopy.subConfig2);
            
            UpdateAlternance(this, EventArgs.Empty);
            UpdateColorisFunction(this, EventArgs.Empty);
            UpdateNbreAlt(this, EventArgs.Empty);
            UpdateRichTextBox(this, EventArgs.Empty);

            this.ResumeLayout();
        }

        private void UpdateAlternance(object sender, EventArgs e)
        {
            logger.ConditionalDebug("UpdateAlternance val: ´\'{0}\'", duoConfCopy.alternance);
            if (duoConfCopy.alternance == DuoConfig.Alternance.lignes)
            {
                rbtnLignes.Checked = true;
                rbtnMots.Checked = false;
            }
            else if (duoConfCopy.alternance == DuoConfig.Alternance.mots)
            {
                rbtnLignes.Checked = false;
                rbtnMots.Checked = true;
            }
            else
            {
                logger.Error("Valeur d'alternance inattendue: {0}", duoConfCopy.alternance);
                // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                // Même si on a un peu de récusion dans cette affaire.
                duoConfCopy.alternance = DuoConfig.Alternance.mots;
            }

        }

        private void UpdateColorisFunction(object sender, EventArgs e)
        {
            logger.ConditionalDebug("UpdateColorisFunction val: ´\'{0}\'", duoConfCopy.colorisFunction);
            switch (duoConfCopy.colorisFunction)
            {
                case DuoConfig.ColorisFunction.syllabes:
                    rbtnSyylabes.Checked = true;
                    break;
                case DuoConfig.ColorisFunction.arcs:
                    rbtnArcs.Checked = true;
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
                    logger.Error("Valeur de \"colorisFunction\" inattendue: {0}", duoConfCopy.colorisFunction);
                    // Essayons de sauver les meubles et mettons-nous dans une situation claire.
                    duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.syllabes;
                    break;
            }
        }

        private void UpdateNbreAlt(object sender, EventArgs e)
        {
            logger.ConditionalDebug("UpdateNbreAlt new:´\'{0}\'", duoConfCopy.nbreAlt);
            nudNbreAlt.Value = duoConfCopy.nbreAlt;
        }

        private void UpdateRichTextBox(object sender, EventArgs e)
        {
            logger.ConditionalTrace("UpdateRichTextBox");
            if (!resetting)
            {
                ProgressNotifier.thePN.Start();
                rTBText.MarkNoir(theConfCopy);
                rTBText.MarkDuo(theConfCopy);
                ProgressNotifier.thePN.Completed();
            }
                
        }

        private void RegisterConfigEvents(Config c)
        {
            logger.ConditionalDebug("RegisterConfigEvents config: \'{0}\'", c.GetConfigName());
            c.ConfigReplacedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].DefBehModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].IllModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].SonCBModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].SonCharFormattingModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.muettes].DefBehModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.muettes].IllModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.muettes].SonCBModifiedEvent += UpdateRichTextBox;
            c.colors[PhonConfType.muettes].SonCharFormattingModifiedEvent += UpdateRichTextBox;
            c.pBDQ.LetterButtonModifiedEvent += UpdateRichTextBox;
            c.pBDQ.MarkAsBlackModifiedEvent += UpdateRichTextBox;
            c.sylConf.DoubleConsStdModifiedEvent += UpdateRichTextBox;
            c.sylConf.ModeModifiedEvent += UpdateRichTextBox;
            c.sylConf.SylButtonModifiedEvent += UpdateRichTextBox;
            c.sylConf.MarquerMuettesModified += UpdateRichTextBox;
            c.sylConf.ChercherDiereseModified += UpdateRichTextBox;
            c.sylConf.NbrPiedsModified += UpdateRichTextBox;
            c.unsetBeh.CheckboxUnsetModifiedEvent += UpdateRichTextBox;
            c.ConfigReplacedEvent += HandleConfigReplaced;
        }

        private void UnregisterConfigEvents(Config c)
        {
            logger.ConditionalDebug("UnregisterConfigEvents config: \'{0}\'", c.GetConfigName());
            c.ConfigReplacedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].DefBehModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].IllModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].SonCBModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.phonemes].SonCharFormattingModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.muettes].DefBehModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.muettes].IllModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.muettes].SonCBModifiedEvent -= UpdateRichTextBox;
            c.colors[PhonConfType.muettes].SonCharFormattingModifiedEvent -= UpdateRichTextBox;
            c.pBDQ.LetterButtonModifiedEvent -= UpdateRichTextBox;
            c.pBDQ.MarkAsBlackModifiedEvent -= UpdateRichTextBox;
            c.sylConf.DoubleConsStdModifiedEvent -= UpdateRichTextBox;
            c.sylConf.ModeModifiedEvent -= UpdateRichTextBox;
            c.sylConf.SylButtonModifiedEvent -= UpdateRichTextBox;
            c.sylConf.MarquerMuettesModified -= UpdateRichTextBox;
            c.sylConf.ChercherDiereseModified -= UpdateRichTextBox;
            c.sylConf.NbrPiedsModified -= UpdateRichTextBox;
            c.unsetBeh.CheckboxUnsetModifiedEvent -= UpdateRichTextBox;
            c.ConfigReplacedEvent -= HandleConfigReplaced;
        }

        private void HandleConfigReplaced(object sender, ConfigReplacedEventArgs e)
        {
            Config oldConf = (Config)sender;
            logger.ConditionalDebug("HandleConfigReplaced old: \'{0}\', new: \'{1}\'", 
                oldConf.GetConfigName(), e.newConfig.GetConfigName());
            UnregisterConfigEvents(oldConf);
            RegisterConfigEvents(e.newConfig);
        }

        private void btnValider_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnValider_Click");
            theConf.duoConf = duoConfCopy;
            this.Dispose();
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnAnnuler_Click");
            this.Dispose();
        }

        private void btnDefConf1_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnDefConf1_Click");
            resetting = true;
            duoConfCopy.subConfig1.Reset();
            resetting = false;
        }

        private void btnDefConf2_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnDefConf2_Click");
            resetting = true;
            duoConfCopy.subConfig2.Reset();
            resetting = false;
        }

        private void btnDefaut_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnDefaut_Click");
            resetting = true;
            duoConfCopy.Reset();
            resetting = false;
        }

        private void rbtnSyylabes_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnSyylabes_CheckedChanged, chk: \'{0}\'", rbtnSyylabes.Checked);
            if (rbtnSyylabes.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.syllabes;
            }
        }

        private void rbtnColorMots_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnColorMots_CheckedChanged, chk: \'{0}\'", rbtnColorMots.Checked);
            if (rbtnColorMots.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.mots;
            }
        }

        private void rbtnLettres_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnLettres_CheckedChanged, chk: \'{0}\'", rbtnLettres.Checked);
            if (rbtnLettres.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.lettres;
            }
        }

        private void rbtnVoyCons_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnVoyCons_CheckedChanged, chk: \'{0}\'", rbtnVoyCons.Checked);
            if (rbtnVoyCons.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.voyCons;
            }
        }

        private void rbtnPhonemes_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnPhonemes_CheckedChanged, chk: \'{0}\'", rbtnPhonemes.Checked);
            if (rbtnPhonemes.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.phonemes;
            }
        }

        private void rbtnMuettes_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnMuettes_CheckedChanged, chk: \'{0}\'", rbtnMuettes.Checked);
            if (rbtnMuettes.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.muettes;
            }
        }

        private void rbtnMots_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnMots_CheckedChanged, chk: \'{0}\'", rbtnMots.Checked);
            if (rbtnMots.Checked)
            {
                duoConfCopy.alternance = DuoConfig.Alternance.mots;
            }
        }

        private void rbtnLignes_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnLignes_CheckedChanged, chk: \'{0}\'", rbtnLignes.Checked);
            if (rbtnLignes.Checked)
            {
                duoConfCopy.alternance = DuoConfig.Alternance.lignes;
            }
        }

        private void nudNbreAlt_ValueChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("nudNbreAlt_ValueChanged, val: \'{0}\'", nudNbreAlt.Value);
            duoConfCopy.nbreAlt = (int)nudNbreAlt.Value;
        }

        private void rbtnArcs_CheckedChanged(object sender, EventArgs e)
        {
            logger.ConditionalDebug("rbtnArcs_CheckedChanged, chk: \'{0}\'", rbtnArcs.Checked);
            if (rbtnArcs.Checked)
            {
                duoConfCopy.colorisFunction = DuoConfig.ColorisFunction.arcs;
            }
        }
    }
}
