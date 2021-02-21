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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorLib;

namespace ColorizationControls
{
    public partial class CharFormatForm : Form
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void SetCharFormResult (string buttonName, CharFormatting cf);
        

        private string son;
        private CharFormatting inCF;
        private bool bold = false;
        private bool italic = false;
        private bool underscore = false;
        private bool caps = false;
        private bool colorSet; // stores the corect value for the CharFormatting
        private RGB theColor;
        private bool hilightSet; // stores the corect value for the CharFormatting
        private RGB theHilightColor;

        private MyColorDialog mcd;
        private SetCharFormResult charFormResult; // la fonction à appeler quand le nouveau format est connu

        private FormatButtonHandler2 bHandler, iHandler, uHandler;

        /// <summary>
        /// Ouvre une fenêtre pour le choix des paramètres de formatage des caractères.
        /// </summary>
        /// <param name="cf">La configuration de départ à afficher.</param>
        /// <param name="theSon">Le nom du son à formater. Ne doit pas obligatoirement être un son.
        /// Est utilisé lors de l'appel de la méthode pour stocker le résultat.</param>
        /// <param name="displayText">Le texte à afficher comme titre de la fenêtre.</param>
        /// <param name="inCharFormResult">La méthode à appeler pour communiquer le nouveau
        /// <see cref="CharFormatting"/>. <paramref name="theSon"/> est passé également lors de cet
        /// appel.</param>
        public CharFormatForm(CharFormatting cf, string theSon, 
            string displayText, SetCharFormResult inCharFormResult)
        {
            logger.ConditionalDebug("CTOR CharFormatForm");
            InitCtor(cf, theSon, displayText, inCharFormResult);
        }

        /// <summary>
        /// Constructeur où il est connu que <paramref name="theSon"/> est effectivement un son.
        /// Le titre de la fenêtre est alors calculé automatiquement en utilisant les possibilités
        /// de <see cref="ColConfWin"/>.
        /// </summary>
        /// <param name="cf">Le formatage initial.</param>
        /// <param name="theSon">Le son (au sens <see cref="ColConfWin"/> pour lequel le formatage
        /// est édité.</param>
        /// <param name="inCharFormResult">La méthode à appeler pour communiquer le nouveau
        /// <see cref="CharFormatting"/>. A noter que <paramref name="theSon"/> est également 
        /// transmis à cette méthode.</param>
        public CharFormatForm(CharFormatting cf, string theSon, SetCharFormResult inCharFormResult)
        {
            logger.ConditionalDebug("CTOR CharFormatForm");
            InitCtor(cf, theSon, FormName(theSon), inCharFormResult);
        }

        private void InitCtor(CharFormatting cf, string theSon,
            string displayText, SetCharFormResult inCharFormResult)
        {
            logger.ConditionalDebug("InitCtor");

            InitializeComponent();
            if (!HilightForm.CanOperate())
            {
                // We are not in Word
                btnCouleur.Width = 114;
                btnSurl.Visible = false;
                btnSurl.Enabled = false;
            }
            mcd = new MyColorDialog();
            mcd.CustomColors = StaticColorizControls.customColors;
            mcd.AnyColor = true;
            mcd.FullOpen = true;
            mcd.Color = cf.color;
            colorSet = true; // Si le bouton "valider" est cliqué, la couleur doit être la couleur mise.
            inCF = cf;
            theColor = cf.color;
            hilightSet = cf.changeHilight;
            theHilightColor = cf.hilightColor;
            bold = cf.bold;
            italic = cf.italic;
            underscore = cf.underline;
            son = theSon;

            bHandler = new FormatButtonHandler2(pbxBold, Properties.Resources.Bold, Properties.Resources.BoldSet,
                Properties.Resources.BoldPressed, Properties.Resources.BoldSetMouseOn1, SetBold, UnsetBold, bold);
            iHandler = new FormatButtonHandler2(pbxItalic, Properties.Resources.Italic, Properties.Resources.ItalicSet,
                Properties.Resources.ItalicPressed, Properties.Resources.ItalicSetOver, SetItalic, UnsetItalic, italic);
            uHandler = new FormatButtonHandler2(pbxUnderscore, Properties.Resources.Underscore, Properties.Resources.UnderscoreSet,
                Properties.Resources.UnderscorePressed, Properties.Resources.UnderscoreSetOver, SetUnderscore, UnsetUnderscore, underscore);

            btnCouleur.BackColor = theColor;
            btnSurl.BackColor = theHilightColor;
            this.Text = displayText;
            charFormResult = inCharFormResult;
        }

        protected virtual string FormName(string son)
        {
            logger.ConditionalDebug("FormName {0}", son);
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Text);
            sb.Append(" ");
            sb.Append(ColConfWin.DisplayText(son));
            sb.Append(" ");
            sb.Append(ColConfWin.ExampleText(son));
            return sb.ToString();
        }

        private void CharFormatForm_Load(object sender, EventArgs e)
        {
            logger.ConditionalDebug("CharFormatForm_Load");
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------  BOLD  ----------------------------------
        //-----------------------------------------------------------------------------

        private void SetBold() => bold = true;
        private void UnsetBold() => bold = false;

        //-----------------------------------------------------------------------------
        //-----------------------------------  Italic  --------------------------------
        //-----------------------------------------------------------------------------

        private void SetItalic() => italic = true;
        private void UnsetItalic() => italic = false;

        //-----------------------------------------------------------------------------------
        //-----------------------------------  Underscore  ----------------------------------
        //-----------------------------------------------------------------------------------

        private void SetUnderscore() => underscore = true;
        private void UnsetUnderscore() => underscore = false;
       

        //-----------------------------------------------------------------------------
        //------------------------------------  Color  --------------------------------
        //-----------------------------------------------------------------------------

        private void btnCouleur_Click(object sender, EventArgs e)
        {
            Button theBtn = (Button)sender;
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            p.Offset(-450, -100);
            mcd.SetPos(p);
            if (mcd.ShowDialog() == DialogResult.OK)
            {
                colorSet = true;
                theColor = mcd.Color;
                btnCouleur.BackColor = mcd.Color;
                StaticColorizControls.customColors = mcd.CustomColors;
            }
            btnValider.Focus();
        }

        //-----------------------------------------------------------------------------
        //---------------------------------  Surlignage  ------------------------------
        //-----------------------------------------------------------------------------

        private void btnSurl_Click(object sender, EventArgs e)
        {
            logger.ConditionalDebug("btnSurl_Click");
            Debug.Assert(HilightForm.CanOperate());
            Button theBtn = (Button)sender;
            Point p = theBtn.PointToScreen(((MouseEventArgs)e).Location); // Mouse position relative to the screen
            HilightForm hiForm = new HilightForm(inCF);
            p.Offset(-hiForm.Width, -(hiForm.Height / 2));
            hiForm.Location = p;
            if (hiForm.ShowDialog() == DialogResult.OK)
            {
                hilightSet = hiForm.ResultCF.changeHilight;
                theHilightColor = hiForm.ResultCF.hilightColor;
                btnSurl.BackColor = theHilightColor;
            }
            hiForm.Dispose();
            btnValider.Focus();
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------  Valider  -------------------------------
        //-----------------------------------------------------------------------------

        private void btnValider_Click(object sender, EventArgs e)
        {
            charFormResult(son, new CharFormatting(bold, italic, underscore, caps, colorSet, theColor,
                           hilightSet, theHilightColor));
        }

        
    }
}
