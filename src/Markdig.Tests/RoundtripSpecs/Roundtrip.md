# Roundtrip support
Roundtrip support allows parsing of Markdown to subsequently render it back to Markdown without changes. This requires storing all characters on the parse tree, including whitespace and special characters. This document outlines decisions and guidelines on how these characters are stored.

# Guidelines
- newlines before blocks are assigned to that block
- whitespace starting on a line is assigned to the block on that line

## Quoteblock
Quoteblocks may have different syntactical characters applied per line. That is, some lines belonging to a Quoteblock may and others **may not** contain the quote marker character `>`. Each line of a Quoteblock therefore stores the quote marker character and its surrounding whitespace.

## Lists
- beforewhitespace on list item

# TODO
In order:
- `p\n p`: affects many tests
- `\r\n` and `\r` support
- support link parsing
- support LinkReferenceDefinition
- fix broken pre-existing tests
- fix `TODO: RTP: `
- generate spec examples as tests for roundtrip
- run pull request feedback
- introduce feature flag
- extract MarkdownRenderer
- deduplicate MarkdownRenderer and NormalizeRenderer code
- `\0`
- support extensions
- review complete PR and follow conventions
- run perf test
- document how trivia are handled generically and specifically
- create todo list with perf optimization focus points
- write tree comparison tests?
- write tree visualization tool?
