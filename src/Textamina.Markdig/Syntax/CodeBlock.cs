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
        public static readonly BlockParser Parser = new ParserInternal();


        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref StringLiner liner, ref Block block)
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
                    if (block == null)
                    {
                        block = new CodeBlock();
                    }
                    return MatchLineResult.Continue;
                }

                return MatchLineResult.None;
            }
        }
    }
}