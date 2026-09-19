# Extensions

This section describes the different extensions supported:

## Gfm Pipe Table

These tests exercise `Configure("gfm-pipetables")`, equivalent to
`UsePipeTables(new PipeTableOptions { UseGfmRules = true })`.
The official specification examples are in `GfmTableSpecs.md`.

A pipe table is detected when:

**Rule #1**
- Cells are split on unescaped pipes before parsing any inline content. Body rows may omit pipes.
- The second row must separate the first header row from sub-sequent rows by containing a **header column separator** for each column separated by a column delimiter. A header column separator is:
  - starting by optional spaces
  - followed by an optional `:` to specify left align
  - followed by a sequence of at least one `-` character
  - followed by an optional `:` to specify right align (or center align if left align is also defined)
  - ending by optional spaces
- Header and separator rows must contain the same number of cells.
 
A table header row separator may start with two dashes:

```````````````````````````````` example
a | b
-- | -
0 | 1
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
````````````````````````````````

List markers take precedence over tables in cmark-gfm. Use a leading pipe or
at least two dashes to disambiguate this delimiter row:

```````````````````````````````` example
a | b
- | -
0 | 1
.
<p>a | b</p>
<ul>
<li>| -
0 | 1</li>
</ul>
````````````````````````````````

A pipe table with only one header row is allowed:

```````````````````````````````` example
a | b
-- | --
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
</table>
````````````````````````````````

After a row separator header, they will be interpreted as plain column:

```````````````````````````````` example
a | b
-- | --
-- | --
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>--</td>
<td>--</td>
</tr>
</tbody>
</table>
````````````````````````````````

But if a table doesn't start with a column delimiter, it is not interpreted as a table, even if following lines have a column delimiter

```````````````````````````````` example
a b
c | d
e | f
.
<p>a b
c | d
e | f</p>
````````````````````````````````

Without a delimiter row, the table is not detected:

```````````````````````````````` example
a | b
c no d
.
<p>a | b
c no d</p>
````````````````````````````````

If a row contains more columns than the header row, the extra columns will be ignored:

```````````````````````````````` example
a  | b 
-- | --
0  | 1 | 2
3  | 4
5  |
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
<tr>
<td>3</td>
<td>4</td>
</tr>
<tr>
<td>5</td>
<td></td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #2**
A pipe table ends at a blank line, another block-level structure, or the end of the file.

**Rule #3**
A cell content is trimmed (start and end) from white-spaces.

```````````````````````````````` example
a          | b              |
-- | --
0      | 1       |
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #4**
Column delimiters `|` at the very beginning of a line or just before a line ending with only spaces and/or terminated by a newline can be omitted

```````````````````````````````` example
  a     | b     |
--      | --
| 0     | 1
| 2     | 3     |
  4     | 5 
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
<tr>
<td>2</td>
<td>3</td>
</tr>
<tr>
<td>4</td>
<td>5</td>
</tr>
</tbody>
</table>
````````````````````````````````

A pipe may be present at both the beginning/ending of each line:

```````````````````````````````` example
|a|b|
|-|-|
|0|1|
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
````````````````````````````````

Or may be omitted on one side:

```````````````````````````````` example
a|b|
-|-|
0|1|
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
````````````````````````````````

```````````````````````````````` example
|a|b
|-|-
|0|1
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
</table>
````````````````````````````````



Single column table can be declared with lines starting only by a column delimiter: 

```````````````````````````````` example
| a
| --
| b
| c 
.
<table>
<thead>
<tr>
<th>a</th>
</tr>
</thead>
<tbody>
<tr>
<td>b</td>
</tr>
<tr>
<td>c</td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #5**

The first row is considered as a **header row** if it is separated from the regular rows by a row containing a **header column separator** for each column. A header column separator is:

- starting by optional spaces
- followed by an optional `:` to specify left align
- followed by a sequence of at least one `-` character
- followed by an optional `:` to specify right align (or center align if left align is also defined)
- ending by optional spaces
 
```````````````````````````````` example
 a     | b 
-------|-------
 0     | 1 
 2     | 3 
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
<tr>
<td>2</td>
<td>3</td>
</tr>
</tbody>
</table>
````````````````````````````````

