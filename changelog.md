# Changelog

## 0.27.0 (23 Jan 2022)
- Fix link reference definition parse bug with title and CRLF ([PR #590](https://github.com/lunet-io/markdig/pull/590))
- Move tests to net6.0 ([PR #560](https://github.com/lunet-io/markdig/pull/560))

## 0.26.0 (27 Aug 2021)
- Fix rendering diff between line endings ([PR #560](https://github.com/lunet-io/markdig/pull/560))
- Make Mathematics extension respect EnableHtml* options ([PR #570](https://github.com/lunet-io/markdig/pull/570))

## 0.25.0 (10 June 2021)
- Fix regression when parsing link reference definitions (#543)
- Make digits in JiraKey's posible ([PR #548](https://github.com/lunet-io/markdig/pull/548))

## 0.24.0 (20 Mar 2021)
- Add support for roundtrip Markdown ([PR #481](https://github.com/lunet-io/markdig/pull/481))
- Introduction of nullability ([PR #522](https://github.com/lunet-io/markdig/pull/522) [PR #524](https://github.com/lunet-io/markdig/pull/524) [PR #525](https://github.com/lunet-io/markdig/pull/525) [PR #526](https://github.com/lunet-io/markdig/pull/526) [PR #527](https://github.com/lunet-io/markdig/pull/527))
- Various internal cleanup and small performance improvements ([PR #521](https://github.com/lunet-io/markdig/pull/521) [PR #524](https://github.com/lunet-io/markdig/pull/524) [PR #525](https://github.com/lunet-io/markdig/pull/525) [PR #529](https://github.com/lunet-io/markdig/pull/529) [PR #531](https://github.com/lunet-io/markdig/pull/531) [PR #532](https://github.com/lunet-io/markdig/pull/532))

## 0.23.0 (16 Jan 2021)
- Add depth limits to avoid pathological-case parsing times/StackOverflows (#500)
- Breaking change: rename AutolineInlineParser to AutolinkInlineParser

## 0.22.1 (2 Dec 2020)
- Update logo for NuGet package

## 0.22.0 (05 Oct 2020)
- Fix Setext headings in block quotes.
- Fix tel: treated as autolink ([PR #478](https://github.com/lunet-io/markdig/pull/478))
- Make Inline.FirstParentOfType public ([PR #474](https://github.com/lunet-io/markdig/pull/474))
- Fix `&` to be parsed as a punctuation while it was detected as a html entity in certain cases ([PR #471](https://github.com/lunet-io/markdig/pull/471))
- Add ParentBlock property to ContainerInline ([PR #468](https://github.com/lunet-io/markdig/pull/468))

## 0.21.1 (17 Aug 2020)
- Fix Markdig.Signed on GitHub Actions

## 0.21.0 (17 Aug 2020)
- Restore support for .NET 4.5 (#)
- Add IReadonlyList interface to ContainerBlock to unify and simplify enumeration (#425)
- Fix relative uri detection to be cross-platform compatible (#430)
- Escape URLs scheme (#431)
- Fix media links (#435)
- Fix parsing math blocks with no leading or trailing whitespace (#452)
- Add support for autolink `tel:` uri (#453)
- Fallback to non-punycode encoding for invalid IDN urls (#449)
- Pipe Tables: Normalize using header column count (#455)
- Expose IndentCount of FencedCodeBlock (#464)

## 0.20.0 (18 Apr 2020)
- Markdig is now compatible only with `NETStandard 2.0`, `NETStandard 2.1`, `NETCoreApp 2.1` and `NETCoreApp 3.1`.
- Many performance improvements from [PR #416](https://github.com/lunet-io/markdig/pull/416) 
[PR #417](https://github.com/lunet-io/markdig/pull/417)
[PR #418](https://github.com/lunet-io/markdig/pull/418) 
[PR #421](https://github.com/lunet-io/markdig/pull/421) 
[PR #422](https://github.com/lunet-io/markdig/pull/422) 
[PR #410](https://github.com/lunet-io/markdig/pull/410)

## 0.18.3 (8 Mar 2020)
- Publish NuGet Symbol packages

## 0.18.2 (8 Mar 2020)
- Optimize LineReader.ReadLine in [PR #393](https://github.com/lunet-io/markdig/pull/393)
- Use HashSet<T> instead of Dictionary<TKey, TValue> in CharacterMap<T> in [PR #394](https://github.com/lunet-io/markdig/pull/394)
- Use BitVector128 in CharacterMap<T> in [PR #396](https://github.com/lunet-io/markdig/pull/396)
- Optimizations in StringLineGroup in [PR #399](https://github.com/lunet-io/markdig/pull/399)
- Fixed a bug in HeadingRenderer in [PR #402](https://github.com/lunet-io/markdig/pull/402)
- Fixes issue #303 in [PR #404](https://github.com/lunet-io/markdig/pull/404)
- Make output of HtmlTableRenderer XML wellformed in [PR #406](https://github.com/lunet-io/markdig/pull/406)

## 0.18.1 (21 Jan 2020)
- Re-allow emojis and smileys customization, that was broken in [PR #308](https://github.com/lunet-io/markdig/pull/308) ([PR #386](https://github.com/lunet-io/markdig/pull/386))
- Add `IHostProvider` for medialink customization (#337), support protocol-less url (#135) ([(PR #341)](https://github.com/lunet-io/markdig/pull/341))
- Add missing Descendants<T> overload ([(PR #387)](https://github.com/lunet-io/markdig/pull/387))

## 0.18.0 (24 Oct 2019)
- Ignore backslashes in GFM AutoLinks ([(PR #357)](https://github.com/lunet-io/markdig/pull/357))
- Fix SmartyPants quote matching ([(PR #360)](https://github.com/lunet-io/markdig/pull/360))
- Fix generic attributes with values of length 1 ([(PR #361)](https://github.com/lunet-io/markdig/pull/361))
- Fix link text balanced bracket matching ([(PR #375)](https://github.com/lunet-io/markdig/pull/375))
- Improve overall performance and substantially reduce allocations ([(PR #377)](https://github.com/lunet-io/markdig/pull/377))

## 0.17.1 (04 July 2019)
- Fix regression when escaping HTML characters ([(PR #340)](https://github.com/lunet-io/markdig/pull/340))
- Update Emoji Dictionary ([(PR #346)](https://github.com/lunet-io/markdig/pull/346))

## 0.17.0 (10 May 2019)
- Update to latest CommonMark specs 0.29 ([(PR #327)](https://github.com/lunet-io/markdig/pull/327))
- Add `AutoLinkOptions` with `OpenInNewWindow`, `UseHttpsForWWWLinks` ([(PR #327)](https://github.com/lunet-io/markdig/pull/327))
- Add `DisableHeadings` extension method to `MarkdownPipelineBuilder` ([(PR #327)](https://github.com/lunet-io/markdig/pull/327))
- Drop support for netstandard1.1 and Portable Class Libraries ([(PR #319)](https://github.com/lunet-io/markdig/pull/319))
- Allow non-ASCII characters in url domain names ([(PR #319)](https://github.com/lunet-io/markdig/pull/319))
- Add better support for youtu.be link ([(PR #336)](https://github.com/lunet-io/markdig/pull/336))
- Fix backsticks in Markdown.Normalize ([(PR #334)](https://github.com/lunet-io/markdig/pull/334))

## 0.16.0 (25 Feb 2019)
- Improve performance of emoji-abbreviation parser ([(PR #305)](https://github.com/lunet-io/markdig/pull/305))
- Change output for math extension to use a rendering more compatible with existing Math JS libraries ([(PR #311)](https://github.com/lunet-io/markdig/pull/311))
- Improve emphasis parser to allow to match more than 2 characters ([(PR #301)](https://github.com/lunet-io/markdig/pull/301))
- Output attached attributes to a `<tr>` from a table row ([(PR #300)](https://github.com/lunet-io/markdig/pull/300))
- Improve MarkdownObject.Descendants() search ([(PR #288)](https://github.com/lunet-io/markdig/pull/288))
- Allow to pass a `MarkdownParserContext` ([(PR #285)](https://github.com/lunet-io/markdig/pull/285))

## 0.15.7 (11 Jan 2019)
- Add configurable leading count for ATX headers ([(PR #282)](https://github.com/lunet-io/markdig/pull/282))
- Render XML well-formed boolean attribute ([(PR #281)](https://github.com/lunet-io/markdig/pull/281))

## 0.15.6 (28 Dec 2018)
- Fix potential hang when parsing LinkReferenceDefinition #278
- Fix parsing of an invalid html entity (#277)
- Fix IndexOutOfRangeException while parsing fenced code block with a single trailing space (#276)
- Add tests for checking that ArgumentOutOfRangeException doesn't occur on invalid input md string (#275)

## 0.15.5 (11 Dec 2018)
- Empty image alt text for link reference definitions ([(PR #254)](https://github.com/lunet-io/markdig/pull/254))
- Fix AutoLink Match links without slash after domain ([(PR #260)](https://github.com/lunet-io/markdig/pull/260))
- Make AutoLink ValidPreviousCharacters configurable ([(PR #264)](https://github.com/lunet-io/markdig/pull/264))
- Ensuring line breaks when renderer does not have html enabled ([(PR #270)](https://github.com/lunet-io/markdig/pull/270))

## 0.15.4 (07 Oct 2018)
- Add autolink domain GFM validation ([(PR #253)](https://github.com/lunet-io/markdig/pull/253))

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
- fix potential cast exception with Abbreviation extension and empty literals
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
