using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestParserAuthoringHelpers
{
    [Test]
    public void GetParserStateResetsBetweenLeafBlocksAndEmitAppendsInline()
    {
        var pipeline = new MarkdownPipelineBuilder();
        pipeline.InlineParsers.InsertBefore<AutolinkInlineParser>(new CountingInlineParser());

        var html = Markdown.ToHtml(
            """
            @@

            @
            """.ReplaceLineEndings("\n"),
            pipeline.Build());

        Assert.That(html, Is.EqualTo("<p>12</p>\n<p>1</p>\n"));
    }

    [Test]
    public void TryDiscardOnlyDiscardsOpenNonRootBlocks()
    {
        var parser = new ParagraphBlockParser();
        var document = new MarkdownDocument();
        var processor = new BlockProcessor(document, new BlockParserList([parser]), context: null, trackTrivia: false);

        var detached = new ParagraphBlock(parser);
        Assert.That(processor.TryDiscard(detached), Is.False);

        var paragraph = new ParagraphBlock(parser);
        document.Add(paragraph);
        processor.Open(paragraph);

        Assert.That(document.Count, Is.EqualTo(1));
        Assert.That(processor.TryDiscard(paragraph), Is.True);
        Assert.That(document.Count, Is.EqualTo(0));
        Assert.That(paragraph.Parent, Is.Null);

        Assert.That(processor.TryDiscard(document), Is.False);
    }

    [Test]
    public void EmitUpdatesInlineAndLeafSpans()
    {
        var pipeline = new MarkdownPipelineBuilder();
        pipeline.InlineParsers.InsertBefore<AutolinkInlineParser>(new SpanEmittingInlineParser());

        var document = Markdown.Parse("~", pipeline.Build());
        var paragraph = document[0] as ParagraphBlock;

        Assert.That(paragraph, Is.Not.Null);
        Assert.That(paragraph!.Inline, Is.Not.Null);
        Assert.That(paragraph.Inline!.Span, Is.EqualTo(new SourceSpan(0, 0)));
        Assert.That(paragraph.Span, Is.EqualTo(new SourceSpan(0, 0)));
    }

    private sealed class CountingInlineParser : InlineParser
    {
        public CountingInlineParser()
        {
            OpeningCharacters = ['@'];
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (slice.CurrentChar != '@')
            {
                return false;
            }

            var state = processor.GetParserState<CounterState>(this);
            state.Count++;

            processor.Emit(new LiteralInline(state.Count.ToString()));
            slice.SkipChar();
            return true;
        }
    }

    private sealed class CounterState
    {
        public int Count { get; set; }
    }

    private sealed class SpanEmittingInlineParser : InlineParser
    {
        public SpanEmittingInlineParser()
        {
            OpeningCharacters = ['~'];
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (slice.CurrentChar != '~')
            {
                return false;
            }

            int start = processor.GetSourcePosition(slice.Start, out int line, out int column);
            processor.Emit(new LiteralInline("x")
            {
                Line = line,
                Column = column,
                Span = new SourceSpan(start, start)
            });

            slice.SkipChar();
            return true;
        }
    }
}
