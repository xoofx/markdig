## Jira Links

The JiraLinks extension will automatically add links to JIRA issue items within your markdown, e.g. XX-1234. For this to happen, you must configure the extension when adding to the pipeline, e.g.

```
var pipeline = new MarkdownPipelineBuilder()
	.UseJiraLinks(new JiraLinkOptions("http://your.company.abc"))
	.Build();
```

The rules for detecting a link are:

- The project key must be composed of one or more capitalized ASCII letters or digits `[A-Z,0-9]+`
- A single hyphen `-` must separate the project key and issue number.
- The issue number is composed of 1 or more digits `[0, 9]+`
- The reference must be preceded by either `(` or whitespace or EOF.
- The reference must be followed by either `)` or whitespace or EOF.

The following are valid examples:

```````````````````````````````` example
This is a ABCD-123 issue
.
<p>This is a <a href="http://your.company.abc/browse/ABCD-123" target="_blank">ABCD-123</a> issue</p>
````````````````````````````````

```````````````````````````````` example
This is a ABC4-123 issue
.
<p>This is a <a href="http://your.company.abc/browse/ABC4-123" target="_blank">ABC4-123</a> issue</p>
````````````````````````````````

```````````````````````````````` example
This is a ABC45-123 issue
.
<p>This is a <a href="http://your.company.abc/browse/ABC45-123" target="_blank">ABC45-123</a> issue</p>
````````````````````````````````

```````````````````````````````` example
This is a KIRA-1 issue
.
<p>This is a <a href="http://your.company.abc/browse/KIRA-1" target="_blank">KIRA-1</a> issue</p>
````````````````````````````````

```````````````````````````````` example
This is a Z-1 issue
.
<p>This is a <a href="http://your.company.abc/browse/Z-1" target="_blank">Z-1</a> issue</p>
````````````````````````````````

These are also valid links with `(` and `)`:

```````````````````````````````` example
This is a (ABCD-123) issue
.
<p>This is a (<a href="http://your.company.abc/browse/ABCD-123" target="_blank">ABCD-123</a>) issue</p>
````````````````````````````````

```````````````````````````````` example
This is a (ABC4-123) issue
.
<p>This is a (<a href="http://your.company.abc/browse/ABC4-123" target="_blank">ABC4-123</a>) issue</p>
````````````````````````````````

```````````````````````````````` example
This is a (KIRA-1) issue
.
<p>This is a (<a href="http://your.company.abc/browse/KIRA-1" target="_blank">KIRA-1</a>) issue</p>
````````````````````````````````

```````````````````````````````` example
This is a (Z-1) issue
.
<p>This is a (<a href="http://your.company.abc/browse/Z-1" target="_blank">Z-1</a>) issue</p>
````````````````````````````````

These are not valid links:

```````````````````````````````` example
This is not aJIRA-123 issue
.
<p>This is not aJIRA-123 issue</p>
````````````````````````````````

```````````````````````````````` example
This is not 4JIRA-123 issue
.
<p>This is not 4JIRA-123 issue</p>
````````````````````````````````

```````````````````````````````` example
This is not JIRA-123a issue
.
<p>This is not JIRA-123a issue</p>
````````````````````````````````

```````````````````````````````` example
This is not JIRA- issue
.
<p>This is not JIRA- issue</p>
````````````````````````````````

```````````````````````````````` example
This is not JIR4- issue
.
<p>This is not JIR4- issue</p>
````````````````````````````````
