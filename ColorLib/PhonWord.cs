﻿/********************************************************************************
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
using System.Text;

namespace ColorLib
{
    public class PhonWord : Word
    {
        private List<PhonInW> phons;
        private List<SylInW> syls;
        
        /// <summary>
        /// Crée un <c>PhonWord</c>.
        /// </summary>
        /// <param name="inT">Le <see cref="TheText"/> sur le quel est construit le <c>PhonWord</c>.</param>
        /// <param name="inFirst">La position de la première lettre (zero based) du mot dans le texte.</param>
        /// <param name="inLast">La position de la dernière lettre du mot dans le texte.</param>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour la détection des phonèmes.</param>
        public PhonWord(TheText inT, int inFirst, int inLast, Config conf)
            : base(inT, inFirst, inLast)
        {
            syls = null;
            phons = new List<PhonInW>((inLast - inFirst) + 1);
            AutomAutomat.autom.FindPhons(this, conf);
        }

        /// <summary>
        /// Crée un <c>PhonWord</c> sur la base d'un <c>Word</c>.
        /// </summary>
        /// <param name="w">Le <c>Word</c> sur la base duquel le <c>PhonWord</c> doit être créé.</param>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour la détection des phonèmes.</param>
        public PhonWord(Word w, Config conf)
            :base(w)
        {
            syls = null;
            phons = new List<PhonInW>((Last - First) + 1);
            AutomAutomat.autom.FindPhons(this, conf);
        }

        /// <summary>
        /// Ajout un phonème (<see cref="PhonInW"/>) au <c>PhonWord</c>.
        /// </summary>
        /// <param name="piw">Le <see cref="PhonInW"/> à ajouter.</param>
        public void AddPhon(PhonInW piw)
        {
            phons.Add(piw);
            Debug.Assert(phons.Count <= GetWord().Length);
        }
          
        /// <summary>
        /// Applique le formatage de phonèmes défini dans le <see cref="PhonConfType"/> <c>pct</c> de 
        /// la <see cref="Config"/> <c>conf</c>. 
        /// </summary>
        /// <param name="conf">La <see cref="Config"/> à utiliser pour le formatage des phonèmes.</param>
        /// <param name="pct">Le <see cref="PhonConfType"/> définissant s'il s'agit de la configuration pour
        /// les phonèmes, les muettes, ...</param>
        public void ColorPhons(Config conf, PhonConfType pct)
        {
            foreach (PhonInW piw in phons)
                piw.PutColor(conf, pct);
        }

        /// <summary>
        /// Calcule et formate les syllabes avec la <see cref="Config"/> donnée.
        /// </summary>
        /// <param name="conf">La <c>Config</c> à utiliser pour savoir quelles options sont choisies 
        /// et le formatage à appliquer pour les syllabes.</param>
        public void ComputeAndColorSyls(Config conf)
        {
            SylInW siw;
            int i, j;
            SylConfig sylConfig = conf.sylConf;

            // Algorithme de Marie-Pierre
            if (syls == null)
            {
                syls = new List<SylInW>((Last - First) / 2);

                // créons une syllabe pour chaque phonème
                for (i = 0; i < phons.Count; i++)
                    syls.Add(new SylInW(phons[i]));

                if (syls.Count > 1)
                {
                    // Si le décodage est standard dupliquer les phonèmes qui comportent des consonnes doubles
                    if (sylConfig.DoubleConsStd)
                    {
                        for (i = 0; i < syls.Count; i++)
                        {
                            if (syls[i].EstConsonneRedoublee())
                            {
                                siw = new SylInW(syls[i]);
                                syls[i].ReduitADerniereLettre();
                                siw.ReduitAPremiereLettre();
                                syls.Insert(i, siw);
                            }
                        }
                    }

                    // mixer les doubles phonèmes de consonnes qui incluent [l] et [r] ; ex. : bl, tr, cr, chr, pl
                    // mixer les doubles phonèmes [y] et [i], [u] et [i,e_tilda,o_tilda]
                    // accrocher les lettres muettes aux lettres qui précèdent
                    for (i = 0; i < syls.Count - 1; i++)
                    {
                        if ((syls[i].EstBkptgdfv() && ((syls[i+1].P == Phonemes.l) || (syls[i + 1].P == Phonemes.R)))  // [bkptgdfv][lR]
                            ||
                            (((syls[i].P == Phonemes.y) && (syls[i + 1].P == Phonemes.i))  // ui
                             ||
                             ((syls[i].P == Phonemes.u) && ((syls[i+1].P == Phonemes.i) || (syls[i+1].P == Phonemes.e_tilda) || (syls[i+1].P == Phonemes.o_tilda))) // u(i|e_tilda|o_tilda)
                            )
                            ||
                            syls[i + 1].EstMuet()
                            )
                        {
                            // mixer les deux phonèmes puis raccourcir la chaîne
                            syls[i].AbsorbeSuivant(syls[i + 1]);
                            syls.RemoveAt(i + 1);
                            i--; // faire en sorte que la prochaine itération considère le nouveau phonème fusionné et son successeur
                        } 
                    }

                    // construire les syllabes par association de phonèmes consonnes et voyelles
                    // Les syllabes sont constituées de tout ce qui précède un phonème voyelle jusqu'à la syllabe précédente ou le début du mot.
                    // De plus si le phonème voyelle est suivi de deux consonnes, la première fait partie de la première syllabe.

                    i = 0;
                    j = 0; // début de la syllabe
                    while (i < syls.Count)
                    {
                        if(syls[i].EstVoyelle())
                        {
                            // fusionner les syllabes de j à i
                            for (int k = 0; k<(i-j); k++)
                            {
                                syls[j].AbsorbeSuivant(syls[j + 1]);
                                syls.RemoveAt(j + 1);
                            }
                            i = j;
                            j++;

                            // si les deux lettres qui suivent sont des consonnes, la première fait partie de la syllabe que nous venons de créer
                            // A condition qu'elles ne soient pas toutes les deux dans la même syllabe.
                            if (j < syls.Count)
                            {
                                int pos = syls[j].First; // position de la lettre suivante dans le texte sous-jacent
                                if ((syls[j].Last == syls[j].First) & (pos < this.Last) && EstConsonne(GetChar(pos)) && EstConsonne(GetChar(pos+1))) 
                                {
                                    syls[j - 1].EtendDroite(1);
                                    if (!syls[j].ReduitGauche(1))
                                        syls.RemoveAt(j);
                                }
                            }
                        }
                        i++;
                    } // while

                    // précaution de base : si pas de syllabes reconnues, on concatène simplement les phonèmes
                    if (j == 0)
                    {
                        // le mot ne comprend pas de voyelles --> une seule syllabe
                        syls.Clear();
                        siw = new SylInW(this, this.First, this.Last, Phonemes.firstPhon);
                        syls.Add(siw);
                    } 
                    else 
                    {
                        // il ne doit rester à la fin que les lettres muettes ou des consonnes qu'on ajoute à la dernière syllabe
                        while (j < syls.Count)
                        {
                            syls[j-1].AbsorbeSuivant(syls[j]);
                            syls.RemoveAt(j);
                            j++;
                        }
                    }

                    
                    if ((syls.Count > 1) && (phons[phons.Count - 1].P == Phonemes.q_caduc)) 
                    {
                        // s'il y a plus d'une syllabe, il y a aussi plus d'un phonème
                        if (sylConfig.mode == SylConfig.Mode.oral)
                        {
                            // si nous sommes en mode oral, les e caducs des dernières syllabes
                            // doivent être concaténés avec la syllabe précédente
                            syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                            syls.RemoveAt(syls.Count - 1);
                        }
                        else if (sylConfig.mode == SylConfig.Mode.poesie)
                        {
                            // voir http://mamiehiou.over-blog.com/article-versification-comment-compter-les-pieds-syllabes-d-un-vers-97149081.html
                            // dont nous nous inspirons ici. Il faut toutefois noter que quand le 
                            // "e" ne compte pas pour un pied, nous le relions simplement avec la
                            // syllabe précédente, ce qui n'est pas tout à fait correct au niveau
                            // de la prononciation.

                            const string blancs = " /t";  
                            // Si le mot suivant commence par une voyelle, --> une seule syllabe
                            string txt = T.GetSmallCapsText();
                            int k = Last + 1;
                            while (k < txt.Length && blancs.IndexOf(txt[k]) > -1)
                            {
                                k++;
                            }

                            int l = k;
                            while (l < txt.Length && (TextEl.EstConsonne(txt[l]) || TextEl.EstVoyelle(txt[l])))
                            {
                                l++;
                            }
                            // k est l'index du début du mot suivant, l celui de la lettre qui suit.
                            string nextWord = null;
                            if (l > k)
                            {
                                nextWord = txt.Substring(k, l - k);
                            }

                            if (k < txt.Length)
                            {
                                // il peut y avaoir un mot suivant.
                                if (Disjonction(nextWord))
                                {
                                    // La syllabe se prononce
                                }
                                else if (PasDisjonction(nextWord))
                                {
                                    // La syllabe ne se prononce pas
                                    syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                    syls.RemoveAt(syls.Count - 1);
                                }

                                // k est l'index du caractère suivant.
                                else if (txt[k] == 'y')
                                {
                                    // il y a des exceptions avec le y :-)
                                    // Le cas normal est que le y se comporte comme une consonne
                                    // le e-caduc forme une syllabe). Mais
                                    if (nextWord == "yeux" || nextWord == "yeuse" || true)
                                    {

                                    }
                                }
                                else if (TextEl.EstVoyelle(txt[k])) 
                                {
                                    // si on termine par un 's' il y a liaison et donc syllabe, 
                                    // sinon, pas de syllabe
                                    string wrd = ToLower();
                                    if (wrd[wrd.Length - 1] != 's')
                                    {
                                        // pas de liaison donc pas de syllabe.
                                        syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                        syls.RemoveAt(syls.Count - 1);
                                    }
                                }
                                else if (txt[k] == 'h')
                                {
                                    // Le 'h' semble mériter un dictionnaire à lui tout seul
                                }
                                else if (TextEl.EstConsonne(txt[k])) 
                                {
                                    // le e-caduc forme un syllabe. On laisse donc la syllabe pour
                                    // le phonème.
                                }
                                else
                                {
                                    // Il ne s'agit pas d'un lettre. Donc soit de la ponctuation,
                                    // une fin de ligne ou autre chose... On traite ce cas comme
                                    // une fin de vers --> pas de syllabe pour le e-caduc
                                    syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                    syls.RemoveAt(syls.Count - 1);
                                }
                            }
                            else
                            {
                                // C'est la fin du texte. Le e caduc ne forme pas une syllabe.
                                syls[syls.Count - 2].AbsorbeSuivant(syls[syls.Count - 1]);
                                syls.RemoveAt(syls.Count - 1);
                            }
                        }

                    }
                    
                } // if (syls.Count > 1)
            } // if (syls == null)

            // Mettre les syllabes en couleur
            foreach(SylInW s in syls)
                s.PutColor(conf);

            // s'il le faut, marquer par-dessus les phonemes muets.
            if (sylConfig.marquerMuettes)
                foreach(PhonInW piw in phons)
                    if (piw.EstMuet())
                        piw.PutColor(conf, PhonConfType.muettes);
        }

        // returns the phonetical representation of the PhonWord (notation from lexique.org)
        public string Phonetique()
        {
            StringBuilder sb = new StringBuilder(GetWord().Length - 1);
            foreach (PhonInW piw in phons)
            {
                sb.Append(piw.Phon2String());
            }
            return sb.ToString();
        }

        public override string AllStringInfo ()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.AllStringInfo());

            sb.Append("phons:");
            if (phons != null)
            {
                sb.AppendLine("");
                foreach (PhonInW piw in phons)
                    sb.AppendLine(piw.AllStringInfo());
            } else
                sb.AppendLine(" null");

            sb.Append("syls:");
            if (syls != null)
            {
                sb.AppendLine("");
                foreach (SylInW siw in syls)
                    sb.AppendLine(siw.AllStringInfo());
            } else
                sb.AppendLine(" null");

            return sb.ToString();
        }

        // for debugging returns the word with hyphens '-' betheen the syllabes
        public string Syllabes()
        {
            StringBuilder sb = new StringBuilder(GetWord().Length+4);
            for (int i = 0; i<syls.Count; i++) 
            {
                sb.Append(syls[i].ToString());
                if (i < syls.Count-1)
                    sb.Append("-");
            }
            return sb.ToString();
        }

        // For test purpose, clear all phons
        public void ClearPhons() => phons.Clear();

        static string[] disjonctions =
        {
            // 'h' aspiré --> 
            "hâblerie", "hâbleur", "hableuse", "hachage", "hache", "haché", "hachée", "hache-viande",
            "hache-légumes", "hache-paille", "hachement", "hacher", "hachette", "hacheur",
            "hachis", "hachisch", "hachoir", "hachure", "hachurer", "hackle", "hadal",
            "hadale", "hadaux", "haddock",
"haflinger",
"hafnium",
"hagard",
"haggis",
"haie",
"haillon",
"haillonneux",
"haillonneuse",
"haine",
"haineusement",
"haineux",
"haineuse",
"haïr",
"haire",
"haïssable",
"haïtien",
"haïtienne",
"halage",
"hâlage",
"halal",
"halbi",
"halbran",
"halbrené",
"halbrenée",
"halbrenée",
"halde",
"hâle",
"hâlé",
"hâlée",
"hale",
"halefis",
"hâle-haut",
"haler",
"hâler",
"haletant",
"halètement",
"haleter",
"haleur",
"haleuse",
"half",
"hall",
"halle",
"hallebarde",
"halo",
"hallux",
"halte",
"hamac",
"hamada",
"hamburger",
"hameau",
"hamman",
"hampe",
"hamster",
"hanche",
"hanchement",
"hancher",
"hand-ball",
"handballeur",
"handballeuse",
"handicap",
"handicapant",
"handicapante",
"handicapé",
"handicapée",
"handicaper",
"handicapeur",
"handisport",
"hangar",
"hanneton",
"hanon",
"hanse",
"hanter",
"hantise",
"happement",
"happening",
"happer",
"happy",
"haque",
"haquenée",
"haquet",
"hara-kiri",
"harangue",
"haranguer",
"harangueur",
"harangueuse",
"haras",
"harassant",
"harassante",
"harassé",
"harassée",
"harassement",
"harasser",
"harcelant",
"harcelante",
"harcèlement",
"harceler",
"hard",
"hard-bop",
"hard-core",
"harde",
"hardé",
"hard-edge",
"harder",
"hardes",
"hard ground",
"hardi",
"hardie",
"hardiesse",
"hardiment",
"hard",
"hard",
"hardware",
"harem",
"hareng",
"harengade",
"harengaison",
"harengère",
"harenguet",
"harengueux",
"harenguier",
"harenguière",
"haret",
"harfang",
"hargne",
"hargneusement",
"hargneux",
"hargneuse",
"haricot",
"haridelle",
"harissa",
"harka",
"harki",
"harnachement",
"harnacher",
"harnais",
"harnat",
"harnois",
"haro",
"harouelle",
"harpail",
"harpaye",
"harpe",
"harpe-cithare",
"harpette",
"harpie",
"harpiste",
"harpe-luth",
"harpocéras",
"harpodon",
"harpoise",
"harpon",
"harponnage",
"harponnement",
"harponner",
"harponneur",
"harpye",
"harrier",
"hart",
"hasard",
"hasarder",
"hasardeusement",
"hasardeux",
"hasardeuse",
"has",
"hasch",
"haschisch",
"hase",
"hâte",
"hâter",
"hatha",
"hâtier",
"hâtif",
"hâtive",
"hattéria",
"hauban",
"haubanage",
"haubaner",
"haubert",
"hausse",
"haussement",
"hausser",
"haussier",
"haussière",
"haut",
"haute",
"hautain",
"hautain",
"hautaine",
"haut-bar",
"hautbois",
"hautboïste",
"hautement",
"hauteur",
"hautin",
"hauturier",
"hauturière",
"havage",
"havane",
"hâve",
"havée",
"haveneau",
"havenet",
"haver",
"haveur",
"haveuse",
"havre",
"havresac",
"havrit",
"haylage",
"hayon",
"hé",
"heat",
"heaume",
"hein",
"héler",
"hem",
"henné",
"hennin",
"hennir",
"hennissant",
"hennissante",
"hennissement",
"hennuyer",
"hep",
"héraut",
"hère",
"hérissé",
"hérissée",
"hérissement",
"hérisser",
"hérisson",
"hérissonne",
"herniaire",
"hernie",
"hernié",
"herniée",
"hernieux",
"hernieuse",
"héron",
"héronneau",
"héronnier",
"héronnière",
"héros",
"héros",
"héroïne",
"hersage",
"herse",
"herser",
"hersillon",
"hêtraie",
"hêtre",
"heu !",
"heurt",
"heurter",
"heurtoir",
"hibou",
"hic",
"hickory",
"hideur",
"hideusement",
"hideux",
"hideuse",
"hiement",
"hiérarchie",
"hiérarchique",
"hiérarchiquement",
"hiérarchisation",
"hiérarchiser",
"hiérarque",
"hi",
"high",
"hilaire",
"hile",
"hindi",
"hip",
"hippie",
"hippy",
"hidjab",
"hissage",
"hit",
"ho",
"hobby",
"hobbyste",
"hobereau",
"hochement",
"hochepot",
"hochequeue",
"hocher",
"hochet",
"hockey",
"hockeyeur",
"hockeyeuse",
"holding",
"ho! hisse!",
"holà",
"holding",
"hold-up",
"hollandite",
"hollywoodien",
"hollywoodienne",
"homard",
"homarderie",
"homardier",
"home",
"home-trainer",
"hongre",
"hongrer",
"hongreur",
"hongreuse",
"hongroierie",
"hongroyage",
"hongroyer",
"hongroyeur",
"honning",
"honnir",
"honoris causa",
"honte",
"honteusement",
"honteux",
"honteuse",
"hooligan",
"hooliganisme",
"hop !",
"hop-je",
"hoquet",
"hoqueter",
"hoqueton",
"horde",
"horion",
"hormis",
"hornblende",
"hors",
"horsain",
"horsin",
"horse",
"horst",
"hot",
"hotinus",
"hotte",
"hottée",
"hotu",
"hou",
"houache",
"houaiche",
"houage",
"houblon",
"houe",
"houer",
"houille",
"houiller",
"houillère",
"houillère",
"houle",
"houlette",
"houleux",
"houleuse",
"houligan",
"hooligan",
"houliganisme",
"hooliganisme",
"houlque",
"houp",
"houppe",
"houppelande",
"houppette",
"houppier",
"hourd",
"hourdage",
"hourder",
"hourdir",
"hourdis",
"houri",
"hourque",
"hourra !",
"hourri",
"hourrite",
"hourvari",
"housche",
"houseau",
"house-boat",
"houspiller",
"houssage",
"housse",
"housser",
"housset",
"houssière",
"houst !",
"houx",
"hovéa",
"hoyau",
"hoyé",
"hoyée",
"huard",
"huart",
"hublot",
"huche",
"hucher",
"huchier",
"hue",
"huée",
"huer",
"huerta",
"huguenot",
"huipil",
"huir",
"huis",
"huit",
"huitain",
"huitaine",
"huitante",
"huitième",
"huitièmement",
"hulotte",
"hululation",
"hululement",
"hululer",
"hum",
"humantin",
"humer",
"hune",
"hunier",
"hunter",
"huppe",
"huppé",
"huppée",
"huque",
"hurdler",
"hure",
"hurlant",
"hurlement",
"hurler",
"hurleur",
"hurleuse",
"huron",
"huronne",
"hurrah !",
"hurricane",
"husky",
"hussard",
"hussarde",
"hutinet",
"hutte",
"hutteau",
        };

        /// <summary>
        /// Analyse si le mot est une exception et se comporte comme s'il commençait pas une
        /// consonne en ce qui concerne les liaisons et le pronnonciation des syllabes
        /// qui précèdent. Typiquement on aura un 'le' ou un 'la' devant.
        /// </summary>
        /// <param name="wrd">Le mot à analyser en minuscules. Peut-être <c>null</c>.</param>
        /// <returns>Le mot fait partie de la liste des exceptions.</returns>
        private bool Disjonction(string wrd)
        {
            bool toReturn = false;
            return toReturn;
        }

        static string[] pasDisjonctions =
        {
            "",
        };

        /// <summary>
        /// Analyse si le mot est une exception et se comporte comme s'il commençait pas une
        /// voyelle en ce qui concerne les liaisons et le pronnonciation des syllabes
        /// qui précèdent. Typiquement on aura un "l'" devant.
        /// </summary>
        /// <param name="wrd">Le mot à analyser en minuscules. Peut-être <c>null</c>.</param>
        /// <returns>Le mot fait partie de la liste des exceptions.</returns>
        private bool PasDisjonction(string wrd)
        {
            bool toReturn = false;
            return toReturn;
        }


    }
}
