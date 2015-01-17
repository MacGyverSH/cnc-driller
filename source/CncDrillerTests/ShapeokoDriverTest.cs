using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeokoDriver;

namespace CncDrillerTests
{
    [TestClass]
    public class ShapeokoDriverTest
    {
        [TestMethod]
        public void TestTextComment()
        {

            Assert.IsNull(GComment.parse("G34"));

            GComment c = GComment.parse("G32 (test comment)");
            Assert.IsNotNull(c);
            Assert.AreEqual("test comment", c.Text);

            try
            {
                GComment.parse("G54 (test ) comment)");
                Assert.Fail();
            }
            catch (FormatException) { }

            try
            {
                GComment.parse("G57 (test ( comment)");
                Assert.Fail();
            }
            catch (FormatException) { }

            try
            {
                GComment.parse("G57 (test  comment");
                Assert.Fail();
            }
            catch (FormatException) { }
            
        }

        [TestMethod]
        public void TestSimpleCommand()
        {
            GCommand cmd = new GCommand("G32 (test comment)");

            Assert.AreEqual("test comment", cmd.Comment.Text);
            Assert.AreEqual("G32", cmd.Name);
            Assert.AreEqual("G32 (test comment)", cmd.ToString());
        }

        [TestMethod]
        public void TestCommandChain()
        {
            string[] cmds = new string[] { "G32", "G43", "G56", "G21" };
            GCommandChain cmd = new GCommandChain("G32 G43 G56 G21 (test comment)");

            Assert.AreEqual("test comment", cmd.Comment.Text);
            Assert.AreEqual("G32 G43 G56 G21", cmd.Name);

            int i = 0;
            foreach (GCommand c in cmd.Commands)
            {
                Assert.AreEqual(cmds[i++], c.Name);
            }

        }


        [TestMethod]
        public void TestQueue()
        {
            GCommand c1 = new GCommand("G23");
            GCommand c2 = new GCommand("G53");
            GCommand c3 = new GCommand("G76");
            GCommand c4 = new GCommandChain("G74 G23");

            GQueue<GCommand> g = new GQueue<GCommand>();

            g.Enqueue(c1);
            g.Enqueue(c2);
            g.Enqueue(c3);
            g.Enqueue(c4);

            Assert.AreEqual(c1,g.Dequeue());
            Assert.AreEqual(c2, g.Dequeue());
            Assert.AreEqual(c3, g.Dequeue());
            Assert.AreEqual(c4, g.Dequeue());

        }

        [TestMethod]
        public void TestReplyStatus()
        {
            string reply = "<Idle,MPos:-115.400,0.000,-15.200,WPos:-115.400,0.000,-15.200>";

            GReplyStatus status = new GReplyStatus(reply);

            

        }

    }
}
