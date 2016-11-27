using System;
using PetitParser.Results;
using PetitParser.Utils;
using System.Collections.Generic;
using System.Linq;

namespace PetitParser
{
    class SequenceParser : Parser
    {
        private List<Parser> parsers = new List<Parser>();

        public SequenceParser(IEnumerable<Parser> parsers)
        {
            this.parsers.AddRange(parsers);
        }

        public override Parser Then(Parser parser)
        {
            parsers.Add(parser);
            return this;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            List<object> elements = new List<object>();
            foreach (Parser pp in parsers)
            {
                ParseResult result = pp.ParseOn(stream);
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
            return new ParseSuccess(elements.ToArray());
        }
    }
}