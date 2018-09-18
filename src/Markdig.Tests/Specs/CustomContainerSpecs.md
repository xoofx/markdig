# Extensions

This section describes the different extensions supported:

## Custom Container

A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</div>` block.

```````````````````````````````` example
:::spoiler
This is a *spoiler*
:::
.
<div class="spoiler"><p>This is a <em>spoiler</em></p>
</div>
````````````````````````````````

The text following the opened custom container is optional:

```````````````````````````````` example
:::
This is a regular div
:::
.
<div><p>This is a regular div</p>
</div>
````````````````````````````````

Like for fenced code block, you can use more than 3 `:` characters as long as the closing has the same number of characters:


```````````````````````````````` example
::::::::::::spoiler
This is a spoiler
::::::::::::
.
<div class="spoiler"><p>This is a spoiler</p>
</div>
````````````````````````````````

Like for fenced code block, a custom container can span over multiple empty lines in a list block:

```````````````````````````````` example
- This is a list
  :::spoiler
  This is a spoiler
  - item1
  - item2
  :::
- A second item in the list
.
<ul>
<li>This is a list
<div class="spoiler">This is a spoiler
<ul>
<li>item1</li>
<li>item2</li>
</ul>
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
<div id="myspoiler" class="spoiler" myprop="yes"><p>This is a spoiler</p>
</div>
````````````````````````````````

The content of a custom container can contain any blocks:

```````````````````````````````` example
:::mycontainer
<p>This is a raw spoiler</p>
:::
.
<div class="mycontainer"><p>This is a raw spoiler</p>
</div>
````````````````````````````````

## Inline Custom Container 

A custom container can also be used within an inline container (e.g: paragraph, heading...) by enclosing a text by a new emphasis `::`

```````````````````````````````` example
This is a text ::with special emphasis::
.
<p>This is a text <span>with special emphasis</span></p>
````````````````````````````````

Any other emphasis inline can be used within this emphasis inline container:

```````````````````````````````` example
This is a text ::with special *emphasis*::
.
<p>This is a text <span>with special <em>emphasis</em></span></p>
````````````````````````````````

Attributes can be attached to a inline custom container:


```````````````````````````````` example
This is a text ::with special emphasis::{#myId .myemphasis}
.
<p>This is a text <span id="myId" class="myemphasis">with special emphasis</span></p>
````````````````````````````````
