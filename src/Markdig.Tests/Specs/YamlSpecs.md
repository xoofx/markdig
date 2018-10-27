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

It can end with three dots `...`:

```````````````````````````````` example
---
this: is a frontmatter

...
This is a text
.
<p>This is a text</p>
````````````````````````````````

If the end front matter marker (`...` or `---`) is not present, it will render the `---` has a `<hr>`:

```````````````````````````````` example
---
this: is a frontmatter
This is a text
.
<hr />
<p>this: is a frontmatter
This is a text</p>
````````````````````````````````

It expects exactly three dots `...`:

```````````````````````````````` example
---
this: is a frontmatter
....
This is a text
.
<hr />
<p>this: is a frontmatter
....
This is a text</p>
````````````````````````````````

Front matter ends with the first line containing three dots `...` or three dashes `---`:

```````````````````````````````` example
---
this: is a frontmatter
....

Hello
---
This is a text
.
<p>This is a text</p>
````````````````````````````````

It expects whitespace can exist after the leading characters

```````````````````````````````` example
---   
this: is a frontmatter
...
This is a text
.
<p>This is a text</p>
````````````````````````````````

It expects whitespace can exist after the trailing characters

```````````````````````````````` example
---
this: is a frontmatter
...     
This is a text
.
<p>This is a text</p>
````````````````````````````````



