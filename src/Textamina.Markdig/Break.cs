namespace Textamina.Markdig
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class Break : BlockLeaf
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public Break(Block parent) : base(DefaultMatcher, parent)
        {
            IsOpen = false;
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                // 4.1 Thematic breaks 
                // A line consisting of 0-3 spaces of indentation, followed by a sequence of three or more matching -, _, or * characters, each followed optionally by any number of spaces
                var c = liner.Current;

                int count = 0;
                var matchChar = (char)0;
                while (!liner.IsEol)
                {
                    if (count == 0 && (c == '-' || c == '_' || c == '*'))
                    {
                        matchChar = c;
                        count++;
                    }
                    else if (c != matchChar && !Charset.IsSpace(c))
                    {
                        return MatchLineState.Discard;
                    }
                    c = liner.NextChar();
                }

                return MatchLineState.BreakAndKeepCurrent;
            }

            public override Block New(Block parent)
            {
                return new Break(parent);
            }
        }
    }
}