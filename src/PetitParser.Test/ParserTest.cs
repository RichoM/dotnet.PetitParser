using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using PetitParser.Utils;
using PetitParser.Results;

namespace PetitParser.Test
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void TestLiteralObjectParser()
        {
            Parser pp = 'a'.AsParser();
            Assert.AreEqual('a', pp.Parse("a"));
            Throws<ParseException>(() => pp.Parse("b"));
        }

        [TestMethod]
        public void TestSequenceParserCreatedFromString()
        {
            var pp = "abc".AsParser();
            var actual = pp.Parse<object[]>("abc");
            var expected = new char[] { 'a', 'b', 'c' };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("abd"));
        }

        [TestMethod]
        public void TestSequenceParserCreatedByThen()
        {
            var pp = 'a'.AsParser()
                .Then('b'.AsParser())
                .Then('c'.AsParser());
            var actual = pp.Parse<object[]>("abc");
            var expected = new char[] { 'a', 'b', 'c' };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("abd"));
        }

        [TestMethod]
        public void TestChoiceParser()
        {
            var pp = "perro".AsParser().Or("gato".AsParser());
            Assert.AreEqual("perro", string.Join("", pp.Parse<object[]>("perro")));
            Throws<ParseException>(() => pp.Parse("ratón"));
        }

        [TestMethod]
        public void TestFlattenParser()
        {
            var pp = "perro".AsParser().Flatten();
            Assert.AreEqual("perro", pp.Parse("perro"));
        }

        [TestMethod]
        public void TestAndParser()
        {
            var pp = "foo".AsParser().Flatten()
                .Then("bar".AsParser().Flatten().And());
            Stream stream = "foobar".ReadStream();
            ParseResult result = pp.ParseOn(stream);
            string[] actual = ((object[])result.ActualResult)
                .Cast<string>()
                .ToArray();
            string[] expected = new string[] { "foo", "bar" };

            CollectionAssert.AreEqual(expected, actual);
            Assert.AreEqual(3, stream.Position);
        }

        [TestMethod]
        public void TestEndParser()
        {
            var pp = "foo".AsParser().Flatten().End();
            Assert.AreEqual("foo", pp.Parse("foo"));
            Throws<ParseException>(() => pp.Parse("foobar"));
        }

        [TestMethod]
        public void TestRepeatingParserStar()
        {
            var pp = "foo".AsParser().Flatten().Star();
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserStarNoElement()
        {
            var pp = "foo".AsParser().Flatten().Star();
            string[] actual = pp.Parse<object[]>("").Cast<string>().ToArray();
            string[] expected = new string[0];
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserPlus()
        {
            var pp = "foo".AsParser().Flatten().Plus();
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserPlusNoElement()
        {
            var pp = "foo".AsParser().Flatten().Plus();
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("bar"));
        }

        [TestMethod]
        public void TestRepeatingParserTimes()
        {
            var pp = "foo".AsParser().Flatten().Times(3);
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("foofoo"));
        }

        [TestMethod]
        public void TestNotParser()
        {
            var pp = 'a'.AsParser().Not();
            Stream stream = "b".ReadStream();
            Assert.IsNull(pp.ParseOn(stream).ActualResult);
            Assert.AreEqual(0, stream.Position);
            Throws<ParseException>(() => pp.Parse("a"));
        }

        [TestMethod]
        public void TestOptionalParser()
        {
            var pp = "foo".AsParser().Flatten().Optional();
            Assert.AreEqual("foo", pp.Parse("foo"));
            Assert.IsNull(pp.Parse("bar"));
        }

        [TestMethod]
        public void TestTokenParser()
        {
            var pp = "foo".AsParser().Token();
            Token token = pp.Parse<Token>("foo");
            Assert.AreEqual(0, token.Start);
            Assert.AreEqual(3, token.Length);
            CollectionAssert.AreEqual(
                new char[] { 'f', 'o', 'o' },
                (object[])token.ParsedValue);
            Assert.AreEqual("foo", token.InputValue);
        }

        private void Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
                Assert.Fail("Execution should not reach here");
            }
            catch (T) { }
            catch (Exception ex)
            {
                Assert.Fail("Invalid exception: {0}", ex);
            }
        }
    }
}
