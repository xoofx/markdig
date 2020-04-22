# Extensions

This section describes the different extensions supported:

## Pipe Table

A pipe table is detected when:

**Rule #1**
- Each line of a paragraph block have to contain at least a **column delimiter** `|` that is not embedded by either a code inline (backtick \`) or a HTML inline.
- The second row must separate the first header row from sub-sequent rows by containing a **header column separator** for each column separated by a column delimiter. A header column separator is:
  - starting by optional spaces
  - followed by an optional `:` to specify left align
  - followed by a sequence of at least one `-` character
  - followed by an optional `:` to specify right align (or center align if left align is also defined)
  - ending by optional spaces
 
Because a list has a higher precedence than a pipe table, a table header row separator requires at least 2 dashes `--` to start a header row:

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

The following is also considered as a table, even if the second line starts like a list:

```````````````````````````````` example
a | b
- | -
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

If a line doesn't have a column delimiter `|` the table is not detected

```````````````````````````````` example
a | b
c no d
.
<p>a | b
c no d</p>
````````````````````````````````

If a row contains more column than the header row, it will still be added as a column:

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
<th></th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
<td>2</td>
</tr>
<tr>
<td>3</td>
<td>4</td>
<td></td>
</tr>
<tr>
<td>5</td>
<td></td>
<td></td>
</tr>
</tbody>
</table>
````````````````````````````````

**Rule #2**
A pipe table ends after a blank line or the end of the file.

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

A backtick/code delimiter has a higher precedence than a column delimiter `|`:
 
```````````````````````````````` example
a | b `
0 | ` 
.
<p>a | b <code>0 |</code></p> 
````````````````````````````````

**Rule #8**

A HTML inline has a higher precedence than a column delimiter `|`: 
 
```````````````````````````````` example
a <a href="" title="|"></a> | b
-- | --
0  | 1
.
<table>
<thead>
<tr>
<th>a <a href="" title="|"></a></th>
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

**Rule #9**

Links have a higher precedence than the column delimiter character `|`:

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
<td><a href="http://google.com">This is a link with a | inside the label</a></td>
<td>1</td>
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

The tables are normalized to the maximum number of columns found in a table


```````````````````````````````` example
a | b
-- | - 
0 | 1 | 2
.
<table>
<thead>
<tr>
<th>a</th>
<th>b</th>
<th></th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td>1</td>
<td>2</td>
</tr>
</tbody>
</table>
````````````````````````````````
