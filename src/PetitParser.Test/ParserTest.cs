﻿using System;
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
            var pp = "perro".AsParser().Flatten;
            Assert.AreEqual("perro", pp.Parse("perro"));
        }

        [TestMethod]
        public void TestAndParser()
        {
            var pp = "foo".AsParser().Flatten
                .Then("bar".AsParser().Flatten.And);
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
            var pp = "foo".AsParser().Flatten.End;
            Assert.AreEqual("foo", pp.Parse("foo"));
            Throws<ParseException>(() => pp.Parse("foobar"));
        }

        [TestMethod]
        public void TestRepeatingParserStar()
        {
            var pp = "foo".AsParser().Flatten.Star;
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserStarNoElement()
        {
            var pp = "foo".AsParser().Flatten.Star;
            string[] actual = pp.Parse<object[]>("").Cast<string>().ToArray();
            string[] expected = new string[0];
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserPlus()
        {
            var pp = "foo".AsParser().Flatten.Plus;
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRepeatingParserPlusNoElement()
        {
            var pp = "foo".AsParser().Flatten.Plus;
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("bar"));
        }

        [TestMethod]
        public void TestRepeatingParserTimes()
        {
            var pp = "foo".AsParser().Flatten.Times(3);
            string[] actual = pp.Parse<object[]>("foofoofoo").Cast<string>().ToArray();
            string[] expected = new string[] { "foo", "foo", "foo" };
            CollectionAssert.AreEqual(expected, actual);
            Throws<ParseException>(() => pp.Parse("foofoo"));
        }

        [TestMethod]
        public void TestRepeatingParserMin()
        {
            var pp = "foo".AsParser().Flatten.Min(3).End;
            CollectionAssert.AreEqual(new string[] { "foo", "foo", "foo" }, pp.Parse<object[]>("foofoofoo"));
            CollectionAssert.AreEqual(new string[] { "foo", "foo", "foo", "foo" }, pp.Parse<object[]>("foofoofoofoo"));
            Throws<ParseException>(() => pp.Parse("foofoo"));
        }

        [TestMethod]
        public void TestRepeatingParserMax()
        {
            var pp = "foo".AsParser().Flatten.Max(3).End;
            CollectionAssert.AreEqual(new string[] { "foo", "foo", "foo" }, pp.Parse<object[]>("foofoofoo"));
            CollectionAssert.AreEqual(new string[] { "foo", "foo" }, pp.Parse<object[]>("foofoo"));
            Throws<ParseException>(() => pp.Parse("foofoofoofoo"));
        }

        [TestMethod]
        public void TestNotParser()
        {
            var pp = 'a'.AsParser().Not;
            Stream stream = "b".ReadStream();
            Assert.IsNull(pp.ParseOn(stream).ActualResult);
            Assert.AreEqual(0, stream.Position);
            Throws<ParseException>(() => pp.Parse("a"));
        }

        [TestMethod]
        public void TestOptionalParser()
        {
            var pp = "foo".AsParser().Flatten.Optional;
            Assert.AreEqual("foo", pp.Parse("foo"));
            Assert.IsNull(pp.Parse("bar"));
        }

        [TestMethod]
        public void TestTokenParser()
        {
            var pp = "foo".AsParser().Token;
            Token token = pp.Parse<Token>("foo");
            Assert.AreEqual(0, token.Start);
            Assert.AreEqual(3, token.Length);
            CollectionAssert.AreEqual(
                new char[] { 'f', 'o', 'o' },
                (object[])token.ParsedValue);
            Assert.AreEqual("foo", token.InputValue);
        }

        [TestMethod]
        public void TestActionParserWithOneArgument()
        {
            var pp = "foo".AsParser().Token
                .Map<Token, string>(token => token.InputValue.ToUpper());
            Assert.AreEqual("FOO", pp.Parse<string>("foo"));
        }

        [TestMethod]
        public void TestActionParserWithMultipleArguments()
        {
            var pp = "foo".AsParser().Token
                .Then("bar".AsParser().Token)
                .Then("baz".AsParser().Token)
                .End
                .Map<Token, Token, Token, string>((t1, t2, t3) =>
                {
                    return string.Format("{0} -> {1} -> {2}", 
                        t1.InputValue, 
                        t2.InputValue,
                        t3.InputValue);
                });
            Assert.AreEqual("foo -> bar -> baz", pp.Parse<string>("foobarbaz"));
        }

        [TestMethod]
        public void TestPredicateParser()
        {
            var pp = Parser.Predicate(chr => "aeiouAEIOU".Contains(chr), "Vowel expected");
            Assert.IsTrue(pp.Matches("a"));
            Assert.IsFalse(pp.Matches("b"));
        }

        [TestMethod]
        public void TestCaseInsensitiveAppliedDirectlyToLiteralParser()
        {
            var pp = "foo!".AsParser().CaseInsensitive.Flatten;
            Assert.AreEqual("Foo!", pp.Parse<string>("Foo!"));
            Assert.AreEqual("FOO!", pp.Parse<string>("FOO!"));
            Assert.AreEqual("foo!", pp.Parse<string>("foo!"));
        }

        [TestMethod]
        public void TestCaseInsensitiveAppliedAtTheTop()
        {
            var pp = ("foo".AsParser().Or("bar".AsParser())).Flatten
                .Then(Parser.Space.Plus)
                .Then(Parser.Digit.Plus.Flatten)
                .Then("!".AsParser().Optional)
                .CaseInsensitive
                .Map<string, object, string, object, Tuple<string, int>>((word, ign1, num, ign2) =>
                {
                    return new Tuple<string, int>(word, int.Parse(num));
                });
            Assert.AreEqual(new Tuple<string, int>("Foo", 4), pp.Parse("Foo 4!"));
            Assert.AreEqual(new Tuple<string, int>("BAR", 443), pp.Parse("BAR     443"));
            Throws<ParseException>(() => pp.Parse("Baz 56"));
        }

        [TestMethod]
        public void TestGreedyRepeatingParserPlus()
        {
            var pp = Parser.Word.PlusGreedy("upper".AsParser().Or("lower".AsParser()).CaseInsensitive).Flatten
                .Then("upper".AsParser().Or("lower".AsParser()).CaseInsensitive.Flatten)
                .End
                .Map<string, string, string>((word, @case) =>
                {
                    if ("lower".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToLower();
                    }
                    else if ("upper".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToUpper();
                    }
                    else
                    {
                        return "WAT";
                    }
                });
            Assert.AreEqual("abcupperlowerupper", pp.Parse("abcupperLowerUPPERlower"));
            Assert.AreEqual("ABC", pp.Parse("abcupper"));
            Throws<ParseException>(() => pp.Parse("upper"));
        }

        [TestMethod]
        public void TestGreedyRepeatingParserStar()
        {
            var pp = Parser.Word.StarGreedy("upper".AsParser().Or("lower".AsParser()).CaseInsensitive).Flatten
                .Then("upper".AsParser().Or("lower".AsParser()).CaseInsensitive.Flatten)
                .End
                .Map<string, string, string>((word, @case) =>
                {
                    if ("lower".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToLower();
                    }
                    else if ("upper".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToUpper();
                    }
                    else
                    {
                        return "WAT";
                    }
                });
            Assert.AreEqual("abcupperlowerupper", pp.Parse("abcupperLowerUPPERlower"));
            Assert.AreEqual("ABC", pp.Parse("abcupper"));
            Assert.AreEqual("", pp.Parse("upper"));
        }

        [TestMethod]
        public void TestLazyRepeatingParserPlus()
        {
            var pp = Parser.Any.PlusLazy("upper".AsParser().Or("lower".AsParser()).CaseInsensitive).Flatten
                .Then("upper".AsParser().Or("lower".AsParser()).CaseInsensitive.Flatten)
                .End
                .Map<string, string, string>((word, @case) =>
                {
                    if ("lower".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToLower();
                    }
                    else if ("upper".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToUpper();
                    }
                    else
                    {
                        return "WAT";
                    }
                });
            Assert.AreEqual(" abc! ", pp.Parse(" abc! LOWER"));
            Assert.AreEqual("ABC", pp.Parse("abcupper"));
            Throws<ParseException>(() => pp.Parse("upper"));
            Throws<ParseException>(() => pp.Parse("abcupperLOWER"));
        }

        [TestMethod]
        public void TestLazyRepeatingParserStar()
        {
            var pp = Parser.Any.StarLazy("upper".AsParser().Or("lower".AsParser()).CaseInsensitive).Flatten
                .Then("upper".AsParser().Or("lower".AsParser()).CaseInsensitive.Flatten)
                .End
                .Map<string, string, string>((word, @case) =>
                {
                    if ("lower".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToLower();
                    }
                    else if ("upper".Equals(@case, StringComparison.OrdinalIgnoreCase))
                    {
                        return word.ToUpper();
                    }
                    else
                    {
                        return "WAT";
                    }
                });
            Assert.AreEqual(" abc! ", pp.Parse(" abc! LOWER"));
            Assert.AreEqual("ABC", pp.Parse("abcupper"));
            Assert.AreEqual("", pp.Parse("upper"));
            Throws<ParseException>(() => pp.Parse("abcupperlower"));
        }

        [TestMethod]
        public void TestParserNegate()
        {
            var pp = Parser.Digit.Negate.Then(Parser.Digit);
            Assert.IsTrue(pp.Matches("a2"));
            Assert.IsTrue(pp.Matches("?2"));
            Assert.IsFalse(pp.Matches("22"));
            Assert.IsFalse(pp.Matches("2a"));
        }

        [TestMethod]
        public void TestParserSeparatedBy()
        {
            var pp = Parser.Digit.SeparatedBy(','.AsParser()).End;
            CollectionAssert.AreEqual(
                new char[] { '3', ',', '4', ',', '5' },
                pp.Parse<object[]>("3,4,5"));
            Throws<ParseException>(() => pp.Parse("3,4,5,"));
        }

        [TestMethod]
        public void TestParserTrim()
        {
            var pp = '+'.AsParser().Trim(Parser.Space);
            Assert.AreEqual('+', pp.Parse("\t\n\r + \r\n\t"));
            Assert.AreEqual('+', pp.Parse("+"));
            Throws<ParseException>(() => pp.Parse(" . "));
        }

        [TestMethod]
        public void TestGreedyRepeatingParserMin()
        {
            var pp = Parser.Any.MinGreedy(3, "END".AsParser().CaseInsensitive).Flatten
                .Then("END".AsParser().CaseInsensitive)
                .End
                .Map(nodes => nodes[0]);
            Assert.AreEqual("abc", pp.Parse("abcEND"));
            Assert.AreEqual("abcdef", pp.Parse("abcdefEND"));
            Assert.AreEqual("end", pp.Parse("endEND"));
            Assert.AreEqual("abcend", pp.Parse("abcendEND"));
            Assert.AreEqual("ENDabc", pp.Parse("ENDabcEND"));
            Throws<ParseException>(() => pp.Parse("abc"));
            Throws<ParseException>(() => pp.Parse("END"));
            Throws<ParseException>(() => pp.Parse("abend"));
        }

        [TestMethod]
        public void TestGreedyRepeatingParserMax()
        {
            var pp = Parser.Any.MaxGreedy(3, "END".AsParser().CaseInsensitive).Flatten
                .Then("END".AsParser().CaseInsensitive)
                .End
                .Map(nodes => nodes[0]);
            Assert.AreEqual("abc", pp.Parse("abcEND"));
            Assert.AreEqual("a", pp.Parse("aend"));
            Assert.AreEqual("", pp.Parse("END"));
            Assert.AreEqual("end", pp.Parse("endEND"));
            Throws<ParseException>(() => pp.Parse("abcendEND"));
            Throws<ParseException>(() => pp.Parse("ENDabcEND"));
            Throws<ParseException>(() => pp.Parse("abc"));
        }

        [TestMethod]
        public void TestParserSeparatedBy2()
        {
            var identifier = '['.AsParser()
                .Then(Parser.Predicate(
                        chr => chr != '[' && chr != ']',
                        "Any except square brackets expected")
                    .Plus.Flatten)
                .Then(']'.AsParser())
                .Map<char, string, char, string>((left, body, right) =>
                {
                    return body;
                })
                .Or(Parser.Word.Or('_'.AsParser()).Plus.Flatten);
            var ws = Parser.Space;
            var pp = ws.Star
                .Then(identifier)
                .Then(ws.Star)
                .Then("ASC".AsParser().CaseInsensitive.Or("DESC".AsParser().CaseInsensitive))
                .Then(ws.Star)
                .SeparatedBy(','.AsParser());

            Assert.IsTrue(pp.Matches("[id] ASC"));
            Assert.IsTrue(pp.Matches("\n	[id] ASC, [model] desc"));
            Assert.IsTrue(pp.Matches("[id]ASC,[model]desc"));
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
