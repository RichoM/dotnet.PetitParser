using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Results
{
    public abstract class ParseResult
    {
        public abstract object ActualResult { get; }

        public virtual bool IsSuccess { get { return false; } }
        public virtual bool IsFailure { get { return false; } }
    }
}
