using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Renderers.Html;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class HtmlFootnoteRenderer : HtmlObjectRenderer<Document>
    {
        public HtmlFootnoteRenderer()
        {
            BackLinkString = "&#8617;";
        }

        public string BackLinkString { get; set; }

        protected override void Write(HtmlRenderer writer, Document document)
        {
            var footnotes = document.GetData(typeof(FootnoteBlock)) as List<FootnoteBlock>;
            if (footnotes == null)
            {
                return;
            }
            writer.EnsureLine();
            writer.WriteLine("<div class=\"footnotes\">");
            writer.WriteLine("<hr />");
            writer.WriteLine("<ol>");

            footnotes.Sort((left, right) => left.Order.HasValue && right.Order.HasValue ? left.Order.Value.CompareTo(right.Order.Value) : 0);

            for (int i = 0; i < footnotes.Count; i++)
            {
                var footnote = footnotes[i];
                if (!footnote.Order.HasValue)
                {
                    continue;
                }
                writer.WriteLine($"<li id=\"fn:{footnote.Order}\">");

                // Insert the footnote back link
                if (footnote.Children.Count > 0)
                {
                    var leafBlock = footnote.Children[footnote.Children.Count - 1] as LeafBlock;
                    // TODO: What if the block doesn't have any inline. Need to create one?
                    if (leafBlock != null && leafBlock.Inline != null)
                    {
                        leafBlock.Inline.AppendChild(new LinkInline($"#fnref:{footnote.Order.Value}", null).AppendChild(new LiteralInline(HtmlHelper.Unescape(BackLinkString))));
                    }
                }

                writer.WriteChildren(footnote);

                writer.WriteLine("</li>");
            }
            writer.WriteLine("</ol>");
            writer.WriteLine("</div>");
        }
    }
}