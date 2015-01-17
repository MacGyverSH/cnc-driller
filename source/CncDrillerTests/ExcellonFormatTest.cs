using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using ExcellonFormat;
using CodeUtils;

namespace CncDrillerTests
{
    [TestClass]
    public class ExcellonFormatTest
    {
        [TestMethod]
        public void TestHole()
        {

            ExcellonFile fi = new ExcellonFile(new MeasureUnitImperial());

            Assert.IsNull(Hole.createFromData("X2999", null, fi));

            Hole h = Hole.createFromData("X-24999Y26987", null, fi);
            Assert.IsNotNull(h);

            Assert.AreEqual(-63.49746, h.Coords.X, 0.0001, "Coordinate X is bad");
            Assert.AreEqual(68.54698, h.Coords.Y, 0.0001, "Coordinate Y is bad");

            fi = new ExcellonFile(new MeasureUnitMetric());

            h = Hole.createFromData("X-2999Y26987", null, fi);
            Assert.IsNotNull(h);

            Assert.AreEqual(-0.2999, h.Coords.X, 0.0001, "Coordinate X is bad");
            Assert.AreEqual(2.6987, h.Coords.Y, 0.0001, "Coordinate Y is bad");


            h = Hole.createFromData("X-2999Y26987", null, null);
            Assert.IsNotNull(h);

            Assert.AreEqual(-0.2999, h.Coords.X, 0.0001, "Coordinate X is bad");
            Assert.AreEqual(2.6987, h.Coords.Y, 0.0001, "Coordinate Y is bad");

            
            fi = new ExcellonFile();
            Tool t = new Tool(1, 0, fi);
            h = new Hole(30, 20, t, fi);

            Assert.AreEqual(t, h.ActiveTool, "Bad active tool");
            Assert.AreEqual(fi, h.File, "Bad owner file");
            Assert.AreEqual(30, h.Coords.X, "Coordinate X is bad");
            Assert.AreEqual(20, h.Coords.Y, "Coordinate Y is bad");

            h = new Hole(new GVector(70, 30), t, fi);

            Assert.AreEqual(t, h.ActiveTool, "Bad active tool");
            Assert.AreEqual(fi, h.File, "Bad owner file");
            Assert.AreEqual(70, h.Coords.X, "Coordinate X is bad");
            Assert.AreEqual(30, h.Coords.Y, "Coordinate Y is bad");

            Assert.AreEqual("Vector2(70, 30)", h.ToString());

        }

        [TestMethod]
        public void TestParseFile()
        {
            double[] coordX = { 2.0249, 2.4999, -2.5374, 2.6874, -2.6499, 2.3749, 2.3499, 2.3624, 2.2749, -1.1887, 1.1887, 5.1257, 5.1257 };
            double[] coordY = { 2.4112, 2.6987, 2.6612, -2.7862, -2.8487, 2.9987, 2.9487, 3.1237, 3.1487, -2.2221, 4.5055, 4.5055, 2.2024 };

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "CncDrillerTests.excellon_test1.drl";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                Assert.IsNotNull(stream, "Test corrupted");

                ExcellonFile fi = new ExcellonFile();

                fi.readFromStream(stream);

                Assert.AreEqual(13, fi.Holes.Count, "Holes count isn't equal");
                Assert.AreEqual(8, fi.Tools.List.Count, "Tools count isn't equal");

                Tool tool01 = fi.Tools.List[0];
                Tool tool08 = fi.Tools.List[7];

                int i = 0;
                foreach(Hole h in fi.Holes)
                {
                    Assert.AreEqual(fi, h.File, "Hole parent file isn't base file");

                    Assert.AreEqual(2, h.Coords.Size, "Hole coordinate vector haven't 2 dimension");

                    Assert.AreEqual(coordX[i], h.Coords.X, "Coordinate X is bad on line {0}", i);
                    Assert.AreEqual(coordY[i], h.Coords.Y, "Coordinate Y is bad on line {0}", i);

                    Tool t = null;
                    if (i < 9)
                    {
                        t = tool01;
                    }
                    else
                    {
                        t = tool08;
                    }
                    Assert.AreEqual(t, h.ActiveTool, "Hole use bad tool");

                    i++;
                }

            }

        }
    }
}
