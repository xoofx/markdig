---
title: Home
layout: simple
og_type: website
---

<section class="text-center py-5">
  <div class="container">
    <img src="{{site.basepath}}/img/markdig.svg" alt="Markdig — Fast, powerful Markdown processor for .NET" class="img-fluid" style="width: min(100%, 12rem); height: auto;">
    <h1 class="display-4 mt-4">Markdig</h1>
    <p class="lead mt-3 mb-4">
      A fast, powerful, <strong>CommonMark compliant</strong>, extensible <strong>Markdown processor</strong> for .NET.
    </p>
    <div class="d-flex justify-content-center gap-3 mt-4 flex-wrap">
      <a href="{{site.basepath}}/docs/getting-started/" class="btn btn-primary btn-lg"><i class="bi bi-rocket-takeoff"></i> Get started</a>
      <a href="{{site.basepath}}/docs/" class="btn btn-outline-secondary btn-lg"><i class="bi bi-book"></i> Documentation</a>
      <a href="https://github.com/xoofx/markdig" class="btn btn-info btn-lg"><i class="bi bi-github"></i> GitHub</a>
    </div>
    <div class="mt-4 text-start mx-auto" style="max-width: 48rem;">
      <pre class="language-shell-session"><code>dotnet add package Markdig</code></pre>
      <p class="text-center text-secondary mt-2" style="font-size: 0.85rem;">Available on <a href="https://www.nuget.org/packages/Markdig/" class="text-secondary">NuGet</a> — .NET Standard 2.0+</p>
    </div>
  </div>
</section>

<!-- Feature cards -->
<section class="container my-5">
  <div class="row row-cols-1 row-cols-lg-2 gx-5 gy-4">
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-lightning-charge lunet-feature-icon lunet-icon--controls"></i> Blazing fast</div>
        <div class="card-body">
          <p class="card-text">
            Regex-free parser and HTML renderer with minimal GC pressure. <strong>20% faster than the reference C implementation</strong> (cmark) and 100× faster than regex-based processors.
          </p>

[Getting started](docs/getting-started.md) · [Usage guide](docs/usage.md)

</div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-check-circle lunet-feature-icon lunet-icon--themes"></i> CommonMark compliant</div>
        <div class="card-body">
          <p class="card-text">
            Passes <strong>600+ tests</strong> from the latest CommonMark specification (0.31.2). Full support for all core Markdown constructs including GFM fenced code blocks.
          </p>

[CommonMark syntax](docs/commonmark.md)

</div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-puzzle lunet-feature-icon lunet-icon--data"></i> 20+ extensions</div>
        <div class="card-body">
          <p class="card-text">
            Tables, task lists, math, diagrams, footnotes, emoji, alert blocks, abbreviations, and more — all included out of the box. Enable them individually or all at once.
          </p>

[Extensions](docs/extensions/readme.md)

</div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-gear lunet-feature-icon lunet-icon--editing"></i> Fully extensible</div>
        <div class="card-body">
          <p class="card-text">
            Pluggable architecture — even core parsing is customizable. Create your own block parsers, inline parsers, and renderers with a clean API.
          </p>

[Developer guide](docs/advanced/readme.md)

</div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-tree lunet-feature-icon lunet-icon--chrome"></i> Rich AST</div>
        <div class="card-body">
          <p class="card-text">
            Full abstract syntax tree with precise source locations. Power syntax highlighters, editors, and document analysis tools with the complete <code>Descendants</code> API.
          </p>

[AST guide](docs/advanced/ast.md)

</div>
      </div>
    </div>
    <div class="col">
      <div class="card h-100">
        <div class="card-header display-6"><i class="bi bi-arrow-repeat lunet-feature-icon lunet-icon--binding"></i> Roundtrip support</div>
        <div class="card-body">
          <p class="card-text">
            Parse with trivia tracking for lossless parse → render roundtrips. Modify Markdown documents programmatically without introducing unwanted changes.
          </p>

[Pipeline &amp; roundtrip](docs/advanced/pipeline.md)

</div>
      </div>
    </div>
  </div>
</section>

<section class="container my-5">
  <div class="card">
    <div class="card-header display-6">
      <i class="bi bi-code-slash lunet-feature-icon lunet-icon--lists"></i> Quick example
    </div>
    <div class="card-body">

```csharp
using Markdig;

// Simple CommonMark conversion
var result = Markdown.ToHtml("This is a text with some *emphasis*");
// => "<p>This is a text with some <em>emphasis</em></p>\n"

// Enable all advanced extensions
var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
var result = Markdown.ToHtml("This is a ~~strikethrough~~", pipeline);
// => "<p>This is a <del>strikethrough</del></p>\n"
```

For more examples, see the [Getting started](docs/getting-started.md) guide.

</div>
  </div>
</section>
