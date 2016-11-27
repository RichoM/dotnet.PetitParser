using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    internal class EndParser : Parser
    {
        private Parser parser;

        public EndParser(Parser parser)
        {
            this.parser = parser;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = parser.ParseOn(stream);
            if (result.IsFailure || stream.AtEnd) return result;
            result = new ParseFailure(stream.Position, "End of input expected");
            stream.Position = start;
            return result;
        }
    }
}