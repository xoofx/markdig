// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Parsers;
using Markdig.Syntax;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Markdig.Renderers.Html;

/// <summary>
/// An HTML renderer for a <see cref="CodeBlock"/> and <see cref="FencedCodeBlock"/>.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{CodeBlock}" />
public class CodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeBlockRenderer"/> class.
    /// </summary>
    public CodeBlockRenderer() { }

    public bool OutputAttributesOnPre { get; set; }

    /// <summary>
    /// Gets a map of fenced code block infos that should be rendered as div blocks instead of pre/code blocks.
    /// </summary>
    public HashSet<string> BlocksAsDiv { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a map of custom block mapping to render as custom blocks instead of pre/code blocks.
    /// For example defining {"mermaid", "pre"} will render a block with info `mermaid` as a `pre` block but without the code HTML element.
    /// </summary>
    public Dictionary<string, string> BlockMapping { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    [field: MaybeNull]
    private FrozenSet<string> SpecialBlockMapping
    {
        get
        {
            return field ?? CreateNew();

            [MethodImpl(MethodImplOptions.NoInlining)]
            FrozenSet<string> CreateNew()
            {
                HashSet<string> set = [.. BlocksAsDiv, .. BlockMapping.Keys];
                return field = set.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
            }
        }
    }

    protected override void Write(HtmlRenderer renderer, CodeBlock obj)
    {
        renderer.EnsureLine();

        if (obj is FencedCodeBlock { Info: string info } && SpecialBlockMapping.Contains(info))
        {
            var infoPrefix = (obj.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                             FencedCodeBlockParser.DefaultInfoPrefix;

            var htmlBlock = BlockMapping.TryGetValue(info, out var blockType) ? blockType : "div";

            // We are replacing the HTML attribute `language-mylang` by `mylang` only for a div block
            // NOTE that we are allocating a closure here

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteRaw('<');
                renderer.Write(htmlBlock)
                        .WriteAttributes(obj.TryGetAttributes(),
                            cls => cls.StartsWith(infoPrefix, StringComparison.Ordinal) ? cls.Substring(infoPrefix.Length) : cls)
                        .WriteRaw('>');
            }

            renderer.WriteLeafRawLines(obj, true, true, true);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("</").Write(htmlBlock).WriteLine(">");
            }
        }
        else
        {
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<pre");

                if (OutputAttributesOnPre)
                {
                    renderer.WriteAttributes(obj);
                }

                renderer.WriteRaw("><code");

                if (!OutputAttributesOnPre)
                {
                    renderer.WriteAttributes(obj);
                }

                renderer.WriteRaw('>');
            }

            renderer.WriteLeafRawLines(obj, true, renderer.EnableHtmlEscape);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</code></pre>");
            }
        }

        renderer.EnsureLine();
    }
}