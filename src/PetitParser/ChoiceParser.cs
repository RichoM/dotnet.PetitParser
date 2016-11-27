using System;
using System.Collections.Generic;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class ChoiceParser : Parser
    {
        private List<Parser> parsers = new List<Parser>();

        public ChoiceParser(Parser[] parsers)
        {
            this.parsers.AddRange(parsers);
        }

        public override Parser Or(Parser parser)
        {
            parsers.Add(parser);
            return this;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            ParseResult fail = new ParseFailure(0, "");
            foreach (Parser pp in parsers)
            {
                ParseResult result = pp.ParseOn(stream);
                if (result.IsSuccess) return result;
                fail = result;
            }
            return fail;
        }
    }
}