using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    public class PonctAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private PonctConfig ponctConf;

        /// <summary>
        /// On distingue 5 types de commandes:
        /// "ponctCF",
        /// "ponctCB",
        /// "masterCF",
        /// "masterState"
        /// "majDebCF",
        /// "majDebCB"
        /// qui ont chacune 2 paramètres: l'ancienne et la nouvelle valeur.
        /// </summary>
        private string type;
        private Ponctuation p;
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private bool prevCB;
        private bool newCB;
        PonctConfig.State prevMasterState;
        PonctConfig.State newMasterState;

        public PonctAction(string name, 
            PonctConfig pc, 
            string inType, 
            Ponctuation inP,
            CharFormatting inPrevCF, 
            CharFormatting inNewCF)
            :base(name)
        {
            ponctConf = pc;
            type = inType;
            prevCF = inPrevCF;
            newCF = inNewCF;
            p = inP;
            // pour ne rien avoir de non défini:
            prevCB = false;
            newCB = false;
        }

        public PonctAction(string name, 
            PonctConfig pc, 
            string inType, 
            Ponctuation inP, 
            bool inPrevCB, 
            bool inNewCB)
            : base(name)
        {
            ponctConf = pc;
            type = inType;
            prevCB = inPrevCB;
            newCB = inNewCB;
            p = inP;
            // pour ne rien avoir de non défini:
            prevCF = null;
            newCF = null;
        }

        public PonctAction(string name,
            PonctConfig pc,
            string inType,
            PonctConfig.State inPrevMasterState,
            PonctConfig.State inNewMasterState)
            :base(name)
        {
            ponctConf = pc;
            type = inType;
            prevMasterState = inPrevMasterState;
            newMasterState = inNewMasterState;

            // pour ne rien avoir de non défini:
            prevCF = null; ;
            newCF = null;
            p = Ponctuation.firstP;
            prevCB = false;
            newCB = false;
        }

        public override string ToString()
        {
            return String.Format("{0} - type: {1}, prevMState: {2}", Name, type, prevMasterState);
        }

        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case "ponctCF":
                    ponctConf.SetCFwoState(p, prevCF);
                    break;

                case "ponctCB":
                    ponctConf.SetCBwoState(p, prevCB);
                    break;

                case "masterCF":
                    ponctConf.SetMasterCFWithoutPropagation(prevCF);
                    // les familles de ponctuation sont restaurées individuellement.
                    break;

                case "masterState":
                    ponctConf.MasterState = prevMasterState;
                    break;

                case "majDebCF":
                    ponctConf.MajDebCF = prevCF;
                    break;

                case "majDebCB":
                    ponctConf.MajDebCB = prevCB;
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
                case "ponctCF":
                    ponctConf.SetCFwoState(p, newCF);
                    break;

                case "ponctCB":
                    ponctConf.SetCBwoState(p, newCB);
                    break;

                case "masterCF":
                    ponctConf.SetMasterCFWithoutPropagation(newCF);
                    // les familles de ponctuation sont restaurées individuellement.
                    break;

                case "masterState":
                    ponctConf.MasterState = newMasterState;
                    break;

                case "majDebCF":
                    ponctConf.MajDebCF = newCF;
                    break;

                case "majDebCB":
                    ponctConf.MajDebCB = newCB;
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }

    }
}
