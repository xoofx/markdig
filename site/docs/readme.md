---
title: "Markdig — User Guide"
---

# Markdig — User Guide

Welcome to the Markdig documentation. Whether you are new to Markdig or an experienced user, this guide helps you make the most of the library.

## Getting started

{.table}
| Guide | What you'll learn |
|---|---|
| [Getting started](getting-started.md) | Install Markdig, parse your first Markdown, and render to HTML |
| [Usage](usage.md) | Parse, render, pipeline architecture, and common patterns |

## CommonMark syntax

{.table}
| Guide | What you'll learn |
|---|---|
| [CommonMark syntax](commonmark.md) | Full reference for headings, paragraphs, emphasis, links, images, code, lists, blockquotes, and more |

## Extensions

{.table}
| Guide | What it provides |
|---|---|
| [Extensions overview](extensions/readme.md) | Index of all 20+ built-in extensions |
| [Tables](extensions/tables.md) | Pipe tables and grid tables |
| [Emphasis extras](extensions/emphasis-extras.md) | Strikethrough, subscript, superscript, inserted, marked |
| [Task lists](extensions/task-lists.md) | Checkbox task lists in GFM style |
| [Mathematics](extensions/mathematics.md) | Inline and block LaTeX math |
| [Diagrams](extensions/diagrams.md) | Mermaid, nomnoml, and other diagram languages |
| [Alert blocks](extensions/alert-blocks.md) | GitHub-style alerts: NOTE, TIP, WARNING, etc. |
| [Footnotes](extensions/footnotes.md) | Reference-style footnotes |
| [Generic attributes](extensions/generic-attributes.md) | Attach CSS classes, IDs, and attributes to any element |
| [Custom containers](extensions/custom-containers.md) | Fenced `:::` div containers |
| [Abbreviations](extensions/abbreviations.md) | Abbreviation definitions and auto-expansion |
| [Definition lists](extensions/definition-lists.md) | `<dl>` / `<dt>` / `<dd>` lists |
| [Auto-identifiers](extensions/auto-identifiers.md) | Automatic heading IDs |
| [Auto-links](extensions/auto-links.md) | Automatic URL detection |
| [Figures & footers](extensions/figures-footers-citations.md) | Figures, footers, and citations |
| [Emoji & SmartyPants](extensions/emoji-smartypants.md) | Emoji shortcodes and smart typography |
| [Media links](extensions/media-links.md) | Embedded YouTube, Vimeo, and media players |
| [List extras](extensions/list-extras.md) | Alpha and Roman numeral ordered lists |
| [YAML front matter](extensions/yaml-frontmatter.md) | YAML metadata blocks |
| [Other extensions](extensions/other.md) | Bootstrap, hardline breaks, JIRA links, globalization, and more |

## Developer guide

{.table}
| Guide | What you'll learn |
|---|---|
| [Developer guide overview](advanced/readme.md) | How to extend Markdig with custom parsers and renderers |
| [Abstract syntax tree](advanced/ast.md) | Structure and traversal of the AST |
| [Pipeline architecture](advanced/pipeline.md) | How the parsing pipeline works |
| [Creating extensions](advanced/creating-extensions.md) | Implement `IMarkdownExtension` and register it |
| [Block parsers](advanced/block-parsers.md) | Write custom block-level parsers |
| [Inline parsers](advanced/inline-parsers.md) | Write custom inline-level parsers |
| [Renderers](advanced/renderers.md) | Write custom renderers for HTML and other formats |
| [Performance](advanced/performance.md) | Tips for high-performance Markdown processing |