The text alignment is defined by default to be center for header and left for cells. If the left alignment is applied, it will force the column heading to be left aligned.
There is no way to define a different alignment for heading and cells (apart from the default).
The text alignment can be changed by using the character `:` with the header column separator:
 
```````````````````````````````` example
 a     | b       | c 
:------|:-------:| ----:
 0     | 1       | 2 
 3     | 4       | 5 
.
<table>
<thead>
<tr>
<th style="text-align: left;">a</th>
<th style="text-align: center;">b</th>
<th style="text-align: right;">c</th>
</tr>
</thead>
<tbody>
<tr>
<td style="text-align: left;">0</td>
<td style="text-align: center;">1</td>
<td style="text-align: right;">2</td>
</tr>
<tr>
<td style="text-align: left;">3</td>
<td style="text-align: center;">4</td>
<td style="text-align: right;">5</td>
</tr>
</tbody>
</table>
````````````````````````````````

Test alignment with starting and ending pipes:

```````````````````````````````` example
| abc | def | ghi |
|:---:|-----|----:|
|  1  | 2   | 3   |
.
<table>
<thead>
<tr>
<th style="text-align: center;">abc</th>
<th>def</th>
<th style="text-align: right;">ghi</th>
</tr>
</thead>
<tbody>
<tr>
<td style="text-align: center;">1</td>
<td>2</td>
<td style="text-align: right;">3</td>
</tr>
</tbody>
</table>
````````````````````````````````

The following example shows a non matching header column separator:
 
```````````````````````````````` example
 a     | b
-------|---x---
 0     | 1
 2     | 3 
.
<p>a     | b
-------|---x---
0     | 1
2     | 3</p> 
````````````````````````````````

**Rule #6**

A column delimiter has a higher priority than emphasis delimiter
 
```````````````````````````````` example
 *a*   | b
-----  |-----
 0     | _1_
 _2    | 3* 
.
<table>
<thead>
<tr>
<th><em>a</em></th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td><em>1</em></td>
</tr>
<tr>
<td>_2</td>
<td>3*</td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #7**

Without a delimiter row this remains an ordinary paragraph with a multiline code span:
 
```````````````````````````````` example
a | b `
0 | ` 
.
<p>a | b <code>0 |</code></p> 
````````````````````````````````

**Rule #8**

An unescaped pipe inside HTML still counts as a column delimiter. Here the
header and delimiter counts do not match, so no table is recognized:
 
```````````````````````````````` example
a <a href="" title="|"></a> | b
-- | --
0  | 1
.
<p>a <a href="" title="|"></a> | b
-- | --
0  | 1</p>
````````````````````````````````

**Rule #9**

Unescaped pipes split cells even inside link labels:

```````````````````````````````` example
a  | b
-- | --
[This is a link with a | inside the label](http://google.com) | 1
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>[This is a link with a</td>
<td>inside the label](http://google.com)</td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #10**

It is possible to have a single row header only:

```````````````````````````````` example
a  | b
-- | --
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
</table>
````````````````````````````````

```````````````````````````````` example
|a|b|c
|---|---|---|
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
<th>c</th>
</tr>
</thead>
</table>
````````````````````````````````

**Tests**

Tests trailing spaces after pipes

```````````````````````````````` example
| abc | def | 
|---|---|
| cde| ddd| 
| eee| fff|
| fff | fffff   | 
|gggg  | ffff | 
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
<td>cde</td>
<td>ddd</td>
</tr>
<tr>
<td>eee</td>
<td>fff</td>
</tr>
<tr>
<td>fff</td>
<td>fffff</td>
</tr>
<tr>
<td>gggg</td>
<td>ffff</td>
</tr>
</tbody>
</table>
````````````````````````````````

**Normalized columns count**

The tables are normalized to the number of columns found in the table header.
Extra columns will be ignored, missing columns will be inserted.


```````````````````````````````` example
a | b
-- | - 
0 | 1 | 2
3 |
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
</tr>
<tr>
<td>3</td>
<td></td>
</tr>
</tbody>
</table>
````````````````````````````````
