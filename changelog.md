# Changelog

## 0.15.4 (07 Oct 2018)
- Add autolink domain GFM validation ([(PR #239)](https://github.com/lunet-io/markdig/pull/253))

## 0.15.3 (15 Sep 2018)
- Add support for RTL ([(PR #239)](https://github.com/lunet-io/markdig/pull/239))
- Add MarkdownDocument.LineCount ([(PR #241)](https://github.com/lunet-io/markdig/pull/241))
- Fix source positions for link definitions ([(PR #243)](https://github.com/lunet-io/markdig/pull/243))
- Add ListItemBlock.Order ([(PR #244)](https://github.com/lunet-io/markdig/pull/244))
- Add MarkdownDocument.LineStartIndexes ([(PR #247)](https://github.com/lunet-io/markdig/pull/247))

## 0.15.2 (21 Aug 2018)
- Fix footnotes parsing when they are defined after a container that has been closed in the meantime (#223)

## 0.15.1 (10 July 2018)
- Add support for `netstandard2.0`
- Make AutoIdentifierExtension thread safe

## 0.15.0 (4 Apr 2018)
- Add `ConfigureNewLine` extension method to `MarkdownPipelineBuilder` ([(PR #214)](https://github.com/lunet-io/markdig/pull/214))
- Add alternative `Use` extension method to `MarkdownPipelineBuilder` that receives an object instance ([(PR #213)](https://github.com/lunet-io/markdig/pull/213))
- Added class attribute to media link extension ([(PR #203)](https://github.com/lunet-io/markdig/pull/203))
- Optional link rewriter func for HtmlRenderer #143 ([(PR #201)](https://github.com/lunet-io/markdig/pull/201))
- Upgrade NUnit3TestAdapter from 3.2 to 3.9 to address Resharper test runner problems ([(PR #199)](https://github.com/lunet-io/markdig/pull/199))
- HTML renderer supports converting relative URLs on links and images to absolute #143 ([(PR #197)](https://github.com/lunet-io/markdig/pull/197))

## 0.14.9 (15 Jan 2018)
- AutoLinkParser should to remove mailto: in outputted text ([(PR #195)](https://github.com/lunet-io/markdig/pull/195))
- Add support for `music.yandex.ru` and `ok.ru` for MediaLinks extension ([(PR #193)](https://github.com/lunet-io/markdig/pull/193))
## 0.14.8 (05 Dec 2017)
- Fix potential StackOverflow exception when processing deep nested `|` delimiters (#179)
## 0.14.7 (25 Nov 2017)
- Fix autolink attached attributes not being displayed properly (#175)
## 0.14.6 (21 Nov 2017)
- Fix yaml frontmatter issue when ending with a empty line (#170)
## 0.14.5 (18 Nov 2017)
- Fix changelog link from nuget package
## 0.14.4 (18 Nov 2017)
- Add changelog.md
- Fix bug when a thematic break is inside a fenced code block inside a pending list (#164)
- Add support for GFM autolinks (#165, #169)
- Better handle YAML frontmatter in case the opening `---` is never actually closed (#160)
- Fix link conflict between a link to an image definition and heading auto-identifiers (#159)
## 0.14.3
- Make EmojiExtension.EnableSmiley public
## 0.14.2
- Fix issue with emphasis preceded/followed by an HTML entity (#157)
- Add support for link reference definitions for Normalize renderer (#155)   
- Add option to disable smiley parsing in EmojiAndSmiley extension
## 0.14.1
- Fix crash in Markdown.Normalize to handle HtmlBlock correctly        
- Add better handling of bullet character for lists in Markdown.Normalize
## 0.14.0
- Add Markdown.ToPlainText, Add option HtmlRenderer.EnableHtmlForBlock.
- Add Markdown.Normalize, to allow to normalize a markdown document. Add NormalizeRenderer, to render a MarkdownDocument back to markdown.
## 0.13.4
- Add support for single table header row without a table body rows (#141)
- ADd support for `nomnoml` diagrams
## 0.13.3
- Add support for Pandoc YAML frontmatter (#138)
## 0.13.2
- Add support for UAP10.0 (#137)
## 0.13.1
- Fix indenting issue after a double digit list block using a tab (#134)
## 0.13.0
- Update to latest CommonMark specs 0.28
## 0.12.3
 - Fix issue with HTML blocks for heading h2,h3,h4,h5,h6 that were not correctly identified as HTML blocks as per CommonMark spec
## 0.12.2
 - Fix issue with generic attributes used just before a pipe table (issue #121)
## 0.12.1
 - Fix issue with media links extension when a URL to video is used, an unexpected closing `&lt;/iframe&gt;` was inserted (issue #119)
## 0.12.0
 - Add new extension JiraLink support (thanks to @clarkd)
 - Fix issue in html attributes not parsing correctly properties (thanks to @meziantou)
 - Fix issues detected by an automatic static code analysis tool
## 0.11.0
 - Fix issue with math extension and $$ block parsing not handling correctly beginning of a $$ as a inline math instead (issue #107)
 - Fix issue with custom attributes for emphasis
 - Add support for new special custom arrows emoji (`->` `<-` `<->` `<=` `=>` `<==>`)
## 0.10.7
 - Fix issue when an url ends by a dot `.`
## 0.10.6
 - Fix emphasis with HTML entities
## 0.10.5
 - Several minor fixes
## 0.10.4
 - Fix issue with autolinks
 - Normalize number of columns for tables
## 0.10.3
 - Fix issue with pipetables shifting a cell to a new column (issue #73)
## 0.10.2
 - Fix exception when trying to urlize an url with an unicode character outside the supported range by NormD (issue #75)
## 0.10.1
- Update to latest CommonMark specs
- Fix source span for LinkReferenceDefinition
## 0.10.0
- Breaking change of the IMarkdownExtension to allow to receive the MarkdownPipeline for the renderers setup
## 0.9.1
- Fix regression bug with conflicts between autolink extension and html inline/regular links
## 0.9.0
- Add new Autolink extension
## 0.8.5
- Allow to force table column alignment to left
## 0.8.4
- Fix issue when calculating the span of an indented code block within a list. Make sure to include first whitespace on the line
## 0.8.3
- fix NullReferenceException with Gridtables extension when a single `+` is entered on a line
## 0.8.2
- fix potential cast exception with Abreviation extension and empty literals
## 0.8.1
- new extension to disable URI escaping for non-US-ASCII characters to workaround a bug in Edge/IE
- Fix an issue with abbreviations with left/right multiple non-punctuation/space characters
## 0.8.0
- Update to latest CommonMark specs
- Fix empty literal
- Add YAML frontmatter extension
## 0.7.5
- several bug fixes (pipe tables, disable HTML, special attributes, inline maths, abbreviations...)
- add support for rowspan in grid tables
## 0.7.4
- Fix bug with strong emphasis starting at the beginning of a line
## 0.7.3
- Fix threading issue with pipeline
## 0.7.2
- Fix rendering of table colspan with non english locale
- Fix grid table colspan parsing
- Add nofollow extension for links
## 0.7.1
- Fix issue in smarty pants which could lead to an InvalidCastException
- Update parsers to latest CommonMark specs
## 0.7.0
- Update to latest NETStandard.Library 1.6.0
- Fix issue with digits in auto-identifier extension
- Fix incorrect start of span calculated for code indented blocks
## 0.6.2
- Handle latest CommonMark specs for corner cases for emphasis (See https://talk.commonmark.org/t/emphasis-strong-emphasis-corner-cases/2123/1 )
## 0.6.1:
- Fix issue with autoidentifier extension overriding manual HTML attributes id on headings
## 0.6.0
- Fix conflicts between PipeTables and SmartyPants extensions
- Add SelfPipeline extension
