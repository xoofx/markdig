---
title: Inline parsers
---

# Inline parsers

Inline parsers identify inline-level elements (emphasis, links, code spans, custom syntax, etc.) from the text content of `LeafBlock` nodes. They run during the second phase of parsing, after all blocks have been identified.

## How inline parsing works

After block parsing produces a tree of blocks, the `InlineProcessor` visits every `LeafBlock` and processes its text:

1. Walk through the text character by character.
2. At each position, check if any `InlineParser` has that character in its `OpeningCharacters`.
3. Call `Match` on matching parsers (in priority order) until one returns `true`.
4. If a parser matches, add the created inline to the current container.
5. If no parser matches, the `LiteralInlineParser` consumes the character into a `LiteralInline`.
6. After all text is consumed, run post-processing (e.g., emphasis restructuring).

## The InlineParser base class

```csharp
public abstract class InlineParser : ParserBase<InlineProcessor>, IInlineParser<InlineProcessor>
{
    // Characters that trigger this parser
    public char[]? OpeningCharacters { get; set; }

    // Try to match an inline at the current position
    public abstract bool Match(InlineProcessor processor, ref StringSlice slice);
}
```

The interface is deliberately simple: set `OpeningCharacters` and implement `Match`.

## Writing a custom inline parser

### Step 1: Define the AST node

Create a class inheriting from `LeafInline` (for simple inlines) or `ContainerInline` (for inlines that contain other inlines):

```csharp
using Markdig.Syntax.Inlines;

/// <summary>
/// An inline representing a keyboard shortcut: [[Ctrl+S]]
/// </summary>
public class KeyboardInline : LeafInline
{
    /// <summary>
    /// The keyboard shortcut text.
    /// </summary>
    public string? Shortcut { get; set; }
}
```

### Step 2: Implement the inline parser

```csharp
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

public class KeyboardInlineParser : InlineParser
{
    public KeyboardInlineParser()
    {
        // This parser triggers on '[' characters
        OpeningCharacters = ['['];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // Check for [[ opening
        if (slice.CurrentChar != '[' || slice.PeekChar(1) != '[')
            return false;

        // Find the closing ]]
        var start = slice.Start;
        var contentStart = slice.Start + 2;
        var text = slice.Text;

        for (int i = contentStart; i < slice.End; i++)
        {
            if (text[i] == ']' && i + 1 <= slice.End && text[i + 1] == ']')
            {
                // Found closing ]]
                var shortcut = text[contentStart..i];

                // Create the inline
                var inline = new KeyboardInline
                {
                    Shortcut = shortcut,
                    Span = new Markdig.Syntax.SourceSpan(
                        processor.GetSourcePosition(start, out int line, out int column),
                        processor.GetSourcePosition(i + 1, out _, out _)),
                    Line = line,
                    Column = column
                };

                // Set the result on the processor
                processor.Inline = inline;

                // Advance the slice past the closing ]]
                slice.Start = i + 2;

                return true;
            }
        }

        // No closing ]] found — not a match
        return false;
    }
}
```

### Step 3: Key points about Match

- **Return `false` if no match** — Don't advance the slice if you don't match.
- **Set `processor.Inline`** — This is how you return the matched inline to the processor.
- **Advance `slice.Start`** — Move past the consumed characters. Other parsers will see text starting from the new position.
- **Set source position** — Use `processor.GetSourcePosition` for accurate `Span`, `Line`, and `Column`.

## Working with StringSlice

The `slice` parameter is a mutable view into the `LeafBlock`'s text. Key operations:

{.table}
| Member | Description |
|---|---|
| `slice.CurrentChar` | Character at current position |
| `slice.PeekChar(offset)` | Look ahead without advancing |
| `slice.NextChar()` | Advance and return the next character |
| `slice.SkipChar()` | Skip one character |
| `slice.Start` | Start index (mutable — advance this to consume) |
| `slice.End` | End index |
| `slice.Text` | The underlying string |
| `slice.IsEmpty` | True if `Start > End` |

## The InlineProcessor

Inside `Match`, the `processor` provides context:

