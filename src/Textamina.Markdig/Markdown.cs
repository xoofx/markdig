using System;
using System.IO;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig
{
    public class Markdown
    {
        public static string Convert(string markdown)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var reader = new StringReader(markdown);
            return Convert(reader)?.ToString() ?? string.Empty;
        }

        public static object Convert(TextReader reader, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            pipeline = pipeline ?? new MarkdownPipeline();

            if (pipeline.Renderer == null)
            {
                pipeline.Renderer = new HtmlMarkdownRenderer(new StringWriter());
            }

            var document = Parse(reader, pipeline);
            return pipeline.Renderer.WriteDocument(document);
        }

        public static void Convert(TextReader reader, TextWriter writer, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            pipeline = pipeline ?? new MarkdownPipeline();

            if (!(pipeline.Renderer is HtmlMarkdownRenderer))
            {
                pipeline.Renderer = new HtmlMarkdownRenderer(writer);
            }

            var document = Parse(reader, pipeline);
            pipeline.Renderer.WriteChildren(document);
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