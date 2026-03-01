---
title: CJK friendly emphasis
---

# CJK friendly emphasis

Enable with `.UseCjkFriendlyEmphasis()` (not included in `UseAdvancedExtensions()`).

This extension adjusts emphasis delimiter rules to better support **Chinese/Japanese/Korean (CJK)** text, where words are commonly written without spaces. It follows the [markdown-cjk-friendly specification](https://github.com/tats-u/markdown-cjk-friendly/) and mitigates the CommonMark emphasis limitation discussed in [commonmark-spec#650](https://github.com/commonmark/commonmark-spec/issues/650).

## Enable

```csharp
using Markdig;

var pipeline = new MarkdownPipelineBuilder()
    .UseCjkFriendlyEmphasis()
    .Build();
```

You can also enable it via string configuration:

```csharp
var pipeline = new MarkdownPipelineBuilder()
    .Configure("common+cjk-friendly-emphasis")
    .Build();
```

## Examples

Some emphasis sequences that often fail with plain CommonMark in CJK text become parseable with this extension enabled:

```markdown
**この文を強調できますか？**残念ながらできません。
我可以强调**这个`code`**吗？
**이 용어(This term)**를 강조해 주세요.
```

## Notes

- This extension intentionally deviates from strict CommonMark emphasis behavior to improve real-world CJK authoring.
- It only affects how emphasis delimiters are interpreted; it does not add new syntax.
