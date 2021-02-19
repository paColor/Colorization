using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ColorLib
{
    class MajDebInT : TextEl
    {
        public MajDebInT(TheText tt, int pos)
            : base(tt, pos, pos)
        {}

        public override void PutColor(Config conf)
        {
            Debug.Assert(conf.ponctConf.MajDebCB);
            base.SetCharFormat(conf.ponctConf.MajDebCF);
        }
    }
}
