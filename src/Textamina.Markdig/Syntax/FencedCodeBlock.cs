


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : BlockLeaf
    {
        public static readonly BlockBuilder Builder = new BuilderInternal();

        private int fencedCharCount;

        private char fencedChar;

        private class BuilderInternal : BlockBuilder
        {
            public override bool Match(ref StringLiner liner, ref Block block)
            {
                liner.SkipLeadingSpaces3();

                var fenced = block as FencedCodeBlock;
                if (fenced != null)
                {
                    var c = liner.Current;

                    int count = fenced.fencedCharCount;
                    var matchChar = fenced.fencedChar;
                    while (!liner.IsEol)
                    {
                        if (c != matchChar || count < 0)
                        {
                            if (count > 0)
                            {
                                break;
                            }

                            return false;
                        }
                        c = liner.NextChar();
                        count--;
                    }

                    // TODO: It is unclear how to handle this correctly
                    // Break only if Eof
                    return true;
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
                            if (count < 3)
                            {
                                break;
                            }

                            // Store the number of matched string into the context
                            block = new FencedCodeBlock()
                            {
                                fencedChar = matchChar,
                                fencedCharCount = count
                            };
                            return true;
                        }
                        c = liner.NextChar();
                        count++;
                    }

                    return false;
                }
            }
        }
    }
}