using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteBlock : ContainerBlock
    {
        public FootnoteBlock(BlockParser parser) : base(parser)
        {
            RemoveAfterProcessInlines = true;
        }

        public string Label { get; set; }

        public int? Order { get; set; }

        internal bool IsLastLineEmpty { get; set; }
    }

    public class FootnoteBlockParser : BlockParser
    {
        public FootnoteBlockParser()
        {
            OpeningCharacters = new [] {'['};
        }

        public override BlockState TryOpen(BlockParserState state)
        {
            // We expect footnote to appear only at document level and not indented more than a code indent block
            if (state.IsCodeIndent || state.CurrentContainer.GetType() != typeof(Document) )
            {
                return BlockState.None;
            }

            var saved = state.Column;
            string label;
            int start = state.Start;
            if (!LinkHelper.TryParseLabel(ref state.Line, false, out label) || !label.StartsWith("^") || state.CurrentChar != ':')
            {
                state.ResetToColumn(saved);
                return BlockState.None;
            }
           
            // Advance the column
            int deltaColumn = state.Start - start;
            state.Column = state.Column + deltaColumn;

            state.NextChar(); // Skip ':'

            var footnote = new FootnoteBlock(this) {Label = label};
            state.NewBlocks.Push(footnote);
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var footnote = (FootnoteBlock) block;

            if (state.IsBlankLine)
            {
                footnote.IsLastLineEmpty = true;
                return BlockState.ContinueDiscard;
            }

            if (footnote.IsLastLineEmpty && state.Start == 0)
            {
                return BlockState.Break;
            }
            footnote.IsLastLineEmpty = false;

            return BlockState.Continue;
        }

        public override bool Close(BlockParserState state, Block block)
        {
            var footnote = (FootnoteBlock) block;
            // Maintain a list of all footnotes at document level
            var footnotes = state.Document.GetData(typeof (FootnoteBlock)) as FootnoteCollection;
            if (footnotes == null)
            {
                footnotes = new FootnoteCollection();
                state.Document.SetData(typeof (FootnoteBlock), footnotes);
            }
            footnotes.Add(footnote);

            var linkRef = new LinkReferenceDefinitionBlock {CreateLinkInline = CreateLinkToFootnote};
            linkRef.SetData(typeof(FootnoteBlock), footnote);

            state.Document.LinkReferenceDefinitions[footnote.Label] = linkRef;
            return true;
        }

        private static LinkInline CreateLinkToFootnote(InlineParserState state, LinkReferenceDefinitionBlock linkRef, out bool acceptChild)
        {
            acceptChild = false;

            var footnote = (FootnoteBlock) linkRef.GetData(typeof (FootnoteBlock));
            if (footnote.Order == null)
            {
                var footnotes = (FootnoteCollection)state.Document.GetData(typeof(FootnoteBlock));
                footnotes.Order++;
                footnote.Order = footnotes.Order;
            }

            var link = new LinkInline
            {
                Url = $"#fn:{footnote.Order}",
                Id = $"fnref:{footnote.Order}"
            };

            // TODO: Add superscript
            link.AppendChild(new LiteralInline($"^{footnote.Order}^"));
            return link;
        }

        private class FootnoteCollection : List<FootnoteBlock>
        {
            public int Order { get; set; }
        }
    }
}