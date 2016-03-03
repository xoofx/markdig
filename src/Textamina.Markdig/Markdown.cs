using System;
using System.IO;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig
{
    public class Markdown
    {
        public static string ConvertToHtml(string markdown, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var reader = new StringReader(markdown);
            return ConvertToHtml(reader, pipeline) ?? string.Empty;
        }

        public static string ConvertToHtml(TextReader reader, MarkdownPipeline pipeline = null)
        {
            var writer = new StringWriter();
            ConvertToHtml(reader, writer, pipeline);
            return writer.ToString();
        }

        public static void ConvertToHtml(TextReader reader, TextWriter writer, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            pipeline = pipeline ?? new MarkdownPipeline();
            pipeline.Renderer = new HtmlRenderer(writer);

            var document = Parse(reader, pipeline);
            pipeline.Renderer.Render(document);
            writer.Flush();
        }

        public static Document Parse(string markdown)
        {
            return Parse(new StringReader(markdown), new MarkdownPipeline());
        }

        public static Document Parse(TextReader input, MarkdownPipeline pipeline = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            pipeline = pipeline ?? new MarkdownPipeline();

            BlockParserState blockParserState;
            InlineParserState inlineParserState;
            pipeline.Initialize(out blockParserState, out inlineParserState);

            var markdownParser = new MarkdownParser(input, blockParserState, inlineParserState);
            return markdownParser.Parse();
        }
    }
}