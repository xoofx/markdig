# Extensions

This section describes the different extensions supported:

## AutoLinks

Autolinks will format as a HTML link any string that starts by:

- `http://` or `https://` 
- `ftp://`
- `mailto:`
- `www.` 
 
```````````````````````````````` example
This is a http://www.google.com URL and https://www.google.com
This is a ftp://test.com
And a mailto:email@toto.com
And a plain www.google.com
.
<p>This is a <a href="http://www.google.com">http://www.google.com</a> URL and <a href="https://www.google.com">https://www.google.com</a>
This is a <a href="ftp://test.com">ftp://test.com</a>
And a <a href="mailto:email@toto.com">mailto:email@toto.com</a>
And a plain <a href="http://www.google.com">www.google.com</a></p>
````````````````````````````````

But incomplete links will not be matched:
 
```````````````````````````````` example
This is not a http:/www.google.com URL and https:/www.google.com
This is not a ftp:/test.com
And not a mailto:emailtoto.com
And not a plain www. or a www.x 
.
<p>This is not a http:/www.google.com URL and https:/www.google.com
This is not a ftp:/test.com
And not a mailto:emailtoto.com
And not a plain www. or a www.x</p>
````````````````````````````````

Previous character must be a punctuation or a valid space (tab, space, new line):
 
```````````````````````````````` example
This is not a nhttp://www.google.com URL but this is (https://www.google.com)
.
<p>This is not a nhttp://www.google.com URL but this is (<a href="https://www.google.com">https://www.google.com</a>)</p>
````````````````````````````````

An autolink should not interfere with an `<a>` HTML inline:
 
```````````````````````````````` example
This is an HTML <a href="http://www.google.com">http://www.google.com</a> link
.
<p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>
````````````````````````````````
or even within emphasis:
 
```````````````````````````````` example
This is an HTML <a href="http://www.google.com"> **http://www.google.com** </a> link
.
<p>This is an HTML <a href="http://www.google.com"> <strong>http://www.google.com</strong> </a> link</p>
````````````````````````````````


An autolink should not interfere with a markdown link:
 
```````````````````````````````` example
This is an HTML [http://www.google.com](http://www.google.com) link
.
<p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>
````````````````````````````````

A link embraced by pending emphasis should let the emphasis takes precedence if characters are placed at the end of the matched link:
 
```````````````````````````````` example
Check **http://www.a.com** or __http://www.b.com__
.
<p>Check <strong><a href="http://www.a.com">http://www.a.com</a></strong> or <strong><a href="http://www.b.com">http://www.b.com</a></strong></p>
````````````````````````````````
