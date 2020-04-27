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

namespace ColorLib
{
    public enum CERASColors { CERAS_oi, CERAS_o, CERAS_an, CERAS_5, CERAS_E, CERAS_e, CERAS_u, CERAS_on, CERAS_eu,
        CERAS_oin, CERAS_muet }
    public enum PredefCols { black, darkYellow, orange, darkGreen, violet, darkBlue, red, brown, blue, green, grey, pureBlue,
        white, neutral}

    public delegate void ExecuteTask();

    public delegate void ExecTaskOnSon(string son);

    public class ColConfWin
    {
        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  public static members -----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------
        
        public const int nrSons = 40; // don't forget to increase in case...

        // phonemes: les couleurs par phonème qui peuvent être éditées et qui seront appliquées lors de la colorisation des phonèmes.
        // muettes: couleurs appliquées aux phonèmes dans le cas où on ne veut que coloriser les muettes

        public static RGB[] predefinedColors = new RGB[] {
            new RGB(000, 000, 000), // CERAS_oi     --> noir
            new RGB(222, 211, 000), // CERAS_o      --> jaune
            new RGB(237, 125, 049), // CERAS_an     --> orange
            new RGB(051, 153, 102), // CERAS_5      --> vert comme sapin
            new RGB(164, 020, 210), // CERAS_E      --> violet
            new RGB(000, 020, 208), // CERAS_e      --> (bleu) foncé
            new RGB(255, 000, 000), // CERAS_u      --> rouge
            new RGB(171, 121, 066), // CERAS_on     --> marron
            new RGB(071, 115, 255), // CERAS_eu     --> bleu
            new RGB(0, 200, 0),     // CERAS_oin    --> vert
            new RGB(166, 166, 166), // CERAS_muet   --> gris
            new RGB(0, 0, 255),     // bleuPur      --> bleu
            new RGB(255, 255, 255), // blanc        --> blanc
            new RGB(221, 221, 221), // neutre       --> gris // il est important qu'il ne s'agisse pas d'une couleur de WdColorIndex
        };

        public static CharFormatting[] predefCF; 
        // CharFormattings corresponding to the predefined colors.

        

        

        // -------------------------------------------------------------------------------------------------------------------
        // --------------------------------------------  private static members ----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static CharFormatting defChF; // CharFormatting returned for phonemes where the checkbox is not set
        public enum DefBeh { transparent, noir }
        public static DefBeh defBeh;

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
            {"j",   new string[2] {"[j]",   "fille"     } },
            {"ng",  new string[2] {"[ng]",  "parking"   } },
            {"gn",  new string[2] {"[gn]",  "ligne"     } },
            {"l",   new string[2] {"[l]",   "ville"     } },
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
        // -----------------------------------------------  public  members -------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public ExecuteTask updateAllSoundCbxAndButtons { set; private get; }
        // Method to call in order to update the checkboxes for the sounds (sons)
        public ExecTaskOnSon updateButton { set; private get; }
        public ExecTaskOnSon updateCbx { set; private get; }
        
        // -------------------------------------------------------------------------------------------------------------------
        // -----------------------------------------------  private  members -------------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        private bool disableSonsCBXEventsH = false; // avoid handling checkbox events 
        private PhonConfType pct; // définit la configuration par défaut appliquée par le constructeur

        // -------------------------------------- CharFormattings per Phonemes ------------------------------------------------
        private CharFormatting[] cfPhon;
        private bool[] chkPhon; // indique si le CharFormatting du phonème doit être appliqué ou non.

        // -------------------------------------- CharFormattings per "son"  ------------------------------------------------
        private Dictionary<string, CharFormatting> cfSon;
        private Dictionary<string, bool> chkSon; // indique si le CharFormatting du son doit être appliqué ou non.


        // -------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------  public static methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public static void Init()
        {
            predefCF = new CharFormatting[predefinedColors.Length + 1];  // +1 for the enutral entry
            for (int i = 0; i< predefinedColors.Length; i++)
                predefCF[i] = new CharFormatting(predefinedColors[i]);
            predefCF[(int)PredefCols.neutral] = new CharFormatting();

            defBeh = DefBeh.transparent;
            defChF = predefCF[(int)PredefCols.neutral];
        }

        // -------------------------------------------------- Mapping "sons" to text  ------------------------------------------

        public static string DisplayText(string son) => sonOutMap[son][0];

        public static string ExampleText(string son) => sonOutMap[son][1];

        // ------------------------------------------------------- About sons  -----------------------------------------------

        public static Dictionary<string, List<Phonemes>>.KeyCollection GetListOfSons() => sonMap.Keys;

        public static void DefaultBehaviourChangedTo(bool val)
        // true --> phonèmes non traitées en noir
        // false --> phonèmes non traitées sans changement de couleur
        {
            if (val)
            {
                defBeh = DefBeh.noir;
                defChF = predefCF[(int)PredefCols.black];
            }
            else
            {
                defBeh = DefBeh.transparent;
                defChF = predefCF[(int)PredefCols.neutral];
            }
        }

        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  public  methods ----------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        public ColConfWin(PhonConfType inPct)
        {
            cfPhon = new CharFormatting[(int)Phonemes.lastPhon];
            chkPhon = new bool[(int)Phonemes.lastPhon];

            cfSon = new Dictionary<string, CharFormatting>(sonMap.Count);
            chkSon = new Dictionary<string, bool>(sonMap.Count);

            pct = inPct;
            switch (pct)
            {
                case PhonConfType.phonemes:
                    InitColorCeras();
                    break;
                case PhonConfType.muettes:
                    InitColorMuettes();
                    break;
                default:
                    break;
            }
        }

