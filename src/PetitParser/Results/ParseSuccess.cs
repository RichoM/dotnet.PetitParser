using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Results
{
    class ParseSuccess : ParseResult
    {
        private object actualResult;

        public ParseSuccess(object actualResult)
        {
            this.actualResult = actualResult;
        }

        public override object ActualResult { get { return actualResult; } }
        public override bool IsSuccess { get { return true; } }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), actualResult);
        }
    }
}
