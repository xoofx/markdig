# Extensions

The following additional emphasis are supported:

## Strikeout
 
Allows to strikeout a span of text by surrounding it by `~~`

```````````````````````````````` example
The following text ~~is deleted~~
.
<p>The following text <del>is deleted</del></p>
````````````````````````````````

## Superscript and Subscript
 
Superscripts can be written by surrounding a text by ^ characters; subscripts can be written by surrounding the subscripted text by ~ characters:

```````````````````````````````` example
H~2~O is a liquid. 2^10^ is 1024
.
<p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>
````````````````````````````````

## Inserted

Inserted text can be used to specify that a text has been added to a document.
 
```````````````````````````````` example
++Inserted text++
.
<p><ins>Inserted text</ins></p>
````````````````````````````````

## Marked

Marked text can be used to specify that a text has been marked in a document.
 
```````````````````````````````` example
==Marked text==
.
<p><mark>Marked text</mark></p>
````````````````````````````````
