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
        /// "majDebCF",
        /// "majDebCB"
        /// qui ont chacune 2 paramètres: l'ancienne et la nouvelle valeur.
        /// </summary>
        private string type;
        private Ponctuation p;
        private PonctConfig.State prevMasterState;
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private bool prevCB;
        private bool newCB;

        public PonctAction(string name, 
            PonctConfig pc, 
            string inType, 
            Ponctuation inP,
            CharFormatting inPrevCF, 
            CharFormatting inNewCF,
            PonctConfig.State inPrevState = PonctConfig.State.undef)
            :base(name)
        {
            ponctConf = pc;
            type = inType;
            prevCF = inPrevCF;
            newCF = inNewCF;
            p = inP;
            prevMasterState = inPrevState;
            // pour ne rien avoir de non défini:
            prevCB = false;
            newCB = false;
        }

        public PonctAction(string name, PonctConfig pc, string inType, Ponctuation inP, bool inPrevCB, bool inNewCB)
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

        public override void Undo()
        {
            switch (type)
            {
                case "ponctCF":
                    ponctConf.SetCF(p, prevCF);
                    break;

                case "ponctCB":
                    ponctConf.SetCB(p, prevCB);
                    break;

                case "masterCF":
                    ponctConf.MasterState = prevMasterState;
                    ponctConf.SetMasterCFWithoutPropagation(prevCF);
                    break;

                case "majDebCF":
                    break;

                case "majDebCB":
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));

            }
        }

        public override void Redo()
        {
            throw new NotImplementedException();
        }

    }
}
