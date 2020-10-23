# Roundtrip support
Roundtrip support allows parsing of Markdown to subsequently render it back to Markdown without changes. This requires storing all characters on the parse tree, including whitespace and special characters. This document outlines decisions and guidelines on how these characters are stored.

# Guidelines
- newlines before blocks are assigned to that block
- whitespace starting on a line is assigned to the block on that line
- assigning whitespace *before* a node has precedence over asigning whitespace *after* a node
- whitespace vs trivia
  - AtxHeading can have #s after the title, white are including as trivia

## Quoteblock
Quoteblocks may have different syntactical characters applied per line. That is, some lines belonging to a Quoteblock may and others **may not** contain the quote marker character `>`. Each line of a Quoteblock therefore stores the quote marker character and its surrounding whitespace.

## Lists
- beforewhitespace on list item

## Trivia
- whitespace
  - ` ` (space)
  - `\t`
  - `\f`
  - `\v`
- trailing `#`
- TODO: ThematicBreak
- TODO: link url `>`, link title `(`, `'`, `"`

# TODO
In order:
- ~~`p\n p`: affects many tests~~
- ~~`\r\n` and `\r` support~~
- ~~support SetextHeading~~
- ~~support LinkReferenceDefinition~~
- ~~support link parsing~~
- ~~support AutolinkInline~~
- ~~generate spec examples as tests for roundtrip~~
- fix `TODO: RTP: `
- check char.IsWhitespace() calls
- check char.IsNewline() calls
- introduce feature flag
- extract MarkdownRenderer
- cleanup NormalizeRenderer (MarkdownRenderer)
- deduplicate MarkdownRenderer and NormalizeRenderer code
- do pull request feedback
- split HeadingBlock into AtxHeadingBlock and SetextHeadingBlock?
- fix broken pre-existing tests
- support extensions
- review complete PR and follow conventions
- run perf test
- create todo list with perf optimization focus points
- optimize perf
- `\0`
- merge from main
- document how trivia are handled generically and specifically
- write tree comparison tests?
- write tree visualization tool?

# Pull request discussion
- LinkHelper duplication
- StringSlice vs String
- amount of tests
  - should we create even more permutations using `\v`, `\f`?
- newlines
  - Newline struct itself
  - handling newlines
  - should newlines be supported?
- Example 207, 209: Special-casing certain edgecases