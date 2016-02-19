using System.Collections.Generic;
using System.Linq;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class EmphasisDelimiterInline : DelimiterInline
    {
        public EmphasisDelimiterInline(InlineParser parser) : base(parser)
        {
            FinalEmphasisInlines = new List<EmphasisInline>();
        }

        /// <summary>
        /// The delimiter character found.
        /// </summary>
        public char DelimiterChar { get; set; }

        /// <summary>
        /// The number of delimiter characters found for this delimiter.
        /// </summary>
        public int DelimiterCount { get; set; }

        public List<EmphasisInline> FinalEmphasisInlines { get; set; }

        public override string ToString()
        {
            var str = string.Join(", ", FinalEmphasisInlines);
            return $"Emphasis: {DelimiterChar} [{str}]";
        }
    }
}