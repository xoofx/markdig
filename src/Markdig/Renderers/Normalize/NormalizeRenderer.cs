// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.IO;
using Markdig.Syntax;
using Markdig.Renderers.Normalize.Inlines;
using Markdig.Helpers;

namespace Markdig.Renderers.Normalize;

/// <summary>
/// Default HTML renderer for a Markdown <see cref="MarkdownDocument"/> object.
/// </summary>
/// <seealso cref="TextRendererBase{NormalizeRenderer}" />
public class NormalizeRenderer : TextRendererBase<NormalizeRenderer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NormalizeRenderer"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="options">The normalize options</param>
    public NormalizeRenderer(TextWriter writer, NormalizeOptions? options = null) : base(writer)
    {
        Options = options ?? new NormalizeOptions();
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

        // Default inline renderers
        ObjectRenderers.Add(new AutolinkInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new DelimiterInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new NormalizeHtmlInlineRenderer());
        ObjectRenderers.Add(new NormalizeHtmlEntityInlineRenderer());            
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LiteralInlineRenderer());
    }

    public NormalizeOptions Options { get; }

    public bool CompactParagraph { get; set; }

    public void FinishBlock(bool emptyLine)
    {
        if (!IsLastInContainer)
        {
            WriteLine();
            if (emptyLine)
            {
                WriteLine();
            }
        }
    }

    ///// <summary>
    ///// Writes the attached <see cref="HtmlAttributes"/> on the specified <see cref="MarkdownObject"/>.
    ///// </summary>
    ///// <param name="obj">The object.</param>
    ///// <returns></returns>
    //public NormalizeRenderer WriteAttributes(MarkdownObject obj)
    //{
    //    if (obj is null) throw new ArgumentNullException(nameof(obj));
    //    return WriteAttributes(obj.TryGetAttributes());
    //}

    ///// <summary>
    ///// Writes the specified <see cref="HtmlAttributes"/>.
    ///// </summary>
    ///// <param name="attributes">The attributes to render.</param>
    ///// <returns>This instance</returns>
    //public NormalizeRenderer WriteAttributes(HtmlAttributes attributes)
    //{
    //    if (attributes is null)
    //    {
    //        return this;
    //    }

    //    if (attributes.Id != null)
    //    {
    //        Write(" id=\"").WriteEscape(attributes.Id).Write('"');
    //    }

    //    if (attributes.Classes != null && attributes.Classes.Count > 0)
    //    {
    //        Write(" class=\"");
    //        for (int i = 0; i < attributes.Classes.Count; i++)
    //        {
    //            var cssClass = attributes.Classes[i];
    //            if (i > 0)
    //            {
    //                Write(" ");
    //            }
    //            WriteEscape(cssClass);
    //        }
    //        Write('"');
    //    }

    //    if (attributes.Properties != null && attributes.Properties.Count > 0)
    //    {
    //        foreach (var property in attributes.Properties)
    //        {
    //            Write(' ').Write(property.Key);
    //            if (property.Value != null)
    //            {
    //                Write('=').Write('"');
    //                WriteEscape(property.Value);
    //                Write('"');
    //            }
    //        }
    //    }

    //    return this;
    //}

    /// <summary>
    /// Writes the lines of a <see cref="LeafBlock"/>
    /// </summary>
    /// <param name="leafBlock">The leaf block.</param>
    /// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
    /// <param name="indent">Whether to write indents.</param>
    /// <returns>This instance</returns>
    public NormalizeRenderer WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool indent = false)
    {
        if (leafBlock is null) ThrowHelper.ArgumentNullException_leafBlock();
        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (int i = 0; i < lines.Count; i++)
            {
                if (!writeEndOfLines && i > 0)
                {
                    WriteLine();
                }

                if (indent)
                {
                    Write("    ");
                }

                Write(ref slices[i].Slice);

                if (writeEndOfLines)
                {
                    WriteLine();
                }
            }
        }
        return this;
    }
}