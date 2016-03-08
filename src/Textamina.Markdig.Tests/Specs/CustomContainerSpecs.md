# Extensions

This section describes the different extensions supported:

## Custom Container

A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</dib>` block.

```````````````````````````````` example
:::spoiler
This is a spoiler
:::
.
<div class="spoiler">This is a spoiler
</div>
````````````````````````````````

The text following the opened custom container is optional:

```````````````````````````````` example
:::
This is a regular div
:::
.
<div>This is a regular div
</div>
````````````````````````````````

Like for fenced code block, you can use more than 3 `:` characters as long as the closing has the same number of characters:


```````````````````````````````` example
::::::::::::spoiler
This is a spoiler
::::::::::::
.
<div class="spoiler">This is a spoiler
</div>
````````````````````````````````

Like for fenced code block, a custom container can span over multiple empty lines in a list block:

```````````````````````````````` example
- This is a list
  :::spoiler
  This is a spoiler


  :::
- A second item in the list
.
<ul>
<li>This is a list
<div class="spoiler">This is a spoiler


</div>
</li>
<li>A second item in the list</li>
</ul>
````````````````````````````````

Attributes extension is also supported for Custom Container, as long as the Attributes extension is activated after the CustomContainer extension (`.UseCustomContainer().UseAttributes()`)

```````````````````````````````` example
:::spoiler {#myspoiler myprop=yes}
This is a spoiler
:::
.
<div id="myspoiler" class="spoiler" myprop="yes">This is a spoiler
</div>
````````````````````````````````
