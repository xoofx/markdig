using System;
using System.IO;

namespace Textamina.Markdig.Formatters.Html
{
    public class HtmlWriter
    {
        private readonly TextWriter textWriter;

        public HtmlWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public bool WriteOnlyContent { get; set; }

        public bool ImplicitParagraph { get; set; }

        public HtmlWriter EnsureLine()
        {
            return this;
        }

        public HtmlWriter Write(string content)
        {
            textWriter.Write(content);
            return this;
        }

        public HtmlWriter Write(string content, int offset, int length)
        {
            throw new NotImplementedException();
            //textWriter.Write(content, offset, length);
            return this;
        }

        public HtmlWriter WriteLine()
        {
            textWriter.WriteLine();
            return this;
        }

        public HtmlWriter WriteLine(string content)
        {
            textWriter.WriteLine(content);
            return this;
        }

    }
}