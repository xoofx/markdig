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
    public class CodeBlock : LeafBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();


        private class ParserInternal : BlockParser
        {
            public ParserInternal()
            {
                CanInterruptParagraph = false;
            }

            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Line;
                int position = liner.Start;
                liner.SkipLeadingSpaces3();

                // 4.4 Indented code blocks 
                var c = liner.Current;
                var isTab = Utility.IsTab(c);
                var isSpace = Utility.IsSpace(c);
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