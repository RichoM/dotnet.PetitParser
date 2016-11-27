using System;
using PetitParser.Results;
using PetitParser.Utils;
using System.Collections.Generic;

namespace PetitParser
{
    internal class RepeatingParser : Parser
    {
        private int max;
        private Parser parser;
        private int min;

        public RepeatingParser(Parser parser, int min, int max)
        {
            this.parser = parser;
            this.min = min;
            this.max = max;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            List<object> elements = new List<object>();

            while (elements.Count < min)
            {
                ParseResult result = parser.ParseOn(stream);
                if (result.IsSuccess)
                {
                    elements.Add(result.ActualResult);
                }
                else
                {
                    stream.Position = start;
                    return result;
                }
            }

            while (elements.Count < max)
            {
                ParseResult result = parser.ParseOn(stream);
                if (result.IsSuccess)
                {
                    elements.Add(result.ActualResult);
                }
                else
                {
                    return new ParseSuccess(elements.ToArray());
                }
            }
            return new ParseSuccess(elements.ToArray());
        }
    }
}