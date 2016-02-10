using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class BlockQuote : BlockContainer
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public BlockQuote() : base(DefaultMatcher)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public MatcherInternal()
            {
                IsContainer = true;
            }

            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                liner.SkipLeadingSpaces3();

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
                if (Utility.IsSpace(c))
                {
                    liner.NextChar();
                }

                return MatchLineState.Continue;
            }

            public override Block New()
            {
                return new BlockQuote();
            }
        }
    }
}