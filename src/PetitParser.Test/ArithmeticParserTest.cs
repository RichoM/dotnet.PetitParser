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

        [TestMethod]
        public void TestAdd()
        {
            Assert.AreEqual(3.0, pp.Parse("1 + 2"));
            Assert.AreEqual(3.0, pp.Parse("2 + 1"));
            Assert.AreEqual(3.3, pp.Parse("1 + 2.3"));
            Assert.AreEqual(3.3, pp.Parse("2.3 + 1"));
            Assert.AreEqual(-1.0, pp.Parse("1 + -2"));
            Assert.AreEqual(-1.0, pp.Parse("-2 + 1"));
        }

        [TestMethod]
        public void TestAddMany()
        {
            Assert.AreEqual(3.0, pp.Parse("1 + 2"));
            Assert.AreEqual(6.0, pp.Parse("1 + 2 + 3"));
            Assert.AreEqual(10.0, pp.Parse("1 + 2 + 3 + 4"));
            Assert.AreEqual(15.0, pp.Parse("1 + 2 + 3 + 4 + 5"));
        }

        [TestMethod]
        public void TestDiv()
        {
            Assert.AreEqual(4.0, pp.Parse("12 / 3"));
            Assert.AreEqual(4.0, pp.Parse("-16 / -4"));
        }

        [TestMethod]
        public void TestDivMany()
        {
            Assert.AreEqual(50.0, pp.Parse("100 / 2"));
            Assert.AreEqual(25.0, pp.Parse("100 / 2 / 2"));
            Assert.AreEqual(5.0, pp.Parse("100 / 2 / 2 / 5"));
            Assert.AreEqual(1.0, pp.Parse("100 / 2 / 2 / 5 / 5"));
        }

        [TestMethod]
        public void TestMul()
        {
            Assert.AreEqual(6.0, pp.Parse("2 * 3"));
            Assert.AreEqual(-8.0, pp.Parse("2 * -4"));
        }

        [TestMethod]
        public void TestMulMany()
        {
            Assert.AreEqual(2.0, pp.Parse("1 * 2"));
            Assert.AreEqual(6.0, pp.Parse("1 * 2 * 3"));
            Assert.AreEqual(24.0, pp.Parse("1 * 2 * 3 * 4"));
            Assert.AreEqual(120.0, pp.Parse("1 * 2 * 3 * 4 * 5"));
        }

        [TestMethod]
        public void TestPow()
        {
            Assert.AreEqual(8.0, pp.Parse("2 ^ 3"));
            Assert.AreEqual(-8.0, pp.Parse("-2 ^ 3"));
            Assert.AreEqual(-0.125, pp.Parse("-2 ^ -3"));
        }

        [TestMethod]
        public void TestPowMany()
        {
            Assert.AreEqual(64.0, pp.Parse("4 ^ 3"));
            Assert.AreEqual(262144.0, pp.Parse("4 ^ 3 ^ 2"));
            Assert.AreEqual(262144.0, pp.Parse("4 ^ 3 ^ 2 ^ 1"));
            Assert.AreEqual(262144.0, pp.Parse("4 ^ 3 ^ 2 ^ 1 ^ 0"));
        }

        [TestMethod]
        public void TestSub()
        {
            Assert.AreEqual(-1.0, pp.Parse("1 - 2"));
            Assert.AreEqual(0.0, pp.Parse("1.2 - 1.2"));
            Assert.AreEqual(3.0, pp.Parse("1 - -2"));
            Assert.AreEqual(1.0, pp.Parse("-1 - -2"));
        }

        [TestMethod]
        public void TestSubMany()
        {
            Assert.AreEqual(-1.0, pp.Parse("1 - 2"));
            Assert.AreEqual(-4.0, pp.Parse("1 - 2 - 3"));
            Assert.AreEqual(-8.0, pp.Parse("1 - 2 - 3 - 4"));
            Assert.AreEqual(-13.0, pp.Parse("1 - 2 - 3 - 4 - 5"));
        }

        [TestMethod]
        public void TestPriority()
        {
            Assert.AreEqual(10.0, pp.Parse("2 * 3 + 4"));
            Assert.AreEqual(14.0, pp.Parse("2 + 3 * 4"));
            Assert.AreEqual(6.0, pp.Parse("6 / 3 + 4"));
            Assert.AreEqual(5.0, pp.Parse("2 + 6 / 2"));
        }

        [TestMethod]
        public void TestBrackets()
        {
            Assert.AreEqual(1.0, pp.Parse("(1)"));
            Assert.AreEqual(3.0, pp.Parse("(1 + 2)"));
            Assert.AreEqual(1.0, pp.Parse("((1))"));
            Assert.AreEqual(3.0, pp.Parse("((1 + 2))"));
            Assert.AreEqual(14.0, pp.Parse("2 * (3 + 4)"));
            Assert.AreEqual(20.0, pp.Parse("(2 + 3) * 4"));
            Assert.AreEqual(1.0, pp.Parse("6 / (2 + 4)"));
            Assert.AreEqual(4.0, pp.Parse("(2 + 6) / 2"));
        }
    }
}
