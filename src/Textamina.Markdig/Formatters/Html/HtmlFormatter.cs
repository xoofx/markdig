using System.IO;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlFormatter : FormatterBase<HtmlFormatter, HtmlWriter>
    {
        private readonly TextWriter writer;

        public HtmlFormatter(TextWriter writer)
        {
            this.writer = writer;
        }
    }
}