using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ColorLib;

namespace ColorizationControls
{
    /// <summary>
    /// Classe pour le choix d'une couleur de "hilight".
    /// <remarks>Créer une instance, puis lire le résultat dans ResultingCF. S'il est <c>null</c>
    /// rien n'a changé, sinon, le CF contient les valeurs d'entrée avec le nouveau 
    /// surlignage.</remarks>
    /// </summary>
    public class HiFormatForm
    {
        /// <summary>
        /// Ouvre la fenêtre où l'utilisateur peut choisir une couleur de surlignage. Dialogue modal,
        /// càd que l'utilisateur doit choisir une couleur et cliquer OK. Toute action en dehors de
        /// la fenêtre est impossible.
        /// </summary>
        /// <param name="p">Position où la fenêtre est ouverte.</param>
        /// <param name="cf"><see cref="CharFormatting"/> de départ. S'il y a une couleur de 
        /// surlignage, elle est marquée comme valeur de départ.</param>
        public HiFormatForm(Point p, CharFormatting cf)
        {
            HilightForm hiForm = new HilightForm(cmsCF.hilightColor);
        }
    }
}
