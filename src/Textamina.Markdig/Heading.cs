namespace Textamina.Markdig
{
    /// <summary>
    /// Repressents a thematic break.
    /// </summary>
    public class Heading : BlockLeaf
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public Heading(Block parent) : base(DefaultMatcher, parent)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                // 4.2 ATX headings
                // An ATX heading consists of a string of characters, parsed as inline content, 
                // between an opening sequence of 1–6 unescaped # characters and an optional 
                // closing sequence of any number of unescaped # characters. The opening sequence 
                // of # characters must be followed by a space or by the end of line. The optional
                // closing sequence of #s must be preceded by a space and may be followed by spaces
                // only. The opening # character may be indented 0-3 spaces. The raw contents of 
                // the heading are stripped of leading and trailing spaces before being parsed as 
                // inline content. The heading level is equal to the number of # characters in the 
                // opening sequence.
                var c = liner.Current;

                int leadingCount = 0;
                for (; !liner.IsEol && leadingCount <= 6; leadingCount++)
                {
                    if (c != '#' && Charset.IsSpace(c))
                    {
                        break;
                    }

                    c = liner.NextChar();
                }

                // closing # will be handled later, because anyway we have matched 

                // A space is required after leading #
                if (Charset.IsSpace(c))
                {
                    liner.NextChar();
                    return MatchLineState.BreakAndKeepCurrent;
                }

                return MatchLineState.Discard;
            }

            public override Block New(Block parent)
            {
                return new Heading(parent);
            }
        }
    }
}