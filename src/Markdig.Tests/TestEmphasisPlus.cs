// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public partial class TestEmphasisPlus
{
    [Test]
    public void StrongNormal()
    {
        TestParser.TestSpec("***Strong emphasis*** normal", "<p><em><strong>Strong emphasis</strong></em> normal</p>", "");
    }

    [Test]
    public void NormalStrongNormal()
    {
        TestParser.TestSpec("normal ***Strong emphasis*** normal", "<p>normal <em><strong>Strong emphasis</strong></em> normal</p>", "");
    }

    [Test]
    public void SupplementaryPunctuation()
    {
        TestParser.TestSpec("a*a∇*a\n\na*∇a*a\n\na*a𝜵*a\n\na*𝜵a*a\n\na*𐬼a*a\n\na*a𐬼*a", "<p>a*a∇*a</p>\n<p>a*∇a*a</p>\n<p>a*a𝜵*a</p>\n<p>a*𝜵a*a</p>\n<p>a*𐬼a*a</p>\n<p>a*a𐬼*a</p>", "");
    }

    [Test]
    public void MinusAndPlusArePunctuationForFlanking()
    {
        // `-` and `+` are ASCII punctuation, so `**` after them is not right-flanking when followed by a letter,
        // and `*` between a letter and `-` is not left-flanking (same as for any other punctuation such as `?`).
        TestParser.TestSpec("**x-**h", "<p>**x-**h</p>", "");
        TestParser.TestSpec("**x+**h", "<p>**x+**h</p>", "");
        TestParser.TestSpec("a-*a**", "<p>a-<em>a</em>*</p>", "");
        TestParser.TestSpec("*a**-", "<p><em>a</em>*-</p>", "");
        TestParser.TestSpec("a-_b_", "<p>a-<em>b</em></p>", "");
    }

    [Test]
    public void RecognizeSupplementaryChars()
    {
        TestParser.TestSpec("🌶️**𰻞**🍜**𰻞**🌶️**麺**🍜", "<p>🌶️<strong>𰻞</strong>🍜<strong>𰻞</strong>🌶️<strong>麺</strong>🍜</p>", "");
    }

    [Test]
    public void OpenEmphasisHasConvenientContentStringSlice()
    {
        var pipeline = new MarkdownPipelineBuilder().Build();

        var document = Markdown.Parse("test*test", pipeline);

        var emphasisDelimiterLiteral = (LiteralInline)((ParagraphBlock)document.LastChild).Inline.ElementAt(1);
        Assert.That(emphasisDelimiterLiteral.Content.Text == "test*test");
        Assert.That(emphasisDelimiterLiteral.Content.Start == 4);
        Assert.That(emphasisDelimiterLiteral.Content.End == 4);
    }
}