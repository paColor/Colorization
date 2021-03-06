﻿/********************************************************************************
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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using ColorizationControls;
using ColorLib;
using System.Windows.Forms;

namespace ColorizationWord
{


    public partial class ColorizationMSW
    {
        public static ColorizationMSW thisAddIn { get; private set; }
        // the AddIn is instantiatied only once

        private static List<WordRibbon> wRibbonL = new List<WordRibbon>();
        // A priori il n'y a qu'un ruban d'instancié, mais je n'arrive pas à être 100% sûr.
        // Donc une liste :-)

        public static void RegisterRibbon(WordRibbon wr)
        {
            wRibbonL.Add(wr);
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            InitNLog.StartNLog();

            logger.ConditionalDebug("ThisAddIn_Startup");
            MSWText.Initialize();
            WordRibbon.Init();

            thisAddIn = this;

            foreach (WordRibbon wr in wRibbonL)
            {
                wr.InitHandlers(this.Application);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            logger.ConditionalDebug("ThisAddIn_Shutdown");
            InitNLog.CloseNLog();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
