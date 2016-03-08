# Extensions

This section describes the different extensions supported:

## Grid Table

A grid table allows to have multiple lines per cells and allows to span cells over multiple columns. The following shows a simple grid table  

```
+---------+---------+
| Header  | Header  |
| Column1 | Column2 |
+=========+=========+
| 1. ab   | > This is a quote
| 2. cde  | > For the second column 
| 3. f    |
+---------+---------+
| Second row spanning
| on two columns
+---------+---------+
| Back    |         |
| to      |         |
| one     |         |
| column  |         | 
```

**Rule #1**
The first line of a grid table must a **row separator**. It must start with the column separator character `+` used to separate columns in a row separator. Each column separator is:
  - starting by optional spaces
  - followed by an optional `:` to specify left align, followed by optional spaces
  - followed by a sequence of at least one `-` character, followed by optional spaces
  - followed by an optional `:` to specify right align (or center align if left align is also defined)
  - ending by optional spaces

The first row separator must be followed by a *regular row*. A regular row must start with the character `|` that is starting at the same position than the column separator `+` of the first row separator.


The following is a valid row separator 

```````````````````````````````` example
+---------+---------+
| This is | a table |
.
<table>
<col·style="width:50%">
<col·style="width:50%">
<tbody>
<tr>
<td>This is</td>
<td>a table</td>
</tr>
</tbody>
</table>
````````````````````````````````


The following is not a valid row separator 
```````````````````````````````` example
|-----xxx----+---------+
| This is    | not a table
.
<p>|-----xxx----+---------+
| This is    | not a table</p>
````````````````````````````````

**Rule #2**
A regular row can continue a previous regular row when column separator `|` are positioned at the same  position than the previous line. If they are positioned at the same location, the column may span over multiple columns:

```````````````````````````````` example
+---------+---------+---------+
| Col1    | Col2    | Col3    |
| Col1a   | Col2a   | Col3a   |
| Col12             | Col3b   |
| Col123                      |
.
<table>
<tbody>
<tr>
<td>Col1 Col1a</td>
<td>Col2 Col2a</td>
<td>Col3 Col3a</td>
</tr>
<tr>
<td colspan="2">Col12</td>
<td>Col3b</td>
</tr>
<tr>
<td colspan="3">Col123</td>
</tr>
</tbody>
</table>
````````````````````````````````
