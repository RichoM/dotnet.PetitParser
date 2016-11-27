using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class OptionalParser : Parser
    {
        private Parser parser;

        public OptionalParser(Parser parser)
        {
            this.parser = parser;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            ParseResult result = parser.ParseOn(stream);
            if (result.IsSuccess) return result;
            return new ParseSuccess(null);
        }
    }
}