using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    public class NotParser : Parser
    {
        private Parser parser;

        public NotParser(Parser parser)
        {
            this.parser = parser;
        }

        public override Parser CaseInsensitive()
        {
            return new NotParser(parser.CaseInsensitive());
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            ParseResult result = parser.ParseOn(stream);
            stream.Position = start;
            if (result.IsSuccess) return new ParseFailure(stream.Position, "");
            return new ParseSuccess(null);
        }
    }
}