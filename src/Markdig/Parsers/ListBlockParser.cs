// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using Markdig.Helpers;
using Markdig.Syntax;

namespace Markdig.Parsers
{
    /// <summary>
    /// A parser for a list block and list item block.
    /// </summary>
    /// <seealso cref="Markdig.Parsers.BlockParser" />
    public class ListBlockParser : BlockParser
    {
        private CharacterMap<ListItemParser> mapItemParsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBlockParser"/> class.
        /// </summary>
        public ListBlockParser()
        {
            ItemParsers = new OrderedList<ListItemParser>()
            {
                new UnorderedListItemParser(),
                new NumberedListItemParser()
            };
        }

        /// <summary>
        /// Gets the parsers for items.
        /// </summary>
        public OrderedList<ListItemParser> ItemParsers { get; }

        public override void Initialize()
        {
            var tempMap = new Dictionary<char, ListItemParser>();

            foreach (var itemParser in ItemParsers)
            {
                if (itemParser.OpeningCharacters == null)
                {
                    throw new InvalidOperationException($"The list item parser of type [{itemParser.GetType()}] cannot have OpeningCharacters to null. It must define a list of valid opening characters");
                }

                foreach (var openingCharacter in itemParser.OpeningCharacters)
                {
                    if (tempMap.ContainsKey(openingCharacter))
                    {
                        throw new InvalidOperationException(
                            $"A list item parser with the same opening character `{openingCharacter}` is already registered");
                    }
                    tempMap.Add(openingCharacter, itemParser);
                }
            }
            mapItemParsers = new CharacterMap<ListItemParser>(tempMap);
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            // When both a thematic break and a list item are possible
            // interpretations of a line, the thematic break takes precedence
            var thematicParser = ThematicBreakParser.Default;
            if (thematicParser.HasOpeningCharacter(processor.CurrentChar))
            {
                var result = thematicParser.TryOpen(processor);
                if (result.IsBreak())
                {
                    return result;
                }
            }

            return TryParseListItem(processor, null);
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (block is ListBlock && processor.NextContinue is ListItemBlock)
            {
                // We try to match only on item block if the ListBlock
                return BlockState.Skip;
            }

            // When both a thematic break and a list item are possible
            // interpretations of a line, the thematic break takes precedence
            BlockState result;
            var thematicParser = ThematicBreakParser.Default;
            if (thematicParser.HasOpeningCharacter(processor.CurrentChar))
            {
                result = thematicParser.TryOpen(processor);
                if (result.IsBreak())
                {
                    // TODO: We remove the thematic break, as it will be created later, but this is inefficient, try to find another way
                    processor.NewBlocks.Pop();
                    return BlockState.None;
                }
            }

            // 5.2 List items 
            // TODO: Check with specs, it is not clear that list marker or bullet marker must be followed by at least 1 space

            // If we have already a ListItemBlock, we are going to try to append to it
            var listItem = block as ListItemBlock;
            result = BlockState.None;
            if (listItem != null)
            {
                result = TryContinueListItem(processor, listItem);
            }

            if (result == BlockState.None)
            {
                result = TryParseListItem(processor, block);
            }

            return result;
        }

        private BlockState TryContinueListItem(BlockProcessor state, ListItemBlock listItem)
        {
            var list = (ListBlock)listItem.Parent;

            // Allow all blanks lines if the last block is a fenced code block
            // Allow 1 blank line inside a list
            // If > 1 blank line, terminate this list
            var isBlankLine = state.IsBlankLine;

            var isCurrentBlockBreakable = state.CurrentBlock != null && state.CurrentBlock.IsBreakable;
            if (isBlankLine)
            {
                if (isCurrentBlockBreakable)
                {
                    if (!(state.NextContinue is ListBlock))
                    {
                        list.CountAllBlankLines++;
                        listItem.Add(new BlankLineBlock());
                    }
                    list.CountBlankLinesReset++;
                }

                if (list.CountBlankLinesReset == 1 && listItem.ColumnWidth < 0)
                {
                    state.Close(listItem);

                    // Leave the list open
                    list.IsOpen = true;
                    return BlockState.Continue;
                }

                // Update list-item source end position
                listItem.UpdateSpanEnd(state.Line.End);
                
                return BlockState.Continue;
            }

            list.CountBlankLinesReset = 0;

            int columWidth = listItem.ColumnWidth;
            if (columWidth < 0)
            {
                columWidth = -columWidth;
            }

            if (state.Indent >= columWidth)
            {
                if (state.Indent > columWidth && state.IsCodeIndent)
                {
                    state.GoToColumn(state.ColumnBeforeIndent + columWidth);
                }

                // Update list-item source end position
                listItem.UpdateSpanEnd(state.Line.End);

                return BlockState.Continue;
            }

            return BlockState.None;
        }

