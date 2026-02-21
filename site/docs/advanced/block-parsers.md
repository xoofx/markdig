---
title: Block parsers
---

# Block parsers

Block parsers identify block-level elements (paragraphs, headings, lists, code blocks, custom containers, etc.) from the Markdown source text. They run during the first phase of parsing, processing the document **line by line**.

## How block parsing works

The `BlockProcessor` orchestrates block parsing. For each line in the source:

1. **Continue** — All currently open blocks are asked if they continue on the current line (`TryContinue`). Blocks that don't continue are closed.
2. **Open** — The processor tries to open new blocks by calling `TryOpen` on block parsers whose `OpeningCharacters` match the current character.
3. **Dispatch** — Parsers are dispatched based on their `OpeningCharacters` array — a parser is only tried when one of its opening characters matches the current position.

## The BlockParser base class

All block parsers inherit from `BlockParser`:

```csharp
public abstract class BlockParser : ParserBase<BlockProcessor>, IBlockParser<BlockProcessor>
{
    // Characters that trigger this parser
    public char[]? OpeningCharacters { get; set; }

    // Whether this parser can interrupt an open paragraph
    public virtual bool CanInterrupt(BlockProcessor processor, Block block) => true;

    // Try to open a new block at the current position
    public abstract BlockState TryOpen(BlockProcessor processor);

    // Try to continue an already-open block
    public virtual BlockState TryContinue(BlockProcessor processor, Block block)
        => BlockState.None;

    // Called when a block is closing. Return false to remove it from the AST.
    public virtual bool Close(BlockProcessor processor, Block block) => true;

    // Event fired when a block is closed
    public event ProcessBlockDelegate? Closed;
}
```

## BlockState return values

The `TryOpen` and `TryContinue` methods return a `BlockState` enum:

{.table}
| Value | Meaning |
|---|---|
| `None` | No match — parser did not recognize anything |
| `Skip` | Skip this parser for the current line, try others |
| `Continue` | Block stays open; for leaf blocks, the line content is appended |
| `ContinueDiscard` | Block stays open; line is consumed but not appended to the block |
| `Break` | Block is closed; current line remains available for other parsers |
| `BreakDiscard` | Block is closed; current line is consumed (not available to others) |

## Writing a custom block parser

### Step 1: Define the AST node

Create a class inheriting from `LeafBlock` (for blocks with inline content) or `ContainerBlock` (for blocks containing other blocks):

```csharp
using Markdig.Parsers;
using Markdig.Syntax;

/// <summary>
/// A custom note block: !!! note "Title"
/// </summary>
public class NoteBlock : LeafBlock
{
    public NoteBlock(BlockParser parser) : base(parser)
    {
    }

    /// <summary>
    /// The note title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The note type (note, warning, etc.).
    /// </summary>
    public string? NoteType { get; set; }
}
```

### Step 2: Implement the block parser

```csharp
using Markdig.Helpers;
using Markdig.Parsers;

public class NoteBlockParser : BlockParser
{
    public NoteBlockParser()
    {
        // This parser triggers on '!' characters
        OpeningCharacters = ['!'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        // Don't match if indented as code (4+ spaces)
        if (processor.IsCodeIndent)
            return BlockState.None;

        var line = processor.Line;
        var startPosition = line.Start;

        // Check for "!!!" prefix
        if (line.CurrentChar != '!' || line.PeekChar(1) != '!' || line.PeekChar(2) != '!')
            return BlockState.None;

        // Advance past "!!!"
        line.Start += 3;
        line.TrimStart(); // Skip whitespace

        // Read the note type (e.g., "note", "warning")
        var noteType = line.ToString().Trim();
        string? title = null;

        // Check for quoted title: !!! note "My Title"
        var quoteIndex = noteType.IndexOf('"');
        if (quoteIndex >= 0)
        {
            title = noteType[(quoteIndex + 1)..].TrimEnd('"').Trim();
            noteType = noteType[..quoteIndex].Trim();
        }

        // Create the block and push it
        var block = new NoteBlock(this)
        {
            NoteType = noteType,
            Title = title,
            Span = new SourceSpan(startPosition, line.End),
            Line = processor.LineIndex,
            Column = processor.Column
        };

        processor.NewBlocks.Push(block);
        return BlockState.Break; // Single-line block
    }
}
```

### Step 3: Key points about TryOpen

- **Check `processor.IsCodeIndent`** — If the line is indented 4+ spaces, it's a code block, not your custom syntax.
- **Set the `Parser` property** — The `Block` constructor automatically sets it from the argument.
- **Set `Span`, `Line`, and `Column`** — These enable precise source location tracking.
- **Push to `processor.NewBlocks`** — This tells the processor a new block was found.
- **Return the right `BlockState`** — `Break` for a single-line block, `Continue` for multi-line blocks.

## Multi-line block parsers

