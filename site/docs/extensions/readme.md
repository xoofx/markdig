---
title: Extensions
---

# Extensions

Markdig ships with **20+ built-in extensions** that go beyond CommonMark. Extensions are enabled via the `MarkdownPipelineBuilder` fluent API.

## Quick start

Enable all advanced extensions at once:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();
```

Or enable specific extensions individually:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables()
    .UseFootnotes()
    .UseMathematics()
    .Build();
```

## What UseAdvancedExtensions includes

`UseAdvancedExtensions()` enables these extensions:

{.table}
| Extension | Method | Description |
|---|---|---|
| [Alert blocks](alert-blocks.md) | `.UseAlertBlocks()` | GitHub-style `[!NOTE]`, `[!TIP]`, etc. |
| [Abbreviations](abbreviations.md) | `.UseAbbreviations()` | Abbreviation definitions: `*[HTML]: Hyper Text Markup Language` |
| [Auto-identifiers](auto-identifiers.md) | `.UseAutoIdentifiers()` | Automatic `id` attributes on headings |
| [Citations](figures-footers-citations.md) | `.UseCitations()` | Citation text with `""...""` |
| [Custom containers](custom-containers.md) | `.UseCustomContainers()` | Fenced `:::` div containers |
| [Definition lists](definition-lists.md) | `.UseDefinitionLists()` | `<dl>` / `<dt>` / `<dd>` lists |
| [Emphasis extras](emphasis-extras.md) | `.UseEmphasisExtras()` | Strikethrough, sub/superscript, inserted, marked |
| [Figures](figures-footers-citations.md) | `.UseFigures()` | `^^^` figure blocks |
| [Footers](figures-footers-citations.md) | `.UseFooters()` | `^^` footers |
| [Footnotes](footnotes.md) | `.UseFootnotes()` | `[^ref]` footnotes |
| [Grid tables](tables.md) | `.UseGridTables()` | Pandoc-style grid tables |
| [Mathematics](mathematics.md) | `.UseMathematics()` | `$...$` inline / `$$...$$` block math |
| [Media links](media-links.md) | `.UseMediaLinks()` | Embed YouTube, Vimeo, etc. |
| [Pipe tables](tables.md) | `.UsePipeTables()` | GitHub-style pipe tables |
| [List extras](list-extras.md) | `.UseListExtras()` | Alpha and Roman numeral ordered lists |
| [Task lists](task-lists.md) | `.UseTaskLists()` | `- [x]` / `- [ ]` checkboxes |
| [Diagrams](diagrams.md) | `.UseDiagrams()` | Mermaid, nomnoml diagram blocks |
| [Auto-links](auto-links.md) | `.UseAutoLinks()` | Auto-detect `http://`, `www.` URLs |
| [Generic attributes](generic-attributes.md) | `.UseGenericAttributes()` | `{.class #id key=value}` attributes |

## Additional extensions (not in UseAdvancedExtensions)

{.table}
| Extension | Method | Description |
|---|---|---|
| [Emoji](emoji-smartypants.md) | `.UseEmojiAndSmiley()` | `:emoji:` shortcodes and smileys |
| [SmartyPants](emoji-smartypants.md) | `.UseSmartyPants()` | Smart quotes, dashes, ellipses |
| [Bootstrap](other.md) | `.UseBootstrap()` | Bootstrap CSS classes |
| [Hardline breaks](other.md) | `.UseSoftlineBreakAsHardlineBreak()` | Treat soft line breaks as `<br>` |
| [YAML front matter](yaml-frontmatter.md) | `.UseYamlFrontMatter()` | Parse and discard YAML front matter |
| [JIRA links](other.md) | `.UseJiraLinks(options)` | Auto-link Jira issue keys |
| [Globalization](other.md) | `.UseGlobalization()` | Right-to-left text support |
| [Referral links](other.md) | `.UseReferralLinks(rels)` | Add `rel` attributes to links |
| [Self pipeline](other.md) | `.UseSelfPipeline()` | Auto-configure pipeline from document |
| [Pragma lines](other.md) | `.UsePragmaLines()` | Line number pragma IDs |
| [Non-ASCII no escape](other.md) | `.UseNonAsciiNoEscape()` | Disable URI escaping for non-ASCII |

## Extension ordering

Extensions are applied in the order they are added to the builder. Most are order-independent, but **`UseGenericAttributes()` should be added last** because it modifies other parsers.

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UsePipeTables()
    .UseFootnotes()
    .UseMathematics()
    .UseGenericAttributes() // Always last!
    .Build();
```
