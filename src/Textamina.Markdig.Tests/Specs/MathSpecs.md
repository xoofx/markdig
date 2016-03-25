# Extensions

Adds support for mathematics spans:

## Math spans
 
Allows to define a mathematic block embraced by `$....$`

```````````````````````````````` example
This is a $math block$
.
<p>This is a <span class="math">math block</span></p>
````````````````````````````````

A mathematic block takes precedence over standard emphasis `*` `_`:

```````````````````````````````` example
This is *a $math* block$
.
<p>This is *a <span class="math">math* block</span></p>
````````````````````````````````
