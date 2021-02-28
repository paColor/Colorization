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
    /// Liste de couleurs prédéfinies. L'idée est d'utiliser
    /// les valeurs de cet énuméré comme indice dans <c>predefinedColors</c>.
    /// L'utilisation dans <c>cerasCF</c> est par contre interdite!
    /// </summary>
    /// <remarks>
    /// <c>CERASColor</c> et <see cref="PredefCol"/> sont deux manières différentes
    /// d'indexer <c>predefinedColors</c>. Les deux énumérés doivent donc absolument correspondre.
    /// On doit par conséquence avoir ici les couleurs utilisées dans la configuration CERAS.
    /// Faire très attention en cas de modifications!
    /// </remarks>
    /// <example>
    /// Pour écrire en orange
    /// <code>
    /// toR.Font.Fill.ForeColor.RGB = ColConfWin.predefinedColors[(int)PredefCols.orange];
    /// </code>
    /// </example>
    [Serializable]
    public enum PredefCol { black, darkYellow, orange, darkGreen, violet, darkBlue, red,
        brown, blue, turquoise, grey, pink, frogGreen, cerasUn,
        neutral, pureBlue, lightBlue, darkRed, white, pinky
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
    /// dans le code. Il est à noter cependant que formellement, seule la liste dans <c>sonMap</c> 
    /// (qui est un tableau 'private') fait foi. Cette liste est disponible dans la liste 
    /// <see cref="sonsValides"/> si elle s'avère nécessaire.
    /// <code>
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
    ///     {"47",  new string[2] {"[47]",  "0..9"      } },
    ///     {"oin", new string[2] {"[oin]", "soin"      } },
    ///     {"uni", new string[2] {"[uni]", "0001"      } },
    ///     {"diz", new string[2] {"[diz]", "0010"      } },
    ///     {"cen", new string[2] {"[cen]", "0100"      } },
    ///     {"mil", new string[2] {"[mil]", "1000"      } },
    ///     {"_muet", new string[2] {"[#]", "\'muet\'"  } },
    ///     {"q_caduc", new string[2] {"[-]", "e caduc" } }, 
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
        public enum DefBeh { 
            /// <summary>
            /// Les phonèmes pour lesquels on n'a pas de <see cref="CharFormatting"/> attribué et
            /// activé, ne sont pas touchés. Il restent tels qu'ils sont dans le texte d'origine.
            /// </summary>
            transparent,

            /// <summary>
            /// Les phonèmes pour lesquels on n'a pas de <see cref="CharFormatting"/> attribué et
            /// activé, sont mis en noir. (Les autres attributs de formatage comme 'bold' ou 
            /// 'italic' ne sont pas touchés).
            /// </summary>
            noir,

            /// <summary>
            /// Valeur avant l'initialisation. Comportement indéfini. Pourrait déclencher une
            /// exception.
            /// </summary>
            undefined }

        /// <summary>
        /// Identifiants pour les flags de contrôle des règles de l'automate. Est typiquement
        /// utilisé comme indice dans un tableau de flags.
        /// </summary>
        [Serializable]
        public enum RuleFlag
        {
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
            last
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------    private types   -------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Les différemtes couleurs qui sont utilisées dans la configuration CERAS. Le but est de 
        /// pouvoir utiliser une de ces valeurs pour accéder à la couleur ou au 
        /// <see cref="CharFormatting"/> correspondant en l'utilisant comme index dans une des tables
        /// définies dans cette classe.
        /// </summary>
        /// <example>
        /// <code>
        /// new CharFormatting(true, false, false, false, 
        ///         true, predefinedColors[(int)CERASColors.CERAS_oi], 
        ///         false, predefinedColors[(int)PredefCols.neutral]));
        /// SetCFSon("oin", cerasCF[(int)CERASColors.CERAS_oin]);
        /// </code>
        /// </example>
        /// /// <remarks>
        /// <see cref="CERASColor"/> et <see cref="PredefCol"/> sont deux manières différentes
        /// d'indexer les mêmes tableaux. Les deux énumérés doivent donc absolument correspondre.
        /// Faire très attention en cas de modifications!
        /// </remarks>
        [Serializable]
        private enum CERASColor
        {
#pragma warning disable CA1707 // Identifiers should not contain underscores
            CERAS_oi, CERAS_o, CERAS_an, CERAS_5, CERAS_E, CERAS_e, CERAS_u,
            CERAS_on, CERAS_eu, CERAS_oin, CERAS_muet, CERAS_rosé, CERAS_ill, CERAS_1
#pragma warning restore CA1707 // Identifiers should not contain underscores
        }


        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  public static members -----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Nombre de sons
        /// </summary>
        public const int nrSons = 46; // don't forget to increase in case...

        
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
            new RGB(222, 222, 222), // CERAS_1      --> gris // la couleur ne devrait jamais être utilisée.

            new RGB(221, 221, 221), // neutre       --> gris // il est important qu'il ne s'agisse pas d'une couleur de WdColorIndex
            new RGB(000, 000, 255), // bleuPur      --> bleu
            new RGB(091, 215, 255), // bleu clair   --> bleu clair
            new RGB(175, 000, 000), // rouge foncé  --> rouge foncé
            new RGB(255, 255, 255), // blanc        --> blanc
            new RGB(255, 000, 128), // pinky        --> rose foncé
        };

        /// <summary>
        /// Contient un <see cref="CharFormatting"/> pour chaque couleur de <see cref="PredefCol"/>.
        /// Ils ne formattent que les couleurs et laissent les différents flags sur <c>false</c>.
        /// Le bon <c>CharFormatting</c> sera trouvé en indexant avec <see cref="PredefCol"/>:
        /// <code>
        /// coloredCF[(int)PredefCol.frogGreen]
        /// </code>
        /// </summary>
        /// <remarks>
        /// Attention: <c>coloredCF[(int)PredefCol.neutral]</c> correspond à un gris clair et non
        /// à <c>CharFormatting.NeutralCF</c>.
        /// </remarks>
        public static CharFormatting[] coloredCF { get; private set; }

        /// <summary>
        /// Listes des sons, tels que définis dans <c>sonMap</c>.
        /// </summary>
        public static HashSet<string> sonsValides { get; private set; }

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  private static members ----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Tableau des <see cref="CharFormatting"/> correspondant à la définition CERAS.
        /// Peut être accédé en utilisant <see cref="CERASColor"/> comme index afin de
        /// trouver le bon <c>CharFormatting</c>.
        /// <para>
        /// Attention: Les <see cref="CharFormatting"/> correspondent à la définition du CERAS. 
        /// Ils contiennent parfois un formatage qui va au-delà de la couleur ("oi" est en gras,
        /// "un" est souligné, "ill" est en italique...). 
        /// Si on veut un <c>CharFormatting</c> qui ne contienne qu'une couleur il faut
        /// utiliser <c>coloredCF</c>, ceux qui sont définis dans <see cref="CharFormatting"/>:
        /// <code>
        /// defChF = CharFormatting.NeutralCF;
        /// SetCFSon(son, CharFormatting.BlackCF);
        /// </code>
        /// ou alors en créer un nouveau:
        /// <code>
        /// RGB maCouleur = new RGB (123, 31, 27);
        /// new CharFormatting(maCouleur);
        /// </code>
        /// Ces différentes manières ne touchent pas aux atres caractéristiques de 
        /// <see cref="CharFormatting"/>. 
        /// </para>
        /// </summary>
        /// <remarks>
        /// Utiliser <c>PredefCol</c> comme index
        /// <code>
        /// cerasCF[(int)PredefCol.xxx]
        /// </code>
        /// est interdit!
        /// </remarks>
        private static CharFormatting[] cerasCF { get; set; }

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
            {"j",   new List<Phonemes> (2) {Phonemes.j, Phonemes.ji}},
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
            {"47",  new List<Phonemes> (1) {Phonemes.chiffre}},
            {"uni", new List<Phonemes> (1) {Phonemes.unité}},
            {"diz", new List<Phonemes> (1) {Phonemes.dizaine}},
            {"cen", new List<Phonemes> (1) {Phonemes.centaine}},
            {"mil", new List<Phonemes> (1) {Phonemes.milliers}},
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
            {"47",  new string[2] {"[47]",  "0..9"      } },
            {"oin", new string[2] {"[oin]", "soin"      } },
            {"uni", new string[2] {"[uni]", "0001"      } },
            {"diz", new string[2] {"[diz]", "0010"      } },
            {"cen", new string[2] {"[cen]", "0100"      } },
            {"mil", new string[2] {"[mil]", "1000"      } },
            {"_muet", new string[2] {"[#]", "\'muet\'"  } },
            {"q_caduc", new string[2] {"[-]", "e caduc" } }, 
        };


        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  public static methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public static void Init()
        {
            logger.ConditionalDebug("Init");
            cerasCF = new CharFormatting[predefinedColors.Length];
            coloredCF = new CharFormatting[predefinedColors.Length];
            for (int i = 0; i < predefinedColors.Length; i++)
            {
                cerasCF[i] = new CharFormatting(predefinedColors[i]);
                coloredCF[i] = cerasCF[i];
            }
            cerasCF[(int)PredefCol.neutral] = CharFormatting.NeutralCF;

            cerasCF[(int)CERASColor.CERAS_ill] = new CharFormatting(false, true, false, false,
                true, predefinedColors[(int)CERASColor.CERAS_ill],
                false, predefinedColors[(int)PredefCol.neutral]);

            cerasCF[(int)CERASColor.CERAS_oi] = new CharFormatting(true, false, false, false, 
                true, predefinedColors[(int)CERASColor.CERAS_oi],
                false, predefinedColors[(int)PredefCol.neutral]);

            cerasCF[(int)CERASColor.CERAS_1] = new CharFormatting(false, false, true);

            sonsValides = new HashSet<string>();
            foreach (KeyValuePair<string, List<Phonemes>> kvp in sonMap)
            {
                sonsValides.Add(kvp.Key);
            }
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
                else if (flags[(int)RuleFlag.IllLireCouleur])
                    return IllRule.lirecouleur;
                else
                    return IllRule.undefined;
            }

            set
            {
                if (value != IllRuleToUse)
                {
                    UndoFactory.ExceutingAction(new ColPhonAct("Ill", this, IllRuleToUse, value));
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
                    else if (value == IllRule.undefined)
                    {
                        flags[(int)RuleFlag.IllCeras] = false;
                        flags[(int)RuleFlag.IllLireCouleur] = false;
                        OnIllModified(pct);
                    }
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
        
        /// <summary>
        /// Définit le comportement adopté pour les phonèmes qui ne sont pas activés. Voir la
        /// définition de <see cref="DefBeh"/> pour les valeurs possibles et leur sémantique.
        /// </summary>
        /// <remarks>
        /// Veuillez utiliser <see cref="SetDefaultBehaviourTo(DefBeh)"/> pour définir la valeur
        /// ce 'membre'. Il est accessible de cette manière pour rester compatible avec d'éventuels
        /// anciens fichiers de sauvegarde. Attention à ne pas en profiter par erreur.
        /// </remarks>
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
            defChF = CharFormatting.NeutralCF;

            cfPhon = new CharFormatting[(int)Phonemes.lastPhon];
            chkPhon = new bool[(int)Phonemes.lastPhon];

            cfSon = new Dictionary<string, CharFormatting>(sonMap.Count);
            chkSon = new Dictionary<string, bool>(sonMap.Count);

            flags = new List<bool>((int)RuleFlag.last) { true, true, true }; // par défaut tout à true
            // notons au passage que la plupart des règles utilisent le flag 'undefined'

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
            UndoFactory.StartRecording("Réinitialiser sons");

            IllRule prevIllRule = IllRuleToUse;
            flags[(int)RuleFlag.IllLireCouleur] = false; // config par défaut
            UndoFactory.ExceutingAction(new ColPhonAct("Ill", this, prevIllRule, IllRuleToUse));

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
            UndoFactory.EndRecording();
        }


        // ------------------------------------------------------- Phonemes --------------------------------------------------

        /// <summary>
        /// Retourne le <c>CharFormatting</c> pour le phonème <paramref name="p"/>
        /// </summary>
        /// <param name="p">Le phonème pour lequel on veut le <c>CharFormatting</c>.</param>
        /// <returns><c>CharFormatting</c> pour le phonème</returns>
        public CharFormatting GetCF(Phonemes p)
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
        /// <exception cref="KeyNotFoundException"> si le <paramref name="son"/> n'est pas 
        /// un son connu. Voir <see cref="sonsValides"/></exception>
        /// <exception cref="ArgumentNullException"> si <paramref name="son"/> est <c>null</c>.
        /// </exception>
        public bool GetCheck(string son) => chkSon[son];

        /// <summary>
        /// Donne le <c>CharFormatting</c> pour le <c>son</c>.
        /// </summary>
        /// <param name="son">Le son pour lequel on veut le <c>CharFormatting</c>. Voir 
        /// <c>sonMap</c> pour les valeurs autorisées.</param>
        /// <returns>le <c>CharFormatting</c> recherché.</returns>
        /// <exception cref="KeyNotFoundException"> si le <paramref name="son"/> n'est pas 
        /// un son connu. Voir <see cref="sonsValides"/></exception>
        /// <exception cref="ArgumentNullException"> si <paramref name="son"/> est <c>null</c>.
        /// </exception>
        public CharFormatting GetCF(string son) => cfSon[son];

        /// <summary>
        /// Définit un nouveau <see cref="CharFormatting"/> pour le son et met la checkbox
        /// correspondante à true.
        /// </summary>
        /// <param name="son">Le son dont on veut modifier le <see cref="CharFormatting"/>"/> et la 
        /// checkbox.</param>
        /// <param name="cf">Le nouveau <see cref="CharFormatting"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"> si le <paramref name="son"/> n'est pas 
        /// un son connu. Voir <see cref="sonsValides"/></exception>
        /// <exception cref="ArgumentNullException"> si un des paramètres est <c>null</c>.
        /// </exception>
        public void SetCbxAndCF(string son, CharFormatting cf)
        {
            logger.ConditionalDebug("SetCbxAndCF, son: {0}", son);
            UndoFactory.StartRecording(string.Format("cbx et format du son {0}", son));
            SetChkSon(son, true);
            SetCFSon(son, cf);
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Réinitialise la checkbox et le <see cref="CharFormatting"/> pour le son à <c>false</c>
        /// et noir.
        /// </summary>
        /// <param name="son">Le son à réinitialiser.</param>
        public void ClearSon(string son)
        {
            logger.ConditionalDebug("ClearSon");
            UndoFactory.StartRecording(string.Format("effacer son {0}", son));
            SetChkSon(son, false);
            SetCFSon(son, CharFormatting.BlackCF);
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Met toutes les checkboxes à <c>true</c>.
        /// </summary>
        public void SetAllCbxSons()
        {
            logger.ConditionalDebug("SetAllCbxSons");
            UndoFactory.StartRecording("Toutes les cbx");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, true);
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Met toutes les checkboxes à <c>false</c>.
        /// </summary>
        public void ClearAllCbxSons()
        {
            logger.ConditionalDebug("ClearAllCbxSons");
            UndoFactory.StartRecording("Aucune cbx");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, false);
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Initialise les sons à la configuration dite CERAS.
        /// </summary>
        public void SetCeras()
        {
            logger.ConditionalDebug("SetCeras");
            UndoFactory.StartRecording("Ceras");
            HashSet<string> cerasSons = new HashSet<string>();

            // o
            cerasSons.Add("o");
            SetCbxAndCF("o", cerasCF[(int)CERASColor.CERAS_o]);

            // a_tilda (son an)
            cerasSons.Add("an");
            SetCbxAndCF("an", cerasCF[(int)CERASColor.CERAS_an]);

            // e_tilda (son ain)
            cerasSons.Add("5");
            SetCbxAndCF("5", cerasCF[(int)CERASColor.CERAS_5]);

            // E (son è)
            cerasSons.Add("è");
            SetCbxAndCF("è", cerasCF[(int)CERASColor.CERAS_E]);

            // e (son é)
            cerasSons.Add("é");
            SetCbxAndCF("é", cerasCF[(int)CERASColor.CERAS_e]);

            // u (son ou)
            cerasSons.Add("u");
            SetCbxAndCF("u", cerasCF[(int)CERASColor.CERAS_u]);

            // w (son oi)
            // bold & black
            cerasSons.Add("oi");
            SetCbxAndCF("oi", cerasCF[(int)CERASColor.CERAS_oi]);

            // o_tilda (son on)
            cerasSons.Add("on");
            SetCbxAndCF("on", cerasCF[(int)CERASColor.CERAS_on]);

            // (sons eu et oeu)
            cerasSons.Add("2");
            SetCbxAndCF("2", cerasCF[(int)CERASColor.CERAS_eu]);

            // (oin)
            cerasSons.Add("oin");
            SetCbxAndCF("oin", cerasCF[(int)CERASColor.CERAS_oin]);

            // (son un)
            cerasSons.Add("1");
            SetCbxAndCF("1", cerasCF[(int)CERASColor.CERAS_1]); // underline

            // ph_muet
            cerasSons.Add("_muet");
            SetCbxAndCF("_muet", cerasCF[(int)CERASColor.CERAS_muet]);

            CleanAllSonsBut(cerasSons);
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Initialise le <c>ColConfWin</c> à la configuration dite "CERAS rosé"
        /// </summary>
        public void SetCerasRose()
        {
            logger.ConditionalDebug("SetCerasRose");
            UndoFactory.StartRecording("Ceras rosé");
            // est construit en delta par rapport à SetCeras
            SetCeras();
            // changer la couleur du é en rosé
            SetCFSon("é", cerasCF[(int)CERASColor.CERAS_rosé]);

            // activer le son ill
            SetChkSon("ill", true);
            SetCFSon("ill", cerasCF[(int)CERASColor.CERAS_ill]);

            // commenté le 14.05.2020 - J'ai eu un feedback de M. Tissot qui suggérait de le 
            // laisser mais avec une autre couleur.
            // désactiver le (oin)
            // SetChkSon("oin", false);
            // Set("oin", CharFormatting.BlackCF);

            IllRuleToUse = IllRule.ceras;
            UndoFactory.EndRecording();
        }

        /// <summary>
        /// Assigne un nouveau <see cref="CharFormatting"/> au son <paramref name="son"/>.
        /// </summary>
        /// <param name="son">Le son dont le <see cref="CharFormatting"/> doit être modifié. 
        /// Voir <c>sonMap</c> pour la liste des sons reconnus.</param>
        /// <param name="cf">Le nouveau <see cref="CharFormatting"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"> si le <paramref name="son"/> n'est pas 
        /// un son connu. Voir <see cref="sonsValides"/></exception>
        /// <exception cref="ArgumentNullException"> si un des paramètres est <c>null</c>.
        /// </exception>
        public void SetCFSon(string son, CharFormatting cf)
        {
            if (cf == null)
            {
                logger.Fatal("CharFormatting null passé à SetCFSon");
                throw new ArgumentNullException(nameof(cf), "CharFormatting null passé à SetCFSon");
            }
            if (son == null)
            {
                logger.Fatal("son null passé à SetCFSon");
                throw new ArgumentNullException(nameof(son), "son null passé à SetCFSon");
            }
            if (!sonsValides.Contains(son))
            {
                logger.Fatal("Les son {0} n'est pas autorisé.", son);
                throw new ArgumentOutOfRangeException(nameof(son), "Son non autorisé: " + son);
            }
            logger.ConditionalDebug("SetCFSon \'{0}\'", son);
            CharFormatting valCF;
            if(!(cfSon.TryGetValue(son, out valCF) && valCF == cf))
            {
                Debug.Assert(sonMap.ContainsKey(son), String.Format(ConfigBase.cultF, "{0} n'est pas un son connu", son));
                UndoFactory.ExceutingAction(new ColPhonAct(String.Format("Format son {0}", son),
                    this, son, cfSon[son], cf));
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
        /// <exception cref="ArgumentOutOfRangeException"> si le <paramref name="son"/> n'est pas 
        /// un son connu. Voir <see cref="sonsValides"/></exception>
        /// <exception cref="ArgumentNullException"> si un des paramètres est <c>null</c>.
        /// </exception>
        public void SetChkSon(string son, bool checkVal)
        {
            if (son == null)
            {
                logger.Fatal("son null passé à SetCFSon");
                throw new ArgumentNullException(nameof(son), "son null passé à SetCFSon");
            }
            if (!sonsValides.Contains(son))
            {
                logger.Fatal("Les son {0} n'est pas autorisé.", son);
                throw new ArgumentOutOfRangeException(nameof(son), "Son non autorisé: " + son);
            }
            logger.ConditionalDebug("SetChkSon \'{0}\' to {1}", son, checkVal);
            bool valCK;
            if (!(chkSon.TryGetValue(son, out valCK) && valCK == checkVal)) 
            {
                UndoFactory.ExceutingAction(new ColPhonAct(String.Format("Cbx son {0}", son),
                    this, son, chkSon[son], checkVal));
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
        public void SetDefaultBehaviourTo(DefBeh val)
        {
            logger.ConditionalDebug("SetDefaultBehaviourTo {0}", val);
            if (val != defBeh)
            {
                UndoFactory.ExceutingAction(new ColPhonAct("comportement par déf.",
                    this, defBeh, val));
                defBeh = val;
                switch (defBeh)
                {
                    case DefBeh.noir:
                        defChF = CharFormatting.BlackCF;
                        break;

                    case DefBeh.transparent:
                        defChF = CharFormatting.NeutralCF;
                        break;

                    default:
                        defChF = CharFormatting.NeutralCF;
                        break;
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
                SetCFSon("ill", CharFormatting.BlackCF);
                SetChkSon("ill", false);
                logger.ConditionalDebug("Son \"ill\" initialisé.");
            }
            if (!cfSon.ContainsKey("47"))
            {
                SetCFSon("47", CharFormatting.BlackCF);
                SetChkSon("47", false);
                logger.ConditionalDebug("Son \"47\" initialisé.");
            }
            if (!cfSon.ContainsKey("uni"))
            {
                SetCFSon("uni", CharFormatting.BlackCF);
                SetChkSon("uni", false);
                logger.ConditionalDebug("Son \"uni\" initialisé.");
            }
            if (!cfSon.ContainsKey("diz"))
            {
                SetCFSon("diz", CharFormatting.BlackCF);
                SetChkSon("diz", false);
                logger.ConditionalDebug("Son \"diz\" initialisé.");
            }
            if (!cfSon.ContainsKey("cen"))
            {
                SetCFSon("cen", CharFormatting.BlackCF);
                SetChkSon("cen", false);
                logger.ConditionalDebug("Son \"cen\" initialisé.");
            }
            if (!cfSon.ContainsKey("mil"))
            {
                SetCFSon("mil", CharFormatting.BlackCF);
                SetChkSon("mil", false);
                logger.ConditionalDebug("Son \"mil\" initialisé.");
            }
            if (cfPhon[(int)Phonemes.ji] == null)
            {
                cfPhon[(int)Phonemes.ji] = CharFormatting.BlackCF;
            }
        }

        // ------------------------------------------------------- Rule Flags ---------------------------------------------------

        /// <summary>
        /// Gives the value of the said rule flag.
        /// </summary>
        /// <param name="rf">identifier of the flag one wants the value of. Must be smaller than <c>last</c>.</param>
        /// <returns>The value of the flag.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="rf"/> est plus grand
        /// ou égal à <c>last</c></exception>
        public bool GetFlag(RuleFlag rf) => flags[(int)rf];

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  private  methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------


        private void Set(Phonemes p, CharFormatting chF) => cfPhon[(int)p] = chF;

        private void CleanAllSonsBut(HashSet<string> alreadyCleaned)
        {
            logger.ConditionalDebug("CleanAllSons");
            UndoFactory.StartRecording("Effacer tous les sons");
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
            {
                if (!alreadyCleaned.Contains(k.Key))
                {
                    SetCFSon(k.Key, CharFormatting.BlackCF);
                    SetChkSon(k.Key, false);
                }
            }
            UndoFactory.EndRecording();
        }

        private void InitColorMuettes()
        {
            logger.ConditionalDebug("InitColorMuettes");
            HashSet<string> sonsMuettes = new HashSet<string>();
            sonsMuettes.Add("_muet");
            UndoFactory.StartRecording("Initialiser muettes");
            SetCbxAndCF("_muet", cerasCF[(int)CERASColor.CERAS_muet]);
            CleanAllSonsBut(sonsMuettes);
            UndoFactory.EndRecording();
        }

        [OnDeserializing()]
        private void SetOptionalFieldsToDefaultVal(StreamingContext sc)
        {
            logger.ConditionalDebug("SetOptionalFieldsToDefaultVal");
            flags = new List<bool>((int)RuleFlag.last) { true, true, true };
            flags[(int)RuleFlag.IllLireCouleur] = false; // config par défaut
            defBeh = DefBeh.transparent;
            defChF = CharFormatting.NeutralCF;
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
