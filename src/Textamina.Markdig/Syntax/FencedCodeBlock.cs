


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

        private bool hasFencedEnd;

        private int indentCount;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                var liner = state.Line;

                var fenced = state.Pending as FencedCodeBlock;
                if (fenced != null)
                {
                    var saveLiner = liner.Save();
                    liner.SkipLeadingSpaces3();

                    var c = liner.Current;
                    int count = fenced.fencedCharCount;
                    var matchChar = fenced.fencedChar;
                    while (!liner.IsEol)
                    {
                        if (c != matchChar)
                        {
                            break;
                        }
                        c = liner.NextChar();
                        count--;
                    }

                    if (count <= 0)
                    {
                        int endPosition = liner.Start;
                        liner.TrimEnd(true);
                        if (liner.IsEol || liner.End == endPosition)
                        {
                            fenced.hasFencedEnd = true;
                            return MatchLineResult.Last;
                        }
                    }

                    liner.Restore(ref saveLiner);

                    // TODO: It is unclear how to handle this correctly
                    // Break only if Eof
                    return MatchLineResult.Continue;
                }
                else
                {
                    // TODO: We need to count the number of leading space to remove them on each line
                    var start = liner.Start;
                    liner.SkipLeadingSpaces3();
                    var indentCount = liner.Start - start;
                    var c = liner.Current;

                    int count = 0;
                    var matchChar = (char)0;
                    while (!liner.IsEol)
                    {
                        if (count == 0 && (c == '`' || c == '~'))
                        {
                            matchChar = c;
                        }
                        else if (c != matchChar)
                        {
                            break;
                        }
                        c = liner.NextChar();
                        count++;
                    }

                    if (count >= 3)
                    {
                        // specs spaces: Is space and tabs? or only spaces? Use space and tab for this case
                        liner.TrimStart(true);
                        liner.TrimEnd(true);

                        string infoString = null;
                        string argString = null;

                        // An info string cannot contain any backsticks
                        int firstSpace = -1;
                        for (int i = liner.Start; i <= liner.End; i++)
                        {
                            c = liner[i];
                            if (c == '`')
                            {
                                return MatchLineResult.None;
                            }
                            else if (firstSpace < 0 && c.IsSpaceOrTab())
                            {
                                firstSpace = i;
                            }
                        }

                        if (firstSpace > 0)
                        {
                            infoString = liner.Text.Substring(liner.Start, firstSpace - liner.Start);

                            // Skip any spaces after info string
                            firstSpace++;
                            while (true)
                            {
                                c = liner[firstSpace];
                                if (c.IsSpaceOrTab())
                                {
                                    firstSpace++;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            argString = liner.Text.Substring(firstSpace, liner.End - firstSpace + 1);
                        }
                        else
                        {
                            infoString = liner.ToString();
                        }

                        // Store the number of matched string into the context
                        state.NewBlocks.Push(new FencedCodeBlock(this)
                        {
                            fencedChar = matchChar,
                            fencedCharCount = count,
                            indentCount = indentCount,
                            Language = HtmlHelper.Unescape(infoString),
                            Arguments = HtmlHelper.Unescape(argString),
                        });
                        return MatchLineResult.Continue;
                    }

                    return MatchLineResult.None;
                }
            }

            public override void Close(BlockParserState state)
            {
                var fenced = (FencedCodeBlock) state.Pending;
                fenced.Lines.RemoveAt(0);
                if (fenced.hasFencedEnd)
                {
                    fenced.Lines.RemoveAt(fenced.Lines.Count - 1);
                }
                for (int i = 0; i < fenced.Lines.Count; i++)
                {
                    var line = fenced.Lines[i];

                    // Fences can be indented. If the opening fence is indented, 
                    // content lines will have equivalent opening indentation removed, if present:
                    for (int j = 0; j < fenced.indentCount; j++)
                    {
                        if (line[line.Start].IsSpace())
                        {
                            line.Start++;
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