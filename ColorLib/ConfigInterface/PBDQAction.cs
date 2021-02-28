using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    public class PBDQAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// on traite les actions des types suivants:
        /// "cfOnly"
        /// "leterAndCF"
        /// "markAsBlack"
        /// </summary>
        private string type;
        PBDQConfig pbdqConf;
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private int buttonNr;
        private char prevLetter;
        private char newLetter;
        private bool prevMarkAsBlack;
        private bool newMarkAsBlack;

        /// <summary>
        /// Action de mise à jour d'un bouton - CFOnly
        /// </summary>
        /// <param name="name">Nom de l'action</param>
        /// <param name="inPBDQConf">Le <see cref="PBDQConfig"/> sur lequel se passe l'action.
        /// </param>
        /// <param name="inButtonNr">Le bouton</param>
        /// <param name="inPrevCF">Le formatage précédent.</param>
        /// <param name="inNewCF">Le nouveau formatage.</param>
        public PBDQAction(string name, PBDQConfig inPBDQConf, int inButtonNr,
            CharFormatting inPrevCF, CharFormatting inNewCF)
            : base(name)
        {
            type = "cfOnly";
            pbdqConf = inPBDQConf;
            buttonNr = inButtonNr;
            prevCF = inPrevCF;
            newCF = inNewCF;

            // Pour éviter les champs non initialisés.
            prevLetter = ' ';
            newLetter = ' ';
            prevMarkAsBlack = false;
            newMarkAsBlack = false;
        }

        /// <summary>
        /// Action de mise à jour d'un bouton, y.c. une lettre - "letterAndCF"
        /// </summary>
        /// <param name="name"></param>
        /// <param name="inPBDQConf"></param>
        /// <param name="inButtonNr"></param>
        /// <param name="inPrevLetter"></param>
        /// <param name="inNewLetter"></param>
        /// <param name="inPrevCF"></param>
        /// <param name="inNewCF"></param>
        public PBDQAction(string name, PBDQConfig inPBDQConf, int inButtonNr,
            char inPrevLetter, char inNewLetter, CharFormatting inPrevCF, CharFormatting inNewCF)
            : base(name)
        {
            type = "cfOnly";
            pbdqConf = inPBDQConf;
            buttonNr = inButtonNr;
            prevCF = inPrevCF;
            newCF = inNewCF;
            prevLetter = inPrevLetter;
            newLetter = inNewLetter;

            // Pour éviter les champs non initialisés.
            prevMarkAsBlack = false;
            newMarkAsBlack = false;
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override void Redo()
        {
            throw new NotImplementedException();
        }
    }
}
