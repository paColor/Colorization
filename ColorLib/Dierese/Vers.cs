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

        static private int FindLastVersPos(TheText tt, int startPos)
        {
            int i = startPos;
            while (i < tt.S.Length && tt.S[i] != '\r' && tt.S[i] != '\v')
            {
                i++;
            }
            return i;
        }


        // ****************************************************************************************
        // *                                     public methods                                   *
        // ****************************************************************************************

        /// <summary>
        /// Crée un vers sur la base du texte <c>tt</c>. Le premier caractère du vers se trouve
        /// à la position <c>first</c>.
        /// </summary>
        /// <param name="tt">Le texte dont fait partie le vers.</param>
        /// <param name="first">La position (zero based) du premier caractère du vers. </param>
        /// <param name="eolPos">Out: la position (zero based) du caractère marquant la fin de vers. 
        /// Typiquement '\r'. Est égal à <c>tt.ToString().Length</c> si la fin du texte est
        /// atteinte. </param>
        public Vers(TheText tt, int first)
            : base(tt, first, FindLastVersPos(tt, first))
        {

        }

        
    }

}
