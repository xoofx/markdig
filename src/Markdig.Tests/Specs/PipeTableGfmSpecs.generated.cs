
// --------------------------------
//          GFM Pipe Tables
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.GFMPipeTables
{
    [TestFixture]
    public class TestExtensionsGfmPipeTable
    {
        // # Extensions
        // 
        // This section describes the different extensions supported:
        // 
        // ## Gfm Pipe Table
        // 
        // This groups a certain set of behaviors that makes pipe tables adhere more strictly to the GitHub-flavored Markdown specification.
        // 
        // A pipe table is detected when:
        // 
        // **Rule #1**
        // - Each line of a paragraph block have to contain at least a **column delimiter** `|` that is not embedded by either a code inline (backtick \`) or a HTML inline.
        // - The second row must separate the first header row from sub-sequent rows by containing a **header column separator** for each column separated by a column delimiter. A header column separator is:
        //   - starting by optional spaces
        //   - followed by an optional `:` to specify left align
        //   - followed by a sequence of at least one `-` character
        //   - followed by an optional `:` to specify right align (or center align if left align is also defined)
        //   - ending by optional spaces
        //  
        // Because a list has a higher precedence than a pipe table, a table header row separator requires at least 2 dashes `--` to start a header row:
        [Test]
        public void ExtensionsGfmPipeTable_Example001()
        {
            // Example 1
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     -- | -
            //     0 | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a | b\n-- | -\n0 | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 1\nSection Extensions / Gfm Pipe Table\n");
        }

        // The following is also considered as a table, even if the second line starts like a list:
        [Test]
        public void ExtensionsGfmPipeTable_Example002()
        {
            // Example 2
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     - | -
            //     0 | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a | b\n- | -\n0 | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 2\nSection Extensions / Gfm Pipe Table\n");
        }

        // A pipe table with only one header row is allowed:
        [Test]
        public void ExtensionsGfmPipeTable_Example003()
        {
            // Example 3
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     -- | --
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     </table>

            TestParser.TestSpec("a | b\n-- | --", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n</table>", "gfm-pipetables", context: "Example 3\nSection Extensions / Gfm Pipe Table\n");
        }

        // After a row separator header, they will be interpreted as plain column:
        [Test]
        public void ExtensionsGfmPipeTable_Example004()
        {
            // Example 4
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     -- | --
            //     -- | --
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>--</td>
            //     <td>--</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a | b\n-- | --\n-- | --", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>--</td>\n<td>--</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 4\nSection Extensions / Gfm Pipe Table\n");
        }

        // But if a table doesn't start with a column delimiter, it is not interpreted as a table, even if following lines have a column delimiter
        [Test]
        public void ExtensionsGfmPipeTable_Example005()
        {
            // Example 5
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a b
            //     c | d
            //     e | f
            //
            // Should be rendered as:
            //     <p>a b
            //     c | d
            //     e | f</p>

            TestParser.TestSpec("a b\nc | d\ne | f", "<p>a b\nc | d\ne | f</p>", "gfm-pipetables", context: "Example 5\nSection Extensions / Gfm Pipe Table\n");
        }

        // If a line doesn't have a column delimiter `|` the table is not detected
        [Test]
        public void ExtensionsGfmPipeTable_Example006()
        {
            // Example 6
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     c no d
            //
            // Should be rendered as:
            //     <p>a | b
            //     c no d</p>

            TestParser.TestSpec("a | b\nc no d", "<p>a | b\nc no d</p>", "gfm-pipetables", context: "Example 6\nSection Extensions / Gfm Pipe Table\n");
        }

        // If a row contains more columns than the header row, the extra columns will be ignored:
        [Test]
        public void ExtensionsGfmPipeTable_Example007()
        {
            // Example 7
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a  | b 
            //     -- | --
            //     0  | 1 | 2
            //     3  | 4
            //     5  |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>3</td>
            //     <td>4</td>
            //     </tr>
            //     <tr>
            //     <td>5</td>
            //     <td></td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a  | b \n-- | --\n0  | 1 | 2\n3  | 4\n5  |", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>3</td>\n<td>4</td>\n</tr>\n<tr>\n<td>5</td>\n<td></td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 7\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #2**
        // A pipe table ends after a blank line or the end of the file.
        // 
        // **Rule #3**
        // A cell content is trimmed (start and end) from white-spaces.
        [Test]
        public void ExtensionsGfmPipeTable_Example008()
        {
            // Example 8
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a          | b              |
            //     -- | --
            //     0      | 1       |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a          | b              |\n-- | --\n0      | 1       |", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 8\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #4**
        // Column delimiters `|` at the very beginning of a line or just before a line ending with only spaces and/or terminated by a newline can be omitted
        [Test]
        public void ExtensionsGfmPipeTable_Example009()
        {
            // Example 9
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //       a     | b     |
            //     --      | --
            //     | 0     | 1
            //     | 2     | 3     |
            //       4     | 5 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>2</td>
            //     <td>3</td>
            //     </tr>
            //     <tr>
            //     <td>4</td>
            //     <td>5</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("  a     | b     |\n--      | --\n| 0     | 1\n| 2     | 3     |\n  4     | 5 ", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>2</td>\n<td>3</td>\n</tr>\n<tr>\n<td>4</td>\n<td>5</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 9\nSection Extensions / Gfm Pipe Table\n");
        }

        // A pipe may be present at both the beginning/ending of each line:
        [Test]
        public void ExtensionsGfmPipeTable_Example010()
        {
            // Example 10
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     |a|b|
            //     |-|-|
            //     |0|1|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("|a|b|\n|-|-|\n|0|1|", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 10\nSection Extensions / Gfm Pipe Table\n");
        }

        // Or may be omitted on one side:
        [Test]
        public void ExtensionsGfmPipeTable_Example011()
        {
            // Example 11
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a|b|
            //     -|-|
            //     0|1|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a|b|\n-|-|\n0|1|", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 11\nSection Extensions / Gfm Pipe Table\n");
        }

        [Test]
        public void ExtensionsGfmPipeTable_Example012()
        {
            // Example 12
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     |a|b
            //     |-|-
            //     |0|1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("|a|b\n|-|-\n|0|1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 12\nSection Extensions / Gfm Pipe Table\n");
        }

        // Single column table can be declared with lines starting only by a column delimiter: 
        [Test]
        public void ExtensionsGfmPipeTable_Example013()
        {
            // Example 13
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     | a
            //     | --
            //     | b
            //     | c 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>b</td>
            //     </tr>
            //     <tr>
            //     <td>c</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| a\n| --\n| b\n| c ", "<table>\n<thead>\n<tr>\n<th>a</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>b</td>\n</tr>\n<tr>\n<td>c</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 13\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #5**
        // 
        // The first row is considered as a **header row** if it is separated from the regular rows by a row containing a **header column separator** for each column. A header column separator is:
        // 
        // - starting by optional spaces
        // - followed by an optional `:` to specify left align
        // - followed by a sequence of at least one `-` character
        // - followed by an optional `:` to specify right align (or center align if left align is also defined)
        // - ending by optional spaces
        [Test]
        public void ExtensionsGfmPipeTable_Example014()
        {
            // Example 14
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //      a     | b 
            //     -------|-------
            //      0     | 1 
            //      2     | 3 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>2</td>
            //     <td>3</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec(" a     | b \n-------|-------\n 0     | 1 \n 2     | 3 ", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>2</td>\n<td>3</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 14\nSection Extensions / Gfm Pipe Table\n");
        }

        // The text alignment is defined by default to be center for header and left for cells. If the left alignment is applied, it will force the column heading to be left aligned.
        // There is no way to define a different alignment for heading and cells (apart from the default).
        // The text alignment can be changed by using the character `:` with the header column separator:
        [Test]
        public void ExtensionsGfmPipeTable_Example015()
        {
            // Example 15
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //      a     | b       | c 
            //     :------|:-------:| ----:
            //      0     | 1       | 2 
            //      3     | 4       | 5 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th style="text-align: left;">a</th>
            //     <th style="text-align: center;">b</th>
            //     <th style="text-align: right;">c</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td style="text-align: left;">0</td>
            //     <td style="text-align: center;">1</td>
            //     <td style="text-align: right;">2</td>
            //     </tr>
            //     <tr>
            //     <td style="text-align: left;">3</td>
            //     <td style="text-align: center;">4</td>
            //     <td style="text-align: right;">5</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec(" a     | b       | c \n:------|:-------:| ----:\n 0     | 1       | 2 \n 3     | 4       | 5 ", "<table>\n<thead>\n<tr>\n<th style=\"text-align: left;\">a</th>\n<th style=\"text-align: center;\">b</th>\n<th style=\"text-align: right;\">c</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td style=\"text-align: left;\">0</td>\n<td style=\"text-align: center;\">1</td>\n<td style=\"text-align: right;\">2</td>\n</tr>\n<tr>\n<td style=\"text-align: left;\">3</td>\n<td style=\"text-align: center;\">4</td>\n<td style=\"text-align: right;\">5</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 15\nSection Extensions / Gfm Pipe Table\n");
        }

        // Test alignment with starting and ending pipes:
        [Test]
        public void ExtensionsGfmPipeTable_Example016()
        {
            // Example 16
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     | abc | def | ghi |
            //     |:---:|-----|----:|
            //     |  1  | 2   | 3   |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th style="text-align: center;">abc</th>
            //     <th>def</th>
            //     <th style="text-align: right;">ghi</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td style="text-align: center;">1</td>
            //     <td>2</td>
            //     <td style="text-align: right;">3</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| abc | def | ghi |\n|:---:|-----|----:|\n|  1  | 2   | 3   |", "<table>\n<thead>\n<tr>\n<th style=\"text-align: center;\">abc</th>\n<th>def</th>\n<th style=\"text-align: right;\">ghi</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td style=\"text-align: center;\">1</td>\n<td>2</td>\n<td style=\"text-align: right;\">3</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 16\nSection Extensions / Gfm Pipe Table\n");
        }

        // The following example shows a non matching header column separator:
        [Test]
        public void ExtensionsGfmPipeTable_Example017()
        {
            // Example 17
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //      a     | b
            //     -------|---x---
            //      0     | 1
            //      2     | 3 
            //
            // Should be rendered as:
            //     <p>a     | b
            //     -------|---x---
            //     0     | 1
            //     2     | 3</p> 

            TestParser.TestSpec(" a     | b\n-------|---x---\n 0     | 1\n 2     | 3 ", "<p>a     | b\n-------|---x---\n0     | 1\n2     | 3</p> ", "gfm-pipetables", context: "Example 17\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #6**
        // 
        // A column delimiter has a higher priority than emphasis delimiter
        [Test]
        public void ExtensionsGfmPipeTable_Example018()
        {
            // Example 18
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //      *a*   | b
            //     -----  |-----
            //      0     | _1_
            //      _2    | 3* 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th><em>a</em></th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td><em>1</em></td>
            //     </tr>
            //     <tr>
            //     <td>_2</td>
            //     <td>3*</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec(" *a*   | b\n-----  |-----\n 0     | _1_\n _2    | 3* ", "<table>\n<thead>\n<tr>\n<th><em>a</em></th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td><em>1</em></td>\n</tr>\n<tr>\n<td>_2</td>\n<td>3*</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 18\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #7**
        // 
        // A backtick/code delimiter has a higher precedence than a column delimiter `|`:
        [Test]
        public void ExtensionsGfmPipeTable_Example019()
        {
            // Example 19
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b `
            //     0 | ` 
            //
            // Should be rendered as:
            //     <p>a | b <code>0 |</code></p> 

            TestParser.TestSpec("a | b `\n0 | ` ", "<p>a | b <code>0 |</code></p> ", "gfm-pipetables", context: "Example 19\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #8**
        // 
        // A HTML inline has a higher precedence than a column delimiter `|`: 
        [Test]
        public void ExtensionsGfmPipeTable_Example020()
        {
            // Example 20
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a <a href="" title="|"></a> | b
            //     -- | --
            //     0  | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a <a href="" title="|"></a></th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a <a href=\"\" title=\"|\"></a> | b\n-- | --\n0  | 1", "<table>\n<thead>\n<tr>\n<th>a <a href=\"\" title=\"|\"></a></th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 20\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #9**
        // 
        // Links have a higher precedence than the column delimiter character `|`:
        [Test]
        public void ExtensionsGfmPipeTable_Example021()
        {
            // Example 21
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a  | b
            //     -- | --
            //     [This is a link with a | inside the label](http://google.com) | 1
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td><a href="http://google.com">This is a link with a | inside the label</a></td>
            //     <td>1</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a  | b\n-- | --\n[This is a link with a | inside the label](http://google.com) | 1", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td><a href=\"http://google.com\">This is a link with a | inside the label</a></td>\n<td>1</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 21\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Rule #10**
        // 
        // It is possible to have a single row header only:
        [Test]
        public void ExtensionsGfmPipeTable_Example022()
        {
            // Example 22
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a  | b
            //     -- | --
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     </table>

            TestParser.TestSpec("a  | b\n-- | --", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n</table>", "gfm-pipetables", context: "Example 22\nSection Extensions / Gfm Pipe Table\n");
        }

        [Test]
        public void ExtensionsGfmPipeTable_Example023()
        {
            // Example 23
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     |a|b|c
            //     |---|---|---|
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     <th>c</th>
            //     </tr>
            //     </thead>
            //     </table>

            TestParser.TestSpec("|a|b|c\n|---|---|---|", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n<th>c</th>\n</tr>\n</thead>\n</table>", "gfm-pipetables", context: "Example 23\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Tests**
        // 
        // Tests trailing spaces after pipes
        [Test]
        public void ExtensionsGfmPipeTable_Example024()
        {
            // Example 24
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     | abc | def | 
            //     |---|---|
            //     | cde| ddd| 
            //     | eee| fff|
            //     | fff | fffff   | 
            //     |gggg  | ffff | 
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>abc</th>
            //     <th>def</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>cde</td>
            //     <td>ddd</td>
            //     </tr>
            //     <tr>
            //     <td>eee</td>
            //     <td>fff</td>
            //     </tr>
            //     <tr>
            //     <td>fff</td>
            //     <td>fffff</td>
            //     </tr>
            //     <tr>
            //     <td>gggg</td>
            //     <td>ffff</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| abc | def | \n|---|---|\n| cde| ddd| \n| eee| fff|\n| fff | fffff   | \n|gggg  | ffff | ", "<table>\n<thead>\n<tr>\n<th>abc</th>\n<th>def</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>cde</td>\n<td>ddd</td>\n</tr>\n<tr>\n<td>eee</td>\n<td>fff</td>\n</tr>\n<tr>\n<td>fff</td>\n<td>fffff</td>\n</tr>\n<tr>\n<td>gggg</td>\n<td>ffff</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 24\nSection Extensions / Gfm Pipe Table\n");
        }

        // **Normalized columns count**
        // 
        // The tables are normalized to the number of columns found in the table header.
        // Extra columns will be ignored, missing columns will be inserted.
        [Test]
        public void ExtensionsGfmPipeTable_Example025()
        {
            // Example 25
            // Section: Extensions / Gfm Pipe Table
            //
            // The following Markdown:
            //     a | b
            //     -- | - 
            //     0 | 1 | 2
            //     3 |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>a</th>
            //     <th>b</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>0</td>
            //     <td>1</td>
            //     </tr>
            //     <tr>
            //     <td>3</td>
            //     <td></td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("a | b\n-- | - \n0 | 1 | 2\n3 |", "<table>\n<thead>\n<tr>\n<th>a</th>\n<th>b</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>0</td>\n<td>1</td>\n</tr>\n<tr>\n<td>3</td>\n<td></td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 25\nSection Extensions / Gfm Pipe Table\n");
        }
    }
}
