using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser.Utils
{
    public class Stream
    {
        public Stream(string source)
        {
            Source = source;
        }

        public string Source { get; }
        public int Position { get; set; }

        public bool AtEnd
        {
            get { return Position >= Source.Length; }
        }

        public char? Next()
        {
            if (AtEnd) return null;
            Position += 1;
            return Source[Position - 1];
        }

        public char? Peek()
        {
            if (AtEnd) return null;
            return Source[Position];
        }
    }
}
