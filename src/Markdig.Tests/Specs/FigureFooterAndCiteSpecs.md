# Extensions

The following the figure extension:

## Figures
 
A figure can be defined by using a pattern equivalent to a fenced code block but with the character `^`

```````````````````````````````` example
^^^
This is a figure
^^^ This is a *caption*
.
<figure>
<p>This is a figure</p>
<figcaption>This is a <em>caption</em></figcaption>
</figure>
````````````````````````````````

## Footers

A footer equivalent to a block quote parsing but starts with double character ^^

```````````````````````````````` example
^^ This is a footer
^^ multi-line
.
<footer>This is a footer
multi-line</footer>
````````````````````````````````

## Cite

A cite is working like an emphasis but using the double character ""

```````````````````````````````` example
This is a ""citation of someone""
.
<p>This is a <cite>citation of someone</cite></p>
````````````````````````````````
