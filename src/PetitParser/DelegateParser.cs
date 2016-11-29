using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetitParser.Results;
using PetitParser.Utils;

namespace PetitParser
{
    class DelegateParser : Parser
    {
        private Parser parser;
        private bool caseInsensitive = false;

        public void Define(Parser actual)
        {
            parser = caseInsensitive ? actual.CaseInsensitive : actual;
        }

        public override Parser CaseInsensitive
        {
            get
            {
                caseInsensitive = true;
                return this;
            }
        }

        public override ParseResult ParseOn(Stream stream)
        {
            return parser.ParseOn(stream);
        }
    }
}
