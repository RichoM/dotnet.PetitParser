using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class PredicateParser : Parser
    {
        private Func<char, bool> predicate;
        private string message;

        public PredicateParser(Func<char, bool> predicate, string message)
        {
            this.predicate = predicate;
            this.message = message;
        }

        public override ParseResult ParseOn(Stream stream)
        {
            if (!stream.AtEnd && predicate(stream.Peek().Value)) return new ParseSuccess(stream.Next());
            return new ParseFailure(stream.Position, message);
        }
    }
}
