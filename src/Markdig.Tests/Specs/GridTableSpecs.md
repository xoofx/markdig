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
<col style="width:50%" />
<col style="width:50%" />
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
| Col1b             | Col3b   |
| Col1c                       |
.
<table>
<col style="width:33.33%" />
<col style="width:33.33%" />
<col style="width:33.33%" />
<tbody>
<tr>
<td>Col1
Col1a</td>
<td>Col2
Col2a</td>
<td>Col3
Col3a</td>
</tr>
<tr>
<td colspan="2">Col1b</td>
<td>Col3b</td>
</tr>
<tr>
<td colspan="3">Col1c</td>
</tr>
</tbody>
</table>
````````````````````````````````

A row header is separated using `+========+` instead of `+---------+`:

```````````````````````````````` example
+---------+---------+
| This is | a table |
+=========+=========+
.
<table>
<col style="width:50%" />
<col style="width:50%" />
<thead>
<tr>
<th>This is</th>
<th>a table</th>
</tr>
</thead>
</table>
````````````````````````````````

The last column separator `|` may be omitted:

```````````````````````````````` example
+---------+---------+
| This is | a table with a longer text in the second column
.
<table>
<col style="width:50%" />
<col style="width:50%" />
<tbody>
<tr>
<td>This is</td>
<td>a table with a longer text in the second column</td>
</tr>
</tbody>
</table>
````````````````````````````````

The respective width of the columns are calculated from the ratio between the total size of the first table row without counting the `+`: `+----+--------+----+` would be divided between:

Total size is : 16 

- `----` -> 4
- `--------` -> 8
- `----` -> 4

So the width would be 4/16 = 25%, 8/16 = 50%, 4/16 = 25%

```````````````````````````````` example
+----+--------+----+
| A  |  B C D | E  |
+----+--------+----+
.
<table>
<col style="width:25%" />
<col style="width:50%" />
<col style="width:25%" />
<tbody>
<tr>
<td>A</td>
<td>B C D</td>
<td>E</td>
</tr>
</tbody>
</table>
````````````````````````````````

Alignment might be specified on the first row using the character `:`:


```````````````````````````````` example
+-----+:---:+-----+
|  A  |  B  |  C  |
+-----+-----+-----+
.
<table>
<col style="width:33.33%" />
<col style="width:33.33%" />
<col style="width:33.33%" />
<tbody>
<tr>
<td>A</td>
<td style="text-align: center;">B</td>
<td>C</td>
</tr>
</tbody>
</table>
````````````````````````````````

 A grid table may have cells spanning both columns and rows:

```````````````````````````````` example
+---+---+---+
| AAAAA | B |
+---+---+ B +
| D | E | B |
+ D +---+---+
| D | CCCCC |
+---+---+---+
.
<table>
<col style="width:33.33%" />
<col style="width:33.33%" />
<col style="width:33.33%" />
<tbody>
<tr>
<td colspan="2">AAAAA</td>
<td rowspan="2">B
B
B</td>
</tr>
<tr>
<td rowspan="2">D
D
D</td>
<td>E</td>
</tr>
<tr>
<td colspan="2">CCCCC</td>
</tr>
</tbody>
</table>
````````````````````````````````

A grid table may have cells with both colspan and rowspan:

```````````````````````````````` example
+---+---+---+
| AAAAA | B |
+ AAAAA +---+
| AAAAA | C |
+---+---+---+
| D | E | F |
+---+---+---+
.
<table>
<col style="width:33.33%" />
<col style="width:33.33%" />
<col style="width:33.33%" />
<tbody>
<tr>
<td colspan="2" rowspan="2">AAAAA
AAAAA
AAAAA</td>
<td>B</td>
</tr>
<tr>
<td>C</td>
</tr>
<tr>
<td>D</td>
<td>E</td>
<td>F</td>
</tr>
</tbody>
</table>
````````````````````````````````

A grid table may not have irregularly shaped cells:

```````````````````````````````` example
+---+---+---+
| AAAAA | B |
+ A +---+ B +
| A | C | B |
+---+---+---+
| DDDDD | E |
+---+---+---+
.
<p>+---+---+---+
| AAAAA | B |
+ A +---+ B +
| A | C | B |
+---+---+---+
| DDDDD | E |
+---+---+---+</p>
````````````````````````````````

An empty `+` on a line should result in a simple empty list output:


```````````````````````````````` example
+
.
<ul>
<li></li>
</ul>
````````````````````````````````
