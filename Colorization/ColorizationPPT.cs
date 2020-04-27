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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using ColorLib;
using ColorizationControls;
using System.Windows.Forms;
using NLog;

namespace Colorization
{
    public partial class ColorizationPPT
    {
        public static ColorizationPPT thisAddIn { get; private set; }
            // the AddIn is instantiatied only once

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            InitNLog.StartNLog();

            logger.Info("ThisAddIn_Startup {0}", this.Application.Name);

            PPTText.Initialize();
            Ribbon1.Init();

            thisAddIn = this;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            logger.Info("ThisAddIn_Shutdown {0}", this.Application.Name);
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
