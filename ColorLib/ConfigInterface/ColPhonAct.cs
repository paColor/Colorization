using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    /// <summary>
    /// Action en relation avec <see cref="ColConfWin"/>.
    /// </summary>
    public class ColPhonAct : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// On distingue les commandes suivantes:
        /// "sonCF"
        /// "sonCB"
        /// "ill"
        /// "defBeh"
        /// "grapheme"
        /// </summary>
        private string type;
        private ColConfWin ccw;
        private string son;
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private bool prevCB;
        private bool newCB;
        private ColConfWin.IllRule prevIllRule;
        private ColConfWin.IllRule newIllRule;
        private ColConfWin.DefBeh prevDefBeh;
        private ColConfWin.DefBeh newDefBeh;
        private Dictionary<string, bool> prevGrConf;
        private Dictionary<string, bool> newGrConf;
        private string grSon;


        /// <summary>
        /// Crée une "action" pour une modification du formatage d'un son.
        /// </summary>
        /// <remarks>Il s'agit d'une action de type "sonCF".</remarks>
        /// <param name="name">Le nom de l'action.</param>
        /// <param name="inCcw">Le <see cref="ColConfWin"/> auquel l'action se rapporte.</param>
        /// <param name="inSon">Le son sur lequel porte l'action.</param>
        /// <param name="inPrevCF">La valeur précédente de formatage (avant l'action).</param>
        /// <param name="inNewCF">La nouvelle valeur de formatage (après l'action).</param>
        public ColPhonAct(string name, ColConfWin inCcw, string inSon, 
            CharFormatting inPrevCF, CharFormatting inNewCF)
            :base(name)
        {
            type = "sonCF";
            ccw = inCcw;
            son = inSon;
            prevCF = inPrevCF;
            newCF = inNewCF;
            // pour éviter les membres non définis
            prevCB = false;
            newCB = false;
            prevIllRule = ColConfWin.IllRule.undefined;
            newIllRule = ColConfWin.IllRule.undefined;
            prevDefBeh = ColConfWin.DefBeh.undefined;
            newDefBeh = ColConfWin.DefBeh.undefined;
        }

        /// <summary>
        /// Crée une "action" pour une modification de la checkbox d'un son.
        /// </summary>
        /// <remarks>Il s'agit d'une action de type "sonCB".</remarks>
        /// <param name="name">Le nom de l'action.</param>
        /// <param name="inCcw">Le <see cref="ColConfWin"/> auquel l'action se rapporte.</param>
        /// <param name="inSon">Le son sur lequel porte l'action.</param>
        /// <param name="inPrevCB">La valeur précédente de la checkbox (avant l'action).</param>
        /// <param name="inNewCB">La nouvelle valeur de la checkbox (après l'action).</param>
        public ColPhonAct(string name, ColConfWin inCcw, string inSon,
            bool inPrevCB, bool inNewCB)
            : base(name)
        {
            type = "sonCB";
            ccw = inCcw;
            son = inSon;
            prevCB = inPrevCB;
            newCB = inNewCB;
            // pour éviter les membres non définis
            prevCF = null;
            newCF = null;
            prevIllRule = ColConfWin.IllRule.undefined;
            newIllRule = ColConfWin.IllRule.undefined;
            prevDefBeh = ColConfWin.DefBeh.undefined;
            newDefBeh = ColConfWin.DefBeh.undefined;
        }

        /// <summary>
        /// Crée une "action" pour une modification de la règle "ill".
        /// </summary>
        /// <remarks>Il s'agit d'une action de type "ill".</remarks>
        /// <param name="name">Le nom de l'action.</param>
        /// <param name="inCcw">Le <see cref="ColConfWin"/> auquel l'action se rapporte.</param>
        /// <param name="inPrevIllRule">La valeur précédente de la règle "ill" 
        /// (avant l'action).</param>
        /// <param name="inNewIllRule">La nouvelle valeur de la règle "ill"
        /// (après l'action).</param>
        public ColPhonAct(string name, ColConfWin inCcw,
            ColConfWin.IllRule inPrevIllRule, ColConfWin.IllRule inNewIllRule)
            : base(name)
        {
            type = "ill";
            ccw = inCcw;
            prevIllRule = inPrevIllRule;
            newIllRule = inNewIllRule;
            // pour éviter les membres non définis
            son = null;
            prevCF = null;
            newCF = null;
            prevCB = false;
            newCB = false;
            prevDefBeh = ColConfWin.DefBeh.undefined;
            newDefBeh = ColConfWin.DefBeh.undefined;
        }

        /// <summary>
        /// Crée une "action" pour une modification du comportement par défaut.
        /// </summary>
        /// <remarks>Il s'agit d'une action de type "defBeh".</remarks>
        /// <param name="name">Le nom de l'action.</param>
        /// <param name="inCcw">Le <see cref="ColConfWin"/> auquel l'action se rapporte.</param>
        /// <param name="inPrevIllRule">La valeur précédente de la règle "ill" 
        /// (avant l'action).</param>
        /// <param name="inNewIllRule">La nouvelle valeur de la règle "ill"
        /// (après l'action).</param>
        public ColPhonAct(string name, ColConfWin inCcw,
            ColConfWin.DefBeh inPrevDefBeh, ColConfWin.DefBeh inNewDefBeh)
            : base(name)
        {
            type = "defBeh";
            ccw = inCcw;
            prevDefBeh = inPrevDefBeh;
            newDefBeh = inNewDefBeh;
            // pour éviter les membres non définis
            son = null;
            prevCF = null;
            newCF = null;
            prevCB = false;
            newCB = false;
            prevIllRule = ColConfWin.IllRule.undefined;
            newIllRule = ColConfWin.IllRule.undefined;
        }

        /// <summary>
        /// Crée une "action" pour une modification des graphèmes relatifs à un son.
        /// </summary>
        /// <param name="inCcw">Le <see cref="ColConfWin"/> auquel l'action se rapporte.</param>
        /// <param name="inSon">Le son dont la liste de graphèmes est modifiée</param>
        /// <param name="inPrevGrConf">La liste de grpahèmes avant la modification</param>
        /// <param name="inNewGrConf">La liste de grpahèmes après la modification</param>
        public ColPhonAct(ColConfWin inCcw, string inSon, Dictionary<string, bool> inPrevGrConf,
            Dictionary<string, bool> inNewGrConf)
            : base(String.Format("Graphèmes pour {0}", inSon))
        {
            type = "grapheme";
            ccw = inCcw;
            prevGrConf = inPrevGrConf;
            newGrConf = inNewGrConf;
            grSon = inSon;
            // pour éviter les membres non définis
            son = null;
            prevCF = null;
            newCF = null;
            prevCB = false;
            newCB = false;
            prevIllRule = ColConfWin.IllRule.undefined;
            newIllRule = ColConfWin.IllRule.undefined;
            prevDefBeh = ColConfWin.DefBeh.undefined;
            newDefBeh = ColConfWin.DefBeh.undefined;
        }

        public override void Undo()
        {
            logger.ConditionalDebug("Undo");
            switch (type)
            {
                case "sonCF":
                    if (prevCF != null)
                        ccw.SetCFSon(son, prevCF);
                    break;

                case "sonCB":
                    ccw.SetChkSon(son, prevCB);
                    break;

                case "ill":
                    ccw.IllRuleToUse = prevIllRule;
                    break;

                case "defBeh":
                    ccw.SetDefaultBehaviourTo(prevDefBeh);
                    break;

                case "grapheme":
                    ccw.SetGraphemes(grSon, prevGrConf);
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
                case "sonCF":
                    ccw.SetCFSon(son, newCF);
                    break;

                case "sonCB":
                    ccw.SetChkSon(son, newCB);
                    break;

                case "ill":
                    ccw.IllRuleToUse = newIllRule;
                    break;

                case "defBeh":
                    ccw.SetDefaultBehaviourTo(newDefBeh);
                    break;

                case "grapheme":
                    ccw.SetGraphemes(grSon, newGrConf);
                    break;

                default:
                    logger.Error("Type de commande non traitée: {0}", type);
                    throw new InvalidOperationException(String.Format("Type de commande non traitée: {0}", type));
            }
        }
    }
}
