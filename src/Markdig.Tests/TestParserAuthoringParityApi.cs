using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Tests;

[TestFixture]
public sealed class TestParserAuthoringParityApi
{
    [Test]
    public void TrackTriviaCanBeConfiguredFromBuilder()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            TrackTrivia = true
        }.Build();

        Assert.That(pipeline.TrackTrivia, Is.True);
    }

    [Test]
    public void CustomBlockParserCanTakeLinesBefore()
    {
        var pipeline = new MarkdownPipelineBuilder
        {
            TrackTrivia = true
        };
        pipeline.BlockParsers.InsertBefore<ParagraphBlockParser>(new LinesBeforeBlockParser());

        var document = Markdown.Parse("\n\n!marker", pipeline.Build());

        Assert.That(document.Count, Is.EqualTo(1));
        var block = document[0] as LinesBeforeLeafBlock;
        Assert.That(block, Is.Not.Null);
        Assert.That(block!.LinesBefore, Is.Not.Null);
        Assert.That(block.LinesBefore!.Count, Is.EqualTo(2));
    }

    [Test]
    public void InlineParserCanReplaceParentContainerThroughPublicApi()
    {
        var pipeline = new MarkdownPipelineBuilder();
        pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new ParentContainerReplacementInlineParser());

        var document = Markdown.Parse(
            """
            > [!TEST]
            > body
            """.ReplaceLineEndings("\n"),
            pipeline.Build());

        Assert.That(document.Count, Is.EqualTo(1));
        var replacementBlock = document[0] as ReplacementContainerBlock;
        Assert.That(replacementBlock, Is.Not.Null);

        var paragraph = replacementBlock!.Count > 0 ? replacementBlock[0] as ParagraphBlock : null;
        Assert.That(paragraph, Is.Not.Null);
        var literal = paragraph!.Inline?.FirstChild as LiteralInline;
        Assert.That(literal, Is.Not.Null);
        Assert.That(literal!.Content.ToString(), Is.EqualTo("body"));
    }

    private sealed class LinesBeforeBlockParser : BlockParser
    {
        public LinesBeforeBlockParser()
        {
            OpeningCharacters = ['!'];
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.CurrentChar != '!')
            {
                return BlockState.None;
            }

            var block = new LinesBeforeLeafBlock(this)
            {
                Line = processor.LineIndex,
                Column = processor.Column,
                Span = new SourceSpan(processor.Start, processor.Line.End)
            };

            if (processor.TrackTrivia)
            {
                block.LinesBefore = processor.TakeLinesBefore();
            }

            processor.NewBlocks.Push(block);
            return BlockState.BreakDiscard;
        }
    }

    private sealed class LinesBeforeLeafBlock : LeafBlock
    {
        public LinesBeforeLeafBlock(BlockParser parser) : base(parser)
        {
            ProcessInlines = false;
        }
    }

    private sealed class ReplacementContainerBlock : ContainerBlock
    {
        public ReplacementContainerBlock() : base(null)
        {
        }
    }

    private sealed class ParentContainerReplacementInlineParser : InlineParser
    {
        private const string Marker = "[!TEST]";

        public ParentContainerReplacementInlineParser()
        {
            OpeningCharacters = ['['];
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (processor.Block is not ParagraphBlock paragraphBlock
                || paragraphBlock.Parent is not QuoteBlock quoteBlock
                || paragraphBlock.Inline?.FirstChild is not null
                || quoteBlock.Parent is not MarkdownDocument)
            {
                return false;
            }

            if (!slice.Match(Marker))
            {
                return false;
            }

            slice.Start += Marker.Length;

            while (slice.CurrentChar.IsSpaceOrTab())
            {
                slice.NextChar();
            }

            var c = slice.CurrentChar;
            if (c == '\r')
            {
                slice.SkipChar();
                if (slice.CurrentChar == '\n')
                {
                    slice.SkipChar();
                }
            }
            else if (c == '\n')
            {
                slice.SkipChar();
            }
            else if (c != '\0')
            {
                return false;
            }

            var replacementBlock = new ReplacementContainerBlock
            {
                Span = quoteBlock.Span,
                Line = quoteBlock.Line,
                Column = quoteBlock.Column
            };

            var parent = quoteBlock.Parent!;
            var quoteBlockIndex = parent.IndexOf(quoteBlock);
            parent[quoteBlockIndex] = replacementBlock;

            while (quoteBlock.Count > 0)
            {
                var child = quoteBlock[0];
                quoteBlock.RemoveAt(0);
                replacementBlock.Add(child);
            }

            processor.ReplaceParentContainer(quoteBlock, replacementBlock);
            return true;
        }
    }

}
