using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Syntax
{
    public class LinkReferenceDefinition : LeafBlock
    {
        public delegate Inline CreateLinkInlineDelegate(InlineParserState inlineState, LinkReferenceDefinition linkRef, Inline child = null);

        public LinkReferenceDefinition() : base(null)
        {
            IsOpen = false;
        }

        public LinkReferenceDefinition(string label, string url, string title) : this()
        {
            Label = label;
            Url = url;
            Title = title;
        }

        public string Label { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public CreateLinkInlineDelegate CreateLinkInline;

        public static bool TryParse<T>(ref T text, out LinkReferenceDefinition block) where T : ICharIterator
        {
            block = null;
            string label;
            string url;
            string title;

            if (!LinkHelper.TryParseLinkReferenceDefinition(ref text, out label, out url, out title))
            {
                return false;
            }

            block = new LinkReferenceDefinition(label, url, title);
            return true;
        }
    }
}