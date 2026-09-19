---
title: "Emoji & SmartyPants"
---

# Emoji & SmartyPants

## Emoji

Enable with `.UseEmojiAndSmiley()` (not included in `UseAdvancedExtensions()`).

This extension converts emoji shortcodes and (optionally) ASCII smileys into Unicode emoji characters.

### Smileys and emphasis

When a smiley ends with emphasis delimiters, emphasis takes priority if it uses any of those trailing characters. For example, `*text):*` and `**text):**` render as italic and bold text ending in a colon, not as a kissing emoji. The same rule applies to custom mappings ending in configured emphasis delimiters, including those enabled by `UseEmphasisExtras()`.

If the complete smiley remains literal after emphasis parsing, it is still converted: standalone `:*` becomes 😗, and `:**` becomes 😗 followed by a literal asterisk. This also means `:*text*` renders a colon followed by italic text, and `*text :* more*` closes emphasis at the first asterisk after the colon. Use `:kissing:` when you want an unambiguous emoji inside emphasis, for example `*:kissing:*`.

This is a trailing-delimiter rule, not a general change to parser precedence. Named shortcodes, smileys without trailing emphasis delimiters, and custom mappings consisting only of repetitions of one delimiter character retain their existing matching behavior. Disable ASCII smileys if your input should use only named emoji shortcodes.

With generic attributes enabled, attributes immediately following a deferred delimiter run follow the normal emphasis rules too. For example, `*text):*{.label}` attaches the class to the emphasis, while `:*{.label}` has an unmatched delimiter and does not apply the class to the paragraph. Use `:kissing:{.label}` for that purpose.

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
