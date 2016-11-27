using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Results
{

    class ParseFailure : ParseResult
    {
        public ParseFailure(int position, string message, params object[] args)
        {
            Message = string.Format(message, args);
            Position = position;
        }

        public int Position { get; }
        public string Message { get; }
        public override object ActualResult { get { throw ParseException.FromFailure(this); } }
        public override bool IsFailure { get { return true; } }
        public override string ToString()
        {
            return string.Format("{0} at {1}", Message, Position);
        }
    }
}
