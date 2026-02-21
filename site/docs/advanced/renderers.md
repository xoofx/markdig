---
title: Renderers
---

# Renderers

Renderers walk the AST and produce output. Markdig ships with an `HtmlRenderer` (HTML output), a `NormalizeRenderer` (canonical Markdown output), and supports fully custom renderers for any output format.

## How rendering works

When you call `document.ToHtml(pipeline)`:

1. An `HtmlRenderer` is created (internally pooled for performance).
2. `pipeline.Setup(renderer)` is called — each extension's `Setup(MarkdownPipeline, IMarkdownRenderer)` runs, registering per-type `ObjectRenderers`.
3. The renderer walks the AST depth-first, dispatching each node to the `ObjectRenderer` registered for its runtime type.
4. Output is written to the underlying `TextWriter`.

## The IMarkdownRenderer interface

```csharp
public interface IMarkdownRenderer
{
    event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteBefore;
    event Action<IMarkdownRenderer, MarkdownObject> ObjectWriteAfter;
    ObjectRendererCollection ObjectRenderers { get; }
    object Render(MarkdownObject markdownObject);
}
```

## HTML renderers

### HtmlObjectRenderer&lt;T&gt;

The most common way to render custom AST nodes to HTML is to create a class inheriting from `HtmlObjectRenderer<T>`:

```csharp
using Markdig.Renderers;
using Markdig.Renderers.Html;

public class HtmlNoteBlockRenderer : HtmlObjectRenderer<NoteBlock>
{
    protected override void Write(HtmlRenderer renderer, NoteBlock obj)
    {
        // Open the div with attributes from the AST node
        renderer.Write("<div class=\"note note-")
            .Write(obj.NoteType ?? "info")
            .Write("\"");

        // Write any HTML attributes attached to the node
        renderer.WriteAttributes(obj);
        renderer.WriteLine(">");

        // Write the title
        if (!string.IsNullOrEmpty(obj.Title))
        {
            renderer.Write("<p class=\"note-title\">")
                .WriteEscape(obj.Title)
                .WriteLine("</p>");
        }

        // Write inline content (for LeafBlocks)
        renderer.WriteLeafInline(obj);

        renderer.WriteLine("</div>");
    }
}
```

The generic parameter `T` automatically registers this renderer for all `NoteBlock` nodes.

### HtmlRenderer write methods

The `HtmlRenderer` provides these commonly used methods:

{.table}
| Method | Description |
|---|---|
| `Write(string)` | Write raw text |
| `Write(char)` | Write a single character |
| `WriteEscape(string)` | Write HTML-escaped text |
| `WriteEscape(StringSlice)` | Write HTML-escaped slice |
| `WriteLeafInline(LeafBlock)` | Render all inlines of a leaf block |
| `WriteLeafRawLines(LeafBlock)` | Write raw line content (for code blocks) |
| `WriteAttributes(MarkdownObject)` | Write attached HTML attributes |
| `WriteLine()` | Write a newline |
| `EnsureLine()` | Write a newline only if not already at line start |
| `PushIndent(string)` | Push indentation for nested content |
| `PopIndent()` | Pop indentation |

### Writing container blocks

For custom `ContainerBlock` nodes, render children with `WriteChildren`:

```csharp
public class HtmlMyContainerRenderer : HtmlObjectRenderer<MyContainerBlock>
{
    protected override void Write(HtmlRenderer renderer, MyContainerBlock obj)
    {
        renderer.Write("<div class=\"my-container\"");
        renderer.WriteAttributes(obj);
        renderer.WriteLine(">");

        // Render all child blocks
        renderer.WriteChildren(obj);

        renderer.WriteLine("</div>");
    }
}
```

### Writing inline renderers

For custom `Inline` nodes:

```csharp
public class HtmlKeyboardRenderer : HtmlObjectRenderer<KeyboardInline>
{
    protected override void Write(HtmlRenderer renderer, KeyboardInline obj)
    {
        renderer.Write("<kbd");
        renderer.WriteAttributes(obj);
        renderer.Write(">");
        renderer.WriteEscape(obj.Shortcut ?? "");
        renderer.Write("</kbd>");
    }
}
```

### Rendering container inlines

For `ContainerInline` types, render children inline:

```csharp
public class HtmlHighlightRenderer : HtmlObjectRenderer<HighlightInline>
{
    protected override void Write(HtmlRenderer renderer, HighlightInline obj)
    {
        renderer.Write("<mark");
        renderer.WriteAttributes(obj);
        renderer.Write(">");

        // Render child inlines
        renderer.WriteChildren(obj);

        renderer.Write("</mark>");
    }
}
```

## Registering renderers

Renderers are registered in the extension's `Setup(MarkdownPipeline, IMarkdownRenderer)` method:

```csharp
public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
{
    if (renderer is HtmlRenderer htmlRenderer)
    {
        // Add if not already present
        htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlNoteBlockRenderer>();
        htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlKeyboardRenderer>();
    }
}
```

