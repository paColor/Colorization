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

namespace ColorLib
{

    public enum Phonemes
    {
        firstPhon,

        // Voyelles
        a,       // --> 'a'  bat, plat -> A | A de X-Sampa
        q,       // le e de je, te, me, le , se... --> schwa '°', le e de abordera, schwa élidable | @ de X-Sampa
        q_caduc, // e final p. ex de correctes --> schwa '°', le e de abordera, schwa élidable | @ de X-Sampa
        i,       // --> 'i'  lit, émis -> I
        o,       // sot, coefficient, automne --> 'O' éloge, fort --> o ouvert
        o_comp,  // eau, au, --> 'o' peau --> o fermé
        u,       // ou --> 'u' roue --> Ou
        y,       // u --> 'y' lu --> U
        e,       // é --> 'e' été --> e-fermé
        E,       // è --> 'E' paire, treize --> e-ouvert
        E_comp,  // è --> 'E' paire, treize --> e-ouvert
        e_comp,  // é --> 'e' été --> e-fermé
        e_tilda, // in --> cinq  '5', cinq, linge --> in (voy. nasale) | e~ de X-Sampa
        a_tilda, // an --> an '@', ange --> an (voy. nasale) | a~ de X-Sampa
        o_tilda, // on --> on '§', on, savon --> on (voy. nasale) | o~ de X-Sampa
        x_tilda, // un --> un '1', un, parfum --> un (voy. nasale) | 9~ de X-Sampa
        x2,      // eu --> deux '2', deux, oeuf nous renonçons à distinguer x2 et x9
        // x9,      // oeil, oeuf --> neuf  '9', oeuf, peur --> e-ouvert Nous renonçons à distinguer x2 et x9
        oi,      // Spécialement introduit pour identifier des deux lettres qui donnent --> 'wa'
        w_e_tilda,  // oin de poing, oint --> 'w5'
        // w_E_comp,   // oue de ouest, oued --> 'wE' Le cas particulier crée plus de confusion qu'il n'aide
        // w_i,        // oui, kiwi --> 'wi' Nous renonçons à ce cas particulier. kiwi donnera 'kiwi' en phonétique :-)

        // Semi-voyelles
        w,       // kiwi, sanwich, steward  --> 'w' pour le lexique.
        j,       // paille, ail, thaï, païen --> 'j' yeux, paille --> y (semi-voyelle)
        J,       // ing en fin de mot, prononcé à l'anglaise --> 'iG'
        i_j,     // le son [ij] de affrioler -->´'ij'
        N,          // gn --> 'N' agneau, vigne --> gn (c. nasale palatine)  | J de X-Sampa

        // Consonnes
        p,          // p --> 'p' père, soupe --> p (occlusive)
        b,          // b --> 'b' bon, robe --> b (occlusive)
        t,          // t --> 't' terre, vite --> t (occlusive)
        d,          // d --> 'd' dans aide --> d (occlusive)
        k,          // k --> 'k' carré, laque --> k (occlusive)
        g,          // g --> 'g' gare, bague --> g (occlusive)
        f,          // f --> 'f' feu, neuf --> f (fricative)
        v,          // v --> 'v' vous, rêve --> v (fricative)
        s,          // s --> 's' sale, dessous --> s (fricative)
        z,          // z --> 'z' zéro, maison --> z (fricative)
        S,          // ch --> 'S' chat, tâche --> ch (fricative)
        Z,          // j --> 'Z' gilet, mijoter --> ge (fricative)
        m,          // m --> 'm' main, ferme --> m (cons. nasale)
        n,          // n --> 'n' nous, tonne --> n (cons. nasale
        l,          // l --> 'l' lent, sol --> l (liquide)
        R,          // R --> 'R' rue, venir --> R
        f_ph,       // ph de philosophie --> 'f'
        k_qu,       // qu de quel --> 'k'
        g_u,        // g de gueule --> 'g'
        s_c,        // son s dans ceci --> 's'
        s_t,        // son s dans partition --> 's'
        s_x,        // sons s dans six, dix --> 's'
        z_s,        // s se prononce z --> 'z'
        ks,         // son x de rixe --> 'ks'
        gz,         // son x de examiner, exact --> 'gz'

        verb_3p,    // nt ou ent des verbes conjugués --> muet ""
        _muet,      // --> muet ""

        // o_ouvert,
        // O,       // Marie-Pierre a renoncé à l'analyse o ouvert / fermé. Nous nous contentons de son travail.
        // z_g, ex z^, remplacé par Z
        // w5,  ???

