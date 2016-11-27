using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Utils
{
    public static class StreamExtensions
    {
        public static Stream ReadStream(this string self)
        {
            return new Stream(self);
        }
    }
}
