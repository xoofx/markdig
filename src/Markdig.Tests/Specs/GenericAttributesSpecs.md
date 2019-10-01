# Extensions

This section describes the different extensions supported:

## Generic Attributes

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

# This is a heading # {#heading-link2}

[This is a link](http://google.com){#a-link .myclass data-lang=fr data-value="This is a value"}

This is a heading{#heading-link2}
-----------------

This is a paragraph with an attached attributes {#myparagraph attached-bool-property attached-bool-property2}
.
<h1 id="heading-link">This is a heading with an an attribute</h1>
<h1 id="heading-link2">This is a heading</h1>
<p><a href="http://google.com" id="a-link" class="myclass" data-lang="fr" data-value="This is a value">This is a link</a></p>
<h2 id="heading-link2">This is a heading</h2>
<p id="myparagraph" attached-bool-property="" attached-bool-property2="">This is a paragraph with an attached attributes </p>
````````````````````````````````

The following shows that attributes can be attached to the next block if they are used inside a single line just preceding the block (and preceded by a blank line or beginning of a block container):

```````````````````````````````` example
{#fenced-id .fenced-class}
~~~
This is a fenced with attached attributes
~~~ 
.
<pre><code id="fenced-id" class="fenced-class">This is a fenced with attached attributes
</code></pre>
````````````````````````````````

Attribute values can be one character long

```````````````````````````````` example
[Foo](url){data-x=1}

[Foo](url){data-x='1'}

[Foo](url){data-x=11}
.
<p><a href="url" data-x="1">Foo</a></p>
<p><a href="url" data-x="1">Foo</a></p>
<p><a href="url" data-x="11">Foo</a></p>
````````````````````````````````
