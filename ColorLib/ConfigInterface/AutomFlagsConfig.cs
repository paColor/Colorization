using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    [Serializable]
    public class AutomFlagsConfig
    {
        /// <summary>
        /// Contient les identifiants pour les flags de contrôle des règles de l'automate
        /// </summary>
        /// <remarks>
        /// IllCeras: les "ill" et "il" sont traités comme un son.
        /// IllLireCouleur: les "ill" et "il" sont traités en fonction des phonèmes effectivement présents
        /// dans les mots. fille par exemple donne fij°. l'un des deux flags doit être mis.
        /// </remarks>
        public enum RuleFlag { dummy, IllCeras,IllLireCouleur, last }
        
        /// <summary>
        /// mode à utiliser pour les "ill"
        /// </summary>
        public enum IllRule { ceras, lirecouleur }

        /// <summary>
        /// Permet de déterminer le mode à utiliser pour les "ill"
        /// </summary>
        public IllRule IllRuleToUse {
            get
            {
                if (flags[(int)RuleFlag.IllCeras])
                    return IllRule.ceras;
                else
                    return IllRule.lirecouleur;
            }

            set
            {
                if (value == IllRule.ceras)
                {
                    flags[(int)RuleFlag.IllCeras] = true;
                    flags[(int)RuleFlag.IllLireCouleur] = false;
                }
                else if (value == IllRule.lirecouleur)
                {
                    flags[(int)RuleFlag.IllCeras] = false;
                    flags[(int)RuleFlag.IllLireCouleur] = true;
                }
            }
        }

        private bool[] flags;
        // on se sert de RuleFlags comme index dans le tableau.

        public AutomFlagsConfig()
        {
            flags = new bool[(int)RuleFlag.last];
            for (int i = 0; i < (int)RuleFlag.last; i++)
                flags[i] = true; // par défaut, les règles sont actives.
            flags[(int)RuleFlag.IllLireCouleur] = false; // config par défaut
        }

        /// <summary>
        /// Sets the <c>rf</c> flag to val.
        /// </summary>
        /// <param name="rf">the identifier of the flag.</param>
        /// <param name="val">the value to set the flag to.</param>
        private void SetFlag(RuleFlag rf, bool val) => flags[(int)rf] = val;

        /// <summary>
        /// Gives the value of the said flag.
        /// </summary>
        /// <param name="rf">identifier of the flag one wants the value of.</param>
        /// <returns>The value of the flag.</returns>
        public bool GetFlag(RuleFlag rf) => flags[(int)rf];

    }
}
