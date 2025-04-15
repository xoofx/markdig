
// --------------------------------
//          Emphasis Extra
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.EmphasisExtra
{
    [TestFixture]
    public class TestExtensionsStrikethrough
    {
        // # Extensions
        // 
        // The following additional emphasis are supported:
        // 
        // ## Strikethrough
        //  
        // Allows to strikethrough a span of text by surrounding it by `~~`. The semantic used for the generated HTML is the tag `<del>`.
        [Test]
        public void ExtensionsStrikethrough_Example001()
        {
            // Example 1
            // Section: Extensions / Strikethrough
            //
            // The following Markdown:
            //     The following text ~~is deleted~~
            //
            // Should be rendered as:
            //     <p>The following text <del>is deleted</del></p>

            TestParser.TestSpec("The following text ~~is deleted~~", "<p>The following text <del>is deleted</del></p>", "emphasisextras|advanced", context: "Example 1\nSection Extensions / Strikethrough\n");
        }
    }

    [TestFixture]
    public class TestExtensionsSuperscriptAndSubscript
    {
        // ## Superscript and Subscript
        //  
        // Superscripts can be written by surrounding a text by ^ characters; subscripts can be written by surrounding the subscripted text by ~ characters
        [Test]
        public void ExtensionsSuperscriptAndSubscript_Example002()
        {
            // Example 2
            // Section: Extensions / Superscript and Subscript
            //
            // The following Markdown:
            //     H~2~O is a liquid. 2^10^ is 1024
            //
            // Should be rendered as:
            //     <p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>

            TestParser.TestSpec("H~2~O is a liquid. 2^10^ is 1024", "<p>H<sub>2</sub>O is a liquid. 2<sup>10</sup> is 1024</p>", "emphasisextras|advanced", context: "Example 2\nSection Extensions / Superscript and Subscript\n");
        }

        // Certain punctuation characters are exempted from the rule forbidding them within inline delimiters
        [Test]
        public void ExtensionsSuperscriptAndSubscript_Example003()
        {
            // Example 3
            // Section: Extensions / Superscript and Subscript
            //
            // The following Markdown:
            //     One quintillionth can be expressed as 10^-18^
            //     
            //     Daggers^†^ and double-daggers^‡^ can be used to denote notes.
            //
            // Should be rendered as:
            //     <p>One quintillionth can be expressed as 10<sup>-18</sup></p>
            //     <p>Daggers<sup>†</sup> and double-daggers<sup>‡</sup> can be used to denote notes.</p>

            TestParser.TestSpec("One quintillionth can be expressed as 10^-18^\n\nDaggers^†^ and double-daggers^‡^ can be used to denote notes.", "<p>One quintillionth can be expressed as 10<sup>-18</sup></p>\n<p>Daggers<sup>†</sup> and double-daggers<sup>‡</sup> can be used to denote notes.</p>", "emphasisextras|advanced", context: "Example 3\nSection Extensions / Superscript and Subscript\n");
        }
    }

    [TestFixture]
    public class TestExtensionsInserted
    {
        // ## Inserted
        // 
        // Inserted text can be used to specify that a text has been added to a document.  The semantic used for the generated HTML is the tag `<ins>`.
        [Test]
        public void ExtensionsInserted_Example004()
        {
            // Example 4
            // Section: Extensions / Inserted
            //
            // The following Markdown:
            //     ++Inserted text++
            //
            // Should be rendered as:
            //     <p><ins>Inserted text</ins></p>

            TestParser.TestSpec("++Inserted text++", "<p><ins>Inserted text</ins></p>", "emphasisextras|advanced", context: "Example 4\nSection Extensions / Inserted\n");
        }
    }

    [TestFixture]
    public class TestExtensionsMarked
    {
        // ## Marked
        // 
        // Marked text can be used to specify that a text has been marked in a document.  The semantic used for the generated HTML is the tag `<mark>`.
        [Test]
        public void ExtensionsMarked_Example005()
        {
            // Example 5
            // Section: Extensions / Marked
            //
            // The following Markdown:
            //     ==Marked text==
            //
            // Should be rendered as:
            //     <p><mark>Marked text</mark></p>

            TestParser.TestSpec("==Marked text==", "<p><mark>Marked text</mark></p>", "emphasisextras|advanced", context: "Example 5\nSection Extensions / Marked\n");
        }
    }

    [TestFixture]
    public class TestExtensionsEmphasisOnHtmlEntities
    {
        // ## Emphasis on Html Entities
        // 
        // Note that Unicode symbols are treated as punctuation, which are not allowed to open the emphasis unless they are preceded by a space.
        [Test]
        public void ExtensionsEmphasisOnHtmlEntities_Example006()
        {
            // Example 6
            // Section: Extensions / Emphasis on Html Entities
            //
            // The following Markdown:
            //     This is text MyBrand ^&reg;^ and MyTrademark ^&trade;^
            //     This is text MyBrand^&reg;^ and MyTrademark^&trade;^
            //     This is text MyBrand ~&reg;~ and MyCopyright ^&copy;^
            //
            // Should be rendered as:
            //     <p>This is text MyBrand <sup>®</sup> and MyTrademark <sup>TM</sup>
            //     This is text MyBrand^®^ and MyTrademark^TM^
            //     This is text MyBrand <sub>®</sub> and MyCopyright <sup>©</sup></p>

            TestParser.TestSpec("This is text MyBrand ^&reg;^ and MyTrademark ^&trade;^\nThis is text MyBrand^&reg;^ and MyTrademark^&trade;^\nThis is text MyBrand ~&reg;~ and MyCopyright ^&copy;^", "<p>This is text MyBrand <sup>®</sup> and MyTrademark <sup>TM</sup>\nThis is text MyBrand^®^ and MyTrademark^TM^\nThis is text MyBrand <sub>®</sub> and MyCopyright <sup>©</sup></p>", "emphasisextras|advanced", context: "Example 6\nSection Extensions / Emphasis on Html Entities\n");
        }
    }
}
