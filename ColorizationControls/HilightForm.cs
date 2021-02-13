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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ColorizationControls
{
    public partial class HilightForm : Form
    {
        /// <summary>
        /// nr of colors that are handled by the form.
        /// </summary>
        public const int nrColors = 16;

        /// <summary>
        /// array with nrColors RGB datasets.
        /// these colors are proposed in the form. The buttons are numberes from 0 to nrColors -1 and display
        /// the corresponding color in hiliColors.
        /// </summary>
        /// <remarks>
        /// If hiliCOlors is not set, it means that the application cannot handle hilighting. In this case, 
        /// <see cref="CanOperate"/> will return <c>false</c>
        /// </remarks>
        public static RGB[] hiliColors { set; private get; }

        /// <summary>
        /// indicates whether the selection of hilight colors is possible.
        /// no HilightForm should be crated if the response is <c>false</c>.
        /// </summary>
        /// <returns><c>true</c> if the selection of hilight colors is possible.</returns>
        public static bool CanOperate() => (hiliColors != null);


        private Button[] colButtons;
        private RGB selCol; // selected color

        public HilightForm(RGB selectedCol)
        {
            InitializeComponent();
            colButtons = new Button[nrColors];
            colButtons[0] = btnC0;
            colButtons[1] = btnC1;
            colButtons[2] = btnC2;
            colButtons[3] = btnC3;
            colButtons[4] = btnC4;
            colButtons[5] = btnC5;
            colButtons[6] = btnC6;
            colButtons[7] = btnC7;
            colButtons[8] = btnC8;
            colButtons[9] = btnC9;
            colButtons[10] = btnC10;
            colButtons[11] = btnC11;
            colButtons[12] = btnC12;
            colButtons[13] = btnC13;
            colButtons[14] = btnC14;
            colButtons[15] = btnC15;
            for (int i = 0; i < nrColors; i++)
                colButtons[i].BackColor = hiliColors[i];
            int j = 0;
            while ((j < nrColors) && (hiliColors[j] != selectedCol))
                j++;
            if (j < nrColors)
            {
                colButtons[j].FlatStyle = FlatStyle.Flat;
                colButtons[j].FlatAppearance.BorderSize = 2;
            }
            selCol = selectedCol;
        }

        public RGB GetSelectedColor() => selCol;

        private void btnC_Click(object sender, EventArgs e)
        {
            Button theBtn = (Button)sender;
            Debug.Assert(theBtn.Name.StartsWith("btnC"));
            string butNrTxt = theBtn.Name.Substring(4, theBtn.Name.Length - 4);
            int butNr = int.Parse(butNrTxt);
            selCol = hiliColors[butNr];
        }

        private void FormShown(object sender, EventArgs e)
        {
            btnACancel.Focus();
        }

    }
}
