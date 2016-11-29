using PetitParser.Results;
using PetitParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser
{
    public abstract class Parser
    {
        public static Parser Predicate(Func<char, bool> predicate, string message)
        {
            return new PredicateParser(predicate, message);
        }

        public static Parser Any
        {
            get { return Predicate(chr => true, "Input expected"); }
        }

        public static Parser Digit
        {
            get { return Predicate(chr => char.IsDigit(chr), "Digit expected"); }
        }

        public static Parser Letter
        {
            get { return Predicate(chr => char.IsLetter(chr), "Letter expected"); }
        }

        public static Parser Word
        {
            get { return Predicate(chr => char.IsLetterOrDigit(chr), "Letter or digit expected"); }
        }

        public static Parser Space
        {
            get { return Predicate(chr => char.IsWhiteSpace(chr), "White space expected"); }
        }

        public virtual T Parse<T>(string str)
        {
            return (T)Parse(str);
        }

        public virtual object Parse(string str)
        {
            return ParseOn(str.ReadStream()).ActualResult;
        }

        public abstract ParseResult ParseOn(Stream stream);

        public bool Matches(string str)
        {
            return ParseOn(str.ReadStream()).IsSuccess;
        }

        public virtual Parser Then(Parser parser)
        {
            return new SequenceParser(new Parser[] { this, parser });
        }

        public virtual Parser Or(Parser parser)
        {
            return new ChoiceParser(new Parser[] { this, parser });
        }

        public Parser And
        {
            get { return new AndParser(this); }
        }

        public Parser Flatten
        {
            get { return new FlattenParser(this); }
        }

        public Parser End
        {
            get { return new EndParser(this); }
        }

        public Parser Star
        {
            get { return new RepeatingParser(this, 0, int.MaxValue); }
        }

        public Parser Plus
        {
            get { return new RepeatingParser(this, 1, int.MaxValue); }
        }

        public Parser Times(int times)
        {
            return new RepeatingParser(this, times, times);
        }

        public Parser Min(int min)
        {
            return new RepeatingParser(this, min, int.MaxValue);
        }

        public Parser Max(int max)
        {
            return new RepeatingParser(this, 0, max);
        }

        public Parser StarGreedy(Parser limit)
        {
            return new GreedyRepeatingParser(this, 0, int.MaxValue, limit);
        }

        public Parser PlusGreedy(Parser limit)
        {
            return new GreedyRepeatingParser(this, 1, int.MaxValue, limit);
        }

        public Parser StarLazy(Parser limit)
        {
            return new LazyRepeatingParser(this, 0, int.MaxValue, limit);
        }

        public Parser PlusLazy(Parser limit)
        {
            return new LazyRepeatingParser(this, 1, int.MaxValue, limit);
        }

        public Parser MinGreedy(int min, Parser limit)
        {
            return new GreedyRepeatingParser(this, min, int.MaxValue, limit);
        }

        public Parser MaxGreedy(int max, Parser limit)
        {
            return new GreedyRepeatingParser(this, 0, max, limit);
        }

        public Parser MinLazy(int min, Parser limit)
        {
            return new LazyRepeatingParser(this, min, int.MaxValue, limit);
        }

        public Parser MaxLazy(int max, Parser limit)
        {
            return new LazyRepeatingParser(this, 0, max, limit);
        }

        public Parser Not
        {
            get { return new NotParser(this); }
        }

        public Parser Optional
        {
            get { return new OptionalParser(this); }
        }

        public Parser Token
        {
            get { return new TokenParser(this); }
        }

        public virtual Parser CaseInsensitive
        {
            get { return this; }
        }

        public Parser Negate
        {
            get { return Not.Then(Any).Map(elements => elements[1]); }
        }

        public Parser SeparatedBy(Parser parser)
        {
            return Then(parser.Then(this).Star)
                .Map<object, object[], object[]>((first, second) =>
                {
                    object[] result = new object[2 * second.Length + 1];
                    result[0] = first;
                    int index = 0;
                    foreach (object[] pair in second)
                    {
                        int start = 2 * index++;
                        result[start + 1] = pair[0];
                        result[start + 2] = pair[1];
                    }
                    return result;
                });
        }

        public Parser Trim(Parser trimmer)
        {
            return new TrimmingParser(this, trimmer);
        }

        public Parser Map<TArg, TResult>(Func<TArg, TResult> function)
        {
            return new ActionParser<TResult>(this, (result) =>
            {
                return function((TArg)result);
            });
        }

        public Parser Map<TResult>(Func<object[], TResult> function)
        {
            return Map<object[], TResult>((result) => function(result));
        }

        #region Autogenerated
        /*
         * INFO(Richo): The following methods where autogenerated with a Smalltalk script.
         * 
            Transcript clear.
            str := 'public Parser Map<{1}, TResult>(Func<{1}, TResult> function)
            \{
	            return Map((elements) =>
	            \{
		            return function({2});
                  \});
            \}'.
            2 to: 10 do: [:i || t1 t2 |
	            t1 := String streamContents: [:stream |
		            (1 to: i) do: [:n | stream nextPut: $T; nextPutAll:  n asString]
			            separatedBy: [stream nextPutAll: ', ']].
	            t2 := String streamContents: [:stream |
		            (1 to: i) do: [:n | stream cr; tab; tab; tab; 
				            nextPutAll: ('(T{1})elements[{2}]' format: { n . n - 1})]
			            separatedBy: [stream nextPut: $,]].
	            Transcript show: (str format: { t1. t2 });
		            cr; cr]
         */
        public Parser Map<T1, T2, TResult>(Func<T1, T2, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1]);
            });
        }

        public Parser Map<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2]);
            });
        }

        public Parser Map<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4],
                    (T6)elements[5]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4],
                    (T6)elements[5],
                    (T7)elements[6]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4],
                    (T6)elements[5],
                    (T7)elements[6],
                    (T8)elements[7]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4],
                    (T6)elements[5],
                    (T7)elements[6],
                    (T8)elements[7],
                    (T9)elements[8]);
            });
        }

        public Parser Map<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> function)
        {
            return Map((elements) =>
            {
                return function(
                    (T1)elements[0],
                    (T2)elements[1],
                    (T3)elements[2],
                    (T4)elements[3],
                    (T5)elements[4],
                    (T6)elements[5],
                    (T7)elements[6],
                    (T8)elements[7],
                    (T9)elements[8],
                    (T10)elements[9]);
            });
        }
        #endregion
    }
}
