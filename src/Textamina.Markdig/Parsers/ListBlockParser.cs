using System;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsers
{
    public class ListBlockParser : BlockParser
    {
        public ListBlockParser()
        {
            OpeningCharacters = new[] {'-', '+', '*', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            OrderedDelimiter = new[] {'.', ')'};
        }

        public char[] OrderedDelimiter { get; set; }

        public override BlockState TryOpen(BlockParserState state)
        {
            // When both a thematic break and a list item are possible
            // interpretations of a line, the thematic break takes precedence
            var thematicParser = ThematicBreakParser.Default;
            if (thematicParser.HasOpeningCharacter(state.CurrentChar))
            {
                var result = thematicParser.TryOpen(state);
                if (result.IsBreak())
                {
                    return result;
                }
            }

            return TryParseListItem(state, null);
        }

        public override BlockState TryContinue(BlockParserState state, Block block)
        {
            if (block is ListBlock && state.NextContinue is ListItemBlock)
            {
                // We try to match only on item block if the ListBlock
                return BlockState.Skip;
            }

            // When both a thematic break and a list item are possible
            // interpretations of a line, the thematic break takes precedence
            BlockState result;
            var thematicParser = ThematicBreakParser.Default;
            if (thematicParser.HasOpeningCharacter(state.CurrentChar))
            {
                result = thematicParser.TryOpen(state);
                if (result.IsBreak())
                {
                    // TODO: We remove the thematic break, as it will be created later, but this is inefficient, try to find another way
                    state.NewBlocks.Pop();
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
                result = TryContinueListItem(state, listItem);
            }

            if (result == BlockState.None)
            {
                result = TryParseListItem(state, block);
            }

            return result;
        }

        private BlockState TryContinueListItem(BlockParserState state, ListItemBlock listItem)
        {
            var list = (ListBlock)listItem.Parent;

            // Allow all blanks lines if the last block is a fenced code block
            // Allow 1 blank line inside a list
            // If > 1 blank line, terminate this list
            var isBlankLine = state.IsBlankLine;

            //if (isBlankLine && !(state.LastBlock is FencedCodeBlock)) // TODO: Handle this case
            var isInFencedBlock = state.LastBlock is FencedCodeBlock;
            if (isBlankLine)
            {
                // TODO: Check with a generic way (allow a block to have multiple empty lines)
                if (!isInFencedBlock)
                {
                    if (!(state.NextContinue is ListBlock))
                    {
                        list.CountAllBlankLines++;
                        listItem.Children.Add(BlankLineBlock.Instance);
                    }
                    list.CountBlankLinesReset++;
                }

                if (list.CountBlankLinesReset > 1)
                {
                    // TODO: Close all lists and not only this one
                    return BlockState.BreakDiscard;
                }

                if (list.CountBlankLinesReset == 1 && listItem.ColumnWidth < 0)
                {
                    state.Close(listItem);

                    // Leave the list open
                    list.IsOpen = true;
                    return BlockState.Continue;
                }

                return BlockState.Continue;
            }

            list.CountBlankLinesReset = 0;

            int columWidth = listItem.ColumnWidth;
            if (columWidth < 0)
            {
                columWidth = -columWidth;
            }

            // TODO: Handle code indent
            if (state.Indent >= columWidth)
            {
                if (state.Indent > columWidth && state.IsCodeIndent)
                {
                    state.ResetToColumn(columWidth);
                }

                return BlockState.Continue;
            }

            return BlockState.None;
        }

        private BlockState TryParseListItem(BlockParserState state, Block block)
        {
            var initStart = state.Start;
            var initColumnBeforeIndent = state.ColumnBeforeIndent;
            var initColumn = state.Column;

            // If we have a code indent and we are not in a ListItem, early exit
            if (!(block is ListItemBlock) && state.IsCodeIndent)
            {
                return BlockState.None;
            }

            var isOrdered = false;
            var bulletChar = (char) 0;
            int orderedStart = 0;
            var orderedDelimiter = (char) 0;

            var c = state.CurrentChar;
            if (c.IsDigit())
            {
                int countDigit = 0;
                while (c.IsDigit())
                {
                    orderedStart = orderedStart*10 + c - '0';
                    c = state.NextChar();
                    countDigit++;
                }

                // Note that ordered list start numbers must be nine digits or less:
                if (countDigit > 9)
                {
                    // Reset to an a start position
                    state.ResetToPosition(initStart);
                    return BlockState.None;
                }

                // Check if we have an ordered delimiter
                bool isOrderedDelimiter = false;
                for (int i = 0; i < OrderedDelimiter.Length; i++)
                {
                    if (OrderedDelimiter[i] == c)
                    {
                        isOrderedDelimiter = true;
                        break;
                    }
                }
                if (!isOrderedDelimiter)
                {
                    // Reset to an a start position
                    state.ResetToPosition(initStart);
                    return BlockState.None;
                }

                isOrdered = true;
                orderedDelimiter = c;
            }
            else if (OpeningCharacters.Contains(c))
            {
                // Else we have a bullet char
                bulletChar = c;
            }
            else
            {
                // Reset to an a start position
                state.ResetToPosition(initStart);
                return BlockState.None;
            }

            // Skip Bullet or '.' or ')'
            c = state.NextChar();

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
                    state.ResetToPosition(initStart);
                    return BlockState.None;
                }

                // We require at least one char
                state.NextChar();

                // Parse the following indent
                state.RestartIndent();
                var positionBeforeIndent = state.Start;
                state.ParseIndent();

                if (state.IsCodeIndent)
                {
                    state.ResetToPosition(positionBeforeIndent);
                }

                // Number of spaces required for the following content to be part of this list item
                columnWidth = state.Column - initColumnBeforeIndent;
            }

            var newListItem = new ListItemBlock(this)
            {
                Column = initColumn,
                ColumnWidth = columnWidth
            };
            state.NewBlocks.Push(newListItem);

            var currentListItem = block as ListItemBlock;
            var currentParent = block as ListBlock ?? (ListBlock)currentListItem?.Parent;

            if (currentParent != null)
            {
                // If we have a new list item, close the previous one
                if (currentListItem != null)
                {
                    state.Close(currentListItem);
                }

                // Reset the list if it is a new list or a new type of bullet
                if (currentParent.IsOrdered != isOrdered ||
                    (isOrdered && currentParent.OrderedDelimiter != orderedDelimiter) ||
                    (!isOrdered && currentParent.BulletChar != bulletChar)
                    //(numberOfSpaces < ((ListItemBlock) currentParent.LastChild).ColumnWidth)
                    )
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
                    IsOrdered = isOrdered,
                    BulletChar = bulletChar,
                    OrderedDelimiter = orderedDelimiter,
                    OrderedStart = orderedStart,
                };
                state.NewBlocks.Push(newList);
            }

            return BlockState.Continue;
        }

        public override bool Close(BlockParserState state, Block blockToClose)
        {
            var listBlock = blockToClose as ListBlock;

            // Process only if we have blank lines
            if (listBlock == null || listBlock.CountAllBlankLines <= 0)
            {
                return true;
            }

            // TODO: This code is UGLY and WAY TOO LONG, simplify!
            bool isLastListItem = true;
            for (int listIndex = listBlock.Children.Count - 1; listIndex >= 0; listIndex--)
            {
                var block = listBlock.Children[listIndex];
                var listItem = (ListItemBlock) block;
                var children = listItem.Children;
                bool isLastElement = true;
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    var item = children[i];
                    if (item is BlankLineBlock)
                    {
                        if ((isLastElement &&  listIndex < listBlock.Children.Count - 1) || (children.Count > 2 && (i > 0 && i < (children.Count - 1))))
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
                                parentListItemBlock.Children.Add(BlankLineBlock.Instance);
                            }
                        }

                        children.RemoveAt(i);

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

            return true;
        }
    }
}