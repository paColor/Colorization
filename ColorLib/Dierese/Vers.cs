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

        /// <summary>
        /// Au-delà de cette limite la méthode <see cref="ChercheDierese(int)"/> ne travaille pas.
        /// L'idée est que si un vers fait plus de ce nombre de pieds, il ne s'agit plus d'un vers
        /// mais d'un paragraphe. Un paragraphe court, risque encore d'êtr etraité comme un vers...
        /// </summary>
        public const int MaxNrPieds = 16;

        /// <summary>
        /// Nombre de pieds du vers.
        /// </summary>
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
                while (i < pws.Count && pws[i].Last <= this.Last)
                {
                    pWordList.Add(pws[i]);
                    i++;
                }
                    
                nrPieds = ComptePieds(pWordList);
            }
            // else
                // On cherche un vers qui commence après le dernier mot du texte. Il s'agit
                // probablement d'une ligne vide après le texte. --> ne rien faire
        }

        /// <summary>
        /// Cherche une ou plusieurs diérèses dans le vers, transforme les mots correspondants 
        /// jusqu'à ce que le nombre de pieds voulu soit atteint ou qu'il n'y ait plus de diérèse
        /// détectable.
        /// </summary>
        /// <param name="conf">La <c>Config</c> à utiliser pour l'identification des pieds.</param>
        /// <param name="nbrPiedsVoulu">Le nombre de pieds souhaité après la mise en évidence des
        /// diérèses. </param>
        /// <exception cref="ArgumentOutOfRangeException">Si <c>nbrPiedsVoulu</c> n'est pas 
        /// plus grand que zéro.</exception>
        public void ChercheDierese (int nbrPiedsVoulu)
        {
            logger.ConditionalDebug("ChercheDierese, nrPiedsVoulu: {0}, vers; {1}", nbrPiedsVoulu, ToString());
            if (nbrPiedsVoulu > MaxNrPieds)
            {
                logger.Info(
                    "Chercher diérèse pour vers de plus de [0] pieds. Non exécuté. nrPiedsVoulu: {1}",
                    MaxNrPieds, nbrPiedsVoulu);
                return;
            }
            if (nbrPiedsVoulu <= 0)
            {
                logger.Fatal("nbrPiedsVoulu == {0}", nbrPiedsVoulu);
                throw new ArgumentOutOfRangeException(nameof(nbrPiedsVoulu), "doit être plus grand que 0");
            }
            // traitement spécial pour les nombres de pieds pairs, parcequ'ils le valent bien!
            if (nbrPiedsVoulu % 2 == 0)
            {
                // nombre pair de pieds
                // Y a-t-il un hémistiche à la moitié du vers ou juste avant?
                int demiVers = nbrPiedsVoulu / 2;
                List<PhonWord> moitie1 = new List<PhonWord>(5);
                int i = 0;
                int piedsM1 = 0;
                while (i < pWordList.Count && piedsM1 < demiVers -1)
                {
                    moitie1.Add(pWordList[i]);
                    piedsM1 = ComptePieds(moitie1);
                    i++;
                }

                // En prenant les cas dans cet ordre, on favorise légèrement la recherche de la
                // diérèse dans la première partie du vers. Il y a au moins un exemple dans le
                // poème de référence où cette approche est justifiée:
                // "D'affreux bohémiens, des vainqueurs de charnier" 
                // Faut-il rallonger "bohémines" ou "charnier"? Il y a certainement des cas
                // qui demanderaient l'approche opposée. Je ne vois pas comment les distinguer
                // sans tenir compte du sens ou d'éléments que j'ignore jusqu'ici comme la 
                // virgule dans le vers de V. Hugo.

                if (piedsM1 == demiVers - 1)
                {
                    List<PhonWord> moitie2 = new List<PhonWord>(5);
                    while (i < pWordList.Count)
                    {
                        moitie2.Add(pWordList[i]);
                        i++;
                    }
                    ChercherDierese(moitie1, demiVers);
                    if (ComptePieds(pWordList) < nbrPiedsVoulu)
                        ChercherDierese(moitie2, demiVers);
                    if (ComptePieds(pWordList) < nbrPiedsVoulu)
                        ChercherDierese(pWordList, nbrPiedsVoulu);
                }
                else if (piedsM1 == demiVers)
                {
                    // hypothèse: on a trouvé l'hémistiche
                    List<PhonWord> moitie2 = new List<PhonWord>(5);
                    while (i < pWordList.Count)
                    {
                        moitie2.Add(pWordList[i]);
                        i++;
                    }
                    ChercherDierese(moitie2, demiVers);
                    if (ComptePieds(pWordList) < nbrPiedsVoulu)
                        ChercherDierese(moitie1, demiVers);
                    if (ComptePieds(pWordList) < nbrPiedsVoulu)
                        ChercherDierese(pWordList, nbrPiedsVoulu);
                }
                else if (piedsM1 > demiVers)
                {
                    // On est allés trop loin. 
                    // on n'a pas réussi à trouver d'hémistiche.
                    ChercherDierese(pWordList, nbrPiedsVoulu);
                }
                else
                {
                    // Bizarre: le vers entier semble faire moins de la moitié des pieds voulus...
                    logger.Info("On demande {0} pieds pour le vers {1}.", nbrPiedsVoulu, ToString());
                    ChercherDierese(pWordList, nbrPiedsVoulu); // ça ne devrait pas marcher...
                }
            }
            else
            {
                // nombre impair de pieds voulu.
                logger.ConditionalDebug("Nombre impair ({0}) de pieds voulus pour {1}", 
                    nbrPiedsVoulu, ToString());
                ChercherDierese(pWordList, nbrPiedsVoulu);
            }
            nrPieds = ComptePieds(pWordList);
            if (nrPieds != nbrPiedsVoulu)
            {
                logger.ConditionalTrace(
                    "!! Diérèse pas trouvée. nbrPiedsVoulu: {0}, nrPieds: {1}, vers: \'{2}\'," +
                    "syls: \'{3}\'", nbrPiedsVoulu, nrPieds, ToString(), Syllabes());
            }
        }

        private void ChercherDierese(List<PhonWord> pws, int nbrPiedsVoulu)
        {
            int i = 0;
            while (i < pws.Count && ComptePieds(pws) < nbrPiedsVoulu)
            {
                pws[i].ComputeSyls(true);
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
