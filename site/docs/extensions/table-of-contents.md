---
title: Table of Contents
---

# Table of Contents

Enable with `.UseTableOfContents()` (not included in `UseAdvancedExtensions()`).

This extension replaces a paragraph that consists of `[TOC]` alone with a nested list of the headings of the document. Every entry links to its heading, so the extension is usually combined with [auto-identifiers](auto-identifiers.md), which give every heading an ID.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAutoIdentifiers()
    .UseTableOfContents()
    .Build();

var html = Markdown.ToHtml("[TOC]\n\n# Install\n## Windows\n## Linux\n", pipeline);
```

```html
<nav class="toc" role="doc-toc">
<ul>
<li><a href="#install">Install</a>
<ul>
<li><a href="#windows">Windows</a></li>
<li><a href="#linux">Linux</a></li>
</ul>
</li>
</ul>
</nav>
<h1 id="install">Install</h1>
<h2 id="windows">Windows</h2>
<h2 id="linux">Linux</h2>
```

## The marker

A paragraph is a marker if it consists of `[TOC]` alone: a single line in exactly this casing, which may be indented by up to three spaces and followed by spaces or tabs. The marker also works inside block quotes, list items and other container blocks, and it needs no blank line after it when the next line starts a block that interrupts a paragraph, such as `# Heading`.

The table of contents lists every heading of the document, including the headings before the marker and the headings inside container blocks. Headings without content are left out, and every marker of a document renders the same list.

These are not markers and render as usual:

{.table}
| Markdown | Reason |
|---|---|
| `[TOC] text` | The paragraph contains more than the marker. |
| `[TOC]` followed directly by a line of text or a pipe table | The next line belongs to the same paragraph. |
| `> Quote` followed directly by `[TOC]` | `[TOC]` continues the quoted paragraph. |
| `[toc]`, `[ TOC ]` | The casing or the spacing differs. |
| `` `[TOC]` ``, or `[TOC]` indented by four spaces | The marker is code. |
| `[TOC]` followed by `===` or `---` | The paragraph is a setext heading. |

The marker takes precedence over a link with the label `TOC`, whether a link reference definition such as `[TOC]: /url` or a heading with the text `TOC` provides it. Write `\[TOC]` to keep the text, or `[TOC][]` to keep the link.

## HTML output

