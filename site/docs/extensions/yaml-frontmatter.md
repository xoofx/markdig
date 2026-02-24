---
title: YAML front matter
---

# YAML front matter

Enable with `.UseYamlFrontMatter()` (not included in `UseAdvancedExtensions()`).

This extension parses YAML front matter blocks at the beginning of a document. The YAML content is parsed into the AST as a `YamlFrontMatterBlock` but is **not rendered** in the HTML output.

## Syntax

The YAML front matter is enclosed between `---` delimiters at the very beginning of the document:

```markdown
---
title: My Document
author: John Doe
date: 2025-01-15
tags:
  - markdown
  - documentation
---

# My Document

Content starts here.
```

## Usage

```csharp
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;

var pipeline = new MarkdownPipelineBuilder()
    .UseYamlFrontMatter()
    .Build();

var document = Markdown.Parse(markdownText, pipeline);

// The YAML front matter is in the AST but not rendered
var html = document.ToHtml(pipeline); // Front matter is excluded

// Access the YAML block from the AST
var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
if (yamlBlock != null)
{
    // Get the raw YAML content (you can then parse it with a YAML library)
    var yaml = yamlBlock.Lines.ToString();
}
```

## Rules

- The `---` opener must be the very first line of the document (no leading blank lines).
- The closing `---` must appear on its own line.
- Only one YAML front matter block is recognized per document.

## Processing the YAML content

Markdig only **parses** the YAML front matter — it does not evaluate it. To process the YAML content, use a YAML library such as [YamlDotNet](https://github.com/aaubry/YamlDotNet):

```csharp
using YamlDotNet.Serialization;

var yamlContent = yamlBlock.Lines.ToString();
var deserializer = new DeserializerBuilder().Build();
var metadata = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
```

## Common use case

YAML front matter is widely used in static site generators (Hugo, Jekyll, Lunet) to store document metadata like title, date, author, and tags. Markdig's YAML extension lets you preview such documents while stripping the metadata from the rendered output.