{.table}
| Member | Description |
|---|---|
| `processor.Inline` | Set this to your created inline on match |
| `processor.Block` | The `LeafBlock` currently being processed |
| `processor.Root` | The root `ContainerInline` of the current block |
| `processor.Document` | The root `MarkdownDocument` |
| `processor.Context` | The per-call `MarkdownParserContext` (may be `null`) |
| `processor.GetSourcePosition(pos, out line, out column)` | Map a slice position to source position |
| `processor.GetParserState<TState>(this)` | Get/create parser state scoped to the current leaf processing pass |
| `processor.Emit(inline)` | Append `inline` into the deepest open inline container and set `processor.Inline` |
| `processor.BlockNew` | Request replacing the current leaf block after inline processing completes |
| `processor.ReplaceParentContainer(old, @new)` | Advanced: synchronize traversal if you replace a parent container block during inline processing |

## Container inlines and delimiters

Some inline syntaxes are **paired delimiters**: they open, later close, and the content between them becomes children of a `ContainerInline` node.

Markdig implements this pattern with temporary delimiter nodes (subclasses of `DelimiterInline`) plus a post-processing step that rewires the inline linked-list into the final AST shape.

Before building your own delimiter system, consider:

- If your syntax can be expressed as a simple paired delimiter (`~~`, `==`, `^^`, `""...""`, etc.), prefer extending `EmphasisInlineParser` by adding an `EmphasisDescriptor`. This gives you correct nesting rules and integrates with existing HTML renderers.
- If you need custom pairing rules (like links/images, tables, or non-trivial delimiter constraints), follow the built-in patterns:
  - `EmphasisInlineParser` + `EmphasisDelimiterInline`
  - `LinkInlineParser` + `LinkDelimiterInline`
  - `PipeTableDelimiterInline` (tables)

## Post-processing with IPostInlineProcessor

For complex inlines that need restructuring after all inline parsing is complete, implement `IPostInlineProcessor`:

```csharp
public interface IPostInlineProcessor
{
    bool PostProcess(
        InlineProcessor state,
        Inline? root,
        Inline? lastChild,
        int postInlineProcessorIndex,
        bool isFinalProcessing);
}
```

The emphasis system uses this to restructure nested delimiter runs into properly ordered `EmphasisInline` nodes.

## Inline manipulation helpers

When post-processing, use these helper methods on `Inline`:

{.table}
| Method | Description |
|---|---|
| `InsertAfter(inline)` | Insert a new inline after this one in the parent |
| `InsertBefore(inline)` | Insert before this one |
| `Remove()` | Remove this inline from its parent |
| `ReplaceBy(newInline)` | Replace this inline with another, optionally moving children |
| `PreviousSibling` | Previous sibling in the linked list |
| `NextSibling` | Next sibling |
| `FirstParentOfType<T>()` | Find the nearest ancestor of type `T` |

## Example: simple emoji parser

A complete inline parser that converts `:name:` shortcodes:

```csharp
public class SimpleEmojiParser : InlineParser
{
    private readonly Dictionary<string, string> _emojis = new()
    {
        { "smile", "😊" },
        { "heart", "❤️" },
        { "rocket", "🚀" }
    };

    public SimpleEmojiParser()
    {
        OpeningCharacters = [':'];
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var start = slice.Start;

        // Skip opening ':'
        var c = slice.NextChar();

        // Read the emoji name
        var nameStart = slice.Start;
        while (c != ':' && c != '\0' && !c.IsWhitespace())
        {
            c = slice.NextChar();
        }

        if (c != ':')
        {
            // No closing ':', reset and fail
            slice.Start = start;
            return false;
        }

        var name = slice.Text[nameStart..slice.Start];
        if (!_emojis.TryGetValue(name, out var emoji))
        {
            slice.Start = start;
            return false;
        }

        // Skip closing ':'
        slice.NextChar();

        processor.Inline = new LiteralInline(emoji)
        {
            Span = new SourceSpan(
                processor.GetSourcePosition(start, out int line, out int column),
                processor.GetSourcePosition(slice.Start - 1, out _, out _)),
            Line = line,
            Column = column
        };

        return true;
    }
}
```

## Next steps

- [Block parsers](block-parsers.md) — Write block-level parsers
- [Renderers](renderers.md) — Create renderers for your custom inlines
- [Creating extensions](creating-extensions.md) — Wire parsers and renderers into an extension
