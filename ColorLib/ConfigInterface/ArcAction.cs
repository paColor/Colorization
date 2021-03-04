using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class ArcAction : CLAction
    {
        public enum ArcActType { arcButton, clearArcBut, hauteur, ecartement, epaisseur, decalage };

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private ArcActType type;
        private ArcConfig arcConf;
        private int butNr;
        private int prevNrSetArcButtons;
        private RGB prevCol;
        private RGB newCol;
        private float prevFloat;
        private float newFloat;
        private int prevInt;
        private int newInt;

        public ArcAction(string name, ArcConfig inArcConf, int inButNr, int inNrSetABut, 
            RGB inPrevCol, RGB inNewCol)
            : base(name)
        {
            type = ArcActType.arcButton;
            arcConf = inArcConf;
            butNr = inButNr;
            prevNrSetArcButtons = inNrSetABut;
            prevCol = inPrevCol;
            newCol = inNewCol;
        }

        public ArcAction(string name, ArcConfig inArcConf, int inButNr, RGB inPrevCol)
            : base(name)
        {
            type = ArcActType.clearArcBut;
            arcConf = inArcConf;
            butNr = inButNr;
            prevCol = inPrevCol;

        }

        public ArcAction(string name, ArcActType inType, ArcConfig inArcConf, float inPrevFloat,
            float inNewFloat)
            : base(name)
        {
            type = inType;
            arcConf = inArcConf;
            prevFloat = inPrevFloat;
            newFloat = inNewFloat;
        }

        public ArcAction(string name, ArcActType inType, ArcConfig inArcConf, int inPrevInt,
            int inNewInt)
            : base(name)
        {
            type = inType;
            arcConf = inArcConf;
            prevInt = inPrevInt;
            newInt = inNewInt;
        }

        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case ArcActType.arcButton:
                    if (butNr == prevNrSetArcButtons)
                    {
                        arcConf.ClearButton(butNr);
                    }
                    else
                    {
                        arcConf.SetArcButtonCol(butNr, prevCol);
                    }
                    break;

                case ArcActType.clearArcBut:
                    arcConf.SetArcButtonCol(butNr, prevCol);
                    break;

                case ArcActType.decalage:
                    arcConf.Decalage = prevFloat;
                    break;

                case ArcActType.ecartement:
                    arcConf.Ecartement = prevInt;
                    break;

                case ArcActType.epaisseur:
                    arcConf.Epaisseur = prevFloat;
                    break;

                case ArcActType.hauteur:
                    arcConf.Hauteur = prevInt;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }

        public override void Redo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case ArcActType.arcButton:
                    arcConf.SetArcButtonCol(butNr, newCol);
                    break;

                case ArcActType.clearArcBut:
                    arcConf.ClearButton(butNr);
                    break;

                case ArcActType.decalage:
                    arcConf.Decalage = newFloat;
                    break;

                case ArcActType.ecartement:
                    arcConf.Ecartement = newInt;
                    break;

                case ArcActType.epaisseur:
                    arcConf.Epaisseur = newFloat;
                    break;

                case ArcActType.hauteur:
                    arcConf.Hauteur = newInt;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
