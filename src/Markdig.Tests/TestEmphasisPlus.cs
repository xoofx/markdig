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