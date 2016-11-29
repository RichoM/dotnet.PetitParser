using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Test
{
    [TestClass]
    public class ArithmeticParserTest
    {
        private ArithmeticParser pp;

        [TestInitialize]
        public void Setup()
        {
            pp = new ArithmeticParser();
        }

        [TestMethod]
        public void TestNum()
        {
            Assert.AreEqual(0.0, pp.Parse("0"));
            Assert.AreEqual(0.0, pp.Parse("0.0"));
            Assert.AreEqual(1.0, pp.Parse("1"));
            Assert.AreEqual(1.2, pp.Parse("1.2"));
            Assert.AreEqual(34.0, pp.Parse("34"));
            Assert.AreEqual(56.78, pp.Parse("56.78"));
            Assert.AreEqual(-9.0, pp.Parse("-9"));
            Assert.AreEqual(-9.9, pp.Parse("-9.9"));
        }
    }
}
