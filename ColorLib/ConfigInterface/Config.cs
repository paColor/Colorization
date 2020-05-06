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
using System.Runtime.CompilerServices;
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
        private const string DefaultAutomaticExtension = ".clrz"; // for automatic saving
        private const string SavedConfiExtension = ".clrzn"; // for user saved configs
        private static readonly string DefaultConfFileWOExt = Path.Combine(ConfigDirPath, DefaultFileName);
        private static readonly string DefaultConfFile = DefaultConfFileWOExt + DefaultAutomaticExtension;

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

        /// <summary>
        /// Retourne la liste des configurations enregistrées.
        /// </summary>
        /// <remarks>Si un nom de la liste est utilisé pour sauvegarder une config, l'ancienne sera écrasée.</remarks>
        /// <returns>Liste des noms de configuration.</returns>
        public static List<string> GetSavedConfigNames()
        {
            List<string> toReturn = new List<string>();
            try
            {
                var configFiles = Directory.EnumerateFiles(ConfigDirPath, "*" + SavedConfiExtension);
                foreach (string fileName in configFiles)
                {
                    toReturn.Add(fileName.Substring(0, fileName.Length - SavedConfiExtension.Length));
                }
            }
            catch (System.IO.IOException e)
            {
                logger.Error("Impossible de charger la liste de fichiers de sauvegarde du répertoire {0}. Message: {1}",
                    ConfigDirPath, e.Message);
            }
            return toReturn;
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
                // Does a default file configuration exist?
                if (File.Exists(DefaultConfFile))
                {
                    try
                    {
                        // Load it
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(DefaultConfFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                        toReturn = (Config)formatter.Deserialize(stream);
                        stream.Close();
                        logger.Info("Default Config File loaded.");
                    }
                    catch (Exception e) when (e is IOException || e is SerializationException)
                    {
                        logger.Error("Impossible de lire la config par défaut dans le fichier {0}. Erreur {1}", 
                            DefaultConfFile, e.Message);
                        toReturn = new Config(); // essayons de sauver les meubles.
                        logger.ConditionalTrace("New Config created.");
                    }
                }
                else
                {
                    // create a new config
                    toReturn = new Config();
                    logger.ConditionalTrace("New Config created.");
                }
                theConfs.Add(win, toReturn);

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
                        _ = conf.SaveConfig(DefaultConfFileWOExt, DefaultAutomaticExtension);
                    }
                    else
                    {
                        logger.ConditionalTrace("No config is found for closing window.");
                    }
                    _ = theConfs.Remove(win);
                }
                _ = doc2Win.Remove(doc);
            }
            else
            {
                logger.ConditionalTrace("DocClosed. The document was not found.");
                // there was never a Config for this document. This can happen if no colorization took place.
            }
        }

        // *************************************************** Instantiated ****************************************************

        [NonSerialized]
        public ExecuteTask updateConfigName;
        [NonSerialized]
        public ExecuteTask updateListeConfigs;

        public PBDQConfig pBDQ { get; private set; }
        public Dictionary<PhonConfType, ColConfWin> colors { get; private set; }
        public SylConfig sylConf { get; private set; }
        public UnsetBehConf unsetBeh { get; private set; }

        [OptionalField(VersionAdded = 2)]
        private string configName;

        public Config()
        {
            logger.ConditionalTrace("Config");
            pBDQ = new PBDQConfig();
            sylConf = new SylConfig();
            unsetBeh = new UnsetBehConf();
            colors = new Dictionary<PhonConfType, ColConfWin>(2);
            colors[PhonConfType.muettes] = new ColConfWin(PhonConfType.muettes);
            colors[PhonConfType.phonemes] = new ColConfWin(PhonConfType.phonemes);
            configName = "";
        }

        /// <summary>
        /// Donne le nom de la configuration tel qu'enregistré dans le fichier de suavegarde.
        /// </summary>
        /// <returns>Le nom de la config.</returns>
        public string GetConfigName() => configName;

        

        /// <summary>
        /// Sauve la configuration sous le nom indiqué.
        /// </summary>
        /// <param name="name">Le nom sous lequel la configuration doit être enregistrée.</param>
        /// <returns>true si la sauvegarde a pu avoir lieu, sinon false. </returns>
        public bool SaveConfig(string name) => SaveConfig(Path.Combine(ConfigDirPath, name), SavedConfiExtension);

        private bool SaveConfig (string fileName, string extension)
        {
            bool toReturn = false;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName + extension, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
                toReturn = true;
                logger.Info("Config {0} enregistrée", fileName + extension);
            }
            catch (Exception e) when (e is IOException || e is SerializationException)
            {
                logger.Error("Impossible d'écrire la config par défaut dans le fichier {0}. Erreur {1}",
                    DefaultConfFile, e.Message);
                toReturn = false;
            }
            return toReturn;
        }

        [OnDeserializing]
        private void SetOptionalFieldsDefault(StreamingContext sc)
        {
            configName = "";
        }

    }
}
