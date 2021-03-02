using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class SylAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// On traite les actions des types suivants
        /// "sylBut"
        /// "clearBut"
        /// "doubleCons"
        /// "mode"
        /// "marquerMuettes"
        /// "dierese"
        /// "nbrePieds"
        /// </summary>
        private string type;
        private SylConfig sylConf;
        private int prevNrSetButtons;
        private int butNr;
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private SylConfig.Mode prevMode;
        private SylConfig.Mode newMode;
        private int prevNrPieds;
        private int newNrPieds;
        private bool prevBoolVal;
        private bool newBoolVal;

        public SylAction(string name,SylConfig inSylConf, int inButNr, int inPrevNrSetBut,
            CharFormatting inPrevCF, CharFormatting inNewCF)
            : base(name)
        {
            type = "sylBut";
            sylConf = inSylConf;
            prevNrSetButtons = inPrevNrSetBut;
            butNr = inButNr;
            prevCF = inPrevCF;
            newCF = inNewCF;
        }

        public SylAction(string name, SylConfig inSylConf, int inButNr, CharFormatting inPrevCF)
            : base(name)
        {
            type = "clearBut";
            sylConf = inSylConf;
            butNr = inButNr;
            prevCF = inPrevCF;
        }

        /// <summary>
        /// Crée une action de modification d'un booléen - "doubleCons", "marquerMuettes" ou 
        /// "dierese"
        /// </summary>
        /// <param name="name">le nom de l'action.</param>
        /// <param name="inType">Le type de l'action. "doubleCons", "marquerMuettes" ou 
        /// "dierese"</param>
        /// <param name="inSylConf">La <see cref="SylConfig"/> sur laquelle l'action agit.</param>
        /// <param name="inPrevVal">La valeur du booléen avant l'action.</param>
        /// <param name="inNewVal">La valeur du booléen après l'action.</param>
        public SylAction(string name, string inType, SylConfig inSylConf, 
            bool inPrevVal, bool inNewVal)
            : base(name)
        {
            type = inType;
            sylConf = inSylConf;
            prevBoolVal = inPrevVal;
            newBoolVal = inNewVal;
        }

        public SylAction(string name, SylConfig inSylConf,
            SylConfig.Mode inPrevMode, SylConfig.Mode inNewMode)
            : base(name)
        {
            type = "mode";
            sylConf = inSylConf;
            prevMode = inPrevMode;
            newMode = inNewMode;
        }

        public SylAction(string name, SylConfig inSylConf,
            int inPrevNrPieds, int inNewNrPieds)
            : base(name)
        {
            type = "nbrePieds";
            sylConf = inSylConf;
            prevNrPieds = inPrevNrPieds;
            newNrPieds = inNewNrPieds;
        }


        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case "sylBut":
                    if (butNr == prevNrSetButtons)
                    {
                        sylConf.ClearButton(butNr);
                    }
                    else
                    {
                        sylConf.SetSylButtonCF(butNr, prevCF);
                    }
                    break;

                case "clearBut":
                    sylConf.SetSylButtonCF(butNr, prevCF);
                    break;

                case "doubleCons":
                    sylConf.DoubleConsStd = prevBoolVal;
                    break;

                case "mode":
                    sylConf.mode = prevMode;
                    break;

                case "marquerMuettes":
                    sylConf.marquerMuettes = prevBoolVal;
                    break;

                case "dierese":
                    sylConf.chercherDierese = prevBoolVal;
                    break;

                case "nbrePieds":
                    sylConf.nbrPieds = prevNrPieds;
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
                case "sylBut":
                    sylConf.SetSylButtonCF(butNr, newCF);
                    break;

                case "clearBut":
                    sylConf.ClearButton(butNr);
                    break;

                case "doubleCons":
                    sylConf.DoubleConsStd = newBoolVal;
                    break;

                case "mode":
                    sylConf.mode = newMode;
                    break;

                case "marquerMuettes":
                    sylConf.marquerMuettes = newBoolVal;
                    break;

                case "dierese":
                    sylConf.chercherDierese = newBoolVal;
                    break;

                case "nbrePieds":
                    sylConf.nbrPieds = newNrPieds;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
