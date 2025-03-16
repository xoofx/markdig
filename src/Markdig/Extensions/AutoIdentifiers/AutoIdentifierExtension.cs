// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.AutoIdentifiers;

/// <summary>
/// The auto-identifier extension
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class AutoIdentifierExtension : IMarkdownExtension
{
    private const string AutoIdentifierKey = "AutoIdentifier";

    private static readonly StripRendererCache _rendererCache = new();

    private readonly AutoIdentifierOptions _options;
    private readonly ProcessInlineDelegate _processInlinesBegin;
    private readonly ProcessInlineDelegate _processInlinesEnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoIdentifierExtension"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public AutoIdentifierExtension(AutoIdentifierOptions options)
    {
        _options = options;
        _processInlinesBegin = DocumentOnProcessInlinesBegin;
        _processInlinesEnd = HeadingBlock_ProcessInlinesEnd;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
        if (headingBlockParser != null)
        {
            // Install a hook on the HeadingBlockParser when a HeadingBlock is actually processed
            headingBlockParser.Closed -= HeadingBlockParser_Closed;
            headingBlockParser.Closed += HeadingBlockParser_Closed;
        }
        var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
        if (paragraphBlockParser != null)
        {
            // Install a hook on the ParagraphBlockParser when a HeadingBlock is actually processed as a Setex heading
            paragraphBlockParser.Closed -= HeadingBlockParser_Closed;
            paragraphBlockParser.Closed += HeadingBlockParser_Closed;
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    /// <summary>
    /// Process on a new <see cref="HeadingBlock"/>
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <param name="block">The heading block.</param>
    private void HeadingBlockParser_Closed(BlockProcessor processor, Block block)
    {
        // We may have a ParagraphBlock here as we have a hook on the ParagraphBlockParser
        if (!(block is HeadingBlock headingBlock))
        {
            return;
        }

        // If the AutoLink options is set, we register a LinkReferenceDefinition at the document level
        if ((_options & AutoIdentifierOptions.AutoLink) != 0)
        {
            var headingLine = headingBlock.Lines.Lines[0];

            var text = headingLine.ToString();

            var linkRef = new HeadingLinkReferenceDefinition(headingBlock)
            {
                CreateLinkInline = CreateLinkInlineForHeading
            };

            var doc = processor.Document;
            var dictionary = doc.GetData(this) as Dictionary<string, HeadingLinkReferenceDefinition>;
            if (dictionary is null)
            {
                dictionary = new Dictionary<string, HeadingLinkReferenceDefinition>();
                doc.SetData(this, dictionary);
                doc.ProcessInlinesBegin += _processInlinesBegin;
            }
            dictionary[text] = linkRef;
        }

        // Then we register after inline have been processed to actually generate the proper #id
        headingBlock.ProcessInlinesEnd += _processInlinesEnd;
    }

    private void DocumentOnProcessInlinesBegin(InlineProcessor processor, Inline? inline)
    {
        var doc = processor.Document;
        doc.ProcessInlinesBegin -= _processInlinesBegin;
        var dictionary = (Dictionary<string, HeadingLinkReferenceDefinition>)doc.GetData(this)!;
        foreach (var keyPair in dictionary)
        {
            // Here we make sure that auto-identifiers will not override an existing link definition
            // defined in the document
            // If it is the case, we skip the auto identifier for the Heading
            if (!doc.TryGetLinkReferenceDefinition(keyPair.Key, out var linkDef))
            {
                doc.SetLinkReferenceDefinition(keyPair.Key, keyPair.Value, true);
            }
        }
        // Once we are done, we don't need to keep the intermediate dictionary around
        doc.RemoveData(this);
    }

    /// <summary>
    /// Callback when there is a reference to found to a heading.
    /// Note that reference are only working if they are declared after.
    /// </summary>
    private static Inline CreateLinkInlineForHeading(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline? child)
    {
        var headingRef = (HeadingLinkReferenceDefinition) linkRef;
        return new LinkInline()
        {
            // Use GetDynamicUrl to allow late binding of the Url (as a link may occur before the heading is declared and
            // the inlines of the heading are actually processed by HeadingBlock_ProcessInlinesEnd)
            GetDynamicUrl = () => HtmlHelper.Unescape("#" + headingRef.Heading.GetAttributes().Id),
            Title = HtmlHelper.Unescape(linkRef.Title),
        };
    }

    /// <summary>
    /// Process the inlines of the heading to create a unique identifier
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <param name="inline">The inline.</param>
    private void HeadingBlock_ProcessInlinesEnd(InlineProcessor processor, Inline? inline)
    {
        var identifiers = processor.Document.GetData(AutoIdentifierKey) as HashSet<string>;
        if (identifiers is null)
        {
            identifiers = new HashSet<string>();
            processor.Document.SetData(AutoIdentifierKey, identifiers);
        }

        var headingBlock = (HeadingBlock) processor.Block!;
        if (headingBlock.Inline is null)
        {
            return;
        }

        // If id is already set, don't try to modify it
        var attributes = processor.Block!.GetAttributes();
        if (attributes.Id != null)
        {
            return;
        }

        // Use internally a HtmlRenderer to strip links from a heading
        var stripRenderer = _rendererCache.Get();

        stripRenderer.Render(headingBlock.Inline);
        ReadOnlySpan<char> rawHeadingText = ((FastStringWriter)stripRenderer.Writer).AsSpan();

        // Urilize the link
        string headingText = (_options & AutoIdentifierOptions.GitHub) != 0
            ? LinkHelper.UrilizeAsGfm(rawHeadingText)
            : LinkHelper.Urilize(rawHeadingText, (_options & AutoIdentifierOptions.AllowOnlyAscii) != 0);

        _rendererCache.Release(stripRenderer);

        // If the heading is empty, use the word "section" instead
        var baseHeadingId = string.IsNullOrEmpty(headingText) ? "section" : headingText;

        // Add a trailing -1, -2, -3...etc. in case of collision
        var headingId = baseHeadingId;
        if (!identifiers.Add(headingId))
        {
            var headingBuffer = new ValueStringBuilder(stackalloc char[ValueStringBuilder.StackallocThreshold]);
            headingBuffer.Append(baseHeadingId);
            headingBuffer.Append('-');
            uint index = 0;
            do
            {
                index++;
                headingBuffer.Append(index);
                headingId = headingBuffer.AsSpan().ToString();
                headingBuffer.Length = baseHeadingId.Length + 1;
            }
            while (!identifiers.Add(headingId));
            headingBuffer.Dispose();
        }

        attributes.Id = headingId;
    }

    private sealed class StripRendererCache : ObjectCache<HtmlRenderer>
    {
        protected override HtmlRenderer NewInstance()
        {
            var headingWriter = new FastStringWriter();
            var stripRenderer = new HtmlRenderer(headingWriter)
            {
                // Set to false both to avoid having any HTML tags in the output
                EnableHtmlForInline = false,
                EnableHtmlEscape = false
            };
            return stripRenderer;
        }

        protected override void Reset(HtmlRenderer instance)
        {
            instance.ResetInternal();

            ((FastStringWriter)instance.Writer).Reset();
        }
    }
}