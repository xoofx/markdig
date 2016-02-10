


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class Paragraph : BlockLeaf
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public Paragraph() : base(DefaultMatcher)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState,
                ref object matchContext)
            {
                liner.SkipLeadingSpaces3();

                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                if (matchLineState == MatchLineState.None)
                {
                    if (isBlankLine)
                    {
                        return MatchLineState.Discard;
                    }
                }
                else if (isBlankLine)
                {
                    return MatchLineState.Break;
                }

                return MatchLineState.Continue;
            }

            public override Block New()
            {
                return new Paragraph();
            }
        }
    }
}