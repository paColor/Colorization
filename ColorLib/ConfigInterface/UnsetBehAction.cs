using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class UnsetBehAction : CLAction
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        UnsetBehConf ubc;
        Ucbx flag;
        bool prevFlagValue;
        bool newFlagValue;


        public UnsetBehAction(string name, UnsetBehConf inUbc, Ucbx inUcbx, bool inPrevFlag, 
            bool inNewFlag)
            : base(name)
        {
            ubc = inUbc;
            flag = inUcbx;
            prevFlagValue = inPrevFlag;
            newFlagValue = inNewFlag;
        }

        public override void Undo()
        {
            ubc.SetCbuFlag(flag, prevFlagValue);
        }

        public override void Redo()
        {
            ubc.SetCbuFlag(flag, newFlagValue);
        }
    }
}
