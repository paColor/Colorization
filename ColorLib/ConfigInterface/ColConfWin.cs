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
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ColorLib
{

    public class SonConfigModifiedEventArgs : EventArgs
    {
        public string son { get; private set; }
        public PhonConfType pct { get; private set; }

        public SonConfigModifiedEventArgs(string inSon, PhonConfType inPCT)
        {
            son = inSon;
            pct = inPCT;
        }
    }


    public class PhonConfModifiedEventArgs: EventArgs
    {
        public PhonConfType pct;
        public PhonConfModifiedEventArgs(PhonConfType inPCT)
        {
            pct = inPCT;
        }
    }

    /// <summary>
    /// Les différemtes couleurs qui sont utilisées dans la configuration CERAS. Le but est de 
    /// pouvoir utiliser une de ces valeurs pour accéder à la couleur ou au 
    /// <see cref="CharFormatting"/> correspondant en l'utilisant comme index dans une des tables
    /// définies dans cette classe.
    /// </summary>
    /// <example>
    /// <code>
    /// new CharFormatting(true, false, false, false, true, predefinedColors[(int)CERASColors.CERAS_oi],
    ///         false, predefinedColors[(int)PredefCols.neutral]));
    ///         
    /// SetCFSon("oin", predefCF[(int)CERASColors.CERAS_oin]);
    /// </code>
    /// </example>
    /// /// <remarks>
    /// <see cref="CERASColor"/> et <see cref="PredefCol"/> sont deux manières différentes
    /// d'indexer les mêmes tableaux. Les deux énumérés doivent donc absolument correspondre.
    /// Faire très attention en cas de modifications!
    /// </remarks>
    [Serializable]
    public enum CERASColor { CERAS_oi, CERAS_o, CERAS_an, CERAS_5, CERAS_E, CERAS_e, CERAS_u, CERAS_on, CERAS_eu,
        CERAS_oin, CERAS_muet, CERAS_rosé, CERAS_ill,
    }

    /// <summary>
    /// Liste de couleurs prédéfinies. Comme pour <see cref="CERASColor"/>, l'idée est d'utiliser
    /// les valeurs de cet énuméré comme indice dans les tableaux <c>predefCF</c> et 
    /// <c>predefinedColors</c>
    /// </summary>
    /// <remarks>
    /// <see cref="CERASColor"/> et <see cref="PredefCol"/> sont deux manières différentes
    /// d'indexer les mêmes tableaux. Les deux énumérés doivent donc absolument correspondre.
    /// On a donc ici les couleurs utilisées dans la configuration CERAS.
    /// Faire très attention en cas de modifications!
    /// </remarks>
    /// <example>
    /// <code>
    /// SetCFSon(son, predefCF[(int)PredefCols.black]);
    /// toR.Font.Fill.ForeColor.RGB = ColConfWin.predefinedColors[(int)PredefCols.black];
    /// </code>
    /// </example>
    [Serializable]
    public enum PredefCol { black, darkYellow, orange, darkGreen, violet, darkBlue, red, brown, blue, turquoise, grey, pink, 
        frogGreen,
        pureBlue, neutral, lightBlue, darkRed, white
    }

    /// <summary>
    /// <para>
    /// Classe pour la gestion des éléments de configuration nécessaires pour la colorisation des
    /// phonèmes. C'est ici qu'on trouve la définition des "sons" qui sont utilisés par l'interface
    /// utilisateur. On trouve en particulier le 'mapping' entre les sons et les 
    /// <see cref="Phonemes"/>. La table correspondante (<c>sonMap</c>) est de fait la définition
    /// formelle des sons et de leur nom, tels qu'ils sont acceptés par le programme.
    /// </para>
    /// <para>
    /// Pour chaque son on a un flag (checkbox) qui indique si la colorisation pour ce son est
    /// activée et un <see cref="CharFormatting"/> à utiliser si le flag correspondant est mis.
    /// Les méthodes principales permettent de lire et d'écrire ces deux paramètres pour chaque
    /// son.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Pour que la documentation soit complète, voici une liste des sons telle qu'on la trouve
    /// dans le code. Il est à noter cepenant, que formellemnt, seule la liste dans <c>sonMap</c> 
    /// (qui est un tableau 'private') fait foi.
    /// <code>
    /// private static Dictionary<string, string[]> sonOutMap = new Dictionary<string, string[]> (nrSons)
    /// {
    ///     {"a",   new string[2] {"[a]",   "ta, plat"  } },
    ///     {"q",   new string[2] {"[e]",   "le"        } },
    ///     {"i",   new string[2] {"[i]",   "il, lit"   } },
    ///     {"y",   new string[2] {"[y]",   "tu, lu"    } },
    ///     {"1",   new string[2] {"[1]",   "parfum"    } },
    ///     {"u",   new string[2] {"[u]",   "cou, roue" } },
    ///     {"é",   new string[2] {"[é]",   "né, été"   } },
    ///     {"o",   new string[2] {"[o]",   "mot, eau"  } },
    ///     {"è",   new string[2] {"[è]",   "sel"       } },
    ///     {"an",  new string[2] {"[@]",   "grand"     } },
    ///     {"on",  new string[2] {"[§]",   "son"       } },
    ///     {"2",   new string[2] {"[2]",   "feu, oeuf" } },
    ///     {"oi",  new string[2] {"[oi]",  "noix"      } },
    ///     {"5",   new string[2] {"[5]",   "fin"       } },
    ///     {"w",   new string[2] {"[w]",   "kiwi"      } },
    ///     {"j",   new string[2] {"[j]",   "payer"     } },
    ///     {"ill", new string[2] {"[ill]", "feuille"   } },
    ///     {"ng",  new string[2] {"[ng]",  "parking"   } },
    ///     {"gn",  new string[2] {"[gn]",  "ligne"     } },
    ///     {"l",   new string[2] {"[l]",   "aller"     } },
    ///     {"v",   new string[2] {"[v]",   "veau"      } },
    ///     {"f",   new string[2] {"[f]",   "effacer"   } },
    ///     {"p",   new string[2] {"[p]",   "papa"      } },
    ///     {"b",   new string[2] {"[b]",   "bébé"      } },
    ///     {"m",   new string[2] {"[m]",   "pomme"     } },
    ///     {"z",   new string[2] {"[z]",   "zoo"       } },
    ///     {"s",   new string[2] {"[s]",   "scie"      } },
    ///     {"t",   new string[2] {"[t]",   "tortue"    } },
    ///     {"d",   new string[2] {"[d]",   "dindon"    } },
    ///     {"ks",  new string[2] {"[ks]",  "rixe"      } },
    ///     {"gz",  new string[2] {"[gz]",  "examen"    } },
    ///     {"r",   new string[2] {"[r]",   "rare"      } },
    ///     {"n",   new string[2] {"[n]",   "Nicole"    } },
    ///     {"ge",  new string[2] {"[ge]",  "jupe"      } },
    ///     {"k",   new string[2] {"[k]",   "coq"       } },
    ///     {"g",   new string[2] {"[g]",   "gare"      } },
    ///     {"ch",  new string[2] {"[ch]",  "chat"      } },
    ///     {"ij",  new string[2] {"[ij]",  "pria"      } },
    ///     {"oin", new string[2] {"[oin]", "soin"      } },
    ///     {"_muet", new string[2] {"[#]", "\'muet\'"  } },
    ///     {"q_caduc", new string[2] {"[-]", "e caduc" } }, 
    /// };
    /// </code>
    /// </remarks>
    [Serializable]
    public class ColConfWin : ConfigBase
    {
        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------    public types   --------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// mode à utiliser pour les "ill"
        /// </summary>
        [Serializable]
        public enum IllRule { ceras, lirecouleur, undefined }

        /// <summary>
        /// posibilités de valeur pour le flag <c>defBeh</c> qui indique comment doit se comporter la
        /// mise en couleur des phonèmes qui n'ont pas d'instructions de fomratage.
        /// </summary>
        [Serializable]
        public enum DefBeh { transparent, noir, undefined }

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  public static members -----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Nombre de sons
        /// </summary>
        public const int nrSons = 41; // don't forget to increase in case...

        
        /// <summary>
        /// Tableau de couleurs prédéfinies auquel on peut accéder en utilisant un indice de type
        /// <see cref="CERASColor"/> ou <see cref="PredefCol"/>.
        /// </summary>
        public readonly static RGB[] predefinedColors = new RGB[] {
            new RGB(000, 000, 000), // CERAS_oi     --> noir
            new RGB(240, 222, 000), // CERAS_o      --> jaune
            new RGB(237, 125, 049), // CERAS_an     --> orange
            new RGB(051, 153, 102), // CERAS_5      --> vert comme sapin
            new RGB(164, 020, 210), // CERAS_E      --> violet
            new RGB(000, 020, 208), // CERAS_e      --> (bleu) foncé
            new RGB(255, 000, 000), // CERAS_u      --> rouge
            new RGB(171, 121, 066), // CERAS_on     --> marron
            new RGB(071, 115, 255), // CERAS_eu     --> bleu
            new RGB(015, 201, 221), // CERAS_oin    --> turquoise
            new RGB(166, 166, 166), // CERAS_muet   --> gris
            new RGB(255, 100, 177), // CERAS_rosé   --> rose
            new RGB(127, 241, 000), // CERAS_ill    --> vert grenouille

            new RGB(000, 000, 255), // bleuPur      --> bleu
            new RGB(221, 221, 221), // neutre       --> gris // il est important qu'il ne s'agisse pas d'une couleur de WdColorIndex
            new RGB(091, 215, 255), // bleu clair   --> bleu clair
            new RGB(175, 000, 000), // rouge foncé  --> rouge foncé
            new RGB(255, 255, 255), // blanc        --> blanc
        };

        /// <summary>
        /// Tableau des <see cref="CharFormatting"/> correspondant aux couleurs prédéfinies.
        /// Correspond à <see cref="predefinedColors"/>.
        /// </summary>
        /// <remarks>
        /// Instead of using <c>predefCF[(int)PredefCols.neutral]</c> use
        /// <c>CharFormatting.NeutralCF</c> which is equivalent.
        /// </remarks>
        public static CharFormatting[] predefCF { get; private set; }
        

        // -------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------  Internal Types ---------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Identifiants pour les flags de contrôle des règles de l'automate. Est typiquement
        /// utilisé comme indice dans un tableau de flags.
        /// </summary>
        public enum RuleFlag { 
            /// <summary>
            /// valeur non définie.
            /// </summary>
            undefined,

            /// <summary>
            /// les "ill" et "il" sont traités comme un son.
            /// </summary>
            IllCeras,

            /// <summary>
            /// les "ill" et "il" sont traités en fonction des phonèmes effectivement présents
            /// dans les mots. fille par exemple donne fij°.
            /// </summary>
            IllLireCouleur,
            
            /// <summary>
            /// Dernière valeur de l'énuméré. Peut-être utilisé si on désire itérer sur toutes
            /// les valeurs...
            /// </summary>
            last }

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  private static members ----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static Dictionary<string, List<Phonemes>> sonMap = new Dictionary<string, List<Phonemes>>(nrSons) // don't forget to increase in case...
        {
            {"a",   new List<Phonemes> (1) {Phonemes.a}},
            {"q",   new List<Phonemes> (1) {Phonemes.q}},
            {"i",   new List<Phonemes> (1) {Phonemes.i}},
            {"y",   new List<Phonemes> (1) {Phonemes.y}},
            {"1",   new List<Phonemes> (1) {Phonemes.x_tilda}},
            {"u",   new List<Phonemes> (1) {Phonemes.u}},
            {"é",   new List<Phonemes> (2) {Phonemes.e, Phonemes.e_comp}},
            {"o",   new List<Phonemes> (2) {Phonemes.o, Phonemes.o_comp}},
            {"è",   new List<Phonemes> (2) {Phonemes.E, Phonemes.E_comp}},
            {"an",  new List<Phonemes> (1) {Phonemes.a_tilda}},
            {"on",  new List<Phonemes> (1) {Phonemes.o_tilda}},
            {"2",   new List<Phonemes> (1) {Phonemes.x2}},
            {"oi",  new List<Phonemes> (1) {Phonemes.oi}},
            {"5",   new List<Phonemes> (1) {Phonemes.e_tilda}},
            {"w",   new List<Phonemes> (1) {Phonemes.w}},
            {"j",   new List<Phonemes> (1) {Phonemes.j}},
            {"ill", new List<Phonemes> (2) {Phonemes.j_ill, Phonemes.i_j_ill}},
            {"ng",  new List<Phonemes> (1) {Phonemes.J}},
            {"gn",  new List<Phonemes> (1) {Phonemes.N}},
            {"l",   new List<Phonemes> (1) {Phonemes.l}},
            {"v",   new List<Phonemes> (1) {Phonemes.v}},
            {"f",   new List<Phonemes> (2) {Phonemes.f, Phonemes.f_ph}},
            {"p",   new List<Phonemes> (1) {Phonemes.p}},
            {"b",   new List<Phonemes> (1) {Phonemes.b}},
            {"m",   new List<Phonemes> (1) {Phonemes.m}},
            {"z",   new List<Phonemes> (2) {Phonemes.z, Phonemes.z_s}},
            {"s",   new List<Phonemes> (4) {Phonemes.s, Phonemes.s_c, Phonemes.s_t, Phonemes.s_x}},
            {"t",   new List<Phonemes> (1) {Phonemes.t}},
            {"d",   new List<Phonemes> (1) {Phonemes.d}},
            {"ks",  new List<Phonemes> (1) {Phonemes.ks}},
            {"gz",  new List<Phonemes> (1) {Phonemes.gz}},
            {"r",   new List<Phonemes> (1) {Phonemes.R}},
            {"n",   new List<Phonemes> (1) {Phonemes.n}},
            {"ge",  new List<Phonemes> (1) {Phonemes.Z}},
            {"ch",  new List<Phonemes> (1) {Phonemes.S}},
            {"k",   new List<Phonemes> (2) {Phonemes.k, Phonemes.k_qu}},
            {"g",   new List<Phonemes> (2) {Phonemes.g, Phonemes.g_u}},
            {"ij",  new List<Phonemes> (1) {Phonemes.i_j}},
            {"oin", new List<Phonemes> (1) {Phonemes.w_e_tilda}},
            {"_muet",   new List<Phonemes> (2) {Phonemes.verb_3p, Phonemes._muet}},
            {"q_caduc", new List<Phonemes> (1) {Phonemes.q_caduc}}
        };

        private static Dictionary<string, string[]> sonOutMap = new Dictionary<string, string[]> (nrSons)
        {
            {"a",   new string[2] {"[a]",   "ta, plat"  } },
            {"q",   new string[2] {"[e]",   "le"        } },
            {"i",   new string[2] {"[i]",   "il, lit"   } },
            {"y",   new string[2] {"[y]",   "tu, lu"    } },
            {"1",   new string[2] {"[1]",   "parfum"    } },
            {"u",   new string[2] {"[u]",   "cou, roue" } },
            {"é",   new string[2] {"[é]",   "né, été"   } },
            {"o",   new string[2] {"[o]",   "mot, eau"  } },
            {"è",   new string[2] {"[è]",   "sel"       } },
            {"an",  new string[2] {"[@]",   "grand"     } },
            {"on",  new string[2] {"[§]",   "son"       } },
            {"2",   new string[2] {"[2]",   "feu, oeuf" } },
            {"oi",  new string[2] {"[oi]",  "noix"      } },
            {"5",   new string[2] {"[5]",   "fin"       } },
            {"w",   new string[2] {"[w]",   "kiwi"      } },
            {"j",   new string[2] {"[j]",   "payer"     } },
            {"ill", new string[2] {"[ill]", "feuille"   } },
            {"ng",  new string[2] {"[ng]",  "parking"   } },
            {"gn",  new string[2] {"[gn]",  "ligne"     } },
            {"l",   new string[2] {"[l]",   "aller"     } },
            {"v",   new string[2] {"[v]",   "veau"      } },
            {"f",   new string[2] {"[f]",   "effacer"   } },
            {"p",   new string[2] {"[p]",   "papa"      } },
            {"b",   new string[2] {"[b]",   "bébé"      } },
            {"m",   new string[2] {"[m]",   "pomme"     } },
            {"z",   new string[2] {"[z]",   "zoo"       } },
            {"s",   new string[2] {"[s]",   "scie"      } },
            {"t",   new string[2] {"[t]",   "tortue"    } },
            {"d",   new string[2] {"[d]",   "dindon"    } },
            {"ks",  new string[2] {"[ks]",  "rixe"      } },
            {"gz",  new string[2] {"[gz]",  "examen"    } },
            {"r",   new string[2] {"[r]",   "rare"      } },
            {"n",   new string[2] {"[n]",   "Nicole"    } },
            {"ge",  new string[2] {"[ge]",  "jupe"      } },
            {"k",   new string[2] {"[k]",   "coq"       } },
            {"g",   new string[2] {"[g]",   "gare"      } },
            {"ch",  new string[2] {"[ch]",  "chat"      } },
            {"ij",  new string[2] {"[ij]",  "pria"      } },
            {"oin", new string[2] {"[oin]", "soin"      } },
            {"_muet", new string[2] {"[#]", "\'muet\'"  } },
            {"q_caduc", new string[2] {"[-]", "e caduc" } }, 
        };

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  public static methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public static void Init()
        {
            logger.ConditionalDebug("Init");
            predefCF = new CharFormatting[predefinedColors.Length + 1];  // +1 for the enutral entry
            for (int i = 0; i < predefinedColors.Length; i++)
                predefCF[i] = new CharFormatting(predefinedColors[i]);
            predefCF[(int)PredefCol.neutral] = CharFormatting.NeutralCF;
        }

        // -------------------------------------------------- Mapping "sons" to text  ------------------------------------------

        /// <summary>
        /// Retourne le texte utilisé dans l'affichage pour identifier un son. La pluspart du temps il s'agit
        /// du nom du son entre crochet (par exemple [@])
        /// </summary>
        /// <param name="son">Le son dont on veut le texte affiché.</param>
        /// <returns></returns>
        public static string DisplayText(string son) => sonOutMap[son][0];

        /// <summary>
        /// Retourne l'exemple à utiliser pour le son donné. Par exemple "feuille".
        /// </summary>
        /// <param name="son">Le son pour lequel on veut un example.</param>
        /// <returns>L'example illsutrant le son.</returns>
        public static string ExampleText(string son) => sonOutMap[son][1];

        // ------------------------------------------------------- About sons  -----------------------------------------------

        public static Dictionary<string, List<Phonemes>>.KeyCollection GetListOfSons() => sonMap.Keys;

        // ******************************************************************************************************************
        //  ****************************************************** INSTANTIATED *********************************************
        // ******************************************************************************************************************

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  Event Handlers ----------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Evènement déclenché quand le <c>CharFormatting</c> d'un son est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SonConfigModifiedEventArgs> SonCharFormattingModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand la checkBox d'un son est modifiée..
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<SonConfigModifiedEventArgs> SonCBModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand l'option de traitement de "ill" est modifiée
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PhonConfModifiedEventArgs> IllModifiedEvent;

        /// <summary>
        /// Evènement déclenché quand le "default behaviour" pour les formatages "false" est modifié.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<PhonConfModifiedEventArgs> DefBehModifiedEvent;


        // -------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------  public  members --------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Permet de déterminer le mode à utiliser pour les "ill". À utiliser en lecture et en écriture.
        /// </summary>
        public IllRule IllRuleToUse
        {
            get
            {
                if (flags[(int)RuleFlag.IllCeras])
                    return IllRule.ceras;
                else
                    return IllRule.lirecouleur;
            }

            set
            {
                if ((value == IllRule.ceras) && (flags[(int)RuleFlag.IllCeras] == false))
                {
                    flags[(int)RuleFlag.IllCeras] = true;
                    flags[(int)RuleFlag.IllLireCouleur] = false;
                    OnIllModified(pct);
                }
                else if ((value == IllRule.lirecouleur) && (flags[(int)RuleFlag.IllLireCouleur] == false))
                {
                    flags[(int)RuleFlag.IllCeras] = false;
                    flags[(int)RuleFlag.IllLireCouleur] = true;
                    OnIllModified(pct);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------  private  members -------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private PhonConfType pct; // définit la configuration par défaut appliquée par le constructeur

        // -------------------------------------- CharFormattings per Phonemes ------------------------------------------------
        private CharFormatting[] cfPhon;
        private bool[] chkPhon; // indique si le CharFormatting du phonème doit être appliqué ou non.

        // -------------------------------------- CharFormattings per "son"  ------------------------------------------------
        private Dictionary<string, CharFormatting> cfSon;
        private Dictionary<string, bool> chkSon; // indique si le CharFormatting du son doit être appliqué ou non.

        // --------------------------------- La configuration du traitement des "ill"  ---------------------------------------
        [OptionalField(VersionAdded = 2)]
        private List<bool> flags;
        // on se sert de RuleFlags comme index dans le tableau.

        // ------------------------ Le paramétrage du comportement pour le phonèmes "non formatés"  ---------------------------
        [OptionalField(VersionAdded = 3)]
        public DefBeh defBeh; // {get; private set;}

        [OptionalField(VersionAdded = 3)]
        private CharFormatting defChF; // CharFormatting returned for phonemes where the checkbox is not set


        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  public  methods ----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Construit un <c>ColConfWin</c> et l'initialise pour le type de phonèmes défini par
        /// <paramref name="inPct"/>.
        /// </summary>
        /// <param name="inPct">Le type de phonèmes pour lequel le <c>ColConfWin</c> est construit.</param>
        public ColConfWin(PhonConfType inPct)
        {
            logger.ConditionalDebug("ColConfWin Constructor for {0}", inPct);
            defBeh = DefBeh.transparent;
            defChF = predefCF[(int)PredefCol.neutral];

            cfPhon = new CharFormatting[(int)Phonemes.lastPhon];
            chkPhon = new bool[(int)Phonemes.lastPhon];

            cfSon = new Dictionary<string, CharFormatting>(sonMap.Count);
            chkSon = new Dictionary<string, bool>(sonMap.Count);

            flags = new List<bool>((int)RuleFlag.last);

            pct = inPct;

            Reset();
        }

        /// <summary>
        /// Réinitialise le <c>ColConfWin</c> aux valeurs par défaut propres au <c>PhonConfType</c>
        /// défini lors de la création de l'objet.
        /// </summary>
        public override void Reset()
        {
            logger.ConditionalDebug("Reset");
            for (int i = 0; i < (int)RuleFlag.last; i++)
                flags.Add(true); // par défaut, les règles sont actives.
            flags[(int)RuleFlag.IllLireCouleur] = false; // config par défaut

            switch (pct)
            {
                case PhonConfType.phonemes:
                    SetCerasRose();
                    break;
                case PhonConfType.muettes:
                    InitColorMuettes();
                    break;
                default:
                    break;
            }
        }


        // ------------------------------------------------------- Phonemes --------------------------------------------------

        /// <summary>
        /// Retourne le <c>CharFormatting</c> pour le phonème <paramref name="p"/>
        /// </summary>
        /// <param name="p">Le phonème pour lequel on veut le <c>CharFormatting</c>.</param>
        /// <returns><c>CharFormatting</c> pour le phonème</returns>
        public CharFormatting Get(Phonemes p)
        // get the Charformatting for the given Phoneme
        {
            CharFormatting toReturn;
            if (chkPhon[(int)p])
                toReturn = cfPhon[(int)p];
            else
                toReturn = defChF;
            return toReturn;
        }

        // ---------------------------------------------------------- Son ------------------------------------------------------

        /// <summary>
        /// Valeur de la checkbox pour le <paramref name="son"/>
        /// </summary>
        /// <param name="son">Le son pour lequel on veut la valeur de la checkbox.</param>
        /// <returns>La valeur de la checkbox.</returns>
        public bool GetCheck(string son) => chkSon[son];

        /// <summary>
        /// DOnne le <c>CharFormatting</c> pour le <c>son</c>.
        /// </summary>
        /// <param name="son">Le son pour lequel on veut le <c>CharFormatting</c>. Voir 
        /// <c>sonMap</c> pour les valeurs autorisées.</param>
        /// <returns>le <c>CharFormatting</c> recherché.</returns>
        public CharFormatting GetCF(string son) => cfSon[son];

        /// <summary>
        /// Définit un nouveau <see cref="CharFormatting"/> pour le son et met la checkbox
        /// correspondante à true.
        /// </summary>
        /// <param name="son">Le son dont on veut modifier le <see cref="CharFormatting"/>"/> et la 
        /// checkbox.</param>
        /// <param name="cf">Le nouveau <see cref="CharFormatting"/>.</param>
        public void SetCbxAndCF(string son, CharFormatting cf)
        {
            logger.ConditionalDebug("SetCbxAndCF, son: {0}", son);
            SetChkSon(son, true);
            SetCFSon(son, cf);
        }

        /// <summary>
        /// Réinitialise la checkbox et le <see cref="CharFormatting"/> pour le son à <c>false</c>
        /// et noir.
        /// </summary>
        /// <param name="son">Le son à réinitialiser.</param>
        public void ClearSon(string son)
        {
            logger.ConditionalDebug("ClearSon");
            SetChkSon(son, false);
            SetCFSon(son, predefCF[(int)PredefCol.black]);
        }

        /// <summary>
        /// Met toutes les checkboxes à <c>true</c>.
        /// </summary>
        public void SetAllCbxSons()
        {
            logger.ConditionalDebug("SetAllCbxSons");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, true);
        }

        /// <summary>
        /// Met toutes les checkboxes à <c>false</c>.
        /// </summary>
        public void ClearAllCbxSons()
        {
            logger.ConditionalDebug("ClearAllCbxSons");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, false);
        }

        /// <summary>
        /// Initialise les sons à la configuration dite CERAS.
        /// </summary>
        public void SetCeras()
        {
            logger.ConditionalDebug("SetCeras");
            CleanAllSons();

            // o
            SetChkSon("o", true);
            SetCFSon("o", predefCF[(int)CERASColor.CERAS_o]);

            // a_tilda (son an)
            SetChkSon("an", true);
            SetCFSon("an", predefCF[(int)CERASColor.CERAS_an]);

            // e_tilda (son ain)
            SetChkSon("5", true);
            SetCFSon("5", predefCF[(int)CERASColor.CERAS_5]);

            // E (son è)
            SetChkSon("è", true);
            SetCFSon("è", predefCF[(int)CERASColor.CERAS_E]);

            // e (son é)
            SetChkSon("é", true);
            SetCFSon("é", predefCF[(int)CERASColor.CERAS_e]);

            // u (son ou)
            SetChkSon("u", true);
            SetCFSon("u", predefCF[(int)CERASColor.CERAS_u]);

            // w (son oi)
            // bold & black
            SetChkSon("oi", true);
            SetCFSon("oi", new CharFormatting(true, false, false, false, true, predefinedColors[(int)CERASColor.CERAS_oi],
                false, predefinedColors[(int)PredefCol.neutral]));

            // o_tilda (son on)
            SetChkSon("on", true);
            SetCFSon("on", predefCF[(int)CERASColor.CERAS_on]);

            // (sons eu et oeu)
            SetChkSon("2", true);
            SetCFSon("2", predefCF[(int)CERASColor.CERAS_eu]);

            // (oin)
            SetChkSon("oin", true);
            SetCFSon("oin", predefCF[(int)CERASColor.CERAS_oin]);

            // (son un)
            SetChkSon("1", true);
            SetCFSon("1", new CharFormatting(false, false, true)); // underline

            // ph_muet
            SetChkSon("_muet", true);
            SetCFSon("_muet", predefCF[(int)CERASColor.CERAS_muet]);
        }

        /// <summary>
        /// Initialise le <c>ColConfWin</c> à la configuration dite "CERAS rosé"
        /// </summary>
        public void SetCerasRose()
        {
            logger.ConditionalDebug("SetCerasRose");
            // est construit en delta par rapport à SetCeras
            SetCeras();
            // changer la couleur du é en rosé
            SetCFSon("é", predefCF[(int)CERASColor.CERAS_rosé]);

            // activer le son ill
            SetChkSon("ill", true);
            SetCFSon("ill", new CharFormatting(false, true, false, false, true, predefinedColors[(int)CERASColor.CERAS_ill],
                false, predefinedColors[(int)PredefCol.neutral]));

            // commenté le 14.05.2020 - J'ai eu un feedback de M. Tissot qui suggérait de le laisser mais avec une autre couleur.
            // désactiver le (oin)
            // SetChkSon("oin", false);
            // Set("oin", predefCF[(int)PredefCols.black]);

            IllRuleToUse = IllRule.ceras;
        }

        /// <summary>
        /// Assigne un nouveau <see cref="CharFormatting"/> au son <paramref name="son"/>.
        /// </summary>
        /// <param name="son">Le son dont le <see cref="CharFormatting"/> doit être modifié. 
        /// Voir <c>sonMap</c> pour la liste des sons reconnus.</param>
        /// <param name="cf">Le nouveau <see cref="CharFormatting"/>.</param>
        public void SetCFSon(string son, CharFormatting cf)
        {
            logger.ConditionalDebug("SetCFSon \'{0}\'", son);
            CharFormatting valCF;
            if(!(cfSon.TryGetValue(son, out valCF) && valCF == cf))
            {
                Debug.Assert(sonMap.ContainsKey(son), String.Format(BaseConfig.cultF, "{0} n'est pas un son connu", son));
                cfSon[son] = cf;
                foreach (Phonemes p in sonMap[son])
                    Set(p, cf);
                OnSonCharFormattingModified(new SonConfigModifiedEventArgs(son, pct));
            }
        }

        /// <summary>
        /// Met le flag checkbox à la valeur <paramref name="checkVal"/> pour le son <paramref name="son"/>.
        /// </summary>
        /// <param name="son">Le son dont la checkbox est modifiée.</param>
        /// <param name="checkVal">La nouvelle valeur du flag checkbox.</param>
        public void SetChkSon(string son, bool checkVal)
        {
            logger.ConditionalDebug("SetChkSon \'{0}\' to {1}", son, checkVal);
            bool valCK;
            if (!(chkSon.TryGetValue(son, out valCK) && valCK == checkVal)) 
            {
                chkSon[son] = checkVal;
                foreach (Phonemes p in sonMap[son])
                    chkPhon[(int)p] = checkVal;
                OnSonCBModified(new SonConfigModifiedEventArgs(son, pct));
            }
        }

        // ---------------------------------------------------  Default Behaviour  ------------------------------------------------------

        /// <summary>
        /// <para>
        /// Met le comportement par défaut pour la façon de traiter la couleur des phonèmes (ou sons)
        /// sans couleur définie:
        /// </para>
        /// <para>
        /// true --> phonèmes non traités en noir
        /// </para>
        /// <para>
        /// false --> phonèmes non traités sans changement de couleur
        /// </para>
        /// </summary>
        /// <param name="val">La nouvelle valeur pour ce flag.</param>
        public void DefaultBehaviourChangedTo(bool val)
        {
            logger.ConditionalDebug("DefaultBehaviourChangedTo {0}", val);
            if (val != (defBeh == DefBeh.noir))
            {
                if (val)
                {
                    defBeh = DefBeh.noir;
                    defChF = predefCF[(int)PredefCol.black];
                }
                else
                {
                    defBeh = DefBeh.transparent;
                    defChF = predefCF[(int)PredefCol.neutral];
                }
                OnDefBehModified(pct);
            }
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  internal  methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        internal override void PostLoadInitOptionalFields()
        {
            logger.ConditionalDebug("PostLoadInitOptionalFields");
            if (cfPhon.Length < (int)Phonemes.lastPhon)
            {
                Array.Resize(ref cfPhon, (int)Phonemes.lastPhon);
                Array.Resize(ref chkPhon, (int)Phonemes.lastPhon);
                logger.ConditionalDebug("cfPhon & chkPhon resized.");
            }
            if (!cfSon.ContainsKey("ill"))
            {
                SetCFSon("ill", predefCF[(int)PredefCol.black]);
                SetChkSon("ill", false);
                logger.ConditionalDebug("Son \"ill\" initialisé.");
            }
        }

        // ------------------------------------------------------- Rule Flags ---------------------------------------------------

        /// <summary>
        /// Gives the value of the said rule flag.
        /// </summary>
        /// <param name="rf">identifier of the flag one wants the value of.</param>
        /// <returns>The value of the flag.</returns>
        internal bool GetFlag(RuleFlag rf) => flags[(int)rf];

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  private  methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------


        private void Set(Phonemes p, CharFormatting chF) => cfPhon[(int)p] = chF;

        private void CleanAllSons()
        {
            logger.ConditionalDebug("CleanAllSons");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
            {
                SetCFSon(k.Key, predefCF[(int)PredefCol.black]);
            }
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
            {
                SetChkSon(k.Key, false);
            }
        }

        private void InitColorMuettes()
        {
            logger.ConditionalDebug("InitColorMuettes");
            CleanAllSons();
            SetChkSon("_muet", true);
            SetCFSon("_muet", predefCF[(int)CERASColor.CERAS_muet]);
        }

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            flags = new List<bool>((int)RuleFlag.last);
            for (int i = 0; i < (int)RuleFlag.last; i++)
                flags.Add(true); // par défaut, les règles sont actives.
            flags[(int)RuleFlag.IllLireCouleur] = false; // config par défaut
            defBeh = DefBeh.transparent;
            defChF = predefCF[(int)PredefCol.neutral];
        }


        // --------------------------------------- Events --------------------------------

        protected virtual void OnSonCharFormattingModified(SonConfigModifiedEventArgs e)
        {
            logger.ConditionalDebug("OnSonCharFormattingModified e.son: \'{0}\', e.pct: {1}", e.son, e.pct);
            EventHandler<SonConfigModifiedEventArgs> eventHandler = SonCharFormattingModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnSonCBModified(SonConfigModifiedEventArgs e)
        {
            logger.ConditionalDebug("OnSonCBModified e.son: \'{0}\', e.pct: {1}", e.son, e.pct);
            EventHandler<SonConfigModifiedEventArgs> eventHandler = SonCBModifiedEvent;
            eventHandler?.Invoke(this, e);
        }

        protected virtual void OnIllModified(PhonConfType inPCT)
        {
            logger.ConditionalDebug("OnIllModified");
            EventHandler<PhonConfModifiedEventArgs> eventHandler = IllModifiedEvent;
            eventHandler?.Invoke(this, new PhonConfModifiedEventArgs(inPCT));
        }

        protected virtual void OnDefBehModified(PhonConfType inPCT)
        {
            logger.ConditionalDebug("OnDefBehModified");
            EventHandler<PhonConfModifiedEventArgs> eventHandler = DefBehModifiedEvent;
            eventHandler?.Invoke(this, new PhonConfModifiedEventArgs(inPCT));
        }
    }
}
