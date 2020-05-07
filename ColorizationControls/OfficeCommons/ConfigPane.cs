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
using System.Threading.Tasks;
using Microsoft.Office.Tools;
using ColorLib;
using System.Windows.Forms;

namespace ColorizationControls
{
    /// <summary>
    /// ConfigPane contains the code necessary to instantiate and manage ConfigPanes. A ConfigPane instance can potentially
    /// exist for each document window within the application. In fact each window that can have a ribbon.
    /// </summary>
    /// <remarks>
    /// From the perspective of the layers in the solution, this class belongs to the layer of the addin, i.e. the top one.
    /// However, it is common to Word and PowerPoint. I decided not to create a namespace for this special layer that is 
    /// in fact the common part of the Office addin.
    /// </remarks>

    public class ConfigPane
    {
        // *************************************************** Static **********************************************************

        private static Dictionary<Object, ConfigPane> confTaskPanes = new Dictionary<Object, ConfigPane>();
        // key is the window the CongigPane is attached to.

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Opens the ConfigPane for the given Window. 
        /// </summary>
        /// <param name="theWin">The window the ConfigPane is attached to</param>
        /// <param name="theDoc">The document (or presentation in PPT) that is opened in <c>theWin</c></param>
        /// <param name="inCustomTaskPanes">The <c>CustomTaskPaneCOllection the ConfigPane should be added to.</c></param>
        /// <param name="version">The version of the software as should be displayed in the pannel </param>
        /// <remarks>We assume that there can be several windows for the same document</remarks>
        public static void MakePaneVisibleInWin(Object theWin, Object theDoc, CustomTaskPaneCollection inCustomTaskPanes,
            string version)
        {
            logger.ConditionalTrace("MakePaneVisibleInWin");
            ConfigPane theCP;
            if (!confTaskPanes.TryGetValue(theWin, out theCP))
            {
                theCP = new ConfigPane(theWin, theDoc, inCustomTaskPanes, version);
                confTaskPanes.Add(theWin, theCP);
            } 
            else
            {
                theCP.confContr.UpdateAll();
                // formellement, ceci n'est pas nécessaire. Néanmoins, ça donne un peu de robustesse. Si l'utilisateur constate
                // qqch de bizarre, il peut fermer et réouvrir le panneau qui se remettra à jour.
            }

            theCP.configTaskPane.Visible = true;
        }

        /// <summary>
        /// Handler of the Document closed event. It mus be called when a document is closed. Since <paramref name="inDoc"/> is of type 
        /// <c>Object</c>, the application specific ribbon that manages the ConfigPanes openings and closings, must have its own 
        /// handler that calls <c>DocClosed</c>. We handle here <c>Application.PresentationClose</c>
        /// </summary>
        /// <param name="inDoc">The document that is closed.</param>
        /// <remarks>
        /// <para>
        /// Je voulais à l'origine traiter l'événement Close de la classe Document. D'après mes tests, il n'est pas déclenché
        /// lorsque le même document est ouvert dans plusieurs fenêtres et que la dernière est fermée. Par contre l'évèenement de 
        /// l'Application, lui, a lieu. Je prends donc celui-là, même si la sémantique est potentiellement différente.
        /// </para>
        /// </remarks>
        public static void DocClosed(Object inDoc)
        {
            logger.ConditionalTrace("DocClosed");
            List<Object> winToRemove = new List<Object>(1); // pas sûr qu'on puisse enlever des élément à une liste qu'on traverse...
            foreach (ConfigPane theCP in confTaskPanes.Values)
            {
                if (Object.ReferenceEquals(theCP.theDoc, inDoc))
                {
                    winToRemove.Add(theCP.theWin);
                    theCP.Close();
                }
            }
            foreach (Object w in winToRemove)
            {
                confTaskPanes.Remove(w);
            }
            Config.DocClosed(inDoc); // informaer Config
        }


        // *************************************************** Instantiated ******************************************************

        private CustomTaskPaneCollection customTaskPanes; // the collection the taskPane is added to.
        private ConfigControl confContr; // the ConfigControl
        private Microsoft.Office.Tools.CustomTaskPane configTaskPane; // the pane the control is 'included' into
        private Object theWin; // the window the pane is attached to
        private Object theDoc;

        private ConfigPane(Object inWin, Object inDoc, CustomTaskPaneCollection inCustomTaskPanes, string version)
        {
            const int OrigWidth = 363;
            logger.ConditionalTrace("ConfigPane");
            theWin = inWin;
            theDoc = inDoc;
            customTaskPanes = inCustomTaskPanes;
            confContr = new ConfigControl(inWin, inDoc, version);
            configTaskPane = customTaskPanes.Add(confContr, BaseConfig.ColorizationName, theWin);

            double dimWidth;
            if (confContr.AutoScaleMode == AutoScaleMode.Dpi)
                dimWidth = 96; // value observed on the development machine
            else if (confContr.AutoScaleMode == AutoScaleMode.Font)
                dimWidth = 6; // value observed on the development machine
            else
            {
                dimWidth = confContr.AutoScaleDimensions.Width;
                logger.Warn("Unexpected AutoScaleMode encountered. Scaling may not work properly.");
            }

            double factor = confContr.CurrentAutoScaleDimensions.Width / dimWidth;
            configTaskPane.Width = ((int)(OrigWidth * factor)) + 3;
            
            logger.Info("confContr.CurrentAutoScaleDimensions.Width == {0}", confContr.CurrentAutoScaleDimensions.Width);
            logger.Info("confContr.AutoScaleDimensions.Width == {0}", confContr.AutoScaleDimensions.Width);
            logger.Info("factor == {0}", factor);
            logger.Info("AutoScaleMode is {0}", confContr.AutoScaleMode.ToString());
        }

        internal void Close()
        {
            logger.ConditionalTrace("Close");
            // since we handle the doc closing events, it is possible to close a window without having the 
            // corresponding entry removed here (if the document is opened in multiple windows). In this case 
            // I was once able to produce a situation where the corresponding configTaskPane was removed from 
            // customTaskPanes in the background, and when we came here, it crashed. The pane was visible in the window that was closed
            // and was reused in a new window I created. So in order to avoid crashing in this strange
            // situation let's catch the exception. 
            // The only way I can think of handlicng this cleanly would be with a window closing event, but I did not find out how to 
            // get it (in word)...
            // I could also try to work on open events, but there is no window open event
            // Note that the Document.Close event does not fire at all in case of multiple windows on the same document
            // which is exactly the problem I am trying to address... 
            try
            {
                configTaskPane.Visible = false;
                customTaskPanes.Remove(configTaskPane);
                configTaskPane.Dispose();
                confContr.Dispose();
            }
            catch   // I do not know which kind of exception is raised in my strange case... We assume it means that everything
                    // was already cleaned up.
            {
                logger.Error("Exception when closing a ConfigTaskPane");
            }
            
        }

    }
}
