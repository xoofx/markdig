

namespace Textamina.Markdig
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

        public Paragraph(Block parent) : base(DefaultMatcher, parent)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState,
                ref object matchContext)
            {

                var isNotSpaceOrTab = !Charset.IsSpaceOrTab(liner.Current);
                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                if (matchLineState == MatchLineState.None)
                {
                    if (isNotSpaceOrTab || !isBlankLine)
                    {
                        return MatchLineState.Continue;
                    }

                    return MatchLineState.Discard;
                }

                if (isNotSpaceOrTab)
                {
                    return MatchLineState.Continue;
                }

                return isBlankLine ? MatchLineState.Break : MatchLineState.Continue;
            }

            public override Block New(Block parent)
            {
                return new Paragraph(parent);
            }
        }
    }
}