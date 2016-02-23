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
                var isTab = c.IsTab();
                var isSpace = c.IsSpace();
                var isBlankLine = liner.IsBlankLine();
                var codeBlock = state.Pending as CodeBlock;
                // && !isBlankLine) || (isBlankLine && codeBlock != null && !codeBlock.Lines[codeBlock.Lines.Count - 1].IsBlankLine())
                if ((codeBlock != null && isBlankLine) || (isTab || (isSpace && (liner.Start - position) == 3)))
                //if (((isTab || (isSpace && (liner.Start - position) == 3)) && !isBlankLine))
                {
                    liner.NextChar();
                    if (state.Pending == null)
                    {
                        if (isBlankLine)
                        {
                            return MatchLineResult.None;
                        }

                        state.NewBlocks.Push(new CodeBlock());
                    }
                    return MatchLineResult.Continue;
                }

                return MatchLineResult.None;
            }

            public override void Close(MatchLineState state)
            {
                var codeBlock = state.Pending as CodeBlock;
                if (codeBlock != null)
                {
                    var lines = codeBlock.Lines;
                    // Remove trailing blankline
                    for (int i = lines.Count - 1; i >= 0; i--)
                    {
                        if (lines[i].IsBlankLine())
                        {
                            lines.RemoveAt(i);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
        }
    }
}