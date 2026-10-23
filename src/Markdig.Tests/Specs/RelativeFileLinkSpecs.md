# Extensions

This section describes the different extensions supported:

## Relative File Links

The relative file link extension rewrites links to Markdown files into links to the HTML pages generated from them, so that the same files can be browsed on GitHub and as a generated site. It is enabled with `UseRelativeFileLinks()`.

A link whose path ends with `.md` or `.markdown`, compared case-insensitively, has that extension replaced with `.html`:

```````````````````````````````` example
[Usage](usage.md)

[Read me](README.MD)

[Guide](guide.markdown)
.
<p><a href="usage.html">Usage</a></p>
<p><a href="README.html">Read me</a></p>
<p><a href="guide.html">Guide</a></p>
````````````````````````````````

The query and the fragment are kept:

```````````````````````````````` example
[Preview](usage.md#preview)

[Version](usage.md?v=1#preview)
.
<p><a href="usage.html#preview">Preview</a></p>
<p><a href="usage.html?v=1#preview">Version</a></p>
````````````````````````````````

Relative and root-relative paths are rewritten, and so are the targets of reference links:

```````````````````````````````` example
[Usage](./usage.md)

[Install](../guide/install.md)

[Docs](/docs/usage.md)

[Linux][linux]

[linux]: guide/install.md#linux
.
<p><a href="./usage.html">Usage</a></p>
<p><a href="../guide/install.html">Install</a></p>
<p><a href="/docs/usage.html">Docs</a></p>
<p><a href="guide/install.html#linux">Linux</a></p>
````````````````````````````````

Only the link target changes. The link text and the title stay as written, and the target is escaped as usual:

```````````````````````````````` example
[usage.md](<my usage.md> "usage.md")
.
<p><a href="my%20usage.html" title="usage.md">usage.md</a></p>
````````````````````````````````

Links with a scheme and protocol-relative links point outside the site and are left unchanged, email autolinks included:

```````````````````````````````` example
[Source](https://github.com/xoofx/markdig/blob/main/readme.md)

[CDN](//cdn.example.com/usage.md)

[Mail](mailto:info@example.md)

<info@example.md>
.
<p><a href="https://github.com/xoofx/markdig/blob/main/readme.md">Source</a></p>
<p><a href="//cdn.example.com/usage.md">CDN</a></p>
<p><a href="mailto:info@example.md">Mail</a></p>
<p><a href="mailto:info@example.md">info@example.md</a></p>
````````````````````````````````

A link is left unchanged when its path does not end with a Markdown extension, even if the query or the fragment does:

```````````````````````````````` example
[Notes](notes.html#usage.md)

[Download](get.html?file=usage.md)

[Anchor](#usage.md)

[MDX](usage.mdx)

[Folder](docs.md/)
.
<p><a href="notes.html#usage.md">Notes</a></p>
<p><a href="get.html?file=usage.md">Download</a></p>
<p><a href="#usage.md">Anchor</a></p>
<p><a href="usage.mdx">MDX</a></p>
<p><a href="docs.md/">Folder</a></p>
````````````````````````````````

Links written as raw HTML are not rewritten:

```````````````````````````````` example
<a href="usage.md">Usage</a>
.
<p><a href="usage.md">Usage</a></p>
````````````````````````````````
