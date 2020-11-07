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
- ~~check char.IsWhitespace() calls~~
- ~~check char.IsNewline() calls~~
- ~~introduce feature flag~~
- ~~extract MarkdownRenderer~~
- ~~cleanup NormalizeRenderer (MarkdownRenderer)~~
- ~~deduplicate MarkdownRenderer and NormalizeRenderer code~~
- ~~merge from main~~
- ~~fix broken pre-existing tests~~
- ~~use StringSlice where possible instead of String~~
- ~~document newly added syntax properties~~
- ~~review complete PR and follow conventions~~
- fix `TODO: RTP: `
- do pull request feedback
- support extensions
- run perf test
- create todo list with perf optimization focus points
- optimize perf
- `\0`
- split HeadingBlock into AtxHeadingBlock and SetextHeadingBlock?
- document how trivia are handled generically and specifically
- write tree comparison tests?
- write tree visualization tool?

# Pull request discussion
- LinkHelper duplication
  - keep current?
  - if not, how to deduplicate?
- StringSlice vs String
  - StringSlice is preferred
- amount of tests
  - should we create even more permutations using `\v`, `\f`?
- newlines
  - Newline struct itself
  - handling newlines
  - should newlines be supported?
  - LineBreakInline now also parses /r and /r/n: this effectively removes an optimization
- Example 207, 209, 291: Special-casing certain edgecases
- TrackTrivia flag trickling down into 5-6 classes
  - InlineProcessor
  - BlockProcessor
  - MarkdownParser
  - Markdown static class
  - LinkReferenceDefinitionHelper
