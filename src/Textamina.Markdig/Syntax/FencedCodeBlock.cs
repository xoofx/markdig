


using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : CodeBlock
    {
        public new static readonly BlockParser Parser = new ParserInternal();

        public FencedCodeBlock(BlockParser parser) : base(parser)
        {
        }

        public string Language { get; set; }

        public string Arguments { get; set; }

        private int fencedCharCount;

        private char fencedChar;

        private int indentCount;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                int count;
                char matchChar;
                char c = state.CurrentChar;
                int offset = 0;

                var currentFenced = state.Pending as FencedCodeBlock;
                if (currentFenced != null)
                {
                    count = currentFenced.fencedCharCount;
                    matchChar = currentFenced.fencedChar;
                    while (c == matchChar)
                    {
                        offset++;
                        c = state.Line.PeekChar(offset);
                    }

                    if (offset >= count)
                    {
                        state.Line.TrimEnd(true);
                        if (state.CurrentChar == matchChar)
                        {
                            return MatchLineResult.LastDiscard;
                        }
                    }

                    // TODO: It is unclear how to handle this correctly
                    // Break only if Eof
                    return MatchLineResult.Continue;
                }

                // Else if the we have an indent, it is not valid
                if (state.IsCodeIndent)
                {
                    return MatchLineResult.None;
                }

                count = 0;
                matchChar = (char) 0;
                while (c != '\0')
                {
                    if (count == 0 && (c == '`' || c == '~'))
                    {
                        matchChar = c;
                    }
                    else if (c != matchChar)
                    {
                        break;
                    }
                    count++;
                    c = state.PeekChar(count);
                }

                if (count >= 3)
                {
                    return MatchLineResult.None;
                }

                // TODO: We need to count the number of leading space to remove them on each line
                var column = state.Column;

                // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
                while (c.IsSpaceOrTab())
                {
                    offset++;
                    c = state.PeekChar(offset);
                }
                var start = state.Start + count + offset;

                string infoString;
                string argString = null;

                // An info string cannot contain any backsticks
                int firstSpace = -1;
                for (int i = start; i <= state.EndOffset; i++)
                {
                    c = state.Line[i];
                    if (c == '`')
                    {
                        return MatchLineResult.None;
                    }

                    if (firstSpace < 0 && c.IsSpaceOrTab())
                    {
                        firstSpace = i;
                    }
                }

                if (firstSpace > 0)
                {
                    infoString = state.Line.Text.Substring(start, firstSpace - start);

                    // Skip any spaces after info string
                    firstSpace++;
                    while (true)
                    {
                        c = state.Line[firstSpace];
                        if (c.IsSpaceOrTab())
                        {
                            firstSpace++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    argString = state.Line.Text.Substring(firstSpace, state.Line.End - firstSpace + 1);
                }
                else
                {
                    infoString = state.Line.Text.Substring(start, state.EndOffset - start + 1);
                }

                // Store the number of matched string into the context
                state.NewBlocks.Push(new FencedCodeBlock(this)
                {
                    Column = column,
                    fencedChar = matchChar,
                    fencedCharCount = count,
                    indentCount = state.Indent,
                    Language = HtmlHelper.Unescape(infoString),
                    Arguments = HtmlHelper.Unescape(argString),
                });

                // Discard the current line
                return MatchLineResult.ContinueDiscard;
            }

            public override void Close(BlockParserState state)
            {
                var fenced = ((FencedCodeBlock) state.Pending);
                var lines = fenced.Lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    // Fences can be indented. If the opening fence is indented, 
                    // content lines will have equivalent opening indentation removed, if present:
                    for (int j = 0; j < fenced.indentCount; j++)
                    {
                        var start = lines.Slices[i].Start;
                        if (start < lines.Slices[i].End && lines.Slices[i][start].IsSpace())
                        {
                            lines.Slices[i].Start++;
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