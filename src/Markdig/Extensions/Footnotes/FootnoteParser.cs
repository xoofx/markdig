// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Footnotes
{
    /// <summary>
    /// The block parser for a <see cref="Footnote"/>.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class FootnoteParser : BlockParser
    {
        /// <summary>
        /// The key used to store at the document level the pending <see cref="FootnoteGroup"/>
        /// </summary>
        private static readonly object DocumentKey = typeof(Footnote);

        public FootnoteParser()
        {
            OpeningCharacters = new [] {'['};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            return TryOpen(processor, false);
        }

        private BlockState TryOpen(BlockProcessor processor, bool isContinue)
        {
            // We expect footnote to appear only at document level and not indented more than a code indent block
            if (processor.IsCodeIndent || (!isContinue && processor.CurrentContainer.GetType() != typeof(MarkdownDocument)) || (isContinue && !(processor.CurrentContainer is Footnote)))
            {
                return BlockState.None;
            }

            var saved = processor.Column;
            string label;
            int start = processor.Start;
            SourceSpan labelSpan;
            if (!LinkHelper.TryParseLabel(ref processor.Line, false, out label, out labelSpan) || !label.StartsWith("^") || processor.CurrentChar != ':')
            {
                processor.GoToColumn(saved);
                return BlockState.None;
            }

            // Advance the column
            int deltaColumn = processor.Start - start;
            processor.Column = processor.Column + deltaColumn;

            processor.NextChar(); // Skip ':'

            var footnote = new Footnote(this)
            {
                Label = label,
                LabelSpan = labelSpan,
            };

            // Maintain a list of all footnotes at document level
            var footnotes = processor.Document.GetData(DocumentKey) as FootnoteGroup;
            if (footnotes == null)
            {
                footnotes = new FootnoteGroup(this);
                processor.Document.Add(footnotes);
                processor.Document.SetData(DocumentKey, footnotes);
                processor.Document.ProcessInlinesEnd += Document_ProcessInlinesEnd;
            }
            footnotes.Add(footnote);

            var linkRef = new FootnoteLinkReferenceDefinition()
            {
                Footnote = footnote,
                CreateLinkInline = CreateLinkToFootnote
            };
            processor.Document.SetLinkReferenceDefinition(footnote.Label, linkRef);
            processor.NewBlocks.Push(footnote);
            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var footnote = (Footnote) block;

            if (processor.CurrentBlock == null || processor.CurrentBlock.IsBreakable)
            {
                if (processor.IsBlankLine)
                {
                    footnote.IsLastLineEmpty = true;
                    return BlockState.ContinueDiscard;
                }

                if (processor.Column == 0)
                {
                    if (footnote.IsLastLineEmpty)
                    {
                        // Close the current footnote
                        processor.Close(footnote);

                        // Parse any opening footnote
                        return TryOpen(processor);
                    }

                    // Make sure that consecutive footnotes without a blanklines are parsed correctly
                    if (TryOpen(processor, true) == BlockState.Continue)
                    {
                        processor.Close(footnote);
                        return BlockState.Continue;
                    }
                }
            }
            footnote.IsLastLineEmpty = false;

            if (processor.IsCodeIndent)
            {
                processor.GoToCodeIndent();
            }

            return BlockState.Continue;
        }

        /// <summary>
        /// Add footnotes to the end of the document
        /// </summary>
        /// <param name="state">The processor.</param>
        /// <param name="inline">The inline.</param>
        private void Document_ProcessInlinesEnd(InlineProcessor state, Inline inline)
        {
            // Unregister
            state.Document.ProcessInlinesEnd -= Document_ProcessInlinesEnd;

            var footnotes = ((FootnoteGroup)state.Document.GetData(DocumentKey));
            // Remove the footnotes from the document and readd them at the end
            state.Document.Remove(footnotes);
            state.Document.Add(footnotes);
            state.Document.RemoveData(DocumentKey);

            footnotes.Sort(
                (leftObj, rightObj) =>
                {
                    var left = (Footnote)leftObj;
                    var right = (Footnote)rightObj;

                    return left.Order >= 0 && right.Order >= 0
                        ? left.Order.CompareTo(right.Order)
                        : 0;
                });

            int linkIndex = 0;
            for (int i = 0; i < footnotes.Count; i++)
            {
                var footnote = (Footnote)footnotes[i];
                if (footnote.Order < 0)
                {
                    // Remove this footnote if it doesn't have any links
                    footnotes.RemoveAt(i);
                    i--;
                    continue;
                }

                // Insert all footnote backlinks
                var paragraphBlock = footnote.LastChild as ParagraphBlock;
                if (paragraphBlock == null)
                {
                    paragraphBlock = new ParagraphBlock();
                    footnote.Add(paragraphBlock);
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

        private static Inline CreateLinkToFootnote(InlineProcessor state, LinkReferenceDefinition linkRef, Inline child)
        {
            var footnote = ((FootnoteLinkReferenceDefinition)linkRef).Footnote;
            if (footnote.Order < 0)
            {
                var footnotes = (FootnoteGroup)state.Document.GetData(DocumentKey);
                footnotes.CurrentOrder++;
                footnote.Order = footnotes.CurrentOrder;
            }

            var link = new FootnoteLink() {Footnote = footnote};
            footnote.Links.Add(link);

            return link;
        }
    }
}