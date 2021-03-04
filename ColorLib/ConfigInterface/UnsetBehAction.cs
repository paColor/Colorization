using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class UnsetBehAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        UnsetBehConf ubc;
        Ucbx ucbx;
        bool prevFlag;
        bool newFlag;


        public UnsetBehAction(string name, UnsetBehConf inUbc)
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
