# Extensions

This section describes the different extensions supported:

## Attributes

Attributes can be attached to:
- The previous inline element if the previous element is not a literal
- The next block if the current block is a paragraph and the attributes is the only inline present in the paragraph
- Or the current block

Attributes can be of 3 kinds:

- An id element, starting by `#` that will be used to set the `id` property of the HTML element
- A class element, starting by `.` that will be appended to the CSS class property of the HTML element
- a `name=value` or `name="value"` that will be appended as an attribute of the HTML element

The following shows that attributes is attached to the current block or the previous inline:

```````````````````````````````` example
# This is a heading with an an attribute{#heading-link}

[This is a link](http://google.com){#a-link .myclass data-lang=fr data-value="This is a value"}

This is a heading{#heading-link2}
-----------------
.
<h1 id="heading-link">This is a heading with an an attribute</h1>
<p><a href="http://google.com" id="a-link" class="myclass" data-lang="fr" data-value="This is a value">This is a link</a></p>
<h2 id="heading-link2">This is a heading</h2>
````````````````````````````````

The following shows that the attributes is attached to the next fenced code block:

```````````````````````````````` example
{#fenced-id .fenced-class}
~~~
This is a fenced with attached attributes
~~~ 
.
<pre><code id="fenced-id" class="fenced-class">This is a fenced with attached attributes
</code></pre>
````````````````````````````````
