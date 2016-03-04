// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System.Collections.Generic;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;
using Textamina.Markdig.Syntax.Inlines;

namespace Textamina.Markdig.Extensions.Footnotes
{
    public class FootnoteParser : BlockParser
    {
        public FootnoteParser()
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
                state.GoToColumn(saved);
                return BlockState.None;
            }
           
            // Advance the column
            int deltaColumn = state.Start - start;
            state.Column = state.Column + deltaColumn;

            state.NextChar(); // Skip ':'

            var footnote = new Footnote(this) {Label = label};

            // Maintain a list of all footnotes at document level
            var footnotes = state.Document.GetData(Footnote.DocumentKey) as FootnoteGroup;
            if (footnotes == null)
            {
                footnotes = new FootnoteGroup(this);
                state.Document.SetData(Footnote.DocumentKey, footnotes);
                state.Document.ProcessInlinesEnd += Document_ProcessInlinesEnd;
            }
            footnotes.Children.Add(footnote);

            var linkRef = new FootnoteLinkReferenceDefinition()
            {
                Footnote = footnote,
                CreateLinkInline = CreateLinkToFootnote
            };
            state.Document.GetLinkReferenceDefinitions()[footnote.Label] = linkRef;

            state.NewBlocks.Push(footnote);
            return BlockState.Continue;
        }

        /// <summary>
        /// Add footnotes to the end of the document
        /// </summary>
        /// <param name="state">The state.</param>
        private void Document_ProcessInlinesEnd(InlineParserState state)
        {
            // Unregister
            state.Document.ProcessInlinesEnd -= Document_ProcessInlinesEnd;

            var footnoteGroup = ((FootnoteGroup)state.Document.GetData(Footnote.DocumentKey));
            var footnotes = footnoteGroup.Children;
            state.Document.Children.Add(footnoteGroup);

            footnotes.Sort(
                (leftObj, rightObj) =>
                {
                    var left = (Footnote) leftObj;
                    var right = (Footnote) rightObj;

                    return left.Order.HasValue && right.Order.HasValue
                        ? left.Order.Value.CompareTo(right.Order.Value)
                        : 0;
                });

            int linkIndex = 0;
            for (int i = 0; i < footnotes.Count; i++)
            {
                var footnote = (Footnote)footnotes[i];
                if (!footnote.Order.HasValue)
                {
                    // Remove this footnote if it doesn't have any links
                    footnotes.RemoveAt(i);
                    i--;
                    continue;
                }

                // Insert all footnote backlinks
                var paragraphBlock = footnote.Children.Count > 0
                    ? footnote.Children[footnote.Children.Count - 1] as ParagraphBlock
                    : null;
                if (paragraphBlock == null)
                {
                    paragraphBlock = new ParagraphBlock();
                    footnote.Children.Add(paragraphBlock);
                }
                if (paragraphBlock.Inline == null)
                {
                    paragraphBlock.Inline = new ContainerInline();
                }

                foreach (var link in footnote.Links)
                {
                    linkIndex++;
                    link.Index = linkIndex;
                    var backLink = new FootnoteLink()
                    {
                        Index = linkIndex,
                        IsBackLink = true,
                        Footnote = footnote
                    };
                    paragraphBlock.Inline.AppendChild(backLink);
                }
            }
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            var footnote = (Footnote) block;

            if (!(state.LastBlock is FencedCodeBlock))
            {
                if (state.IsBlankLine)
                {
                    footnote.IsLastLineEmpty = true;
                    return BlockState.ContinueDiscard;
                }

                if (footnote.IsLastLineEmpty && state.Start == 0)
                {
                    return BlockState.Break;
                }
            }
            footnote.IsLastLineEmpty = false;

            if (state.IsCodeIndent)
            {
                state.GoToCodeIndent();
            }

            return BlockState.Continue;
        }

        private static Inline CreateLinkToFootnote(InlineParserState state, LinkReferenceDefinition linkRef, Inline child)
        {
            var footnote = ((FootnoteLinkReferenceDefinition)linkRef).Footnote;
            if (footnote.Order == null)
            {
                var footnotes = (FootnoteGroup)state.Document.GetData(Footnote.DocumentKey);
                footnotes.CurrentOrder++;
                footnote.Order = footnotes.CurrentOrder;
            }

            var link = new FootnoteLink() {Footnote = footnote};
            footnote.Links.Add(link);

            return link;
        }
    }
}