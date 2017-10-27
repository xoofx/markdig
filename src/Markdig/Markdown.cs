// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.IO;
using System.Reflection;
using Markdig.Extensions.SelfPipeline;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;

namespace Markdig
{
    /// <summary>
    /// Provides methods for parsing a Markdown string to a syntax tree and converting it to other formats.
    /// </summary>
    public static partial class Markdown
    {
#if NETSTANDARD_11
        public static readonly string Version = typeof(Markdown).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
#else
        public static readonly string Version = ((AssemblyFileVersionAttribute) typeof(Markdown).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0]).Version;
#endif

        /// <summary>
        /// Normalizes the specified markdown to a normalized markdown text.
        /// </summary>
        /// <param name="markdown">The markdown.</param>
        /// <param name="options">The normalize options</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>A normalized markdown text.</returns>
        public static string Normalize(string markdown, NormalizeOptions options = null, MarkdownPipeline pipeline = null)
        {
            var writer = new StringWriter();
            Normalize(markdown, writer, options, pipeline);
            return writer.ToString();
        }

        /// <summary>
        /// Normalizes the specified markdown to a normalized markdown text.
        /// </summary>
        /// <param name="markdown">The markdown.</param>
        /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
        /// <param name="options">The normalize options</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>A normalized markdown text.</returns>
        public static MarkdownDocument Normalize(string markdown, TextWriter writer, NormalizeOptions options = null, MarkdownPipeline pipeline = null)
        {
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();
            pipeline = CheckForSelfPipeline(pipeline, markdown);

            // We override the renderer with our own writer
            var renderer = new NormalizeRenderer(writer, options);
            pipeline.Setup(renderer);

            var document = Parse(markdown, pipeline);
            renderer.Render(document);
            writer.Flush();

            return document;
        }

        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static string ToHtml(string markdown, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var writer = new StringWriter();
            ToHtml(markdown, writer, pipeline);
            return writer.ToString();
        }

        /// <summary>
        /// Converts a Markdown string to HTML and output to the specified writer.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The Markdown document that has been parsed</returns>
        /// <exception cref="System.ArgumentNullException">if reader or writer variable are null</exception>
        public static MarkdownDocument ToHtml(string markdown, TextWriter writer, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();
            pipeline = CheckForSelfPipeline(pipeline, markdown);

            // We override the renderer with our own writer
            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);

            var document = Parse(markdown, pipeline);
            renderer.Render(document);
            writer.Flush();

            return document;
        }

        /// <summary>
        /// Converts a Markdown string using a custom <see cref="IMarkdownRenderer"/>.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="renderer">The renderer to convert Markdown to.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <exception cref="System.ArgumentNullException">if markdown or writer variable are null</exception>
        public static object Convert(string markdown, IMarkdownRenderer renderer, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();

            pipeline = CheckForSelfPipeline(pipeline, markdown);
            var document = Parse(markdown, pipeline);
            pipeline.Setup(renderer);
            return renderer.Render(document);
        }

        /// <summary>
        /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
        /// </summary>
        /// <param name="markdown">The markdown text.</param>
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static MarkdownDocument Parse(string markdown)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            return Parse(markdown, null);
        }

        /// <summary>
        /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
        /// </summary>
        /// <param name="markdown">The markdown text.</param>
        /// <param name="pipeline">The pipeline used for the parsing.</param>
        /// <returns>An AST Markdown document</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static MarkdownDocument Parse(string markdown, MarkdownPipeline pipeline)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();

            pipeline = CheckForSelfPipeline(pipeline, markdown);
            return MarkdownParser.Parse(markdown, pipeline);
        }

        private static MarkdownPipeline CheckForSelfPipeline(MarkdownPipeline pipeline, string markdown)
        {
            var selfPipeline = pipeline.Extensions.Find<SelfPipelineExtension>();
            if (selfPipeline != null)
            {
                return selfPipeline.CreatePipelineFromInput(markdown);
            }
            return pipeline;
        }

        /// <summary>
        /// Converts a Markdown string to Plain text and output to the specified writer.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The Markdown document that has been parsed</returns>
        /// <exception cref="System.ArgumentNullException">if reader or writer variable are null</exception>
        public static MarkdownDocument ToPlainText(string markdown, TextWriter writer, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            pipeline = pipeline ?? new MarkdownPipelineBuilder().Build();
            pipeline = CheckForSelfPipeline(pipeline, markdown);

            // We override the renderer with our own writer
            var renderer = new HtmlRenderer(writer)
            {
                EnableHtmlForBlock = false,
                EnableHtmlForInline = false
            };
            pipeline.Setup(renderer);

            var document = Parse(markdown, pipeline);
            renderer.Render(document);
            writer.Flush();

            return document;
        }

        /// <summary>
        /// Converts a Markdown string to HTML.
        /// </summary>
        /// <param name="markdown">A Markdown text.</param>
        /// <param name="pipeline">The pipeline used for the conversion.</param>
        /// <returns>The result of the conversion</returns>
        /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
        public static string ToPlainText(string markdown, MarkdownPipeline pipeline = null)
        {
            if (markdown == null) throw new ArgumentNullException(nameof(markdown));
            var writer = new StringWriter();
            ToPlainText(markdown, writer, pipeline);
            return writer.ToString();
        }
    }
}