### Insertion ordering

Like parsers, renderers can be inserted at specific positions:

```csharp
// Insert at the beginning (highest priority)
htmlRenderer.ObjectRenderers.Insert(0, new HtmlNoteBlockRenderer());

// Insert before a specific type
htmlRenderer.ObjectRenderers.InsertBefore<HtmlCodeBlockRenderer>(
    new HtmlNoteBlockRenderer());
```

## Modifying existing renderers

You can modify built-in renderers without replacing them:

```csharp
public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
{
    if (renderer is HtmlRenderer htmlRenderer)
    {
        // Modify the CodeBlockRenderer to output divs for specific languages
        var codeRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
        if (codeRenderer != null)
        {
            codeRenderer.BlocksAsDiv.Add("mermaid");
            codeRenderer.BlocksAsDiv.Add("nomnoml");
        }
    }
}
```

## HtmlRenderer properties

The `HtmlRenderer` has several useful properties:

{.table}
| Property | Type | Default | Description |
|---|---|---|---|
| `EnableHtmlForInline` | `bool` | `true` | Emit HTML tags for inlines |
| `EnableHtmlForBlock` | `bool` | `true` | Emit HTML tags for blocks |
| `EnableHtmlEscape` | `bool` | `true` | HTML-escape special characters |

When all three are `false`, the renderer produces **plain text** — this is how `Markdown.ToPlainText` works.

## Events: Before and After

You can hook into the rendering pipeline with events:

```csharp
renderer.ObjectWriteBefore += (r, obj) =>
{
    if (obj is HeadingBlock heading)
    {
        Console.WriteLine($"About to render H{heading.Level}");
    }
};

renderer.ObjectWriteAfter += (r, obj) =>
{
    // Post-processing after each node is rendered
};
```

## Building a completely custom renderer

For non-HTML output (LaTeX, XAML, JSON, etc.), you can implement a full custom renderer:

### Option 1: Inherit TextRendererBase

For text-based output formats:

```csharp
using Markdig.Renderers;

public class LatexRenderer : TextRendererBase<LatexRenderer>
{
    public LatexRenderer(TextWriter writer) : base(writer)
    {
        // Register per-type renderers
        ObjectRenderers.Add(new LatexHeadingRenderer());
        ObjectRenderers.Add(new LatexParagraphRenderer());
        ObjectRenderers.Add(new LatexCodeBlockRenderer());
        // ... register all needed renderers
    }
}

// Per-type renderer
public class LatexHeadingRenderer : MarkdownObjectRenderer<LatexRenderer, HeadingBlock>
{
    protected override void Write(LatexRenderer renderer, HeadingBlock obj)
    {
        var command = obj.Level switch
        {
            1 => "section",
            2 => "subsection",
            3 => "subsubsection",
            _ => "paragraph"
        };

        renderer.Write($"\\{command}{{ "{{" }}");
        renderer.WriteLeafInline(obj);
        renderer.WriteLine("}");
    }
}
```

### Option 2: Implement IMarkdownRenderer directly

For fully custom output (JSON, binary, etc.):

```csharp
public class JsonRenderer : IMarkdownRenderer
{
    public event Action<IMarkdownRenderer, MarkdownObject>? ObjectWriteBefore;
    public event Action<IMarkdownRenderer, MarkdownObject>? ObjectWriteAfter;
    public ObjectRendererCollection ObjectRenderers { get; } = new();

    public object Render(MarkdownObject markdownObject)
    {
        // Walk the AST and produce JSON
        // ...
        return jsonBuilder.ToString();
    }
}
```

### Using custom renderers

Pass your renderer to `Markdown.Convert`:

```csharp
var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
var renderer = new LatexRenderer(writer);

// Setup extensions for the custom renderer
pipeline.Setup(renderer);

// Render
Markdown.Convert(markdownText, renderer, pipeline);
```

## Complete example: rendering NoteBlock

Putting it all together — the block parser, renderer, and extension:

```csharp
// Extension
public sealed class NoteExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.BlockParsers.AddIfNotAlready<NoteBlockParser>();
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer htmlRenderer)
        {
            htmlRenderer.ObjectRenderers.AddIfNotAlready<HtmlNoteBlockRenderer>();
        }
    }
}

// Fluent API
public static class NoteExtensionMethods
{
    public static MarkdownPipelineBuilder UseNotes(
        this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready<NoteExtension>();
        return pipeline;
    }
}
```

Usage:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseNotes()
    .Build();

var html = Markdown.ToHtml("!!! warning \"Be careful\"\n", pipeline);
```

## Next steps

- [Block parsers](block-parsers.md) — Write the parsers that produce custom AST nodes
- [Inline parsers](inline-parsers.md) — Write inline-level parsers
- [Performance](performance.md) — Optimize rendering performance
