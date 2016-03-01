# Extensions

This section describes the different extensions supported:

## Pipe Table

A pipe table is detected when:

- The first line of a paragraph block contains at least a **column delimiter** `|`
- All sub-sequent lines must contain at a column delimiter `|`

```````````````````````````````` example
a | b
c | d
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
<tr>
<td>c</td>
<td>d</td>
</tr>
</tbody>
````````````````````````````````

The following example shows that the second line doesn't have a column delimiter `|` so the table is not detected:

```````````````````````````````` example
a | b
c no d
.
<p>a | b
c no d</p>
````````````````````````````````

The number of columns in the first row determine the number of columns for the whole table. Any extra columns delimiter `|` are converted to literal strings instead

```````````````````````````````` example
a | b 
0 | 1 | 2
3 | 4
5 |
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
<tr>
<td>0</td>
<td>1 | 2</td>
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
````````````````````````````````

 
A pipe table ends after a blank line or the end of the file.
A cell content is trimmed (start and end) from white-spaces.

The **header row** is separated from the regular rows by a row containing a **header column separator** for each column. A header column separator is:

- starting by optional spaces
- followed by an optional `:` to specify left align
- followed by a sequence of at least one `-` character
- followed by an optional `:` to specify right align
- ending by optional spaces

The text alignment is defined by default to be left.

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
````````````````````````````````

The pipe `|` may be present at the beginning or at the end of a line:  

```````````````````````````````` example
  a     | b     |
|-------|-------
| 0     | 1     |
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
````````````````````````````````

The pipe `|` may be present at the beginning or at the end of a line:  

```````````````````````````````` example
  a     | b     |
|-------|-------
| 0     | 1     |
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
````````````````````````````````

The text alignment can be changed by using the character `:` with the header column separator:
 
```````````````````````````````` example
 a     | b
-------|-------:
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
````````````````````````````````
