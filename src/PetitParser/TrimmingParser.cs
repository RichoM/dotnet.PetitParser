using System;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class TrimmingParser : Parser
    {
        private Parser parser;
        private Parser trimmer;

        public TrimmingParser(Parser parser, Parser trimmer)
        {
            this.parser = parser;
            this.trimmer = trimmer;
        }

        public override Parser CaseInsensitive
        {
            get { return new TrimmingParser(parser.CaseInsensitive, trimmer.CaseInsensitive); }
        }

        public override ParseResult ParseOn(Stream stream)
        {
            int start = stream.Position;
            while (trimmer.ParseOn(stream).IsSuccess) ;
            ParseResult result = parser.ParseOn(stream);
            if (result.IsFailure)
            {
                stream.Position = start;
                return result;
            }
            while (trimmer.ParseOn(stream).IsSuccess) ;
            return result;
        }
    }
}