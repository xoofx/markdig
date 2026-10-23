---
title: Relative file links
---

# Relative file links

Enable with `.UseRelativeFileLinks()` (not included in `UseAdvancedExtensions()`).

This extension rewrites links to Markdown files into links to the HTML pages generated from them. A set of Markdown files that link to each other, such as a `docs` folder, then works both when browsed on GitHub and as a generated site.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseRelativeFileLinks()
    .Build();

var html = Markdown.ToHtml("See the [usage guide](usage.md#preview).", pipeline);
// <p>See the <a href="usage.html#preview">usage guide</a>.</p>
```

## Rules

A link is rewritten when the path of its target ends with `.md` or `.markdown`, compared case-insensitively. That extension is replaced with `.html`, and the query and the fragment are kept. Only the target changes: the link text and the title stay as written. Links with a scheme, such as `https:` or `mailto:`, and protocol-relative links (`//host/path`) point outside the site and are left unchanged.

{.table}
| Markdown | Rendered `href` |
|---|---|
| `[a](usage.md)` | `usage.html` |
| `[a](usage.md?v=1#preview)` | `usage.html?v=1#preview` |
| `[a](../guide/install.md)` | `../guide/install.html` |
| `[a](/docs/README.MD)` | `/docs/README.html` |
| `[a](https://github.com/xoofx/markdig/blob/main/readme.md)` | unchanged |
| `[a](//cdn.example.com/usage.md)` | unchanged |
| `<info@example.md>` | unchanged (`mailto:info@example.md`) |
| `[a](notes.html#usage.md)` | unchanged |
| `[a](usage.mdx)` | unchanged |

## Options

`UseRelativeFileLinks` accepts a `RelativeFileLinkOptions` object:

```csharp
using Markdig.Extensions.RelativeFileLinks;

var options = new RelativeFileLinkOptions
{
    TargetExtension = "/" // usage.md#preview becomes usage/#preview
};
options.SourceExtensions.Remove(".markdown"); // rewrite .md links only

var pipeline = new MarkdownPipelineBuilder()
    .UseRelativeFileLinks(options)
    .Build();
```

{.table}
| Option | Default | Description |
|---|---|---|
| `SourceExtensions` | `.md`, `.markdown` | The file extensions to rewrite, each starting with `.`. The list starts with the defaults; clear it to replace them. |
| `TargetExtension` | `.html` | The replacement, inserted verbatim: `""` produces extensionless links and `/` directory links. |

The extension copies the options when it is created, so changing them afterwards has no effect.

## Base URL and link rewriters

The extension sets `HtmlRenderer.LinkRewriter`, so it works together with the other URL settings of the renderer:

- `HtmlRenderer.BaseUrl` resolves relative links before the extension sees them. When a base URL is set, the extension therefore rewrites every absolute link with the same scheme, host and port as the base URL, including links that the Markdown source spells out as absolute URLs.
- A `LinkRewriter` assigned to the renderer before the pipeline sets it up runs after the extension, so it receives `usage.html` rather than `usage.md`. A `LinkRewriter` assigned afterwards replaces the extension.

```csharp
using Markdig.Renderers;

var writer = new StringWriter();
var renderer = new HtmlRenderer(writer) { BaseUrl = new Uri("https://example.com/docs/") };
pipeline.Setup(renderer);

renderer.Render(Markdown.Parse("[Install](../guide/install.md)", pipeline));
// <p><a href="https://example.com/guide/install.html">Install</a></p>
```

`BaseUrl` also resolves fragment-only links such as `#install`. Set it to the URL of the rendered page rather than to its folder if links within the page must stay on the page.

## Limitations

- Links written as raw HTML, such as `<a href="usage.md">`, are not rewritten.
- Only the file extension changes: `index.md` becomes `index.html`, not a directory URL. For a mapping that depends on the layout of your site, use your own `HtmlRenderer.LinkRewriter` instead.
- Only HTML output is affected. The parsed `MarkdownDocument` and the output of `Markdown.Normalize` keep the original link targets.
