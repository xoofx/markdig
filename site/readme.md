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
<!-- Playground section -->
<section id="playground" class="container my-5" data-api-url="{{site.playground_api_url}}">
  <div class="card">
    <div class="card-header display-6">
      <i class="bi bi-play-circle lunet-feature-icon lunet-icon--controls"></i> Playground
    </div>
    <div class="card-body">
      <p class="card-text mb-3">
        Try Markdig live with the public API. Edit Markdown, pick extensions, and click <strong>Run</strong> (or press <kbd>Ctrl</kbd>+<kbd>Enter</kbd>).
      </p>
      <div class="row gx-3 gy-3">
        <div class="col-lg-8">
          <label for="playground-markdown" class="form-label fw-bold"><i class="bi bi-markdown"></i> Markdown input</label>
          <textarea id="playground-markdown" class="form-control font-monospace" rows="14" spellcheck="false"></textarea>
          <div class="form-text">The public API truncates input to the first 1000 characters.</div>
        </div>
        <div class="col-lg-4">
          <label for="playground-extensions" class="form-label fw-bold"><i class="bi bi-sliders2"></i> Extensions</label>
          <select id="playground-extensions" class="form-select mb-3">
            <option value="advanced" selected>advanced</option>
            <option value="common">common</option>
            <option value="common+pipetables+tasklists+footnotes">common+pipetables+tasklists+footnotes</option>
            <option value="advanced+nohtml">advanced+nohtml</option>
            <option value="common+autoidentifiers+mathematics">common+autoidentifiers+mathematics</option>
          </select>
          <div class="small text-secondary">
            This uses <code>MarkdownPipelineBuilder.Configure(...)</code> on the remote playground service.
          </div>
          <div class="mt-3">
            <a href="https://markdig.azurewebsites.net/" target="_blank" rel="noopener noreferrer" class="btn btn-outline-secondary btn-sm">
              <i class="bi bi-box-arrow-up-right"></i> Open service
            </a>
          </div>
        </div>
      </div>
      <div class="mt-3 d-flex align-items-center gap-2">
        <button id="playground-run" class="btn btn-primary" disabled>
          <i class="bi bi-play-fill"></i> Run
        </button>
        <span id="playground-status" class="small text-secondary"><i class="bi bi-hourglass-split"></i> Checking service availability…</span>
      </div>
      <div class="row gx-3 gy-3 mt-1">
        <div class="col-lg-6">
          <label for="playground-html" class="form-label fw-bold"><i class="bi bi-filetype-html"></i> Generated HTML</label>
          <pre id="playground-html" class="border rounded p-3 bg-body-tertiary" style="min-height: 14rem; white-space: pre-wrap;"><code>&lt;h1&gt;Markdig Playground&lt;/h1&gt;
&lt;p&gt;Markdig is a &lt;strong&gt;fast&lt;/strong&gt; and extensible Markdown processor for .NET.&lt;/p&gt;
</code></pre>
        </div>
        <div class="col-lg-6">
          <label for="playground-preview" class="form-label fw-bold"><i class="bi bi-eye"></i> Preview</label>
          <div id="playground-preview" class="border rounded p-3 bg-body-tertiary" style="min-height: 14rem;">
            <h1>Markdig Playground</h1>
            <p>Markdig is a <strong>fast</strong> and extensible Markdown processor for .NET.</p>
          </div>
        </div>
      </div>
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

<script>
(function () {
  "use strict";

  var section = document.getElementById("playground");
  if (!section) return;

  var apiUrl = (section.getAttribute("data-api-url") || "").replace(/\/+$/, "");
  var markdownEl = document.getElementById("playground-markdown");
  var extensionsEl = document.getElementById("playground-extensions");
  var runButton = document.getElementById("playground-run");
  var statusEl = document.getElementById("playground-status");
  var htmlEl = document.getElementById("playground-html");
  var previewEl = document.getElementById("playground-preview");
  var defaultMarkdown = [
    "# Markdig Playground",
    "",
    "Markdig is a **fast** and extensible Markdown processor for .NET.",
    "",
    "- [x] Task list support",
    "- [ ] Pipe table support",
    "- [ ] Footnotes",
    "",
    "| Feature | Enabled |",
    "| ------- | :-----: |",
    "| CommonMark | ✅ |",
    "| Extensions | ✅ |",
    "",
    "[^note]: Footnotes require an extension set that includes `footnotes`."
  ].join("\n");

  if (!markdownEl.value) {
    markdownEl.value = defaultMarkdown;
  }

  function setStatus(html, cssClass) {
    statusEl.className = "small " + (cssClass || "text-secondary");
    statusEl.innerHTML = html;
  }

  function setBusy(isBusy) {
    runButton.disabled = isBusy;
    runButton.classList.toggle("btn-secondary", isBusy);
    runButton.classList.toggle("btn-primary", !isBusy);
  }

  function runPlayground() {
    if (runButton.disabled) return;

    setBusy(true);
    setStatus('<i class="bi bi-hourglass-split"></i> Rendering…', "text-info");
    htmlEl.textContent = "";
    previewEl.innerHTML = "";

    var query = new URLSearchParams({
      text: markdownEl.value || "",
      extension: extensionsEl.value || "advanced"
    });

    fetch(apiUrl + "/api/to_html?" + query.toString(), { method: "GET", mode: "cors" })
      .then(function (response) {
        if (!response.ok) throw new Error("HTTP " + response.status);
        return response.json();
      })
      .then(function (payload) {
        var html = payload && typeof payload.html === "string" ? payload.html : "";
        htmlEl.textContent = html;

        if (html.toLowerCase().startsWith("exception:")) {
          previewEl.textContent = html;
          setStatus('<i class="bi bi-exclamation-triangle"></i> Rendering error', "text-danger");
          return;
        }

        previewEl.innerHTML = html;
        var version = payload && payload.version ? " (v" + payload.version + ")" : "";
        setStatus('<i class="bi bi-check-circle"></i> Done' + version, "text-success");
      })
      .catch(function (error) {
        htmlEl.textContent = "Request failed: " + error.message;
        previewEl.textContent = "Unable to reach the Markdig playground service.";
        setStatus('<i class="bi bi-x-circle"></i> Service unavailable or blocked by CORS', "text-danger");
      })
      .finally(function () {
        setBusy(false);
      });
  }

  if (!apiUrl) {
    setStatus('<i class="bi bi-exclamation-triangle"></i> Playground API URL not configured.', "text-warning");
    return;
  }

  setBusy(true);
  fetch(apiUrl + "/api/to_html?text=health&extension=common", { method: "GET", mode: "cors" })
    .then(function (response) {
      if (!response.ok) throw new Error("HTTP " + response.status);
      return response.json();
    })
    .then(function (payload) {
      var version = payload && payload.version ? " (v" + payload.version + ")" : "";
      setStatus('<i class="bi bi-check-circle"></i> Service available' + version, "text-success");
      setBusy(false);
    })
    .catch(function () {
      setStatus('<i class="bi bi-x-circle"></i> Service unavailable or blocked by CORS', "text-danger");
      htmlEl.textContent = "The current browser cannot access the remote API endpoint.";
      previewEl.textContent = "Use the 'Open service' button to try the playground directly.";
    });

  runButton.addEventListener("click", runPlayground);
  document.addEventListener("keydown", function (event) {
    if ((event.ctrlKey || event.metaKey) && event.key === "Enter") {
      event.preventDefault();
      runPlayground();
    }
  });
})();
</script>

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
