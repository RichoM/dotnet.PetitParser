using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Results
{
    public class ParseException : Exception
    {
        internal static ParseException FromFailure(ParseFailure failure)
        {
            return new ParseException(failure.ToString(), failure.Position);
        }

        public ParseException(string message, int position) : base(message)
        {
            Data["Position"] = position;
        }
        
        public int Position { get { return (int)Data["Position"]; } }
    }
}
