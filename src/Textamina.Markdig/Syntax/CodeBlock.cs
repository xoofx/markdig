using System;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
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

        public CodeBlock() : base(DefaultMatcher)
        {
        }

        private class MatcherInternal : BlockMatcher
        {
            public override MatchLineState Match(ref StringLiner liner, MatchLineState matchLineState, ref object matchContext)
            {
                int position = liner.Column;
                liner.SkipLeadingSpaces3();

                // 4.4 Indented code blocks 
                var c = liner.Current;
                var isTab = Utility.IsTab(c);
                var isSpace = Utility.IsSpace(c);
                if ((isTab || (isSpace && (liner.Column - position) == 3)) && !liner.IsBlankLine())
                {
                    liner.NextChar();
                    return MatchLineState.Continue;
                }

                return matchLineState == MatchLineState.Continue
                    ? MatchLineState.Break
                    : MatchLineState.Discard;
            }

            public override Block New()
            {
                return new CodeBlock();
            }
        }
    }
}