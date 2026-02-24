---
title: "Emoji & SmartyPants"
---

# Emoji & SmartyPants

## Emoji

Enable with `.UseEmojiAndSmiley()` (not included in `UseAdvancedExtensions()`).

This extension converts emoji shortcodes and (optionally) ASCII smileys into Unicode emoji characters.

### Shortcode syntax

```markdown
:smile: :+1: :heart: :rocket: :warning:
```

### Disable smileys

By default, ASCII smileys like `:)` are also converted. To use only named shortcodes:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseEmojiAndSmiley(enableSmileys: false)
    .Build();
```

### Custom emoji mappings

```csharp
using Markdig.Extensions.Emoji;

var mapping = new EmojiMapping(
    new Dictionary<string, string>
    {
        { ":custom:", "🎉" },
        { ":markdig:", "📝" }
    });

var pipeline = new MarkdownPipelineBuilder()
    .UseEmojiAndSmiley(mapping)
    .Build();
```

### Common shortcodes

{.table}
| Shortcode | Emoji |
|---|---|
| `:smile:` | 😄 |
| `:+1:` | 👍 |
| `:heart:` | ❤️ |
| `:rocket:` | 🚀 |
| `:warning:` | ⚠️ |
| `:star:` | ⭐ |
| `:fire:` | 🔥 |
| `:bug:` | 🐛 |
| `:bulb:` | 💡 |
| `:memo:` | 📝 |

## SmartyPants

Enable with `.UseSmartyPants()` (not included in `UseAdvancedExtensions()`).

SmartyPants converts ASCII punctuation into typographically correct HTML entities. Inspired by [Daring Fireball — SmartyPants](https://daringfireball.net/projects/smartypants/).

### Transformations

{.table}
| Input | Output | Description |
|---|---|---|
| `"Hello"` | "Hello" | Smart double quotes |
| `'Hello'` | 'Hello' | Smart single quotes |
| `--` | – | En dash |
| `---` | — | Em dash |
| `...` | … | Ellipsis |
| `<<` | « | Left guillemet |
| `>>` | » | Right guillemet |

### Usage

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseSmartyPants()
    .Build();

var html = Markdown.ToHtml("He said \"Hello\" -- she replied 'Hi'...", pipeline);
```

### Options

```csharp
using Markdig.Extensions.SmartyPants;

var options = new SmartyPantOptions();
// Configure options as needed

var pipeline = new MarkdownPipelineBuilder()
    .UseSmartyPants(options)
    .Build();
```
