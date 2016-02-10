


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : BlockLeaf
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public FencedCodeBlock() : base(DefaultMatcher)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                liner.SkipLeadingSpaces3();

                if (matchLineState == MatchLineState.Continue)
                {
                    var match = (MatchContext) matchContext;

                    var c = liner.Current;

                    int count = match.Count;
                    var matchChar = match.FencedChar;
                    while (!liner.IsEol)
                    {
                        if (c != matchChar || count < 0)
                        {
                            if (count > 0)
                            {
                                break;
                            }

                            return MatchLineState.Break;
                        }
                        c = liner.NextChar();
                        count--;
                    }

                    // TODO: It is unclear how to handle this correctly
                    return MatchLineState.BreakAndKeepOnlyIfEof;
                }
                else
                {
                    var c = liner.Current;

                    int count = 0;
                    var matchChar = (char)0;
                    while (!liner.IsEol)
                    {
                        if (count == 0 && (c == '-' || c == '_' || c == '*'))
                        {
                            matchChar = c;
                        }
                        else if (c != matchChar)
                        {
                            if (count < 3)
                            {
                                break;
                            }

                            // Store the number of matched string into the context
                            matchContext = new MatchContext(matchChar, count);
                            return MatchLineState.Continue;
                        }
                        c = liner.NextChar();
                        count++;
                    }

                    return MatchLineState.Discard;
                }
            }

            public override Block New()
            {
                return new FencedCodeBlock();
            }

            private class MatchContext
            {
                public MatchContext(char fencedChar, int count)
                {
                    FencedChar = fencedChar;
                    Count = count;
                }

                public char FencedChar;

                public int Count;
            }
        }
    }
}