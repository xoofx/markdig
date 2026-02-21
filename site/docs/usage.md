---
title: Usage
---

# Usage

This guide covers the core Markdig workflow: parsing Markdown, rendering output, and understanding how the pipeline ties everything together.

## The Markdown static class

The `Markdown` static class is the main entry point. It provides several methods:

{.table}
| Method | Description |
|---|---|
| `Markdown.ToHtml(...)` | Convert Markdown to HTML |
| `Markdown.Parse(...)` | Parse Markdown to an AST (`MarkdownDocument`) |
| `Markdown.ToPlainText(...)` | Convert Markdown to plain text |
| `Markdown.Normalize(...)` | Normalize Markdown to a canonical form |
| `Markdown.Convert(...)` | Convert using any custom `IMarkdownRenderer` |
| `document.ToHtml(...)` | Extension method — render a parsed document to HTML |

All methods optionally accept a `MarkdownPipeline` and a `MarkdownParserContext`.

## Parse + Render: the two-phase model

Markdig uses a **two-phase model**:

1. **Parse** — Convert Markdown text into an Abstract Syntax Tree (AST).
2. **Render** — Walk the AST and produce output (HTML, plain text, or any custom format).

```csharp
using Markdig;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

// Phase 1: Parse
var document = Markdown.Parse(markdownText, pipeline);

// Phase 2: Render
var html = document.ToHtml(pipeline);
```

The convenience method `Markdown.ToHtml(string, pipeline)` does both phases in a single call, but understanding the separation is important for advanced use.

### Why the same pipeline must be passed to both Parse and Render

**This is the most common mistake new users make.** The pipeline serves two distinct purposes:

1. **During parsing**, extensions register custom `BlockParser` and `InlineParser` objects that produce extension-specific AST nodes (e.g., `MathInline`, `TaskList`, `Table`).
2. **During rendering**, extensions register custom `ObjectRenderers` that know how to convert those AST nodes into output (e.g., `HtmlMathInlineRenderer`, `HtmlTaskListRenderer`).

If you parse with one pipeline but render with another (or with no pipeline), the renderer won't know how to handle the extension-specific nodes — they'll be silently skipped or produce incorrect output.

```csharp
// ✅ Correct — same pipeline for parse and render
var pipeline = new MarkdownPipelineBuilder().UsePipeTables().Build();
var document = Markdown.Parse(markdownText, pipeline);
var html = document.ToHtml(pipeline);

// ❌ Wrong — pipeline mismatch
var parsePipeline = new MarkdownPipelineBuilder().UsePipeTables().Build();
var renderPipeline = new MarkdownPipelineBuilder().Build(); // missing PipeTables
var document = Markdown.Parse(markdownText, parsePipeline);
var html = document.ToHtml(renderPipeline); // Tables won't render correctly!

// ❌ Also wrong — no pipeline for render
var document = Markdown.Parse(markdownText, pipeline);
var html = document.ToHtml(); // Uses default pipeline — no extensions!
```

> **Rule of thumb:** Create the pipeline once, store it, and pass the same instance everywhere. Pipelines are thread-safe and immutable after building.

## The MarkdownPipeline

The `MarkdownPipeline` is an **immutable**, **thread-safe** object that holds:

- The collection of **block parsers** (identify block-level elements like paragraphs, headings, lists)
- The collection of **inline parsers** (identify inline elements like emphasis, links, code spans)
- The list of **registered extensions** (which add/modify parsers and renderers)
- Configuration flags (trivia tracking, precise source location, etc.)

You create a pipeline using the `MarkdownPipelineBuilder`:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()  // Enable extensions
    .UsePreciseSourceLocation() // Track precise source spans
    .Build();                  // Produce the immutable pipeline
```

Once built, the pipeline can be reused across threads and calls.

### The MarkdownPipelineBuilder

The builder provides a fluent API for configuration. All extension methods return the builder for chaining:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables()
    .UseFootnotes()
    .UseEmphasisExtras()
    .UseAutoLinks()
    .UseGenericAttributes() // Must be last (modifies other parsers)
    .Build();
```

#### Configuration options

{.table}
| Method | Description |
|---|---|
| `.UseAdvancedExtensions()` | Enable most extensions at once |
| `.UsePreciseSourceLocation()` | Map AST nodes to their exact source location |
| `.EnableTrackTrivia()` | Track whitespace and trivia for roundtripping |
| `.ConfigureNewLine(string)` | Set the newline string for output |
| `.DisableHeadings()` | Disable ATX and Setex heading parsing |
| `.DisableHtml()` | Disable HTML block and inline HTML parsing |

