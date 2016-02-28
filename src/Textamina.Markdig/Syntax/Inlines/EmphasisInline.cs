using System.Collections.Generic;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Parsers.Inlines;

namespace Textamina.Markdig.Syntax.Inlines
{
    public class EmphasisInline : ContainerInline
    {
        public char DelimiterChar { get; set; }

        public bool Strong { get; set; }

        public override string ToString()
        {
            return Strong ? "<strong>" : "<em>";
        }
    }
}