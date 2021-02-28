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
        private CharFormatting prevCF;
        private CharFormatting newCF;
        private int prevButNr;
        private int newButNr;
        private char prevLetter;
        private char newLetter;
        private bool prevMarkAsBlack;
        private bool newMarkAsBlack;

        public PBDQAction(string name)
            : base(name)
        {

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
