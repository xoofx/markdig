# Markdig  [![NuGet](https://img.shields.io/nuget/v/Markdig.svg)](https://www.nuget.org/packages/Markdig/)

Markdig is a fast, powerfull, [CommonMark](http://commonmark.org/) compliant, extensible Markdown processor for .NET.

> NOTE: The repository is under construction. There will be a dedicated website and proper documentation at some point!

## Features

- **Very fast parser** (no-regexp), very lightweight in terms of GC pressure. See benchmarks
- **Abstract Syntax Tree**
- Converter to **HTML**
- Passing more than **600+ tests** from the latest [CommonMark specs](http://spec.commonmark.org/)
- Includes all the core elements of CommonMark:
  - including GFM fenced code blocks. 
- **Extensible** architecture
  - Even the core Markdown/CommonMark parsing is pluggable, so it allows to disable builtin Markdown/Commonmark parsing (e.g [Disable HTML parsing](https://github.com/lunet-io/markdig/blob/7964bd0160d4c18e4155127a4c863d61ebd8944a/src/Markdig/MarkdownExtensions.cs#L306)) or change behaviour (e.g change matching `#` of a headers with `@`)   
- Built-in with **18+ extensions**, including:
  - **Abbreviations** (inspired from [PHP Markdown Extra - Abbreviations](https://michelf.ca/projects/php-markdown/extra/#abbr))
  - **Auto-identifiers** for headings (similar to [Pandoc](http://pandoc.org/README.html#extension-auto_identifiers)
  - **Bootstrap** class (to output bootstrap class)
  - **Citation** text by enclosing `""...""` (inspired by this [CommonMark discussion ](https://talk.commonmark.org/t/referencing-creative-works-with-cite/892))
  - **Custom containers** similar to fenced code block `:::` for generating a proper `<div>...</div>` instead (inspired by this [CommonMark discussion ](https://talk.commonmark.org/t/custom-container-for-block-and-inline/2051))
  - **Definition lists** (inspired from [PHP Markdown Extra - Definitions Lists](https://michelf.ca/projects/php-markdown/extra/#def-list))
  - **Emoji** support (inspired from [Markdown-it](https://markdown-it.github.io/))
  - **Extra emphasis** (inspired from [Pandoc](http://pandoc.org/README.html#strikeout) and [Markdown-it](https://markdown-it.github.io/)) 
    - strike through `~~`,
    - Subscript `~`
    - Superscript `^` 
    - Inserted `++`
    - Marked `==`
  - **Figures** (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/image-tag-should-expand-to-figure-when-used-with-title/265/5))
  - **Footers** (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/syntax-for-footer/2070))
  - **Footnotes** (inspired from [PHP Markdown Extra - Footnotes](https://michelf.ca/projects/php-markdown/extra/#footnotes))
  - **Special attributes** or attached HTML attributes (inspired from [PHP Markdown Extra - Footnotes](https://michelf.ca/projects/php-markdown/extra/#spe-attr))
  - **Soft lines as hard lines**
  - **Extra bullet lists**, supporting alpha bullet `a.` `b.` and roman bullet (`i`, `ii`...etc.)
  - **Mathematics**/Latex extension by enclosing `$$` for block and `$` for inline math (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/mathematics-extension/457/31))
  - **Embed player** for media url (youtube, vimeo, mp4...etc.) (inspired from this [CommonMark discussion](https://talk.commonmark.org/t/embedded-audio-and-video/441))
  - **SmartyPants** (inspired from [Daring Fireball - SmartyPants](https://daringfireball.net/projects/smartypants/))
  - Tables:
    - **Pipe tables** (inspired from Kramdown/[PanDoc](http://pandoc.org/README.html#pipe_tables))
    - **Grid tables** (inspired from [Pandoc](http://pandoc.org/README.html#grid_tables)) 
- Compatible with .NET 3.5, 4.0+ and .NET Core (`netstandard1.1+`)
	
## Download

Markdig is available as a NuGet package: [![NuGet](https://img.shields.io/nuget/v/Markdig.svg)](https://www.nuget.org/packages/Markdig/)

## Usage

The main entry point for the API is the `Markdown` class:

By default, without any options, Markdig is using the plain CommonMark parser:

```csharp
var result = Markdown.ToHtml("This is a text with some **emphasis**");
Console.WriteLine(result);   // prints: <p>This is a text with some <em>emphasis</em></p>
```

In order to activate all extensions (except Emoji)

```csharp
var result = Markdown.ToHtml("This is a text with some **emphasis**", new MarkdownPipeline().UseAllExtensions());
```

You can have a look at the [MarkdownExtensions](https://github.com/lunet-io/markdig/blob/master/src/Markdig/MarkdownExtensions.cs) that describes all actionable extensions (by modifying the MarkdownPipeline)

## License

This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause).


## Benchmarking

This is an early preview of the benchmarking against various implementations:

- Markdig: itself
- CommonMarkCpp: [cmark](https://github.com/jgm/cmark), Reference C implementation of CommonMark 
- [CommonMark.NET](https://github.com/Knagis/CommonMark.NET): CommonMark implementation for .NET, not extensible, port of cmark
  - [CommonMarkNet (devel)](https://github.com/AMDL/CommonMark.NET/tree/pipe-tables): An evolution of CommonMark.NET that is extensible, not released yet
- [MarkdownDeep](https://github.com/toptensoftware/markdowndeep) another .NET implementation
- [MarkdownSharp](https://github.com/Kiri-rin/markdownsharp): Open source C# implementation of Markdown processor, as featured on Stack Overflow, regexp based.
- [Moonshine](https://github.com/brandonc/moonshine): popular C Markdown processor

Markdig is roughly x100 times faster than MarkdownSharp and extremelly competitive to other implems (that are not feature wise comparable) 

Performance in x86:

```
// * Summary *

BenchmarkDotNet-Dev=v0.9.6.0+
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4770 CPU @ 3.40GHz, ProcessorCount=8
Frequency=3319351 ticks, Resolution=301.2637 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1080.0

Type=Program  Mode=SingleRun  LaunchCount=2
WarmupCount=2  TargetCount=10

               Method |    Median |    StdDev |  Gen 0 | Gen 1 |  Gen 2 | Bytes Allocated/Op |
--------------------- |---------- |---------- |------- |------ |------- |------------------- |
          TestMarkdig | 5.4870 ms | 0.0158 ms | 193.00 | 12.00 |  84.00 |       1,425,192.72 |
    TestCommonMarkCpp | 4.0134 ms | 0.1008 ms |      - |     - | 180.00 |         454,859.74 |
    TestCommonMarkNet | 4.6139 ms | 0.0581 ms | 193.00 | 12.00 |  84.00 |       1,406,367.27 |
 TestCommonMarkNetNew | 5.5327 ms | 0.0461 ms | 193.00 | 96.00 |  84.00 |       1,738,465.42 |
     TestMarkdownDeep | 7.5910 ms | 0.1006 ms | 205.00 | 96.00 |  84.00 |       1,758,383.79 |
        TestMoonshine | 5.8843 ms | 0.1758 ms |      - |     - | 215.00 |         565,000.73 |

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
```

Performance for x64:

```
// * Summary *

BenchmarkDotNet-Dev=v0.9.6.0+
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-4770 CPU @ 3.40GHz, ProcessorCount=8
Frequency=3319351 ticks, Resolution=301.2637 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
JitModules=clrjit-v4.6.1080.0

Type=Program  Mode=SingleRun  LaunchCount=2
WarmupCount=2  TargetCount=10

               Method |    Median |    StdDev |  Gen 0 |  Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------- |---------- |---------- |------- |------- |------ |------------------- |
          TestMarkdig | 5.9539 ms | 0.0495 ms | 157.00 |  96.00 | 84.00 |       1,767,834.52 |
    TestCommonMarkNet | 4.3158 ms | 0.0161 ms | 157.00 |  96.00 | 84.00 |       1,747,432.06 |
 TestCommonMarkNetNew | 5.3421 ms | 0.0435 ms | 229.00 | 168.00 | 84.00 |       2,323,922.97 |
     TestMarkdownDeep | 7.4750 ms | 0.0281 ms | 318.00 | 186.00 | 84.00 |       2,576,728.69 |

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
```

## Credits

Thanks to the fantastic work done by [John Mac Farlane](http://johnmacfarlane.net/) for the CommonMark specs and all the people involved in making Markdown a better standard!

This project would not have been possible without this huge foundation.

## Author

Alexandre MUTEL aka [xoofx](http://xoofx.com)













