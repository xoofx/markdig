// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using Textamina.Markdig.Parsers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Extensions.DefinitionLists
{
    /// <summary>
    /// The block parser for a <see cref="DefinitionList"/>.
    /// </summary>
    /// <seealso cref="Textamina.Markdig.Parsers.BlockParser" />
    public class DefinitionListParser : BlockParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionListParser"/> class.
        /// </summary>
        public DefinitionListParser()
        {
            OpeningCharacters = new [] {':', '~'};
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            var paragraphBlock = processor.LastBlock as ParagraphBlock;
            if (processor.IsCodeIndent || paragraphBlock == null || paragraphBlock.LastLine - processor.LineIndex > 1)
            {
                return BlockState.None;
            }

            var column = processor.ColumnBeforeIndent;
            processor.NextChar();
            processor.ParseIndent();
            var delta = processor.Column - column;

            // We expect to have a least
            if (delta < 4)
            {
                // Return back to original position
                processor.GoToColumn(column);
                return BlockState.None;
            }

            if (delta > 4)
            {
                processor.GoToColumn(column + 4);
            }

            var previousParent = paragraphBlock.Parent;
            var indexOfParagraph = previousParent.IndexOf(paragraphBlock);
            var currentDefinitionList = indexOfParagraph - 1 >= 0 ? previousParent[indexOfParagraph - 1] as DefinitionList : null;

            processor.Discard(paragraphBlock);

            // If the paragraph block was not part of the opened blocks, we need to remove it manually from its parent container
            if (paragraphBlock.Parent != null)
            {
                paragraphBlock.Parent.Remove(paragraphBlock);
            }

            if (currentDefinitionList == null)
            {
                currentDefinitionList = new DefinitionList(this);
                previousParent.Add(currentDefinitionList);
            }

            var definitionItem = new DefinitionItem(this)
            {
                Column =  processor.Column,
                OpeningCharacter = processor.CurrentChar,
            };
            currentDefinitionList.Add(definitionItem);

            for (int i = 0; i < paragraphBlock.Lines.Count; i++)
            {
                var line = paragraphBlock.Lines.Lines[i];
                var term = new DefinitionTerm(this)
                {
                    Column =  paragraphBlock.Column,
                    Line = line.Line,
                    IsOpen = false
                };
                term.AppendLine(ref line.Slice, line.Column, line.Line);
                definitionItem.Add(term);
            }

            processor.Open(definitionItem);

            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            var definitionItem = (DefinitionItem)block;
            if (processor.IsCodeIndent)
            {
                processor.GoToCodeIndent();
                return BlockState.Continue;
            }

            var lastBlankLine = definitionItem.LastChild as BlankLineBlock;

            // Check if we have another definition list
            if (Array.IndexOf(OpeningCharacters, processor.CurrentChar) >= 0)
            {
                var column = processor.ColumnBeforeIndent;
                processor.NextChar();
                processor.ParseIndent();
                var delta = processor.Column - column;

                // We expect to have a least
                if (delta < 4)
                {
                    // Remove the blankline before breaking this definition item
                    if (lastBlankLine != null)
                    {
                        definitionItem.RemoveAt(definitionItem.Count - 1);
                    }
                    return BlockState.None;
                }

                if (delta > 4)
                {
                    processor.GoToColumn(column + 4);
                }

                var list = (DefinitionList) definitionItem.Parent;
                processor.Close(definitionItem);
                var nextDefinitionItem = new DefinitionItem(this)
                {
                    Column = processor.Column,
                    OpeningCharacter = processor.CurrentChar,
                };
                list.Add(nextDefinitionItem);
                processor.Open(nextDefinitionItem);

                return BlockState.Continue;
            }

            var isBreakable = definitionItem.LastChild?.IsBreakable ?? true;
            if (processor.IsBlankLine)
            {
                if (lastBlankLine == null && isBreakable)
                {
                    definitionItem.Add(new BlankLineBlock());
                }
                return isBreakable ? BlockState.ContinueDiscard : BlockState.Continue;
            }

            var paragraphBlock = definitionItem.LastChild as ParagraphBlock;
            if (lastBlankLine == null && paragraphBlock != null)
            {
                return BlockState.Continue;
            }

            // Remove the blankline before breaking this definition item
            if (lastBlankLine != null)
            {
                definitionItem.RemoveAt(definitionItem.Count - 1);
            }

            return BlockState.Break;
        }
    }
}