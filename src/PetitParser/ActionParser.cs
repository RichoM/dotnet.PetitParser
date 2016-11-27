using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class ActionParser<TResult> : Parser
    {
        private Func<object, TResult> function;
        private Parser parser;

        public ActionParser(Parser parser, Func<object, TResult> function)
        {
            this.parser = parser;
            this.function = function;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            ParseResult result = parser.ParseOn(stream);
            if (result.IsFailure) return result;
            return new ParseSuccess(function(result.ActualResult));
        }
    }
}