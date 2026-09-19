
// --------------------------------
//       GFM Table Conformance
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.GFMTableConformance
{
    [TestFixture]
    public class TestGFMTableConformanceBasicTable
    {
        // # GFM table conformance
        // 
        // The eight table examples from the [GFM specification, section 4.10](https://github.github.com/gfm/#tables-extension-),
        // retrieved from [cmark-gfm/test/spec.txt](https://github.com/github/cmark-gfm/blob/499789b49373bfa045d0e7547e5ee63444c77bca/test/spec.txt)
        // on 2026-09-19. Markdown inputs are unchanged. Expected HTML uses Markdig's
        // `style="text-align: ...;"` instead of the specification's `align="..."` attributes.
        // The upstream specification is licensed under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).
        // 
        // ## Basic table
        [Test]
        public void GFMTableConformanceBasicTable_Example001()
        {
            // Example 1
            // Section: GFM table conformance / Basic table
            //
            // The following Markdown:
            //     | foo | bar |
            //     | --- | --- |
            //     | baz | bim |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>foo</th>
            //     <th>bar</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>baz</td>
            //     <td>bim</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| foo | bar |\n| --- | --- |\n| baz | bim |", "<table>\n<thead>\n<tr>\n<th>foo</th>\n<th>bar</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>baz</td>\n<td>bim</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 1\nSection GFM table conformance / Basic table\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceAlignmentAndOptionalOuterPipes
    {
        // ## Alignment and optional outer pipes
        [Test]
        public void GFMTableConformanceAlignmentAndOptionalOuterPipes_Example002()
        {
            // Example 2
            // Section: GFM table conformance / Alignment and optional outer pipes
            //
            // The following Markdown:
            //     | abc | defghi |
            //     :-: | -----------:
            //     bar | baz
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th style="text-align: center;">abc</th>
            //     <th style="text-align: right;">defghi</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td style="text-align: center;">bar</td>
            //     <td style="text-align: right;">baz</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| abc | defghi |\n:-: | -----------:\nbar | baz", "<table>\n<thead>\n<tr>\n<th style=\"text-align: center;\">abc</th>\n<th style=\"text-align: right;\">defghi</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td style=\"text-align: center;\">bar</td>\n<td style=\"text-align: right;\">baz</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 2\nSection GFM table conformance / Alignment and optional outer pipes\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceEscapedPipesIncludingInsideCodeAndEmphasis
    {
        // ## Escaped pipes, including inside code and emphasis
        [Test]
        public void GFMTableConformanceEscapedPipesIncludingInsideCodeAndEmphasis_Example003()
        {
            // Example 3
            // Section: GFM table conformance / Escaped pipes, including inside code and emphasis
            //
            // The following Markdown:
            //     | f\|oo  |
            //     | ------ |
            //     | b `\|` az |
            //     | b **\|** im |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>f|oo</th>
            //     </tr>
            //     </thead>
            //     <tbody>
            //     <tr>
            //     <td>b <code>|</code> az</td>
            //     </tr>
            //     <tr>
            //     <td>b <strong>|</strong> im</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| f\\|oo  |\n| ------ |\n| b `\\|` az |\n| b **\\|** im |", "<table>\n<thead>\n<tr>\n<th>f|oo</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>b <code>|</code> az</td>\n</tr>\n<tr>\n<td>b <strong>|</strong> im</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 3\nSection GFM table conformance / Escaped pipes, including inside code and emphasis\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceAnotherBlockEndsTheTable
    {
        // ## Another block ends the table
        [Test]
        public void GFMTableConformanceAnotherBlockEndsTheTable_Example004()
        {
            // Example 4
            // Section: GFM table conformance / Another block ends the table
            //
            // The following Markdown:
            //     | abc | def |
            //     | --- | --- |
            //     | bar | baz |
            //     > bar
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
            //     <td>bar</td>
            //     <td>baz</td>
            //     </tr>
            //     </tbody>
            //     </table>
            //     <blockquote>
            //     <p>bar</p>
            //     </blockquote>

            TestParser.TestSpec("| abc | def |\n| --- | --- |\n| bar | baz |\n> bar", "<table>\n<thead>\n<tr>\n<th>abc</th>\n<th>def</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>bar</td>\n<td>baz</td>\n</tr>\n</tbody>\n</table>\n<blockquote>\n<p>bar</p>\n</blockquote>", "gfm-pipetables", context: "Example 4\nSection GFM table conformance / Another block ends the table\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformancePipeLessBodyRowsAndBlankLineTermination
    {
        // ## Pipe-less body rows and blank-line termination
        [Test]
        public void GFMTableConformancePipeLessBodyRowsAndBlankLineTermination_Example005()
        {
            // Example 5
            // Section: GFM table conformance / Pipe-less body rows and blank-line termination
            //
            // The following Markdown:
            //     | abc | def |
            //     | --- | --- |
            //     | bar | baz |
            //     bar
            //     
            //     bar
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
            //     <td>bar</td>
            //     <td>baz</td>
            //     </tr>
            //     <tr>
            //     <td>bar</td>
            //     <td></td>
            //     </tr>
            //     </tbody>
            //     </table>
            //     <p>bar</p>

            TestParser.TestSpec("| abc | def |\n| --- | --- |\n| bar | baz |\nbar\n\nbar", "<table>\n<thead>\n<tr>\n<th>abc</th>\n<th>def</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>bar</td>\n<td>baz</td>\n</tr>\n<tr>\n<td>bar</td>\n<td></td>\n</tr>\n</tbody>\n</table>\n<p>bar</p>", "gfm-pipetables", context: "Example 5\nSection GFM table conformance / Pipe-less body rows and blank-line termination\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceHeaderAndDelimiterCountsMustMatch
    {
        // ## Header and delimiter counts must match
        [Test]
        public void GFMTableConformanceHeaderAndDelimiterCountsMustMatch_Example006()
        {
            // Example 6
            // Section: GFM table conformance / Header and delimiter counts must match
            //
            // The following Markdown:
            //     | abc | def |
            //     | --- |
            //     | bar |
            //
            // Should be rendered as:
            //     <p>| abc | def |
            //     | --- |
            //     | bar |</p>

            TestParser.TestSpec("| abc | def |\n| --- |\n| bar |", "<p>| abc | def |\n| --- |\n| bar |</p>", "gfm-pipetables", context: "Example 6\nSection GFM table conformance / Header and delimiter counts must match\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceShortAndExcessBodyCells
    {
        // ## Short and excess body cells
        [Test]
        public void GFMTableConformanceShortAndExcessBodyCells_Example007()
        {
            // Example 7
            // Section: GFM table conformance / Short and excess body cells
            //
            // The following Markdown:
            //     | abc | def |
            //     | --- | --- |
            //     | bar |
            //     | bar | baz | boo |
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
            //     <td>bar</td>
            //     <td></td>
            //     </tr>
            //     <tr>
            //     <td>bar</td>
            //     <td>baz</td>
            //     </tr>
            //     </tbody>
            //     </table>

            TestParser.TestSpec("| abc | def |\n| --- | --- |\n| bar |\n| bar | baz | boo |", "<table>\n<thead>\n<tr>\n<th>abc</th>\n<th>def</th>\n</tr>\n</thead>\n<tbody>\n<tr>\n<td>bar</td>\n<td></td>\n</tr>\n<tr>\n<td>bar</td>\n<td>baz</td>\n</tr>\n</tbody>\n</table>", "gfm-pipetables", context: "Example 7\nSection GFM table conformance / Short and excess body cells\n");
        }
    }

    [TestFixture]
    public class TestGFMTableConformanceHeaderOnlyTable
    {
        // ## Header-only table
        [Test]
        public void GFMTableConformanceHeaderOnlyTable_Example008()
        {
            // Example 8
            // Section: GFM table conformance / Header-only table
            //
            // The following Markdown:
            //     | abc | def |
            //     | --- | --- |
            //
            // Should be rendered as:
            //     <table>
            //     <thead>
            //     <tr>
            //     <th>abc</th>
            //     <th>def</th>
            //     </tr>
            //     </thead>
            //     </table>

            TestParser.TestSpec("| abc | def |\n| --- | --- |", "<table>\n<thead>\n<tr>\n<th>abc</th>\n<th>def</th>\n</tr>\n</thead>\n</table>", "gfm-pipetables", context: "Example 8\nSection GFM table conformance / Header-only table\n");
        }
    }
}
