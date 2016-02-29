using System.Globalization;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlListBlockRenderer : HtmlRendererBase<ListBlock>
    {
        protected override void Write(HtmlFormatter visitor, HtmlWriter writer, ListBlock listBlock)
        {
            writer.EnsureLine();
            if (listBlock.IsOrdered)
            {
                writer.Write("<ol");
                if (listBlock.OrderedStart != 1)
                {
                    writer.Write(" start=\"");
                    writer.Write(listBlock.OrderedStart.ToString(CultureInfo.InvariantCulture));
                    writer.Write("\"");
                }
                writer.WriteLine(">");
            }
            else
            {
                writer.WriteLine("<ul>");
            }
            foreach (var item in listBlock.Children)
            {
                var listItem = (ListItemBlock)item;
                var previousImplicit = writer.ImplicitParagraph;
                writer.ImplicitParagraph = !listBlock.IsLoose;
                Write(visitor, writer, listItem, listBlock.IsLoose);
                writer.ImplicitParagraph = previousImplicit;
            }
            writer.WriteLine(listBlock.IsOrdered ? "</ol>" : "</ul>");
        }

        protected void Write(HtmlFormatter visitor, HtmlWriter writer, ListItemBlock listBlockItem, bool isLoose)
        {
            writer.EnsureLine();
            writer.Write("<li>");
            visitor.VisitContainer(writer, listBlockItem);
            writer.WriteLine("</li>");
        }
    }
}