using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class DuoAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public enum DuoActionType { alternance, colorisFunction, nbreAlt }

        private DuoActionType type;
        private DuoConfig duoConf;
        private DuoConfig.Alternance prevAlternance;
        private DuoConfig.Alternance newAlternance;
        private DuoConfig.ColorisFunction prevColFunction;
        private DuoConfig.ColorisFunction newColFunction;
        private int prevNbrAlt;
        private int newNbrAlt;



        public DuoAction(string name, DuoConfig inDuoConf, DuoConfig.Alternance inPrevAlt, 
            DuoConfig.Alternance inNewAlt)
            : base(name)
        {
            type = DuoActionType.alternance;
            duoConf = inDuoConf;
            prevAlternance = inPrevAlt;
            newAlternance = inNewAlt;
        }

        public DuoAction(string name, DuoConfig inDuoConf, DuoConfig.ColorisFunction inPrevColF,
            DuoConfig.ColorisFunction inNewColF)
            : base(name)
        {
            type = DuoActionType.colorisFunction;
            duoConf = inDuoConf;
            prevColFunction = inPrevColF;
            newColFunction = inNewColF;
        }

        public DuoAction(string name, DuoConfig inDuoConf, int inPrevNbrAlt, int inNewNbrAlt)
            : base(name)
        {
            type = DuoActionType.nbreAlt;
            duoConf = inDuoConf;
            prevNbrAlt = inPrevNbrAlt;
            newNbrAlt = inNewNbrAlt;
        }



        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case DuoActionType.alternance:
                    duoConf.alternance = prevAlternance;
                    break;

                case DuoActionType.colorisFunction:
                    duoConf.colorisFunction = prevColFunction;
                    break;

                case DuoActionType.nbreAlt:
                    duoConf.nbreAlt = prevNbrAlt;
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
                case DuoActionType.alternance:
                    duoConf.alternance = newAlternance;
                    break;

                case DuoActionType.colorisFunction:
                    duoConf.colorisFunction = newColFunction;
                    break;

                case DuoActionType.nbreAlt:
                    duoConf.nbreAlt = newNbrAlt;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));

            }
        }
    }
}
