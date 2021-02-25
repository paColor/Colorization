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

        public override void Redo()
        {
            throw new NotImplementedException();
        }

        public override void Undo()
        {
            throw new NotImplementedException();
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
            Assert.AreEqual(40, st.Count);
            for (int i = 0; i < 10; i++)
            {
                _ = st.Pop();
            }
            Assert.AreEqual(30, st.Count);
            CLAction tcla = st.Pop();
            Assert.AreEqual("29", tcla.Name);
            Assert.AreEqual(29, st.Count);
            tcla = st.Pop();
            Assert.AreEqual("28", tcla.Name);
            Assert.AreEqual(28, st.Count);
            for (int i = 0; i < 28; i++)
            {
                tcla = st.Pop();
                Assert.IsNotNull(tcla);
            }
            tcla = st.Pop();
            Assert.IsNull(tcla);
            Assert.AreEqual(0, st.Count);
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
            Assert.AreEqual(CLActionStack.StackSize - 1, st.Count);
            int lastPushedEl = 2 * CLActionStack.StackSize - 1;
            CLAction tcla = st.Pop();
            Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
            Assert.AreEqual(CLActionStack.StackSize - 2, st.Count);
            lastPushedEl--;
            tcla = st.Pop();
            Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
            Assert.AreEqual(CLActionStack.StackSize - 3, st.Count);
            lastPushedEl--;
            for (int i = 0; i < CLActionStack.StackSize - 3; i++)
            {
                tcla = st.Pop();
                Assert.IsNotNull(tcla);
                Assert.AreEqual(lastPushedEl.ToString(), tcla.Name);
                lastPushedEl--;
            }
            Assert.AreEqual(0, st.Count);
            tcla = st.Pop();
            Assert.IsNull(tcla);
            Assert.AreEqual(0, st.Count);
        }

    }
}
