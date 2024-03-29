﻿/********************************************************************************
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

namespace ColorLib
{
    /// <summary>
    /// Les phonèmes qui sont reconus par l'automate.
    /// </summary>
    public enum Phonemes
    {
        firstPhon,

        // Voyelles
        /// <summary>1 --> 'a'  bat, plat -> A | A de X-Sampa</summary>
        a, 
        /// <summary>2 le e de je, te, me, le , se... --> schwa '°', le e de abordera, schwa élidable | @ de X-Sampa</summary>
        q, 
        /// <summary>3 e final p. ex de correctes --> schwa '°', le e de abordera, schwa élidable | @ de X-Sampa</summary>
        q_caduc,
        /// <summary>4 --> 'i'  lit, émis -> I</summary>
        i, 
        /// <summary>5 sot, coefficient, automne --> 'O' éloge, fort --> o ouvert</summary>
        o,
        /// <summary>6 eau, au, --> 'o' peau --> o fermé</summary>
        o_comp,
        /// <summary>7 ou --> 'u' roue --> Ou</summary>
        u,
        /// <summary>8 u --> 'y' lu --> U</summary>
        y,
        /// <summary>9 é --> 'e' été --> e-fermé</summary>
        e,
        /// <summary>10 è --> 'E' paire, treize --> e-ouvert</summary>
        E,
        /// <summary>11 è --> 'E' paire, treize --> e-ouvert</summary>
        E_comp,
        /// <summary>12 é --> 'e' clef, nez --> e-fermé</summary>
        e_comp,
        /// <summary>13 in --> cinq  '5', cinq, linge --> in (voy. nasale) | e~ de X-Sampa</summary>
        e_tilda,
        /// <summary>14 an --> an '@', ange --> an (voy. nasale) | a~ de X-Sampa</summary>
        a_tilda,
        /// <summary>15 on --> on '§', on, savon --> on (voy. nasale) | o~ de X-Sampa</summary>
        o_tilda,
        /// <summary>16 un --> un '1', un, parfum --> un (voy. nasale) | 9~ de X-Sampa</summary>
        x_tilda,
        /// <summary>17 eu --> deux '2', deux, oeuf nous renonçons à distinguer x2 et x9</summary>
        x2,
                     // x9,      // oeil, oeuf --> neuf  '9', oeuf, peur --> e-ouvert Nous renonçons à distinguer x2 et x9
        /// <summary>18 Spécialement introduit pour identifier des deux lettres qui donnent --> 'wa'</summary>
        oi,
        /// <summary>19 oin de poing, oint --> 'w5'</summary>
        w_e_tilda,  
                     // w_E_comp,   // oue de ouest, oued --> 'wE' Le cas particulier crée plus de confusion qu'il n'aide
                     // w_i,        // oui, kiwi --> 'wi' Nous renonçons à ce cas particulier. kiwi donnera 'kiwi' en phonétique :-)

        // Semi-voyelles
        /// <summary>20 kiwi, sanwich, steward  --> 'w' pour le lexique.</summary>
        w,
        /// <summary>21 paille, ail, thaï, païen --> 'j' yeux, paille --> y (semi-voyelle)</summary>
        j,
        /// <summary>22 ng en fin de mot, prononcé à l'anglaise (string) --> 'iG'</summary>
        J,
        /// <summary>23 le son [ij] de affrioler -->´'ij'</summary>
        i_j,
        /// <summary>24 gn --> 'N' agneau, vigne --> gn (c. nasale palatine)  | J de X-Sampa</summary>
        N,

        // Consonnes
        /// <summary>25 p --> 'p' père, soupe --> p (occlusive)</summary>
        p,
        /// <summary>26 b --> 'b' bon, robe --> b (occlusive)</summary>
        b,
        /// <summary>27 t --> 't' terre, vite --> t (occlusive)</summary>
        t,
        /// <summary>28 d --> 'd' dans aide --> d (occlusive)</summary>
        d,
        /// <summary>29 k --> 'k' carré, laque --> k (occlusive)</summary>
        k,
        /// <summary>30 g --> 'g' gare, bague --> g (occlusive)</summary>
        g,
        /// <summary>31 f --> 'f' feu, neuf --> f (fricative)</summary>
        f,
        /// <summary>32 v --> 'v' vous, rêve --> v (fricative)</summary>
        v,
        /// <summary>33 s --> 's' sale, dessous --> s (fricative)</summary>
        s,
        /// <summary>34 z --> 'z' zéro, maison --> z (fricative)</summary>
        z,
        /// <summary>35 ch --> 'S' chat, tâche --> ch (fricative)</summary>
        S,
        /// <summary>36 j --> 'Z' gilet, mijoter --> ge (fricative)</summary>
        Z,
        /// <summary>37 m --> 'm' main, ferme --> m (cons. nasale)</summary>
        m,
        /// <summary>38 n --> 'n' nous, tonne --> n (cons. nasale</summary>
        n,
        /// <summary>39 l --> 'l' lent, sol --> l (liquide)</summary>
        l,
        /// <summary>40 R --> 'R' rue, venir --> R</summary>
        R,
        /// <summary>41 ph de philosophie --> 'f'</summary>
        f_ph,
        /// <summary>42 qu de quel --> 'k'</summary>
        k_qu,
        /// <summary>43 g de gueule ou de guignol --> 'g'</summary>
        g_u,
        /// <summary>44 son s dans ceci --> 's'</summary>
        s_c,
        /// <summary>45 son s dans partition --> 's'</summary>
        s_t,
        /// <summary>46 sons s dans six, dix --> 's'</summary>
        s_x,
        /// <summary>47 s se prononce z raser --> 'z'</summary>
        z_s,
        /// <summary>48 son x de rixe --> 'ks'</summary>
        ks,
        /// <summary>49 son x de examiner, exact --> 'gz'</summary>
        gz,

        /// <summary>50 nt ou ent des verbes conjugués --> muet ""</summary>
        verb_3p,
        /// <summary>51 --> muet ""</summary>
        _muet,

        // o_ouvert,
        // O,       // Marie-Pierre a renoncé à l'analyse o ouvert / fermé. Nous nous contentons de son travail.
        // z_g, ex z^, remplacé par Z
        // w5,  ???

        // créons des réserves pour assurer la compatibilité des fichiers de sauvegarde futurs, si nous introduisons de 
        // nouveaux phonèmes. Comme nous créons des tableaux qui ont cette longueur, ça pourrait poser des problèmes,
        // si les tableaux n'ont pas la même dimension.
        // Les tests semblent prouver que la taille de cet enum n'a pas d'impact sur la compatibilité des sauvegardes...
        // comme ça ne mange presque pas de pain, gardons-les quand même.

        /// <summary>52 utilisé dans la version CERAS des règles pour "ill" et "il" correspond au son 'j'</summary>
        j_ill,
        /// <summary>53 utilisé dans la version CERAS des règles pour "ill" (et "il") correspond au son 'ij'</summary>
        i_j_ill,
        /// <summary>54 le son [j] quand il est produit par la lettre i seule devant une voyelle. 
        /// --> 'j'</summary>
        ji,
        /// <summary>55 pour les chiffres 0 .. 9 qui ne correspondent pas aux autres critères</summary>
        chiffre,
        /// <summary>56 pour les chiffres, quand ils sont en position d'unité dans un nombre</summary>
        unité,
        /// <summary>57 pour les chiffres, quand ils sont en position de dizaine dans un nombre</summary>
        dizaine,
        /// <summary>58 pour les chiffres, quand ils sont en position de centaine dans un nombre</summary>
        centaine,
        /// <summary>59 pour les chiffres, quand ils sont en position de milliers dans un nombre</summary>
        milliers,

        /// <summary>used to iterate through all values. We could avoid this by using a Dictionary, 
        /// but the advantage seems limited...</summary>
        lastPhon
    }

    public enum PhonLexique
        // Nous utilisons les sons comme définis par lexique.org. ils sont proches de X-Sampa avec quelques différences.
        // LireCouleur semble fidèle à X-Sampa pour cet aspect.
    {
        // Voyelles
        a, // bat, plat -> A | A de X-Sampa
        i, // lit, émis -> I
        y, // lu --> U
        u, // roue --> Ou
        o, // peau --> o fermé
        O, // éloge, fort --> o ouvert
        e, // été --> e-fermé
        E, // paire, treize --> e-ouvert
        deux, // '2', deux, e-fermé
        neuf, // '9', oeuf, peur --> e-ouvert
        cinq, // '5', cinq, linge --> in (voy. nasale) | e~ de X-Sampa
        un, // '1', un, parfum --> un (voy. nasale) | 9~ de X-Sampa
        an, // '@', ange --> an (voy. nasale) | a~ de X-Sampa
        on, // '§', on, savon --> on (voy. nasale) | o~ de X-Sampa

        // Semi-Voyelles
        j, // yeux, paille --> y (semi-voyelle)
        ui, // '8', huit, lui --> ui (semi-voyelle) | H de X-Sampa
        w,  // steward, kiwi --> w (semi-voyelle) moi donne par exemple "mwa"

        // Consonnes
        p, // père, soupe --> p (occlusive)
        b, // bon, robe --> b (occlusive)
        t, // terre, vite --> t (occlusive)
        d, // dans aide --> d (occlusive)
        k, // carré, laque --> k (occlusive)
        g, // gare, bague --> g (occlusive)
        f, // feu, neuf --> f (fricative)
        v, // vous, rêve --> v (fricative)
        s, // sale, dessous --> s (fricative)
        z, // zéro, maison --> z (fricative)
        S, // chat, tâche --> ch (fricative)
        Z, // gilet, mijoter --> ge (fricative)
        m, // main, ferme --> m (cons. nasale)
        n, // nous, tonne --> n (cons. nasale)
        N, // agneau, vigne --> gn (c. nasale palatine)  | J de X-Sampa
        l, // lent, sol --> l (liquide)
        R, // rue, venir --> R
        x, // jota --> jota (emprunt espagnol)
        G,  // camping  -> ng (emprunt anglais) | G de X-Sampa

        // Ajouts - voir manuel LireCouleur §4.3.1

        muet,
        sonE,   // le e de je, te, le que LireCouleur note q et Lexique °
        schwa,  // '°', le e de abordera, schwa élidable | @ de X-Sampa
        ks,     // boxe
        gz      // exemple


    } // enum PhonLexique


    public class PhonInW : TextEl
    {
        private const string copied = "Copied";

        public Phonemes P { get; protected set; }
        protected PhonWord PW {get; private set;} // the PhonWord the PhonInW is part of
        private string firedRuleName { get; set; } // name of the rule that was used to define the phoneme

        // Mapping vers la représentation lexique.org des phonèmes.
        static private Dictionary<Phonemes, string> lexMap = new Dictionary<Phonemes, string>
        {
            { Phonemes.a,           "a" },
            { Phonemes.q,           "°" },
            { Phonemes.q_caduc,     ""  }, // e final "" est plus proche de Lexique, normalement il faudrait plutôt °
            { Phonemes.i,           "i" },
            { Phonemes.o,           "O" },
            { Phonemes.o_comp,      "o" },
            { Phonemes.u,           "u" },
            { Phonemes.y,           "y" },
            { Phonemes.e,           "e" },
            { Phonemes.E,           "E" },
            { Phonemes.E_comp,      "E" },
            { Phonemes.e_comp,      "e" },
            { Phonemes.e_tilda,     "5" },
            { Phonemes.a_tilda,     "@" },
            { Phonemes.o_tilda,     "§" },
            { Phonemes.x_tilda,     "1" },
            // { Phonemes.x9,          "9" }, plus de distinction entre x2 et x9
            { Phonemes.x2,          "2" },
            { Phonemes.oi,          "wa" },
            { Phonemes.w,           "w" },
            { Phonemes.i_j,         "ij" },
            { Phonemes.j,           "j" },
            { Phonemes.J,           "G" },
            { Phonemes.p,           "p" },
            { Phonemes.b,           "b" },
            { Phonemes.t,           "t" },
            { Phonemes.d,           "d" },
            { Phonemes.k,           "k" },
            { Phonemes.g,           "g" },
            { Phonemes.f,           "f" },
            { Phonemes.v,           "v" },
            { Phonemes.s,           "s" },
            { Phonemes.z,           "z" },
            { Phonemes.S,           "S" },
            { Phonemes.Z,           "Z" },
            { Phonemes.m,           "m" },
            { Phonemes.n,           "n" },
            { Phonemes.N,           "N" },
            { Phonemes.l,           "l" },
            { Phonemes.R,           "R" },
            { Phonemes.w_e_tilda,   "w5" },
            // { Phonemes.w_E_comp,    "wE" }, // cas enlevé de l'automate
            // { Phonemes.w_i,         "wi" }, résultera en 'wi' sans besoin de le traiter comme un cas particulier.
            { Phonemes.f_ph,        "f" },
            { Phonemes.k_qu,        "k" },
            { Phonemes.g_u,         "g" },
            { Phonemes.s_c,         "s" },
            { Phonemes.s_t,         "s" },
            { Phonemes.s_x,         "s" },
            { Phonemes.z_s,         "z" },
            { Phonemes.ks,          "ks" },
            { Phonemes.gz,          "gz" },
            { Phonemes.verb_3p,     "" },
            { Phonemes._muet,       "" },
            { Phonemes.j_ill,       "j" },
            { Phonemes.i_j_ill,     "ij" },
            { Phonemes.ji,          "j" },
            { Phonemes.chiffre,     "" },
            { Phonemes.unité,       "" },
            { Phonemes.dizaine,     "" },
            { Phonemes.centaine,    "" },
            { Phonemes.milliers,    "" },
            { Phonemes.firstPhon,   "FIRSTPHON" },
            { Phonemes.lastPhon,    "LASTPHON" }
        };

        /// <summary>
        /// Traduction de la représentation ColSimpl étendue (ça devient un peu compliqué) vers les 
        /// phonèmes utilisés par PhonInW et PhonWord.
        /// ColSimpl correspond en fait à Lexique sans la distinction entre "o" et "O". Les extensions
        /// attribuent une représentation en une lettre pour les phonèmes "spéciaux" de colorization.
        /// Extensions: 
        ///     "#" pour muet, 
        ///     "ç" pour e caduc, 
        ///     "4" pour les chiffres, 
        ///     "3" pour oin, 
        ///     "6" pour oi, 
        ///     "x" pour ks, 
        ///     "X" pour gz,
        ///     "%" pour ill
        ///     "/" pour ij
        /// 
        /// </summary>
        private static Dictionary<char, Phonemes> colSE2phoneme = new Dictionary<char, Phonemes> // don't forget to increase in case...
        {
            {'a',   Phonemes.a},
            {'°',   Phonemes.q},
            {'i',   Phonemes.i},
            {'y',   Phonemes.y},
            {'1',   Phonemes.x_tilda},
            {'u',   Phonemes.u},
            {'e',   Phonemes.e},
            {'o',   Phonemes.o},
            {'E',   Phonemes.E},
            {'@',   Phonemes.a_tilda}, // an
            {'§',   Phonemes.o_tilda}, // on
            {'2',   Phonemes.x2},
            {'6',   Phonemes.oi}, // oi
            {'5',   Phonemes.e_tilda},
            {'w',   Phonemes.w},
            {'j',   Phonemes.j},
            {'%',   Phonemes.j_ill}, // ill
            {'G',   Phonemes.J}, // ng
            {'N',   Phonemes.N}, // gn
            {'l',   Phonemes.l},
            {'v',   Phonemes.v},
            {'f',   Phonemes.f},
            {'p',   Phonemes.p},
            {'b',   Phonemes.b},
            {'m',   Phonemes.m},
            {'z',   Phonemes.z},
            {'s',   Phonemes.s},
            {'t',   Phonemes.t},
            {'d',   Phonemes.d},
            {'x',   Phonemes.ks}, // ks
            {'X',   Phonemes.gz}, // gz
            {'R',   Phonemes.R},
            {'r',   Phonemes.R},
            {'n',   Phonemes.n},
            {'Z',   Phonemes.Z}, // ge
            {'S',   Phonemes.S}, // ch
            {'k',   Phonemes.k},
            {'g',   Phonemes.g},
            {'/',   Phonemes.i_j},
            {'3',   Phonemes.w_e_tilda}, // oin
            {'4',   Phonemes.chiffre},
            {'#',   Phonemes._muet},
            {'ç',   Phonemes.q_caduc}
        };

        static private Dictionary<Phonemes, string> phon2colSE = new Dictionary<Phonemes, string>
        {
            { Phonemes.a,           "a" },
            { Phonemes.q,           "°" },
            { Phonemes.q_caduc,     "ç" }, // e final "" est plus proche de Lexique, normalement il faudrait plutôt °
            { Phonemes.i,           "i" },
            { Phonemes.o,           "o" },
            { Phonemes.o_comp,      "o" },
            { Phonemes.u,           "u" },
            { Phonemes.y,           "y" },
            { Phonemes.e,           "e" },
            { Phonemes.E,           "E" },
            { Phonemes.E_comp,      "E" },
            { Phonemes.e_comp,      "e" },
            { Phonemes.e_tilda,     "5" },
            { Phonemes.a_tilda,     "@" },
            { Phonemes.o_tilda,     "§" },
            { Phonemes.x_tilda,     "1" },
            { Phonemes.x2,          "2" },
            { Phonemes.oi,          "6" },
            { Phonemes.w,           "w" },
            { Phonemes.i_j,         "/" },
            { Phonemes.j,           "j" },
            { Phonemes.J,           "G" },
            { Phonemes.p,           "p" },
            { Phonemes.b,           "b" },
            { Phonemes.t,           "t" },
            { Phonemes.d,           "d" },
            { Phonemes.k,           "k" },
            { Phonemes.g,           "g" },
            { Phonemes.f,           "f" },
            { Phonemes.v,           "v" },
            { Phonemes.s,           "s" },
            { Phonemes.z,           "z" },
            { Phonemes.S,           "S" },
            { Phonemes.Z,           "Z" },
            { Phonemes.m,           "m" },
            { Phonemes.n,           "n" },
            { Phonemes.N,           "N" },
            { Phonemes.l,           "l" },
            { Phonemes.R,           "R" },
            { Phonemes.w_e_tilda,   "3" },
            { Phonemes.f_ph,        "f" },
            { Phonemes.k_qu,        "k" },
            { Phonemes.g_u,         "g" },
            { Phonemes.s_c,         "s" },
            { Phonemes.s_t,         "s" },
            { Phonemes.s_x,         "s" },
            { Phonemes.z_s,         "z" },
            { Phonemes.ks,          "x" },
            { Phonemes.gz,          "X" },
            { Phonemes.verb_3p,     "#" },
            { Phonemes._muet,       "#" },
            { Phonemes.j_ill,       "j" },
            { Phonemes.i_j_ill,     "/" },
            { Phonemes.ji,          "j" },
            { Phonemes.chiffre,     "4" },
            { Phonemes.firstPhon,   "" },
            { Phonemes.lastPhon,    "" }
        };

        /// <summary>
        /// Retourne le phonèmes correspondant au son <paramref name="c"/> de la notation
        /// ColSimpl étendue.
        /// </summary>
        /// <param name="c">Un son de la notation ColSimpl étendue.</param>
        /// <returns>Le phonème correspondant.</returns>
        /// <exception cref="KeyNotFoundException">Si <paramref name="s"/> n'est pas un son
        /// connu.</exception>
        public static Phonemes ColSE2phon(char c)
        {
            return colSE2phoneme[c];
        }

        private static HashSet<Phonemes> voyelles = new HashSet<Phonemes> { Phonemes.a, Phonemes.q, 
            Phonemes.q_caduc, Phonemes.i, Phonemes.o, Phonemes.o_comp, Phonemes.u, Phonemes.y, 
            Phonemes.e, Phonemes.E, Phonemes.E_comp, Phonemes.e_comp, Phonemes.e_tilda,
            Phonemes.a_tilda, Phonemes.o_tilda, Phonemes.x_tilda, Phonemes.x2, Phonemes.oi, 
            Phonemes.w_e_tilda, Phonemes.i_j, Phonemes.i_j_ill};

        private static HashSet<Phonemes> consonnes = new HashSet<Phonemes> { Phonemes.p, Phonemes.b, 
            Phonemes.t, Phonemes.d, Phonemes.k, Phonemes.g, Phonemes.f, Phonemes.v, Phonemes.s,
            Phonemes.z, Phonemes.S, Phonemes.Z, Phonemes.m, Phonemes.n, Phonemes.l, Phonemes.R, 
            Phonemes.f_ph, Phonemes.k_qu, Phonemes.g_u, Phonemes.s_c, Phonemes.s_t, Phonemes.s_x,
            Phonemes.z_s,Phonemes.ks, Phonemes.gz};

        private static HashSet<Phonemes> semiVoyelles = new HashSet<Phonemes> { Phonemes.w,
            Phonemes.J, Phonemes.N, Phonemes.j, Phonemes.j_ill, Phonemes.ji };

        private static HashSet<Phonemes> muet = new HashSet<Phonemes> { Phonemes.verb_3p, Phonemes._muet,
            Phonemes.chiffre};

        public static bool IsPhonVoyelle(Phonemes p) => voyelles.Contains(p);
        public static bool IsPhonConsonne(Phonemes p) => consonnes.Contains(p);
        public static bool IsPhonSemiVoyelle(Phonemes p) => semiVoyelles.Contains(p);
        public static bool IsPhonMuet(Phonemes p) => muet.Contains(p);


        /// <summary>
        /// Indique si le <c>PhonInW</c> correspond à un son "consonne".
        /// </summary>
        /// <returns>Le son est une "consonne".</returns>
        public bool EstConsonne() => IsPhonConsonne(P);

        /// <summary>
        /// Indique si le <c>PhonInW</c> correspond à un son "voyelle".
        /// </summary>
        /// <param name="forceDierese">Indique si la diérèse doit être forcée.</param>
        /// <returns>Le son est une "voyelle".</returns>
        public bool EstVoyelle(bool forceDierese = false)
        {
            bool toReturn = false;
            if (P == Phonemes.ji)
            {
                toReturn = forceDierese;
            }
            else
            {
                toReturn = IsPhonVoyelle(P);
            }
            return toReturn;
        }
            
        public bool EstSemiVoyelle() => IsPhonSemiVoyelle(P);
        
        /// <summary>
        /// Indique si le phonème corespond à un son muet.
        /// </summary>
        /// <returns>Le son est muet</returns>
        public bool EstMuet() => (muet.Contains(P));

        static public List<Phonemes> String2Phons(string s)
        {
            List<Phonemes> toReturn = new List<Phonemes>(1);
            foreach (KeyValuePair<Phonemes, string> k in lexMap)
            {
                if (k.Value == s)
                    toReturn.Add(k.Key);
            }
            return toReturn;
        }

        public string Phon2String() => lexMap[P];
        // Retourne la correspondance "Lexique" (voir lexique.org) du phonème.

        /// <summary>
        /// La correspondance au format ColSE du phonème. Chaque phonème est représenté par un caractère
        /// </summary>
        /// <returns></returns>
        public string Phon2ColSE() => phon2colSE[P];
        

        /// <summary>
        /// Crée un <see cref="PhonInW"/> et l'ajoute à la liste des phonèmes de 
        /// <paramref name="inW"/>
        /// </summary>
        /// <param name="inW">Le <see cref="PhonWord"/> à l'intérieur duquel se trouve le 
        /// <see cref="PhonInW"/></param>
        /// <param name="inBeg">Position dans le mot (<paramref name="inW"/>) de la première
        /// lettre qui correspond au phonème. 0 correspond à la première lettre du mot.</param>
        /// <param name="inEnd">Position dans le mot (<paramref name="inW"/>) de la dernière
        /// lettre qui correspond au phonème. 0 correspond à la première lettre du mot.</param>
        /// <param name="inP">Le phonème</param>
        /// <param name="ruleName">La règle qui a détecté le phonème.</param>
        public PhonInW(PhonWord inW, int inBeg, int inEnd, Phonemes inP, string ruleName)
            : base(inW.T, inW.First+ inBeg, inW.First + inEnd)
        {
            PW = inW;
            P = inP;
            firedRuleName = ruleName;
            inW.AddPhon(this);
        }

        /// <summary>
        /// Crée un <see cref="PhonInW"/> et l'ajoute à la liste des phonèmes de 
        /// <paramref name="inW"/>
        /// </summary>
        /// <param name="inW">Le <see cref="PhonWord"/> à l'intérieur duquel se trouve le 
        /// <see cref="PhonInW"/></param>
        /// <param name="inBeg">Position dans le mot (<paramref name="inW"/>) de la première
        /// lettre qui correspond au phonème. 0 correspond à la première lettre du mot.</param>
        /// <param name="inEnd">Position dans le mot (<paramref name="inW"/>) de la dernière
        /// lettre qui correspond au phonème. 0 correspond à la première lettre du mot.</param>
        /// <param name="colSE">Le phonème au format ColSimplifiéEtendu.</param>
        /// <param name="ruleName">La règle qui a détecté le phonème.</param>
        public PhonInW(PhonWord inW, int inBeg, int inEnd, char colSE, string ruleName)
            : base(inW.T, inW.First + inBeg, inW.First + inEnd)
        {
            PW = inW;
            P = ColSE2phon(colSE);
            firedRuleName = ruleName;
            inW.AddPhon(this);
        }

        /// <summary>
        /// Crée un <see cref="PhonInW"/>. Il n'est PAS ajouté à la liste des phonèmes du mot.
        /// </summary>
        /// <param name="inW">Le <see cref="PhonWord"/> à l'intérieur duquel se trouve le 
        /// <see cref="PhonInW"/></param>
        /// <param name="inBeg">Postion dans le <see cref="TheText"/> de base, de la première   
        /// lettre correspondant au phonème.</param>
        /// <param name="inEnd">Postion dans le <see cref="TheText"/> de base, de la dernière   
        /// lettre correspondant au phonème.</param>
        /// <param name="inP">Le phonème</param>
        public PhonInW(PhonWord inW, int inBeg, int inEnd, Phonemes inP)
            // inBeg and inEnd are relative to the original TheText!
            : base(inW.T, inBeg, inEnd)
        {
            PW = inW;
            P = inP;
            firedRuleName = "NoRule";
        }

        protected PhonInW(PhonInW piw)
            : base(piw)
        {
            PW = piw.PW;
            P = piw.P;
            firedRuleName = copied;
        }

        public override void PutColor(Config conf) => PutColor(conf, PhonConfType.phonemes);

        public void PutColor (Config conf, PhonConfType pct) => 
            base.SetCharFormat(conf.colors[pct].GetCF(P, this.ToLowerString()));

        public override string AllStringInfo()
        {
            return String.Format(ConfigBase.cultF, "{0, -25} Rule: {1, -7} --> Phon: {2, -8} LexSound: {3, -3}",
                base.AllStringInfo(), firedRuleName, P, Phon2String());
        }

        /// <summary>
        /// Retourne le texte correspondant au phonème, et sa représentation phonétique séparés
        /// par '-'
        /// </summary>
        /// <returns></returns>
        public string ExceptDict()
        {
            return String.Format(ConfigBase.cultF, "{0}-{1}", ToLowerString(),phon2colSE[P]);
        }

        //public override string ToString()
        //{
        //    return String.Format(BaseConfig.cultF, "Text: {0, -5} Rule: {1, -7} --> Phon: {2, -8} LexSound: {3, -3}", 
        //        base.ToString(), firedRuleName, P, Phon2String());
        //}

    }
}
