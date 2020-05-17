using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using ColorLib;

namespace TestConfigWis
{
    public class BastText : TheText
    {
        public CharFormatting[] charFormattings;

        public BastText(string s, Config inConf)
            :base(s, inConf)
        {
            charFormattings = new CharFormatting[s.Length];
        }

        protected override void SetChars(FormattedTextEl fte)
        {
            for (int i = fte.First; i<=fte.Last; i++)
            {
                charFormattings[i] = fte.cf;
            }
        }
    }
}
