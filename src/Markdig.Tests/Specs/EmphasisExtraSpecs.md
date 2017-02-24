# Extensions

The following additional emphasis are supported:

## Strikethrough
 
Allows to strikethrough a span of text by surrounding it by `~~`. The semantic used for the generated HTML is the tag `<del>`.

```````````````````````````````` example
The following text ~~is deleted~~
.
<p>The following text <del>is deleted</del></p>
````````````````````````````````

## Superscript and Subscript
 
Superscripts can be written by surrounding a text by ^ characters; subscripts can be written by surrounding the subscripted text by ~ characters

```````````````````````````````` example
H~2~O is a liquid. 2^10^ is 1024
.
<p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>
````````````````````````````````
 
Certain punctuation characters are exempted from the rule forbidding them within inline delimiters

```````````````````````````````` example
One quintillionth can be expressed as 10^-18^

Daggers^†^ and double-daggers^‡^ can be used to denote notes.
.
<p>One quintillionth can be expressed as 10<sup>-18</sup></p>
<p>Daggers<sup>†</sup> and double-daggers<sup>‡</sup> can be used to denote notes.</p>
````````````````````````````````

## Inserted

Inserted text can be used to specify that a text has been added to a document.  The semantic used for the generated HTML is the tag `<ins>`.
 
```````````````````````````````` example
++Inserted text++
.
<p><ins>Inserted text</ins></p>
````````````````````````````````

## Marked

Marked text can be used to specify that a text has been marked in a document.  The semantic used for the generated HTML is the tag `<mark>`.
 
```````````````````````````````` example
==Marked text==
.
<p><mark>Marked text</mark></p>
````````````````````````````````
## Emphasis on Html Entities


```````````````````````````````` example
This is text MyBrand ^&reg;^ and MyTrademark ^&trade;^
This is text MyBrand^&reg;^ and MyTrademark^&trade;^
This is text MyBrand~&reg;~ and MyCopyright^&copy;^
.
<p>This is text MyBrand <sup>®</sup> and MyTrademark <sup>TM</sup>
This is text MyBrand<sup>®</sup> and MyTrademark<sup>TM</sup>
This is text MyBrand<sub>®</sub> and MyCopyright<sup>©</sup></p>
````````````````````````````````

