using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeUtils;

namespace CncDrillerTests
{
    [TestClass]
    public class CodeUtilsTest
    {
        [TestMethod]
        public void TestLineParser()
        {
            string[] lines = { "M48", "M72", "T03C0.0394", "T04C0.0400", "T05C0.0520", "T06C0.1102", "T07C0.1280", "T08C0.1575", "T01", "X20249Y24112", "X24999Y26987" };
            char[] commands = { 'M', 'M', 'T', 'C', 'T', 'C', 'T', 'C', 'T', 'C', 'T', 'C', 'T', 'C', 'T', 'X', 'Y', 'X', 'Y' };
            string[] data = { "48", "72", "03", "0.0394", "04", "0.0400", "05", "0.0520", "06", "0.1102", "07", "0.1280", "08", "0.1575", "01", "20249", "24112", "24999", "26987" };
            int commandPos = 0;
            int dataPos = 0;
          
            foreach(string line in lines){

                LineParser lp = new LineParser(line);
                
                while (lp.next())
                {

                    Assert.AreEqual(data[dataPos], lp.Data, "Data isn't correct", line);
                    Assert.AreEqual(commands[commandPos], lp.Command, "Data isn't correct", line);

                    commandPos++;
                    dataPos++;
                }

            }

        }

        [TestMethod]
        public void TestGVector()
        {
            GVector v1 = new GVector(0, 5, 0);
            Assert.AreEqual(5, v1.Length);

            GVector v2 = new GVector(-5, 5, 0);

            GVector v3 = new GVector(-5, 10, 0);

            Assert.AreEqual(v3, v1 + v2);

            GVector v4 = new GVector(5, 0, 0);

            Assert.AreEqual(0, v4.Dot(v1));

        }

        [TestMethod]
        public void TestGVectorDot()
        {
            GVector v1 = new GVector(0, 5, 0);

            GVector v2 = new GVector(5, 0, 0);

            Assert.AreEqual(0, v2.Dot(v1));

        }

        [TestMethod]
        public void TestGVectorAngle()
        {
            GVector v1 = new GVector(0, 5, 0);

            GVector v2 = new GVector(5, 0, 0);

            Assert.AreEqual(Math.PI /2, v2.Angle(v1), 0.01);

        }

        [TestMethod]
        public void TestGVectorNmalize()
        {
            GVector v1 = new GVector(0, 5, 0);

            Assert.AreEqual(new GVector(0, 1, 0), v1.Normalize);

        }
    }
}
