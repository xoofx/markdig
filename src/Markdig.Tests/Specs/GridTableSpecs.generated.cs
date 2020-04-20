// Generated: 2020-04-20 07:21:20

// --------------------------------
//            Grid Tables
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.GridTables
{
    [TestFixture]
    public class TestExtensionsGridTable
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Grid Table
        // 
        // A grid table allows to have multiple lines per cells and allows to span cells over multiple columns. The following shows a simple grid table  
        // 
        // ```
        // +---------+---------+
        // | Header  | Header  |
        // | Column1 | Column2 |
        // +=========+=========+
        // | 1. ab   | > This is a quote
        // | 2. cde  | > For the second column 
        // | 3. f    |
        // +---------+---------+
        // | Second row spanning
        // | on two columns
        // +---------+---------+
        // | Back    |         |
        // | to      |         |
        // | one     |         |
        // | column  |         | 
        // ```
        // 
        // **Rule #1**
        // The first line of a grid table must a **row separator**. It must start with the column separator character `+` used to separate columns in a row separator. Each column separator is:
        //   - starting by optional spaces
        //   - followed by an optional `:` to specify left align, followed by optional spaces
        //   - followed by a sequence of at least one `-` character, followed by optional spaces
        //   - followed by an optional `:` to specify right align (or center align if left align is also defined)
        //   - ending by optional spaces
        // 
        // The first row separator must be followed by a *regular row*. A regular row must start with the character `|` that is starting at the same position than the column separator `+` of the first row separator.
        // 
        // 
        // The following is a valid row separator 
        [Test]
        public void ExtensionsGridTable_Example001()
        {
            // Example 1
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---------+---------+
            //     | This is | a table |
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%" />
            //     <col style="width:50%" />
            //     <tbody>
            //     <tr>
            //     <td>This is</td>
            //     <td>a table</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 1\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---------+---------+\n| This is | a table |", "<table>\n<col style=\"width:50%\" />\n<col style=\"width:50%\" />\n<tbody>\n<tr>\n<td>This is</td>\n<td>a table</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // The following is not a valid row separator 
        [Test]
        public void ExtensionsGridTable_Example002()
        {
            // Example 2
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     |-----xxx----+---------+
            //     | This is    | not a table
            //
            // Should be rendered as:
            //     <p>|-----xxx----+---------+
            //     | This is    | not a table</p>

            Console.WriteLine("Example 2\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("|-----xxx----+---------+\n| This is    | not a table", "<p>|-----xxx----+---------+\n| This is    | not a table</p>", "gridtables|advanced");
        }

        // **Rule #2**
        // A regular row can continue a previous regular row when column separator `|` are positioned at the same  position than the previous line. If they are positioned at the same location, the column may span over multiple columns:
        [Test]
        public void ExtensionsGridTable_Example003()
        {
            // Example 3
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---------+---------+---------+
            //     | Col1    | Col2    | Col3    |
            //     | Col1a   | Col2a   | Col3a   |
            //     | Col1b             | Col3b   |
            //     | Col1c                       |
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <tbody>
            //     <tr>
            //     <td>Col1
            //     Col1a</td>
            //     <td>Col2
            //     Col2a</td>
            //     <td>Col3
            //     Col3a</td>
            //     </tr>
            //     <tr>
            //     <td colspan="2">Col1b</td>
            //     <td>Col3b</td>
            //     </tr>
            //     <tr>
            //     <td colspan="3">Col1c</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 3\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---------+---------+---------+\n| Col1    | Col2    | Col3    |\n| Col1a   | Col2a   | Col3a   |\n| Col1b             | Col3b   |\n| Col1c                       |", "<table>\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<tbody>\n<tr>\n<td>Col1\nCol1a</td>\n<td>Col2\nCol2a</td>\n<td>Col3\nCol3a</td>\n</tr>\n<tr>\n<td colspan=\"2\">Col1b</td>\n<td>Col3b</td>\n</tr>\n<tr>\n<td colspan=\"3\">Col1c</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // A row header is separated using `+========+` instead of `+---------+`:
        [Test]
        public void ExtensionsGridTable_Example004()
        {
            // Example 4
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---------+---------+
            //     | This is | a table |
            //     +=========+=========+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%" />
            //     <col style="width:50%" />
            //     <thead>
            //     <tr>
            //     <th>This is</th>
            //     <th>a table</th>
            //     </tr>
            //     </thead>
            //     </table>

            Console.WriteLine("Example 4\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---------+---------+\n| This is | a table |\n+=========+=========+", "<table>\n<col style=\"width:50%\" />\n<col style=\"width:50%\" />\n<thead>\n<tr>\n<th>This is</th>\n<th>a table</th>\n</tr>\n</thead>\n</table>", "gridtables|advanced");
        }

        // The last column separator `|` may be omitted:
        [Test]
        public void ExtensionsGridTable_Example005()
        {
            // Example 5
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---------+---------+
            //     | This is | a table with a longer text in the second column
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:50%" />
            //     <col style="width:50%" />
            //     <tbody>
            //     <tr>
            //     <td>This is</td>
            //     <td>a table with a longer text in the second column</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 5\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---------+---------+\n| This is | a table with a longer text in the second column", "<table>\n<col style=\"width:50%\" />\n<col style=\"width:50%\" />\n<tbody>\n<tr>\n<td>This is</td>\n<td>a table with a longer text in the second column</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // The respective width of the columns are calculated from the ratio between the total size of the first table row without counting the `+`: `+----+--------+----+` would be divided between:
        // 
        // Total size is : 16 
        // 
        // - `----` -> 4
        // - `--------` -> 8
        // - `----` -> 4
        // 
        // So the width would be 4/16 = 25%, 8/16 = 50%, 4/16 = 25%
        [Test]
        public void ExtensionsGridTable_Example006()
        {
            // Example 6
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +----+--------+----+
            //     | A  |  B C D | E  |
            //     +----+--------+----+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:25%" />
            //     <col style="width:50%" />
            //     <col style="width:25%" />
            //     <tbody>
            //     <tr>
            //     <td>A</td>
            //     <td>B C D</td>
            //     <td>E</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 6\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+----+--------+----+\n| A  |  B C D | E  |\n+----+--------+----+", "<table>\n<col style=\"width:25%\" />\n<col style=\"width:50%\" />\n<col style=\"width:25%\" />\n<tbody>\n<tr>\n<td>A</td>\n<td>B C D</td>\n<td>E</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // Alignment might be specified on the first row using the character `:`:
        [Test]
        public void ExtensionsGridTable_Example007()
        {
            // Example 7
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +-----+:---:+-----+
            //     |  A  |  B  |  C  |
            //     +-----+-----+-----+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <tbody>
            //     <tr>
            //     <td>A</td>
            //     <td style="text-align: center;">B</td>
            //     <td>C</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 7\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+-----+:---:+-----+\n|  A  |  B  |  C  |\n+-----+-----+-----+", "<table>\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<tbody>\n<tr>\n<td>A</td>\n<td style=\"text-align: center;\">B</td>\n<td>C</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        //  A grid table may have cells spanning both columns and rows:
        [Test]
        public void ExtensionsGridTable_Example008()
        {
            // Example 8
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---+---+---+
            //     | AAAAA | B |
            //     +---+---+ B +
            //     | D | E | B |
            //     + D +---+---+
            //     | D | CCCCC |
            //     +---+---+---+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <tbody>
            //     <tr>
            //     <td colspan="2">AAAAA</td>
            //     <td rowspan="2">B
            //     B
            //     B</td>
            //     </tr>
            //     <tr>
            //     <td rowspan="2">D
            //     D
            //     D</td>
            //     <td>E</td>
            //     </tr>
            //     <tr>
            //     <td colspan="2">CCCCC</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 8\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---+---+---+\n| AAAAA | B |\n+---+---+ B +\n| D | E | B |\n+ D +---+---+\n| D | CCCCC |\n+---+---+---+", "<table>\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<tbody>\n<tr>\n<td colspan=\"2\">AAAAA</td>\n<td rowspan=\"2\">B\nB\nB</td>\n</tr>\n<tr>\n<td rowspan=\"2\">D\nD\nD</td>\n<td>E</td>\n</tr>\n<tr>\n<td colspan=\"2\">CCCCC</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // A grid table may have cells with both colspan and rowspan:
        [Test]
        public void ExtensionsGridTable_Example009()
        {
            // Example 9
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---+---+---+
            //     | AAAAA | B |
            //     + AAAAA +---+
            //     | AAAAA | C |
            //     +---+---+---+
            //     | D | E | F |
            //     +---+---+---+
            //
            // Should be rendered as:
            //     <table>
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <col style="width:33.33%" />
            //     <tbody>
            //     <tr>
            //     <td colspan="2" rowspan="2">AAAAA
            //     AAAAA
            //     AAAAA</td>
            //     <td>B</td>
            //     </tr>
            //     <tr>
            //     <td>C</td>
            //     </tr>
            //     <tr>
            //     <td>D</td>
            //     <td>E</td>
            //     <td>F</td>
            //     </tr>
            //     </tbody>
            //     </table>

            Console.WriteLine("Example 9\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---+---+---+\n| AAAAA | B |\n+ AAAAA +---+\n| AAAAA | C |\n+---+---+---+\n| D | E | F |\n+---+---+---+", "<table>\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<col style=\"width:33.33%\" />\n<tbody>\n<tr>\n<td colspan=\"2\" rowspan=\"2\">AAAAA\nAAAAA\nAAAAA</td>\n<td>B</td>\n</tr>\n<tr>\n<td>C</td>\n</tr>\n<tr>\n<td>D</td>\n<td>E</td>\n<td>F</td>\n</tr>\n</tbody>\n</table>", "gridtables|advanced");
        }

        // A grid table may not have irregularly shaped cells:
        [Test]
        public void ExtensionsGridTable_Example010()
        {
            // Example 10
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +---+---+---+
            //     | AAAAA | B |
            //     + A +---+ B +
            //     | A | C | B |
            //     +---+---+---+
            //     | DDDDD | E |
            //     +---+---+---+
            //
            // Should be rendered as:
            //     <p>+---+---+---+
            //     | AAAAA | B |
            //     + A +---+ B +
            //     | A | C | B |
            //     +---+---+---+
            //     | DDDDD | E |
            //     +---+---+---+</p>

            Console.WriteLine("Example 10\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+---+---+---+\n| AAAAA | B |\n+ A +---+ B +\n| A | C | B |\n+---+---+---+\n| DDDDD | E |\n+---+---+---+", "<p>+---+---+---+\n| AAAAA | B |\n+ A +---+ B +\n| A | C | B |\n+---+---+---+\n| DDDDD | E |\n+---+---+---+</p>", "gridtables|advanced");
        }

        // An empty `+` on a line should result in a simple empty list output:
        [Test]
        public void ExtensionsGridTable_Example011()
        {
            // Example 11
            // Section: Extensions / Grid Table
            //
            // The following Markdown:
            //     +
            //
            // Should be rendered as:
            //     <ul>
            //     <li></li>
            //     </ul>

            Console.WriteLine("Example 11\nSection Extensions / Grid Table\n");
            TestParser.TestSpec("+", "<ul>\n<li></li>\n</ul>", "gridtables|advanced");
        }
    }
}
