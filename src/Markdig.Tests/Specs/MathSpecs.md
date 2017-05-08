# Extensions

Adds support for mathematics spans:

## Math Inline
 
Allows to define a mathematic block embraced by `$...$`

```````````````````````````````` example
This is a $math block$
.
<p>This is a <span class="math">math block</span></p>
````````````````````````````````

Or by `$$...$$` embracing it by:

```````````````````````````````` example
This is a $$math block$$
.
<p>This is a <span class="math">math block</span></p>
````````````````````````````````

The opening `$` and closing `$` is following the rules of the emphasis delimiter `_`:

```````````````````````````````` example
This is not a $ math block $
.
<p>This is not a $ math block $</p>
````````````````````````````````

For the opening `$` it requires a space or a punctuation before (but cannot be used within a word):

```````````````````````````````` example
This is not a m$ath block$
.
<p>This is not a m$ath block$</p>
````````````````````````````````

For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):

```````````````````````````````` example
This is not a $math bloc$k
.
<p>This is not a $math bloc$k</p>
````````````````````````````````

For the closing `$` it requires a space after or a punctuation (but cannot be preceded by a space and cannot be used within a word):

```````````````````````````````` example
This is should not match a 16$ or a $15
.
<p>This is should not match a 16$ or a $15</p>
````````````````````````````````

A `$` can be escaped between a math inline block by using the escape `\\` 

```````````````````````````````` example
This is a $math \$ block$
.
<p>This is a <span class="math">math \$ block</span></p>
````````````````````````````````

At most, two `$` will be matched for the opening and closing:

```````````````````````````````` example
This is a $$$math block$$$
.
<p>This is a <span class="math">$math block$</span></p>
````````````````````````````````

Regular text can come both before and after the math inline

```````````````````````````````` example
This is a $math block$ with text on both sides.
.
<p>This is a <span class="math">math block</span> with text on both sides.</p>
````````````````````````````````
A mathematic block takes precedence over standard emphasis `*` `_`:

```````````````````````````````` example
This is *a $math* block$
.
<p>This is *a <span class="math">math* block</span></p>
````````````````````````````````
An opening $$ at the beginning of a line should not be interpreted as a Math block:

```````````````````````````````` example
$$ math $$ starting at a line
.
<p><span class="math">math</span> starting at a line</p>
````````````````````````````````

## Math Block

The match block can spawn on multiple lines by having a $$ starting on a line.
It is working as a fenced code block.

```````````````````````````````` example
$$
\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
$$
.
<div class="math">\begin{equation}
  \int_0^\infty \frac{x^3}{e^x-1}\,dx = \frac{\pi^4}{15}
  \label{eq:sample}
\end{equation}
</div>
````````````````````````````````
