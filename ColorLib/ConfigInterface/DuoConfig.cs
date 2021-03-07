using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ColorLib
{
    /// <summary>
    /// Données de configuration pour la fonction "Duo" ou "2".
    /// </summary>
    [Serializable]
    public class DuoConfig : ConfigBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ------------------------------------ Event Handlers --------------------------------------

        /// <summary>
        /// Evènement déclenché quand <c>alteranance</c> est modifié.
        /// </summary>
        [field:NonSerialized]
        public event EventHandler AlternanceModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand <c>colorisFunction</c> est modifié.
        /// </summary>
        [field:NonSerialized]
        public event EventHandler ColorisFunctionModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand <c>nbreAlt</c> est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler NbreAltModifiedEvent;

        // --------------------------------------  members ----------------------------------

        /// <summary>
        /// La <c>Congig</c> no 1 pour la commande "duo"
        /// </summary>
        public Config subConfig1
        {
            get
            {
                return _subConfig1;
            }
            private set
            {
                logger.ConditionalTrace("Set subConfig1");
                _subConfig1 = value;
            }
        }

        /// <summary>
        /// La <c>Congig</c> no 2 pour la commande "duo"
        /// </summary>
        public Config subConfig2
        {
            get
            {
                return _subConfig2;
            }
            private set
            {
                logger.ConditionalTrace("Set subConfig2");
                _subConfig2 = value;
            }
        }

        /// <summary>
        /// Type utilisé pour définir quelle aternance est désirée dans la commande "duo"
        /// </summary>
        public enum Alternance { mots, lignes, undefined }

        /// <summary>
        /// indique le mode d'alternance sélectionné pour la commande "duo" ou "2"
        /// </summary>
        public Alternance alternance {
            get 
            {
                return _alternance;
            }
            set 
            {
                if (value != _alternance)
                {
                    logger.ConditionalTrace("Set alternance to {0}", value);
                    UndoFactory.ExceutingAction(new DuoAction("Duo alternance", this, alternance, value));
                    _alternance = value;
                    OnAlternanceModified(EventArgs.Empty);
                }
            } 
        }

        /// <summary>
        /// Type utilisé pour définir quelle fonction doit être appliquée par la commande "duo"
        /// </summary>
        public enum ColorisFunction { syllabes, mots, lettres, voyCons, phonemes, muettes, undefined, arcs }

        /// <summary>
        /// Inidique quelle fonction doit être exécutée en alternance par la commande "duo" ou "2"
        /// </summary>
        public ColorisFunction colorisFunction
        {
            get
            {
                return _colorisFunction;
            }
            set
            {
                logger.ConditionalDebug("Set colorisFunction to {0}", value);
                if (value != _colorisFunction)
                {
                    UndoFactory.ExceutingAction(new DuoAction("Duo colorisFunction", this, colorisFunction, value));
                    _colorisFunction = value;
                    OnColorisFunctionModified(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Nombre de mots ou de lignes à regrouper dans le traitement alterné.
        /// </summary>
        public int nbreAlt
        {
            get
            {
                return _nbreAlt;
            }
            set
            {
                if (_nbreAlt != value)
                {
                    UndoFactory.ExceutingAction(new DuoAction("Duo nombre alt.", this, nbreAlt, value));
                    _nbreAlt = value;
                    OnNbreAltModifed(EventArgs.Empty);
                }
            }
        }

        private Config _subConfig1;
        private Config _subConfig2;
        private Alternance _alternance = Alternance.mots; // pour mettre une base aux séries d'annulation
        private ColorisFunction _colorisFunction = ColorisFunction.syllabes; // pour mettre une base aux séries d'annulation

        [OptionalField(VersionAdded = 4)]
        private int _nbreAlt = 1; // pour mettre une base aux séries d'annulation

        // ------------------------------------------ Methods -------------------------------------------------

        /// <summary>
        /// Construit un DuoConfig avec une configuration par défaut.
        /// </summary>
        public DuoConfig()
        {
            logger.ConditionalDebug("DuoConfig");
            UndoFactory.DisableUndoRegistration();
            subConfig1 = new Config(1);
            subConfig1.ConfigReplacedEvent += SubConfig1Replaced;

            subConfig2 = new Config(2);
            subConfig2.ConfigReplacedEvent += SubConfig2Replaced;

            InitFields();
            UndoFactory.EnableUndoRegistration();
        }

        public override void Reset()
        {
            logger.ConditionalDebug("DuoConfig");
            UndoFactory.StartRecording("Réinitialiser duo");
            subConfig1.Reset();
            subConfig2.Reset();
            InitFields();
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Execute une "deep copy" de l'objet, c'est à dire que tous les éléments attachés à <c>this</c> sont copiés
        /// dans une nouvelle instance
        /// </summary>
        /// <returns>Une copie exacte de <c>this</c> qui n'a rien en commun avec ce dernier. Retourne <c>null</c> 
        /// en cas d'erreur.</returns>
        public DuoConfig DeepCopy()
        {
            logger.ConditionalDebug("DeepCopy");
            DuoConfig toReturn = null;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    toReturn = (DuoConfig)formatter.Deserialize(stream);
                    toReturn.PostLoadInitOptionalFields();
                }
            }
            catch (Exception e) when (e is IOException || e is SerializationException)
            {
                logger.Error("Impossible d'effectuer une copie de la \'DuoConfig\'. Erreur: {0}, Stack: {2}",
                    e.Message, e.StackTrace);
            }
            return toReturn;
        }

        // --------------------------------------- Serialization ----------------------------------

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            nbreAlt = 1;
        }


        internal override void PostLoadInitOptionalFields()
        {
            logger.ConditionalDebug("PostLoadInitOptionalFields");

            subConfig1.IsSubConfig(1);
            subConfig1.PostLoadInitOptionalFields();
            subConfig1.ConfigReplacedEvent += SubConfig1Replaced;

            subConfig2.IsSubConfig(2);
            subConfig2.PostLoadInitOptionalFields();
            subConfig2.ConfigReplacedEvent += SubConfig2Replaced;

            if (nbreAlt < 1)
                nbreAlt = 1;
        }
        
        /// <summary>
        /// Initialise les champs (membres) <c>alternance</c> et <c>colorisFunction</c> à leur valeur
        /// par défaut.
        /// </summary>
        /// <remarks>Est placé dans une méthode séparée pour qu'il n'y ait qu'un endroit où ces valeurs
        /// par défaut sont définies.</remarks>
        private void InitFields()
        {
            logger.ConditionalDebug("InitFields");
            UndoFactory.StartRecording("Réinitialise les champs duo.");
            alternance = Alternance.mots;
            colorisFunction = ColorisFunction.syllabes;
            nbreAlt = 1;
            UndoFactory.EndRecording();
        }


        // ---------------------------------------- Event Handlers ------------------------------------

        private void SubConfig1Replaced (object sender, ConfigReplacedEventArgs e)
        {
            logger.ConditionalDebug("SubConfig1Replaced");
            subConfig1.ConfigReplacedEvent -= SubConfig1Replaced;
            subConfig1 = e.newConfig;
            subConfig1.IsSubConfig(1);
            subConfig1.ConfigReplacedEvent += SubConfig1Replaced;
        }

        private void SubConfig2Replaced(object sender, ConfigReplacedEventArgs e)
        {
            logger.ConditionalDebug("SubConfig2Replaced");
            subConfig2.ConfigReplacedEvent -= SubConfig2Replaced;
            subConfig2 = e.newConfig;
            subConfig2.IsSubConfig(2);
            subConfig2.ConfigReplacedEvent += SubConfig2Replaced;
        }

        // ----------------------------------------- OnEvents ---------------------------------------

        protected virtual void OnAlternanceModified(EventArgs e)
        {
            logger.ConditionalDebug("OnAlternanceModified");
            EventHandler eventHandler = AlternanceModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnColorisFunctionModified(EventArgs e)
        {
            logger.ConditionalDebug("OnColorisFunctionModified");
            EventHandler eventHandler = ColorisFunctionModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnNbreAltModifed(EventArgs e)
        {
            logger.ConditionalDebug("OnNbreAltModifed");
            EventHandler eventHandler = NbreAltModifiedEvent;
            eventHandler?.Invoke(this, e);
        }


    }
}
