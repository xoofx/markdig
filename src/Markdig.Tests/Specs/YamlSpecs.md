# Extensions

Adds support for YAML frontmatter parsing:

## YAML frontmatter discard
 
If a frontmatter is present, it will not be rendered:

```````````````````````````````` example
---
this: is a frontmatter
---
This is a text
.
<p>This is a text</p>
````````````````````````````````
 
But if a frontmatter doesn't happen on the first line, it will be parse as regular Markdown content

```````````````````````````````` example
This is a text1
---
this: is a frontmatter
---
This is a text2
.
<h2>This is a text1</h2>
<h2>this: is a frontmatter</h2>
<p>This is a text2</p>
````````````````````````````````

It expects an exact 3 dashes `---`:

```````````````````````````````` example
----
this: is a frontmatter
----
This is a text
.
<hr />
<h2>this: is a frontmatter</h2>
<p>This is a text</p>
````````````````````````````````

