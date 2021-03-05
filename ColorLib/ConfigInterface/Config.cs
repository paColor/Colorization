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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// Argument passé lors de l'évènement <c>ConfigRplacedEvent</c>.
    /// </summary>
    public class ConfigReplacedEventArgs : EventArgs
    {
        public Config newConfig { get; private set; } // La nouvelle configuration qui remplace l'ancienne

        public ConfigReplacedEventArgs(Config theNewConf)
        {
            newConfig = theNewConf;
        }
    }

    /// <summary>
    /// Argument passé lors de l'évènement <c>DuoConfReplacedEvent</c>.
    /// </summary>
    public class DuoConfReplacedEventArgs : EventArgs
    {
        public DuoConfig newDuoConfig { get; private set; } // La nouvelle configuration qui remplace l'ancienne

        public DuoConfReplacedEventArgs(DuoConfig theNewDuoConf)
        {
            newDuoConfig = theNewDuoConf;
        }
    }

    /// <summary>
    /// Sert à différentier les <see cref="ColConfWin"/>.
    /// </summary>
    [Serializable]
    public enum PhonConfType { 
        /// <summary>
        /// identifie le <see cref="ColConfWin"/> dédié à la colorisation de phonèmes.
        /// </summary>
        phonemes,

        /// <summary>
        /// identifie le <see cref="ColConfWin"/> dédié à la colorisation des muettes.
        /// </summary>
        muettes
    }

    /// <summary>
    /// Stocke la configuration nécessaire au comportement des outils de fomatage de <see cref="TheText"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// La partie statique de la classe gère l'attribution d'une <c>Config</c> à chaque fenêtre de l'interface
    /// utilisateur. L'idée est que chaque fenêtre dispose de sa propre <c>Config</c> que l'utilisateur peut
    /// modifier à sa guise.
    /// </para>
    /// <para>
    /// Comme il n'est pas possible de recevoir un événement quand une fenêtre est fermée (en tout cas je n'ai
    /// pas trouvé comment faire), les fenêtres sont rattachées à un document (il peut y avoir plusieurs fenêtres
    /// pour le même document). L'évènement de fermeture de document peut appeler <see cref="DocClosed(object)"/>
    /// qui libérera les ressources nécessaires.
    /// </para>
    /// <para>
    /// La méthode de base de la partie statique est <see cref="GetConfigFor(object, object, out string)"/> 
    /// qui retourne une 
    /// <c>Config</c> pour la fenêtre donnée. Un certain nombre d'autres méthodes permettent d'obtenir une 
    /// <c>Config</c> par défaut ou de charger une config sauvegardée.
    /// </para>
    /// <para>La suvegarde et le chargement d'une configuration depuis le disque se fait dans cette
    /// classe. Le nom de la <see cref="Config"/> est défini au moment de la sauvegarde sur le
    /// disque.</para>
    /// <para>Lorsque le dernier document est fermé, la configuration est sauvegardée sur le disque.
    /// Cette sauvegarde est rechargée lorsqu'une premipre configuration est demandée. C'est ce qui
    /// permet à l'utilisateur de se retrouver dans la situation qu'il a laissée lors de sa 
    /// précédente session.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Config : ConfigBase
    {
        // *************************************************** Static **********************************************************

        // --------------------------------------------------- Events ----------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand la la liste des <c>Config</c>(s) sauvegardées est modifiée.
        /// </summary>
        [field: NonSerialized]
        public static event EventHandler ListSavedConfigsModified;


        // -------------------------------------- Private Static Members -------------------------------------------------------

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string ConfigDirName = "Config";
        private static readonly string ConfigDirPath =
            Path.Combine(ConfigBase.colorizationDirPath, ConfigDirName);
        private const string DefaultFileName = "ClrzConfig";
        private const string DefaultAutomaticExtension = ".clrz"; // for automatic saving
        private const string SavedConfigExtension = ".clrzn"; // for user saved configs
        private static readonly string DefaultConfFile = Path.Combine(ConfigDirPath, DefaultFileName) + DefaultAutomaticExtension;
        private const string DefaultConfigName = "Hippocampéléphantocamélos";
        private const string DefaultSubConf1Name = "Castor";
        private const string DefaultSubConf2Name = "Pollux";

        private static Dictionary<Object, Config> theConfs; // key is a window
        private static Dictionary<Object, List<Object>> doc2Win; // key is document, value is list of windows

        // -------------------------------------- Public Static Methods -------------------------------------------------------

        /// <summary>
        /// Initialise la partie statique de la classe (la gestion du  mapping entre documents, fenêtres et configurations).
        /// Appelle les <c>Init()</c> statiques des autres classes de configuration.
        /// </summary>
        /// <remarks> Est responsable de la création du répertoire où seront sauvegardées les configs.</remarks>
        /// <param name="errMsgs">Si une erreur se produit, un message est ajouté à la liste. 
        /// La liste n'est pas touchée si tout se passe bien. <c>null</c> indique que le message
        /// n'est pas souhaité par l'appelant.</param>
        public static new void Init(List<string> errMsgs = null)
        {
            logger.ConditionalDebug("Init");
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
                    errMsgs?.Add("Impossible de créer le répertoire" + ConfigDirPath);
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
            logger.ConditionalDebug("GetSavedConfigNames");
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
        /// <para>
        /// Donne la confiuration qui correspond à la fenêtre <c>win</c>. Mémorise les infos pour
        /// que l'évènement de fermeture du document <c>doc</c> soit traité correctement. Voir
        /// <see cref="DocClosed(object)"/>.
        /// </para>
        /// <para>
        /// Si une configuration a été sauvegardée à la fin de la session de travail précédente,
        /// c'est elle qui est chargée. Dans le cas contraire, ou si le chargement échoue, une
        /// nouvelle <see cref="Config"/> est créée avec des valeurs par défaut.
        /// </para>
        /// </summary>
        /// <param name="win">La fenêtre pour laquelle on veut une <see cref="Config"/>.</param>
        /// <param name="doc">Le document attaché à la fenêtre. </param>
        /// <param name="errMsg">OUT: En cas de problème lors du chargement de la configuration de
        /// la dernière session, ce paramètre contient le message pour l'utilisateur. <c>null</c>
        /// si tout s'est bien passé.</param>
        /// <returns>La <c>Config</c> pour la fenêtre.</returns>
        /// <example>
        /// Il est vivement conseillé de tester la valeur de <c>errMsg</c> et le cas échéant de
        /// présenter le message à l'utilisateur. Par exemple, dans une application
        /// <c>Windows.Forms</c> on peut utiliser <c>MessageBox</c> comme ci-dessous.
        /// <code>
        /// using System.Windows.Forms;
        /// 
        /// string errMsg;
        /// Config conf = Config.GetConfigFor(theWin, theDoc, out errMsg));
        /// if (!string.IsNullOrEmpty(errMsg))
        ///     MessageBox.Show(errMsg, ConfigBase.ColorizationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        /// </code>
        /// </example>
        public static Config GetConfigFor(Object win, Object doc, out string errMsg)
        // returns the Config associated with the Object win, normally the active window. 
        // if there is none, a new one with the defauilt config is created.
        {
            logger.ConditionalDebug("GetConfigFor");
            Config toReturn;
            errMsg = null;
            if (!theConfs.TryGetValue(win, out toReturn))
            {
                // it is a new window
                // Does a default file configuration exist?
                if (File.Exists(DefaultConfFile))
                {
                    string eM;
                    toReturn = LoadConfigFile(DefaultConfFile, out eM);
                    if (toReturn == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Ouuuups! Une erreur s'est produite en chargeant la configuration de votre dernière session. Désolé!");
                        sb.AppendLine("La configuration par défaut est chargée à la place.");
                        sb.Append("Message du système: ");
                        sb.AppendLine(eM);
                        errMsg = sb.ToString();
                        logger.Warn("Error MessageBox displayed.");
                        toReturn = new Config(); // essayons de sauver les meubles.
                        logger.ConditionalDebug("New Config created.");
                    }
                }
                else
                {
                    // create a new config
                    toReturn = new Config();
                    logger.ConditionalDebug("New Config created.");
                }
                UpdateWindowsLists(win, doc, toReturn);
                UndoFactory.Clear();
            }
            return toReturn;
        }

        /// <summary>
        /// Informe la gestion de configurations, que le document <c>doc</c> a été fermé par
        /// l'utilisateur.
        /// </summary>
        /// <remarks>
        /// S'assure que toutes les configurations liées au document soient sauvegardées, et 
        ///  oubliées. Met à jour la gestion de fenêtres et de documents.
        /// </remarks>
        /// <param name="doc">Le document qui se ferme.</param>
        public static void DocClosed(Object doc)
        {
            logger.ConditionalDebug("DocClosed");
            List<Object> theWindows;
            if (doc2Win.TryGetValue(doc, out theWindows))
            {
                logger.ConditionalDebug("DocClosed. {0} corresponding window(s) to remove.", 
                    theWindows.Count);
                foreach (Object win in theWindows)
                {
                    Config conf;
                    if (theConfs.TryGetValue(win, out conf))
                    {
                        string msgTxt;
                        _ = conf.SaveConfigFile(DefaultConfFile, out msgTxt);
                    }
                    else
                    {
                        logger.ConditionalDebug("No config is found for closing window.");
                    }
                    _ = theConfs.Remove(win);
                }
                _ = doc2Win.Remove(doc);
            }
            else
            {
                logger.ConditionalDebug("DocClosed. The document was not found.");
                // there was never a Config for this document. This can happen if no colorization took place.
            }
        }

        /// <summary>
        /// Efface la configuration portant le nom <paramref name="confName"/> du disque.
        /// </summary>
        /// <param name="confName">Le nom de la configuration à effacer des configurations enregistrées.</param>
        /// <param name="msgTxt">Le texte du message d'erreur au cas où la <c>Config</c> ne peut pas être effacée.</param>
        /// <returns><c>true</c> si <c>confName</c> a pu être effacé, <c>false</c> en cas de problème. Dans ce dernier cas,
        /// <paramref name="msgTxt"/> contient un message d'erreur. "" (empty string) sinon.</returns>
        public static bool DeleteSavedConfig(string confName, out string msgTxt)
        {
            bool toReturn = false;
            string fileName = "";
            msgTxt = "";
            try
            {
                fileName = Path.Combine(ConfigDirPath, confName) + SavedConfigExtension;
                File.Delete(fileName);
                toReturn = true;
                logger.ConditionalDebug("Fichier \'{0}\' effacé", fileName);
                OnListSavedConfigsModified(null, EventArgs.Empty);
            }
            catch (Exception e) when (e is IOException || e is SerializationException || e is UnauthorizedAccessException)
            {
                logger.Error(ConfigBase.cultF,
                    "Impossible d'effacer la configuration \'{0}\' dans le fichier \"{1}\". Erreur {2}. StackTrace: {3}", 
                    confName, fileName, e.Message, e.StackTrace);
                msgTxt = e.Message == null ? "" : e.Message; // probabelment jamais null, mais...
                toReturn = false;
            }
            return toReturn;
        }


        // -------------------------------------- Private Static Methods -------------------------------------------------------

        /// <summary>
        /// Charge la <c>Config</c> sauvegardée dans le fichier donné par <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">Le nom du fichier.</param>
        /// <param name="errMsg">Le message d'erreur si le fichier n'a pas pu être chargé, sinon "".</param>
        /// <returns>La <c>Config</c> chargée ou <c>null</c> en cas d'erreur.</returns>
        private static Config LoadConfigFile(string fileName, out string errMsg)
        {
            Config toReturn = null;
            Stream stream = null;
            errMsg = "";
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
                logger.Error("Impossible de lire la config dans le fichier \'{0}\'. Erreur: {1}. StackTrace: {2}",
                   fileName, e.Message, e.StackTrace);
                errMsg = e.Message;
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
            logger.ConditionalDebug("UpdateWindowsLists");
            theConfs.Add(win, theNewConf);
            theNewConf.ConfigReplacedEvent += ConfigReplaced;

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

        private static void ConfigReplaced(object sender, ConfigReplacedEventArgs e)
        {
            // Pour chaque fenêtre à laquelle est associée la Config sender, enlevons l'ancienne
            // Config et insérons la nouvelle.

            // il y a moyen de faire ça avec Linq, mais j'ai la flemme de chercher
            List<object> foundWins = new List<Object>();
            foreach (KeyValuePair<Object, Config> kvp in theConfs)
            {
                if (object.ReferenceEquals(kvp.Value, sender))
                {
                    foundWins.Add(kvp.Key);
                }
            }

            foreach (Object win in foundWins)
            {
                theConfs.Remove(win);
                theConfs.Add(win, e.newConfig);
            }

            // Désinscrivons cette méthode de l'évèenemnt sur l'ancienne Config
            // et inscrivons-la sur la nouvelle.
            Config oldConf = (Config)sender;
            oldConf.ConfigReplacedEvent -= ConfigReplaced;
            e.newConfig.ConfigReplacedEvent += ConfigReplaced;
        }

        private static void OnListSavedConfigsModified(object sender, EventArgs e)
        {
            logger.ConditionalDebug("OnListSavedConfigsModified");
            EventHandler eventHandler = ListSavedConfigsModified;
            eventHandler?.Invoke(sender, e);
        }


        // *********************************************************************************************************************
        // *********************************************************************************************************************
        // *                                                                                                                   *
        // * ------------------------------------------------- INSTANTIATED -------------------------------------------------- *
        // *                                                                                                                   *
        // *********************************************************************************************************************
        // *********************************************************************************************************************


        // --------------------------------- EventHandlers ----------------------------------

        /// <summary>
        /// Evènement déclenché quand la <c>Config</c> est remplacée par une nouvelle <c>Config</c>. Le paramètre
        /// de l'évènement contient la nouvelle <c>Config</c>.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<ConfigReplacedEventArgs> ConfigReplacedEvent;

        /// <summary>
        /// Evènement déclenché quand la <see cref="DuoConfig"/> est remplacée. Les arguments de
        /// de l'évènement contiennent la nouvelle <c>DuoConfig</c>.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<DuoConfReplacedEventArgs> DuoConfReplacedEvent;

        /// <summary>
        /// Evènement déclenché quand le nom de la <c>Config</c> est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler ConfigNameModifiedEvent;

        // ---------------------------------  Members ---------------------------------------

        // L'ordre est imposé par la date de création des membres... C'est une contrainte pour assurer que
        // le "serializing" (la sauvegarde) fonctionne d'une version à l'autre.

        /// <value>
        /// La configuration pour le formatage de lettres.
        /// </value>
        public PBDQConfig pBDQ { get; private set; }

        /// <value>
        /// Contient les configurations pour le formatage des phonèmes pour les différents types de phonèmes
        /// prévus dans <see cref="PhonConfType"/>.
        /// </value>
        /// <example>
        /// Pour effacer toutes les sélections de phonèmes dans la configuration pour le lettres muettes:
        /// <code>
        ///     Config conf = Config.GetDefaultConfigFor(theWin, theDoc);
        ///     conf.colors[PhonConfType.muettes].ClearAllCbxSons();
        /// </code>
        /// </example>
        public Dictionary<PhonConfType, ColConfWin> colors { get; private set; }

        /// <value>
        /// La configuration pour le formatage des syllabes, des mots, ... EN fait pour les commandes qui ont
        /// besoin d'un formatage alterné.
        /// </value>
        public SylConfig sylConf { get; private set; }

        /// <value>
        /// La configuration pour les options avancées, c'est-à-dire le comportement à adopter quand
        /// les flags de <c>CharFormatting</c> comme <c>bold</c> sont sur <c>false</c> (Unset Behaviour).
        /// </value>
        public UnsetBehConf unsetBeh { get; private set; }

        /// <summary>
        /// La configuration pour la commande "duo" ou "2". Attention, est <c>null</c> pour les <c>Config</c>(s)
        /// qui sont des "subConfigs".
        /// </summary>
        public DuoConfig duoConf
        {
            get
            {
                if (isSubConfig)
                {
                    return null;
                }
                else if (_duoConf == null)
                {
                    _duoConf = new DuoConfig();
                }
                return _duoConf;
            }
            set
            {
                if(!isSubConfig)
                {
                    UndoFactory.ExceutingAction(new ConfigAction("Sous config", this, _duoConf,
                        value));
                    _duoConf = value;
                    OnDuoConfReplaced(_duoConf);
                }
                else
                {
                    string msg = "On ne peut pas modifier le \'duoConf\' d'une subConfig.";
                    logger.Error(msg);
                    throw new ArgumentException(msg);
                }
            }
        }

        public ArcConfig arcConf
        {
            get { return _arcConf; }
            private set { _arcConf = value; }
        }

        public PonctConfig ponctConf
        {
            get { return _ponctConf; }
            private set { _ponctConf = value; }
        }

        [OptionalField(VersionAdded = 2)]
        private string configName;

        [OptionalField(VersionAdded = 3)]
        private DuoConfig _duoConf;

        [OptionalField(VersionAdded = 5)]
        private ArcConfig _arcConf;

        [OptionalField(VersionAdded = 6)]
        private PonctConfig _ponctConf;


        /// <summary>
        /// indique s'il s'agit d'une 'subConfig? c-à-d attachée à une <c>DuoConfig</c>.
        /// </summary>
        [NonSerialized]
        private bool isSubConfig;

        /// <summary>
        /// s'il s'agit d'une 'subConfig' (voir le flag précédent) le numéro de la subConfig.
        /// </summary>
        [NonSerialized]
        private int subConfNr;

        // ---------------------------------------------- Methods ----------------------------------

        /// <summary>
        /// Crée une config par défaut pour les différents membres.
        /// </summary>
        private void InitCtor()
        {
            logger.ConditionalDebug("InitCtor");
            pBDQ = new PBDQConfig();
            sylConf = new SylConfig();
            unsetBeh = new UnsetBehConf();
            colors = new Dictionary<PhonConfType, ColConfWin>(2);
            colors[PhonConfType.muettes] = new ColConfWin(PhonConfType.muettes);
            colors[PhonConfType.phonemes] = new ColConfWin(PhonConfType.phonemes);
            _duoConf = null;
            _arcConf = new ArcConfig();
            _ponctConf = new PonctConfig();
        }

        /// <summary>
        /// Crée une <c>Config</c> par défaut.
        /// </summary>
        public Config()
        {
            logger.ConditionalDebug("Config()");
            UndoFactory.DisableUndoRegistration();
            isSubConfig = false;
            subConfNr = 0;
            InitCtor();
            SetConfigName(DefaultConfigName);
            UndoFactory.EnableUndoRegistration();
        }

        /// <summary>
        /// Créée une "subconfig" pour la <c>Config</c> <paramref name="mother"/>.
        /// </summary>
        /// <param name="mother">La <c>Config</c> pour laquelle une "subconfig" doit être créée.</param>
        /// <param name="daughterNr">Le numéro de la config. Valeurs possibles, 1 ou 2. </param>
        public Config(int daughterNr)
        {
            logger.ConditionalDebug("Config(Config), daughterNr: {0}", daughterNr);
            UndoFactory.DisableUndoRegistration();
            isSubConfig = true;
            subConfNr = daughterNr;
            InitCtor();
            ResetSubConfig(subConfNr);
            UndoFactory.EnableUndoRegistration();
        }

        /// <summary>
        /// Dit à la <c>Config</c> qu'elle est une subConfig de type donné par <paramref name="theNr"/>.
        /// </summary>
        /// <param name="theNr">Le type de subConfig</param>
        public void IsSubConfig(int theNr)
        {
            isSubConfig = true;
            subConfNr = theNr;
            _duoConf = null; // si par hasard une DuoConf était attachée, on la jette.
        }

        public override void Reset()
        {
            logger.ConditionalDebug("Reset");
            UndoFactory.StartRecording("Réinitialiser config");
            pBDQ.Reset();
            foreach (ColConfWin ccf in colors.Values)
            {
                ccf.Reset();
            }
            sylConf.Reset();
            unsetBeh.Reset();
            arcConf.Reset();
            ponctConf.Reset();
            if (isSubConfig)
            {
                ResetSubConfig(subConfNr);
            }
            else
            {
                _duoConf?.Reset(); // on ne fait le reset que si la duoConf existe
                SetConfigName(DefaultConfigName);
            }
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Donne le nom de la configuration tel qu'enregistré dans le fichier de suavegarde.
        /// </summary>
        /// <returns>Le nom de la config.</returns>
        public string GetConfigName() => configName;

        /// <summary>
        /// Charge la <c>Config</c> sauvgardée identifiée par <paramref name="name"/>. Si ça marche,
        /// un évènement <see cref="ConfigReplacedEvent"/> est déclenché avec la référence de la nouvelle
        /// config et <c>true</c> est retourné. Si ça échoue, <c>false</c> est retourné et 
        /// <paramref name="errMsg"/> contient le message d'erreur.
        /// </summary>
        /// <remarks>
        /// Les évènements <see cref="ConfigNameModifiedEvent"/> et <see cref="DuoConfReplacedEvent"/> ne 
        /// sont pas déclenchés! L'idée est que <see cref="ConfigReplacedEvent"/> suffit.
        /// </remarks>
        /// <example>
        /// Deux choses sont un peu surprenantes: 1 - Devoir disposer d'une <see cref="Config"/>
        /// pour en charger une autre. Cela provient de l'interface utilisateur actuelle de 
        /// Colorization. 2 - Devoir aller chercher la nouvelle <see cref="Config"/>
        /// dans les paramètres de l'évènement...
        /// <code>
        /// Config conf = new Config();
        /// conf.LoadConfig("SuperbeConf", out errMsg);
        /// 
        /// // L'évènement 'ConfigReplacedEvent' a lieu ici.
        /// 
        /// private void HandleConfigReplaced(object sender, ConfigReplacedEventArgs e)
        /// {
        ///    conf = e.newConfig;
        ///    AssignHandlersTo(conf);
        /// }
        /// 
        /// </code>
        /// </example>
        /// <param name="name">Le nom de la <c>Config</c> sauvegardée à charger.</param>
        /// <param name="errMsg">Le message d'erreur si une erreur s'est produite, sinon "".</param>
        /// <returns><c>true</c> si la <c>Config</c> a pu être chargée, sinon <c>false</c>.</returns>
        public bool LoadConfig(string name, out string errMsg)
        {
            logger.ConditionalDebug("LoadConfig \'{0}\'", name);
            Config theConf = LoadConfigFile(Path.Combine(ConfigDirPath, name) + SavedConfigExtension, out errMsg);
            bool toReturn = (theConf != null); 
            if (toReturn)
            {
                if (isSubConfig)
                {
                    theConf.IsSubConfig(subConfNr);
                }
                OnConfigReplaced(theConf);
            }
            return toReturn;
        }

        /// <summary>
        /// Sauve la configuration sous le nom indiqué.
        /// </summary>
        /// <param name="name">Le nom sous lequel la configuration doit être enregistrée.</param>
        /// <param name="msgTxt">Retourne un message d'erreur dans le cas où la <c>Config</c> n'a pas pu être 
        /// sauvegardée.</param>
        /// <returns><c>true</c> si la sauvegarde a pu avoir lieu, sinon <c>false</c>. </returns>
        public bool SaveConfig(string name, out string msgTxt)
        {
            logger.ConditionalDebug("SaveConfig \'{0}\'", name);
            SetConfigName(name);
            bool toReturn = SaveConfigFile(Path.Combine(ConfigDirPath, name) + SavedConfigExtension, out msgTxt);
            if (toReturn)
            {
                OnListSavedConfigsModified(this, EventArgs.Empty);
            }
            return toReturn;
        }

        /// <summary>
        /// Execute une "deep copy" de l'objet, c'est à dire que tous les éléments attachés à <c>this</c> sont copiés
        /// dans une nouvelle instance
        /// </summary>
        /// <returns>Une copie exacte de <c>this</c> qui n'a rien en commun avec ce dernier. Retourne <c>null</c> 
        /// en cas d'erreur.</returns>
        public Config DeepCopy()
        {
            logger.ConditionalDebug("DeepCopy");
            Config toReturn = null;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    toReturn = (Config)formatter.Deserialize(stream);
                    toReturn.PostLoadInitOptionalFields();
                }
            }
            catch (Exception e) when (e is IOException || e is SerializationException)
            {
                logger.Error("Impossible d'effectuer une copie de la \'Config\'. Erreur: {0}, Stack: {2}",
                    e.Message, e.StackTrace);
            }
            return toReturn;
        }

        private bool SaveConfigFile(string fileName, out string msgTxt)
        {
            bool toReturn = false;
            msgTxt = "";
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    toReturn = true;
                    logger.Info("Fichier de config \'{0}\' enregistré", fileName);
                }
            }
            catch (Exception e) when (e is IOException || e is SerializationException || e is UnauthorizedAccessException)
            {
                logger.Error("Impossible d'écrire le fichier de config \'{0}\'. Erreur: {1}. StackTrace {2}",
                    fileName, e.Message, e.StackTrace);
                msgTxt = e.Message;
                toReturn = false;
            }
            return toReturn;
        }

        /// <summary>
        /// Effectue les configurations spécifiques à une subConfig de no <paramref name="theSubConfigNr"/>. 
        /// Précondition: les deux premiers boutons du <c>sylConf</c> sont configurés. 
        /// </summary>
        /// <param name="theSubConfigNr">Le numéro de subConfig qui définit les paramètres par défaut.</param>
        private void ResetSubConfig(int theSubConfigNr)
        {
            if (theSubConfigNr == 1)
            {
                SetConfigName(DefaultSubConf1Name);
                sylConf.SetSylButtonCF(0, ColConfWin.coloredCF[(int)PredefCol.pureBlue]);
                sylConf.SetSylButtonCF(1, ColConfWin.coloredCF[(int)PredefCol.lightBlue]);
                arcConf.SetArcButtonCol(0, ColConfWin.predefinedColors[(int)PredefCol.darkBlue]);
            }
            else if (theSubConfigNr == 2)
            {
                SetConfigName(DefaultSubConf2Name);
                sylConf.SetSylButtonCF(0, ColConfWin.coloredCF[(int)PredefCol.darkRed]);
                sylConf.SetSylButtonCF(1, ColConfWin.coloredCF[(int)PredefCol.pink]);
                arcConf.SetArcButtonCol(0, ColConfWin.predefinedColors[(int)PredefCol.darkRed]);
            }
            else
            {
                logger.Error("Seules des subCOnfigs 1 et 2 peuvent être initialisés.");
                Debug.Assert(false);
            }
        }

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            SetConfigName(DefaultConfigName);
            _duoConf = null;
            isSubConfig = false;
            subConfNr = 0;
            _arcConf = new ArcConfig();
            _ponctConf = new PonctConfig();
        }

        /// <summary>
        /// Définit le <c>configName</c>.
        /// </summary>
        /// <remarks>Normalement, on devrait avoir un setter et un getter pour <c>configName</c>,
        /// mais je n'avais pas bien compris comment ça marche avec la sérialisation et 
        /// je ne veux plus toucher à ça de peur de devenir incompatible avec d'anciens fichiers de 
        /// sauvegarde. </remarks>
        /// <remarks>N'est <c>public</c> que pour <see cref="ConfigAction"/></remarks>
        /// <param name="theName">Le nouveau nom.</param>
        public void SetConfigName (string theName)
        {
            UndoFactory.ExceutingAction(new ConfigAction("Nom de config", this, configName, theName));
            configName = theName;
            OnConfigNameModified();
        }

        internal override void PostLoadInitOptionalFields()
        {
            logger.ConditionalDebug("PostLoadInitOptionalFields");
            pBDQ.PostLoadInitOptionalFields();
            foreach (ColConfWin ccf in colors.Values)
            {
                ccf.PostLoadInitOptionalFields();
            }
            sylConf.PostLoadInitOptionalFields();
            unsetBeh.PostLoadInitOptionalFields();
            _duoConf?.PostLoadInitOptionalFields();
            _arcConf.PostLoadInitOptionalFields();
            ponctConf.PostLoadInitOptionalFields();
        }

        // ------------------------------------------------- Events --------------------------------------------

        protected virtual void OnConfigReplaced (Config newConfig)
        {
            logger.ConditionalDebug("OnConfigReplaced");
            EventHandler<ConfigReplacedEventArgs> eventHandler = ConfigReplacedEvent;
            // il est conseillé de faire ceci pour le cas tordu où le dernier "handler" se désabonnerait entre
            // le test sur null et l'évèenement. Je ne suis pas sûr que ça puisse jamais arriver ici. 
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.

            eventHandler?.Invoke(this, new ConfigReplacedEventArgs(newConfig));
        }

        protected virtual void OnDuoConfReplaced(DuoConfig newDuoConf)
        {
            logger.ConditionalDebug("OnDuoConfReplaced");
            EventHandler<DuoConfReplacedEventArgs> eventHandler = DuoConfReplacedEvent;
            eventHandler?.Invoke(this, new DuoConfReplacedEventArgs(newDuoConf));
        }

        protected virtual void OnConfigNameModified()
        {
            logger.ConditionalDebug("OnConfigNameModified");
            EventHandler eventHandler = ConfigNameModifiedEvent;
            eventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
