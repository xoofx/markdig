
// --------------------------------
//       CJK-friendly Emphasis
// --------------------------------

using System;
using NUnit.Framework;

namespace Markdig.Tests.Specs.CJKFriendlyEmphasis
{
    [TestFixture]
    public class TestCJKFriendlyEmphasisExtension
    {
        // ## CJK-friendly Emphasis Extension
        // 
        // See https://github.com/tats-u/markdown-cjk-friendly/blob/main/specification.md for details about the spec of this extension.
        // 
        // This extension drastically mitigates [the long-standing issue (specification flaw)](https://github.com/commonmark/commonmark-spec/issues/650) in CommonMark that emphasis in CJK languages is often not parsed as expected.
        // 
        // The plain CommonMark cannot recognize even the following emphasis in CJK languages:
        [Test]
        public void CJKFriendlyEmphasisExtension_Example001()
        {
            // Example 1
            // Section: CJK-friendly Emphasis Extension
            //
            // The following Markdown:
            //     **この文を強調できますか（Can I emphasize this sentence）？**残念ながらこの文のせいでできません（Unfortunately not possible due to this sentence）。
            //
            // Should be rendered as:
            //     <p><strong>この文を強調できますか（Can I emphasize this sentence）？</strong>残念ながらこの文のせいでできません（Unfortunately not possible due to this sentence）。</p>

            TestParser.TestSpec("**この文を強調できますか（Can I emphasize this sentence）？**残念ながらこの文のせいでできません（Unfortunately not possible due to this sentence）。", "<p><strong>この文を強調できますか（Can I emphasize this sentence）？</strong>残念ながらこの文のせいでできません（Unfortunately not possible due to this sentence）。</p>", "emphasisextras|advanced", context: "Example 1\nSection CJK-friendly Emphasis Extension\n");
        }

        // ````````````````````````````````` example
        // 我可以强调**这个`code`**吗（Can I emphasize **this `code`**）？
        // .
        // <p>我可以强调<code>这个`code`</code>吗（Can I emphasize <strong>this <code>code</code></strong>）？</p>
        // `````````````````````````````````
        [Test]
        public void CJKFriendlyEmphasisExtension_Example002()
        {
            // Example 2
            // Section: CJK-friendly Emphasis Extension
            //
            // The following Markdown:
            //     **이 용어(This term)**를 강조해 주세요. (Please emphasize **this term**.)
            //
            // Should be rendered as:
            //     <p><strong>이 용어(This term)</strong>를 강조해 주세요. (Please emphasize <strong>this term</strong>.)</p>

            TestParser.TestSpec("**이 용어(This term)**를 강조해 주세요. (Please emphasize **this term**.)", "<p><strong>이 용어(This term)</strong>를 강조해 주세요. (Please emphasize <strong>this term</strong>.)</p>", "emphasisextras|advanced", context: "Example 2\nSection CJK-friendly Emphasis Extension\n");
        }
        // You can compare the results with and without this extension: https://tats-u.github.io/markdown-cjk-friendly/?sc8=KirjgZPjga7mlofjgpLlvLfoqr_jgafjgY3jgb7jgZnjgYvvvIhDYW4gSSBlbXBoYXNpemUgdGhpcyBzZW50ZW5jZe-8ie-8nyoq5q6L5b-144Gq44GM44KJ44GT44Gu5paH44Gu44Gb44GE44Gn44Gn44GN44G-44Gb44KT77yIVW5mb3J0dW5hdGVseSBub3QgcG9zc2libGUgZHVlIHRvIHRoaXMgc2VudGVuY2XvvInjgIIKCuaIkeWPr-S7peW8uuiwgyoq6L-Z5LiqYGNvZGVgKirlkJfvvIhDYW4gSSBlbXBoYXNpemUgKip0aGlzIGBjb2RlYCoq77yJ77yfCgoqKuydtCDsmqnslrQoVGhpcyB0ZXJtKSoq66W8IOqwleyhsO2VtCDso7zshLjsmpQuIChQbGVhc2UgZW1waGFzaXplICoqdGhpcyB0ZXJtKiouKQo&gfm=1&engine=markdown-it
        // 
        // You will find how poor the plain CommonMark is for CJK languages.
        // 
        // To use this extension, configure the pipeline as follows:
        // 
        // ```csharp
        // var pipeline = new MarkdownPipelineBuilder()
        //     .UseCJKFriendlyEmphasis() // Add this
        //     .Build();
        // ```
    }
}
