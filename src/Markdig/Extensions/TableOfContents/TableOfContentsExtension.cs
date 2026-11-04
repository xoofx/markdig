// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.TableOfContents;

/// <summary>
/// Extension that replaces a paragraph consisting of `[TOC]` alone with a <see cref="TableOfContentsBlock"/>, which renders a nested list of the headings of the document.
/// </summary>
/// <remarks>
/// A paragraph is a marker only if it is a single line that reads `[TOC]`, ignoring surrounding spaces and tabs. The marker takes precedence over a link with the label `TOC`; `\[TOC]` and `[TOC][]` keep the text and the link.
/// When trivia is tracked, the marker stays a paragraph, so that a roundtrip writes it back unchanged.
/// Entries link to headings that have an ID, for example from <see cref="MarkdownExtensions.UseAutoIdentifiers"/>; the extension does not add IDs itself.
/// </remarks>
public sealed class TableOfContentsExtension : IMarkdownExtension
{
    internal const string Marker = "[TOC]";

    private readonly TableOfContentsListKind _listKind;
    private readonly int _minLevel;
    private readonly int _maxLevel;
    private readonly ProcessInlineDelegate _documentProcessInlinesEnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableOfContentsExtension"/> class with the default options.
    /// </summary>
    public TableOfContentsExtension() : this(new TableOfContentsOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableOfContentsExtension"/> class. The options are copied.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">The list kind is not a defined <see cref="TableOfContentsListKind"/> value, the minimum level is less than 1, or the maximum level is less than the minimum level.</exception>
    public TableOfContentsExtension(TableOfContentsOptions options)
    {
        if (options is null) ThrowHelper.ArgumentNullException(nameof(options));

        if (options.ListKind is not (TableOfContentsListKind.Unordered or TableOfContentsListKind.Ordered))
        {
            ThrowHelper.ArgumentException($"Invalid list kind `{options.ListKind}`.", nameof(options));
        }

        if (options.MinLevel < 1)
        {
            ThrowHelper.ArgumentException($"Invalid minimum level `{options.MinLevel}`: the level must be at least 1.", nameof(options));
        }

        if (options.MaxLevel < options.MinLevel)
        {
            ThrowHelper.ArgumentException($"Invalid maximum level `{options.MaxLevel}`: the level must not be less than the minimum level `{options.MinLevel}`.", nameof(options));
        }

        _listKind = options.ListKind;
        _minLevel = options.MinLevel;
        _maxLevel = options.MaxLevel;
        _documentProcessInlinesEnd = Document_ProcessInlinesEnd;
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
        if (paragraphBlockParser != null)
        {
            paragraphBlockParser.Closed -= ParagraphBlockParser_Closed;
            paragraphBlockParser.Closed += ParagraphBlockParser_Closed;
        }
    }

    /// <summary>
    /// Configures this extension for the specified pipeline stage.
    /// </summary>
    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlTableOfContentsRenderer>();
        }

        if (renderer is NormalizeRenderer normalizeRenderer)
        {
            normalizeRenderer.ObjectRenderers.AddIfNotAlready<NormalizeTableOfContentsRenderer>();
        }
    }

    private void ParagraphBlockParser_Closed(BlockProcessor processor, Block block)
    {
        // A replacement block would lose the trivia of the paragraph, and no roundtrip renderer could write it back
        if (processor.TrackTrivia || block is not ParagraphBlock { Parent: { } parent } paragraph || !IsMarker(paragraph))
        {
            return;
        }

        var tableOfContents = new TableOfContentsBlock(null)
        {
            Span = paragraph.Span,
            Line = paragraph.Line,
            Column = paragraph.Column,
            ListKind = _listKind,
            // The Block constructor opens every block, but this one never enters the stack of open blocks
            IsOpen = false,
        };
        var attributes = tableOfContents.GetAttributes();
        attributes.AddClass("toc");
        attributes.AddProperty("role", "doc-toc");

        // Safe while the event runs: the processor only removes the paragraph from its open blocks afterwards and then
        // takes its last block from the container again. The paragraph keeps its lines for any later Closed handler.
        parent[parent.IndexOf(paragraph)] = tableOfContents;

        var document = processor.Document;
        if (document.GetData(this) is not List<TableOfContentsBlock> tablesOfContents)
        {
            tablesOfContents = [];
            document.SetData(this, tablesOfContents);
            document.ProcessInlinesEnd += _documentProcessInlinesEnd;
        }
        tablesOfContents.Add(tableOfContents);
    }

    private static bool IsMarker(ParagraphBlock paragraph)
    {
        // ParagraphBlockParser.Close has already removed link reference definitions and trimmed the line
        return paragraph.Lines.Count == 1 && paragraph.Lines.Lines[0].Slice.AsSpan().SequenceEqual(Marker.AsSpan());
    }

    private void Document_ProcessInlinesEnd(InlineProcessor processor, Inline? inline)
    {
        var document = processor.Document;
        var tablesOfContents = (List<TableOfContentsBlock>)document.GetData(this)!;
        document.RemoveData(this);

        var entries = BuildEntries(document);
        foreach (var tableOfContents in tablesOfContents)
        {
            tableOfContents.Entries = entries;
        }
    }

    private List<TableOfContentsEntry> BuildEntries(MarkdownDocument document)
    {
        var entries = new List<TableOfContentsEntry>();
        var parents = new Stack<TableOfContentsEntry>();
        foreach (var heading in document.Descendants<HeadingBlock>())
        {
            if (heading.Level < _minLevel || heading.Level > _maxLevel || heading.Inline?.FirstChild is null)
            {
                continue;
            }

            while (parents.Count > 0 && parents.Peek().Heading.Level >= heading.Level)
            {
                parents.Pop();
            }

            var entry = new TableOfContentsEntry(heading);
            (parents.Count > 0 ? parents.Peek().Children : entries).Add(entry);
            parents.Push(entry);
        }

        return entries;
    }
}
