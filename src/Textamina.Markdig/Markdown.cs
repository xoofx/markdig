// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.IO;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Renderers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig
{
    /// <summary>
    /// Provides methods for parsing to a syntax tree and converting Markdown to other formats.
    /// </summary>
    public class Markdown
    {
        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static string ConvertToHtml(string markdown, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var reader = new StringReader(markdown);
            return ConvertToHtml(reader, pipeline) ?? string.Empty;
        }

        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="reader">A Markdown text from a <see cref="TextReader"/>.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static string ConvertToHtml(TextReader reader, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var writer = new StringWriter();
            ConvertToHtml(reader, writer, pipeline);
            return writer.ToString();
        }

        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="reader">A Markdown text from a <see cref="TextReader"/>.</param>
        /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <exception cref="System.ArgumentNullException">if reader or writer variable are null</exception>
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

        /// <summary>
        /// Parses the specified markdown into an AST <see cref="Document"/>
        /// </summary>
        /// <param name="markdown">The markdown text.</param>
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static Document Parse(string markdown)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            return Parse(new StringReader(markdown), new MarkdownPipeline());
        }

        /// <summary>
        /// Parses the specified markdown into an AST <see cref="Document"/>
        /// </summary>
        /// <param name="reader">A Markdown text from a <see cref="TextReader"/>.</param>
        /// <param name="pipeline">The pipeline used for the parsing.</param>
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if reader variable is null</exception>
        public static Document Parse(TextReader reader, MarkdownPipeline pipeline = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            pipeline = pipeline ?? new MarkdownPipeline();

            // Initialize the pipeline
            pipeline.Initialize();
            var stringBuilderCache = pipeline.StringBuilderCache ?? new StringBuilderCache();

            var document = new Document();

            // Initialize the block parsers
            var blockParserList = new BlockParserList();
            blockParserList.AddRange(pipeline.BlockParsers);
            var blockParserState = new BlockParserState(stringBuilderCache, document, blockParserList);

            // Initialize the inline parsers
            var inlineParserList = new InlineParserList();
            inlineParserList.AddRange(pipeline.InlineParsers);
            var inlineParserState = new InlineParserState(stringBuilderCache, document, inlineParserList);

            // Perform the parsing
            var markdownParser = new MarkdownParser(reader, blockParserState, inlineParserState);
            return markdownParser.Parse();
        }
    }
}