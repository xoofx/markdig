


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a paragraph.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class Paragraph : BlockLeaf
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref StringLiner liner, ref Block block)
            {
                liner.SkipLeadingSpaces3();

                // Else it is a continue, we don't break on blank lines
                var isBlankLine = liner.IsBlankLine();

                if (block == null)
                {
                    if (isBlankLine)
                    {
                        return false;
                    }
                }
                else if (isBlankLine)
                {
                    return false;
                }

                if (block == null)
                {
                    block = new Paragraph();
                }

                return true;
            }
        }
    }
}