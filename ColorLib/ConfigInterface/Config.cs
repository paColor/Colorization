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
using System.Diagnostics;
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
    public class Config : ConfigBase
    {
        // *************************************************** Static **********************************************************

        // -------------------------------------- Private Static Members -------------------------------------------------------

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string ConfigDirName = "Config";
        private static readonly string ConfigDirPath =
            Path.Combine(BaseConfig.colorizationDirPath, ConfigDirName);
        private const string DefaultFileName = "ClrzConfig";
        private const string DefaultAutomaticExtension = ".clrz"; // for automatic saving
        private const string SavedConfigExtension = ".clrzn"; // for user saved configs
        private static readonly string DefaultConfFile = Path.Combine(ConfigDirPath, DefaultFileName) + DefaultAutomaticExtension;

        private static Dictionary<Object, Config> theConfs; // key is a window
        private static Dictionary<Object, List<Object>> doc2Win; // key is document, value is list of windows

        // -------------------------------------- Public Static Methods -------------------------------------------------------

        /// <summary>
        /// Initialise la partie statique de la classe (la gestion du  mapping entre documents, fenêtres et configurations).
        /// </summary>
        /// <remarks> Est responsable de la création du répertoire où seront sauvegardées les configs.</remarks>
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
                    logger.Info("Dossier \'{0}\' créé.", ConfigDirPath);
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
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
                var configFiles = Directory.EnumerateFiles(ConfigDirPath, "*" + SavedConfigExtension);
                foreach (string fileName in configFiles)
                {
                    toReturn.Add(Path.GetFileNameWithoutExtension(fileName));
                }
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                logger.Error("Impossible de charger la liste de fichiers de sauvegarde du répertoire \'{0}\'. Message: {1}",
                    ConfigDirPath, e.Message);
            }
            return toReturn;
        }

        /// <summary>
        /// Charge la configuration portant le nom <c>name</c>.
        /// </summary>
        /// <param name="name">Le nom de la configuration à charger.</param>
        /// <returns>La configuration chargée ou null si le chargement échoue.</returns>
        public static Config LoadConfig(string name, Object win, Object doc)
        {
            logger.ConditionalTrace("LoadConfig \'{0}\'", name);
            Config toReturn = null;
            toReturn = LoadConfigFile(Path.Combine(ConfigDirPath, name) + SavedConfigExtension);
            if (toReturn != null)
            {
                if (theConfs.ContainsKey(win))
                {
                    theConfs.Remove(win);
                }
                else
                {
                    // le cas où win ne serait pas connu ne devrait pas arriver.
                    logger.Info("On charge une config dans une fenêtre qui n'en avait pas jusqu'à présent.");
                }
                UpdateWindowsLists(win, doc, toReturn);
            }
            return toReturn;
        }

        /// <summary>
        /// Donne la confiuration qui correspond à la fenêtre <c>win</c>. Mémorise les infos pour
        /// que l'évènement de fermeture du document <c>doc</c> soit traité correctement
        /// </summary>
        /// <param name="win">La fenêtre pour la quelle on veut un objet <c>Config</c>.</param>
        /// <param name="doc">Le document attaché à la fenêtre. </param>
        /// <returns>La <c>Config</c> pour la fenêtre.</returns>
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
                    toReturn = LoadConfigFile(DefaultConfFile);
                    if (toReturn == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Ouuuups! Une erreur s'est produite en chargeant votre dernière configuration. Désolé!");
                        sb.AppendLine("La configuration par défaut est chargée à la place.");
                        MessageBox.Show(sb.ToString(), BaseConfig.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        logger.Info("Error MessageBox displayed.");
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
                UpdateWindowsLists(win, doc, toReturn);
            }
            return toReturn;
        }

        /// <summary>
        /// Informe la gestion de configurations, que le document <c>doc</c> a été fermé par l'utilisateur.
        /// </summary
        /// <remarks>
        /// S'assure que tous les configs, liées au document soient sauvegardées, et oubliées et met à jour la 
        /// gestion de fenêtres et de documents.
        /// </remarks>
        /// <param name="doc">Le document qui se ferme.</param>
        public static void DocClosed(Object doc)
        {
            logger.ConditionalTrace("DocClosed");
            List<Object> theWindows;
            if (doc2Win.TryGetValue(doc, out theWindows))
            {
                logger.ConditionalTrace("DocClosed. {0} corresponding window(s) to remove.", theWindows.Count);
                foreach (Object win in theWindows)
                {
                    Config conf;
                    if (theConfs.TryGetValue(win, out conf))
                    {
                        _ = conf.SaveConfigFile(DefaultConfFile);
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

        /// <summary>
        /// Efface la configuration portant le nom <c>confName</c>.
        /// </summary>
        /// <param name="confName">Le nom de la configuration à effacer des configurations enregistrées.</param>
        public static bool DeleteSavedConfig(string confName)
        {
            bool toReturn = false;
            string fileName = "";
            try
            {
                fileName = Path.Combine(ConfigDirPath, confName) + SavedConfigExtension;
                File.Delete(fileName);
                toReturn = true;
                logger.ConditionalTrace("Fichier \'{0}\' effacé", fileName);
            }
            catch (Exception e) when (e is IOException || e is SerializationException || e is UnauthorizedAccessException)
            {
                logger.Error("Impossible d'effacer la configuration \'{0}\' dans le fichier \"{1}\". Erreur {2}",
                   confName, fileName, e.Message);
                toReturn = false;
            }
            return toReturn;
        }


        // -------------------------------------- Public Static Methods -------------------------------------------------------

        private static Config LoadConfigFile(string fileName)
        {
            Config toReturn = null;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                toReturn = (Config)formatter.Deserialize(stream);
                stream.Close();
                toReturn.PostLoadInitOptionalFields();
                logger.Info("Config File \'{0}\' loaded.", fileName);
            }
            catch (Exception e) // when (e is IOException || e is SerializationException || e is UnauthorizedAccessException)
            {
                logger.Error("Impossible de lire la config dans le fichier \'{0}\'. Erreur:  {1}. StackTrace: {2}",
                   fileName, e.Message, e.StackTrace);
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Met à jour les listes statiques rattachées au doc et à win pour mémoriser que <c>theNewConf</c>
        /// leur est rattaché.
        /// </summary>
        /// <param name="win">la fenêtre à laquelle <c>theNewConf</c> doir être rattaché.</param>
        /// <param name="doc">le document auquel est rattaché la fenêtre.</param>
        /// <param name="theNewConf">La configuration qui doit être mémorisée.</param>
        private static void UpdateWindowsLists(Object win, Object doc, Config theNewConf)
        {
            theConfs.Add(win, theNewConf);

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
        public bool SaveConfig(string name)
        {
            logger.ConditionalTrace("SaveConfig \'{0}\'", name);
            bool toReturn = SaveConfigFile(Path.Combine(ConfigDirPath, name) + SavedConfigExtension);
            configName = name;
            updateListeConfigs();
            return toReturn;
        }

        private bool SaveConfigFile(string fileName)
        {
            bool toReturn = false;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
                toReturn = true;
                logger.Info("Config \'{0}\' enregistrée", fileName);
            }
            catch (Exception e) when (e is IOException || e is SerializationException || e is UnauthorizedAccessException)
            {
                logger.Error("Impossible d'écrire le fichier de config \'{0}\'. Erreur: {1}",
                    fileName, e.Message);
                if (stream != null)
                {
                    stream.Dispose();
                }
                toReturn = false;
            }
            return toReturn;
        }

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalTrace("SetOptionalFieldsToDefaultVal");
            configName = "";
        }

        internal override void PostLoadInitOptionalFields()
        {
            logger.ConditionalTrace("PostLoadInitOptionalFields");
            pBDQ.PostLoadInitOptionalFields();
            foreach (ColConfWin ccf in colors.Values)
            {
                ccf.PostLoadInitOptionalFields();
            }
            sylConf.PostLoadInitOptionalFields();
            unsetBeh.PostLoadInitOptionalFields();
        }

    }
}
