using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class AndParser : Parser
    {
        private Parser parser;

        public AndParser(Parser parser)
        {
            this.parser = parser;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = parser.ParseOn(stream);
            stream.Position = start;
            return result;
        }
    }
}