For blocks that span multiple lines, use `TryContinue`:

```csharp
public class AdmonitionParser : BlockParser
{
    public AdmonitionParser()
    {
        OpeningCharacters = ['!'];
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsCodeIndent) return BlockState.None;

        var line = processor.Line;
        if (line.CurrentChar != '!' || line.PeekChar(1) != '!' || line.PeekChar(2) != '!')
            return BlockState.None;

        line.Start += 3;
        line.TrimStart();

        var block = new AdmonitionBlock(this)
        {
            // ... set properties
        };

        processor.NewBlocks.Push(block);
        return BlockState.Continue; // Expect more lines
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        // Continue while lines are indented (part of the admonition)
        if (processor.IsBlankLine)
            return BlockState.Continue; // Blank lines are allowed

        if (processor.Indent >= 4)
        {
            // Indented content belongs to this block
            processor.GoToColumn(processor.ColumnBeforeIndent + 4);
            return BlockState.Continue;
        }

        // Not indented — close the block
        return BlockState.Break;
    }
}
```

## Using FencedBlockParserBase

For blocks that use opening/closing fences (like `:::` custom containers or ``` code blocks), inherit from `FencedBlockParserBase<T>` to get fencing logic for free:

```csharp
using Markdig.Parsers;

/// <summary>
/// A custom "spoiler" block: |||spoiler ... |||
/// </summary>
public class SpoilerBlock : FencedCodeBlock
{
    public SpoilerBlock(BlockParser parser) : base(parser) { }
}

public class SpoilerParser : FencedBlockParserBase<SpoilerBlock>
{
    public SpoilerParser()
    {
        OpeningCharacters = ['|'];
        InfoPrefix = null; // No info string prefix required
    }

    protected override SpoilerBlock CreateFencedBlock(BlockProcessor processor)
    {
        return new SpoilerBlock(this);
    }
}
```

The base class handles:
- Counting opening fence characters (minimum 3)
- Matching the closing fence
- Storing the info string
- Managing `TryContinue` automatically

This is the pattern used by `CustomContainerParser` (3+ colons `:::`) — it's extremely concise.

## The BlockProcessor state

Inside `TryOpen` and `TryContinue`, you have access to the `BlockProcessor` which provides:

{.table}
| Property | Description |
|---|---|
| `processor.Line` | Current source line as a `StringSlice` |
| `processor.CurrentChar` | Character at current position |
| `processor.Column` | Current column number |
| `processor.Indent` | Columns since the last indent reference point |
| `processor.IsCodeIndent` | `true` if indent ≥ 4 |
| `processor.IsBlankLine` | `true` if the line is empty |
| `processor.LineIndex` | Zero-based line number in the source |
| `processor.NewBlocks` | Stack where you push newly created blocks |
| `processor.CurrentContainer` | The innermost open container block |
| `processor.Document` | The root `MarkdownDocument` |

### Useful StringSlice methods

The `processor.Line` is a `StringSlice` — a window into the source text:

{.table}
| Method | Description |
|---|---|
| `CurrentChar` | Character at current position |
| `PeekChar(int offset)` | Look ahead without advancing |
| `NextChar()` | Advance by one character |
| `SkipChar()` | Skip the current character |
| `TrimStart()` | Skip leading whitespace |
| `IsEmpty` | True if no characters remain |
| `Start` / `End` | Start/end positions (mutable) |

## CanInterrupt

The `CanInterrupt` method controls whether your parser can interrupt an open paragraph. By default it returns `true`, meaning your block syntax can start in the middle of a paragraph. Override to return `false` if you want your syntax to only work at the start of a block context.

```csharp
public override bool CanInterrupt(BlockProcessor processor, Block block)
{
    // Only allow after a blank line, not in the middle of a paragraph
    return false;
}
```

## The Close method

`Close` is called when a block is being finalized. Return `false` to **remove the block from the AST** (useful if the block turned out to be invalid):

```csharp
public override bool Close(BlockProcessor processor, Block block)
{
    var myBlock = (MyBlock)block;
    if (!myBlock.IsValid)
    {
        return false; // Remove from AST
    }
    return true; // Keep in AST
}
```

## Trivia tracking

When `TrackTrivia` is enabled, use `processor.TakeLinesBefore()` in `TryOpen` to capture pending blank/trivia lines:

```csharp
public override BlockState TryOpen(BlockProcessor processor)
{
    // ... detection logic ...

    var block = new MyBlock(this);
    block.LinesBefore = processor.TakeLinesBefore();

    processor.NewBlocks.Push(block);
    return BlockState.Continue;
}
```

## Next steps

- [Inline parsers](inline-parsers.md) — Write parsers for inline elements
- [Renderers](renderers.md) — Create HTML renderers for your custom blocks
- [Creating extensions](creating-extensions.md) — Wire everything together
