using Textamina.Markdig.Helpers;

namespace Textamina.Markdig.Syntax
{
    public class LinkReferenceDefinitionBlock : LeafBlock
    {
        public LinkReferenceDefinitionBlock()
        {
        }

        public LinkReferenceDefinitionBlock(string label, string url, string title)
        {
            Label = label;
            Url = url;
            Title = title;
        }

        public string Label { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public static bool TryParse(StringLineGroup lines, out LinkReferenceDefinitionBlock block)
        {
            block = null;
            string label;
            string url;
            string title;

            if (!LinkHelper.TryParseLinkReferenceDefinition(lines, out label, out url, out title))
            {
                return false;
            }

            block = new LinkReferenceDefinitionBlock(label, url, title);
            return true;
        }
    }
}