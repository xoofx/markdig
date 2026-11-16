---
title: TOML front matter
---

# TOML front matter

Enable with `.UseTomlFrontMatter()` (not included in `UseAdvancedExtensions()`).

This extension parses TOML front matter blocks at the beginning of a document. The TOML content is parsed into the AST as a `TomlFrontMatterBlock` but is **not rendered** in the HTML output.

## Syntax

The TOML front matter is enclosed between `+++` delimiters at the very beginning of the document:

```markdown
+++
title = "My Document"
author = "John Doe"
+++

# My Document

Content starts here.
```

## Usage

```csharp
using Markdig;
using Markdig.Extensions.Toml;
using Markdig.Syntax;

var pipeline = new MarkdownPipelineBuilder()
    .UseTomlFrontMatter()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);

// The TOML front matter is in the AST but not rendered
var html = document.ToHtml(pipeline); // Front matter is excluded

// Access the TOML block from the AST
var tomlBlock = document.Descendants<TomlFrontMatterBlock>().FirstOrDefault();
if (tomlBlock != null)
{
    // Get the raw TOML content (you can then parse it with a TOML library)
    var toml = tomlBlock.Lines.ToString();
}
```

## Rules

- The `+++` opener must be the very first line of the document (no leading blank lines).
- The closing `+++` must appear on its own line.
- Only `+++` is accepted as a closing fence; `...` is YAML-only.
- Only one TOML front matter block is recognized per document.

## Processing the TOML content

Markdig only **parses** the TOML front matter — it does not evaluate it. To process the TOML content, use a TOML library such as [Tomlyn](https://github.com/xoofx/Tomlyn):

```csharp
using Tomlyn;
using Tomlyn.Model;

var tomlContent = tomlBlock.Lines.ToString();
var metadata = Toml.ToModel(tomlContent);
```

## Common use case

TOML front matter is used by static site generators such as [Hugo](https://gohugo.io/content-management/front-matter/#front-matter-formats). Markdig's TOML extension lets you preview such documents while stripping the metadata from the rendered output.