using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// Les signes de ponctuation différentiés.
    /// </summary>
    public enum Ponctuation
    {
        firstP, // Pour avoir un délimiteur clair

        point,              // .
        virgule,            // ,
        deuxPoints,         // :
        pointVirgule,       // ;
        paranthèse,         // ()[]{}
        pointDExclamation,  // !
        pointDInterrogation,// ?
        guillemets,         // " 
        apostrophe,         // ' ´ `
        maths,              // + - / * % < > = ¬ | ° 
        monnaie,            // $ £ €
        divers,             // & _ § ¦ @ # ¢ ~ \
        autres,             // tout ce qui n'est pas entré dans une catégorie précédente

        lastP // Pour avoir un délimiteur clair
    }


    class PunctInT : TextEl
    {
        private static HashSet<char> carPoint = new HashSet<char>
        {
            '.'
        };

        private static HashSet<char> carCirgule = new HashSet<char>
        {
            ','
        };

        private static HashSet<char> carDeuxPoints = new HashSet<char>
        {
            ':'
        };

        private static HashSet<char> carPointVirgule = new HashSet<char>
        {
            ';'
        };

        private static HashSet<char> carParanthèse = new HashSet<char>
        {
            '(', ')', '[', ']', '{', '}',
        };

        private static HashSet<char> carPointDExclamation = new HashSet<char>
        {
            '!'
        };

        private static HashSet<char> carPointDInterrogation = new HashSet<char>
        {
            '?'
        };

        private static HashSet<char> carGuillemets = new HashSet<char>
        {
            '?'
        };




        /// <summary>
        /// Représente un signe de ponctuation dans <paramref name="tt"/>. il ne s'agit par définition
        /// que d'un seul caractère à la position <paramref name="pos"/> (zero based) dans le texte.
        /// </summary>
        /// <exception cref="ArgumentNullException"> si <paramref name="tt"/> est <c>null</c>. 
        /// Provient de <see cref="TextEl"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="pos"/> >= 
        /// <c>tt.S.Length</c>. Provient de <see cref="TextEl"/>.</exception>
        /// <param name="tt">Le texte dont on indique un signe de ponctuation.</param>
        /// <param name="pos">La position du signe de pnctuation dans <paramref name="tt"/>
        /// </param>
        /// <param name="inLast"></param>
        public PunctInT(TheText tt, int pos)
            : base(tt, pos, pos)
        {
            char c = tt.S[pos];
            string s;
            s.
        }
    }
}
