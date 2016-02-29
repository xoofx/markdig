using System;
using System.IO;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig
{
    public class Markdown
    {
        public static string ConvertToHtml(string markdown)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var reader = new StringReader(markdown);
            var writer = new StringWriter();
            ConvertToHtml(reader, writer);
            return writer.ToString();
        }

        public static string ConvertToHtml(StringReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var writer = new StringWriter();
            ConvertToHtml(reader, writer);
            return writer.ToString();
        }

        public static void ConvertToHtml(TextReader reader, TextWriter writer, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            pipeline = pipeline ?? new MarkdownPipeline();
            if (!(pipeline.Renderer is HtmlMarkdownRenderer))
            {
                pipeline.Renderer = new HtmlMarkdownRenderer(writer);
            }

            Convert(reader, writer, pipeline);
            writer.Flush();
        }

        public static Document Parse(string markdown)
        {
            return Parse(new StringReader(markdown), new MarkdownPipeline());
        }

        public static Document Parse(TextReader input, MarkdownPipeline pipeline)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

            BlockParserState blockParserState;
            InlineParserState inlineParserState;
            pipeline.Initialize(out blockParserState, out inlineParserState);

            var markdownParser = new MarkdownParser(input, blockParserState, inlineParserState);
            return markdownParser.Parse();
        }

        public static void Convert(TextReader reader, TextWriter writer, MarkdownPipeline pipeline)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

            var document = Parse(reader, pipeline);
            pipeline.Renderer.WriteChildren(document);
            writer.Flush();
        }
    }
}