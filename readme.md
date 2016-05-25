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

You can have a look at the [MarkdownPipeline](https://github.com/lunet-io/markdig/blob/master/src/Markdig/MarkdownPipeline.cs) file that describes all actionable extensions.

## License

This software is released under the [BSD-Clause 2 license](http://opensource.org/licenses/BSD-2-Clause).

## Credits

Thanks to the fantastic work done by [John Mac Farlane](http://johnmacfarlane.net/) for the CommonMark specs and all the people involved in making Markdown a better standard!

This project would not have been possible without this huge foundation.

## Author

Alexandre MUTEL aka [xoofx](http://xoofx.com)













