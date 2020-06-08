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
using System.Text;
using System.Globalization;
using System.Windows.Forms;

namespace ColorLib
{
    public static class BaseConfig
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static CultureInfo cultF = new CultureInfo("fr-FR");
        public const string ColorizationName = "Coloriƨation";

        private const string colorizationDirName = "Colorization";
        public static readonly string colorizationDirPath =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), colorizationDirName);

        public static void Init()
        {
            logger.ConditionalDebug("Init");
            // Ensure that colorizationDirPath folder does exist
            if (!System.IO.Directory.Exists(colorizationDirPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(colorizationDirPath);
                    logger.Info("Dossier {0} créé.", colorizationDirPath);
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show("Impossible de créer le répertoire" + colorizationDirPath);
                    logger.Error("Impossible de créer le répertoire {0}. Erreur {1}", colorizationDirPath, e.Message);
                }
            }
        }

    }
}
