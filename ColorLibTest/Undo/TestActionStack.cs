using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System;

namespace ColorLibTest.Undo
{
    public class CLActionTest : CLAction 
    {

        public CLActionTest(string Name)
            : base (Name)
        {

        }

    }

    [TestClass]
    public class TestActionStack
    {
        [TestMethod]
        public void TestMethod1()
        {
            CLActionStack st = new CLActionStack();
            for (int i = 0; i < 40; i++)
            {
                CLActionTest t = new CLActionTest(i.ToString());
                st.Push(t);
            }
            for (int i = 0; i < 10; i++)
            {
                _ = st.Pop();
            }
            CLAction tcla = st.Pop();
            Assert.AreEqual("29", tcla.Name);
            tcla = st.Pop();
            Assert.AreEqual("28", tcla.Name);
            for (int i = 0; i < 28; i++)
            {
                tcla = st.Pop();
                Assert.IsNotNull(tcla);
            }
            tcla = st.Pop();
            Assert.IsNull(tcla);
        }

        [TestMethod]
        public void TestMethod2()
        {
            CLActionStack st = new CLActionStack();
            for (int i = 0; i < 2 * CLActionStack.StackSize; i++)
            {
                CLActionTest t = new CLActionTest(i.ToString());
                st.Push(t);
            }
            int lastPushedEl = 2 * CLActionStack.StackSize - 1;
            CLAction tcla = st.Pop();
            Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
            lastPushedEl--;
            tcla = st.Pop();
            Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
            lastPushedEl--;
            for (int i = 0; i < CLActionStack.StackSize - 3; i++)
            {
                tcla = st.Pop();
                Assert.IsNotNull(tcla);
                Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
                lastPushedEl--;
            }
            tcla = st.Pop();
            Assert.IsNull(tcla);
        }

    }
}
