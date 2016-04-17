// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using NUnit.Framework;

namespace Markdig.Tests
{
    [TestFixture]
    public class TestPlayParser
    {
        [Test]
        public void TestSimple()
        {
            var text = @" *[HTML]: Hypertext Markup Language

Later in a text we are using HTML and it becomes an abbr tag HTML
";

            //            var reader = new StringReader(@"> > toto tata
            //> titi toto
            //");

            //var result = Markdown.ToHtml(text, new MarkdownPipeline().UseFootnotes().UseEmphasisExtra());
            var result = Markdown.ToHtml(text, new MarkdownPipeline().UseAbbreviation());
            //File.WriteAllText("test.html", result, Encoding.UTF8);
            Console.WriteLine(result);
        }

// Test for emoji and smileys
//        var text = @" This is a test with a :) and a :angry: smiley";


// Test for definition lists:
//
//        var text = @"
//Term 1
//:   This is a definition item
//    With a paragraph
//    > This is a block quote

//    - This is a list
//    - item2

//    ```java
//    Test


//    ```

//    And a lazy line
//:   This ia another definition item

//Term2
//Term3 *with some inline*
//:   This is another definition for term2
//";


        // Test for grid table


        //        var text = @"
        //+-----------------------------------+--------------------------------------+
        //| - this is a list                  | > We have a blockquote
        //| - this is a second item           |
        //|                                   |
        //| ```                               |
        //| Yes                               |
        //| ```                               |
        //+===================================+======================================+
        //| This is a second line             | 
        //+-----------------------------------+--------------------------------------+

        //:::spoiler  {#yessss}
        //This is a spoiler
        //:::

        ///| we have mult | paragraph    |
        ///| we have a new colspan with a long line
        ///| and lots of text
        //";


    }
}