        private BlockState TryParseListItem(BlockProcessor state, Block block)
        {
            // If we have a code indent and we are not in a ListItem, early exit
            if (!(block is ListItemBlock) && state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var currentListItem = block as ListItemBlock;
            var currentParent = block as ListBlock ?? (ListBlock)currentListItem?.Parent;

            var initColumnBeforeIndent = state.ColumnBeforeIndent;
            var initColumn = state.Column;
            var sourcePosition = state.Start;
            var sourceEndPosition = state.Line.End;

            var c = state.CurrentChar;
            var itemParser = mapItemParsers[c];
            bool isOrdered = itemParser is OrderedListItemParser;
            if (itemParser == null)
            {
                return BlockState.None;
            }

            // Try to parse the list item
            ListInfo listInfo;
            if (!itemParser.TryParse(state, currentParent?.BulletType ?? '\0', out listInfo))
            {
                // Reset to an a start position
                state.GoToColumn(initColumn);
                return BlockState.None;
            }

            // Gets the current character after a succesfull parsing of the list information
            c = state.CurrentChar;

            // Item starting with a blank line
            int columnWidth;

            // Do we have a blank line right after the bullet?
            if (c == '\0')
            {
                // Use a negative number to store the number of expected chars
                columnWidth = -(state.Column - initColumnBeforeIndent + 1);
            }
            else
            {
                if (!c.IsSpaceOrTab())
                {
                    state.GoToColumn(initColumn);
                    return BlockState.None;
                }

                // We require at least one char
                state.NextColumn();

                // Parse the following indent
                state.RestartIndent();
                var columnBeforeIndent = state.Column;
                state.ParseIndent();

                if (state.IsCodeIndent)
                {
                    state.GoToColumn(columnBeforeIndent);
                }

                // Number of spaces required for the following content to be part of this list item
                // If the list item starts with a blank line, the number of spaces
                // following the list marker doesn't change the required indentation
                columnWidth = (state.IsBlankLine ? columnBeforeIndent : state.Column) - initColumnBeforeIndent;
            }

            // Starts/continue the list unless:
            // - an empty list item follows a paragraph
            // - an ordered list is not starting by '1'
            var isPreviousParagraph = (block ?? state.LastBlock) is ParagraphBlock;
            if (isPreviousParagraph)
            {
                var isOpen = state.IsOpen(block ?? state.LastBlock);
                if (state.IsBlankLine || (isOpen && listInfo.BulletType == '1' && listInfo.OrderedStart != "1"))
                {
                    state.GoToColumn(initColumn);
                    return BlockState.None;
                }
            }

            var newListItem = new ListItemBlock(this)
            {
                Column = initColumn,
                ColumnWidth = columnWidth,
                Span = new SourceSpan(sourcePosition, sourceEndPosition)
            };
            state.NewBlocks.Push(newListItem);

            if (currentParent != null)
            {
                // If we have a new list item, close the previous one
                if (currentListItem != null)
                {
                    state.Close(currentListItem);
                }

                // Reset the list if it is a new list or a new type of bullet
                if (currentParent.IsOrdered != isOrdered ||
                    currentParent.OrderedDelimiter != listInfo.OrderedDelimiter ||
                    currentParent.BulletType != listInfo.BulletType)
                {
                    state.Close(currentParent);
                    currentParent = null;
                }
            }

            if (currentParent == null)
            {
                var newList = new ListBlock(this)
                {
                    Column = initColumn,
                    Span = new SourceSpan(sourcePosition, sourceEndPosition),
                    IsOrdered = isOrdered,
                    BulletType = listInfo.BulletType,
                    OrderedDelimiter = listInfo.OrderedDelimiter,
                    DefaultOrderedStart = listInfo.DefaultOrderedStart,
                    OrderedStart = listInfo.OrderedStart,
                };
                state.NewBlocks.Push(newList);
            }

            return BlockState.Continue;
        }

        public override bool Close(BlockProcessor processor, Block blockToClose)
        {
            var listBlock = blockToClose as ListBlock;

            // Process only if we have blank lines
            if (listBlock == null || listBlock.CountAllBlankLines <= 0)
            {
                return true;
            }

            // TODO: This code is UGLY and WAY TOO LONG, simplify!
            bool isLastListItem = true;
            for (int listIndex = listBlock.Count - 1; listIndex >= 0; listIndex--)
            {
                var block = listBlock[listIndex];
                var listItem = (ListItemBlock) block;
                bool isLastElement = true;
                for (int i = listItem.Count - 1; i >= 0; i--)
                {
                    var item = listItem[i];
                    if (item is BlankLineBlock)
                    {
                        if ((isLastElement &&  listIndex < listBlock.Count - 1) || (listItem.Count > 2 && (i > 0 && i < (listItem.Count - 1))))
                        {
                            listBlock.IsLoose = true;
                        }

                        if (isLastElement && isLastListItem)
                        {
                            // Inform the outer list that we have a blank line
                            var parentListItemBlock = listBlock.Parent as ListItemBlock;
                            if (parentListItemBlock != null)
                            {
                                var parentList = (ListBlock) parentListItemBlock.Parent;

                                parentList.CountAllBlankLines++;
                                parentListItemBlock.Add(new BlankLineBlock());
                            }
                        }

                        listItem.RemoveAt(i);

                        // If we have remove all blank lines, we can exit
                        listBlock.CountAllBlankLines--;
                        if (listBlock.CountAllBlankLines == 0)
                        {
                            break;
                        }
                    }
                    isLastElement = false;
                }
                isLastListItem = false;
            }

            //// Update end-position for the list
            //if (listBlock.Count > 0)
            //{
            //    listBlock.Span.End = listBlock[listBlock.Count - 1].Span.End;
            //}

            return true;
        }
    }
}