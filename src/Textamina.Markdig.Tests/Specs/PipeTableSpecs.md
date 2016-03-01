# Extensions

This section describes the different extensions supported:

## Pipe Table

A pipe table is detected when:

**Rule #1**
Each line of a paragraph block have to contain at least a **column delimiter** `|` that is not embedded by either a code inline (backstick \`) or a HTML inline.

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

A pipe table with one row is also possible:

```````````````````````````````` example
a | b
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
</tbody>
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

The number of columns in the first row determine the number of columns for the whole table. Any extra columns delimiter `|` for sub-sequent lines are converted to literal strings instead:

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
</tr>
</tbody>
````````````````````````````````

**Rule #2**
A pipe table ends after a blank line or the end of the file.

**Rule #3**
A cell content is trimmed (start and end) from white-spaces.

```````````````````````````````` example
a          | b              | 
0      | 1       |
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
<tr>
<td>0</td>
<td>1</td>
</tr>
</tbody>
````````````````````````````````

**Rule #4**
Column delimiters `|` at the very beginning of a line or just before a line ending with only spaces and/or terminated by a newline can be omitted

```````````````````````````````` example
  a     | b     |
| 0     | 1
| 2     | 3     |
  4     | 5 
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
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
````````````````````````````````
Single column table can be declared with lines starting only by a column delimiter: 

```````````````````````````````` example
| a
| b
| c 
.
<table>
<tbody>
<tr>
<td>a</td>
</tr>
<tr>
<td>b</td>
</tr>
<tr>
<td>c</td>
</tr>
</tbody>
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
````````````````````````````````

The text alignment is defined by default to be left.
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
<th>a</th>
<th style="text-align: center;">b</th>
<th style="text-align: right;">c</th>
</tr>
</thead>
<tbody>
<tr>
<td>0</td>
<td style="text-align: center;">1</td>
<td style="text-align: right;">2</td>
</tr>
<tr>
<td>3</td>
<td style="text-align: center;">4</td>
<td style="text-align: right;">5</td>
</tr>
</tbody>
````````````````````````````````

The following example shows a non matching header column separator:
 
```````````````````````````````` example
 a     | b
-------|---x---
 0     | 1
 2     | 3 
.
<table>
<tbody>
<tr>
<td>a</td>
<td>b</td>
</tr>
<tr>
<td>-------</td>
<td>---x---</td>
</tr>
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

**Rule #6**

A column delimiter has a higher priority than emphasis delimiter
 
```````````````````````````````` example
 *a*   | b
 0     | _1_
 _2     | 3* 
.
<table>
<tbody>
<tr>
<td><em>a</em></td>
<td>b</td>
</tr>
<tr>
<td>0</td>
<td><em>1</em></td>
</tr>
<tr>
<td>*2</td>
<td>3*</td>
</tr>
</tbody>
````````````````````````````````