        // ------------------------------------------------------- Phonemes --------------------------------------------------

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

        public bool GetCheck(string son) => chkSon[son];  // CheckBox value

        public CharFormatting Get(string son) => cfSon[son];
        // if son is not known an exception will be thrown...

        public void UpdateCF(string son, CharFormatting cf)
            // to be called by the UI when a new CharFormatting has to be set
        {
            Set(son, cf);
            updateButton(son);
            //colorizeAllSelPhons();
        }

        public void SetCbxAndCF(string son, CharFormatting cf)
        {
            logger.ConditionalTrace("SetCbxAndCF, son: {0}", son);
            SetChkSon(son, true);
            updateCbx(son);
            UpdateCF(son, cf);
        }

        public void ClearSon(string son)
        {
            Set(son, predefCF[(int)PredefCols.black]);
            SetChkSon(son, false);
            updateCbx(son);
            updateButton(son);
        }


        public void SetAllCbxSons()
        {
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, true);
            UpdCBXs();
        }

        public void ClearAllCbxSons()
        {
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, false);
            UpdCBXs();
        }

        public void SetCeras()
        {
            InitColorCeras();
            UpdCBXs();
        }

        public void DisableCbxSonsEventHandling() => disableSonsCBXEventsH = true;
        public void EnableCbxSonsEventHandling() => disableSonsCBXEventsH = false;

        public void SonConfCheckedChanged(string son, bool chkValue)
        // is called when a 'son' checkbox was clicked on the configurator TaskPane
        {
            if (!disableSonsCBXEventsH)
                // the check makes sure that even when several checkboxes are set at once, for example through the CERAS button,
                // we do not enter into a high number of recomputation of the phonemes to display.
                // there is a small risk that we get out of sync if an event that is not generated by the data update is generated.
                // Made more sense when we where Colorizing everything at each update.
            {
                SetChkSon(son, chkValue);
                updateButton(son);
            }
        }


        // -------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------  private  methods ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------------------

        
        private void Set(Phonemes p, CharFormatting chF) => cfPhon[(int)p] = chF;

        private void Set(string son, CharFormatting cf)
        // if son is not known an KeyNotFoundException will be thrown...
        {
            Debug.Assert(sonMap.ContainsKey(son), String.Format(BaseConfig.cultF, "{0} n'est pas un son connu", son));
            cfSon[son] = cf;
            List<Phonemes> lp = sonMap[son];
            foreach (Phonemes p in lp)
                Set(p, cf);
        }

        private void SetChkSon(string son, bool checkVal)
        {
            chkSon[son] = checkVal;
            List<Phonemes> lp = sonMap[son];
            foreach (Phonemes p in lp)
                chkPhon[(int)p] = checkVal;
        }

        private void CleanAllSons()
        {
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                Set(k.Key, predefCF[(int)PredefCols.black]);
            foreach (KeyValuePair<string, List<Phonemes>> k in sonMap)
                SetChkSon(k.Key, false);
        }

        private void InitColorCeras()
        {
            CleanAllSons();

            // o
            SetChkSon("o", true);
            Set("o", predefCF[(int)CERASColors.CERAS_o]);

            // a_tilda (son an)
            SetChkSon("an", true);
            Set("an", predefCF[(int)CERASColors.CERAS_an]);

            // e_tilda (son ain)
            SetChkSon("5", true);
            Set("5", predefCF[(int)CERASColors.CERAS_5]);

            // E (son è)
            SetChkSon("è", true);
            Set("è", predefCF[(int)CERASColors.CERAS_E]);

            // e (son é)
            SetChkSon("é", true);
            Set("é", predefCF[(int)CERASColors.CERAS_e]);

            // u (son ou)
            SetChkSon("u", true);
            Set("u", predefCF[(int)CERASColors.CERAS_u]);

            // w (son oi)
            // bold & black
            SetChkSon("oi", true);
            Set("oi", new CharFormatting(true, false, false, false, true, predefinedColors[(int)CERASColors.CERAS_oi], 
                false, predefinedColors[(int)PredefCols.neutral]));

            // o_tilda (son on)
            SetChkSon("on", true);
            Set("on", predefCF[(int)CERASColors.CERAS_on]);

            // (sons eu et oeu)
            SetChkSon("2", true);
            Set("2", predefCF[(int)CERASColors.CERAS_eu]);

            // (oin)
            SetChkSon("oin", true);
            Set("oin", predefCF[(int)CERASColors.CERAS_oin]);

            // (son un)
            SetChkSon("1", true);
            Set("1", new CharFormatting(false, false, true)); // underline

            // ph_muet
            SetChkSon("_muet", true);
            Set("_muet", predefCF[(int)CERASColors.CERAS_muet]);
        }

        private void InitColorMuettes()
        {
            CleanAllSons();
            SetChkSon("_muet", true);
            Set("_muet", predefCF[(int)CERASColors.CERAS_muet]);
        }

        private void UpdCBXs ()
        {
            updateAllSoundCbxAndButtons();
            //colorizeAllSelPhons();
        }

    }
}
