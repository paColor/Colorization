using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib.Dierese
{
    /// <summary>
    /// Représente une zone de plusieurs vers consécutifs dans un poème. Ces vers ont à peu près
    /// le même nombre de pieds. L'hypothèse est qu'ils devraient tous en compter exactement le
    /// même nombre et que les pieds qui manquent sont des diérèses qu'il reste à trouver...
    /// </summary>
    public class ZonePoeme
    {
        private const float DeltaMoins = 2.7f;
        private const float DeltaPlus = 2.1f;
        private int nrPiedsVoulus;
        private float nrPiedsMoyen;
        private List<Vers> vList;

        public ZonePoeme()
        {
            nrPiedsMoyen = 0.0f;
            nrPiedsVoulus = 0;
            vList = new List<Vers>();
        }

        /// <summary>
        /// Ajout le vers <c>v</c> à la zone. 
        /// </summary>
        /// <param name="v">Le vers à ajouter à la zone. Non <c>null</c>.</param>
        /// <returns><c>true</c>: Le vers a été ajouté à la zone. <c>false</c>: le vers a un nombre
        /// de pieds trop éloigné des autres vers de la zone. Il fait partie d'une autre zone.
        /// Il n'a pas été ajouté.</returns>
        public bool AddVers(Vers v)
        {
            bool toReturn = false;
            if (vList.Count == 0)
            {
                vList.Add(v);
                nrPiedsMoyen = v.nrPieds;
                nrPiedsVoulus = v.nrPieds;
                toReturn = true;
            }
            else if ((nrPiedsMoyen >= v.nrPieds && nrPiedsMoyen - v.nrPieds < DeltaMoins)
                || (nrPiedsMoyen < v.nrPieds && v.nrPieds - nrPiedsMoyen < DeltaPlus))
            {
                nrPiedsMoyen =  ((nrPiedsMoyen * vList.Count) + v.nrPieds) / (vList.Count + 1);
                nrPiedsVoulus = (int)nrPiedsMoyen;
                if (nrPiedsVoulus < (nrPiedsMoyen - (0.3f)))
                    nrPiedsVoulus++;
                vList.Add(v);
                toReturn = true;
            }
            return toReturn;
        }

        /// <summary>
        /// Pour tous les vers de la zone dont le nombre de pieds est plus petit que le nombre de
        /// pieds voulu, on cherche s'il n'y aurait pas une diérèse qui pourrait augmenter le
        /// nombre de pieds du vers.
        /// </summary>
        /// <param name="nrPieds">Nombre de pieds à rechercher. 0 s'il faut prendre le nombre de
        /// pieds estimé par l'algorithme.</param>
        public void ChercheDierese(int nrPieds)
        {
            if (nrPieds != 0)
                nrPiedsVoulus = nrPieds;
            foreach (Vers v in vList)
            {
                if(v.nrPieds < nrPiedsVoulus)
                {
                    v.ChercheDierese(nrPiedsVoulus);
                }
            }
        }
    }
}
