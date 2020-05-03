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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace ColorLib
{
    [Serializable]
    public enum PhonConfType { phonemes, muettes }

    [Serializable]
    public class Config
    {
        // *************************************************** Static **********************************************************
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string ConfigDirName = "Config";
        private static readonly string ConfigDirPath = 
            Path.Combine(BaseConfig.colorizationDirPath, ConfigDirName);
        private const string DefaultFileName = "ClrzConfig";
        private const string ClrzExtension = ".clrz";
        private static readonly string DefaultConfFile = Path.Combine(ConfigDirPath, DefaultFileName + ClrzExtension);


        private static Dictionary<Object, Config> theConfs; // key is a window
        private static Dictionary<Object, List<Object>> doc2Win; // key is document, value is list of windows

        public static void Init()
        {
            logger.ConditionalTrace("Init");
            ColConfWin.Init();
            // Ensure that ConfigDirPath folder does exist
            if (!System.IO.Directory.Exists(ConfigDirPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(ConfigDirPath);
                    logger.Info("Dossier {0} créé.", ConfigDirPath);
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show("Impossible de créer le répertoire" + ConfigDirPath);
                    logger.Error("Impossible de créer le répertoire {0}. Erreur {1}", ConfigDirPath, e.Message);
                }
            }
            theConfs = new Dictionary<object, Config>();
            doc2Win = new Dictionary<object, List<object>>();
        }

        public static Config GetConfigFor(Object win, Object doc)
            // returns the Config associated with the Object, normally the active window. 
            // if there is none, a new one with the defauilt config is created.
        {
            logger.ConditionalTrace("GetConfigFor");
            Config toReturn;
            if (!theConfs.TryGetValue(win, out toReturn))
            {
                // it is a new window
                toReturn = new Config();
                theConfs.Add(win, toReturn);
                logger.ConditionalTrace("New Config created");

                // is it a new document?
                List<Object> theWindows;
                if (!doc2Win.TryGetValue(doc, out theWindows))
                {
                    // the document is not yet known
                    theWindows = new List<Object>();
                    doc2Win.Add(doc, theWindows);
                }
                theWindows.Add(win);
            }
            return toReturn;
        }

        public static void DocClosed(Object doc)
        {
            List<Object> theWindows;
            if (doc2Win.TryGetValue(doc, out theWindows))
            {
                logger.ConditionalTrace("DocClosed. {0} corresponding window(s) to remove.", theWindows.Count);
                foreach (Object win in theWindows)
                {
                    Config conf;
                    if (theConfs.TryGetValue(win, out conf))
                    {
                        try
                        {
                            IFormatter formatter = new BinaryFormatter();
                            Stream stream = new FileStream(DefaultConfFile, FileMode.Create, FileAccess.Write, FileShare.None);
                            formatter.Serialize(stream, conf);
                            stream.Close();
                        }
                        catch (System.IO.IOException e)
                        {
                            logger.Error("Impossible d'écrire la config par défaut. Erreur {0}", e.Message);
                        }
                    }
                    else
                    {
                        logger.Error("No config is found for closing window.");
                    }
                    theConfs.Remove(win);
                }
                doc2Win.Remove(doc);
            }
            else
            {
                logger.ConditionalTrace("DocClosed. The document was not found.");
                // there was never a Config for this document. This can happen if no colorization took place.
            }
        }

        // *************************************************** Instantiated ****************************************************

        public PBDQConfig pBDQ { get; private set; }
        public Dictionary<PhonConfType, ColConfWin> colors { get; private set; }
        public SylConfig sylConf { get; private set; }
        public UnsetBehConf unsetBeh { get; private set; }

        public Config()
        {
            logger.ConditionalTrace("Config");
            pBDQ = new PBDQConfig();
            sylConf = new SylConfig();
            unsetBeh = new UnsetBehConf();
            colors = new Dictionary<PhonConfType, ColConfWin>(2);
            colors[PhonConfType.muettes] = new ColConfWin(PhonConfType.muettes);
            colors[PhonConfType.phonemes] = new ColConfWin(PhonConfType.phonemes);
        }

    }
}
