using System;
using PetitParser.Results;
using PetitParser.Utils;
using System.Collections.Generic;

namespace PetitParser
{
    class LazyRepeatingParser : Parser
    {
        private Parser parser;
        private int min;
        private int max;
        private Parser limit;

        public LazyRepeatingParser(Parser parser, int min, int max, Parser limit)
        {
            this.parser = parser;
            this.min = min;
            this.max = max;
            this.limit = limit;
        }

        public override Parser CaseInsensitive
        {
            get { return new LazyRepeatingParser(parser.CaseInsensitive, min, max, limit.CaseInsensitive); }
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            List<object> elements = new List<object>();
            while (elements.Count < min)
            {
                ParseResult result = parser.ParseOn(stream);
                if (result.IsFailure)
                {
                    stream.Position = start;
                    return result;
                }
                elements.Add(result.ActualResult);
            }
            while (!MatchesLimitOn(stream))
            {
                if (elements.Count >= max)
                {
                    stream.Position = start;
                    return new ParseFailure(start, "overflow");
                }
                ParseResult result = parser.ParseOn(stream);
                if (result.IsFailure)
                {
                    stream.Position = start;
                    return result;
                }
                elements.Add(result.ActualResult);
            }
            return new ParseSuccess(elements.ToArray());
        }

        private bool MatchesLimitOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = limit.ParseOn(stream);
            stream.Position = start;
            return result.IsSuccess;
        }
    }
}