The list is wrapped in a `nav` element, which is a navigation landmark. The `doc-toc` role from the [Digital Publishing WAI-ARIA Module 1.1](https://www.w3.org/TR/dpub-aria-1.1/#doc-toc) identifies the landmark as a table of contents, so it needs no label that would have to match the language of the document. The `toc` class is a hook for styling.

- An entry contains the plain text of its heading. Formatting such as emphasis or code is dropped, so that all entries look the same and links or footnote references in a heading do not end up inside the entry.
- A heading is nested below the closest preceding heading with a lower level. A skipped level, such as an `h3` directly below an `h1`, produces no empty entry.
- If no heading is listed, the marker renders nothing.
- `Markdown.ToPlainText` writes one entry per line, and `Markdown.Normalize` writes the marker back as `[TOC]`.

## Links

An entry links to `#` followed by the ID of its heading. The extension does not create IDs: enable `UseAutoIdentifiers()`, or set IDs with [generic attributes](generic-attributes.md), as in `# Install {#setup}`. An entry for a heading without an ID contains the text without a link.

The links are written like Markdown links, so `HtmlRenderer.BaseUrl` resolves them and `HtmlRenderer.LinkRewriter` receives them. With a base URL, an entry links to the same URL as `[Install](#install)`:

```csharp
using Markdig.Renderers;

var writer = new StringWriter();
var renderer = new HtmlRenderer(writer) { BaseUrl = new Uri("https://example.com/docs/install.html") };
pipeline.Setup(renderer);

renderer.Render(Markdown.Parse("[TOC]\n\n# Install\n", pipeline));
// <li><a href="https://example.com/docs/install.html#install">Install</a></li>
```

Set `BaseUrl` to the URL of the rendered page rather than to its folder, so that the links stay on the page.

## Options

`UseTableOfContents` accepts a `TableOfContentsOptions` object:

```csharp
using Markdig.Extensions.TableOfContents;

var pipeline = new MarkdownPipelineBuilder()
    .UseAutoIdentifiers()
    .UseTableOfContents(new TableOfContentsOptions
    {
        ListKind = TableOfContentsListKind.Ordered, // ol instead of ul
        MinLevel = 2, // leave out the document title
        MaxLevel = 3, // leave out h4 and deeper
    })
    .Build();
```

{.table}
| Option | Default | Description |
|---|---|---|
| `ListKind` | `Unordered` | `Unordered` renders `ul` elements, and `Ordered` renders `ol` elements. |
| `MinLevel` | `1` | The minimum level of the headings to list. It must be at least 1. |
| `MaxLevel` | `6` | The maximum level of the headings to list. It must not be less than `MinLevel`. Levels above 6 exist only if `HeadingBlockParser.MaxLeadingCount` is raised. |

Invalid options throw an `ArgumentException` when the extension is created. The extension copies the options, so changing them afterwards has no effect.

## Styling

The extension writes no styles. For numbers such as `1`, `1.1` and `1.2`, combine `TableOfContentsListKind.Ordered` with CSS counters. Browsers restart every nested `ol` at 1, so dotted numbers need CSS in any case:

```css
nav.toc ol { counter-reset: toc; list-style: none; }
nav.toc ol > li { counter-increment: toc; }
nav.toc ol > li::before { content: counters(toc, ".") " "; }
```

CSS multi-column layout spreads a long table of contents over as many columns as fit, and keeps every section together with its subsections:

```css
nav.toc > ul, nav.toc > ol { columns: 16em; }
nav.toc > ul > li, nav.toc > ol > li { break-inside: avoid; }
```

## Custom rendering

`TableOfContentsBlock.Entries` holds the listed headings as a tree: every `TableOfContentsEntry` has a `Heading` and the `Children` nested below it. To change the markup, replace `HtmlTableOfContentsRenderer` with a subclass. This one wraps the table of contents in an `aside` element:

```csharp
using Markdig.Extensions.TableOfContents;
using Markdig.Renderers;

public sealed class AsideTableOfContentsRenderer : HtmlTableOfContentsRenderer
{
    protected override void Write(HtmlRenderer renderer, TableOfContentsBlock obj)
    {
        // Keep the plain text output and the empty case of the base renderer
        if (obj.Entries.Count == 0 || !renderer.EnableHtmlForBlock)
        {
            base.Write(renderer, obj);
            return;
        }

        renderer.EnsureLine();
        renderer.WriteLine("<aside>");
        base.Write(renderer, obj);
        renderer.WriteLine("</aside>");
    }
}
```

Replace the renderer after the pipeline has set up the `HtmlRenderer`:

```csharp
var writer = new StringWriter();
var renderer = new HtmlRenderer(writer);
pipeline.Setup(renderer);
renderer.ObjectRenderers.Replace<HtmlTableOfContentsRenderer>(new AsideTableOfContentsRenderer());

renderer.Render(Markdown.Parse(markdown, pipeline));
```

An `aside` element has the implicit role `complementary`, and the [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/practices/landmark-regions/#complementary) recommend that complementary landmarks are not contained in other landmarks. Use such a wrapper only if the rendered Markdown is not placed inside a landmark such as `main`. See [Renderers](../advanced/renderers.md) for more about custom renderers.

## Limitations

- When trivia is tracked (`EnableTrackTrivia()`), the marker stays a paragraph and renders as `<p>[TOC]</p>`, so that a roundtrip writes it back unchanged.
- The table of contents is part of the document. To render one elsewhere, for example in the sidebar of a page layout, walk `document.Descendants<HeadingBlock>()` instead.
- Entries contain plain text only. A custom renderer can render the inlines of `entry.Heading` differently.
