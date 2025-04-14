// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;

namespace Markdig;

/// <summary>
/// Provides methods for parsing a Markdown string to a syntax tree and converting it to other formats.
/// </summary>
public static class Markdown
{
    [field: MaybeNull]
    public static string Version => field ??= typeof(Markdown).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown";

    internal static readonly MarkdownPipeline DefaultPipeline = new MarkdownPipelineBuilder().Build();

    [field: MaybeNull]
    private static MarkdownPipeline DefaultTrackTriviaPipeline => field ??= new MarkdownPipelineBuilder().EnableTrackTrivia().Build();

    private static MarkdownPipeline GetPipeline(MarkdownPipeline? pipeline, string markdown)
    {
        if (pipeline is null)
        {
            return DefaultPipeline;
        }

        if (pipeline.SelfPipeline is not null)
        {
            return pipeline.SelfPipeline.CreatePipelineFromInput(markdown);
        }

        return pipeline;
    }


    /// <summary>
    /// Normalizes the specified markdown to a normalized markdown text.
    /// </summary>
    /// <param name="markdown">The markdown.</param>
    /// <param name="options">The normalize options</param>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>A normalized markdown text.</returns>
    public static string Normalize(string markdown, NormalizeOptions? options = null, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        var writer = new StringWriter();
        Normalize(markdown, writer, options, pipeline, context);
        return writer.ToString();
    }

    /// <summary>
    /// Normalizes the specified markdown to a normalized markdown text.
    /// </summary>
    /// <param name="markdown">The markdown.</param>
    /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
    /// <param name="options">The normalize options</param>
    /// <param name="pipeline">The pipeline.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>A normalized markdown text.</returns>
    public static MarkdownDocument Normalize(string markdown, TextWriter writer, NormalizeOptions? options = null, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();

        pipeline = GetPipeline(pipeline, markdown);

        var document = MarkdownParser.Parse(markdown, pipeline, context);

        var renderer = new NormalizeRenderer(writer, options);
        pipeline.Setup(renderer);

        renderer.Render(document);
        writer.Flush();

        return document;
    }

    /// <summary>
    /// Converts a Markdown string to HTML.
    /// </summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>The HTML string.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="markdown"/> is null.</exception>
    public static string ToHtml(string markdown, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();

        pipeline = GetPipeline(pipeline, markdown);

        var document = MarkdownParser.Parse(markdown, pipeline, context);

        return ToHtml(document, pipeline);
    }

    /// <summary>
    /// Converts a Markdown document to HTML.
    /// </summary>
    /// <param name="document">A Markdown document.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <returns>The HTML string.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="document"/> is null.</exception>
    public static string ToHtml(this MarkdownDocument document, MarkdownPipeline? pipeline = null)
    {
        if (document is null) ThrowHelper.ArgumentNullException(nameof(document));

        pipeline ??= DefaultPipeline;

        using var rentedRenderer = pipeline.RentHtmlRenderer();
        HtmlRenderer renderer = rentedRenderer.Instance;

        renderer.Render(document);
        renderer.Writer.Flush();

        return renderer.Writer.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Converts a Markdown document to HTML.
    /// </summary>
    /// <param name="document">A Markdown document.</param>
    /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <returns>The HTML string.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="document"/> is null.</exception>
    public static void ToHtml(this MarkdownDocument document, TextWriter writer, MarkdownPipeline? pipeline = null)
    {
        if (document is null) ThrowHelper.ArgumentNullException(nameof(document));
        if (writer is null) ThrowHelper.ArgumentNullException_writer();

        pipeline ??= DefaultPipeline;

        using var rentedRenderer = pipeline.RentHtmlRenderer(writer);
        HtmlRenderer renderer = rentedRenderer.Instance;

        renderer.Render(document);
        renderer.Writer.Flush();
    }

    /// <summary>
    /// Converts a Markdown string to HTML and output to the specified writer.
    /// </summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>The Markdown document that has been parsed</returns>
    /// <exception cref="ArgumentNullException">if reader or writer variable are null</exception>
    public static MarkdownDocument ToHtml(string markdown, TextWriter writer, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();
        if (writer is null) ThrowHelper.ArgumentNullException_writer();

        pipeline = GetPipeline(pipeline, markdown);

        var document = MarkdownParser.Parse(markdown, pipeline, context);

        ToHtml(document, writer, pipeline);

        return document;
    }

    /// <summary>
    /// Converts a Markdown string using a custom <see cref="IMarkdownRenderer"/>.
    /// </summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="renderer">The renderer to convert Markdown to.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <exception cref="ArgumentNullException">if markdown or writer variable are null</exception>
    public static object Convert(string markdown, IMarkdownRenderer renderer, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();
        if (renderer is null) ThrowHelper.ArgumentNullException(nameof(renderer));

        pipeline = GetPipeline(pipeline, markdown);

        var document = MarkdownParser.Parse(markdown, pipeline, context);

        pipeline.Setup(renderer);
        return renderer.Render(document);
    }

    /// <summary>
    /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
    /// </summary>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="trackTrivia">Whether to parse trivia such as whitespace, extra heading characters and unescaped string values.</param>
    /// <returns>An AST Markdown document</returns>
    /// <exception cref="ArgumentNullException">if markdown variable is null</exception>
    public static MarkdownDocument Parse(string markdown, bool trackTrivia = false)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();

        MarkdownPipeline? pipeline = trackTrivia ? DefaultTrackTriviaPipeline : null;

        return Parse(markdown, pipeline);
    }

    /// <summary>
    /// Parses the specified markdown into an AST <see cref="MarkdownDocument"/>
    /// </summary>
    /// <param name="markdown">The markdown text.</param>
    /// <param name="pipeline">The pipeline used for the parsing.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>An AST Markdown document</returns>
    /// <exception cref="ArgumentNullException">if markdown variable is null</exception>
    public static MarkdownDocument Parse(string markdown, MarkdownPipeline? pipeline, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();

        pipeline = GetPipeline(pipeline, markdown);

        return MarkdownParser.Parse(markdown, pipeline, context);
    }

    /// <summary>
    /// Converts a Markdown string to Plain text and output to the specified writer.
    /// </summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="writer">The destination <see cref="TextWriter"/> that will receive the result of the conversion.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>The Markdown document that has been parsed</returns>
    /// <exception cref="ArgumentNullException">if reader or writer variable are null</exception>
    public static MarkdownDocument ToPlainText(string markdown, TextWriter writer, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();
        if (writer is null) ThrowHelper.ArgumentNullException_writer();

        pipeline = GetPipeline(pipeline, markdown);

        var document = MarkdownParser.Parse(markdown, pipeline, context);

        // We override the renderer with our own writer
        var renderer = new HtmlRenderer(writer)
        {
            EnableHtmlForBlock = false,
            EnableHtmlForInline = false,
            EnableHtmlEscape = false,
        };
        pipeline.Setup(renderer);

        renderer.Render(document);
        writer.Flush();

        return document;
    }

    /// <summary>
    /// Converts a Markdown string to Plain text by using a <see cref="StringWriter"/> .
    /// </summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="context">A parser context used for the parsing.</param>
    /// <returns>The result of the conversion</returns>
    /// <exception cref="ArgumentNullException">if markdown variable is null</exception>
    public static string ToPlainText(string markdown, MarkdownPipeline? pipeline = null, MarkdownParserContext? context = null)
    {
        if (markdown is null) ThrowHelper.ArgumentNullException_markdown();
        var writer = new StringWriter();
        ToPlainText(markdown, writer, pipeline, context);
        return writer.ToString();
    }
}
