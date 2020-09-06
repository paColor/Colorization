using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorLib.Morphalou
{

    /// <summary>
    /// Nous somme confrontés à au moins 3 façons de noter les phonèmes dans les différentes
    /// sources utilisées pour améliorer la qualité de Colorization:
    ///   1 - Lexique.utilisé par lexique.org et que j'ai repris dans Colorization
    ///   2 - Colorization - Une version simplifiée de Lexique, où les différences par exemple
    /// entre sons ouverts ou fermés(o vs. O) ne sont pas pris en compte, conformément au
    /// moteur de reconnaissance des phonèmes.
    ///   3 - SAMPA, utilisé par Morphalou
    ///
    /// Notre but est de pouvoir comparer les différentes bases de données avec ce que produit
    ///   le moteur de Colorization. Il nous faut donc des tables de traduction Lexique --> Col
    /// et SAMPA --> Col.
    ///
    /// Le but de cette classe est de fournir ces fonctions de conversion.
    /// </summary>
    public static class NotationsPhon
    {
        // La notation exacte utilisés par Colorization dans la production d'une visualisation phonétique
        // d'un mot, sont spécifiés dans PhonInW.cs

        private static Dictionary<string, string> Lex2ColSimpl = new Dictionary<string, string>()
        {
            {"a","a" },
            {"i","i" },
            {"y","y" },
            {"u","u" },
            {"o","o" },
            {"O","o" },
            {"e","e" },
            {"E","E" },
            {"°","°" },
            {"2","2" },
            {"9","9" },
            {"5","5" },
            {"1","1" },
            {"@","@" },
            {"§","§" },
            {"j","j" },
            {"8","8" },
            {"w","w" },
            {"p","p" },
            {"b","b" },
            {"t","t" },
            {"d","d" },
            {"k","k" },
            {"g","g" },
            {"f","f" },
            {"v","v" },
            {"s","s" },
            {"z","z" },
            {"S","S" },
            {"Z","Z" },
            {"m","m" },
            {"n","n" },
            {"N","N" },
            {"l","l" },
            {"R","R" },
            {"x","x" },
            {"G","G" }
        };

        private static Dictionary<string, string> Col2ColSimpl = Lex2ColSimpl;

        private static Dictionary<string, string> Sampa2ColSimpl = new Dictionary<string, string>()
        {
            { "p", "p" },
            { "b", "b" },
            { "t", "t" },
            { "d", "d" },
            { "k", "k" },
            { "g", "g" },
            { "f", "f" },
            { "v", "v" },
            { "s", "s" },
            { "z", "z" },
            { "S", "S" },
            { "Z", "Z" },
            { "j", "j" },
            { "m", "m" },
            { "n", "n" },
            { "J", "N" },
            { "N", "G" },
            { "l", "l" },
            { "R", "R" },
            { "w", "w" },
            { "H", "u" },
            { "i", "i" },
            { "e", "e" },
            { "E", "E" },
            { "a", "a" },
            { "A", "a" },
            { "O", "o" },
            { "o", "o" },
            { "u", "u" },
            { "y", "y" },
            { "2", "2" },
            { "9", "2" },
            { "6", "2" }, // 6 est parfois utilisé autrement :-(
            { "@", "°" },
            { "e~", "5" },
            { "a~", "@" },
            { "o~", "§" },
            { "9~", "1" },
            { "E/", "E" },
            { "O/", "o" },
        };

        /// <summary>
        /// Traduction de la représentation ColSimpl étendue (ça devient un peu compliqué) vers les 
        /// phonèmes utilisés par PhonInW et PhonWord.
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
        private static Dictionary<char, Phonemes> son2phoneme = new Dictionary<char, Phonemes>() // don't forget to increase in case...
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
            {'N',   Phonemes.J}, // ng
            {'G',   Phonemes.N}, // gn
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

        /// <summary>
        /// Retourne le phonèmes correspondant au son <paramref name="s"/> de la notation
        /// ColSimpl étendue.
        /// </summary>
        /// <param name="s">Un son de la notation ColSimpl étendue.</param>
        /// <returns>Le phonème correspondant.</returns>
        /// <exception cref="KeyNotFoundException">Si <paramref name="s"/> n'est pas un son
        /// connu.</exception>
        public static Phonemes Son2phon(char c)
        {
            return son2phoneme[c];
        }

        /// <summary>
        /// Retourne la représentation Colorization simplifiée de la représentation SAMPA
        /// </summary>
        /// <param name="sampa">par exemple "d e s O l i d a R i z @ R E", avec un espace entre
        /// chaque phonème. Ne doit pas commencer par un esapce. </param>
        /// <returns>Le mot sous forme ColSimpl</returns>
        /// <exception cref="KeyNotFoundException"><paramref name="sampa"/> n'est pas reconnu
        /// comme du format SAMPA.
        /// </exception>
        public static string S2CS(string sampa)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int j = 0;
            while (i < sampa.Length)
            {
                if (sampa[i] == ' ')
                {
                    sb.Append(Sampa2ColSimpl[sampa.Substring(j, i - j)]);
                    j = i + 1;
                }
                i++;
            }
            if (j < i)
            {
                sb.Append(Sampa2ColSimpl[sampa.Substring(j, i - j)]);
            }
            return sb.ToString();
        }

        public static string L2CS(string lexique)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lexique.Length; i++)
            {
                sb.Append(Lex2ColSimpl[lexique.Substring(i, 1)]);
            }
            return sb.ToString();
        }

        public static string C2CS(string col)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < col.Length; i++)
            {
                sb.Append(Col2ColSimpl[col.Substring(i, 1)]);
            }
            return sb.ToString();
        }

    }
}
