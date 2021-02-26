# Extensions

This section describes the different extensions supported:

## Abbreviation

Abbreviation can be declared by using the `*[Abbreviation Label]: Abbreviation description`

Abbreviation definition will be removed from the original document. Any Abbreviation label found in literals will be replaced by the abbreviation description:
 
```````````````````````````````` example
*[HTML]: Hypertext Markup Language

Later in a text we are using HTML and it becomes an abbr tag HTML
.
<p>Later in a text we are using <abbr title="Hypertext Markup Language">HTML</abbr> and it becomes an abbr tag <abbr title="Hypertext Markup Language">HTML</abbr></p>
````````````````````````````````

An abbreviation definition can be indented at most 3 spaces
 
```````````````````````````````` example
*[HTML]: Hypertext Markup Language
    *[This]: is not an abbreviation
.
<pre><code>*[This]: is not an abbreviation
</code></pre>
````````````````````````````````

An abbreviation may contain spaces:
 
```````````````````````````````` example
*[SUPER HTML]: Super Hypertext Markup Language

This is a SUPER HTML document    
.
<p>This is a <abbr title="Super Hypertext Markup Language">SUPER HTML</abbr> document</p>
````````````````````````````````

Abbreviation may contain any unicode characters:

```````````````````````````````` example
*[😃 HTML]: Hypertext Markup Language

This is a 😃 HTML document    
.
<p>This is a <abbr title="Hypertext Markup Language">😃 HTML</abbr> document</p>
````````````````````````````````

Abbreviations may be similar:

```````````````````````````````` example
*[1A]: First
*[1A1]: Second
*[1A2]: Third

We can abbreviate 1A, 1A1 and 1A2!
.
<p>We can abbreviate <abbr title="First">1A</abbr>, <abbr title="Second">1A1</abbr> and <abbr title="Third">1A2</abbr>!</p>
````````````````````````````````

Abbreviations should match whole word only:

```````````````````````````````` example
*[1A]: First

We should not abbreviate 1.1A or 11A!
.
<p>We should not abbreviate 1.1A or 11A!</p>
````````````````````````````````

Abbreviations should match whole word only, even if the word is the entire content:

```````````````````````````````` example
*[1A]: First

1.1A
.
<p>1.1A</p>
````````````````````````````````

Abbreviations should match whole word only, even if there is another glossary term:

```````````````````````````````` example
*[SCO]: First
*[SCOM]: Second

SCOM
.
<p><abbr title="Second">SCOM</abbr></p>
````````````````````````````````

Abbreviations should only match when surrounded by whitespace:

```````````````````````````````` example
*[PR]: Pull Request

PRAA
.
<p>PRAA</p>
````````````````````````````````

Single character abbreviations should be matched

```````````````````````````````` example
*[A]: Foo

A
.
<p><abbr title="Foo">A</abbr></p>
````````````````````````````````

The longest matching abbreviation should be used

```````````````````````````````` example
*[Foo]: foo
*[Foo Bar]: foobar

Foo B
.
<p><abbr title="foo">Foo</abbr> B</p>
````````````````````````````````