#### Extension ordering

Extensions are applied in the order they are added. Most extensions are order-independent, but a few need specific positioning:

- **`UseGenericAttributes()`** should be last — it modifies other parsers to support `{.class #id}` syntax.
- Extensions that modify the same parser (e.g., adding emphasis characters) should be aware of potential conflicts.

#### Dynamic configuration with strings

For scenarios where extensions are configured at runtime (e.g., from a config file), use the `Configure` method:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .Configure("common+pipetables+footnotes+figures")
    .Build();
```

Available extension tokens: `common`, `advanced`, `alerts`, `pipetables`, `gfm-pipetables`, `emphasisextras`, `listextras`, `hardlinebreak`, `footnotes`, `footers`, `citations`, `attributes`, `gridtables`, `abbreviations`, `emojis`, `definitionlists`, `customcontainers`, `figures`, `mathematics`, `bootstrap`, `medialinks`, `smartypants`, `autoidentifiers`, `tasklists`, `diagrams`, `nofollowlinks`, `noopenerlinks`, `noreferrerlinks`, `nohtml`, `yaml`, `nonascii-noescape`, `autolinks`, `globalization`.

## The MarkdownParserContext

For advanced scenarios, a `MarkdownParserContext` lets you pass per-call state to parsers:

```csharp
var context = new MarkdownParserContext();
// Extensions or custom parsers can read properties from the context
var document = Markdown.Parse(markdownText, pipeline, context);
```

The context is useful when custom parsers need external information (e.g., base URLs for link resolution).

## Thread safety

The `MarkdownPipeline` is thread-safe and should be shared. **Do not** create a new pipeline for every call — building a pipeline has a cost (extension setup, parser allocation).

```csharp
// ✅ Good — build once, use everywhere
private static readonly MarkdownPipeline Pipeline = 
    new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

public string RenderMarkdown(string input) 
    => Markdown.ToHtml(input, Pipeline);
```

The `Markdown.ToHtml(string, pipeline)` and `Markdown.Parse(string, pipeline)` methods are thread-safe when given a shared pipeline.

## Outputting to a TextWriter

For streaming output (e.g., directly to an HTTP response), use the `TextWriter` overloads:

```csharp
using var writer = new StreamWriter(responseStream);

// Returns the parsed MarkdownDocument
var document = Markdown.ToHtml(markdownText, writer, pipeline);
```

This avoids building the complete HTML string in memory.

## Rendering to other formats

Markdig's architecture separates parsing from rendering, so you can render the same AST to different formats:

```csharp
// Render to HTML
var html = document.ToHtml(pipeline);

// Render to plain text
var plainText = Markdown.ToPlainText(markdownText, pipeline);

// Render to normalized Markdown
var normalized = Markdown.Normalize(markdownText, pipeline: pipeline);

// Render to a custom format
Markdown.Convert(markdownText, myCustomRenderer, pipeline);
```

See the [Renderers guide](advanced/renderers.md) for how to implement custom renderers.

## Common patterns

### Parse once, render multiple times

```csharp
var document = Markdown.Parse(markdownText, pipeline);

// Render to HTML
var html = document.ToHtml(pipeline);

// Analyze the AST
var headings = document.Descendants<HeadingBlock>().ToList();
var links = document.Descendants<LinkInline>().Where(l => !l.IsImage).ToList();
```

### Extract metadata from the AST

```csharp
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);

// Get all headings
foreach (var heading in document.Descendants<HeadingBlock>())
{
    // Extract the heading text
    var text = heading.Inline?.FirstChild?.ToString();
    Console.WriteLine($"H{heading.Level}: {text}");
}

// Get all links
foreach (var link in document.Descendants<LinkInline>())
{
    Console.WriteLine($"Link: {link.Url} (image: {link.IsImage})");
}
```

### Modify the AST before rendering

```csharp
var document = Markdown.Parse(markdownText, pipeline);

// Add a CSS class to all tables
foreach (var table in document.Descendants<Table>())
{
    table.GetAttributes().AddClass("table table-striped");
}

// Render the modified document
var html = document.ToHtml(pipeline);
```

## Next steps

- [CommonMark syntax](commonmark.md) — Core Markdown syntax reference
- [Extensions](extensions/readme.md) — All built-in extensions
- [Developer guide](advanced/readme.md) — Create custom parsers and renderers