        lastPhon // used to iterate through all values. We could avoid this by using a Dictionary, but the advantage seems limited...
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

        public Phonemes P { get; }
        protected PhonWord pw; // the PhonWord the PhonInW is part of
        private string firedRuleName { get; set; } // name of the rule that was used to define the phoneme

        // Mapping vers la représentation lexique.org des phonèmes.
        static private Dictionary<Phonemes, string> lexMap = new Dictionary<Phonemes, string>((int)Phonemes.lastPhon)
        {
            { Phonemes.a,           "a" },
            { Phonemes.q,           "°" },
            { Phonemes.q_caduc,     "" }, // e final "" est plus proche de Lexique, normalement il faudrait plutôt °
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
            { Phonemes.firstPhon,   "FIRSTPHON" },
            { Phonemes.lastPhon,    "LASTPHON" }
        };

        private static List<Phonemes> voyelles = new List<Phonemes> { Phonemes.a, Phonemes.q, Phonemes.q_caduc, Phonemes.i,
        Phonemes.o, Phonemes.o_comp, Phonemes.u, Phonemes.y, Phonemes.e, Phonemes.E, Phonemes.E_comp, Phonemes.e_comp,
        Phonemes.e_tilda, Phonemes.a_tilda, Phonemes.o_tilda, Phonemes.x_tilda, Phonemes.x2, Phonemes.oi, Phonemes.w_e_tilda};

        private static List<Phonemes> consonnes = new List<Phonemes> { Phonemes.p, Phonemes.b, Phonemes.t, Phonemes.d, Phonemes.k,
        Phonemes.g, Phonemes.f, Phonemes.v, Phonemes.s, Phonemes.z, Phonemes.S, Phonemes.Z, Phonemes.m, Phonemes.n, Phonemes.l,
        Phonemes.R, Phonemes.f_ph, Phonemes.k_qu, Phonemes.g_u, Phonemes.s_c, Phonemes.s_t, Phonemes.s_x, Phonemes.z_s,
        Phonemes.ks, Phonemes.gz};

        private static List<Phonemes> semiVoyelles = new List<Phonemes> { Phonemes.w, Phonemes.j, Phonemes.J, Phonemes.i_j, Phonemes.N };

        private static List<Phonemes> muet = new List<Phonemes> { Phonemes.verb_3p, Phonemes._muet };

        public bool EstConsonne() => (consonnes.BinarySearch(P) >= 0);
        public bool EstVoyelle() => (voyelles.BinarySearch(P) >= 0);
        public bool EstSemiVoyelle() => (semiVoyelles.BinarySearch(P) >= 0);
        public bool EstMuet() => (muet.BinarySearch(P) >= 0);

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

        public static void Init()
        {
            voyelles.Sort();
            consonnes.Sort();
            semiVoyelles.Sort();
            muet.Sort();
        }

        public string Phon2String() => lexMap[P];
            // Retourne la correspondance "Lexique" (voir lexique.org) du phonème.

        public PhonInW(PhonWord inW, int inBeg, int inEnd, Phonemes inP, string ruleName)
            // inBeg and inEnd are relative to inW!
            : base(inW.T, inW.First+ inBeg, inW.First + inEnd)
        {
            pw = inW;
            P = inP;
            firedRuleName = ruleName;
        }

        public PhonInW(PhonWord inW, int inBeg, int inEnd, Phonemes inP)
            // inBeg and inEnd are relative to the original TheText!
            : base(inW.T, inBeg, inEnd)
        {
            pw = inW;
            P = inP;
            firedRuleName = "NoRule";
        }

        protected PhonInW(PhonInW piw)
            : base(piw)
        {
            pw = piw.pw;
            P = piw.P;
            firedRuleName = copied;
        }

        public override void PutColor() => PutColor(PhonConfType.phonemes);

        public void PutColor (PhonConfType pct) => base.SetCharFormat(this.T.GetConfig().colors[pct].Get(P));

        public override string AllStringInfo()
        {
            return String.Format(BaseConfig.cultF, "{0, -25} Rule: {1, -7} --> Phon: {2, -8} LexSound: {3, -3}",
                base.AllStringInfo(), firedRuleName, P, Phon2String());
        }

        //public override string ToString()
        //{
        //    return String.Format(BaseConfig.cultF, "Text: {0, -5} Rule: {1, -7} --> Phon: {2, -8} LexSound: {3, -3}", 
        //        base.ToString(), firedRuleName, P, Phon2String());
        //}

    }
}
