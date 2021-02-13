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
        paranthese,         // ()[]{}
        pointDExclamation,  // !
        pointDInterrogation,// ?
        guillemets,         // " 
        apostrophe,         // ' 
        maths,              // + - / * % < > = ¬ | ° 
        monnaie,            // $ £ €
        espace,             // " ", tab
        divers,             // & _ § ¦ @ # ~ \
        autres,             // tout ce qui n'est pas entré dans une catégorie précédente

        lastP // Pour avoir un délimiteur clair
    }

    /// <summary>
    /// Un signe de ponctuation dans une <see cref="TheText"/>.
    /// </summary>
    /// <remarks>Tout caractère qui n'est pas reconnu comme faisant partie d'un mot (expression
    /// régulière \w) est considéré comme de la ponctuation.</remarks>
    public class PonctInT : TextEl
    {
        private static Dictionary<char, Ponctuation> ponctu = new Dictionary<char, Ponctuation>()
        {
            { '.',  Ponctuation.point },
            { ',',  Ponctuation.virgule },
            { ':',  Ponctuation.deuxPoints },
            { ';',  Ponctuation.pointVirgule },
            { '(',  Ponctuation.paranthese },
            { ')',  Ponctuation.paranthese },
            { '{',  Ponctuation.paranthese },
            { '}',  Ponctuation.paranthese },
            { '[',  Ponctuation.paranthese },
            { ']',  Ponctuation.paranthese },
            { '!',  Ponctuation.pointDExclamation },
            { '?',  Ponctuation.pointDInterrogation },
            { '"',  Ponctuation.guillemets },
            { '«',  Ponctuation.guillemets },
            { '»',  Ponctuation.guillemets },
            { 'ʹ',  Ponctuation.guillemets },
            { 'ʺ',  Ponctuation.guillemets },
            { 'ʻ',  Ponctuation.guillemets },
            { 'ʼ',  Ponctuation.guillemets },
            { '“',  Ponctuation.guillemets },
            { '”',  Ponctuation.guillemets },
            { '„',  Ponctuation.guillemets },
            { '‟',  Ponctuation.guillemets },
            { '′',  Ponctuation.guillemets },
            { '″',  Ponctuation.guillemets },
            { '‚',  Ponctuation.guillemets },
            { '‛',  Ponctuation.guillemets },
            { '\'', Ponctuation.apostrophe },
            { '’',  Ponctuation.apostrophe },
            { '+',  Ponctuation.maths },
            { '-',  Ponctuation.maths },
            { '/',  Ponctuation.maths },
            { '*',  Ponctuation.maths },
            { '%',  Ponctuation.maths },
            { '>',  Ponctuation.maths },
            { '<',  Ponctuation.maths },
            { '=',  Ponctuation.maths },
            { '¬',  Ponctuation.maths },
            { '|',  Ponctuation.maths },
            { '°',  Ponctuation.maths },
            { '$',  Ponctuation.monnaie },
            { '£',  Ponctuation.monnaie },
            { '€',  Ponctuation.monnaie },
            { '¢',  Ponctuation.monnaie },
            { '¥',  Ponctuation.monnaie },
            { ' ',  Ponctuation.espace },
            { '\t', Ponctuation.espace },
            { '\r', Ponctuation.espace },
            { '\n', Ponctuation.espace },
            { '&',  Ponctuation.divers },
            { '_',  Ponctuation.divers },
            { '§',  Ponctuation.divers },
            { '¦',  Ponctuation.divers },
            { '@',  Ponctuation.divers },
            { '#',  Ponctuation.divers },
            { '~',  Ponctuation.divers },
            { '\\', Ponctuation.divers },

        };

        /// <summary>
        /// Le signe de ponctuation pour l'objet.
        /// </summary>
        public Ponctuation ponct { get; private set; }


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
        public PonctInT(TheText tt, int pos)
            : base(tt, pos, pos)
        {
            Ponctuation p;
            if (ponctu.TryGetValue(tt.S[pos], out p))
            {
                ponct = p;
            }
            else
            {
                ponct = Ponctuation.autres;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("-");
            sb.Append(ponct.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Retourne tous les memmbres de l'objet.
        /// </summary>
        /// <returns>un string donnant la valeur de tous ls membres de l'objet.</returns>
        public override string AllStringInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.AllStringInfo());
            sb.Append(", ponctuation: ");
            sb.Append(ponct.ToString());
            return sb.ToString();
        }

        public override void PutColor(Config conf)
        {
            base.SetCharFormat(conf.ponctConf.GetCF(ponct));
        }
    }
}
