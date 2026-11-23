// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

#nullable enable

using System.Diagnostics;
using System.Reflection;

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestDebuggerDisplay
{
    private static IEnumerable<TestCaseData> DisplayCases()
    {
        yield return new TestCaseData(new BlankLineBlock(), "");
        yield return new TestCaseData(new ContainerInline(), "");
        yield return new TestCaseData(new MarkdownDocument(), ", Count: 0");
        yield return new TestCaseData(new ParagraphBlock(new ParagraphBlockParser()), ", Lines: 0");
        yield return new TestCaseData(new HeadingBlock(new HeadingBlockParser()) { Level = 3 }, ", Level: 3");
        yield return new TestCaseData(new ListBlock(new ListBlockParser()) { BulletType = '-' }, ", Count: 0, Ordered: False, Marker: '-'");
        yield return new TestCaseData(
            new ListBlock(new ListBlockParser()) { IsOrdered = true, BulletType = '1', OrderedDelimiter = ')' },
            ", Count: 0, Ordered: True, Marker: ')'");
        yield return new TestCaseData(new ListItemBlock(new ListBlockParser()) { Order = 3 }, ", Count: 0, Order: 3");
        yield return new TestCaseData(new QuoteBlock(new QuoteBlockParser()) { QuoteChar = '>' }, ", Count: 0, Marker: '>'");
        yield return new TestCaseData(
            new FencedCodeBlock(new FencedCodeBlockParser()) { Info = "csharp", Lines = new StringLineGroup("var x = 1;") },
            ", Info: csharp, Lines: 1");
        yield return new TestCaseData(new LineBreakInline { IsHard = true, IsBackslash = true }, ", Hard: True, Backslash: True");
    }

    [TestCaseSource(nameof(DisplayCases))]
    public void SyntaxNodeShowsSourceLocationAndDetails(MarkdownObject node, string details)
    {
        node.Line = 2;
        node.Span = new SourceSpan(10, 20);

        Assert.That(GetDisplay(node), Is.EqualTo($"{node.GetType().Name} Line: 2, Span: 10-20{details}"));
    }

    [Test]
    public void SyntaxNodeShowsWhenSourceSpanIsEmpty()
    {
        Assert.That(GetDisplay(new ContainerInline()), Is.EqualTo("ContainerInline Line: 0, Span: (empty)"));
    }

    [Test]
    public void UnlabeledFenceShowsLineCount()
    {
        var fence = (FencedCodeBlock)Markdown.Parse("```\nfirst\nsecond\n```")[0];

        Assert.That(GetDisplay(fence), Does.EndWith(", Info: (none), Lines: 2"));
    }

    private static string GetDisplay(MarkdownObject node)
    {
        for (Type? type = node.GetType(); type is not null; type = type.BaseType)
        {
            var attribute = type.GetCustomAttribute<DebuggerDisplayAttribute>(inherit: false);
            if (attribute is null)
            {
                continue;
            }

            Assert.That(attribute.Value, Does.StartWith("{").And.EndWith(",nq}"));
            var propertyName = attribute.Value[1..^4];
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new AssertionException($"{type.Name} has no debugger display property named {propertyName}");
            return property.GetValue(node) as string
                ?? throw new AssertionException($"{type.Name}.{propertyName} returned no display string");
        }

        throw new AssertionException($"{node.GetType().Name} has no debugger display");
    }
}
