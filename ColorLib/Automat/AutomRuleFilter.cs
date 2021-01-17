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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ColorLib
{

    public class AutomRuleFilter : AutomElement
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private delegate bool CheckRuleFunction(string mot, int posMot);
        static private Dictionary<string, CheckRuleFunction> checkRuleFs = new Dictionary<string, CheckRuleFunction>()
        {
            {"Regle_ient", Regle_ient},
            {"Regle_mots_ent", Regle_mots_ent},
            {"Regle_ment", Regle_ment},
            {"Regle_verbe_mer", Regle_verbe_mer},
            {"Regle_er", Regle_er},
            {"Regle_nc_ai_final", Regle_nc_ai_final},
            {"Regle_avoir", Regle_avoir},
            {"Regle_s_final", Regle_s_final},
            {"Regle_t_final", Regle_t_final},
            {"Regle_tien", Regle_tien},
            {"Regle_finD", Regle_finD},
            {"Regle_ill", Regle_ill},
            {"Regle_ierConjI", Regle_ierConjI },
            {"Regle_ierConjE", Regle_ierConjE },
            {"Regle_VerbesTer", Regle_VerbesTer },
            {"Regle_MotsUM", Regle_MotsUM },
            {"Regle_X_Final", Regle_X_Final },
            {"Regle_ChK", Regle_ChK },
            {"Regle_MotsUN_ON", Regle_MotsUN_ON },
            {"Regle_finAM", Regle_finAM },
            {"RegleMotsQUkw", RegleMotsQUkw },
            {"RegleMotsEn5", RegleMotsEn5 },
            {"RegleMotsGnGN", RegleMotsGnGN },
            {"RegleMotsOYoj", RegleMotsOYoj },
            {"RegleMotsRe", RegleMotsRe },
        };

        private CheckRuleFunction crf;
        private Regex prevRegEx;
        private Regex follRegEx;
        private bool isFirstLetter;
        private bool isLastLetter;

        public AutomRuleFilter(string s, ref int pos)
            : base(s, pos)
        // on exit, pos points to the last character of the AutomRuleFilter. i.e. ']' or the character 
        // before the ',' in case of a "regle" function.
        {
            logger.ConditionalTrace("AutomRuleFilter");
            /*
             * A RuleFilter has the syntax
             * either 
             * {List of DirectionRegEx separated by ','}
             * where DirectionRegex is '+' | '-' : /regularExpression/i
             * example: {'-':/para/i,'+':/it/i}
             * 
             *or
             * 
             * this.name where name is the name of a CheckRuleFunction
             * Note that the name is followed by a coma, but the coma is the separator used for 
             * the upper level. Hence the returned pos must point on the last character before the ',' 
             */

            crf = null;
            prevRegEx = null;
            follRegEx = null;
            isFirstLetter = false;
            isLastLetter = false;


            // let's find what is next. It is either a function name or a list of DirectionRegEx
            pos = GetNextChar(pos);
            if (s[pos] == '{') // "normal" case
            {
                pos = GetNextChar(pos + 1);
                while (s[pos] != '}')
                {
                    //let's find the '
                    Debug.Assert(s[pos] == '\'', String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: on attend un \''\' en début de règle. {0}",
                        ErrorExcerpt(pos)));
                    //let's find the + or -
                    pos = GetNextChar(pos + 1);
                    bool plus;
                    if (s[pos] == '+')
                    {
                        plus = true;
                    }
                    else if (s[pos] == '-')
                    {
                        plus = false;
                    }
                    else
                        throw new ArgumentException(String.Format(ConfigBase.cultF, 
                            "AutomRuleFilter: on attend un \'+\' ou un \'+\'. {0}", ErrorExcerpt(pos)));
                    //let's find the '
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == '\'', String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: on attend un \''\' après + ou -. {0}", ErrorExcerpt(pos)));
                    //let's find the :
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == ':', String.Format(ConfigBase.cultF,
                        "AutomRuleFilter: on attend un \':\' après + ou -. {0}", ErrorExcerpt(pos)));
                    //let's find the /
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == '/', String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: on attend un \'/\' avant une regex. {0}", ErrorExcerpt(pos)));
                    // let's load the regex
                    // Let's assume that there is no '/' in the regex itself. Else we would need to
                    // handle the escape character that is necessary in .js
                    pos = GetNextChar(pos + 1);
                    int endOfRegexSlashPos = s.IndexOf('/', pos);
                    Debug.Assert(endOfRegexSlashPos > pos, String.Format(ConfigBase.cultF,
                        "AutomRuleFilter: on attend un \'/\' pour clore une regex. {0}", ErrorExcerpt(pos)));
                    Debug.Assert(s[pos - 1] != '\\', String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: le cas d\'un \\ n'est pas traité. {0}", ErrorExcerpt(pos)));
                    string theExpression = s.Substring(pos, endOfRegexSlashPos - pos).Trim();
                    StringBuilder sb = new StringBuilder(); // Building the regular expression
                    if (plus)
                    {
                        if ((theExpression[0] == '$') && (theExpression.Length == 1))
                        {
                            sb.Append(theExpression);
                            isLastLetter = true;
                        }
                        else
                        {
                            if (theExpression[0] != '^')
                                sb.Append(@"^"); 
                                // The match must occur at the start of the string. It is not in
                                // the table due to the algorithm of Marie-Pierre
                            sb.Append(theExpression);
                        }
                        Debug.Assert(follRegEx == null, String.Format(ConfigBase.cultF,
                            "AutomRuleFilter:  follRegEx must be null. {0}", ErrorExcerpt(pos)));
                        follRegEx = new Regex(sb.ToString(), RegexOptions.Compiled);
                    }
                    else
                    {
                        sb.Append(theExpression);
                        if ((theExpression[0] == '^') && (theExpression.Length == 1))
                            // A '^' alone means that the letter must be at the begining of the
                            // string - Special semantics defined by Marie-Pierre
                            isFirstLetter = true;
                        else if ((theExpression.Length >= 1) && (theExpression[theExpression.Length - 1] != '$'))
                            sb.Append("$"); 
                            // The match must occur at the end of the string. This $ is not in the
                            // regexes in the table, but could be there as well.
                        Debug.Assert(prevRegEx == null, String.Format(ConfigBase.cultF,
                            "AutomRuleFilter:  prevRegEx must be null. {0}", ErrorExcerpt(pos)));
                        prevRegEx = new Regex(sb.ToString(), RegexOptions.Compiled);
                    }
                    // let's find the i
                    pos = GetNextChar(endOfRegexSlashPos + 1);
                    Debug.Assert(s[pos] == 'i', String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: on attend un \'i\' après une regex. {0}", ErrorExcerpt(pos)));
                    // let's find the next character
                    pos = GetNextChar(pos + 1);
                    // it is either ',' or '}'
                    Debug.Assert((s[pos] == ',' || s[pos] == '}'), String.Format(ConfigBase.cultF, 
                        "AutomRuleFilter: on attend une \',\' entre deux DirectionRegex ou un \'}}\'. {0}",
                        ErrorExcerpt(pos)));
                    if (s[pos] == ',')
                        pos = GetNextChar(pos + 1);
                } // while
            }
            else  // it is a "regle" procedure
            {
                // the "regle" starts with "this."
                // let's find the dot
                var endOfThisDot = s.IndexOf('.', pos);
                Debug.Assert(endOfThisDot > 0, String.Format(ConfigBase.cultF, 
                    "AutomRuleFiletr: on attend un \'.\' pour délimiter \'this\'. {0}",
                    ErrorExcerpt(pos)));
                var thisTxt = s.Substring(pos, endOfThisDot - pos);
                Debug.Assert(thisTxt == "this", String.Format(ConfigBase.cultF,
                    "AutomRuleFiletr: La référence à une fonction de règle doit commencer par \"this\". {0}",
                    ErrorExcerpt(pos)));
                pos = endOfThisDot;
                // let's find the comma that terminates the name of the regle function
                pos = GetNextChar(pos + 1);
                var endOfNameComma = s.IndexOf(',', pos);
                var thisName = s.Substring(pos, endOfNameComma - (pos)).Trim();
                Debug.Assert(endOfNameComma > pos, String.Format(ConfigBase.cultF, 
                    "AutomRuleFilter: on attend une \',\' après le nom de fonction. {0}", ErrorExcerpt(pos)));
                var found = checkRuleFs.TryGetValue(thisName, out crf);
                Debug.Assert(found, String.Format(ConfigBase.cultF, 
                    "AutomRuleFilter: {0} n'est pas un nom valide. {1}", thisName, ErrorExcerpt(pos)));
                pos = endOfNameComma - 1; // pos points to the last char before the comma
            }
            end = pos;
        } // Constructor AutomRuleFilter

        private string FindNameForCRF(CheckRuleFunction crf)
        {
            foreach (KeyValuePair<string, CheckRuleFunction> k in checkRuleFs)
            {
                if (k.Value == crf)
                    return k.Key;
            }
            return "";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Original Text: " + base.ToString());
            sb.AppendLine("crf: " + FindNameForCRF(crf));
            sb.AppendLine("prevRegex: " + prevRegEx);
            sb.AppendLine("follRegEx: " + follRegEx);
            sb.AppendLine("isFirstLetter: " + isFirstLetter);
            return sb.ToString();
        }

        private static bool initiated = false;

        public static void InitAutomat()
        {
            logger.ConditionalDebug("InitAutomat");
            if (!initiated)
            {
                for (int i = 0; i < avoir_eu.Length; i++)
                    avoir_eu_hashed.Add(avoir_eu[i], null);
            }
            initiated = true;
        }


        private static Dictionary<char, char> accentMapping = new Dictionary<char, char> {
                {'à', 'a'},
                {'á', 'a'},
                {'ä', 'a'},
                {'â', 'a'},
                {'è', 'e'},
                {'é', 'e'},
                {'ê', 'e'},
                {'ë', 'e'},
                {'ï', 'i'},
                {'î', 'i'},
                {'ö', 'o'},
                {'ô', 'o'},
                {'ù', 'u'},
                {'ü', 'u'},
                {'û', 'u'}
            };

        // returns version of s, without accents.
        // works only for lower case chars
        public static string ChaineSansAccents(string s)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "ChaineSansAccents \'{0}\'", s);
            StringBuilder sb = new StringBuilder(s.Length);
            char woAccent;
            for (var i = 0; i < s.Length; i++)
            {
                if (accentMapping.TryGetValue(s[i], out woAccent))
                    sb.Append(woAccent);
                else
                    sb.Append(s[i]);
            }
            return sb.ToString();
        } // ChaineSansAccents

        public static string SansSFinal(string s)
        // s est en minuscules et n'est pas vide!
        {
            logger.ConditionalTrace(ConfigBase.cultF, "SansSFinal \'{0}\'", s);
            if (s[s.Length - 1] == 's')
                return s.Substring(0, s.Length - 1);
            else
                return s;
        } // SansSFinal

        public static string SansEFinal(string s)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "SansEFinal \'{0}\'", s);
            if (s[s.Length - 1] == 'e')
                return s.Substring(0, s.Length - 1);
            else
                return s;
        }


        private static Regex rxConsIent = new Regex("[bcçdéfghjklnmpqrstvwxz]ient$", RegexOptions.Compiled);

        /// <summary>
        /// Cherche si <paramref name="mot"/> se termine par "ient" et est la forme conjuguée au
        /// présent, 3e pers. du pluriel d'un verbe du premier groupe en "ier". 
        /// </summary>
        /// <remarks>Le mot doit être en minuscules.</remarks>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position dans le mot de la lettre actuellement analysée. Doit être
        /// dans les quatre dernières lettres. </param>
        /// <returns><c>true</c> si c'est une terminaison en "ient" et un verbe conjugué.</returns>
        public static bool Regle_ient(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ient - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert((pos >= 0) && (pos < mot.Length));

            bool toReturn = false;
            if (pos >= mot.Length - 4) // checking i or e from the "ient" any previous letter in the word would not fit.
            {
                if (rxConsIent.IsMatch(mot)) // le mot doit se terminer par 'ient' (précédé d'une consonne ou d'un é)
                {
                    // il faut savoir si le mot est un verbe dont l'infinitif se termine par 'ier' ou non
                    StringBuilder sb = new StringBuilder(mot.Length);
                    sb.Append(mot.Substring(0, mot.Length - 2));
                    sb.Append('r');
                    toReturn = verbes_ier.Contains(sb.ToString());
                }
            }
            return toReturn;
        }

        private static HashSet<string> termFutCond = new HashSet<string>()
        {
            { "ai" }, { "as" }, { "a" }, { "ons" }, { "ez" }, { "ont" },
            { "ais" }, { "ait" }, { "ions" }, { "iez" }, { "aient" }
        };

        /// <summary>
        /// Vérifie se le mote est une forme conjuguée en ier[*] au futur ou au conditionnel.
        /// </summary>
        /// <param name="mot">Le mot à vérifier.</param>
        /// <param name="pos_mot">La position du i de "ier" dans le mot.</param>
        /// <returns><c>true</c> s'il s'agit d'un verbe en ier conjugué au futur ou au
        /// conditionnel. <c>false</c> dans le cas contraire.
        /// </returns>
        public static bool Regle_ierConjI(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ierConjI - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            return (pos_mot > 0
                && pos_mot < mot.Length - 3
                && TextEl.EstConsonne(mot[pos_mot - 1])
                && mot[pos_mot] == 'i'
                && mot[pos_mot + 1] == 'e'
                && mot[pos_mot + 2] == 'r'
                && termFutCond.Contains(mot.Substring(pos_mot + 3)));
        }

        /// <summary>
        /// Comme <see cref="Regle_ierConjI(string, int)"/> mais avec <paramref name="pos_mot"/>
        /// pointant sur le 'e'.
        /// </summary>
        /// <param name="mot">Le mot à analyser</param>
        /// <param name="pos_mot">La position de la lettre e dans la terminason 'ier'...</param>
        /// <returns><c>true</c> s'il s'agit d'un verbe en 'ier' conjugué au futur ou au
        /// conditionnel, <c>false</c> sinon. </returns>
        public static bool Regle_ierConjE(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ierConjE - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            return (pos_mot > 1
                && pos_mot < mot.Length - 2
                && TextEl.EstConsonne(mot[pos_mot - 2])
                && mot[pos_mot - 1] == 'i'
                && mot[pos_mot] == 'e'
                && mot[pos_mot + 1] == 'r'
                && termFutCond.Contains(mot.Substring(pos_mot + 2)));
        }

        /// <summary>
        /// Liste des mots se terminant par ent et se prononçant a~/@
        /// </summary>
        /// <remarks>
        /// Pas besoin que liste contienne les version féminines ou plurielles des mots.
        /// </remarks>
        private static HashSet<string> mots_ent = new HashSet<string>
        {
            "absent", "abstinent", "accent", "accident", "adhérent", "adjacent",
            "adolescent", "afférent", "agent", "ambivalent", "antécédent", "apparent",
            "arborescent", "ardent", "argent", "arpent", "astringent", "auvent",
            "avent", "cent", "chiendent", "client", "coefficient", "cohérent", "compétent","dent",
            "conscient", "conséquent", "continent", "concurrent", "conférent", "confluent",
            "différent", "diligent", "dissident", "divergent", "dolent", "décadent", "décent",
            "déficient", "déférent", "déliquescent", "détergent", "excipient", "fervent", "flatulent",
            "fluorescent", "fréquent", "féculent", "gent", "gradient", "grandiloquent",
            "immanent", "imminent", "impatient", "impertinent", "impotent", "imprudent",
            "impudent", "impénitent", "incandescent", "incident", "incohérent", "incompétent",
            "inconscient", "inconséquent", "incontinent", "inconvénient", "indifférent", "indigent",
            "indolent", "indulgent", "indécent", "ingrédient", "inhérent", "inintelligent",
            "innocent", "insolent", "intelligent", "interférent", "intermittent", "iridescent",
            "lactescent", "latent", "lent", "luminescent", "malcontent", "mécontent", "occident",
            "omnipotent", "omniprésent", "omniscient", "onguent", "opalescent", "opulent",
            "orient", "paravent", "parent", "patent", "patient", "permanent", "pertinent", "phosphorescent",
            "polyvalent", "pourcent", "proéminent", "prudent", "précédent", "présent",
            "prévalent-", "pschent", "purulent", "putrescent", "pénitent", "quotient",
            "relent", "récent", "récipient", "récurrent", "référent", "régent", "rémanent",
            "réticent", "sanguinolent", "sergent", "serpent", "somnolent", "souvent",
            "spumescent", "strident", "subconscient", "subséquent", "succulent", "tangent",
            "torrent", "transparent", "trident", "truculent", "tumescent", "turbulent",
            "turgescent", "urgent", "vent", "ventripotent", "violent", "virulent", "effervescent",
            "efficient", "effluent", "engoulevent", "entregent", "escient", "event",
            "excédent", "expédient", "éloquent", "éminent", "émollient", "évanescent", "évent",
            "agrément", "aliment", "ciment","content","compliment","boniment","document",
            "parlement","ornement","supplément","tourment","spent", "argument", "abrivent",
            "accrescent", "acescent", "albescent", "alcalescent", "amarescent", "concupiscent",
            "convalescent", "dégénérescent", "déhiscent", "délitescent", "détumescent", "efflorescent",
            "érubescent", "flavescent", "florescent", "frutescent", "ignescent", "imputrescent",
            "indéhiscent", "marcescent", "négrescent", "photoluminescent", "pubescent", "quiescent",
            "rarescent", "recrudescent", "résipiscent", "reviviscent", "réviviscent", "rubescent",
            "sénescent", "somnolescent", "spinescent", "thermoluminescent","adent", "affident",
            "affluent", "appétent", "attingent", "avirulent", "bénéolent", "bénévolent", "bident",

            "bivalent","compétent", "concurrent", "conférent", "confluent", "congruent", "connivent", "consent",
            "conséquent", "constringent", "continent", "contingent", "contrevent", "convent",
            "convergent-", "corpulent", "couvent-", "covalent", "crément", "décurrent", "déponent",
            "diffringent", "efférent", "émergent", "émollient", "émulgent", "équipollent",
            "équipotent", "équivalent", "esculent", "excellent", "expédient", "floculent",
            "fluent", "gradient", "gravéolent", "impatient", "inapparent", "inexpérient",
            "influent", "infravalent", "ingrédient", "insurgent-", "intercurrent", "irrévérent",
            "jacent", "magnificent", "maléolent", "maltalent", "mellifluent", "métalent", "monovalent",
            "munificent", "négligent", "obédient", "orient", "patient", "pestilent", "plurivalent",
            "précellent", "prééminent", "prépotent", "prominent", "pulvérulent", "pustulent",
            "quadrivalent", "quotient", "réfringent", "rémittent", "rénitent", "résilient", "restringent",
            "résurgent", "révérent", "sapient", "sempervirent", "silent", "similargent", "subjacent",
            "suréminent", "taillevent", "talent", "trivalent", "univalent",

        };

        /*
         * Règle spécifique de traitement des successions de lettres '*ent'
         * sert à savoir si le mot figure dans les mots qui se prononcent a_tilda à la fin
         * true si c'est le cas.
         * Attention les mots en "ment" sont traités ailleurs.
         * 
         * Précondition: mot est en minuscules
         */
        public static bool Regle_mots_ent(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_mots_ent - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            string comparateur = SansEFinal(SansSFinal(mot));
            if (pos_mot == comparateur.Length - 3) // on teste le e de la terminaison
            {
                Regex r = new Regex("^[bcdfghjklmnpqrstvwxz]ent$", RegexOptions.IgnoreCase); // mot court, évtlmt imaginaire...
                if (r.IsMatch(comparateur))
                    toReturn = true;
                else
                    toReturn = mots_ent.Contains(comparateur);
            }
            return toReturn;
        }



        /// <summary>
        /// Détermine si <paramref name="mot"/> se termine par "ment" et se prononce a~/@.
        /// </summary>
        /// <remarks>Utilise la liste des verbes en "mer". Si le mot correspond à un verbe
        /// conjugué, retourne <c>false</c>. Sinon <c>true</c></remarks>
        /// <param name="mot">Le mot à examiner.</param>
        /// <param name="pos_mot">La position de 'e' dans la terminaison "ent".</param>
        /// <returns><c>true</c> si le mot en "ment" se prononce a~/@</returns>
        public static bool Regle_ment(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ment - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            Regex r = new Regex("ment$", RegexOptions.IgnoreCase); // le mot doir finir par ment
            if ((pos_mot >= mot.Length - 3) && (r.IsMatch(mot))) // on n'est pas en train de traiter une lettre avant la terminaison
            {
                string pseudo_infinitif = ChaineSansAccents(mot.Substring(0, mot.Length - 2) + 'r');
                toReturn = !verbes_mer.Contains(pseudo_infinitif);
            }
            return toReturn;
        }

        /// <summary>
        /// Détermine si <paramref name="mot"/> se termine par "ment" et est un verbe
        /// conjugué au présent 3e pers. pluriel. De fait l'inverse le la méthode précédente.
        /// </summary>
        /// <param name="mot">Le mot à examiner.</param>
        /// <param name="pos_mot">La position de 'e' dans la terminaison "ent".</param>
        /// <returns><c>true</c> si le mot en "ment" est un verbe conjugué.</returns>
        public static bool Regle_verbe_mer(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_verbe_mer - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            Regex r = new Regex("ment$", RegexOptions.IgnoreCase); // le mot doir finir par ment
            if ((pos_mot >= mot.Length - 3) && (r.IsMatch(mot))) // on n'est pas en train de traiter une lettre avant la terminaison
            {
                string pseudo_infinitif = ChaineSansAccents(mot.Substring(0, mot.Length - 2) + 'r');
                toReturn = verbes_mer.Contains(pseudo_infinitif);
            }
            return toReturn;
        }

        /// <summary>
        /// Liste des mots se terminant par 'er' et qui se prononcent [ER].
        /// </summary>
        /// <remarks>
        /// Attention: sans le 's' final qui est condiéré comme pluriel.
        /// </remarks>
        private static HashSet<string> exceptions_final_er = new HashSet<string>
        {
            "aber", "acquier", "afrikander", "alter", "amer", "amphiaster", "aster", "auster",
            "aver", "baedeker", "ber", "bitter", "-boxer", "bulldozer", "cancer", "carter",
            "cascher", "casher", "cathéter", "cawcher", "charter", "cher", "chester", "cocker",
            "container", "conver", "corner", "coroner", "cracker", "crémaster", "cuiller", "cutter",
            "dever", "déver", "diver", "docker", "doppler", "eider", "enfer", "entrefer", "enver",
            "ester", "éther", "fer", "fier", "gangster", "getter", "geyser", "hamster", "hier",
            "highlifer", "hiver", "inter", "joker", "junker", "khmer", "kirschwasser", "laser",
            "liber", "loader", "mâchefer", "magister", "manager", "master", "mauser", "mer", "munster",
            "obver", "outremer", "-palmer", "panzer", "papaver", "partner", "per", "perver", "poker",
            "polder", "polyester", "poster", "pullover", "-quarter", "quater", "rever", "-reporter",
            "revolver", "révolver", "roadster", "scanner", "schnauzer", "scooter", "setter", "spencer",
            "sphincter", "spider", "spinaker", "springer", "sprinter", "starter", "steamer", "super",
            "-supporter", "sylvaner", "tender", "ter", "thaler", "tier", "traver", "trochanter",
            "tuner", "ulster", "univer", "ver", "vétiver", "water", "weber", "welter", "winchester",
            "vomer",
            "contrefer", "desser", "driver", "dulcimer", "enquier", "er", "ferouer", "ferver",
            "frater", "gaster", "keuper", "maser", "masséter", "messer", "néper", "pater",
            "quarter", "reconquier", "requier", "resser", "ser", "spalter",
            "stathouder", "suber", "ulster", "vétyver", "traminer",
        };

        /*
         * Règle spécifique de traitement des successions de lettres finales 'er'
         * sert à savoir si le mot figure dans la liste des exceptions
         * qui ne se prononcent pas é
         * 
         * Précondition: mot est en minuscules et non null
         */
        /// <summary>
        /// Détermine si le mot se termine en 'er' ou 'ers' et se pronoce [ER].
        /// </summary>
        /// <param name="mot">Le mot à anlayser.</param>
        /// <param name="pos_mot"></param>
        /// <returns></returns>
        public static bool Regle_er(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_er - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 'e'); // sinon un 's' seul ferait sauter la banque plus bas...

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if ((pos_mot >= mSing.Length - 2) && (mSing[mSing.Length - 1] == 'r'))
                // on n'est pas en train de traiter une lettre avant la terminaison
                // le mot se termine par 'r'
                toReturn = exceptions_final_er.Contains(mSing);
            return toReturn;
        }

        /// <summary>
        /// Liste des mots se terminant par 'ai' qui se prononce [E].
        /// </summary>
        private static HashSet<string> noms_ai = new HashSet<string>
        {
            "balai", "brai", "chai", "déblai", "délai", "essai", "frai", "geai", "lai", "mai",
            "minerai", "papegai", "quai", "rai", "remblai", "vrai" // PAE: ajouté "vrai" 18.05.20
        };

        /*
         * Règle spécifique de traitement des noms communs qui se terminent par 'ai'
         * Dans les verbes terminés par 'ai', le phonème est 'é'
         * Dans les noms communs terminés par 'ai', le phonème est 'ê'
         * retourne true s'il s'agit d'un nom donc 'ê'
         * 
         * Précondition: mot est en minuscules
         */
        public static bool Regle_nc_ai_final(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_nc_ai_final - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            if ((pos_mot == mot.Length - 2) && (mot[mot.Length - 1] == 'i'))
                toReturn = noms_ai.Contains(mot);
            return toReturn;
        }

        static string[] avoir_eu =
        {
            "eu", "eue", "eues", "eus", "eut", "eûmes", "eûtes", "eurent",
            "eusse", "eusses", "eût", "eussions", "eussiez", "eussent"
        };

        private static StringDictionary avoir_eu_hashed = new StringDictionary();

        /*
         * Règle spécifique de traitement des successions de lettres 'eu'
         * Sert à savoir si le mot est le verbe avoir conjugué (passé simple, participe
         * passé ou subjonctif imparfait
         */
        public static bool Regle_avoir(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_avoir - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            if ((pos_mot == 0) && (mot.Length > 1) && ((mot[1] == 'u') || (mot[1] == 'û')))
                toReturn = avoir_eu_hashed.ContainsKey(mot);
            return toReturn;
        }

        /// <summary>
        /// Liste des mots se terminant par 's' où le 's' se prononce.
        /// </summary>
        private static HashSet<string> mots_s_final = new HashSet<string>
        {
            "abies", "abraxas", "acarus", "acens", "acinus", "adonis", "agasillis", "agnus", "agnès",
            "agrostis", "albatros", "albinos", "albinos", "alcarazas", "alios", "alkermès", "aloès",
            "amadis", "amaryllis", "ambesas", "ambitus", "amnios", "amorphophallus", "amphioxus",
            "anacampséros", "anagyris", "anchilops", "angélus", "anis", "anschluss", "anthyllis",
            "anthémis", "antinoüs", "antitragus", "anus", "apios", "apis", "apus", "argus", "arsis",
            "artocarpus", "-as", "ascaris", "asclépias", "asparagus", "aspergillus", "aspergès",
            "atlas", "attacus", "autobus", "axis", "azygos", "azygos", "bacchus", "basileus",
            "bellis", "benedictus", "benthos", "bibliobus", "bibus", "biceps", "bis", "bis", "bis",
            "bisness", "blaps", "blockhaus", "blocus", "bonus", "boss", "business", "byblos",
            "byssus", "cacatoès", "cactus", "calasiris", "callaïs", "calvados", "campus", "carolus",
            "carus", "catharsis", "catoblépas", "-cavas", "cawas", "cens", "centranthus", "cers",
            "chionis", "chips", "chips", "chlass", "chorus", "christmas", "cirrus", "citrus",
            "clebs", "clitoris", "coccus", "cochylis", "collapsus", "coléus", "committimus", "complexus",
            "conchylis", "consensus", "contresens", "convolvulus", "corpus", "cortès", "corylopsis",
            "coréopsis", "cosinus", "cosmos", "costus", "couscous", "couscouss", "creps", "criss",
            "crocus", "cross", "crésus", "csardas", "cubitus", "cumulus", "cupressus", "cursus",
            "cycas", "cyclas", "cynips", "cypris", "céréus", "dinornis", "diplodocus", "doris",
            "dromos", "dross", "décubitus", "dégobillis", "détritus", "edelweiss", "eucalyptus",
            "eudémis", "excursus", "express", "express", "facies", "faciès", "famulus", "favus",
            "ficus", "fils", "fœtus", "fongus", "forceps", "fucus", "galéopsis", "garus", "gauss",
            "gibus", "glass", "gneiss", "gradus", "gratis", "gratis", "gus", "gyrus", "habitus",
            "hamadryas", "hamamélis", "hammerless", "heimatlos", "heimatlos", "hendiadys", "hermès",
            "herpès", "hiatus", "hibiscus", "humus", "humérus", "hypocras", "hypothalamus", "hystérésis",
            "hélas", "ibis", "ichtyornis", "ichtys", "ictus", "iléus", "infarctus", "inlandsis",
            "iris", "iritis", "isatis", "ithos", "jacobus", "jadis", "kermès", "klebs", "kleps",
            "knickerbockers", "koumis", "koumys", "kouros", "kriss", "kss", "kwas", "kwass", "labadens",
            "lapis", "laps", "lapsus", "lathyrus", "laïus", "leggings", "leggins", "lexis", "lias",
            "libripens", "links", "liparis", "lituus", "loculus", "locus", "lœss", "logos", "londrès",
            "lotus", "lupus", "lychnis", "lys", "machairodus", "madras", "mallus", "maous", "maouss",
            "maravédis", "mars", "mas", "mathesis", "maïs", "mess", "minus", "mirabilis", "miss",
            "mistress", "modius", "moos", "mordicus", "moss", "motocross", "motus", "mucus", "myosis",
            "myosotis", "médius", "méphistophélès", "mérinos", "métis", "naevus", "naos", "nauplius",
            "nexus", "nimbus", "ninas", "nodus", "nonius", "nostras", "notos", "notus", "nucléus",
            "nystagmus", "négus", "némésis", "népenthès", "oaristys", "oasis", "oculus", "oestrus",
            "olibrius", "omnibus", "omnibus", "ononis", "onyxis", "ophrys", "opus", "orchis",
            "oribus", "orémus", "os", "ours", "oxalis", "pagus", "paliurus", "palmarès", "pancréas",
            "pandanus", "pannus", "papas", "papyrus", "paros", "pastis", "pataquès", "pathos",
            "pelvis", "pemphigus", "phallus", "phimosis", "phtiriasis", "physalis", "phébus",
            "pityriasis", "plexiglas", "plexus", "poncirus", "praxis", "princeps",
            "processus", "prolapsus", "promérops", "pronaos", "propolis", "prospectus", "proteus",
            "protéus", "prunus", "psoas", "psoriasis", "ptosis", "pubis", "pyrosis", "pécoptéris",
            "pénis", "péplos", "quadriceps", "quibus", "quitus", "rachis", "radius", "rams", "raptus",
            "rasibus", "raïs", "reis", "relaps", "relaps", "reps", "reïs", "rhinocéros", "rhombus",
            "rhésus", "rictus", "risorius", "risorius", "rollmops", "rébus", "rétrovirus", "salpiglossis",
            "sanctus", "saros", "sas", "satyriasis", "schlass", "schnaps", "schuss", "schuss",
            "sempervirens", "-sens", "seps", "sialis", "siemens", "sinus", "sirventès", "sitaris",
            "skungs", "skunks", "socius", "solidus", "speiss", "splénius", "spéos", "stamnos",
            "stimulus", "stokes", "stradivarius", "strass", "stratus", "stress", "strophantus",
            "strychnos", "sycosis", "syllabus", "synopsis", "syphilis", "tabès", "tamaris", "tarantass",
            "taxus", "tennis", "terminus", "thalamus", "thermos", "thesaurus", "tholos", "thrips",
            "thrombus", "thymus", "thésis", "tonus", "tophus", "tournevis", "tractus", "tragus",
            "trass", "trias", "triceps", "triceratops", "trichiasis", "trismus", "tumulus", "tupinambis",
            "turneps", "tylenchus", "typhus", "téniasis", "tétanos", "upas", "uraeus", "urus",
            "us", "utérus", "valgus", "valgus", "varus", "vasistas", "vidimus", "virus", "vitellus",
            "volubilis", "volvulus", "vénus", "williams", "xiphias", "xérès", "yass", "échinocactus",
            "édelweiss", "élaeis", "éléphantiasis", "épistaxis", "éros", "éthiops", "acanthéchinus",
            "-honores", "patres", "ains", "alloposus", "amblyopsis", "amblyornis",
            "antivirus", "bathycrinus", "bathyptéroïs", "batrachoseps", "caryorrhexis", "cetorhinus",
            "chéiranthus", "chéiromys", "chélys", "chéniscus", "chiroteuthis", "chlamydomonas",
            "chronotaraxis", "colotyphus", "craniotabes", "criocarcinus", "crypsis", "cynanthémis",
            "cynorchis", "tremens", "profundis", "dyscomyces", "électrobus",
            "électrotonus", "épicanthus", "épispadias", "ès", "muros", "florès", "glomus",
            "glossochilus", "gyrobus", "habeas", "corpus", "haliotis", "halobenthos", "hippotigris",
            "hypertonus", "extremis", "inmedias", "res", "partibus", "intemporalibus",
            "iridodonésis", "knickers", "lophaetus", "macrothésaurus", "malus", "mégacéros", "méningotyphus",
            "métanauplius", "micrococcus", "minibus", "minus", "habens", "monocéros", "morphochorésis",
            "myriagauss", "myxovirus", "naturalibus", "néphrotyphus", "neuroptéris", "nimbostratus",
            "nounours", "numerus", "clausus", "odontophorus", "oligoamnios", "ovibos", "ovotestis",
            "palatoschizis", "pardeuss", "pedibus", "pentacrinus", "périonyxis", "phycomyces",
            "phytéléphas", "pleurocanthus", "poliovirus", "prémycosis", "préoestrus",
            "doloris", "pronéphros", "pronucléus", "protococcus", "protopterus", "provirus", "pterocarpus",
            "pterygotus", "rapidos", "rhinochetus", "rhinocoris", "rhinovirus", "rhizopus", "s",
            "saccharomyces", "schizanthus", "virens", "sensass", "sphénoptéris",
            "s", "stegosaurus", "generis", "syndésis", "syneidésis", "synizésis",
            "syntomis", "thanatos", "thésaurus", "tricératops", "trichorhexis", "trolleybus",
            "tss", "typhlosolis", "ultravirus", "uranoschisis", "uranostaphyloschisis", "vidéobus",
            "pecus", "zoobenthos", "zygoptéris", "zygopteris",
            "abribus", "airbus", "bus", "microbus", "mortibus", "pédibus", "autofocus", "focus",
            "erectus", "modus", "plus", "liquidus", "versus", "ratus", "burnous", "tous",
            "anubis", "craignos", "tranquillos", "alias", "sensas", "tapas", "gambas", "oups",
            "williams", "aegilops", "aepyornis", "afficionados", "bambinos", "s", // "s" isolé se prononce [s]
            // sans accents
            "agnes", "alkermes", "aloes", "anacampseros", "angelus", "anthemis", "antinous", "asclepias",
            "-asperges", "cacatoes", "-callais", "catoblepas", "-coleus", "cortes", "coreopsis",
            "cresus", "-cereus", "decubitus", "degobillis", "detritus", "eudemis", "galeopsis",
            "hamamelis", "hermes", "-herpes", "humerus", "hysteresis", "helas", "ileus", "kermes",
            "laius", "londres", "maravedis", "-mais", "medius", "mephistopheles", "merinos", "metis",
            "nucleus", "negus", "nemesis", "nepenthes", "oremus", "palmares", "pancreas", "pataques",
            "phebus", "promerops", "pecopteris", "penis", "peplos", "-rais", "rhinoceros", "rhesus",
            "rebus", "retrovirus", "-sirventes", "splenius", "speos", "tabes", "thesis", "teniasis",
            "tetanos", "uterus", "-venus", "xeres", "echinocactus", "elaeis", "elephantiasis",
            "epistaxis", "eros", "ethiops", "acanthechinus", "bathypterois", "cheiranthus", "cheiromys",
            "chelys", "cheniscus", "cynanthemis", "electrobus", "electrotonus", "epicanthus",
            "epispadias", "es", "-flores", "iridodonesis", "macrothesaurus", "megaceros", "meningotyphus",
            "metanauplius", "monoceros", "morphochoresis", "nephrotyphus", "neuropteris", "perionyxis",
            "phytelephas", "premycosis", "preoestrus", "pronephros", "pronucleus", "sphenopteris",
            "syndesis", "syneidesis", "synizesis", "videobus", 
        };


        /// <summary>
        /// Dit si le s final se pronoce pour <paramref name="mot"/>.
        /// </summary>
        /// <param name="mot">Le mot à analyser</param>
        /// <param name="pos">La position de la lettre sous analyse. En l'occurence le s 
        /// final.</param>
        /// <returns><c>true</c> si le s final se prononce.</returns>
        public static bool Regle_s_final(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_s_final - mot: \'{0}\', pos: {1}",
                mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert((pos >= 0) && (pos < mot.Length));
            Debug.Assert(mot[pos] == 's');

            bool toReturn = false;
            if (pos == mot.Length - 1)
                toReturn = mots_s_final.Contains(mot);
            return toReturn;
        } // Regle_s_final

        /// <summary>
        /// Liste des mots se termionant par 't' où le 't' se prononce.
        /// </summary>
        private static HashSet<string> mots_t_final = new HashSet<string>
        {
            "abject", "abrupt", "abrupt", "abstract", "accessit", "aconit", "affect", "affidavit",
            "ajust", "alcootest", "anet", "antichrist", "antitrust", "antéchrist", "août", "artéfact",
            "azimut", "ballast", "bit", "boycott", "breakfast", "brut", "béhémot", "catgut", "celebret",
            "chott", "christ", "chut", "cobalt", "cockpit", "colt", "compact", "compost",
            "comput", "concept", "contact", "convict", "copyright", "correct", "covercoat", "coït",
            "cricket", "cronstadt", "digest", "diktat", "direct", "direct", "discount", "district",
            "dot", "drifft", "drift", "duffelcoat", "durit", "déficit", "-est", "exeat", "exocet",
            "fat", "fiat", "flirt", "fret", "gadget", "gestalt", "hast", "horst", "huit", "hypercorrect",
            "impact", "incipit", "incorrect", "indirect", "indult", "inexact", "infect", "intact",
            "intellect", "karst", "kart", "kilowatt", "kilt", "kit", "knout", "kraft", "kumquat",
            "lest", "lift", "magnificat", "malt", "mat", "mat", "mazout", "moult", "net", "net",
            "obit", "occiput", "offset", "offset", "ost", "ouest", "oust", "out", "output", "pat",
            "peppermint", "percept", "pft", "pfut", "pfutt", "phot", "pickpocket", "prout", "prurit",
            "prétérit", "pschent", "pschit", "pschitt", "pscht", "pschtt", "pschut", "pschutt",
            "psit", "psst", "putt", "racket", "raout", "rapt", "runabout", "rut", "réquisit",
            "samizdat", "satisfecit", "scat", "scorbut", "scout", "scout", "script", "select",
            "sephirot", "sept", "set", "shoot", "short", "shunt", "sinciput", "skeet", "smalt",
            "smart", "socket", "soviet", "spalt", "spart", "spot", "sprat", "sprint", "squat",
            "stabat", "stout", "strict", "sunlight", "sécurit", "sélect", "tact", "test", "tilt",
            "toast", "tract", "transat", "transept", "transit", "trust", "tsitsit", "twist", "tzitzit",
            "umlaut", "uppercut", "ut", "veniat", "verdict", "volcelest", "volt", "watt", "whist",
            "yacht", "yaourt", "yoghourt", "yogourt", "zest", "ziggourat", "zut",
            "foot", "bast", "scout", "boat", "centriciput", "coat", "heat", "cart", "but", "exit",
            "west", "-trot", "hectowatt", "input", "instit", "hot", "-jet", "kilovolt","mégawatt", 
            "-pot", "microvolt", "microwatt", "millivolt", "basket", "monowatt", "brest",
            "shot", "permafrost", "pippermint","préconcept","privat", "docent", "radiocobalt", 
            "rocket", "rupt", "séephirot", "government", "boot", "snowboot", "squatt", "steamboat", 
            "subtest", "superjet", "sweat", "shirt", "sweatshirt", "thrombotest", "weight", 
            "closet", "jacket", "spirit", "ziggurat", "zist", "cet", "audit", "cajeput", "granit",
            "internet", "introït", "inuit",
            // sans accents
            "antechrist", "aout", "artefact", "behemot", "coit", "deficit", "preterit", "-requisit",
            "securit", "megawatt", "preconcept", "seephirot", "introit",
        };

        /// <summary>
        /// Dit si le t final se pronoce pour <paramref name="mot"/>. Les mots au pluriel
        /// sont traités également.
        /// </summary>
        /// <param name="mot">Le mot à analyser. Peut être au pluriel.</param>
        /// <param name="pos">La position de la lettre sous analyse. En l'occurence le t final. 
        /// </param>
        /// <returns><c>true</c> si le t final se prononce.</returns>
        public static bool Regle_t_final(string mot, int pos_mot)
        // pos_mot pointe sur un 't'
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_t_final - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 't');

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if (pos_mot == mSing.Length - 1)
                toReturn = mots_t_final.Contains(mSing);
            return toReturn;
        } // Regle_t_final

        static Regex rTien = new Regex(".+[beéfhns]tien.*", RegexOptions.Compiled);
        // hypothèse: il n'existe pas de mot contenant deux fois "tien"

        static Regex rTien2 = new Regex(
            "(^chré|^sou|^appar|^dé|^ap|^ar|^astar|ch(a|â)|flauber|lacer)tien", 
            RegexOptions.Compiled);

        /// <summary>
        /// Recherche si <paramref name="mot"/> se termine par tien(*) où le t se prononce [t]
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos_mot">La position du 't' de "tien" dans <paramref name="mot"/>.</param>
        /// <returns><c>true</c> si le t de "tien" se prononce [t].</returns>
        public static bool Regle_tien(string mot, int pos_mot)
        // pos_mot pointe sur un 't'
        // Cette fonnction n'est pas tout à fait dans la philosophie de l'automate.
        // On aurait pu isoler les deux cas à l'aide de règles et ne traîter ici que
        // les exceptions. --> À faire... à l'occasion
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_tien - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 't'); // pas vraiment nécessaire, mais autre chose n'a pas de sens.

            bool toReturn = false;

            // vérifions que le 't' se trouve bien au début de "tien"
            if ((mot.Length - pos_mot >= 4)
                && (mot[pos_mot + 1] == 'i')
                && (mot[pos_mot + 2] == 'e')
                && (mot[pos_mot + 3] == 'n')) {

                // vérifions si le 'tien se trouve  au début du mot
                if (pos_mot == 0)
                    toReturn = true; // tous les mots commençant par 'tien' ---> 't'
                else
                {
                    if (rTien.IsMatch(mot) 
                        && !mot.StartsWith("capétien")
                        && !mot.StartsWith("lutétien")
                        )
                        toReturn = true;
                    else
                    {
                        // il reste les exceptions qui commencent par "chrétien","soutien",
                        // "appartien", "détien", "aptien" (oui, oui ça semble exister...)
                        toReturn = rTien2.IsMatch(mot);
                    }
                }
            }
            return toReturn;
        } // Regle_tien

        /// <summary>
        /// Liste des mots se termionant par 'd' où le 'd' se prononce.
        /// </summary>
        private static HashSet<string> mots_d_final = new HashSet<string>
        {
            "apartheid", "aïd", "background", "barmaid", "baroud", "band", "bled", "caïd", "celluloïd", "damned",
            "djihad", "kid", "fjord", "hard", "jihad", "lad", "lord", "sud", "oued", "pad", "plaid", "polaroid", "polaroïd",
            "rhodoïd", "shetland", "board", "skateboard", "skinhead", "steward", "tabloïd", "end", "adalid",
        };

        public static bool Regle_finD(string mot, int pos_mot)
        // retourne true si on est sur le d final et le mot se termine par un 'd' qui se prononce
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_finD - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot[pos_mot] == 'd', "Regle_finD: on attend un 'd'");

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if (pos_mot == mSing.Length - 1)
                toReturn = mots_d_final.Contains(mSing);
            return toReturn;
        }

        /// <summary>
        /// Liste des mots se terminant par 'am' où celà se prononce [am].
        /// </summary>
        private static HashSet<string> mots_am_final = new HashSet<string>
        {
            "ayam", "bairam", "baïram", "bantam", "brougham", "clam", "dam", "goddam", "gram",
            "hammam", "imam", "islam", "jéroboam", "lingam", "litham", "macadam", "madapolam",
            "ogam", "ogham", "pyroceram", "pyrocéram", "quidam", "ramdam", "sélam", "siam", "tarmacadam",
            "tram", "wigwam", "ram", "william"
        };

        public static bool Regle_finAM(string mot, int pos_mot)
        // retourne true si on est sur le d final et le mot se termine par un 'd' qui se prononce
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_finAM - mot: \'{0}\', pos: {1}", mot, pos_mot);
            Debug.Assert(mot[pos_mot] == 'a', "Regle_finAM: on attend un 'a'");

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if (pos_mot == mSing.Length - 2)
                toReturn = mots_am_final.Contains(mSing);
            return toReturn;
        }

        

        /// <summary>
        /// Vérifie si le mot est une exception pour les lettres ill qui se prononcent [il]. La 
        /// méthode peut être appelée pour le i de "ill" et pour le prmier 'l' de "ill"
        /// </summary>
        /// <param name="mot">Le mot à vérifier</param>
        /// <param name="pos_mot">la position (basée sur zéro) de la lettre dans le mot qu'on est
        /// en train d'étudier</param>
        /// <returns><c>true</c> si on trouve bien 'ill' à la position donnée et qu'il se prononce
        /// [il]</returns>
        public static bool Regle_ill(string mot, int pos_mot)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ill - mot: \'{0}\', pos: {1}", mot, pos_mot);
            bool condMet = false;
            if (mot[pos_mot] == 'i')
                condMet = ((pos_mot < mot.Length - 2) && (mot[pos_mot + 1] == 'l') && (mot[pos_mot + 2] == 'l'));
            else if (mot[pos_mot] == 'l')
                condMet = ((pos_mot > 0) && (mot[pos_mot - 1] == 'i') && (pos_mot < mot.Length - 1) && (mot[pos_mot + 1] == 'l'));
            return (condMet && except_ill.Contains(mot));
        }

        public bool Check(PhonWord pw, int pos, string firstPart, string secondPart)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Check - pw: \'{0}\', pos: {1}, firstPart: \'{2}\', secPart: \'{3}\'",
                pw, pos, firstPart, secondPart);
            Debug.Assert(pw != null);
            Debug.Assert(pw.GetWord().Length > 0);

            bool toReturn = false;
            if (crf != null)
            {
                toReturn = crf(pw.GetWord(), pos);
            }
            else
            {
                bool prevResp = true;
                bool follResp = true;
                if (prevRegEx != null)
                {
                    if (isFirstLetter)
                        prevResp = (pos == 0);
                    else
                        prevResp = prevRegEx.IsMatch(firstPart);
                }
                if (follRegEx != null)
                {
                    if (isLastLetter)
                        follResp = (pos == pw.GetWord().Length - 1);
                    else
                        follResp = follRegEx.IsMatch(secondPart);
                }
                toReturn = (prevResp && follResp);
            }
            return toReturn;
        }

        /// <summary>
        /// Vérifie si le mot se termine par "um" ou "ums" et si le u se prononce O.
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'u' dans la terminaison um(s?).</param>
        /// <returns><c>true</c> si "um" se prononce Om.</returns>
        public static bool Regle_MotsUM(string mot, int pos)
        {
            Debug.Assert(mot != null);
            bool toReturn = false;
            if (pos > mot.Length - 4 && pos < mot.Length - 1 && mot[pos] == 'u' && mot[pos + 1] == 'm')
            {
                string lemme = SansSFinal(mot);
                toReturn = motsUM.Contains(ChaineSansAccents(lemme));
            }
            return toReturn;
        }

        /// <summary>
        /// Vérifie si le mot est un verbe à la 1e ou 2e personne du pluriel de l'imparfait (fin
        /// en 'tions' ou 'tiez').
        /// Par ex. "nous formations". Le but est de découvrir que "ti" se prononce ti et non si.
        /// </summary>
        /// <remarks>Utilise la liste <c>verbesTer</c> qui se trouve à la fin du fichier.</remarks>
        /// <param name="mot">Mot à analyser.</param>
        /// <param name="pos">Position du t de 'tions' ou 'tiez' dans le mot.</param>
        /// <returns><c>true</c> si 'tions' ou 'tiez' est utilisé pour conjuguer un verbe.
        /// <c>false</c> si <paramref name="pos"/> ne correspond pas au 't' de 'tions' ou 'tiez', 
        /// ou s'il ne s'agit pas d'un verbe à l'imparfait.</returns>
        public static bool Regle_VerbesTer(string mot, int pos)
        {
            Debug.Assert(mot != null);
            bool toReturn = false;
            if ((pos == mot.Length - 5
                && mot.EndsWith("tions"))
                ||
                (pos == mot.Length - 4
                && mot.EndsWith("tiez")))
            {
                StringBuilder sb = new StringBuilder(mot.Length);
                sb.Append(mot.Substring(0, pos + 1));
                sb.Append("er");
                toReturn = verbesTer.Contains(sb.ToString());
            }
            return toReturn;
        }

        /// <summary>
        /// Identifie si le 'x' final de <paramref name="mot"/> se pronoce.
        /// </summary>
        /// <remarks>Les accents ne sont pa prise en compte.</remarks>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position de la lettre x.</param>
        /// <returns><c>true</c> si <paramref name="pos"/> pointe sur un x final qui se prononce.</returns>
        public static bool Regle_X_Final(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_X_Final - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert(mot[pos] == 'x');

            bool toReturn = false;
            if (pos == mot.Length - 1)
                toReturn = motsX.Contains(ChaineSansAccents(mot));
            return toReturn;
        }

        /// <summary>
        /// Checrhe si le 'ch' identifié par <paramref name="pos"/> dans <paramref name="mot"/> se 
        /// prononce [k]. 
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'c' de 'ch'.</param>
        /// <returns><c>true</c> si <paramref name="pos"/> pointe sur une combinaison 'ch' qui se
        /// prononce [k].</returns>
        public static bool Regle_ChK(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_ChK - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);

            bool toReturn = false;
            if (pos < mot.Length - 1
                && mot[pos] == 'c'
                && mot[pos + 1] == 'h')
                toReturn = motsChK.Contains(mot);
            return toReturn;
        }

        /// <summary>
        /// Retourne <c>true</c> si les lettres "un" à la position <paramref name="pos"/> dans
        /// <paramref name="mot"/> se prononcent [§]
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La poistion de 'un' dans <paramref name="mot"/>.</param>
        /// <returns><c>true</c> s'il s'agit bien de 'un' qui se pronoce [§].</returns>
        public static bool Regle_MotsUN_ON(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "Regle_MotsUN_ON - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            return
                pos < mot.Length - 1 && mot[pos] == 'u' && mot[pos + 1] == 'n'
                && motsUNon.Contains(SansSFinal(mot));
        }

        /// <summary>
        /// Identifie si "qua" (ou "qui") dans <paramref name="mot"/> se prononce [qwa] (ou [qwi])
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'q' ou du 'u' de "qua" (ou "qui").</param>
        /// <returns><c>true</c> si "qu" se prononce [qw].</returns>
        public static bool RegleMotsQUkw(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "RegleMotsQUkw - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            return 
                ((pos < mot.Length - 2 && mot[pos] == 'q' && mot[pos + 1] == 'u'
                && (mot[pos + 2] == 'a' || mot[pos + 2] == 'i')) 
                || 
                (pos > 0 && pos < mot.Length - 1 && mot[pos - 1] == 'q' && mot[pos] == 'u') 
                && (mot[pos + 1] == 'a' || mot[pos + 1] == 'i'))
                &&
                motsQUkw.Contains(mot);
        }

        /// <summary>
        /// Identifie si "en" se prononce [5] dans <paramref name="mot"/>.
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'e' de "en" dans le mot.</param>
        /// <returns><c>true</c> si <paramref name="pos"/> pointe bien sur "en" et que ce 
        /// "en" se pronoce [5] d'après la lsite de cas.</returns>
        public static bool RegleMotsEn5(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "RegleMotsEn5 - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert(mot[pos] == 'e');
            return
                (pos < mot.Length - 1 && mot[pos + 1] == 'n' && motsEn5.Contains(mot));
        }

        private static Regex rxGnGN = new Regex(
            @"(gnos|^agnat|^cogn(at|it)|gnath|^gneiss|^gnou(s?)$|^ign(e|é|if|iti|ivo)|^(inex?)pugna|^magn(a|u)|gnom(o|e|i[^n]|a)|^récogni|^stagn|^wagn)",
            RegexOptions.Compiled);

        /// <summary>
        /// Identifie si les lettres "gn" dans le mot se prononcent [gn]
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'g' dans le mot.</param>
        /// <returns><c>true</c> si <paramref name="pos"/> pointe bien sur "gn" et que cela se 
        /// prononce [gn].</returns>
        public static bool RegleMotsGnGN(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "RegleMotsGnGN - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert(mot[pos] == 'g');

            // Essayons une approche un peu différente vu qu'il s'agit vraiment de familles de mot.
            // L'utiisation de Regex n'est pas plus rapide, mais elle est plus tolérante aux fautes
            // d'orthographe.

            return
                (pos < mot.Length - 1 && mot[pos + 1] == 'n' && rxGnGN.IsMatch(mot));
        }

        /// <summary>
        /// Détermine si "oy" dans <paramref name="mot"/> se prononce [oj].
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">La position du 'o' de "oy".</param>
        /// <returns><c>true</c> si <paramref name="pos"/> indique bien "oy" qui se prononce 
        /// [oj].</returns>
        public static bool RegleMotsOYoj(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "RegleMotsOYoj - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert(mot[pos] == 'o');
            return
                (pos < mot.Length - 1 && mot[pos + 1] == 'y' && motsOYoj.Contains(mot));
        }

        /// <summary>
        /// Détermine si le 'e' de "re" en début de mot se prononce [°]
        /// </summary>
        /// <param name="mot">Le mot à analyser.</param>
        /// <param name="pos">1 (la position du 'e' de "re" en début de mot)</param>
        /// <returns><c>true</c> si <paramref name="pos"/> est bien la position du 'e' de "re"
        /// en début de mot et qu'il se pprononce [°].</returns>
        public static bool RegleMotsRe(string mot, int pos)
        {
            logger.ConditionalTrace(ConfigBase.cultF, "RegleMotsRe - mot: \'{0}\', pos: {1}", mot, pos);
            Debug.Assert(mot != null);
            Debug.Assert(mot[pos] == 'e');

            bool toReturn = false;
            if (pos == 1 && mot[0] == 'r')
            {
                string pref6;
                if (mot.Length <= 6)
                {
                    pref6 = mot;
                }
                else
                {
                    pref6 = mot.Substring(0, 6);
                }
                return (!motsRe6.Contains(pref6));
            }
            return false;
        }

        /// <summary>
        /// Liste des mots contenant 'un' où 'un' se prononce [§]
        /// </summary>
        private static HashSet<string> motsUNon = new HashSet<string>
        {
            "acupuncteur", "acupuncteurs", "acupuncture", "acupunctures", "avunculaire", "avunculairement",
            "avunculaires", "avunculat", "avunculats", "bécabunga", "bécabungas", "carborundum",
            "carborundums", "compound", "conjungo", "conjungos", "contrapuntique", "contrapuntiques",
            "contrapuntiste", "contrapuntistes", "fungicide", "homuncule", "homuncules", "infundibulum",
            "infundibulums", "latifundium", "latifundiums", "negundo", "négundo", "negundos",
            "négundos", "nuncupatif", "nuncupatifs", "nuncupation", "nuncupations", "nuncupative",
            "nuncupatives", "nundinal", "nundinale", "nundinales", "nundinaux", "nundines", "opuntia",
            "opuntias", "pacfung", "pacfungs", "punctiforme", "punctiformes", "punctum", "punctums",
            "puntarelle", "puntarelles", "secundo", "skungs", "skunks", "unciforme", "unciformes",
            "uncinaire", "unciné", "uncinée", "uncinées", "uncinés", "uncinule", "uncipenne",
            "uncirostre", "undécennal", "undécennale", "undécennales", "undécennaux", "unguéal",
            "unguéale", "unguéales", "unguéaux", "unguifère", "unguifères", "unguis", "vasopuncture",
            "vérécundie",
        };

        /// <summary>
        /// Liste des verbes en ier. Avec accents.
        /// </summary>
        private static HashSet<string> verbes_ier = new HashSet<string>
        {
            "abrier", "abêtifier", "académifier", "académisier", "acidifier", "acétifier", "affier", "affilier",
            "agatifier", "allier", "amnistier", "amodier", "amplier", "amplifier", "anesthésier", "anémier",
            "apostasier", "apparier", "approprier", "apprécier", "armorier", "artificier", "asphyxier", "associer",
            "atrophier", "aurifier", "authentifier", "autographier", "autopsier", "avarier", "balbutier", "barbifier",
            "baronifier", "biographier", "bistourier", "bonifier", "bougier", "béatifier", "bénéficier", "bêtifier",
            "calcifier", "calligraphier", "calomnier", "certifier", "charabier", "charrier", "chier", "chirographier",
            "chosifier", "choséifier", "châtier", "circonstancier", "clarifier", "classifier", "cocufier", "codifier",
            "colorier", "communier", "complexifier", "conchier", "concilier", "confier", "congédier", "contrarier",
            "-convier", "copier", "corporifier", "crier", "crucifier", "dactylographier", "densifier", "différencier",
            "différentier", "dignifier", "disgracier", "disqualifier", "dissocier", "diversifier", "domicilier",
            "domifier", "dulcifier", "décalcifier", "déclassifier", "décrier", "dédier", "dédifférencier", "défier",
            "déifier", "délier", "démarier", "démultiplier", "démystifier", "démythifier", "dénier", "déparier",
            "dépatrier", "déplier", "déprécier", "désapproprier", "dévier", "dévitrifier", "ectasier", "enlier",
            "envier", "escarrifier", "escoffier", "estropier", "estérifier", "excommunier", "excorier", "exemplifier",
            "exfolier", "expatrier", "expier", "exproprier", "expédier", "extasier", "falsifier", "fantasier",
            "fier", "fluidifier", "fortifier", "frigorifier", "fructifier", "gambier", "gazéifier", "glorifier",
            "gracier", "gratifier", "gélifier", "géographier", "harmonier", "historier", "horrifier", "humidifier",
            "humilier", "hyperplasier", "hypertrophier", "hypostasier", "identifier", "idiotifier", "incendier",
            "ingénier", "initier", "injurier", "intensifier", "interfolier", "inventorier", "irradier", "justicier",
            "justifier", "jérémier", "lapidifier", "licencier", "lier", "lignifier", "liquéfier", "lithographier",
            "lubrifier", "lénifier", "madéfier", "magnifier", "maléficier", "manier", "marier", "massifier",
            "mellifier", "mendier", "modifier", "mollifier", "momifier", "mondifier", "monographier", "mortifier",
            "multiplier", "mystifier", "mythifier", "méfier", "mésallier", "nazifier", "nidifier", "nier", "nitrifier",
            "notifier", "nullifier", "négocier", "obvier", "octavier", "officier", "opacifier", "orthographier",
            "ossifier", "oublier", "pacifier", "palifier", "palinodier", "pallier", "panifier", "parier", "parodier",
            "passéifier", "personnifier", "photocopier", "photographier", "pilorier", "plagier", "planchéier",
            "planifier", "plastifier", "plier", "polycopier", "pontifier", "prier", "privilégier", "prosodier",
            "préjudicier", "présentifier", "psalmodier", "publier", "purifier", "putréfier", "pépier", "pétrifier",
            "qualifier", "quantifier", "quintessencier", "radier", "radiographier", "rallier", "ramifier", "rapatrier",
            "rapparier", "rapproprier", "raréfier", "rassasier", "ratifier", "razzier", "recalcifier", "recopier",
            "rectifier", "relier", "remanier", "remarier", "remercier", "remplier", "remédier", "renier", "renvier",
            "replier", "reprographier", "revivifier", "rigidifier", "rubéfier", "russifier", "réconcilier",
            "récrier", "réexpédier", "réfugier", "réifier", "répertorier", "répudier", "résilier", "résinifier",
            "réunifier", "réédifier", "saccharifier", "sacrifier", "salarier", "salifier", "sanctifier", "sanguifier",
            "saponifier", "scarifier", "schistifier", "scier", "scorifier", "sentencier", "signifier", "simplifier",
            "skier", "solacier", "solfier", "solidifier", "soucier", "spolier", "spécifier", "statufier", "stipendier",
            "stratifier", "strier", "stupéfier", "sténographier", "substantifier", "supplicier", "supplier",
            "surmultiplier", "sérier", "tarifier", "terrifier", "tonifier", "torréfier", "transsubstantier",
            "trier", "tuméfier", "tunisifier", "typifier", "télécopier", "télégraphier", "unifier", "varier",
            "versifier", "vicarier", "vicier", "vinifier", "vitrifier", "vivifier", "vérifier", "écrier", "édifier",
            "électrifier", "émacier", "émier", "émulsifier", "épier", "éthérifier", "étudier", "ambroisier",
            "carier", "défolier", "dragéifier", "gâtifier", "hyperesthésier", "iconographier", "immensifier",
            "indulgencier", "jazzifier", "lividifier", "lubréfier", "luxurier", "lyrifier", "maladier", "mélancholier",
            "mélancolier", "mélancolifier", "moruefier", "multigraphier", "nanifier", "négrifier", "nettifier",
            "noblifier", "odifier", "oedématier", "oursifier", "pécufier", "perlifier", "perruquifier", "phonographier",
            "plasmifier", "plénifier", "prussifier", "réétudier", "réhumidifier", "renégocier", "réoublier",
            "reprier", "republier", "revérifier", "rhodier", "sanifier", "savantifier", "sclérifier", "silencier",
            "starifier", "sublimifier", "sulpicier", "surlier", "syllabifier", "tartufier", "théorifier", "transsubstantifier",
            "typographier", "alcoolifier", "aluminier", "ammonifier", "analgésier", "angarier", "autenticier",
            "autocopier", "autodéprécier", "autojustifier", "autolubrifier", "automodifier", "autoradiographier",
            "autovérifier", "basifier", "bibliographier", "brier", "cadmier", "calorifier", "capier", "caprifier",
            "carnifier", "cartographier", "caséifier", "caustifier", "chalcographier", "chondrifier", "chorégraphier",
            "chylifier", "chymifier", "cinégraphier", "cinématographier", "coassocier", "cokéfier", "compactifier",
            "consonantifier", "consonifier", "copublier", "cosmifier", "covarier", "cryptographier", "cérifier",
            "diazocopier", "distancier", "déclergifier", "décodifier", "décrucifier", "dédensifier", "dédifférencier",
            "défortifier", "délignifier", "démassifier", "dénazifier", "dénitrifier", "dépatrier", "déplanifier",
            "déprier", "dépétrifier", "déqualifier", "dérelier", "dérussifier", "désacidifier", "désaffilier",
            "désallier", "désapparier", "désassocier", "désertifier", "désessencier", "déshumidifier", "désignifier",
            "désilicier", "désilicifier", "désirradier", "désorthographier", "désunifier", "désémulsifier",
            "détoxifier", "dézincifier", "effigier", "entremarier", "escarifier", "escarrifier", "escofier",
            "euthanasier", "eutrophier", "faséier", "frigidifier", "gabarier", "gleyifier", "graphier", "grésifier",
            "holographier", "homogénéifier", "houillifier", "humifier", "hyperhémier", "hyperplasier", "hyperémier",
            "hypolipidémier", "indifférencier", "ingénier", "instancier", "ischémier", "karstifier", "lamifier",
            "latensifier", "lichénifier", "lignifier", "lithifier", "lithotypographier", "lixivier", "macrophotographier",
            "magnésier", "matifier", "microcopier", "microphotographier", "miméographier", "moinifier", "muséifier",
            "méfier", "mésédifier", "neutrographier", "notarier", "olographier", "organifier", "ortier", "pantographier",
            "peptonifier", "phototélécopier", "polyestérifier", "polygraphier", "potentier", "préplastifier",
            "prépublier", "présanctifier", "préétudier", "quartzifier", "radioscopier", "radiotélégraphier",
            "rapprécier", "rebénéficier", "recertifier", "rechâtier", "reclassifier", "recodifier", "recolorier",
            "reconfier", "recongédier", "recontrarier", "recrier", "redactylographier", "redifférencier", "redomicilier",
            "redécalcifier", "redédier", "redéfier", "redéplier", "refortifier", "regazéifier", "reglorifier",
            "relicencier", "relubrifier", "remodifier", "rengracier", "renotifier", "repacifier", "reparier",
            "rephotocopier", "rephotographier", "replanifier", "replastifier", "requalifier", "resacrifier",
            "resignifier", "resupplier", "retrier", "retélégraphier", "réaffilier", "réapparier", "réapproprier",
            "réassocier", "récrier", "réestérifier", "référencier", "réorthographier", "rétifier", "rétrodévier",
            "scénographier", "silicier", "silicifier", "sismographier", "solmifier", "spathifier", "substantier",
            "subérifier", "sudorifier", "surqualifier", "sursignifier", "surédifier", "sélénier", "tanguier",
            "taudifier", "thermoanesthésier", "transestérifier", "tubérifier", "turquifier", "urbanifier", "xylographier",
            "xérocopier", "xérographier", "échographier", "écrier", "élier", "élutrier", "épigraphier", "épitaxier",
            "époutier", "équarrier", "étanchéifier", "étatifier", "substancier", "subsidier", "sérigraphier",
            "saccarifier", "ambifier", "chromolithographier", "compacifier", "décertifier", "décomplexifier",
            "déplastifier", "dérigidifier", "désidentifier", "glacifier", "grâcier", "hyperqualifier", "interrelier",
            "piétonnifier", "recrucifier", "redémultiplier", "réapprécier", "sectifier", "téléfalsifier", "zombifier",
            // sans accents
            "abetifier", "academifier", "academisier", "acetifier", "anesthesier", "anemier",
            "apprecier", "beatifier", "beneficier", "betifier", "choseifier", "chatier", "congedier",
            "differencier", "differentier", "decalcifier", "declassifier", "decrier", "dedier",
            "dedifferencier", "defier", "deifier", "delier", "demarier", "demultiplier", "demystifier",
            "demythifier", "denier", "deparier", "depatrier", "deplier", "deprecier", "desapproprier",
            "-devier", "devitrifier", "esterifier", "expedier", "gazeifier", "gelifier", "geographier",
            "ingenier", "jeremier", "liquefier", "lenifier", "madefier", "maleficier", "mefier",
            "mesallier", "negocier", "passeifier", "plancheier", "privilegier", "prejudicier",
            "presentifier", "putrefier", "pepier", "petrifier", "rarefier", "remedier", "rubefier",
            "reconcilier", "reexpedier", "refugier", "reifier", "repertorier", "repudier", "resilier",
            "resinifier", "reunifier", "reedifier", "specifier", "stupefier", "stenographier",
            "serier", "torrefier", "tumefier", "telecopier", "telegraphier", "verifier", "ecrier",
            "edifier", "electrifier", "emacier", "emier", "emulsifier", "epier", "etherifier",
            "etudier", "defolier", "drageifier", "gatifier", "hyperesthesier", "lubrefier", "melancholier",
            "melancolier", "melancolifier", "negrifier", "oedematier", "pecufier", "plenifier",
            "reetudier", "rehumidifier", "renegocier", "reoublier", "reverifier", "sclerifier",
            "theorifier", "analgesier", "autodeprecier", "autoverifier", "caseifier", "choregraphier",
            "cinegraphier", "cinematographier", "cokefier", "cerifier", "declergifier", "decodifier",
            "decrucifier", "dedensifier", "defortifier", "delignifier", "demassifier", "denazifier",
            "denitrifier", "deplanifier", "deprier", "depetrifier", "dequalifier", "derelier",
            "derussifier", "desacidifier", "desaffilier", "desallier", "desapparier", "desassocier",
            "desertifier", "desessencier", "deshumidifier", "designifier", "desilicier", "desilicifier",
            "desirradier", "desorthographier", "desunifier", "desemulsifier", "detoxifier", "dezincifier",
            "faseier", "gresifier", "homogeneifier", "hyperhemier", "hyperemier", "hypolipidemier",
            "indifferencier", "ischemier", "lichenifier", "magnesier", "mimeographier", "museifier",
            "mesedifier", "phototelecopier", "polyesterifier", "preplastifier", "prepublier",
            "presanctifier", "preetudier", "radiotelegraphier", "rapprecier", "rebeneficier",
            "rechatier", "recongedier", "redifferencier", "redecalcifier", "rededier", "redefier",
            "redeplier", "regazeifier", "retelegraphier", "reaffilier", "reapparier", "reapproprier",
            "reassocier", "reesterifier", "referencier", "reorthographier", "retifier", "retrodevier",
            "scenographier", "suberifier", "suredifier", "selenier", "thermoanesthesier", "transesterifier",
            "tuberifier", "xerocopier", "xerographier", "echographier", "elier", "elutrier", "epigraphier",
            "epitaxier", "epoutier", "equarrier", "etancheifier", "etatifier", "serigraphier",
            "decertifier", "decomplexifier", "deplastifier", "derigidifier", "desidentifier",
            "pietonnifier", "redemultiplier", "reapprecier", "telefalsifier"
        };

        /// <summary>
        /// Liste des mots en "um" se prononçant [Om]. (sans le 's' du pluriel)
        /// </summary>
        private static HashSet<string> motsUM = new HashSet<string>
        {
            "abrotanum", "acanthophyllum", "acérathérium", "acérothérium", "acétabulum", "acrotérium", "actinium",
            "adénoépithélium", "adiantum", "adytum", "aérium", "ageratum", "album", "allopalladium", "aluminium",
            "alyssum", "américium", "ammonium", "ancylothérium", "anoplothérium", "aquarium", "arboretum", "arum",
            "atrium", "auditorium", "barathrum", "barnum", "baryum", "bégum", "béryllium", "bibendum", "blastophyllium",
            "cadmium", "caecum", "caffardum", "calcanéum", "calcium", "caldarium", "cambium","capsicum", "carborundum",
            "castoréum", "cérium", "césium", "coagulum", "cœcum", "colostrum", "columbarium", "compendium",
            "componium", "condominium", "consortium", "continuum", "coronium", "critérium", "cryptosepalum",
            "cuprammonium", "cuprosilicium", "curriculum", "cymbalum", "cypripedium", "décorum", "delirium",
            "delphinium", "dentalium", "deutérium", "dextrorsum", "diachylum", "diascordium", "dictum", "didymium",
            "diluvium", "dinotherium", "dinothérium", "duodénum", "électrum", "emporium", "endothélium", "épithélium",
            "equisetum", "équisetum", "érodium", "factotum", "factum", "fanum", "fatum", "fermium", "flabellum",
            "flamméum", "forum", "francium", "frigidarium", "gadolinium", "galbanum", "galium", "gallium", "garum",
            "géranium", "germanium", "gérontocomium", "glucinium", "hafnium", "harmonium", "hedysarum", "hédysarum",
            "hélium", "hyménium", "hypericum", "iléum", "illuvium", "imperium", "impluvium", "indium", "infundibulum",
            "iridium", "jéjunum", "labarum", "labium", "lactucarium", "ladanum", "latifundium", "laudanum",
            "léontopodium", "leucanthémum", "lilium", "linoléum", "lithium", "lutécium", "magnésium", "magnum",
            "martyrium", "marum", "maximum", "méconium", "médium", "mégathérium", "memorandum", "mémorandum",
            "mendélévium", "mésorectum", "millénium", "minimum", "minium", "molluscum", "muséum", "mycélium",
            "natrium", "natrum", "nébulium", "neptunium", "niobium", "nobélium", "oïdium", "oleum", "omnium",
            "opium", "opossum", "oppidum", "optimum", "organum", "osmium", "paléocérébellum", "paléothérium",
            "palladium", "pallidum", "pallium", "palmarium", "pandémonium", "parabellum", "pedum", "pélargonium",
            "pénicillium", "pensum", "péplum", "perfectum", "pétrolatum", "phormium", "phylum", "physospermum",
            "pilum", "pittosporum", "planétarium", "plasmodium", "platiniridium", "plenum", "plénum", "plutonium",
            "podium", "podophyllum", "polonium", "polygonum", "pomoerium", "populéum", "postulatum", "potassium",
            "praesidium", "préférendum", "presbyterium", "présidium", "préventorium", "proscenium", "psyllium",
            "pterophyllum", "punctum", "pycnogonum", "quadrivium", "quantum", "quorum", "radioaluminium", "radiosilicium",
            "radiosodium", "radium", "rectum", "referendum", "référendum", "reticulatum", "réticulatum", "reticulum",
            "réticulum", "rhénium", "rhinosporidium", "rhodium", "rhum", "rosarium", "rubidium", "ruthénium",
            "sacrum", "samarium", "sanatorium", "sanitarium", "santalum", "scandium", "scriptorium", "scrotum",
            "sébum", "sedum", "sélénium", "sempervivum", "sensorium", "septum", "sérapeum", "serapéum", "sérum",
            "silicium", "silicocalcium", "silphium", "simultaneum", "sium", "sodium", "solanum", "solarium",
            "speculum", "spéculum", "sphagnum", "sphénophyllum", "sternum", "stomodéum", "stramonium", "strontium",
            "sudatorium", "sulfonium", "summum", "symposium", "tactum", "targum", "technécium", "technétium",
            "tépidarium", "terbium", "terebellum", "térebellum", "terébellum", "thallium", "thermopréférendum",
            "thorium", "thulium", "trichodesmium", "triclinium", "triduum", "triforium", "tritérium", "tritium",
            "trivium", "ultimatum", "unicum", "uranium", "vacuum", "vanadium", "vasothélium", "velarium", "vélarium",
            "velum", "vélum", "verumontanum", "vérumontanum", "vexillum", "viburnum", "vivarium", "xiphisternum",
            "ytterbium", "yttrium", "zirconium", "zygantrum", "zygopetalum", "zygophyllum", "zythum", "ageratum",

            // sans accents
            "aceratherium", "acerotherium", "acetabulum", "acroterium", "adenoepithelium", "aerium",
            "americium", "ancylotherium", "anoplotherium", "begum", "beryllium", "calcaneum", "capharnaum",
            "castoreum", "cerium", "cesium", "criterium", "decorum", "deuterium", "duodenum",
            "electrum", "endothelium", "epithelium", "erodium", "flammeum", "geranium", "gerontocomium",
            "helium", "hymenium", "ileum", "jejunum", "leontopodium", "leucanthemum", "linoleum",
            "lutecium", "magnesium", "meconium", "medium", "megatherium", "mendelevium", "mesorectum",
            "millenium", "museum", "mycelium", "nebulium", "nobelium", "oidium", "paleocerebellum",
            "paleotherium", "pandemonium", "pelargonium", "penicillium", "peplum", "petrolatum",
            "planetarium", "populeum", "preferendum", "presidium", "preventorium", "rhenium",
            "ruthenium", "sebum", "selenium", "serapeum", "serapeum", "serum", "sphenophyllum",
            "stomodeum", "technecium", "technetium", "tepidarium", "thermopreferendum", "triterium",
            "vasothelium"
        };

        /// <summary>
        /// Liste des verbes en "ter", plus une forme en ter des verbes d'autres groupes qui se 
        /// conjuguent en 'tions' ou 'tiez' comme démentir qui est présent dans la liste 
        /// comme 'démenter'... 
        /// </summary>
        private static HashSet<string> verbesTer = new HashSet<string>
        {
            "aboter", "abouter", "abricoter", "abriter", "absorbanter", "abuter", "accepter", "accidenter",
            "accoiter",
            "acclimater", "accointer", "accoster", "accoter", "accravanter", "accréditer", "acheter", "acouter",
            "acquêter", "acravanter", "-acter", "adapter", "adenter", "admonester", "admonéter", "-adopter",
            "affaiter", "affaîter", "-affecter", "afforester", "affronter", "affruiter", "affréter", "affûter",
            "aganter", "agater", "agioter", "agiter", "agréanter", "agrémenter", "aheurter", "ahonter",
            "aiguilleter", "aimanter", "ajointer", "ajouter", "ajuster", "ajuter", "alerter", "alimenter",
            "aliter", "allaiter", "amateloter", "ameuter", "amicoter", "amignoter", "amignouter", "amputer",
            "annoter", "antidater", "antidoter", "aoûter", "apléter", "aposter", "apparenter", "appointer",
            "apponter", "apporter", "apprésenter", "apprêter", "appâter", "appéter", "arbreter", "argenter",
            "argoter", "argumenter", "arpenter", "arrenter", "arrêter", "aspecter", "asphalter", "assermenter",
            "assister", "assoter", "asticoter", "atinter", "-attenter", "attester", "attrister", "augmenter",
            "ausculter", "avorter", "azimuter", "bachoter", "bagoter", "bahuter", "baisoter", "ballaster",
            "banqueter", "baqueter", "barboter", "baster", "bavoter", "becqueter", "becter", "beloter",
            "bibeloter", "bijouter", "biscoter", "biscuiter", "biseauter", "bizuter", "blaireauter", "bleuter",
            "bluter", "boiter", "bonimenter", "bonneter", "bouilloter", "bouqueter", "boursicoter", "bouter",
            "bouveter", "branloter", "breveter", "brillanter", "brilloter", "briqueter", "brocanter", "brocheter",
            "brouter", "bruiter", "budgéter", "buter", "buvoter", "bâter", "bécoter", "bégueter", "béqueter",
            "bêcheveter", "caboter", "cacheter", "cadeauter", "cadoter", "cafeter", "cahoter", "cailleboter",
            "cailleter", "caillouter", "caleter", "calfater", "calter", "cameloter", "canoter", "canter",
            "caoutchouter", "capoter", "capter", "caqueter", "carapater", "-carbonater", "casemater", "catapulter",
            "cataracter", "causoter", "chahuter", "chanter", "chapeauter", "charabiater", "charcuter",
            "charpenter", "charreter", "chevreauter", "chevreter", "chevroter", "chicoter", "chipoter",
            "chiqueter", "chouchouter", "chuchoter", "chuinter", "chuter", "cimenter", "citer", "claboter",
            "clapoter", "claqueter", "claveter", "clignoter", "cliqueter", "clocheter", "clouter", "coapter",
            "cocoter", "coexister", "cogiter", "cohabiter", "-collecter", "colleter", "colmater", "colporter",
            "commanditer", "commenter", "commuter", "compacter", "compartimenter", "complanter", "complimenter",
            "comploter", "complémenter", "compléter", "comporter", "composter", "compter", "computer",
            "compéter", "concerter", "concocter", "concréter", "condimenter", "conforter", "confronter",
            "connecter", "connoter", "conquêter", "consister", "constater", "conster", "consulter", "contacter",
            "contenter", "conter", "contester", "contingenter", "-contracter", "contraster", "contrebuter",
            "contreventer", "contrister", "convoiter", "coopter", "copter", "coqueter", "corseter", "coter",
            "couchoter", "coupleter", "courtcircuiter", "coïter", "coûter", "crachoter", "cranter", "crapahuter",
            "crapouilloter", "craqueter", "cravater", "crocheter", "croûter", "créditer", "créosoter",
            "crépiter", "crêter", "culbuter", "cureter", "cémenter", "dansoter", "dater", "diamanter",
            "-dicter", "-diffracter", "dilater", "diligenter", "discréditer", "discuter", "disputer", "disserter",
            "documenter", "doigter", "dompter", "dorloter", "doter", "douter", "duiter", "duveter", "dynamiter",
            "débecqueter", "débequeter", "débiliter", "débiter", "débouter", "déboyauter", "déboîter",
            "débuter", "débâter", "décacheter", "décanter", "décapiter", "décapoter", "-décarbonater", "déchanter",
            "déchiqueter", "déclaveter", "décliqueter", "décolleter", "décompléter", "décompter", "déconcerter",
            "déconforter", "déconnecter", "-décontracter", "décroûter", "décrypter", "décréditer", "décrépiter",
            "décréter", "défunter", "déganter", "dégoter", "dégoûter", "déguster", "déjeter", "délecter",
            "délester", "délimiter", "déliter", "déluter", "démailloter", "démonter", "démoucheter", "démâter",
            "démériter", "dénoter", "dénoyauter", "dépaqueter", "dépiauter", "dépister", "dépiter", "déplanter",
            "dépointer", "déporter", "dépoter", "députer", "dérater", "dérouter", "désacclimater", "désadapter",
            "-désaffecter", "désaimanter", "désajuster", "désappointer", "désargenter", "désenchanter",
            "-déserter", "déshabiter", "déshydrater", "déshériter", "désincruster", "-désinfecter", "désorbiter",
            "désorienter", "-détecter", "détester", "-détracter", "dévaster", "dévelouter", "dévolter", "effriter",
            "effruiter", "emberlificoter", "embouter", "emboîter", "embâter", "embêter", "emmailloter",
            "empaqueter", "empester", "empiéter", "emporter", "empoter", "emprunter", "empâter", "encarter",
            "enchanter", "encravater", "encroter", "encroûter", "endenter", "enfanter", "enfaîter", "enfûter",
            "enkyster", "enquêter", "enrégimenter", "ensanglanter", "enter", "entêter", "envoûter", "ergoter",
            "escamoter", "escompter", "escorter", "esquinter", "essarter", "ester", "exalter", "-excepter",
            "exciter", "excogiter", "excrémenter", "excréter", "-exempter", "exhorter", "exister", "expliciter",
            "exploiter", "exporter", "expérimenter", "exulter", "exécuter", "faciliter", "fagoter", "fainéanter",
            "fauter", "feinter", "fermenter", "feuilleter", "fienter", "fileter", "filouter", "flibuster",
            "flirter", "flûter", "folioter", "fomenter", "forjeter", "fragmenter", "frelater", "fricoter",
            "frisoter", "froufrouter", "fréquenter", "fréter", "fureter", "féliciter", "fêter", "ganter",
            "gigoter", "glavioter", "glouglouter", "gobeloter", "gobeter", "goûter", "graniter", "graviter",
            "grignoter", "guillemeter", "gâter", "gîter", "habiliter", "habiter", "haleter", "hanter",
            "haricoter", "heurter", "hoqueter", "humecter", "hydrater", "hâter", "hébéter", "hériter",
            "hésiter", "illimiter", "illuter", "imiter", "impatienter", "implanter", "importer", "imputer",
            "incanter", "incidenter", "inciter", "incruster", "-infecter", "infester", "ingurgiter", "-injecter",
            "innocenter", "inquiéter", "insister", "-inspecter", "instrumenter", "insulter", "insupporter",
            "-intenter", "-intercepter", "interjeter", "interpréter", "-intersecter", "-introspecter", "-inventer",
            "inviter", "irriter", "jaboter", "jacter", "jarreter", "jeter", "jointer", "jouter", "jouxter",
            "juter", "knouter", "lamenter", "lester", "lichoter", "liciter", "lifter", "ligoter", "limiter",
            "linéamenter", "lister", "liter", "loqueter", "louveter", "luter", "léviter", "machicoter",
            "mailleter", "mailloter", "malter", "maltraiter", "mandater", "mangeoter", "manifester", "maquereauter",
            "margoter", "marmiter", "marqueter", "massicoter", "mater", "mazouter", "mendigoter", "mignoter",
            "mijoter", "militer", "minuter", "miroiter", "miter", "molester", "moleter", "monter", "moucheter",
            "moufter", "mouvementer", "mugueter", "muter", "mâter", "mécontenter", "médicamenter", "méditer",
            "mégoter", "mériter", "mésinterpréter", "nageoter", "neigeoter", "nitrater", "-noter", "noyauter",
            "numéroter", "nécessiter", "-objecter", "occulter", "-opter", "orbiter", "orienter", "ornementer",
            "ouater", "pagnoter", "pailleter", "palpiter", "panneauter", "papilloter", "papoter", "paqueter",
            "parachuter", "parasiter", "parementer", "parlementer", "parloter", "parqueter", "passementer",
            "patenter", "patienter", "patricoter", "pelleter", "peloter", "percuter", "permuter", "pernocter",
            "persister", "-persécuter", "pester", "phagocyter", "phosphater", "pianoter", "picoter", "picter",
            "pieuter", "pigmenter", "piloter", "pimenter", "pinceauter", "pinter", "piqueter", "pirater",
            "pissoter", "pister", "pivoter", "piéter", "placoter", "plaisanter", "planter", "pleuvoter",
            "plébisciter", "pocheter", "pointer", "poireauter", "poiroter", "ponter", "-porter", "postdater",
            "poster", "profiter", "projeter", "-prospecter", "protester", "précipiter", "précompter", "préexister",
            "préméditer", "présenter", "prétexter", "prêter", "pâter", "pécloter", "péricliter", "péter",
            "quarter", "queuter", "quêter", "rabioter", "raboter", "rabouter", "racheter", "raconter",
            "radoter", "ragoter", "ragoûter", "rajouter", "rajuster", "rameuter", "rapioter", "rapiéceter",
            "rapporter", "rassoter", "-rater", "ravigoter", "rebecter", "rebouter", "rebuter", "rechanter",
            "rechuter", "recompter", "recruter", "redouter", "refléter", "rejeter", "-relater", "reloqueter",
            "remboîter", "remmailloter", "remonter", "rempiéter", "remporter", "rempoter", "renfaîter",
            "renter", "replanter", "reporter", "représenter", "requêter", "respecter", "ressauter", "ressusciter",
            "rester", "retraiter", "rewriter", "riboter", "rioter", "riposter", "riveter", "ronfloter",
            "ronéoter", "roter", "rouspéter", "router", "réadapter", "réajuster", "réciter", "récolter",
            "réconforter", "réescompter", "réexporter", "-réfracter", "réfuter", "régater", "régenter",
            "réglementer", "régurgiter", "réhabiliter", "réhydrater", "réimplanter", "réimporter", "-réinfecter",
            "réinventer", "réorienter", "répercuter", "réputer", "répéter", "résister", "résulter", "-rétracter",
            "révolter", "-rééditer", "saboter", "sangloter", "sarter", "sauter", "saveter", "scruter", "sculpter",
            "secréter", "segmenter", "serpenter", "shooter", "shunter", "siester", "siffloter", "siroter",
            "solliciter", "soubresauter", "soucheter", "souffleter", "souhaiter", "souter", "sprinter",
            "subsister", "suinter", "sulfater", "sulfiter", "supplanter", "supplémenter", "supporter",
            "supputer", "suppéditer", "surajouter", "suralimenter", "surexciter", "surexploiter", "surjeter",
            "surmonter", "sursauter", "survolter", "susciter", "suspecter", "sustenter", "suçoter", "sécréter",
            "sédimenter", "-sélecter", "tacheter", "taluter", "tangoter", "tapoter", "tarabiscoter", "tarabuster",
            "teinter", "tempêter", "tenter", "terreauter", "tester", "tinter", "tiqueter", "toaster", "tourmenter",
            "tournicoter", "toussoter", "-tracter", "traficoter", "traiter", "transbahuter", "transiter",
            "-translater", "transmuter", "transplanter", "transporter", "travailloter", "trembloter", "tressauter",
            "tricoter", "tripoter", "trompeter", "truster", "trémater", "turluter", "tuyauter", "tâter",
            "téter", "valeter", "vanter", "velouter", "velter", "venter", "vergeter", "verjuter", "violenter",
            "virevolter", "visiter", "-vivisecter", "vivoter", "voleter", "volter", "voluter", "voter",
            "voûter", "végéter", "warranter", "zester", "zieuter", "zozoter", "zyeuter", "ébouillanter",
            "ébouter", "ébruiter", "écarter", "éclater", "écoqueter", "écourter", "écouter", "écroûter",
            "écrêter", "édenter", "édicter", "-éditer", "-éjecter", "électrocuter", "éliciter", "émoucheter",
            "épater", "épinceter", "épointer", "épousseter", "épouvanter", "équeuter", "éreinter", "éructer",
            "étiqueter", "étêter", "éventer", "éviter", "ôter", "alinéater", "arc-bouter", "chevrèter",
            "co-adapter", "contre-bouter", "contre-pointer", "court-circuiter", "débecter", "désencroûter",
            "détricoter", "dormoter", "ébruter", "emmazouter", "empianoter", "encorseter", "enredingoter",
            "fayoter", "fébriciter", "flânoter", "frégater", "fuiter", "fumoter", "funester", "gargoter",
            "gloussoter", "glouter", "graphiter", "grésilloter", "grognoter", "guéreter", "halter", "héliporter",
            "hirsuter", "humoter", "indulter", "insolenter", "-interlocuter", "-introjecter", "lancicoter",
            "lavementer", "léchoter", "-législater", "lingoter", "lock-outer", "méprisoter", "mithridater",
            "moineauter", "moufeter", "mouffeter", "muleter", "nuiter", "obiter", "onguenter", "opporter",
            "pédanter", "péréquater", "permanenter", "perscruter", "pilloter", "pistoleter", "plaçoter",
            "plaignoter", "plumeter", "popoter", "poussoter", "préadapter", "-préempter", "prester", "progéniter",
            "prouter", "pschuter", "quasi-contracter", "rabanter", "râloter", "raugmenter", "réacclimater",
            "réadopter", "réaffecter", "réaimanter", "réalimenter", "réappâter", "réapprêter", "réaugmenter",
            "recacheter", "recompléter", "rediscuter", "réécouter", "réenchanter", "refeuilleter", "regoûter",
            "réhabiter", "réhumecter", "réinjecter", "réinterpréter", "réinviter", "remâter", "rempaqueter",
            "reprêter", "re-sous-traiter", "revisiter", "rococoter", "roussoter", "rudenter", "saccageoter",
            "sarmenter", "sauveter", "savater", "silicater", "soixanter", "solvater", "souffloter", "sous-affréter",
            "sous-exploiter", "sous-traiter", "stelliter", "substanter", "surcoter", "-surinfecter", "surventer",
            "tableauter", "tabuster", "tacoter", "tangenter", "télétraiter", "testamenter", "touchoter",
            "trempoter", "twister", "ubiquiter", "varianter", "yoyoter", "-ablater", "absenter", "affriter",
            "alester", "alloter", "aloter", "anchoiter", "antiparasiter", "anuiter", "aponter", "appeauter",
            "apériter", "assarmenter", "asserter", "-auditer", "autoadapter", "autoalimenter", "autoamputer",
            "autociter", "autocommuter", "autocontester", "autodicter", "autodécontracter", "autoexciter",
            "autolimiter", "autopiloter", "autoporter", "autosubsister", "autotracter", "azoter", "aéroporter",
            "aérotransporter", "barroter", "baryter", "baréter", "bicarbonater", "bichromater", "billeter",
            "binoter", "biotraiter", "biqueter", "bisegmenter", "bisouter", "-bissecter", "biter", "bizouter",
            "blablater", "borater", "borosilicater", "bouleter", "boyauter", "briffeter", "brifter", "bruter",
            "buffeter", "cacaoter", "cafter", "calamiter", "cambuter", "candidater", "caneter", "canneter",
            "canqueter", "carter", "chaluter", "chapoter", "chariboter", "charioter", "chlorater", "chocolater",
            "chromater", "chucheter", "chélater", "circuiter", "clinquanter", "clocter", "coadapter", "cobalter",
            "coexploiter", "cohériter", "compoter", "confiter", "conjointer", "contrebouter", "contremanifester",
            "contrenquêter", "contrepointer", "copermuter", "coprésenter", "copyrighter", "corneter", "cotransfecter",
            "coupeter", "coéditer", "craboter", "crapaüter", "craquanter", "crypter", "crânoter", "cuiter",
            "curedenter", "cuter", "denter", "dessuinter", "diazoter", "disconnecter", "discounter", "-disjoncter",
            "dismuter", "dolenter", "-duplicater", "déafférenter", "déballaster", "débruter", "débudgéter",
            "débéqueter", "décalfater", "décaoutchouter", "déclimater", "décolmater", "décompacter", "décompartimenter",
            "déconnoter", "décoter", "décranter", "décravater", "décrémenter", "décuiter", "décuscuter",
            "-déflater", "défolioter", "déforester", "-déformater", "défruiter", "défuncter", "dégarroter",
            "dégraphiter", "déguillemeter", "dégurgiter", "dégîter", "déjanter", "déjointer", "déjouter",
            "délaiter", "délenter", "déleucocyter", "délicoter", "déligoter", "délinéamenter", "délister",
            "déléter", "démazouter", "dénitrater", "dépageoter", "dépagnoter", "dépailleter", "dépajoter",
            "dépanneauter", "dépapilloter", "déparasiter", "déparementer", "déparqueter", "déphosphater",
            "dépigmenter", "déposter", "déqueusoter", "dériveter", "dérocter", "déréglementer", "désabouter",
            "désabriter", "désadopter", "désaffronter", "désafférenter", "désagater", "désamianter", "désapparenter",
            "désasphalter", "désattrister", "désazoter", "déschister", "désemboîter", "désemmailloter",
            "désempaqueter", "désemprunter", "désencarter", "désencaster", "désencliqueter", "désenrégimenter",
            "désentêter", "désenvoûter", "désergoter", "désexciter", "déshabiliter", "désilicater", "désinviter",
            "désister", "désocculter", "désolvater", "désulfater", "désulfiter", "désétiqueter", "détriter",
            "déventer", "emboucauter", "embouffeter", "empapaouter", "empapilloter", "empeloter", "emplanter",
            "empointer", "empouter", "encaster", "enceinter", "encliqueter", "encolleter", "encorneter",
            "encrister", "endiamanter", "enganter", "enjanter", "ensaboter", "ensiloter", "entrevoûter",
            "esquimauter", "essarmenter", "essenter", "exorbiter", "explanter", "fabricoter", "faignanter",
            "farnienter", "farter", "fauberter", "feignanter", "ferrouter", "fleureter", "fluater", "folleter",
            "-formater", "frimater", "fruiter", "galeter", "galipoter", "genéter", "gileter", "graffiter",
            "greneter", "grenter", "gruauter", "gruter", "guniter", "horodater", "-hydrocuter", "hélitransporter",
            "impacter", "implémenter", "incrémenter", "indenter", "inexister", "inquarter", "interconnecter",
            "-interjecter", "introjeter", "intuiter", "jésuiter", "knockouter", "langueter", "lanter", "lenter",
            "levreter", "liquater", "lockouter", "maffioter", "mafioter", "microter", "mixter", "muloter",
            "museleter", "mâchoter", "mécompter", "méliniter", "niqueter", "nitriter", "noqueter", "nordester",
            "nordouester", "outer", "pageoter", "pajoter", "paleter", "paloter", "pancarter", "papiéter",
            "perchlorater", "photomonter", "picrater", "pinceter", "pinçoter", "piter", "pituiter", "planéter",
            "pleuroter", "pleuvioter", "plissoter", "plouter", "pluvioter", "poivroter", "polliciter",
            "-poter", "-protracter", "préacheter", "préciter", "précoter", "-prédater", "-préformater", "prémonter",
            "prétester", "prétraiter", "puter", "pédimenter", "rabiauter", "radiodétecter", "raffûter",
            "raineter", "rapapilloter", "rapiater", "rapprêter", "rebachoter", "rebaisoter", "rebarboter",
            "rebecqueter", "rebisouter", "rebizouter", "reboiter", "reboursicoter", "rebâter", "rebécoter",
            "rebéqueter", "recalfater", "recaoutchouter", "recapoter", "rechahuter", "recharcuter", "recharpenter",
            "recimenter", "reciter", "recollecter", "recolmater", "recolporter", "recommanditer", "recommenter",
            "recompartimenter", "recomplimenter", "recomploter", "reconfronter", "reconnecter", "reconstater",
            "reconsulter", "reconter", "recontacter", "recontester", "recontingenter", "recontracter",
            "recoqueter", "recoter", "recravater", "redater", "redicter", "redisjoncter", "redisputer",
            "redompter", "redorloter", "redoter", "redébiter", "redébouter", "redébuter", "redécacheter",
            "redécanter", "redécapoter", "redécompter", "redéconnecter", "redécrypter", "redécréter", "redégoter",
            "redélimiter", "redémonter", "redépaqueter", "redéporter", "redéserter", "redésister", "redétecter",
            "redévaster", "refarter", "reforester", "reformater", "refréquenter", "refureter", "reféliciter",
            "refêter", "reheurter", "rehériter", "relifter", "remaltraiter", "remandater", "remanifester",
            "remiliter", "reminuter", "remprunter", "renoter", "renuméroter", "repapilloter", "repaqueter",
            "reparqueter", "repercuter", "repiloter", "repirater", "replaisanter", "repleuvoter", "repointer",
            "reprofiter", "reprojeter", "repter", "repéter", "resaboter", "resauter", "reshooter", "resolliciter",
            "resulfater", "retenter", "retester", "retransiter", "retransplanter", "retransporter", "retricoter",
            "retâter", "reventer", "revoter", "rhabiter", "rouster", "réabouter", "réabriter", "réabsenter",
            "réaccepter", "réaccidenter", "réaccoster", "réadmonester", "réaffronter", "réaffréter", "réaffûter",
            "réalerter", "réallaiter", "réannoter", "réargenter", "réargumenter", "réarpenter", "réarrêter",
            "réasphalter", "réassister", "réemboîter", "réemmailloter", "réempaqueter", "réempiéter", "réemprunter",
            "réenquêter", "réenvoûter", "réescamoter", "réescorter", "réexalter", "réexhorter", "réexpliciter",
            "réexploiter", "réexpérimenter", "réexécuter", "réimputer", "réincruster", "réinfester", "réingurgiter",
            "réinsister", "réinspecter", "réintenter", "réintercepter", "réobjecter", "réécarter", "réécourter",
            "réédicter", "rééjecter", "réétiqueter", "saligoter", "scheloter", "schloter", "siloter", "simpleter",
            "souffroter", "souqueter", "sousqueter", "sporter", "suracheter", "suradapter", "surmédicamenter",
            "surreprésenter", "sursulfater", "sustanter", "tarter", "tilloter", "tilter", "tomater", "torchecuter",
            "toster", "transfecter", "trichoter", "tripleter", "trouilloter", "truiter", "tréjeter", "télédébiter",
            "-télédétecter", "télépancarter", "télépiloter", "télépointer", "téléporter", "usiter", "vigneter",
            "violeter", "virevouster", "volanter", "véroter", "youyouter", "zéroter", "ébouqueter", "ébûcheter",
            "échaloter", "écointer", "écolleter", "écroter", "écôter", "éjointer", "élaiter", "énoyauter",
            "épiéter", "épuiseter", "îloter", "knock-outer", "pied-au-cuter", "sous-alimenter", "menoter",
            "moqueter", "phagociter", "enchrister", "santer", "-stater", "center", "scripter", "balter",
            "clienter", "moiter", "couter", "giter", "koter", "balloter", "règlementer", "vouter", "gouter",
            "arcbouter", "surinterpréter", "comater", "tûter", "emboiter", "dégouter", "crouter", "encrypter",
            "affuter", "fluter", "défragmenter", "garroter", "charlater", "déboiter", "marabouter", "entreheurter",
            "greloter", "écrouter", "broadcaster", "aouter", "rameter", "soqueter", "entarter", "encrouter",
            "discompter", "flouter", "contre-buter", "trompéter", "enfaiter", "casse-croûter", "envouter",
            "entre-heurter", "casse-crouter", "anticommuter", "contre-manifester", "bouloter", "capahuter",
            "crapoter", "dérèglementer", "désenvouter", "guilleméter", "marcoter", "psychoter", "raffuter",
            "rapièceter", "rouloter", "réaffuter", "ragouter", "bichoter", "bla-blater", "blobloter", "borgnoter",
            "bèqueter", "caïmanter", "charrioter", "cliquoter", "cognoter", "discuputer", "débarboter",
            "débèqueter", "décrouter", "dégiter", "démarabouter", "désindenter", "emballoter", "enfuter",
            "entrevouter", "kaoter", "magoter", "motamoter", "nobscuriter", "raccuspoter", "rapipoter",
            "remboiter", "renfaiter", "rûter", "schmecter", "teseter", "télédicter", "anecdoter", "autocomplimenter",
            "autorecruter", "autoréglementer", "blaster", "choucrouter", "concomiter", "démater", "désinventer",
            "entreciter", "entruster", "exhalter", "hyperdilater", "osculter", "réapparenter", "réditer",
            "régimenter", "surassister", "surcommenter", "amisoter", "amusoter","apléter",
            "décompléter", "décréter", "démixter","décréter",

            // formes spéciales uniquement pour identifier la pronociations [t] de la conjugaison;
            // par exemple 'fouter' pour identifier que le t de foutions se prononce [t]. Il s'agit
            // typiquement de verbes d'un autre groupe.
            "contrefouter", "démenter", "fouter", "refouter", "repenter", "ressorter", "sorter", 
            "départer", "essarter", "parter", "quarter","reparter", "sarter",

            // sans accents
            "accrediter", "acqueter", "admoneter", "affreter", "agreanter", "agrementer", "apleter",
            "appresenter", "appreter", "appater", "appeter", "arreter", "budgeter", "bater", "becoter",
            "begueter", "bequeter", "becheveter", "complementer", "completer", "competer", "concreter",
            "conqueter", "coiter", "crediter", "creosoter", "crepiter", "creter", "cementer",
            "discrediter", "debecqueter", "debequeter", "debiliter", "debiter", "debouter", "deboyauter",
            "deboiter", "debuter", "debater", "decacheter", "decanter", "decapiter", "decapoter",
            "-decarbonater", "dechanter", "dechiqueter", "declaveter", "decliqueter", "decolleter",
            "decompleter", "decompter", "deconcerter", "deconforter", "deconnecter", "-decontracter",
            "decrouter", "decrypter", "decrediter", "decrepiter", "decreter", "defunter", "deganter",
            "degoter", "degouter", "deguster", "dejeter", "delecter", "delester", "delimiter",
            "deliter", "deluter", "demailloter", "demonter", "demoucheter", "demater", "demeriter",
            "denoter", "denoyauter", "depaqueter", "depiauter", "depister", "depiter", "deplanter",
            "depointer", "deporter", "depoter", "deputer", "derater", "derouter", "desacclimater",
            "desadapter", "-desaffecter", "desaimanter", "desajuster", "desappointer", "desargenter",
            "desenchanter", "-deserter", "deshabiter", "deshydrater", "desheriter", "desincruster",
            "-desinfecter", "desorbiter", "desorienter", "-detecter", "detester", "-detracter",
            "devaster", "develouter", "devolter", "embeter", "empieter", "empater",
            "enqueter", "enregimenter", "enteter", "excrementer", "excreter", "experimenter",
            "executer", "faineanter", "frequenter", "freter", "feliciter", "feter", "gater", "hater",
            "hebeter", "heriter", "hesiter", "inquieter", "interpreter", "lineamenter", "leviter",
            "mecontenter", "medicamenter", "mediter", "megoter", "meriter", "mesinterpreter",
            "numeroter", "necessiter", "-persecuter", "pieter", "plebisciter", "precipiter", "precompter",
            "preexister", "premediter", "presenter", "pretexter", "preter", "pater", "pecloter",
            "pericliter", "peter", "queter", "rapieceter", "refleter", "rempieter", "representer",
            "requeter", "roneoter", "rouspeter", "readapter", "reajuster", "recolter", "reconforter",
            "reescompter", "reexporter", "-refracter", "refuter", "regater", "regenter", "reglementer",
            "regurgiter", "rehabiliter", "rehydrater", "reimplanter", "reimporter", "-reinfecter",
            "reinventer", "reorienter", "reputer", "repeter", "resister", "resulter", "-retracter",
            "revolter", "-reediter", "secreter", "supplementer", "suppediter", "secreter", "sedimenter",
            "-selecter", "tempeter", "tremater", "tater", "teter", "vegeter", "ebouillanter",
            "ebouter", "ebruiter", "ecarter", "eclater", "ecoqueter", "ecourter", "ecouter", "ecrouter",
            "ecreter", "edenter", "edicter", "-editer", "-ejecter", "electrocuter", "eliciter",
            "emoucheter", "epater", "epinceter", "epointer", "epousseter", "epouvanter", "equeuter",
            "ereinter", "eructer", "etiqueter", "eteter", "eventer", "eviter", "oter", "alineater",
            "debecter", "desencrouter", "detricoter", "ebruter", "febriciter", "flanoter", "fregater",
            "gresilloter", "guereter", "heliporter", "lechoter", "-legislater", "meprisoter",
            "pedanter", "perequater", "preadapter", "-preempter", "progeniter", "raloter", "reacclimater",
            "readopter", "reaffecter", "reaimanter", "realimenter", "reappater", "reappreter",
            "reaugmenter", "recompleter", "reecouter", "reenchanter", "regouter", "rehabiter",
            "rehumecter", "reinjecter", "reinterpreter", "reinviter", "remater", "repreter", "sous-affreter",
            "teletraiter", "aperiter", "autodecontracter", "aeroporter", "aerotransporter", "bareter",
            "chelater", "coheriter", "contrenqueter", "copresenter", "coediter", "crapauter",
            "cranoter", "deafferenter", "deballaster", "debruter", "debudgeter", "debequeter",
            "decalfater", "decaoutchouter", "declimater", "decolmater", "decompacter", "decompartimenter",
            "deconnoter", "decoter", "decranter", "decravater", "decrementer", "decuiter", "decuscuter",
            "-deflater", "defolioter", "deforester", "-deformater", "defruiter", "defuncter",
            "degarroter", "degraphiter", "deguillemeter", "degurgiter", "degiter", "dejanter",
            "dejointer", "dejouter", "delaiter", "delenter", "deleucocyter", "delicoter", "deligoter",
            "delineamenter", "delister", "deleter", "demazouter", "denitrater", "depageoter",
            "depagnoter", "depailleter", "depajoter", "depanneauter", "depapilloter", "deparasiter",
            "deparementer", "deparqueter", "dephosphater", "depigmenter", "deposter", "dequeusoter",
            "deriveter", "derocter", "dereglementer", "desabouter", "desabriter", "desadopter",
            "desaffronter", "desafferenter", "desagater", "desamianter", "desapparenter", "desasphalter",
            "desattrister", "desazoter", "deschister", "desemboiter", "desemmailloter", "desempaqueter",
            "desemprunter", "desencarter", "desencaster", "desencliqueter", "desenregimenter",
            "desenteter", "desenvouter", "desergoter", "desexciter", "deshabiliter", "desilicater",
            "desinviter", "desister", "desocculter", "desolvater", "desulfater", "desulfiter",
            "desetiqueter", "detriter", "deventer", "geneter", "helitransporter", "implementer",
            "incrementer", "jesuiter", "machoter", "mecompter", "meliniter", "papieter", "planeter",
            "preacheter", "preciter", "precoter", "-predater", "-preformater", "premonter", "pretester",
            "pretraiter", "pedimenter", "radiodetecter", "rappreter", "rebater", "rebecoter",
            "rebequeter", "redebiter", "redebouter", "redebuter", "redecacheter", "redecanter",
            "redecapoter", "redecompter", "redeconnecter", "redecrypter", "redecreter", "redegoter",
            "redelimiter", "redemonter", "redepaqueter", "redeporter", "redeserter", "redesister",
            "redetecter", "redevaster", "refrequenter", "refeliciter", "refeter", "reheriter",
            "renumeroter", "repeter", "retater", "reabouter", "reabriter", "reabsenter", "reaccepter",
            "reaccidenter", "reaccoster", "readmonester", "reaffronter", "reaffreter", "reaffuter",
            "realerter", "reallaiter", "reannoter", "reargenter", "reargumenter", "rearpenter",
            "rearreter", "reasphalter", "reassister", "reemboiter", "reemmailloter", "reempaqueter",
            "reempieter", "reemprunter", "reenqueter", "reenvouter", "reescamoter", "reescorter",
            "reexalter", "reexhorter", "reexpliciter", "reexploiter", "reexperimenter", "reexecuter",
            "reimputer", "reincruster", "reinfester", "reingurgiter", "reinsister", "reinspecter",
            "reintenter", "reintercepter", "reobjecter", "reecarter", "reecourter", "reedicter",
            "reejecter", "reetiqueter", "surmedicamenter", "surrepresenter", "trejeter", "teledebiter",
            "-teledetecter", "telepancarter", "telepiloter", "telepointer", "teleporter", "veroter",
            "zeroter", "ebouqueter", "ebucheter", "echaloter", "ecointer", "ecolleter", "ecroter",
            "ecoter", "ejointer", "elaiter", "enoyauter", "epieter", "epuiseter", "iloter", "reglementer",
            "surinterpreter", "tuter", "degouter", "defragmenter", "deboiter", "ecrouter", "dereglementer",
            "desenvouter", "rapieceter", "reaffuter", "bequeter", "caimanter", "debarboter", "debequeter",
            "decrouter", "degiter", "demarabouter", "desindenter", "ruter", "teledicter", "autoreglementer",
            "demater", "desinventer", "reapparenter", "rediter", "regimenter", "-embater",
        };

        /// <summary>
        /// mots dont le x final se pronoce.
        /// </summary>
        private static HashSet<string> motsX = new HashSet<string>
        {
            "abax", "abrasax", "acromyrmex", "ajax", "alpax", "anthrax", "apex", "archéoptéryx",
            "aspalax", "bembex", "bombyx", "borax", "box", "carex", "chaix", "climax", "codex",
            "contumax", "contumax", "cortex", "cérambyx", "demodex", "duplex", "duplex", "démodex",
            "flytox", "hapax", "hélix", "ibex", "index", "larix", "larynx", "lastex", "latex",
            "lux", "lynx", "meix", "multiplex", "multiplex", "murex", "narthex", "onyx", "opopanax",
            "opoponax", "panax", "pharynx", "phlox", "phénix", "pneumothorax", "pnyx", "pollex",
            "préfix", "pyrex", "pétrosilex", "quadruplex", "reflex", "relax", "rhinopharynx",
            "rumex", "scandix", "scolex", "silex", "sirex", "smilax", "solex", "spalax", "sphex",
            "sphinx", "storax", "strix", "styrax", "syrinx", "thorax", "triplex", "télex", "ulex",
            "vertex", "viandox", "vortex", "acanthonyx", "acanthophœnix", "calythrix", "chénalopex",
            "cladothrix", "épipharynx", "glossanthrax", "glossocalyx", "infinitéisme", "hapax",
            "mégalonyx", "nycticorax", "oléothorax", "pédoclimax", "prépharynx", "protothorax",
            "sphénothorax", "sténothorax", "streptothrix", "vitex", "astérix", "obélix", "remix",
            "mix", "idéfix", "panoramix", "félix", "abraracourcix", "assurancetourix",
            "ocatarinetabellatchitchix", "ordralfabétix", "cétautomatix",
            // sans accents
            "archeopteryx", "cerambyx", "helix", "phenix", "prefix", "petrosilex", "telex", "chenalopex",
            "epipharynx", "infiniteisme", "megalonyx", "oleothorax", "pedoclimax", "prepharynx",
            "sphenothorax", "stenothorax", "asterix", "obelix", "idefix", "felix", "ordralfabetix",
            "cetautomatix",
        };

        /// <summary>
        /// Liste "complète" des verbes en "mer"
        /// </summary>
        private static HashSet<string> verbes_mer = new HashSet<string>
        {
            "abimer",
            "abîmer", "acclamer", "accoutumer", "affamer", "affermer", "affirmer", "aimer", "alarmer",
            "allumer", "amalgamer", "amertumer", "anagrammer", "animer", "apostumer", "approximer",
            "armer", "arrimer", "assommer", "assumer", "bitumer", "blasphémer", "blâmer", "boumer",
            "bramer", "brimer", "brumer", "calmer", "charmer", "chaumer", "chimer", "chloroformer",
            "chromer", "chômer", "clairsemer", "clamer", "comprimer", "confirmer", "conformer",
            "consommer", "consumer", "cramer", "crémer", "damer", "desquamer", "diadémer", "diaphragmer",
            "diffamer", "difformer", "diplômer", "dirimer", "décarêmer", "décharmer", "déchaumer",
            "décimer", "déclamer", "décomprimer", "déflegmer", "déformer", "dégermer", "dégommer",
            "dégrimer", "dénommer", "déplumer", "déprimer", "désaccoutumer", "désaimer", "désarmer",
            "désenrhumer", "dîmer", "effumer", "embaumer", "embrumer", "empaumer", "emplumer",
            "enfermer", "enflammer", "enfumer", "enrhumer", "ensimer", "entamer", "enthousiasmer",
            "entrefermer", "envenimer", "escrimer", "espalmer", "essaimer", "estimer", "exclamer",
            "exhumer", "exprimer", "fermer", "filmer", "former", "frimer", "fumer", "gemmer",
            "germer", "gommer", "gourmer", "grimer", "humer", "imprimer", "infirmer", "informer",
            "inhumer", "intimer", "lamer", "larmer", "limer", "légitimer", "maximer", "microfilmer",
            "millésimer", "mimer", "mésestimer", "nommer", "opprimer", "palmer", "parfumer",
            "parsemer", "paumer", "pommer", "primer", "proclamer", "programmer", "préformer",
            "prénommer", "présumer", "rallumer", "ramer", "ranimer", "refermer", "reformer",
            "remplumer", "renfermer", "renflammer", "renommer", "rimer", "rythmer", "réaccoutumer",
            "réaffirmer", "réanimer", "réarmer", "réclamer", "rédimer", "réformer", "réimprimer",
            "réprimer", "résumer", "rétamer", "semer", "slalomer", "sommer", "sublimer", "subsumer",
            "supprimer", "surcomprimer", "surestimer", "surnommer", "tomer", "tramer", "transformer",
            "transhumer", "trimer", "vacarmer", "victimer", "vidimer", "écimer", "écrémer", "écumer",
            "élimer", "étamer", "éverdumer", "défumer", "flammer", "groomer", "groumer", "infamer",
            "mésaimer", "minimer", "pseudonymer", "raccoutumer", "réaimer", "reconfirmer", "réentamer",
            "rentamer", "ressemer", "retransformer", "sous", "affermer", "sous", "estimer", "surimprimer",
            "sursemer", "termer", "aramer", "arimer", "autoallumer", "autoconsommer", "autoformer",
            "autoproclamer", "barémer", "biotransformer", "boffumer", "bromer", "camer", "chêmer",
            "coanimer", "coexprimer", "costumer", "débitumer", "débromer", "déchromer", "dédamer",
            "défrimer", "délégitimer", "déparfumer", "déprogrammer", "dépâmer", "déramer", "déréprimer",
            "désarrimer", "déschlammer", "désemplumer", "désenflammer", "désenfumer", "désengommer",
            "désensimer", "désenthousiasmer", "désenvenimer", "désinformer", "désâmer", "désétamer",
            "emmiasmer", "empalmer", "empommer", "enchaussumer", "enformer", "engamer", "engommer",
            "enlarmer", "esseimer", "essimer", "fantasmer", "flemmer", "gendarmer", "hydroformer",
            "microprogrammer", "monoprogrammer", "multiprogrammer", "mésinformer", "napalmer",
            "normer", "néoformer", "pantomimer", "performer", "plamer", "plumer", "polychromer",
            "prismer", "préimprimer", "pâmer", "périmer", "raffermer", "rebitumer", "rechaumer",
            "rechromer", "recomprimer", "reconsommer", "redamer", "refilmer", "refumer", "regermer",
            "regommer", "relimer", "relégitimer", "renformer", "renvenimer", "repaumer", "reproclamer",
            "reprogrammer", "resemer", "rhumer", "réaffermer", "réallumer", "réarrimer", "réassumer",
            "réenflammer", "réenrhumer", "réenthousiasmer", "réenvenimer", "réestimer", "réexhumer",
            "réexprimer", "-régimer", "réintimer", "réétamer", "spasmer", "stemer", "stemmer",
            "suranimer", "surarmer", "surconsommer", "surinformer", "thermoformer", "téléimprimer",
            "zoomer", "échaumer", "égermer", "épilamer", "diplomer", "-pimer", "carmer", "abimer",
            "entraimer", "spammer", "terraformer", "apatamer", "chroumer", "autolégitimer",
            "autotransformer", "préprogrammer", "reclamer", "sousestimer", "grammer", "acarêmer",
            // versions sans accents
            "abimer", "blasphemer", "blamer", "chomer", "cremer", "diademer", "diplomer", "decaremer",
            "decharmer", "dechaumer", "decimer", "declamer", "decomprimer", "deflegmer", "deformer",
            "degermer", "degommer", "degrimer", "denommer", "deplumer", "deprimer", "desaccoutumer",
            "desaimer", "desarmer", "desenrhumer", "dimer", "legitimer", "millesimer", "mesestimer",
            "preformer", "prenommer", "presumer", "reaccoutumer", "reaffirmer", "reanimer", "rearmer",
            "reclamer", "redimer", "reformer", "reimprimer", "reprimer", "resumer", "retamer",
            "ecimer", "ecremer", "ecumer", "elimer", "etamer", "everdumer", "defumer", "mesaimer",
            "reaimer", "reentamer", "baremer", "chemer", "debitumer", "debromer", "dechromer",
            "dedamer", "defrimer", "delegitimer", "deparfumer", "deprogrammer", "depamer", "deramer",
            "dereprimer", "desarrimer", "deschlammer", "desemplumer", "desenflammer", "desenfumer",
            "desengommer", "desensimer", "desenthousiasmer", "desenvenimer", "desinformer", "desamer",
            "desetamer", "mesinformer", "neoformer", "preimprimer", "pamer", "perimer", "relegitimer",
            "reaffermer", "reallumer", "rearrimer", "reassumer", "reenflammer", "reenrhumer",
            "reenthousiasmer", "reenvenimer", "reestimer", "reexhumer", "reexprimer", "-regimer",
            "reintimer", "reetamer", "teleimprimer", "echaumer", "egermer", "epilamer", "autolegitimer",
            "preprogrammer", "acaremer",
            // "dormer" est là pour intercepter le cas de ils/elles dorment
            "dormer", "endormer", "rendormer",
        };

        /// <summary>
        /// Liste mots non identifiés par les règles, où 'ch' se prononce [k]
        /// </summary>
        private static HashSet<string> motsChK = new HashSet<string>
        {
            "achéen", "achéenne", "achéennes", "achéens", "achéménide", "achéménides", "achène",
            "achènes", "achillée", "achillées", "achilléine", "achilléines", "achirite", "achirites",
            "achilléen", "achilléenne", "achilléennes", "achilléens", "achilléine", "achilléines",
            "achilléoïde", "achilléoïdes", "achillétine", "achillétines",
            "acholique", "acholiques", "achondroplase", "achondroplases", "achondroplasie", "achondroplasies",
            "aeschne", "aeschnes", "allochtone", "allochtones", "allochtonie", "anchilops", "andrachné",
            "andrachnés", "antichthone", "antichthones", "antichtone", "antichtones", "apocholique",
            "arachnéen", "arachnéenne", "arachnéennes", "arachnéens", "arachnéolithe", "arachnéosites",
            "arachnide", "arachnides", "arachnitis", "arachnoïde", "arachnoïdes", "arachnoïdien",
            "arachnoïdienne", "arachnoïdiennes", "arachnoïdiens", "arachnoïdite", "arachnoïdites",
            "arachnophiles", "archaïque", "archaïquement", "archaïques", "archaïsa", "archaïsai",
            "archaïsaient", "archaïsais", "archaïsait", "archaïsâmes", "archaïsant", "archaïsante",
            "archaïsantes", "archaïsants", "archaïsas", "archaïsasse", "archaïsassent", "archaïsasses",
            "archaïsassiez", "archaïsassions", "archaïsât", "archaïsâtes", "archaïse", "archaïsé",
            "archaïsée", "archaïsées", "archaïsent", "archaïser", "archaïsera", "archaïserai",
            "archaïseraient", "archaïserais", "archaïserait", "archaïseras", "archaïsèrent", "archaïserez",
            "archaïseriez", "archaïserions", "archaïserons", "archaïseront", "archaïses", "archaïsés",
            "archaïsez", "archaïsiez", "archaïsions", "archaïsme", "archaïsmes", "archaïsons",
            "archaïste", "archaïstes", "archal", "archals", "archange", "archangélique", "archangéliques",
            "archanges", "archébiose", "archéen", "archéenne", "archéennes", "archéens", "archégone",
            "archégones", "archéidés", "archencéphale", "archencéphalon", "archentère", "archentères",
            "archentéron", "archentérons", "archétype", "archétypes", "archétypique", "archétypiques",
            "archontat", "archontats", "archonte", "archontes", "arthrochondrite", "aurichalque",
            "aurichalques", "aurochs", "autochtone", "autochtones", "autochtonie", "autochtonies",
            "azédarach", "batrachographe", "batrachomorphes", "batrachophobie", "batrachopides",
            "blastocholines", "brachélytre", "brachélytres", "brachial", "brachiale", "brachiales",
            "brachiaux", "brachycéphale", "brachycéphales", "brachycéphalie", "brachycéphalies",
            "brachycéphalisation", "brachycéphalisations", "brachylogie", "brachylogies", "brachylogique",
            "brachylogiques", "brachyodonte", "brachyodontie", "brachypode", "brachyrhynque",
            "brachysome", "brachytype", "branchial", "branchiale", "branchiales", "branchiaux",
            "broncholithe", "bronchopneumonique", "bronchopneumoniques", "bronchoscope", "bronchoscopes",
            "bronchoscopie", "bronchoscopies", "bronchotomie", "bronchotomies", "callichte", "callichtes",
            "carach", "carachs", "carchésion", "carchésions", "catéchuménat", "catéchuménats", "catéchumène",
            "catéchumènes", "chaetodon", "chaetodons", "chalaze", "chalazes", "chalcaspide", "chalcaspides",
            "chalcogènes", "chalcographie", "chalcographies", "chaldaïque", "chaldaïques", "chaldaïsme",
            "chaldaïsmes", "chaldéen", "chaldéenne", "chaldéennes", "chaldéens", "chaos", "chaotique",
            "chaotiquement", "chaotiques", "charismatique", "charismatiques", "charismatisme",
            "charismatismes", "charisme", "charismes", "charybde", "charybdes", "chéilalgie",
            "chéilanthe", "chéilocace", "chéilodactyle", "chéilophagie", "chéiloraphie", "chéiranthé",
            "chéiranthère", "chéiranthus", "chéirogale", "chéirogaleus", "chéirolepis", "chéiromys",
            "chéiroptères", "chélidoine", "chélidoines", "chéliforme", "chélipède", "chélodonte",
            "chéloïde", "chéloïdes", "chéloïdien", "chéloïdienne", "chéloïdiennes", "chéloïdiens",
            "chélonée", "chélonées", "chélonien", "chélonienne", "chéloniennes", "chéloniens",
            "chéloniens", "chélonographe", "chélonographie", "chélonophage", "chélopode", "chélopodes",
            "chélostome", "chélyde", "chélydés", "chélys", "chénalopex", "chéniscus", "chénocolymbes",
            "chénopodées", "chénopodiacées", "chénopodiées", "chénosure", "chersochélone", "chersohydrochélone",
            "chétocéphale", "chétodiptère", "chétodon", "chétodons", "chétodonte", "chétognathes",
            "chétopode", "chétopodes", "chétosomides", "chianti", "chiantis", "chiasma", "chiasmas",
            "chiasme", "chiasmes", "chiragre", "chiragres", "chirobaliste", "chirocentre", "chirognomonie",
            "chirognomonies", "chirographaire", "chirographaires", "chirographe", "chirographes",
            "chirographia", "chirographiai", "chirographiaient", "chirographiais", "chirographiait",
            "chirographiâmes", "chirographiant", "chirographias", "chirographiasse", "chirographiassent",
            "chirographiasses", "chirographiassiez", "chirographiassions", "chirographiât", "chirographiâtes",
            "chirographie", "chirographié", "chirographiée", "chirographiées", "chirographient",
            "chirographier", "chirographiera", "chirographierai", "chirographieraient", "chirographierais",
            "chirographierait", "chirographieras", "chirographièrent", "chirographierez", "chirographieriez",
            "chirographierions", "chirographierons", "chirographieront", "chirographies", "chirographiés",
            "chirographiez", "chirographiiez", "chirographiions", "chirographions", "chirologie",
            "chirologies", "chironecte", "chironome", "chironomes", "chironomie", "chironomies",
            "chirosophe", "chirosophes", "chirote", "chitine", "chitines", "chitineuse", "chitineuses",
            "chitineux", "chiton", "chitons", "chleuh", "choanoflagellés", "choéphore", "choéphores",
            "cholagogue", "cholagogues", "cholécystectomie", "cholécystectomies", "cholécystite",
            "cholécystites", "cholédocholithotripsie", "cholédociarctie", "cholédocotomie", "cholédoque",
            "cholédoques", "choléra", "choléraphage", "choléras", "cholérifère", "cholériforme",
            "cholériformes", "cholérine", "cholérines", "cholérique", "cholériques", "cholestérol",
            "cholestérols", "choline", "cholines", "cholique", "choliques", "chondrichtyens",
            "chondriome", "chondriomes", "chondroplaste", "chondroptérygiens", "chondrostéens",
            "chthonien", "chthonienne", "chthoniennes", "chthoniens", "cinchonine", "cinchonines",
            "cochléaire", "cochléaires", "cochléaria", "cochléarias", "cochlée", "cochlées", "cochylis",
            "conchite", "conchites", "conchoïdal", "conchoïdale", "conchoïdales", "conchoïdaux",
            "conchoïde", "conchoïdes", "conchyliculteur", "conchyliculteurs", "conchylien", "conchylienne",
            "conchyliennes", "conchyliens", "conchylifère", "conchylifères", "conchyliologie",
            "conchyliologies", "conchyliologiste", "conchyliologistes", "conchyliologue", "conchyliologues",
            "conchylis", "courbachs", "cynorchis", "décadrachme", "décadrachmes", "diachylon",
            "diachylons", "diachylum", "diachylums", "dichotome", "dichotomes", "dichotomie",
            "dichotomies", "dichotomique", "dichotomiques", "dichotomisa", "dichotomisai", "dichotomisaient",
            "dichotomisais", "dichotomisait", "dichotomisâmes", "dichotomisant", "dichotomisas",
            "dichotomisasse", "dichotomisassent", "dichotomisasses", "dichotomisassiez", "dichotomisassions",
            "dichotomisât", "dichotomisâtes", "dichotomise", "dichotomisé", "dichotomisée", "dichotomisées",
            "dichotomisent", "dichotomiser", "dichotomisera", "dichotomiserai", "dichotomiseraient",
            "dichotomiserais", "dichotomiserait", "dichotomiseras", "dichotomisèrent", "dichotomiserez",
            "dichotomiseriez", "dichotomiserions", "dichotomiserons", "dichotomiseront", "dichotomises",
            "dichotomisés", "dichotomisez", "dichotomisiez", "dichotomisions", "dichotomisons",
            "didrachme", "didrachmes", "dolichocéphale", "dolichocéphales", "dolichocéphalie",
            "dolichocéphalies", "dolichocrânien", "dolichodrome", "dolichogyne", "dolichomégacôlon",
            "dolichomorphe", "dolichopode", "dolichosaure", "dolichostylé", "drachme", "drachmes",
            "échidné", "échidnés", "échinocactus", "échinocoque", "échinocoques", "échinoderme",
            "échinodermes", "écho", "écholalie", "écholalies", "échos", "échosonde", "échotier",
            "échotiers", "enchondrome", "enchondromes", "enchymose", "enchymoses", "eucharistie",
            "eucharisties", "eucharistique", "eucharistiques", "euchologe", "euchologes", "euchologue",
            "euchologues", "eunuchisme", "eunuchismes", "eunuchoïde", "eunuchoïdes", "eunuchoïdisme",
            "eunuchoïdismes", "exarchat", "exarchats", "fuchsine", "fuchsiné", "fuchsines", "fuchsinophile",
            "hétérochtone", "hyperdolichocéphalie", "hyperfuchsien", "hypocholestérinémie", "ichneumon",
            "ichneumonides", "ichneumonidés", "ichneumons", "ichnographie", "ichnographies", "ichnographique",
            "ichthyophagie", "ichthyophagies", "ichthyophagique", "ichthyornis", "ichtyobdellidés",
            "ichtyobiologiste", "ichtyographie", "ichtyol", "ichtyologie", "ichtyologies", "ichtyologique",
            "ichtyologiques", "ichtyologiste", "ichtyologistes", "ichtyologue", "ichtyologues",
            "ichtyols", "ichtyomasse", "ichtyophage", "ichtyophages", "ichtyophagie", "ichtyophagies",
            "ichtyophagique", "ichtyophile", "ichtyophtalme", "ichtyophtalmite", "ichtyoptérygiens",
            "ichtyornis", "ichtyosaure", "ichtyosaures", "ichtyosauriens", "ichtyose", "ichtyoses",
            "ichtyostega", "ichtyostégidés", "ichtyotoxique", "ichtyotoxisme", "ichtys", "inchoatif",
            "inchoatifs", "inchoation", "inchoative", "inchoatives", "inchoativité", "intratrachéal",
            "kalanchoé", "kalanchoés", "krach", "krachs", "lichen", "lichénique", "lichéniques",
            "lichens", "loch", "lochs", "looch", "loochs", "lychnide", "lychnides", "lychnis",
            "mach", "machairodus", "machaon", "machaons", "machiavel", "machiavélique", "machiavéliques",
            "machiavélisa", "machiavélisai", "machiavélisaient", "machiavélisais", "machiavélisait",
            "machiavélisâmes", "machiavélisant", "machiavélisas", "machiavélisasse", "machiavélisassent",
            "machiavélisasses", "machiavélisassiez", "machiavélisassions", "machiavélisât", "machiavélisâtes",
            "machiavélise", "machiavélisé", "machiavélisée", "machiavélisées", "machiavélisent",
            "machiavéliser", "machiavélisera", "machiavéliserai", "machiavéliseraient", "machiavéliserais",
            "machiavéliserait", "machiavéliseras", "machiavélisèrent", "machiavéliserez", "machiavéliseriez",
            "machiavéliserions", "machiavéliserons", "machiavéliseront", "machiavélises", "machiavélisés",
            "machiavélisez", "machiavélisiez", "machiavélisions", "machiavélisme", "machiavélismes",
            "machiavélisons", "machiavéliste", "machiavélistes", "machiavels", "machs", "manichéen",
            "manichéenne", "manichéennes", "manichéens", "manichéisme", "manichéismes", "mélancholier",
            "melchior", "melchiors", "melchite", "melchites", "ménechme", "ménechmes", "michelangelesque",
            "michelangelesques", "michelangesque", "michelangesques", "microrchide", "mitochondrie",
            "mitochondries", "moloch", "molochs", "monobrachial", "monorchide", "monorchides",
            "monorchidie", "monorchidies", "munichois", "munichoise", "munichoises", "nabuchodonosor",
            "nabuchodonosors", "neurophychiatre", "neurophychiatres", "neuropsychiatrie", "neuropsychiatries",
            "neuropsychiatrique", "neuropsychiatriques", "oenochoé", "oenochoés", "onychomancie",
            "onychophore", "opodeldoch", "opodeldochs", "orchidacées", "orchidales", "orchidée",
            "orchidées", "orchidien", "orchidocèle", "orchidodynie", "orchidothérapie", "orchiodynie",
            "orchiopexie", "orchiorraphie", "orchiotome", "orchis", "orchite", "orchites", "orchotome",
            "orchotomie", "orichalque", "orichalques", "ostéichthyens", "paracholéra", "parorchide",
            "pelvitrochantérien", "périchondre", "périchondres", "pibroch", "pibrochs", "picholine",
            "picholines", "pichtogorne", "podolachnite", "polychètes", "prétrachéal", "psychédélique",
            "psychédéliques", "psychédélisme", "psychédélismes", "psychiatre", "psychiatres",
            "psychiatrie", "psychiatries", "psychiatrique", "psychiatriques", "rhinotrachéite",
            "rhynchite", "rhynchites", "rhynchobdelles", "rhynchobdellides", "rhynchobdellidés",
            "rhynchonelles", "rhynchotes", "spirochète", "spirochètes", "spirochétose", "spirochétoses",
            "splanchnique", "splanchniques", "splanchnologie", "splanchnologies", "stichomythie",
            "stichomythies", "stochastique", "stochastiques", "stœchiométrie", "stœchiométries",
            "stœchiométrique", "stœchiométriques", "strychnine", "strychnines", "strychnos", "synechthre",
            "synechthrie", "taricheute", "taricheutes", "tétradrachme", "tétradrachmes", "tétrarchat",
            "tétrarchats", "tichodrome", "tichodromes", "trachéen", "trachéenne", "trachéennes",
            "trachéens", "trachéide", "trachéides", "trachéifère", "trachéite", "trachéites",
            "trachélides", "trachome", "trachomes", "trachydolérite", "trachylides", "trachystomates",
            "trachystomes", "trachyte", "trachytes", "trachytique", "trachytiques", "trichiasis",
            "trichinal", "trichinale", "trichinales", "trichinaux", "trichiné", "trichinée", "trichinées",
            "trichinés", "trichinose", "trichinoses", "trichite", "trichites", "trichocardie",
            "trichocyste", "trichode", "trichodesmie", "trichodonte", "trichoépithéliome", "trichogène",
            "trichogyne", "trichopathie", "trichoptères", "trichosanthe", "trichosome", "trochaïque",
            "trochaïques", "trochanter", "trochantérien", "trochantérienne", "trochantériennes",
            "trochantériens", "trochanters", "trochile", "trochiles", "trochilidés", "trochlée",
            "trochlées", "tylenchus", "ultrabrachycéphale", "varech", "varechs", "vichnouisme",
            "vichnouismes", "yachmak", "yachmaks","orchiectomie", "orchiépididymite", "trichodesmium",
            "aechmalotarque", "aechmalotarques", "aechmalote", "aechmalotes", "allochirie", "allochiries",
            "zoopsychiatrie", "zoopsychiatries", "cromlech", "cromlechs",
        };

        /// <summary>
        /// Liste mots où 'qu' se prononce [kw]
        /// </summary>
        private static HashSet<string> motsQUkw = new HashSet<string>
        {
            "aquafortiste", "aquafortistes", "aquamoteur", "aquaphobie", "aquaplane", "aquaplanes",
            "aquarella", "aquarellai", "aquarellaient", "aquarellais", "aquarellait", "aquarellâmes",
            "aquarellant", "aquarellas", "aquarellasse", "aquarellassent", "aquarellasses", "aquarellassiez",
            "aquarellassions", "aquarellât", "aquarellâtes", "aquarelle", "aquarellé", "aquarellée",
            "aquarellées", "aquarellent", "aquareller", "aquarellera", "aquarellerai", "aquarelleraient",
            "aquarellerais", "aquarellerait", "aquarelleras", "aquarellèrent", "aquarellerez",
            "aquarelleriez", "aquarellerions", "aquarellerons", "aquarelleront", "aquarelles",
            "aquarellés", "aquarellez", "aquarelliez", "aquarellions", "aquarelliste", "aquarellistes",
            "aquarellons", "aquarien", "aquarienne", "aquariennes", "aquariens", "aquarium", "aquariums",
            "aquateinte", "aquateintes", "aquatintiste", "aquatintistes", "aquatique", "aquatiques",
            "biquadratique", "biquadratiques", "colliquatif", "colliquatifs", "colliquation",
            "colliquations", "colliquative", "colliquatives", "desquama", "desquamai", "desquamaient",
            "desquamais", "desquamait", "desquamâmes", "desquamant", "desquamas", "desquamasse",
            "desquamassent", "desquamasses", "desquamassiez", "desquamassions", "desquamât", "desquamâtes",
            "desquamation", "desquamations", "desquame", "desquamé", "desquamée", "desquamées",
            "desquament", "desquamer", "desquamera", "desquamerai", "desquameraient", "desquamerais",
            "desquamerait", "desquameras", "desquamèrent", "desquamerez", "desquameriez", "desquamerions",
            "desquamerons", "desquameront", "desquames", "desquamés", "desquamez", "desquamiez",
            "desquamions", "desquamons", "équanimité", "équanimités", "équateur", "équateurs",
            "équation", "équations", "équatorial", "équatoriale", "équatoriales", "équatoriaux",
            "exequatur", "inadéquat", "inadéquate", "inadéquates", "inadéquation", "inadéquations",
            "inadéquats", "inéquation", "inéquations", "kumquat", "kumquats", "liquation", "liquations",
            "péréquation", "péréquations", "quadrature", "quadraturer", "quadratures", "quadrillion",
            "quadrillions", "quadrique", "quadriques", "quadrirème", "quadrirèmes", "quadrisaïeul",
            "quadrisaïeule", "quadrisaïeules", "quadrisaïeuls", "quadrisyllabe", "quadrisyllabes",
            "quadrisyllabique", "quadrisyllabiques", "quadrivalence", "quadrivalences", "quadrivalent",
            "quadrivalente", "quadrivalentes", "quadrivalents", "quadrivium", "quadriviums", "quadrumane",
            "quadrumanes", "quadrupède", "quadrupèdes", "quadrupla", "quadruplai", "quadruplaient",
            "quadruplais", "quadruplait", "quadruplâmes", "quadruplant", "quadruplas", "quadruplasse",
            "quadruplassent", "quadruplasses", "quadruplassiez", "quadruplassions", "quadruplât",
            "quadruplâtes", "quadruple", "quadruplé", "quadruplée", "quadruplées", "quadruplement",
            "quadruplements", "quadruplent", "quadrupler", "quadruplera", "quadruplerai", "quadrupleraient",
            "quadruplerais", "quadruplerait", "quadrupleras", "quadruplèrent", "quadruplerez",
            "quadrupleriez", "quadruplerions", "quadruplerons", "quadrupleront", "quadruples",
            "quadruplés", "quadruplex", "quadruplez", "quadrupliez", "quadruplions", "quadruplons",
            "quaker", "quakereresse", "quakereresses", "quakerien", "quakerisme", "quakerismes",
            "quakeriste", "quakers", "quanta", "quantum", "quantums", "-quarter", "-quarters", "quartet",
            "quartets", "quartette", "quartettes", "quartettiste", "quartettistes", "quarto",
            "quartz", "quartzeuse", "quartzeuses", "quartzeux", "quartzifère", "quartzifères",
            "quartzine", "quartzique", "quartziques", "quartzite", "quartzites", "quasar", "quasars",
            "quassia", "quassias", "quassier", "quassiers", "quassine", "quassines", "quater",
            "quaternaire", "quaternaires", "quaterne", "quaternes", "quaternion", "quaternions",
            "quatrillion", "quattrocentiste", "quattrocentistes", "quattrocento", "quattrocentos",
            "quatuor", "quatuors", "quetsche", "quetsches", "quetschier", "quetschiers", "squale",
            "squales", "squalide", "squalides", "squalidés", "squalidité", "squalidités", "squaloïdes",
            "squamates", "squame", "squames", "squameuse", "squameuses", "squameux", "squamule",
            "squamules", "square", "squares", "squash", "squashs", "squat", "squatine", "squatines",
            "squatinidés", "squats", "squatta", "squattai", "squattaient", "squattais", "squattait",
            "squattâmes", "squattant", "squattas", "squattasse", "squattassent", "squattasses",
            "adéquat", "adéquats", "adéquate", "adéquates", "adéquatement", "adéquata", "adéquatai",
            "adéquataient", "adéquatais", "adéquatais", "adéquatait", "adéquatant", "adéquatas",
            "adéquatasse", "adéquatassent", "adéquatasses", "adéquatassiez", "adéquatassions",
            "adéquate", "adéquate", "adéquate", "adéquate", "adéquate", "adéquatent", "adéquatent",
            "adéquater", "adéquatera", "adéquaterai", "adéquateraient", "adéquaterais", "adéquaterais",
            "adéquaterait", "adéquateras", "adéquaterez", "adéquateriez", "adéquaterions", "adéquaterons",
            "adéquateront", "adéquates", "adéquates", "adéquatez", "adéquatez", "adéquatiez",
            "adéquatiez", "adéquations", "adéquations", "adéquatons", "adéquatons", "adéquatâmes",
            "adéquatât", "adéquatâtes", "adéquatèrent", "adéquatés", "adéquaté", "adéquatée",
            "adéquatées", "adéquation", "adéquations", "aquicole", "aquicoles", "aquiculture",
            "aquicultures", "aquifère", "aquifères"
        };

        /// <summary>
        /// Liste mots où 'en' se prononce [5] et qui ne sont pas intercéptés par les règles déjà
        /// existantes.
        /// </summary>
        private static HashSet<string> motsEn5 = new HashSet<string>
        {
            "agenda", "agendas", "aléoutiens", "algonkiens", "alsaciens", "angioendothéliome",
            "apexiens", "apiens", "aplacentaire", "aplacentaires", "aptiens", "archiloquiens",
            "arsénobenzol", "artiens", "aryens", "astartiens", "asymptotiquement", "attingent",
            "baconiens", "banvilliens", "basedowiens", "bathycentèse", "bayreuthiens", "beethoveniens",
            "bengale", "bengales", "bengali", "bengalis", "benjamin", "benjamine", "benjamines",
            "benjamins", "benjoin", "benjoins", "bens", "benthique", "benthiques", "benthos",
            "benzédrine", "benzène", "benzènes", "benzènesulfonyle", "benzénique", "benzéniques",
            "benzile", "benziles", "benzilique", "benziliques", "benzimide", "benzine", "benzines",
            "benzocarbonique", "benzoène", "benzoestrol", "benzoïque", "benzoïques", "benzol",
            "benzolé", "benzolée", "benzolées", "benzolés", "benzols", "benzone", "benzosulfate",
            "benzosulfurique", "benzoyle", "benzoyles", "benzyle", "benzyles", "berkeleyens",
            "berkeleyen","ben",
            "bibendum", "bibendums", "bienfaisance", "bienfaisances", "bienfaisant", "bienfaisante",
            "bienfaisantes", "bienfaisants", "bienheureuse", "bienheureusement", "bienheureuses",
            "bienheureux", "biscayens", "bismarckiens", "blende", "blendes", "booléens", "booléiens",
            "botticelliens", "brens", "browniens", "byroniens", "cardiocentèse", "carpiens", "caspiens",
            "chérifien", "chérifiens", "chondrichtyens", "citoyens", 
            "compendium", "compendiums", "concitoyens", "confucéiens",
            "consensus", 
            "dengue", "dengues", "diagnosticiens", "dibromobenzène",
            "dostoïevskiens", "doyens", "efendi", "efendis", "effendi", "effendis", "éthiopiens",
            "flaubertiens", "halobenthos", "hégéliens", "hertziens", "himalayens", "hollywoodiens",
            "hornblende", "hornblendes", "kentrophylle", "kentrophylles", "labadens", "lacertiens",
            "leibniziens", "lépidodendron", "lépidodendrons", "libripens", "libyens", "magenta",
            "magentas", "marengo", "marengos", "mayens", "memento", "mémento", "mementos", "mémentos",
            "memnoniens", "mendélévium", "mendéléviums", "mendélienne", "mendéliennes", "mendéliens",
            "mendélisme", "mendélismes", "mendéliste", "mendélistes", "menthane", "menthanes",
            "menthanol", "menthanols", "menthanone", "menthanones", "métacarpiens", "mitoyens",
            "monobromobenzène", "monochlorobenzène", "monodébenzylation", "montiens", "montparnassiens",
            "moyens", "nancéiens", "népenthès", "newtoniens", "nietzschéens", "nitrobenzène",
            "nitrobenzènes", "nitrobenzine", "nitrobenzines", "nitrobenzol", "oedipiens", "olympiens",
            "orthosulfamidobenzoïque", "ostéichthyens", "oxybenzène", "oxybenzoïque", "paracentèse",
            "paracentèses", "paradichlorobenzène", "paradichlorobenzènes", "parkinsoniens", "pechblende",
            "pechblendes", "pélasgiens", "pensum", "pensums", "pentacle", "pentacles", "pentacorde",
            "pentacordes", "pentacrinidés", "pentacrinus", "pentadyname", "pentagonal", "pentagonale",
            "pentagonales", "pentagonaux", "pentagone", "pentagones", "pentagynie", "pentalpha",
            "pentamères", "pentamètre", "pentamètres", "pentandrie", "pentapétalé", "pentapyle",
            "pentarchie", "pentarchies", "pentarhombique", "pentateuque", "pentateuques", "pentathle",
            "pentathles", "pentathlienne", "pentathlon", "pentathlons", "pentatomidés", "pentélique",
            "pentéliques", "pentosane", "périappendiculaire", "phellodendron",
            "philodendron", "philodendrons", "pithécanthropiens", "placenta", "placentaire", "placentaires",
            "placentas", "placentation", "placentations", "plébéiens", "pleurocentèse", "pneumoentérite",
            "pompéiens", "proenzyme", "propylbenzine", "psychosomaticiens", "rhododendron",
            "rhododendrons", "riemanniens", "sapientiaux", "sempervirens", "séroappendicite",
            "shakespeariens", "sidérodendron", "sulfobenzoïque", "tarpéiens",
            "transocéaniens", "tribromobenzène", "tylenchus", "vosgiens", "wagnériens", "wormiens",
            "würmiens", "xiphoïdiens", "zende", "zoobenthos",
            "appendice", "appendices", "appendicectomie", "appendicectomies", "appendicite", "appendicites",
            "appendiculaire", "appendiculaires", "appendiculaire", "appendiculaires", "appendiculé",
            "appendiculés", "appendiculé", "appendiculés", "appendiculée", "appendiculées", "appendicostomie",
            "appendicostomies", "addendas", "addendum", "addenda", "addendums", "algonkien", "algonkiens"
        };

        /// <summary>
        /// Liste des mots où ill se pronoce [il] et non [j] ou [ij]. Les pluriels doivent être
        /// dans la liste.
        /// </summary>
        private static HashSet<string> except_ill = new HashSet<string>
        {
            "abbevillien", "abbevillienne", "abbevilliennes", "abbevilliens", "abbevillois", "abbevilloise",
            "abbevilloises", "admaxillaire", "admaxillaires",
            "achille", "achilles", "achillée", "achillées", "ancillaire", "ancillaires", "aspergillose",
            "aspergilloses", "aspergillus", "axillaire", "axillaires", "bacillaire", "bacillaires",
            "bellevillois", "bellevilloise", "bellevilloises", "bidonville", "bidonvilles", "bill", "billevesée",
            "billevesées", "billion", "billions", "bills", "bougainvillée", "bougainvillées", "bougainvillier", "bougainvilliers",
            "calville", "calvilles", "capillaire", "capillaires", "capillarité", "capillarités",
            "capilliculteur", "capilliculteurs", "caterpillar", "chinchilla", "chinchillas", "codicille",
            "codicilles", "cyrillique", "cyrilliques",
            "désillusion", "désillusionné", "désillusionnés", "désillusionnement", "désillusionnements", "désillusionner",
            "désillusions", "désillusionna", "désillusionnai", "désillusionnaient", "désillusionnais", "désillusionnait",
            "désillusionnâmes", "désillusionnant", "désillusionnas", "désillusionnasse", "désillusionnassent",
            "désillusionnasses", "désillusionnassiez", "désillusionnassions", "désillusionnât",
            "désillusionnâtes", "désillusionne", "désillusionnée", "désillusionnées", "désillusionnent",
            "désillusionnera", "désillusionnerai", "désillusionneraient", "désillusionnerais",
            "désillusionnerait", "désillusionneras", "désillusionnèrent", "désillusionnerez",
            "désillusionneriez", "désillusionnerions", "désillusionnerons", "désillusionneront",
            "désillusionnes", "désillusionnez", "désillusionniez", "désillusionnions", "désillusionnons",
            "drill", "drills", "égorgilla", "égorgillai", "égorgillaient", "égorgillais", "égorgillait", "égorgillâmes",
            "égorgillant", "égorgillas", "égorgillasse", "égorgillasses", "égorgillassiez", "égorgillassions",
            "égorgillât", "égorgillâtes", "égorgille", "égorgillé", "égorgillée", "égorgillées",
            "égorgiller", "égorgillera", "égorgillerai", "égorgilleraient", "égorgillerais", "égorgillerait",
            "égorgilleras", "égorgillèrent", "égorgillerez", "égorgilleriez", "égorgillerions",
            "égorgillerons", "égorgilleront", "égorgilles", "égorgillés", "égorgillez", "égorgillons",
            "fibrillation", "fibrillations",
            "fringillidé", "fritillaires", "gilles", "grill", "imbécillité", "imbécillités", "killer", "killers", "krill", "krills",
            "lilliputien", "lilliputienne", "lilliputiennes", "lilliputiens", "lillois", "lilloise", "lilloises", "mandrill",
            "mandrills", "maxillaire", "maxillaires", "multimilliardaire", "multimilliardaires", "multimillionnaire",
            "multimillionnaires", "papillaire", "papillaires", "pénicilline", "pénicillines", "pupillaire", "pupillaires",
            "pupillarité", "pupillarités", "pusillanime", "pusillanimes", "pusillanimité", "pusillanimités", "quatrillion",
            "quatrillions", "schilling", "schillings", "shilling", "shillings", "sigillaire", "sigillaires", "sigillé", "sigillée",
            "sigillées", "sigillés", "thrill", "thriller", "thrillers", "thrills", "till", "tills", "transillumination",
            "transilluminations", "trillion", "trillions", "twill", "vaudeville", "vaudevilles", "vaudevillesque", "vaudevillesques",
            "verticille", "verticilles", "willaya", "willayas", "william", "williams", "agasillis",
            "archimillionnaire", "archimillionnaires", "armillaire", "armillaires", "aspergillaire",
            "aspergillaires", "aspergille", "aspergilles", "willemite", "willémite", "willémites",
            "willi", "williams", "willis", "vexillaire", "vexillaires", "vexille", "vexilles",
            "vexillum", "vexillums", "verticillaire", "verticillé", "verticillée", "verticillées",
            "verticillés", "verticilliose", "verticillioses", "bimillénaire", "bimillénaires", "branchille",
            "branchilles", "branchillon", "branchillons", "boutillier", "boutilliers", "cabecilla", "cabécilla",
            "cabecillas", "cabécillas", "capillacé", "capillacés", "capillacée", "capillacées", "coutillier",
            "coutilliers", "dégobillis",
            "stilla", "stillai", "stillaient", "stillais", "stillait", "stillâmes", "stillant",
            "stillas", "stillasse", "stillassent", "stillasses", "stillassiez", "stillassions",
            "stillât", "stillâtes", "stillation", "stillations", "stille", "stillé", "stillée",
            "stillées", "stillent", "stiller", "stillera", "stillerai", "stilleraient", "stillerais",
            "stillerait", "stilleras", "stillèrent", "stillerez", "stilleriez", "stillerions",
            "stillerons", "stilleront", "stilles", "stillés", "stillez", "stilligoutte", "stilligouttes",
            "stillons","filliole", "fillioles",
            "fritillaire", "gille", "gillotage", "gillotages", "grills", "imbécillifié", "imbécilliser",
            "intermaxillaire", "intermaxillaires", "lapilli", "lapillis", "mamillaire", "mamillaires",
            "maxille", "maxilles", "maxillifère", "maxilliforme", "mille", "millefeuille", "millefeuilles",
            "millepertuis", "milleraies", "millerandage", "millerandages", "millerole", "milleroles",
            "millerolle", "millerolles", "milles", "multimillénaire", "oscilla", "oscillai", "oscillaient",
            "oscillais", "oscillait", "oscillâmes", "oscillant", "oscillante", "oscillantes",
            "oscillants", "oscillas", "oscillasse", "oscillassent", "oscillasses", "oscillassiez",
            "oscillassions", "oscillât", "oscillâtes", "oscillateur", "oscillateurs", "oscillation",
            "oscillations", "oscillatoire", "oscillatoires", "oscillatrice", "oscillatrices",
            "oscille", "oscillé", "oscillée", "oscillées", "oscillement", "oscillent", "osciller",
            "oscillera", "oscillerai", "oscilleraient", "oscillerais", "oscillerait", "oscilleras",
            "oscillèrent", "oscillerez", "oscilleriez", "oscillerions", "oscillerons", "oscilleront",
            "oscilles", "oscillés", "oscillez", "oscilliez", "oscillions", "oscillogramme", "oscillogrammes",
            "oscillographe", "oscillographes", "oscillographie", "oscillographique", "oscillomètre",
            "oscillomètres", "oscillons", "oscilloscope", "oscilloscopes", "papillite", "papillites",
            "papillomateuse", "papillomateuses", "papillomateux", "papillome", "papillomes", "pénicille",
            "pénicillé", "pénicillée", "pénicillées", "pénicilles", "pénicillés", "pénicillinase",
            "pénicillinases", "pénicillium", "pénicilliums", "phosphovanillique", "précapillaire",
            "prémaxillaire", "quadrillion", "quadrillions", "quintillion", "réilluminer", "saxillaire",
            "saxillaires", "scillarène", "scille", "scilles", "scillitique", "scillitiques", "scintillatrice",
            "scintillatrices", "septillion", "sigillographe", "sigillographes", "sigillographie",
            "sigillographies", "sigillographique", "sigillographiques", "spirillose", "spirilloses",
            "spongille", "spongilles", "sugillation", "sugillations", "tefillin", "téfillin",
            "téfillins", "thriller", "thrillers", "tillodontes", "trillionnaire", "twills", "vanilline",
            "vanillines", "vanillisme", "vanillismes",
        };

        /// <summary>
        /// Liste des mots où "oy" se pronoce [oj] et non [waj]
        /// </summary>
        private static HashSet<string> motsOYoj = new HashSet<string>
        {
            "agoyate", "agoyates", "alcoyle", "alcoyles", "arroyo", "arroyos", "benzoyle", "benzoyles",
            "boy", "boys", "boyard", "boyards", "boycott", "boycotts", "boycottage", "boycottages",
            "boycotta", "boycottai", "boycottaient", "boycottais", "boycottait", "boycottant",
            "boycottas", "boycottasse", "boycottassent", "boycottasses", "boycottassiez", "boycottassions",
            "boycotte", "boycotte", "boycotte", "boycotte", "boycottent", "boycottent", "boycotter",
            "boycottera", "boycotterai", "boycotteraient", "boycotterais", "boycotterait", "boycotteras",
            "boycotterez", "boycotteriez", "boycotterions", "boycotterons", "boycotteront", "boycottes",
            "boycottes", "boycottez", "boycottez", "boycottiez", "boycottiez", "boycottions",
            "boycottions", "boycottons", "boycottons", "boycottâmes", "boycottât", "boycottâtes",
            "boycottèrent", "boycottés", "boycotté", "boycottée", "boycottées", "boycotte", "boycottais",
            "boycotterais", "broyon", "cacaoyer", "cacaoyers", "cacaoyère", "cacaoyères", "caloyère",
            "caloyères", "caloyer", "caloyers", "coyau", "coyaux", "coyote", "coyotes", "goy",
            "goys", "goyau", "goyaux", "goyave", "goyaves", "goyavier", "goyaviers", "goyot",
            "goyots", "halloysite", "halloysites", "oyant", "oyant", "oyants", "oyat", "oyats",
            "samoyède", "samoyèdes", "samoyède", "samoyèdes", "yoyo", "yoyos", "alcoylé", "hoya",
            "métahalloysite", "sulfamoyle",
        };

        /// <summary>
        /// Liste des groupes de 6 lettres (ou moins) au début des mots commençant par "re" et où
        /// le 'e' ne se prononce pas [°].
        /// </summary>
        public static HashSet<string> motsRe6 = new HashSet<string>
        {
            "realia", "recta", "rectal", "rectan", "rectas", "rectau", "rectem", "recteu", "rectic",
            "rectid", "rectif", "rectig", "rectil", "rectim", "rectin", "rectio", "rectir", "rectis",
            "rectit", "rectiu", "recto", "rectoc", "rector", "rectos", "rectri", "rectum", "reddit",
            "redowa", "reflex", "reg", "regenc", "reggae", "regs", "reichs", "rein", "reine",
            "reines", "reinet", "reins", "reinté", "reis", "reïs", "reître", "-relaxe", "remake",
            "rembai", "rembal", "rembar", "rembla", "rembob", "remboî", "rembou", "rembra", "rembru",
            "rembuc", "rembûc", "remmai", "remman", "remmen", "remmèn", "rempai", "rempar", "rempié",
            "rempiè", "rempil", "rempla", "rempli", "remplî", "remplo", "remplu", "rempoc", "rempoi",
            "rempor", "rempot", "rencai", "rencar", "rencha", "renché", "renclo", "renclô", "rencog",
            "rencoi", "rencon", "rencou", "rend", "rendai", "rendan", "rende", "rendem", "renden",
            "rendes", "rendeu", "rendez", "rendie", "rendîm", "rendio", "rendir", "rendis", "rendit",
            "rendît", "rendon", "rendor", "rendra", "rendre", "rendri", "rendro", "rends", "rendu",
            "rendue", "rendus", "renfaî", "renfer", "renfié", "renfil", "renfla", "renflâ", "renfle",
            "renflé", "renflè", "renfli", "renflo", "renflu", "renfon", "renfor", "renfro", "rengag",
            "rengai", "rengor", "rengra", "rengré", "rengrè", "renne", "rennes", "renqui", "rensei",
            "renta", "rentab", "rentai", "rentâm", "rentan", "rentas", "rentât", "rente", "renté",
            "rentée", "renten", "renter", "rentèr", "rentes", "rentés", "rentez", "rentie", "rentiè",
            "rentio", "rentoi", "renton", "rentra", "rentrâ", "rentre", "rentré", "rentrè", "rentri",
            "rentro", "rentru", "renver", "renvi", "renvia", "renviâ", "renvid", "renvie", "renvié",
            "renviè", "renvii", "renvio", "renvis", "renvoi", "renvoy", "reps", "reptat", "reptif",
            "reptil", "-requie", "rescap", "rescis", "rescou", "rescri", "respec", "respir",
            "resple", "respon", "resqui", "ressui", "ressus", "ressuy",
            "resta", "restag", "restai", "restâm", "restan", "restas", "restât", "restau", "reste",
            "resté", "restée", "resten", "rester", "restèr", "restes", "restés", "restez", "restie",
            "restio", "restit", "reston", "restre", "restri", "rets", "revolv", "rewrit", "rexism",
            "rez", "rezzou",
        };

    } // class AutomRuleFilter
} // namespace ColorLib
