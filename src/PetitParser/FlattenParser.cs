using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class FlattenParser : Parser
    {
        private Parser parser;

        public FlattenParser(Parser parser)
        {
            this.parser = parser;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = parser.ParseOn(stream);
            if (result.IsFailure) return result;
            return new ParseSuccess(stream.Source.Substring(start, stream.Position - start));
        }
    }
}