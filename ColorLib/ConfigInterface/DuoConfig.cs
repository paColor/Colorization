using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ColorLib
{
    /// <summary>
    /// Données de configuration pour la fonction "Duo" ou "2".
    /// </summary>
    [Serializable]
    public class DuoConfig : ConfigBase
    {
        /// <summary>
        /// Evènement déclenché quand <c>alteranance</c> est modifié.
        /// </summary>
        public event EventHandler AlternanceChanged;

        /// <summary>
        /// Evènement déclenché quand <c>colorisFunction</c> est modifié.
        /// </summary>
        public event EventHandler ColorisFunctionChanged;

        /// <summary>
        /// La <c>Congig</c> no 1 pour la commande "2"
        /// </summary>
        public Config subConfig1 { get; private set; }

        /// <summary>
        /// La <c>Congig</c> no 2 pour la commande "2"
        /// </summary>
        public Config subConfig2 { get; private set; }

        public enum Alternance { mots, lignes, undefined }

        /// <summary>
        /// indique le mode d'alternance sélectionné pour la commande "duo" ou "2"
        /// </summary>
        public Alternance alternance {
            get 
            {
                return alternance;
            }
            set 
            {
                alternance = value;
                OnAlternanceChanged(EventArgs.Empty);
            } 
        }

        public enum ColorisFunction { syllabes, mots, lettres, voyCons, phonemes, muettes, undefined }

        /// <summary>
        /// Inidique quelle fonction doit être exécutée en alternance par la commande "duo" ou "2"
        /// </summary>
        public ColorisFunction colorisFunction
        {
            get
            {
                return colorisFunction;
            }
            set
            {
                colorisFunction = value;
                OnColorisFunctionChanged(EventArgs.Empty);
            }
        }

        public DuoConfig()
        {
            subConfig1 = new Config(1);
            subConfig2 = new Config(2);
            alternance = Alternance.mots;
            colorisFunction = ColorisFunction.syllabes;
        }

        protected virtual void OnAlternanceChanged(EventArgs e)
        {
            AlternanceChanged?.Invoke(this, e);
        }

        protected virtual void OnColorisFunctionChanged(EventArgs e)
        {
            ColorisFunctionChanged?.Invoke(this, e);
        }
    }
}
