using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class EmphasisDelimiterInline : DelimiterInline
    {
        public EmphasisDelimiterInline(InlineParser parser) : base(parser)
        {
        }

        /// <summary>
        /// The delimiter character found.
        /// </summary>
        public char DelimiterChar { get; set; }

        /// <summary>
        /// The number of delimiter characters found for this delimiter.
        /// </summary>
        public int DelimiterCount { get; set; }
    }
}