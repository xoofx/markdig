using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteLinkReferenceDefinition : LinkReferenceDefinition
    {
        public Footnote Footnote { get; set; }
    }
}