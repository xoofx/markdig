using System;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a indented code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.4 Indented code blocks 
    /// </remarks>
    public class CodeBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public CodeBlock()
        {
            NoInline = true;
        }

        private class ParserInternal : BlockParser
        {
            public ParserInternal()
            {
                CanInterruptParagraph = false;
            }

            public override MatchLineResult Match(MatchLineState state)
            {
                var liner = state.Line;
                int position = liner.Start;
                liner.SkipLeadingSpaces3();

                // 4.4 Indented code blocks 
                var c = liner.Current;
                var isTab = CharHelper.IsTab(c);
                var isSpace = CharHelper.IsSpace(c);
                if ((isTab || (isSpace && (liner.Start - position) == 3)) && !liner.IsBlankLine())
                {
                    liner.NextChar();
                    if (state.Block == null)
                    {
                        state.Block = new CodeBlock();
                    }
                    return MatchLineResult.Continue;
                }

                return MatchLineResult.None;
            }
        }
    }
}