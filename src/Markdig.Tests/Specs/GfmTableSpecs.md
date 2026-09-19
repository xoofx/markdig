# GFM table conformance

The eight table examples from the [GFM specification, section 4.10](https://github.github.com/gfm/#tables-extension-),
retrieved from [cmark-gfm/test/spec.txt](https://github.com/github/cmark-gfm/blob/499789b49373bfa045d0e7547e5ee63444c77bca/test/spec.txt)
on 2026-09-19. Markdown inputs are unchanged. Expected HTML uses Markdig's
`style="text-align: ...;"` instead of the specification's `align="..."` attributes.
The upstream specification is licensed under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).

## Basic table

```````````````````````````````` example
| foo | bar |
| --- | --- |
| baz | bim |
.
<table>
<thead>
<tr>
<th>foo</th>
<th>bar</th>
</tr>
</thead>
<tbody>
<tr>
<td>baz</td>
<td>bim</td>
</tr>
</tbody>
</table>
````````````````````````````````

## Alignment and optional outer pipes

```````````````````````````````` example
| abc | defghi |
:-: | -----------:
bar | baz
.
<table>
<thead>
<tr>
<th style="text-align: center;">abc</th>
<th style="text-align: right;">defghi</th>
</tr>
</thead>
<tbody>
<tr>
<td style="text-align: center;">bar</td>
<td style="text-align: right;">baz</td>
</tr>
</tbody>
</table>
````````````````````````````````

## Escaped pipes, including inside code and emphasis

```````````````````````````````` example
| f\|oo  |
| ------ |
| b `\|` az |
| b **\|** im |
.
<table>
<thead>
<tr>
<th>f|oo</th>
</tr>
</thead>
<tbody>
<tr>
<td>b <code>|</code> az</td>
</tr>
<tr>
<td>b <strong>|</strong> im</td>
</tr>
</tbody>
</table>
````````````````````````````````

## Another block ends the table

```````````````````````````````` example
| abc | def |
| --- | --- |
| bar | baz |
> bar
.
<table>
<thead>
<tr>
<th>abc</th>
<th>def</th>
</tr>
</thead>
<tbody>
<tr>
<td>bar</td>
<td>baz</td>
</tr>
</tbody>
</table>
<blockquote>
<p>bar</p>
</blockquote>
````````````````````````````````

## Pipe-less body rows and blank-line termination

```````````````````````````````` example
| abc | def |
| --- | --- |
| bar | baz |
bar

bar
.
<table>
<thead>
<tr>
<th>abc</th>
<th>def</th>
</tr>
</thead>
<tbody>
<tr>
<td>bar</td>
<td>baz</td>
</tr>
<tr>
<td>bar</td>
<td></td>
</tr>
</tbody>
</table>
<p>bar</p>
````````````````````````````````

## Header and delimiter counts must match

```````````````````````````````` example
| abc | def |
| --- |
| bar |
.
<p>| abc | def |
| --- |
| bar |</p>
````````````````````````````````

## Short and excess body cells

```````````````````````````````` example
| abc | def |
| --- | --- |
| bar |
| bar | baz | boo |
.
<table>
<thead>
<tr>
<th>abc</th>
<th>def</th>
</tr>
</thead>
<tbody>
<tr>
<td>bar</td>
<td></td>
</tr>
<tr>
<td>bar</td>
<td>baz</td>
</tr>
</tbody>
</table>
````````````````````````````````

## Header-only table

```````````````````````````````` example
| abc | def |
| --- | --- |
.
<table>
<thead>
<tr>
<th>abc</th>
<th>def</th>
</tr>
</thead>
</table>
````````````````````````````````
