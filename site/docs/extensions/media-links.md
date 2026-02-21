---
title: Media links
---

# Media links

Enable with `.UseMediaLinks()` (included in `UseAdvancedExtensions()`).

This extension converts image links (`![...](url)`) pointing to known media services into embedded players. When a Markdown image link targets a YouTube video, Vimeo clip, or other supported media URL, Markdig renders an `<iframe>` instead of an `<img>`.

## Supported services

{.table}
| Service | URL pattern |
|---|---|
| YouTube | `youtube.com/watch?v=...`, `youtu.be/...` |
| Vimeo | `vimeo.com/...` |
| Dailymotion | `dailymotion.com/video/...` |
| Yandex | `video.yandex.ru/...` |
| Odnoklassniki | `ok.ru/video/...` |

## Syntax

Use standard Markdown image syntax with a media URL:

```markdown
![Video title](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
```

## HTML output

```html
<iframe src="https://www.youtube.com/embed/dQw4w9WgXcQ"
        width="500" height="281"
        frameborder="0"
        allowfullscreen></iframe>
```

## Direct media files

For direct links to media files the extension supports:

{.table}
| Format | Type |
|---|---|
| `.mp4`, `.webm`, `.ogg` | `<video>` element |
| `.mp3`, `.wav`, `.ogg` | `<audio>` element |

```markdown
![My video](video.mp4)
```

Produces:

```html
<video width="500" height="281" controls>
  <source type="video/mp4" src="video.mp4"></source>
</video>
```

## Options

```csharp
using Markdig.Extensions.MediaLinks;

var options = new MediaOptions
{
    Width = "800",
    Height = "450",
    AddControlsProperty = true
};

var pipeline = new MarkdownPipelineBuilder()
    .UseMediaLinks(options)
    .Build();
```
