using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class SylAction : CLAction
    {
        public enum SylActionType { sylBut, clearBut, doubleCons, mode, marquerMuettes, dierese,
            nbrePieds, exceptMots, monosyllabes }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private SylActionType type;
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
        private ExceptionMots prevExcMots;
        private ExceptionMots newExcMots;

        public SylAction(string name,SylConfig inSylConf, int inButNr, int inPrevNrSetBut,
            CharFormatting inPrevCF, CharFormatting inNewCF)
            : base(name)
        {
            type = SylActionType.sylBut;
            sylConf = inSylConf;
            prevNrSetButtons = inPrevNrSetBut;
            butNr = inButNr;
            prevCF = inPrevCF;
            newCF = inNewCF;
        }

        public SylAction(string name, SylConfig inSylConf, int inButNr, CharFormatting inPrevCF)
            : base(name)
        {
            type = SylActionType.clearBut;
            sylConf = inSylConf;
            butNr = inButNr;
            prevCF = inPrevCF;
        }

        /// <summary>
        /// Crée une action de modification d'un booléen - "doubleCons", "marquerMuettes" ou 
        /// "dierese"
        /// </summary>
        /// <param name="name">le nom de l'action.</param>
        /// <param name="inType">Le type de l'action. "doubleCons", "marquerMuettes" 
        /// "dierese" ou "monosyllabes"</param>
        /// <param name="inSylConf">La <see cref="SylConfig"/> sur laquelle l'action agit.</param>
        /// <param name="inPrevVal">La valeur du booléen avant l'action.</param>
        /// <param name="inNewVal">La valeur du booléen après l'action.</param>
        public SylAction(string name, SylActionType inType, SylConfig inSylConf, 
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
            type = SylActionType.mode;
            sylConf = inSylConf;
            prevMode = inPrevMode;
            newMode = inNewMode;
        }

        public SylAction(string name, SylConfig inSylConf,
            int inPrevNrPieds, int inNewNrPieds)
            : base(name)
        {
            type = SylActionType.nbrePieds;
            sylConf = inSylConf;
            prevNrPieds = inPrevNrPieds;
            newNrPieds = inNewNrPieds;
        }

        public SylAction(string name, SylConfig inSylConf,
            ExceptionMots inPrevExcMots, ExceptionMots inNewExcMots)
            : base(name)
        {
            type = SylActionType.exceptMots;
            sylConf = inSylConf;
            prevExcMots = inPrevExcMots;
            newExcMots = inNewExcMots;
        }


        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case SylActionType.sylBut:
                    if (butNr == prevNrSetButtons)
                    {
                        sylConf.ClearButton(butNr);
                    }
                    else
                    {
                        sylConf.SetSylButtonCF(butNr, prevCF);
                    }
                    break;

                case SylActionType.clearBut:
                    sylConf.SetSylButtonCF(butNr, prevCF);
                    break;

                case SylActionType.doubleCons:
                    sylConf.DoubleConsStd = prevBoolVal;
                    break;

                case SylActionType.mode:
                    sylConf.mode = prevMode;
                    break;

                case SylActionType.marquerMuettes:
                    sylConf.marquerMuettes = prevBoolVal;
                    break;

                case SylActionType.dierese:
                    sylConf.chercherDierese = prevBoolVal;
                    break;

                case SylActionType.nbrePieds:
                    sylConf.nbrPieds = prevNrPieds;
                    break;

                case SylActionType.exceptMots:
                    sylConf.ExcMots = prevExcMots;
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
                case SylActionType.sylBut:
                    sylConf.SetSylButtonCF(butNr, newCF);
                    break;

                case SylActionType.clearBut:
                    sylConf.ClearButton(butNr);
                    break;

                case SylActionType.doubleCons:
                    sylConf.DoubleConsStd = newBoolVal;
                    break;

                case SylActionType.mode:
                    sylConf.mode = newMode;
                    break;

                case SylActionType.marquerMuettes:
                    sylConf.marquerMuettes = newBoolVal;
                    break;

                case SylActionType.dierese:
                    sylConf.chercherDierese = newBoolVal;
                    break;

                case SylActionType.nbrePieds:
                    sylConf.nbrPieds = newNrPieds;
                    break;

                case SylActionType.exceptMots:
                    sylConf.ExcMots = newExcMots;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
