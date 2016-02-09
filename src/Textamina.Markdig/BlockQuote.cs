namespace Textamina.Markdig
{
    public class BlockQuote : BlockContainer
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public BlockQuote(Block parent) : base(DefaultMatcher, parent)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                // 5.1 Block quotes 
                // A block quote marker consists of 0-3 spaces of initial indent, plus (a) the character > together with a following space, or (b) a single character > not followed by a space.
                var c = liner.Current;
                if (c != '>')
                {
                    return matchLineState == MatchLineState.Continue
                        ? MatchLineState.Break
                        : MatchLineState.Discard;
                }

                c = liner.NextChar();
                if (Charset.IsSpace(c))
                {
                    liner.NextChar();
                }

                return MatchLineState.Continue;
            }

            public override Block New(Block parent)
            {
                return new BlockQuote(parent);
            }
        }
    }
}