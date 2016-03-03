using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteLink : Inline
    {
        public bool IsBackLink { get; set; }

        public int Index { get; set; }

        public Footnote Footnote { get; set; }
    }
}