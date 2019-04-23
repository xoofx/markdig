# Extensions

This section describes the different extensions supported:

## Definition lists

A custom container is similar to a fenced code block, but it is using the character `:` to declare a block (with at least 3 characters), and instead of generating a `<pre><code>...</code></pre>` it will generate a `<div>...</div>` block.

```````````````````````````````` example

Term 1
:   This is a definition item
    With a paragraph
    > This is a block quote

    - This is a list
    - with an item2

    ```java
    Test


    ```

    And a last line
:   This ia another definition item

Term2
Term3 *with some inline*
:   This is another definition for term2
.
<dl>
<dt>Term 1</dt>
<dd><p>This is a definition item
With a paragraph</p>
<blockquote>
<p>This is a block quote</p>
</blockquote>
<ul>
<li>This is a list</li>
<li>with an item2</li>
</ul>
<pre><code class="language-java">Test


</code></pre>
<p>And a last line</p>
</dd>
<dd>This ia another definition item</dd>
<dt>Term2</dt>
<dt>Term3 <em>with some inline</em></dt>
<dd>This is another definition for term2</dd>
</dl>
````````````````````````````````

A definition term can be followed at most by one blank line. Lazy continuations are supported for definitions:

```````````````````````````````` example
Term 1

:   Definition
with lazy continuation.

    Second paragraph of the definition.
.
<dl>
<dt>Term 1</dt>
<dd><p>Definition
with lazy continuation.</p>
<p>Second paragraph of the definition.</p>
</dd>
</dl>
````````````````````````````````

The definition must be indented to 4 characters including the `:`. 

```````````````````````````````` example
Term 1
:  Invalid with less than 3 characters
.
<p>Term 1
:  Invalid with less than 3 characters</p>
````````````````````````````````

The `:` can be indented up to 3 spaces:

```````````````````````````````` example
Term 1
   : Valid even if `:` starts at most 3 spaces
.
<dl>
<dt>Term 1</dt>
<dd>Valid even if <code>:</code> starts at most 3 spaces</dd>
</dl>
````````````````````````````````

But more than 3 spaces before `:` will trigger an indented code block:

```````````````````````````````` example
Term 1

    : Not valid
.
<p>Term 1</p>
<pre><code>: Not valid
</code></pre>
````````````````````````````````

Definition lists can be nested inside list items

```````````````````````````````` example
1.  First
    
2.  Second
    
    Term 1
    :   Definition
    
    Term 2
    :   Second Definition
.
<ol>
<li><p>First</p></li>
<li><p>Second</p>
<dl>
<dt>Term 1</dt>
<dd>Definition</dd>
<dt>Term 2</dt>
<dd>Second Definition</dd>
</dl></li>
</ol>
````````````````````````````````
