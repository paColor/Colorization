using System;
using System.Collections.Generic;
using System.Text;

namespace ColorLib
{
    class CLActionList:CLAction
    {
        List<CLAction> actList;

        public CLActionList(string name)
            : base (name)
        {
            actList = new List<CLAction>(20);
        }

        public override void Undo()
        {
            for (int i = actList.Count - 1; i >= 0;  i--)
            {
                actList[i].Undo();
            }
        }

        public override void Redo()
        {
            for (int i = 0; i < actList.Count; i++)
            {
                actList[i].Redo();
            }
        }

        public void Add(CLAction theAction)
        {
            actList.Add(theAction);
        }


    }
}
