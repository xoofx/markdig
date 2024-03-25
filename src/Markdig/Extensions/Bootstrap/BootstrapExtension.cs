// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System.Linq;
using Markdig.Extensions.Alerts;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Bootstrap;

/// <summary>
/// Extension for tagging some HTML elements with bootstrap classes.
/// </summary>
/// <seealso cref="IMarkdownExtension" />
public class BootstrapExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Make sure we don't have a delegate twice
        pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
        pipeline.DocumentProcessed += PipelineOnDocumentProcessed;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            var alertRenderer = htmlRenderer.ObjectRenderers.OfType<AlertBlockRenderer>().FirstOrDefault();
            if (alertRenderer == null)
            {
                alertRenderer = new AlertBlockRenderer();
                renderer.ObjectRenderers.InsertBefore<QuoteBlockRenderer>(new AlertBlockRenderer());
            }

            alertRenderer.RenderKind = (_, _) => { };
        }
    }

    private static void PipelineOnDocumentProcessed(MarkdownDocument document)
    {
        Span<char> upperKind = new char[16];
        foreach (var node in document.Descendants())
        {
            if (node.IsInline)
            {
                if (node.IsContainerInline && node is LinkInline link && link.IsImage)
                {
                    link.GetAttributes().AddClass("img-fluid");
                }
            }
            else if (node.IsContainerBlock)
            {
                if (node is Tables.Table)
                {
                    node.GetAttributes().AddClass("table");
                }
                else if (node is AlertBlock alertBlock) // Needs to be before QuoteBlock
                {
                    var attributes = node.GetAttributes();
                    attributes.AddClass("alert");
                    attributes.AddProperty("role", "alert");
                    if (alertBlock.Kind.Length <= upperKind.Length)
                    {
                        alertBlock.Kind.AsSpan().ToUpperInvariant(upperKind);
                        attributes.AddClass(upperKind.Slice(0, alertBlock.Kind.Length) switch
                        {
                            "NOTE" => "alert-primary",
                            "TIP" => "alert-success",
                            "IMPORTANT" => "alert-info",
                            "WARNING" => "alert-warning",
                            "CAUTION" => "alert-danger",
                            _ => "alert-dark",
                        });
                    }

                    var lastParagraph = alertBlock.Descendants().OfType<ParagraphBlock>().LastOrDefault();
                    lastParagraph?.GetAttributes().AddClass("mb-0");
                }
                else if (node is QuoteBlock)
                {
                    node.GetAttributes().AddClass("blockquote");
                }
                else if (node is Figures.Figure)
                {
                    node.GetAttributes().AddClass("figure");
                }
            }
            else
            {
                if (node is Figures.FigureCaption)
                {
                    node.GetAttributes().AddClass("figure-caption");
                }
            }
        }
    }
}