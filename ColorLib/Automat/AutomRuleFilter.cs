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
        private delegate bool CheckRuleFunction(string mot, int posMot);
        static private Dictionary<string, CheckRuleFunction> checkRuleFs = new Dictionary<string, CheckRuleFunction>(11)
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
            {"Regle_ill", Regle_ill}
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
                    Debug.Assert(s[pos] == '\'', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRuleFilter, on attend un \''\' en début de règle.", pos, s));
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
                        throw new ArgumentException(String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \'+\' ou un \'+\'.", pos));
                    //let's find the '
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == '\'', String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \''\' après + ou -.", pos));
                    //let's find the :
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == ':', String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \':\' après + ou -.", pos));
                    //let's find the /
                    pos = GetNextChar(pos + 1);
                    Debug.Assert(s[pos] == '/', String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \'/\' avant une regex", pos));
                    // let's load the regex
                    // Let's assume that there is no '/' in the regex itself. Else we would need to handle the escape character that is necessary in .js
                    pos = GetNextChar(pos + 1);
                    int endOfRegexSlashPos = s.IndexOf('/', pos);
                    Debug.Assert(endOfRegexSlashPos > pos, String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \'/\' pour clore une regex", pos));
                    Debug.Assert(s[pos - 1] != '\\', String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, le cas d\'un \\ n'est pas traité.", pos));
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
                                sb.Append(@"^"); // The match must occur at the start of the string. It is not in the table due to the algorithm of Marie-Pierre
                            sb.Append(theExpression);
                        }
                        Debug.Assert(follRegEx == null, "prevRegEx must be null");
                        follRegEx = new Regex(sb.ToString(), RegexOptions.Compiled);
                    }
                    else
                    {
                        sb.Append(theExpression);
                        if ((theExpression[0] == '^') && (theExpression.Length == 1))
                            // A '^' alone means that the letter must be at the begining of the string - Special semantics defined by Marie-Pierre
                            isFirstLetter = true;
                        else if ((theExpression.Length >= 1) && (theExpression[theExpression.Length - 1] != '$'))
                            sb.Append("$"); // The match must occur at the end of the string. This $ is not in the regexes in the table, but could be there as well.
                        Debug.Assert(prevRegEx == null, "prevRegEx must be null");
                        prevRegEx = new Regex(sb.ToString(), RegexOptions.Compiled);
                    }
                    // let's find the i
                    pos = GetNextChar(endOfRegexSlashPos + 1);
                    Debug.Assert(s[pos] == 'i', String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend un \'i\' après une regex", pos));
                    // let's find the next character
                    pos = GetNextChar(pos + 1);
                    // it is either ',' or '}'
                    Debug.Assert(((s[pos] == ',') || (s[pos] == '}')),
                        String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend une \',\' entre deux DirectionRegex ou un \'}}\'", pos));
                    if (s[pos] == ',')
                        pos = GetNextChar(pos + 1);
                } // while
            }
            else  // it is a "regle" procedure
            {
                // the "regle" starts with "this."
                // let's find the dot
                var endOfThisDot = s.IndexOf('.', pos);
                Debug.Assert(endOfThisDot > 0, String.Format(BaseConfig.cultF, "AutomRuleFiletr: la pos {0} de {1} doit être suivie d'un \'.\' pour délimiter \'this\'",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                var thisTxt = s.Substring(pos, endOfThisDot - pos);
                Debug.Assert(thisTxt == "this", "La référence à une fonction de règle doit commencer par \"this\".");
                pos = endOfThisDot;
                // let's find the comma that terminates the name of the regle function
                pos = GetNextChar(pos + 1);
                var endOfNameComma = s.IndexOf(',', pos);
                var thisName = s.Substring(pos, endOfNameComma - (pos)).Trim();
                Debug.Assert(endOfNameComma > pos, String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, on attend une \',\' après le nom de fonction", pos));
                var found = checkRuleFs.TryGetValue(thisName, out crf);
                Debug.Assert(found, String.Format(BaseConfig.cultF, "La pos {0} n'est pas un AutomRuleFilter, {1} n'est pas un nom valide", pos, thisName));
                pos = endOfNameComma-1; // pos points to the last char before the comma
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
            if (!initiated)
            {
                for (int i = 0; i < verbes_ier.Length; i++)
                    verbes_ier_hashed.Add(verbes_ier[i], null);
                for (int i = 0; i < mots_ent.Length; i++)
                    mots_ent_hashed.Add(mots_ent[i], null);
                for (int i = 0; i < verbes_mer.Length; i++)
                    verbes_mer_hashed.Add(verbes_mer[i], null);
                for (int i = 0; i < exceptions_final_er.Length; i++)
                    exceptions_final_er_hashed.Add(exceptions_final_er[i], null);
                for (int i = 0; i < noms_ai.Length; i++)
                    noms_ai_hashed.Add(noms_ai[i], null);
                for (int i = 0; i < avoir_eu.Length; i++)
                    avoir_eu_hashed.Add(avoir_eu[i], null);
                for (int i = 0; i < mots_s_final.Length; i++)
                    mots_s_final_hashed.Add(mots_s_final[i], null);
                for (int i = 0; i < mots_t_final.Length; i++)
                    mots_t_final_hashed.Add(mots_t_final[i], null);
                for (int i = 0; i < mots_d_final.Length; i++)
                    mots_d_final_hashed.Add(mots_d_final[i], null);
                for (int i = 0; i <except_ill.Length; i++)
                    except_ill_hashed.Add(except_ill[i], null);
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
            if (s[s.Length - 1] == 's')
                return s.Substring(0, s.Length - 1);
            else
                return s;
        } // SansSFinal


        // Ensemble de verbes qui se terminent par -ier !! attention : pas d'accents !!
        private static string[] verbes_ier = {"affilier","allier","amnistier","amplifier","anesthesier","apparier",
            "approprier","apprecier","asphyxier","associer","atrophier","authentifier","autographier",
            "autopsier","balbutier","bonifier","beatifier","beneficier","betifier","calligraphier","calomnier",
            "carier","cartographier","certifier","charrier","chier","choregraphier","chosifier","chatier",
            "clarifier","classifier","cocufier","codifier","colorier","communier","conchier","concilier",
            "confier","congedier","contrarier","copier","crier","crucifier","dactylographier",
            "differencier","disgracier","disqualifier","dissocier","distancier","diversifier","domicilier",
            "decrier","dedier","defier","deifier","delier","demarier","demultiplier","demystifier","denazifier",
            "denier","deplier","deprecier","dequalifier","dévier","envier","estropier","excommunier",
            "exemplifier","exfolier","expatrier","expier","exproprier","expedier","extasier","falsifier",
            "fier","fluidifier","fortifier","frigorifier","fructifier","gazeifier","glorifier","gracier",
            "gratifier","horrifier","humidifier","humilier","identifier","incendier","ingenier","initier",
            "injurier","intensifier","inventorier","irradier","justifier","licencier","lier","liquefier",
            "lubrifier","magnifier","maleficier","manier","marier","mendier","modifier","momifier","mortifier",
            "multiplier","mystifier","mythifier","mefier","nier","notifier","negocier","obvier","officier",
            "opacifier","orthographier","oublier","pacifier","palinodier","pallier","parier","parodier",
            "personnifier","photocopier","photographier","plagier","planifier","plastifier","plier","polycopier",
            "pontifier","prier","privilegier","psalmodier","publier","purifier","putrefier","pepier","petrifier",
            "qualifier","quantifier","radier","radiographier","rallier","ramifier","rapatrier","rarefier",
            "rassasier","ratifier","razzier","recopier","rectifier","relier","remanier","remarier",
            "remercier","remedier","renier","renegocier","replier","republier","requalifier","revivifier",
            "reverifier","rigidifier","reconcilier","recrier","reexpedier","refugier","repertorier","repudier",
            "resilier","reunifier","reedifier","reetudier","sacrifier","salarier","sanctifier","scier",
            "signifier","simplifier","skier","solidifier","soucier","spolier","specifier","statufier","strier",
            "stupefier","supplicier","supplier","serier","terrifier","tonifier","trier","tumefier",
            "typographier","telegraphier","unifier","varier","versifier","vicier","vitrifier","vivifier",
            "verifier","echographier","ecrier","edifier","electrifier","emulsifier","epier","etudier"};

        private static StringDictionary verbes_ier_hashed = new StringDictionary();
        
        //Règle spécifique de traitement des successions de lettres finales 'ient'
        //sert à savoir si la séquence 'ient' se prononce [i][_muet] ou [j][e_tilda]
        // pour nous, pos_mot correspond à la position de la lettre examinéée. Chez Marie-Pierre, c'est la position après la lettre examinée.

        // Précondition: mot est en minuscules

        public static bool Regle_ient(string mot, int pos_mot)
        {
            Debug.Assert(mot != null); 
            Debug.Assert((pos_mot >=0) && (pos_mot < mot.Length));

            bool toReturn = false;
            if (pos_mot >= mot.Length - 4) // checking i or e from the "ient" any previous letter in the word would not fit.
            {
                Regex r = new Regex("[bcçdfghjklnmpqrstvwxz]ient$", RegexOptions.IgnoreCase);
                if (r.IsMatch(mot)) // le mot doit se terminer pas par 'ient' (précédé d'une consonne)
                {
                    // il faut savoir si le mot est un verbe dont l'infinitif se termine par 'ier' ou non
                    string pseudo_infinitif = ChaineSansAccents(mot.Substring(0, mot.Length - 2) + 'r');
                    toReturn = verbes_ier_hashed.ContainsKey(pseudo_infinitif);
                }

                // Je ne comprends pas le code suivant car je n'ai pas trouve le code de pretraitement de texte.
                // Un déterminant élidé serait un l'. Je ne vois pas bien quand on aurait un cas pareil. Ca dépend de la coupure en mots
                // et il paraît logique de couper "l'animal" en "l'" et "animal"... A voir...
                //
                //pseudo_infinitif = chaine_sans_accent(mot).substring(0, mot.length - 2) + 'r';
                //if ((pseudo_infinitif.length > 1) && (pseudo_infinitif[1] == '@'))
                //{
                //    // mot précédé d'un déterminant élidé - codage de l'apostrophe : voir pretraitement_texte
                //    pseudo_infinitif = pseudo_infinitif.slice(2);
                //}
                //return (verbes_ier.indexOf(pseudo_infinitif) >= 0);
            }
            return toReturn;
        }


        static string[] mots_ent =
        {
            "absent", "abstinent", "accent", "accident", "adhérent", "adjacent",
            "adolescent", "afférent", "agent", "ambivalent", "antécédent", "apparent",
            "arborescent", "ardent", "argent", "arpent", "astringent", "auvent",
            "avent", "cent", "chiendent", "client", "coefficient", "cohérent", "dent",
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
            "prévalent", "pschent", "purulent", "putrescent", "pénitent", "quotient",
            "relent", "récent", "récipient", "récurrent", "référent", "régent", "rémanent",
            "réticent", "sanguinolent", "sergent", "serpent", "somnolent", "souvent",
            "spumescent", "strident", "subconscient", "subséquent", "succulent", "tangent",
            "torrent", "transparent", "trident", "truculent", "tumescent", "turbulent",
            "turgescent", "urgent", "vent", "ventripotent", "violent", "virulent", "effervescent",
            "efficient", "effluent", "engoulevent", "entregent", "escient", "event",
            "excédent", "expédient", "éloquent", "éminent", "émollient", "évanescent", "évent",
            "agrément", "aliment", "ciment","content","compliment","boniment","document",
            "parlement","ornement","supplément","tourment","spent", "argument"
        };

        private static StringDictionary mots_ent_hashed = new StringDictionary();
        /*
         * Règle spécifique de traitement des successions de lettres '*ent'
         * sert à savoir si le mot figure dans les mots qui se prononcent a_tilda à la fin
         * true si c'est le cas.
         * Attention les mots en "ment" sont traités ailleurs.
         * 
         * Précondition: mot est en minuscules
         */
        public static bool Regle_mots_ent (string mot, int pos_mot)
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            string comparateur = SansSFinal(mot);
            if (pos_mot >= comparateur.Length - 3) // on teste a priori le e de la terminaison
            {
                Regex r = new Regex("^[bcdfghjklmnpqrstvwxz]ent$", RegexOptions.IgnoreCase); // mot court, évtlmt imaginaire...
                if (r.IsMatch(comparateur))
                    toReturn = true;
                else
                    toReturn = mots_ent_hashed.ContainsKey(comparateur);
            }
            return toReturn;
        }

        static string[] verbes_mer =
        {
            "abimer","acclamer","accoutumer","affamer","affirmer","aimer",
            "alarmer","allumer","amalgamer","animer","armer","arrimer","assommer","assumer",
            "blasphemer","blamer","bramer","brimer","calmer","camer","carmer","charmer",
            "chloroformer","chomer","clamer","comprimer","confirmer","conformer","consommer",
            "consumer","costumer","cramer","cremer","damer","diffamer","diplomer","decimer",
            "declamer","decomprimer","deformer","degommer","denommer","deplumer","deprimer",
            "deprogrammer","desaccoutumer","desarmer","desinformer","embaumer","embrumer",
            "empaumer","enfermer","enflammer","enfumer","enrhumer","entamer","enthousiasmer",
            "entraimer","envenimer","escrimer","estimer","exclamer","exhumer","exprimer",
            "fantasmer","fermer","filmer","flemmer","former","frimer","fumer","gendarmer",
            "germer","gommer","grammer","grimer","groumer","humer","imprimer","infirmer",
            "informer","inhumer","intimer","lamer","limer","legitimer","mimer","mesestimer",
            "nommer","opprimer","palmer","parfumer","parsemer","paumer","plumer","pommer",
            "primer","proclamer","programmer","preformer","prenommer","presumer","pamer",
            "perimer","rallumer","ramer","ranimer","refermer","reformer","refumer","remplumer",
            "renfermer","renommer","rentamer","reprogrammer","ressemer","retransformer","rimer",
            "rythmer","reaccoutumer","reaffirmer","reanimer","rearmer","reassumer","reclamer",
            "reimprimer","reprimer","resumer","retamer","semer","slalomer","sommer",
            "sublimer","supprimer","surestimer","surnommer","tramer","transformer",
            "trimer","zoomer","ecremer","ecumer","elimer", "dormer" // "dormer" est là pour intercepter le cas de ils/elles dorment
        };

        private static StringDictionary verbes_mer_hashed = new StringDictionary();

        /*
         * Règle spécifique de traitement des successions de lettres 'ment'
         * sert à savoir si le mot figure dans les mots qui se prononcent a_tilda à la fin
         * on vérifie s'il s'agit d'un verbe en mer. Si c'est le cas on retourne false sinon true
         * on considère que les mots terminés par 'ment' se prononcent [a_tilda] sauf s'il s'agit d'un verbe
         * 
         * Précondition: mot est en minuscules
         */
        public static bool Regle_ment(string mot, int pos_mot)
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            Regex r = new Regex("ment$", RegexOptions.IgnoreCase); // le mot doir finir par ment
            if ((pos_mot >= mot.Length - 3) && (r.IsMatch(mot))) // on n'est pas en train de traiter une lettre avant la terminaison
            {
                string pseudo_infinitif = ChaineSansAccents(mot.Substring(0, mot.Length - 2) + 'r');
                toReturn = !verbes_mer_hashed.ContainsKey(pseudo_infinitif);
            }
            return toReturn;
        }

        // retoune true si la terminaison en ment correspond à un verbe. 
        // quasiment l'inverse de Regle_ment
        public static bool Regle_verbe_mer(string mot, int pos_mot)
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            Regex r = new Regex("ment$", RegexOptions.IgnoreCase); // le mot doir finir par ment
            if ((pos_mot >= mot.Length - 3) && (r.IsMatch(mot))) // on n'est pas en train de traiter une lettre avant la terminaison
            {
                string pseudo_infinitif = ChaineSansAccents(mot.Substring(0, mot.Length - 2) + 'r');
                toReturn = verbes_mer_hashed.ContainsKey(pseudo_infinitif);
            }
            return toReturn;
        }


        static string[] exceptions_final_er =
        {
            "amer", "cher", "hier", "mer", "coroner", "charter", "cracker",
            "chester", "doppler", "cascher", "bulldozer", "cancer", "carter", "geyser", "cocker", "pullover",
            "alter", "aster", "fer", "ver", "diver", "perver", "enfer", "traver", "univer", "cuiller", "container",
            "cutter", "révolver", "super", "master"
        };

        private static StringDictionary exceptions_final_er_hashed = new StringDictionary();

        /*
         * Règle spécifique de traitement des successions de lettres finales 'er'
         * sert à savoir si le mot figure dans la liste des exceptions
         * qui ne se prononcent pas é
         * 
         * Précondition: mot est en minuscules et non null
         */
        public static bool Regle_er (string mot, int pos_mot)
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 'e'); // sinon un 's' seul ferait sauter la banque plus bas...

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if ((pos_mot >= mSing.Length - 2) && (mSing[mSing.Length - 1] == 'r')) 
                // on n'est pas en train de traiter une lettre avant la terminaison
                // le mot se termine par 'r'
                toReturn = exceptions_final_er_hashed.ContainsKey(mSing);
            return toReturn;
        }

        static string[] noms_ai =
        {
            "balai", "brai", "chai", "déblai", "délai", "essai", "frai", "geai", "lai", "mai",
            "minerai", "papegai", "quai", "rai", "remblai"
        };

        private static StringDictionary noms_ai_hashed = new StringDictionary();

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
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            if ((pos_mot == mot.Length - 2) && (mot[mot.Length-1] == 'i'))
                toReturn = noms_ai_hashed.ContainsKey(mot);
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
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));

            bool toReturn = false;
            if ((pos_mot == 0) && (mot.Length > 1) && ((mot[1] == 'u') || (mot[1] == 'û')))
                toReturn = avoir_eu_hashed.ContainsKey(mot);
            return toReturn;
        }

        static string[] mots_s_final =
        {
            "abribus","airbus","autobus","bibliobus","bus","nimbus","gibus",
            "microbus","minibus","mortibus","omnibus","oribus", "pédibus", "quibus","rasibus",
            "rébus","syllabus","trolleybus","virus","antivirus","anus","asparagus","médius",
            "autofocus","focus","benedictus","bonus","campus","cirrus","citrus",
            "collapsus","consensus","corpus","crochus","crocus","crésus","cubitus","humérus",
            "diplodocus","eucalyptus","erectus","hypothalamus","mordicus","mucus","stratus",
            "nimbostratus","nodus","modus","opus","ours","papyrus","plexus","plus","processus","prospectus",
            "lapsus","prunus","quitus","rétrovirus","sanctus","sinus","solidus","liquidus",
            "stimulus","stradivarius","terminus","tonus","tumulus","utérus","versus","détritus",
            "ratus","couscous", "burnous", "tous","anis","bis","anubis",
            "albatros","albinos","calvados","craignos","mérinos","rhinocéros","tranquillos","tétanos","os",
            "alias","atlas","hélas","madras","sensas","tapas","trias","vasistas","hypocras","gambas","as",
            "biceps","quadriceps","chips","relaps","forceps","schnaps","laps","oups","triceps","princeps",
            "tricératops"
        };

        private static StringDictionary mots_s_final_hashed = new StringDictionary();

        /*
         * Règle spécifique de traitement des mots qui se terminent par "s".
         * Pour un certain nombre de ces mots, le 's' final se prononce. true dans ce cas.
         */
        public static bool Regle_s_final(string mot, int pos_mot)
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 's');

            bool toReturn = false;
            if (pos_mot == mot.Length - 1)
                toReturn = mots_s_final_hashed.ContainsKey(mot);
            return toReturn;
        } // Regle_s_final


        static string[] mots_t_final =
        {
            "accessit","cet","but","diktat","kumquat","prurit","affidavit","dot","rut","audit",
            "exeat","magnificat","satisfecit","azimut","exit","mat","scorbut","brut",
            "fiat","mazout","sinciput","cajeput","granit","net","internet","transat","sept",
            "chut","huit","obit","transit","coït","incipit","occiput","ut","comput",
            "introït","pat","zut","déficit","inuit","prétérit", "uppercut",
            "gadget","kilt","kit","scout","fret"
        };

        private static StringDictionary mots_t_final_hashed = new StringDictionary();

        /*
         * Règle spécifique de traitement des mots qui se terminent par la lettre "t" prononcée.
         */
        public static bool Regle_t_final(string mot, int pos_mot)
            // pos_mot pointe sur un 't'
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 't'); 

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if (pos_mot == mSing.Length - 1)
                toReturn = mots_t_final_hashed.ContainsKey(mSing);
            return toReturn;
        } // Regle_t_final


        /*
         * Règle spécifique de traitement des mots contenant "tien" et où le t se prononce t
         */
        public static bool Regle_tien(string mot, int pos_mot)
        // pos_mot pointe sur un 't'
        // Cette fonnction n'est pas tout à fait dans la philosophie de l'automate.
        // On aurait pu isoler les deux cas à l'aide de règles et ne traîter ici que
        // les exceptions. --> À faire... à l'occasion
        {
            Debug.Assert(mot != null);
            Debug.Assert((pos_mot >= 0) && (pos_mot < mot.Length));
            Debug.Assert(mot[pos_mot] == 't'); // pas vraiment nécessaire, mais autre chose n'a pas de sens.

            string mSing = SansSFinal(mot);
            bool toReturn = false;
            Regex r;

            // vérifions que le 't' se trouve bien au début de "tien"
            if ((mSing.Length - pos_mot >= 4) 
                && (mSing[pos_mot + 1] == 'i') 
                && (mSing[pos_mot + 2] == 'e')
                && (mSing[pos_mot + 3] == 'n')) {

                // vérifions si le 'tien se trouve  au début du mot
                if (pos_mot == 0)
                    toReturn = true; // tous les mots commençant par 'tien' ---> 't'
                else
                {
                    r = new Regex(".+[befhns]tien.*"); // hypothèse: il n'existe pas de mot contenant deux fois "tien"
                    if (r.IsMatch(mSing))
                        toReturn = true;
                    else
                    {
                        // il reste les exceptions qui commencent par "chrétien","soutien", "appartien", "détien"
                        toReturn = (mot.StartsWith("chré") || mot.StartsWith("sou") || mot.StartsWith("appar") || mot.StartsWith("dé")) ;
                    }
                }
            }
            return toReturn;
        } // Regle_tien


        static string[] mots_d_final =
        {
            "apartheid", "aïd", "background", "barmaid", "baroud", "band", "bled", "caïd", "celluloïd", "damned", 
            "djihad", "kid", "fjord", "hard", "jihad", "lad", "lord", "sud", "oued", "pad", "plaid", "polaroid", "polaroïd",
            "rhodoïd", "shetland", "board", "skateboard", "skinhead", "steward", "tabloïd", "end"
        };

        private static StringDictionary mots_d_final_hashed = new StringDictionary();

        public static bool Regle_finD(string mot, int pos_mot)
            // retourne true si on est sur le d final et le mot se termine par un 'd' qui se prononce
        {
            Debug.Assert(mot[pos_mot] == 'd', "Regle_finD: on attend un 'd'");

            string mSing = SansSFinal(mot);

            bool toReturn = false;
            if (pos_mot == mSing.Length -1)
                toReturn = mots_d_final_hashed.ContainsKey(mSing);
            return toReturn;
        }

        static string[] except_ill = // les mots où ill se pronoce [il] et non [j] ou [ij]
        {
            "achille", "achilles", "achillée", "achillées", "ancillaire", "ancillaires", "armadille", "armadilles", "aspergillose", 
            "aspergilloses", "aspergillus", "axillaire", "axillaires", "bacillaire", "bacillaires", "bacille", "bacilles", 
            "bellevillois", "bellevilloise", "bellevilloises", "bidonville", "bidonvilles", "bill", "billevesée", 
            "billevesées", "billion", "billions", "bills", "bougainvillée", "bougainvillées", "bougainvillier", "bougainvilliers", 
            "calville", "calvilles", "canetille", "canetilles", "capillaire", "capillaires", "capillarité", "capillarités", 
            "capilliculteur", "capilliculteurs", "caterpillar", "chinchilla", "chinchillas", "cochenille", "cochenilles", "codicille", 
            "codicilles", "colibacille", "colibacilles", "cyrillique", "cyrilliques", "défibrillateur", "défibrillateurs", 
            "défibrillation", "défibrillations", "défibriller", "défibrille", "défibrilles", "défibrillons", "défibrillez", 
            "défibrillent", "défibrillé", "défibrillais", "défibrillait", "défibrillions", "défibrilliez", "défibrillaient", 
            "défibrillai", "défibrillas", "défibrilla", "défibrillâmes", "défibrillâtes", "défibrillèrent", "défibrillerai", 
            "défibrilleras", "défibrillera", "défibrillerons", "défibrillerez", "défibrilleront", "défibrillerais", "défibrillerait", 
            "défibrillerions", "défibrilleriez", "défibrilleraient", "défibrillasse", "défibrillasses", "défibrillât", 
            "défibrillassions", "défibrillassiez", "défibrillassent", "défibrillant", "défibrillée", "défibrillées", 
            "désillusion", "désillusionné", "désillusionnés", "désillusionnement", "désillusionnements", "désillusionner", 
            "désillusions", "distillat", "distillateur", "distillation", "distillations", "distiller", "distille", "distilles", 
            "distillons", "distillez", "distillent", "distillé", "distillais", "distillait", "distillions", "distilliez", 
            "distillaient", "distillai", "distillas", "distilla", "distillâmes", "distillâtes", "distillèrent", "distillerai", 
            "distilleras", "distillera", "distillerons", "distillerez", "distilleront", "distillerais", "distillerait", 
            "distillerions", "distilleriez", "distilleraient", "distillasse", "distillasses", "distillât", "distillassions", 
            "distillassiez", "distillassent", "distillant", "distillée", "distillées", "drill", "fibrillation", "fibrillations", 
            "fringillidé", "fritillaires", "gilles", "grill", "imbécillité", "imbécillités", "instiller", "instille", "instilles", 
            "instillons", "instillez", "instillent", "instillé", "instillais", "instillait", "instillions", "instilliez", 
            "instillaient", "instillai", "instillas", "instilla", "instillâmes", "instillâtes", "instillèrent", "instillerai", 
            "instilleras", "instillera", "instillerons", "instillerez", "instilleront", "instillerais", "instillerait", 
            "instillerions", "instilleriez", "instilleraient", "instillasse", "instillasses", "instillât", "instillassions", 
            "instillassiez", "instillassent", "instillant", "instillée", "instillées", "killer", "killers", "krill", "krills", 
            "lilliputien", "lilliputienne", "lilliputiennes", "lilliputiens", "lillois", "lilloise", "lilloises", "mandrill", 
            "mandrills", "maxillaire", "maxillaires", "multimilliardaire", "multimilliardaires", "multimillionnaire", 
            "multimillionnaires", "papillaire", "papillaires", "pénicilline", "pénicillines", "pupillaire", "pupillaires", 
            "pupillarité", "pupillarités", "pusillanime", "pusillanimes", "pusillanimité", "pusillanimités", "quatrillion", 
            "quatrillions", "schilling", "schillings", "shilling", "shillings", "sigillaire", "sigillaires", "sigillé", "sigillée", 
            "sigillées", "sigillés", "thrill", "thriller", "thrillers", "thrills", "till", "tills", "transillumination", 
            "transilluminations", "trillion", "trillions", "twill", "vaudeville", "vaudevilles", "vaudevillesque", "vaudevillesques", 
            "verticille", "verticilles", "willaya", "willayas", "william", "williams"
        };

        private static StringDictionary except_ill_hashed = new StringDictionary();

        /// <summary>
        /// Vérifie si le mot est une exception pour les lettres ill qui se prononcent [il]. La méthode peut
        /// être appelée pour le i de "ill" et pour le prmier 'l' de "ill"
        /// </summary>
        /// <param name="mot">Le mot à vérifier</param>
        /// <param name="pos_mot">la position (basée sur zéro) de la lettre dans le mot qu'on est en train d'étudier</param>
        /// <returns></returns>
        public static bool Regle_ill(string mot, int pos_mot)
        {
            bool condMet = false;
            if (mot[pos_mot] == 'i')
                condMet = ((pos_mot < mot.Length - 2) && (mot[pos_mot + 1] == 'l') && (mot[pos_mot + 2] == 'l'));
            else if (mot[pos_mot] == 'l')
                condMet = ((pos_mot > 0) && (mot[pos_mot - 1] == 'i') && (pos_mot < mot.Length - 1) && (mot[pos_mot + 1] == 'l'));
            return (condMet && except_ill_hashed.ContainsKey(mot));
        }

        public bool Check (PhonWord pw, int pos, string firstPart, string secondPart)
        {
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

    } // class AutomRuleFilter
} // namespace ColorLib
