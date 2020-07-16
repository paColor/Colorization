using System;
using System.Collections.Generic;
using System.Text;
using ColorLib;

namespace ColorLib.Dierese
{
    /// <summary>
    /// Représente un vers dans un texte (si possible un poème :-). Un vers est en fait une ligne.
    /// Une fin de ligne est détectée par la présence du caractère '\r' ou '\v'.
    /// </summary>
    public class Vers : TextEl
    {
        // ****************************************************************************************
        // *                               private static members                                 *
        // ****************************************************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ****************************************************************************************
        // *                               private static methods                                 *
        // ****************************************************************************************

        /// <summary>
        /// Cherche la dernière lettre d'un vers dont on connaît la position de la première lettre.
        /// </summary>
        /// <param name="tt">Le texte contenant le vers.</param>
        /// <param name="startPos">La position de la première lettre du vers.</param>
        /// <returns></returns>
        private static int FindLastVersPos(TheText tt, int startPos)
        {
            if (tt == null)
            {
                const string Message = "Impossible de créer un vers avec un texte == null";
                logger.Fatal(Message);
                throw new ArgumentNullException(nameof(tt), Message);
            }
            if (tt.S.Length == 0)
            {
                const string Message = "On ne peut pas chercher la fin d'un vers sur un texte vide.";
                logger.Error("On ne peut pas chercher la fin d'un vers sur un texte vide.");
                throw new ArgumentException(Message, nameof(tt));
            }
            int i = startPos;
            while (i < tt.S.Length && tt.S[i] != '\r' && tt.S[i] != '\v')
            {
                i++;
            }
            return i - 1;
        }

        private static int ComptePieds(List<PhonWord> pwList)
        {
            int toReturn = 0;
            foreach (PhonWord pw in pwList)
            {
                toReturn += pw.GetNbreSyllabes();
            }
            return toReturn;
        }

        // ****************************************************************************************
        // *                                    public members                                    *
        // ****************************************************************************************

        public int nrPieds { get; private set; }

        // ****************************************************************************************
        // *                                   private members                                    *
        // ****************************************************************************************

        private List<PhonWord> pWordList;

        // ****************************************************************************************
        // *                                   public methods                                     *
        // ****************************************************************************************

        /// <summary>
        /// Crée un vers sur la base du texte <c>tt</c>. Le premier caractère du vers se trouve
        /// à la position <c>first</c>.
        /// </summary>
        /// <param name="tt">Le texte dont fait partie le vers. Ne peut pas être <c>null</c>.</param>
        /// <param name="first">La position (zero based) du premier caractère du vers. </param>
        /// <param name="eolPos">Out: la position (zero based) du caractère marquant la fin de vers. 
        /// Typiquement '\r'. Est égal à <c>tt.ToString().Length</c> si la fin du texte est
        /// atteinte. </param>
        /// <param name="pws">Liste des <c>PhonWord</c> correspondant à <c>tt</c>. Non <c>null</c>. 
        /// </param>
        public Vers(TheText tt, int first, List<PhonWord> pws)
            : base(tt, first, FindLastVersPos(tt, first))
        {
            if (pws == null)
            {
                const string Message = "Impossible de créer un vers avec une liste de mots == null";
                logger.Fatal(Message);
                throw new ArgumentNullException(nameof(pws), Message);
            }
            
            pWordList = new List<PhonWord>();
            nrPieds = 0;

            // Trouvons le début du vers
            int i = 0;
            while (i < pws.Count && pws[i].First < first)
                i++;
            if (i < pws.Count)
            {
                while (pws[i].Last <= this.Last)
                {
                    pWordList.Add(pws[i]);
                    i++;
                }
                    
                nrPieds = ComptePieds(pWordList);
            }
            else
            {
                const string Message = "On cherche un vers qui commence après le dernier mot du texte.";
                logger.Fatal(Message + "pws.Count: {0}, first: {1}", pws.Count, first);
                throw new ArgumentException(Message, nameof(first));
            }
        }

        /// <summary>
        /// Cherche une ou plusieurs diérèses dans le vers, transforme les mots correspondants 
        /// Jusqu'à ce que le nombre de pieds voulu soit atteint ou qu'il n'y ait plus de diérèse
        /// détecetable.
        /// </summary>
        /// <param name="conf">La <c>Config</c> à utiliser pour l'identification des pieds.</param>
        /// <param name="nbrPiedsVoulu">Le nombre de pieds souhaité après la mise en évidence des
        /// diérèses. </param>
        public void ChercheDierese (int nbrPiedsVoulu)
        {
            logger.ConditionalDebug("ChercheDierese, nrPiedsVoulus: {0}", nbrPiedsVoulu);
            int i = 0;
            while (i < pWordList.Count && nrPieds < nbrPiedsVoulu)
            {
                pWordList[i].ComputeSyls(true);
                nrPieds = ComptePieds(pWordList);
                i++;
            }
        }

        /// <summary>
        /// Retourne la représentation en syllabes du vers (syl1-syl2 syl3-syl4-syl5). Les mots
        /// sont séparés par des espaces, les syllabes par des tirets.
        /// </summary>
        /// <returns>La représentation en syllabes du vers.</returns>
        public string Syllabes()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pWordList.Count; i++)
            {
                sb.Append(pWordList[i].Syllabes());
                if (i < pWordList.Count - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }

}
