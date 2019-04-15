# Extensions

Adds support for outputting bootstrap ready tags:

## Bootstrap
 
Adds bootstrap `.table` class to `<table>`:

```````````````````````````````` example
Name | Value
-----| -----
Abc  | 16
.
<table class="table">
<thead>
<tr>
<th>Name</th>
<th>Value</th>
</tr>
</thead>
<tbody>
<tr>
<td>Abc</td>
<td>16</td>
</tr>
</tbody>
</table>
````````````````````````````````

Adds bootstrap `.blockquote` class to `<blockquote>`:

```````````````````````````````` example
> This is a blockquote
.
<blockquote class="blockquote">
<p>This is a blockquote</p>
</blockquote>
````````````````````````````````

Adds bootstrap `.figure` class to `<figure>` and `.figure-caption` to `<figcaption>`

```````````````````````````````` example
^^^
This is a text in a caption
^^^ This is the caption
.
<figure class="figure">
<p>This is a text in a caption</p>
<figcaption class="figure-caption">This is the caption</figcaption>
</figure>
````````````````````````````````

Adds the `.img-fluid` class to all image links `<img>`

```````````````````````````````` example
![Image Link](/url)
.
<p><img src="/url" class="img-fluid" alt="Image Link" /></p>
````````````````````````````````