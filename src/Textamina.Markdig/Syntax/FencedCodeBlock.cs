


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

        private int fencedCharCount;

        private char fencedChar;

        private bool hasFencedEnd;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Liner;
                liner.SkipLeadingSpaces3();

                var fenced = state.Block as FencedCodeBlock;
                if (fenced != null)
                {
                    var c = liner.Current;

                    int count = fenced.fencedCharCount;
                    var matchChar = fenced.fencedChar;
                    while (!liner.IsEol)
                    {
                        if (c != matchChar || count < 0)
                        {
                            break;
                        }
                        c = liner.NextChar();
                        count--;
                    }



                    if (count <= 0)
                    {
                        fenced.hasFencedEnd = true;
                        return MatchLineResult.Last;
                    }

                    // TODO: It is unclear how to handle this correctly
                    // Break only if Eof
                    return MatchLineResult.Continue;
                }
                else
                {
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
                        // Store the number of matched string into the context
                        state.Block = new FencedCodeBlock()
                        {
                            fencedChar = matchChar,
                            fencedCharCount = count
                        };
                        return MatchLineResult.Continue;
                    }

                    return MatchLineResult.None;
                }
            }

            public override void Close(Block block)
            {
                var fenced = (FencedCodeBlock) block;
                fenced.Inline.RemoveAt(0);
                if (fenced.hasFencedEnd)
                {
                    fenced.Inline.RemoveAt(fenced.Inline.Count - 1);
                }
            }
        }
    }
}