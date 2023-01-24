// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.IO;
using Markdig.Syntax;
using Markdig.Renderers.Roundtrip.Inlines;
using Markdig.Helpers;

namespace Markdig.Renderers.Roundtrip;

/// <summary>
/// Markdown renderer honoring trivia for a  <see cref="MarkdownDocument"/> object.
/// </summary>
/// Ensure to call the <see cref="MarkdownExtensions.EnableTrackTrivia"/> extension method when
/// parsing markdown to have trivia available for rendering.
public class RoundtripRenderer : TextRendererBase<RoundtripRenderer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoundtripRenderer"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public RoundtripRenderer(TextWriter writer) : base(writer)
    {
        // Default block renderers
        ObjectRenderers.Add(new CodeBlockRenderer());
        ObjectRenderers.Add(new ListRenderer());
        ObjectRenderers.Add(new HeadingRenderer());
        ObjectRenderers.Add(new HtmlBlockRenderer());
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new ThematicBreakRenderer());
        ObjectRenderers.Add(new LinkReferenceDefinitionGroupRenderer());
        ObjectRenderers.Add(new LinkReferenceDefinitionRenderer());
        ObjectRenderers.Add(new EmptyBlockRenderer());

        // Default inline renderers
        ObjectRenderers.Add(new AutolinkInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new DelimiterInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new RoundtripHtmlInlineRenderer());
        ObjectRenderers.Add(new RoundtripHtmlEntityInlineRenderer());            
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LiteralInlineRenderer());
    }

    /// <summary>
    /// Writes the lines of a <see cref="LeafBlock"/>
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <returns>This instance</returns>
    public void WriteLeafRawLines(LeafBlock leafBlock)
    {
        if (leafBlock is null) ThrowHelper.ArgumentNullException_leafBlock();
        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                var slice = slices[i].Slice;
                Write(ref slice);
                WriteLine(slice.NewLine);
            }
        }
    }

    public void RenderLinesBefore(Block block)
    {
        if (block.LinesBefore is null)
        {
            return;
        }
        foreach (var line in block.LinesBefore)
        {
            Write(line);
            WriteLine(line.NewLine);
        }
    }

    public void RenderLinesAfter(Block block)
    {
        previousWasLine = true;
        if (block.LinesAfter is null)
        {
            return;
        }
        foreach (var line in block.LinesAfter)
        {
            Write(line);
            WriteLine(line.NewLine);
        }
    }
}