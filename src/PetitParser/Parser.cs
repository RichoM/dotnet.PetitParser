using PetitParser.Results;
using PetitParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitParser
{
    public abstract class Parser
    {
        public virtual T Parse<T>(string str)
        {
            return (T)Parse(str);
        }

        public virtual object Parse(string str)
        {
            return ParseOn(str.ReadStream()).ActualResult;
        }

        public abstract ParseResult ParseOn(Stream stream);

        public virtual Parser Then(Parser parser)
        {
            return new SequenceParser(new Parser[] { this, parser });
        }

        public virtual Parser Or(Parser parser)
        {
            return new ChoiceParser(new Parser[] { this, parser });
        }

        public virtual Parser And()
        {
            return new AndParser(this);
        }

        public Parser Flatten()
        {
            return new FlattenParser(this);
        }

        public Parser End()
        {
            return new EndParser(this);
        }

        public Parser Star()
        {
            return new RepeatingParser(this, 0, int.MaxValue);
        }

        public Parser Plus()
        {
            return new RepeatingParser(this, 1, int.MaxValue);
        }

        public Parser Times(int times)
        {
            return new RepeatingParser(this, times, times);
        }

        public Parser Not()
        {
            return new NotParser(this);
        }

        public Parser Optional()
        {
            return new OptionalParser(this);
        }

        public Parser Token()
        {
            return new TokenParser(this);
        }
    }
}
