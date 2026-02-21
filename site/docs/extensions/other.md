---
title: Other extensions
---

# Other extensions

This page covers smaller or more specialized extensions that are not part of `UseAdvancedExtensions()`.

## Bootstrap

Enable with `.UseBootstrap()`.

Adds Bootstrap CSS classes to generated HTML elements:

{.table}
| Element | Class applied |
|---|---|
| `<table>` | `table` |
| `<blockquote>` | `blockquote` |
| `<figure>` | `figure` |
| `<figcaption>` | `figure-caption` |
| `<img>` | `img-fluid` |

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseBootstrap()
    .Build();
```

## Hardline breaks

Enable with `.UseSoftlineBreakAsHardlineBreak()`.

Makes every soft line break (a single newline inside a paragraph) render as a `<br>` tag instead of a space. Useful when you want each line to appear as written.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseSoftlineBreakAsHardlineBreak()
    .Build();
```

**Without** this extension:

```markdown
Line one
Line two
```

Renders as: `<p>Line one Line two</p>`

**With** this extension, it renders as: `<p>Line one<br />Line two</p>`

## JIRA links

Enable with `.UseJiraLinks(options)`.

Automatically converts JIRA-style project references (e.g., `PROJECT-123`) into clickable links.

```csharp
using Markdig.Extensions.JiraLinks;

var pipeline = new MarkdownPipelineBuilder()
    .UseJiraLinks(new JiraLinkOptions("https://jira.example.com/browse/"))
    .Build();

var html = Markdown.ToHtml("Fixed in PROJ-456.", pipeline);
// => <p>Fixed in <a href="https://jira.example.com/browse/PROJ-456">PROJ-456</a>.</p>
```

## Globalization

Enable with `.UseGlobalization()`.

Adds appropriate `dir` attributes on HTML elements for right-to-left content. Detects the text direction of each block and annotates it.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseGlobalization()
    .Build();
```

## Referral links

Enable with `.UseReferralLinks(rels)`.

Adds `rel` attributes to all rendered links:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseReferralLinks("nofollow", "noopener", "noreferrer")
    .Build();
```

All links will have `rel="nofollow noopener noreferrer"` added.

## Self pipeline

Enable with `.UseSelfPipeline()`.

Detects the pipeline configuration from the Markdown document itself via a special HTML comment tag:

```markdown
<!--markdig:extensions=pipetables+footnotes-->

| A | B |
|---|---|
| 1 | 2 |
```

> [!WARNING]
> `UseSelfPipeline` cannot be combined with other extensions on the same builder — it replaces the pipeline entirely based on the document content.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseSelfPipeline() // Must be the only extension
    .Build();
```

## Pragma lines

Enable with `.UsePragmaLines()`.

Inserts `<span id="pragma-line-N"></span>` markers into the HTML output for each source line. This is useful for editor synchronization (scrolling an editor to the rendered position).

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePragmaLines()
    .Build();
```

## Non-ASCII no escape

Enable with `.UseNonAsciiNoEscape()`.

Disables percent-encoding of non-ASCII characters in URLs. This works around a rendering bug in Internet Explorer/Edge with local file links containing non-US-ASCII characters.

> [!CAUTION]
> Only use this extension if you specifically need IE/Edge compatibility with non-ASCII file paths. It changes standard URL encoding behavior.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseNonAsciiNoEscape()
    .Build();
```

## Precise source location

Enable with `.UsePreciseSourceLocation()`.

Maps every AST node to its exact position in the original source text via the `Span` property. Useful for syntax highlighting, editors, and linters.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePreciseSourceLocation()
    .Build();

var document = Markdown.Parse(text, pipeline);
foreach (var node in document.Descendants())
{
    Console.WriteLine($"{node.GetType().Name}: {node.Span}");
}
```

## Disable HTML

Use `.DisableHtml()` (a configuration option, not an extension).

Removes the HTML block parser and disables inline HTML parsing. Useful for safe rendering of user-provided Markdown:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .DisableHtml()
    .Build();
```
