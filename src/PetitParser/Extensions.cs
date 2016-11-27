using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser
{
    public static class Extensions
    {
        public static Parser AsParser(this char self)
        {
            return new LiteralObjectParser(self);
        }

        public static Parser AsParser(this string self)
        {
            return new SequenceParser(self.Select(AsParser));
        }
    }
}
