---
title: Getting started
---

# Getting started

This guide walks you through installing Markdig, converting your first Markdown string to HTML, and configuring the pipeline for extended features.

## Installation

Install the [Markdig NuGet package](https://www.nuget.org/packages/Markdig/):

```shell
dotnet add package Markdig
```

Or via the Package Manager Console:

```powershell
Install-Package Markdig
```

A [strong-named variant](https://www.nuget.org/packages/Markdig.Signed/) is also available:

```shell
dotnet add package Markdig.Signed
```

### Requirements

Markdig targets `net462`, `netstandard2.0`, `netstandard2.1`, `net8.0`, and `net10.0`. It works with .NET Framework 4.6.2+, .NET Core 2.0+, and .NET 5+.

## Your first conversion

The main entry point is the static `Markdown` class in the `Markdig` namespace. The simplest operation converts a Markdown string to HTML:

```csharp
using Markdig;

var html = Markdown.ToHtml("Hello **Markdig**!");
Console.WriteLine(html);
// Output: <p>Hello <strong>Markdig</strong>!</p>
```

By default, Markdig uses a plain **CommonMark** parser — no extensions are enabled.

## Enabling extensions

Most projects benefit from Markdig's rich set of extensions. Use `MarkdownPipelineBuilder` to configure a **pipeline**, then pass it to `Markdown.ToHtml`:

```csharp
using Markdig;

// Build a pipeline with all advanced extensions
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var html = Markdown.ToHtml("This is ~~deleted~~ text.", pipeline);
Console.WriteLine(html);
// Output: <p>This is <del>deleted</del> text.</p>
```

`UseAdvancedExtensions()` activates most extensions at once (tables, task lists, math, footnotes, diagrams, and more). See the [Extensions](extensions/readme.md) section for the full list and individual activation.

You can also enable specific extensions individually:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables()
    .UseFootnotes()
    .UseEmphasisExtras()
    .Build();
```

## Parsing to an AST

If you need to inspect or manipulate the document structure, parse into an abstract syntax tree (AST):

```csharp
using Markdig;
using Markdig.Syntax;

var document = Markdown.Parse("# Hello\n\nA paragraph with **bold** text.");

// Iterate all descendants
foreach (var node in document.Descendants())
{
    Console.WriteLine(node.GetType().Name);
}

// Find specific node types
foreach (var heading in document.Descendants<HeadingBlock>())
{
    Console.WriteLine($"Heading level {heading.Level}");
}
```

The `MarkdownDocument` returned by `Markdown.Parse(...)` is the root of a tree of `Block` and `Inline` nodes. See the [AST guide](advanced/ast.md) for more details.

## Rendering a parsed document

After parsing, you can render the document to HTML separately:

```csharp
using Markdig;
using Markdig.Syntax;

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

// Step 1: Parse
var document = Markdown.Parse("A ~~strikethrough~~ example.", pipeline);

// Step 2: Render
var html = document.ToHtml(pipeline);
Console.WriteLine(html);
```

> **Important:** Always pass the **same pipeline** to both `Parse` and `ToHtml` (or `Convert`). The pipeline configures both parser extensions (which produce custom AST nodes) and renderer extensions (which know how to render those nodes). Using mismatched pipelines results in missing or incorrect output. See [Usage](usage.md) for a detailed explanation.

## Converting to plain text

Markdig can also convert Markdown to plain text (all HTML tags and formatting stripped):

```csharp
var text = Markdown.ToPlainText("Hello **world**!");
Console.WriteLine(text);
// Output: Hello world!
```

## Using a custom renderer

For output formats other than HTML (e.g. LaTeX, XAML), use `Markdown.Convert`:

```csharp
var document = Markdown.Convert(markdownText, myCustomRenderer, pipeline);
```

See the [Renderers](advanced/renderers.md) guide for details on creating custom renderers.

## Next steps

- [Usage guide](usage.md) — Understand pipeline architecture, Parse+Render separation, and common patterns
- [CommonMark syntax](commonmark.md) — Reference for all core Markdown syntax
- [Extensions](extensions/readme.md) — Discover all 20+ built-in extensions
- [Developer guide](advanced/readme.md) — Create your own parsers, renderers, and extensions
