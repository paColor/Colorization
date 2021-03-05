using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    public class ConfigAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public enum ConfigActionType { confName, duoConf}

        private ConfigActionType type;
        private Config conf;
        private string prevConfName;
        private string newConfName;
        private DuoConfig prevDuoConf;
        private DuoConfig newDuoConf;

        public ConfigAction(string name, Config inConf, string inPrevConfName, string inNewConfName)
            : base (name)
        {
            type = ConfigActionType.confName;
            conf = inConf;
            prevConfName = inPrevConfName;
            newConfName = inNewConfName;
        }

        public ConfigAction(string name, Config inConf, DuoConfig inPrevDuoConf,
            DuoConfig inNewDuoConf)
            : base(name)
        {
            type = ConfigActionType.duoConf;
            conf = inConf;
            prevDuoConf = inPrevDuoConf;
            newDuoConf = inNewDuoConf;
        }

        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case ConfigActionType.confName:
                    conf.SetConfigName(prevConfName);
                    break;

                case ConfigActionType.duoConf:
                    conf.duoConf = prevDuoConf;
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
                case ConfigActionType.confName:
                    conf.SetConfigName(newConfName);
                    break;

                case ConfigActionType.duoConf:
                    conf.duoConf = newDuoConf;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
