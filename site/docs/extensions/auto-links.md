---
title: Auto-links
---

# Auto-links

Enable with `.UseAutoLinks()` (included in `UseAdvancedExtensions()`).

This extension automatically detects URLs and email addresses in plain text and converts them into clickable links — no angle brackets or explicit Markdown link syntax required.

## Detected patterns

URLs starting with these protocols are detected automatically:

- `http://`
- `https://`
- `ftp://`
- `mailto:`
- `www.` (rendered as `http://www.`)

```markdown
Check out https://github.com/xoofx/markdig for more info.

Visit www.example.com for details.

Contact support@example.com for help.
```

Check out https://github.com/xoofx/markdig for more info.

Visit www.example.com for details.

## HTML output

```html
<p>Check out <a href="https://github.com/xoofx/markdig">https://github.com/xoofx/markdig</a> for more info.</p>
```

## Options

`UseAutoLinks` accepts an `AutoLinkOptions` object:

```csharp
using Markdig.Extensions.AutoLinks;

var pipeline = new MarkdownPipelineBuilder()
    .UseAutoLinks(new AutoLinkOptions
    {
        OpenInNewWindow = true,  // Add target="_blank"
        UseHttpsForWWWLinks = true // www. links become https:// instead of http://
    })
    .Build();
```

## Difference from CommonMark autolinks

CommonMark already supports **angle-bracket autolinks** (`<https://example.com>`). This extension goes further by detecting bare URLs without any markers.
