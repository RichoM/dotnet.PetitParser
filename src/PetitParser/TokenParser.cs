using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    internal class TokenParser : Parser
    {
        private Parser parser;

        public TokenParser(Parser parser)
        {
            this.parser = parser;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = parser.ParseOn(stream);
            if (result.IsFailure) return result;
            Token token = new Token(stream.Source, start, stream.Position - start, result.ActualResult);
            return new ParseSuccess(token);
        }
    }
}