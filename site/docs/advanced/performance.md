---
title: Performance
---

# Performance

Markdig is designed for high throughput and low allocations. This page covers the patterns used internally and recommendations for extension authors.

## Allocation-free parsing

### StringSlice

The core parsing type is `StringSlice` — a struct that holds a reference to a `string` plus start/end indices:

```csharp
public struct StringSlice
{
    public string? Text;
    public int Start;
    public int End;

    public readonly char CurrentChar => Start <= End ? Text![Start] : '\0';
    public readonly int Length => End - Start + 1;
}
```

All parsers work on `StringSlice` rather than allocating substrings. When writing parsers, always use `StringSlice` methods:

{.table}
| Instead of | Use |
|---|---|
| `text.Substring(...)` | `slice.ToString()` (only when you need a string) |
| `text[i]` | `slice.PeekCharExtra(offset)` |
| `text.Trim()` | `slice.Trim()` (mutates the struct) |
| `text.IndexOf(c)` | `slice.IndexOf(c)` |

### ReadOnlySpan&lt;char&gt;

For hot paths, prefer `ReadOnlySpan<char>` over `string`:

```csharp
// Good — no allocation
ReadOnlySpan<char> span = slice.AsSpan();
if (span.StartsWith("```".AsSpan(), StringComparison.Ordinal))
{
    // ...
}

// Avoid — allocates a string
string text = slice.ToString();
if (text.StartsWith("```"))
{
    // ...
}
```

### stackalloc

For small temporary buffers, use `stackalloc`:

```csharp
Span<char> buffer = stackalloc char[64];
int written = FormatOutput(buffer);
renderer.Write(buffer[..written]);
```

### ArrayPool

For larger buffers, use `ArrayPool<T>`:

```csharp
using System.Buffers;

char[] buffer = ArrayPool<char>.Shared.Rent(1024);
try
{
    // Use buffer
}
finally
{
    ArrayPool<char>.Shared.Return(buffer);
}
```

## Pipeline and renderer reuse

### Build once, use many times

The `MarkdownPipeline` is immutable and thread-safe after `Build()`. Always cache it:

```csharp
// Good — build once, use across threads
private static readonly MarkdownPipeline Pipeline =
    new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

// Called from many threads
public string Convert(string markdown)
    => Markdown.ToHtml(markdown, Pipeline);
```

### Renderer pooling

`Markdown.ToHtml` and friends internally pool `HtmlRenderer` instances. If you create renderers manually, consider pooling them:

```csharp
// The static API handles pooling for you:
Markdown.ToHtml(text, pipeline);  // Preferred

// Only create renderers manually when you need to customize them
using var writer = new StringWriter();
var renderer = new HtmlRenderer(writer);
pipeline.Setup(renderer);
renderer.Render(document);
```

## Extension authoring tips

### Use sealed classes

Mark classes as `sealed` when they're not designed for inheritance. This allows the JIT to devirtualize method calls:

```csharp
// Good — allows devirtualization
public sealed class NoteBlock : LeafBlock
{
    public NoteBlock(BlockParser parser) : base(parser) { }
    public string? NoteType { get; set; }
}
```

### Prefer struct over class for small types

For data-only types that are short-lived, prefer `struct`:

```csharp
// Used only during parsing, never stored long-term
public readonly struct ParseResult
{
    public readonly bool Success;
    public readonly int EndPosition;
}
```

### Avoid LINQ in hot paths

LINQ allocates enumerator objects. In parser code, prefer `for`/`foreach` loops:

```csharp
// Avoid in parsers
var match = list.FirstOrDefault(x => x.Type == type);

// Prefer
MyType? match = null;
for (int i = 0; i < list.Count; i++)
{
    if (list[i].Type == type)
    {
        match = list[i];
        break;
    }
}
```

### Minimize string concatenation

Use `StringBuilder` or the renderer's built-in `Write` chaining:

```csharp
// Good — chained writes, no intermediate strings
renderer.Write("<div class=\"")
    .Write(cssClass)
    .Write("\">");

// Avoid — allocates intermediate strings
renderer.Write($"<div class=\"{cssClass}\">");
```

### Cache frequently used strings

For attribute names and CSS classes that repeat:

```csharp
public sealed class HtmlAlertRenderer : HtmlObjectRenderer<AlertBlock>
{
    // Cache the string to avoid repeated allocations
    private static readonly HtmlAttributes WarningAttributes = new()
    {
        Classes = new List<string> { "alert", "alert-warning" }
    };
}
```

## AOT and trimming compatibility

Markdig is designed to be compatible with Native AOT and IL trimming.

### Avoid reflection

Do not use `Type.GetMethod()`, `Activator.CreateInstance()`, or similar reflection APIs in parsers and renderers:

```csharp
// Bad — breaks trimming
var parser = (BlockParser)Activator.CreateInstance(parserType)!;

// Good — direct construction
var parser = new NoteBlockParser();
```

### Use source generators when applicable

For serialization scenarios, prefer source generators over reflection-based serialization:

```csharp
// Good — trimmer-friendly
[JsonSerializable(typeof(MarkdownMetadata))]
internal partial class MetadataJsonContext : JsonSerializerContext { }
```

### Annotate when reflection is unavoidable

If you must use reflection, annotate with `[DynamicallyAccessedMembers]`:

```csharp
public void RegisterParser(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    Type parserType)
{
    // ...
}
```

## Benchmarking

Markdig includes a benchmarks project for measuring performance:

```bash
cd src
dotnet run -c Release --project Markdig.Benchmarks
```

The benchmarks compare Markdig against other .NET Markdown processors using [BenchmarkDotNet](https://benchmarkdotnet.org/).

To benchmark your extension, add a test case to the benchmarks project:

```csharp
[Benchmark]
public string ConvertWithMyExtension()
{
    return Markdown.ToHtml(MarkdownText, _pipelineWithMyExtension);
}
```

## Summary of recommendations

{.table}
| Area | Recommendation |
|---|---|
| String handling | Use `StringSlice` and `ReadOnlySpan<char>`; avoid `Substring` |
| Buffers | `stackalloc` for small; `ArrayPool<T>` for large |
| Pipeline | Build once, pass everywhere, reuse across threads |
| Classes | `sealed` by default; `struct` for small data types |
| Loops | `for`/`foreach` over LINQ in parsers |
| Output | Chain `Write` calls; avoid string interpolation in renderers |
| AOT | No reflection; use source generators; annotate if unavoidable |
| Verify | Run benchmarks before/after changes |
