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
        private PBDQConfig pbdqConf;
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
        /// <param name="name">Nom de l'action</param>
        /// <param name="inPBDQConf">La <see cref="PBDQConfig"/> sur laquelle se déroule l'action.
        /// </param>
        /// <param name="inButtonNr">Le bouton de l'action.</param>
        /// <param name="inPrevLetter">La lettre précédente.</param>
        /// <param name="inNewLetter">La nouvelle lettre pour le bouton.</param>
        /// <param name="inPrevCF">La configuration précédente pour le bouton.</param>
        /// <param name="inNewCF">La nouvelle configuration pour le bouton.</param>
        public PBDQAction(string name, PBDQConfig inPBDQConf, int inButtonNr,
            char inPrevLetter, char inNewLetter, CharFormatting inPrevCF, CharFormatting inNewCF)
            : base(name)
        {
            type = "leterAndCF";
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

        public PBDQAction(string name, PBDQConfig inPBDQConf, bool inPrevMarkAsBlack, bool inNewMarkAsBlack)
            : base(name)
        {
            type = "markAsBlack";
            pbdqConf = inPBDQConf;
            prevMarkAsBlack = inPrevMarkAsBlack;
            newMarkAsBlack = inNewMarkAsBlack;

            // Pour éviter les champs non initialisés.
            buttonNr = 0;
            prevCF = null;
            newCF = null;
            prevLetter = ' ';
            newLetter = ' ';
        }

        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case "cfOnly":
                    pbdqConf.UpdateLetter(buttonNr, prevCF);
                    break;

                case "leterAndCF":
                    pbdqConf.UpdateLetter(buttonNr, prevLetter, prevCF);
                    break;

                case "markAsBlack":
                    pbdqConf.SetMarkAsBlackTo(prevMarkAsBlack);
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }

        public override void Redo()
        {
            logger.ConditionalDebug("Redo");
            switch (type)
            {
                case "cfOnly":
                    pbdqConf.UpdateLetter(buttonNr, newCF);
                    break;

                case "leterAndCF":
                    pbdqConf.UpdateLetter(buttonNr, newLetter, newCF);
                    break;

                case "markAsBlack":
                    pbdqConf.SetMarkAsBlackTo(newMarkAsBlack);
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
