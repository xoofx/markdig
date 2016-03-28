# Extensions

Adds support for smarty pants:

## SmartyPants
 
Converts the following character to smarty pants:

```````````````````````````````` example
This is a "text"
.
<p>This is a &ldquo;text&rdquo;</p>
````````````````````````````````

```````````````````````````````` example
This is a 'text'
.
<p>This is a &lsquo;text&rsquo;</p>
````````````````````````````````

```````````````````````````````` example
This is a <<text>>
.
<p>This is a &laquo;text&raquo;</p>
````````````````````````````````

```````````````````````````````` example
This is a -- text
.
<p>This is a &ndash; text</p>
````````````````````````````````

```````````````````````````````` example
This is a --- text
.
<p>This is a &mdash; text</p>
````````````````````````````````

```````````````````````````````` example
This is a en ellipsis...
.
<p>This is a en ellipsis&hellip;</p>
````````````````````````````````
