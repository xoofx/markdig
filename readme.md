# Markdig [![ci](https://github.com/xoofx/markdig/actions/workflows/ci.yml/badge.svg)](https://github.com/xoofx/markdig/actions/workflows/ci.yml) [![Coverage Status](https://coveralls.io/repos/github/xoofx/markdig/badge.svg?branch=master)](https://coveralls.io/github/xoofx/markdig?branch=master) [![NuGet](https://img.shields.io/nuget/v/Markdig.svg)](https://www.nuget.org/packages/Markdig/) [![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=FRGHXBTP442JL)

<img align="right" width="160px" height="160px" src="img/markdig.png">

Markdig is a fast, powerful, [CommonMark](http://commonmark.org/) compliant, extensible Markdown processor for .NET.

> NOTE: The repository is under construction. There will be a dedicated website and proper documentation at some point!

You can **try Markdig online** and compare it to other implementations on [babelmark3](https://babelmark.github.io/?text=Hello+**Markdig**!)

## Features

- **Very fast parser and html renderer** (no-regexp), very lightweight in terms of GC pressure. See benchmarks
- **Abstract Syntax Tree** with precise source code location for syntax tree, useful when building a Markdown editor.
  - Checkout [Markdown Editor v2 for Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor2) powered by Markdig!
- Converter to **HTML**
- Passing more than **600+ tests** from the latest [CommonMark specs (0.31.2)](http://spec.commonmark.org/)
- Includes all the core elements of CommonMark:
  - including **GFM fenced code blocks**.  
- **Extensible** architecture
  - Even the core Markdown/CommonMark parsing is pluggable, so it allows to disable builtin Markdown/Commonmark parsing (e.g [Disable HTML parsing](https://github.com/lunet-io/markdig/blob/7964bd0160d4c18e4155127a4c863d61ebd8944a/src/Markdig/MarkdownExtensions.cs#L306)) or change behaviour (e.g change matching `#` of a headers with `@`)   
- [**Roundtrip support**](./src/Markdig/Roundtrip.md): Parses trivia (whitespace, newlines and other characters) to support lossless parse ⭢ render roundtrip. This enables changing markdown documents without introducing undesired trivia changes.
- Built-in with **20+ extensions**, including:
  - 2 kind of tables:
    - [**Pipe tables**](src/Markdig.Tests/Specs/PipeTableSpecs.md) (inspired from GitHub tables and [PanDoc - Pipe Tables](http://pandoc.org/README.html#extension-pipe_tables))
    - [**Grid tables**](src/Markdig.Tests/Specs/GridTableSpecs.md) (inspired from [Pandoc - Grid Tables](http://pandoc.org/README.html#extension-grid_tables)) 
  - [**Extra emphasis**](src/Markdig.Tests/Specs/EmphasisExtraSpecs.md) (inspired from [Pandoc - Emphasis](http://pandoc.org/README.html#strikeout) and [Markdown-it](https://markdown-it.github.io/)) 
    - strike through `~~`,
    - Subscript `~`
    - Superscript `^` 
    - Inserted `++`
    - Marked `==`
  - [**Special attributes**](src/Markdig.Tests/Specs/GenericAttributesSpecs.md) or attached HTML attributes (inspired from [PHP Markdown Extra - Special Attributes](https://michelf.ca/projects/php-markdown/extra/#spe-attr))
  - [**Definition lists**](src/Markdig.Tests/Specs/DefinitionListSpecs.md) (inspired from [PHP Markdown Extra - Definitions Lists](https://michelf.ca/projects/php-markdown/extra/#def-list))
  - [**Footnotes**](src/Markdig.Tests/Specs/FootnotesSpecs.md) (inspired from [PHP Markdown Extra - Footnotes](https://michelf.ca/projects/php-markdown/extra/#footnotes))
  - [**Auto-identifiers**](src/Markdig.Tests/Specs/AutoIdentifierSpecs.md) for headings (similar to [Pandoc - Auto Identifiers](http://pandoc.org/README.html#extension-auto_identifiers))
  - [**Auto-links**](src/Markdig.Tests/Specs/AutoLinks.md) generates links if a text starts with `http://` or `https://` or `ftp://` or `mailto:` or `www.xxx.yyy`
  - [**Task Lists**](src/Markdig.Tests/Specs/TaskListSpecs.md)  inspired from [Github Task lists](https://github.com/blog/1375-task-lists-in-gfm-issues-pulls-comments).
  - [**Extra bullet lists**](src/Markdig.Tests/Specs/ListExtraSpecs.md), supporting alpha bullet `a.` `b.` and roman bullet (`i`, `ii`...etc.)
  - [**Media support**](src/Markdig.Tests/Specs/MediaSpecs.md) for media url (youtube, vimeo, mp4...etc.) (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/embedded-audio-and-video/441))
  - [**Abbreviations**](src/Markdig.Tests/Specs/AbbreviationSpecs.md) (inspired from [PHP Markdown Extra - Abbreviations](https://michelf.ca/projects/php-markdown/extra/#abbr))
  - [**Citation**](src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md) text by enclosing `""...""` (inspired by this [CommonMark discussion ](https://talk.commonmark.org/t/referencing-creative-works-with-cite/892))
  - [**Custom containers**](src/Markdig.Tests/Specs/CustomContainerSpecs.md) similar to fenced code block `:::` for generating a proper `<div>...</div>` instead (inspired by this [CommonMark discussion ](https://talk.commonmark.org/t/custom-container-for-block-and-inline/2051))
  - [**Figures**](src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md) (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/image-tag-should-expand-to-figure-when-used-with-title/265/5))
  - [**Footers**](src/Markdig.Tests/Specs/FigureFooterAndCiteSpecs.md) (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/syntax-for-footer/2070))
  - [**Mathematics**](src/Markdig.Tests/Specs/MathSpecs.md)/Latex extension by enclosing `$$` for block and `$` for inline math (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/mathematics-extension/457/31))
  - [**Soft lines as hard lines**](src/Markdig.Tests/Specs/HardlineBreakSpecs.md)
  - [**Emoji**](src/Markdig.Tests/Specs/EmojiSpecs.md) support (inspired from [Markdown-it](https://markdown-it.github.io/))
  - [**SmartyPants**](src/Markdig.Tests/Specs/SmartyPantsSpecs.md) (inspired from [Daring Fireball - SmartyPants](https://daringfireball.net/projects/smartypants/))
  - [**Bootstrap**](src/Markdig.Tests/Specs/BootstrapSpecs.md) class (to output bootstrap class)
  - [**Diagrams**](src/Markdig.Tests/Specs/DiagramsSpecs.md) extension whenever a fenced code block contains a special keyword, it will be converted to a div block with the content as-is (currently, supports [`mermaid`](https://mermaid.js.org) and [`nomnoml`](https://github.com/skanaar/nomnoml) diagrams)
  - [**YAML Front Matter**](src/Markdig.Tests/Specs/YamlSpecs.md) to parse without evaluating the front matter and to discard it from the HTML output (typically used for previewing without the front matter in MarkdownEditor)
  - [**JIRA links**](src/Markdig.Tests/Specs/JiraLinks.md) to automatically generate links for JIRA project references (Thanks to @clarkd: https://github.com/clarkd/MarkdigJiraLinker)
- Starting with Markdig version `0.20.0+`, Markdig is compatible only with `NETStandard 2.0`, `NETStandard 2.1`, `NETCoreApp 2.1` and `NETCoreApp 3.1`.

If you are looking for support for an old .NET Framework 3.5 or 4.0, you can download Markdig `0.18.3`.
	
### Third Party Extensions

- [**WPF/XAML Markdown Renderer**: `markdig.wpf`](https://github.com/Kryptos-FR/markdig.wpf)
- [**WPF/XAML Markdown Renderer**: `Neo.Markdig.Xaml`](https://github.com/neolithos/NeoMarkdigXaml)
- [**Syntax highlighting**: `Markdig.SyntaxHighlighting`](https://github.com/RichardSlater/Markdig.SyntaxHighlighting)
- [**Syntax highlighting using ColorCode-Universal**: `Markdown.ColorCode`](https://github.com/wbaldoumas/markdown-colorcode)
- [**Syntax highlighting using Prism.js**: `WebStoating.Markdig.Prism`](https://github.com/ilich/Markdig.Prism)
- [**Embedded C# scripting**: `Markdig.Extensions.ScriptCs`](https://github.com/macaba/Markdig.Extensions.ScriptCs)

## Documentation

> The repository is under construction. There will be a dedicated website and proper documentation at some point!

While there is not yet a dedicated documentation, you can find from the [specs documentation](src/Markdig.Tests/Specs/readme.md) how to use these extensions.

In the meantime, you can have a "behind the scene" article about Markdig in my blog post ["Implementing a Markdown Engine for .NET"](http://xoofx.github.io/blog/2016/06/13/implementing-a-markdown-processor-for-dotnet/)

## Download

Markdig is available as a NuGet package: [![NuGet](https://img.shields.io/nuget/v/Markdig.svg)](https://www.nuget.org/packages/Markdig/)

Also [Markdig.Signed](https://www.nuget.org/packages/Markdig.Signed/) NuGet package provides signed assemblies.

## Usage

The main entry point for the API is the `Markdig.Markdown` class:

By default, without any options, Markdig is using the plain CommonMark parser:

```csharp
var result = Markdown.ToHtml("This is a text with some *emphasis*");
Console.WriteLine(result);   // prints: <p>This is a text with some <em>emphasis</em></p>
```

In order to activate most of all advanced extensions (except Emoji, SoftLine as HardLine, Bootstrap, YAML Front Matter, JiraLinks and SmartyPants)

```csharp
// Configure the pipeline with all advanced extensions active
var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
var result = Markdown.ToHtml("This is a text with some *emphasis*", pipeline);
```

[Try it online!](https://dotnetfiddle.net/GoZXyI)

You can have a look at the [MarkdownExtensions](https://github.com/lunet-io/markdig/blob/master/src/Markdig/MarkdownExtensions.cs) that describes all actionable extensions (by modifying the MarkdownPipeline)

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are greatly appreciated. For detailed contributing guidelines, please see [contributing.md](contributing.md).

## Build

In order to build Markdig, you need to install [.NET 6.0](https://dotnet.microsoft.com/en-us/download)

## License

This software is released under the [BSD-Clause 2 license](https://github.com/lunet-io/markdig/blob/master/license.txt).


## Benchmarking

The latest benchmark was collected on April 23 2022, against the following implementations:

- [Markdig](https://github.com/lunet-io/markdig) (version: 0.30.2): itself
- [cmark](https://github.com/commonmark/cmark) (version: 0.30.2): Reference C implementation of CommonMark, no support for extensions
- [CommonMark.NET(master)](https://github.com/Knagis/CommonMark.NET) (version: 0.15.1): CommonMark implementation for .NET, no support for extensions, port of cmark, deprecated.
- [MarkdownSharp](https://github.com/Kiri-rin/markdownsharp) (version: 2.0.5): Open source C# implementation of Markdown processor, as featured previously on Stack Overflow, regexp based.

```
// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=6.0.202
  [Host]     : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  DefaultJob : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT


|            Method |       Mean |     Error |    StdDev |
|------------------ |-----------:|----------:|----------:|
|           markdig |   1.979 ms | 0.0221 ms | 0.0185 ms |
|             cmark |   2.571 ms | 0.0081 ms | 0.0076 ms |
|    CommonMark.NET |   2.016 ms | 0.0169 ms | 0.0158 ms |
|     MarkdownSharp | 221.455 ms | 1.4442 ms | 1.3509 ms |
```

- Markdig is roughly **x100 times faster than MarkdownSharp**
- **20% faster than the reference cmark C implementation** 

## Sponsors

Supports this project with a monthly donation and help me continue improving it. \[[Become a sponsor](https://github.com/sponsors/xoofx)\]

[<img src="https://github.com/lilith.png?size=200" width="64px;" style="border-radius: 50%" alt="lilith"/>](https://github.com/lilith) Lilith River, author of [Imageflow Server, an easy on-demand
image editing, optimization, and delivery server](https://github.com/imazen/imageflow-server)

## Credits

Thanks to the fantastic work done by [John Mac Farlane](http://johnmacfarlane.net/) for the CommonMark specs and all the people involved in making Markdown a better standard!

This project would not have been possible without this huge foundation.

Thanks also to the project [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet) that makes benchmarking so easy to setup!

Some decoding part (e.g HTML [EntityHelper.cs](https://github.com/lunet-io/markdig/blob/master/src/Markdig/Helpers/EntityHelper.cs)) have been re-used from [CommonMark.NET](https://github.com/Knagis/CommonMark.NET)

Thanks to the work done by @clarkd on the JIRA Link extension (https://github.com/clarkd/MarkdigJiraLinker), now included with this project!
## Author

Alexandre MUTEL aka [xoofx](http://xoofx.github.io)
