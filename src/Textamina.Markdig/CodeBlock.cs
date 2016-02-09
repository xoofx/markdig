namespace Textamina.Markdig
{
    /// <summary>
    /// Repressents a indented code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.4 Indented code blocks 
    /// </remarks>
    public class CodeBlock : BlockLeaf
    {
        public static readonly BlockMatcher DefaultMatcher = new MatcherInternal();

        public CodeBlock(Block parent) : base(DefaultMatcher, parent)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                // 4.4 Indented code blocks 
                var c = liner.Current;
                if (liner.Column == 4 && Charset.IsSpace(c))
                {
                    liner.NextChar();
                    return MatchLineState.Continue;
                }

                return matchLineState == MatchLineState.Continue
                    ? MatchLineState.Break
                    : MatchLineState.Discard;
            }

            public override Block New(Block parent)
            {
                return new Heading(parent);
            }
        }
    }
}