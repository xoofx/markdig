---
title: Creating extensions
---

# Creating extensions

Extensions are the primary mechanism for adding new features to Markdig. An extension can add new parsers, modify existing parsers, and register custom renderers.

## The IMarkdownExtension interface

Every extension implements `IMarkdownExtension`, which has two methods:

```csharp
public interface IMarkdownExtension
{
    void Setup(MarkdownPipelineBuilder pipeline);
    void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer);
}
```

- **`Setup(MarkdownPipelineBuilder)`** — Called when the pipeline is built. Register or modify block/inline parsers here.
- **`Setup(MarkdownPipeline, IMarkdownRenderer)`** — Called before rendering. Register object renderers here.

## Extension complexity spectrum

Extensions range from trivial to complex:

### Level 1: Modify an existing parser

The simplest extensions don't add new parsers at all — they just tweak existing ones.

**Example: CitationExtension** — Adds `""...""`  citations by configuring the existing `EmphasisInlineParser`:

```csharp
using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

public sealed class CitationExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Find the existing emphasis parser
        var emphasisParser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
        if (emphasisParser != null && !emphasisParser.HasEmphasisChar('"'))
        {
            // Add " as a 2-character emphasis delimiter: ""text""
            emphasisParser.EmphasisDescriptors.Add(
                new EmphasisDescriptor('"', 2, 2, false));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer) return;

        // Hook into the emphasis renderer to emit <cite> for ""...""
        var emphasisRenderer = renderer.ObjectRenderers.FindExact<EmphasisInlineRenderer>();
        if (emphasisRenderer == null) return;

        var previousTag = emphasisRenderer.GetTag;
        emphasisRenderer.GetTag = inline =>
            (inline.DelimiterCount == 2 && inline.DelimiterChar == '"' ? "cite" : null)
            ?? previousTag(inline);
    }
}
```

**Key pattern:** Reuse `EmphasisInlineParser` for delimiter-based inlines. Many extensions follow this approach.

### Level 2: Add a new inline parser + renderer

When you need custom inline syntax that doesn't fit the emphasis model.

**Example: TaskListExtension** — Adds `[ ]` / `[x]` checkbox parsing:

```csharp
public sealed class TaskListExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Insert the task list parser before the link parser
        if (!pipeline.InlineParsers.Contains<TaskListInlineParser>())
        {
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(
                new TaskListInlineParser());
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlTaskListRenderer>();
        }
    }
}
```

This extension needs:
- A custom `InlineParser` subclass (`TaskListInlineParser`)
- A custom AST node (`TaskList` inline)
- A custom `HtmlObjectRenderer<TaskList>` (`HtmlTaskListRenderer`)

### Level 3: Add a new block parser + renderer

When you need to parse block-level constructs.

**Example: CustomContainerExtension** — Adds `:::` fenced containers:

```csharp
public sealed class CustomContainerExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Add the block parser at position 0 (high priority)
        if (!pipeline.BlockParsers.Contains<CustomContainerParser>())
        {
            pipeline.BlockParsers.Insert(0, new CustomContainerParser());
        }

        // Also add inline container support (::text::)
        var emphasisParser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
        if (emphasisParser != null && !emphasisParser.HasEmphasisChar(':'))
        {
            emphasisParser.EmphasisDescriptors.Add(
                new EmphasisDescriptor(':', 2, 2, false));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlCustomContainerRenderer>();
            htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlCustomContainerInlineRenderer>();
        }
    }
}
```

### Level 4: Complex extension with multiple parsers and ordering

Some extensions (like `FootnoteExtension`) add multiple block parsers, inline parsers, and renderers, and need specific ordering relative to other extensions. These are more complex but follow the same fundamental patterns.

## Registering your extension

### Option A: Generic Use method

For extensions with a parameterless constructor:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .Use<MyExtension>()
    .Build();
```

### Option B: Instance method

For extensions that need configuration:

```csharp
var ext = new MyExtension(someConfig);
var pipeline = new MarkdownPipelineBuilder()
    .Use(ext)
    .Build();
```

### Option C: Custom fluent extension method (recommended)

Create an extension method on `MarkdownPipelineBuilder` for a clean API:

```csharp
public static class MyExtensionMethods
{
    public static MarkdownPipelineBuilder UseMyExtension(
        this MarkdownPipelineBuilder pipeline,
        MyExtensionOptions? options = null)
    {
        pipeline.Extensions.ReplaceOrAdd<MyExtension>(
            new MyExtension(options));
        return pipeline;
    }
}
```

Usage:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseMyExtension(new MyExtensionOptions { /* ... */ })
    .Build();
```

## Complete example: a blink extension

Here's a complete, silly extension that turns `%%%text%%%` into `<blink>text</blink>`:

```csharp
using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

/// <summary>
/// Extension that converts %%%text%%% to &lt;blink&gt; tags.
/// </summary>
public sealed class BlinkExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        var parser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
        if (parser != null && !parser.HasEmphasisChar('%'))
        {
            // 3 consecutive % on each side
            parser.EmphasisDescriptors.Add(new EmphasisDescriptor('%', 3, 3, false));
        }
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer) return;

        var emphasisRenderer = renderer.ObjectRenderers.FindExact<EmphasisInlineRenderer>();
        if (emphasisRenderer == null) return;

        var previousTag = emphasisRenderer.GetTag;
        emphasisRenderer.GetTag = inline =>
            (inline.DelimiterCount == 3 && inline.DelimiterChar == '%'
                ? "blink"
                : null)
            ?? previousTag(inline);
    }
}

// Fluent API extension method
public static class BlinkExtensionMethods
{
    public static MarkdownPipelineBuilder UseBlink(
        this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready<BlinkExtension>();
        return pipeline;
    }
}
```

Usage:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseBlink()
    .Build();

var html = Markdown.ToHtml("This is %%%blinking%%% text.", pipeline);
// => <p>This is <blink>blinking</blink> text.</p>
```

## Next steps

- [Block parsers](block-parsers.md) — How to write custom block parsers from scratch
- [Inline parsers](inline-parsers.md) — How to write custom inline parsers
- [Renderers](renderers.md) — How to create HTML or custom renderers
