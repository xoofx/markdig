# Extensions

## NoHTML

The extension DisableHtml allows to disable the parsing of HTML:

For inline HTML:

```````````````````````````````` example
this is some text</td></tr>
.
<p>this is some text&lt;/td&gt;&lt;/tr&gt;</p>
````````````````````````````````

For Block HTML:

```````````````````````````````` example
<div>
this is some text
</div>
.
<p>&lt;div&gt;
this is some text
&lt;/div&gt;</p>
````````````````````````````````


