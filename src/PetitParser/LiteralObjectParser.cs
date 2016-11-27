using PetitParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class LiteralObjectParser : Parser
    {
        private char literal;

        public LiteralObjectParser(char literal)
        {
            this.literal = literal;
        }

        public override Parser CaseInsensitive()
        {
            char lower = char.ToLowerInvariant(literal);
            return Predicate(chr => lower == char.ToLowerInvariant(chr),
                string.Format("Literal '{0}' expected", literal));
        }

        public override ParseResult ParseOn(Stream stream)
        {
            if (stream.Peek() == literal)
            {
                return new ParseSuccess(stream.Next().Value);
            }
            else
            {
                return new ParseFailure(stream.Position, "Literal '{0}' expected", literal);
            }
        }
    }
}
