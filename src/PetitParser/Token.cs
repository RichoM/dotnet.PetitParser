namespace PetitParser
{
    public class Token
    {
        public Token(string source, int start, int length, object value)
        {
            Source = source;
            Start = start;
            Length = length;
            ParsedValue = value;
        }

        public string Source { get; }
        public int Start { get; }
        public int Length { get; }
        public object ParsedValue { get; }
        public int Stop { get { return Start + Length; } }
        public string InputValue { get { return Source.Substring(Start, Length); } }
        
    }
}