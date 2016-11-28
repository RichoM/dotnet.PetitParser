using System;
using PetitParser.Results;
using PetitParser.Utils;
using System.Collections.Generic;
using System.Linq;

namespace PetitParser
{
    internal class GreedyRepeatingParser : Parser
    {
        private int max;
        private Parser parser;
        private Parser limit;
        private int min;

        public GreedyRepeatingParser(Parser parser, int min, int max, Parser limit)
        {
            this.parser = parser;
            this.min = min;
            this.max = max;
            this.limit = limit;
        }

        public override Parser CaseInsensitive
        {
            get { return new GreedyRepeatingParser(parser.CaseInsensitive, min, max, limit); }
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

            List<int> positions = new List<int>();
            positions.Add(stream.Position);

            {
                ParseResult result = null;
                while (elements.Count < max && !(result = parser.ParseOn(stream)).IsFailure)
                {
                    elements.Add(result.ActualResult);
                    positions.Add(stream.Position);
                }
            }

            while (positions.Count > 0)
            {
                stream.Position = positions.Last();
                ParseResult result = limit.ParseOn(stream);
                if (result.IsSuccess)
                {
                    stream.Position = positions.Last();
                    return new ParseSuccess(elements.ToArray());
                }
                if (elements.Count == 0)
                {
                    stream.Position = start;
                    return result;
                }
                elements.RemoveAt(elements.Count - 1);
                positions.RemoveAt(positions.Count - 1);
            }

            stream.Position = start;
            return new ParseFailure(start, "Overflow");
        }
    }
}