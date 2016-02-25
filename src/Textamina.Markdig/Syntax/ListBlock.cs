


using Textamina.Markdig.Helpers;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class ListBlock : ContainerBlock
    {
        public new static readonly BlockParser Parser = new ParserInternal();

        public ListBlock(BlockParser parser) : base(parser)
        {
        }

        public bool IsOrdered { get; set; }

        public char BulletChar { get; set; }

        public int OrderedStart { get; set; }

        public char OrderedDelimiter { get; set; }

        public bool IsLoose { get; set; }

        private int CountAllBlankLines { get; set; }

        private int CountBlankLinesReset { get; set; }

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                if (state.Pending is ListBlock && state.NextPending is ListItemBlock)
                {
                    // We try to match only on item block if the ListBlock
                    return MatchLineResult.Skip;
                }

                // When both a thematic break and a list item are possible
                // interpretations of a line, the thematic break takes precedence
                var save = state.Line;
                if (ThematicBreakBlock.Parser.Match(state) == MatchLineResult.Last)
                {
                    // Remove the ThematicBreakBlock as we will let the ThematicBreakBlock to catch it later
                    state.NewBlocks.Pop();
                    return MatchLineResult.None;
                }
                state.SetCurrentLine(ref save);

                // 5.2 List items 
                // TODO: Check with specs, it is not clear that list marker or bullet marker must be followed by at least 1 space

                int preIndent = 0;
                for (int i = state.Line.Start - 1; i >= 0; i--)
                {
                    if (state.Line[i].IsSpaceOrTab())
                    {
                        preIndent++;
                    }
                    else
                    {
                        break;
                    }
                }

                var saveLiner = state.Line;

                // If we have already a ListItemBlock, we are going to try to append to it
                var listItem = state.Pending as ListItemBlock;
                if (listItem != null)
                {
                    var list = (ListBlock) listItem.Parent;

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
                            if (!(state.NextPending is ListBlock))
                            {
                                list.CountAllBlankLines++;
                                listItem.Children.Add(BlankLineBlock.Instance);
                            }
                            list.CountBlankLinesReset++;
                        }

                        if (list.CountBlankLinesReset > 1)
                        {
                            // TODO: Close all lists and not only this one
                            return MatchLineResult.LastDiscard;
                        }

                        if (list.CountBlankLinesReset == 1 && listItem.NumberOfSpaces < 0)
                        {
                            state.Close(listItem);

                            // Leave the list open
                            list.IsOpen = true;
                            return MatchLineResult.Continue;
                        }

                        return MatchLineResult.Continue;
                    }

                    list.CountBlankLinesReset = 0;

                    var c = state.Line.CurrentChar;
                    var startPosition = state.Line.Start;

                    // List Item starting with a blank line (-1)
                    if (listItem.NumberOfSpaces < 0)
                    {
                        int expectedCount = -listItem.NumberOfSpaces;
                        int countSpaces = 0;
                        var saved = new StringSlice();
                        while (c.IsSpaceOrTab())
                        {
                            c = state.Line.NextChar();
                            countSpaces = preIndent + state.Line.Column - startPosition;
                            if (countSpaces == expectedCount)
                            {
                                saved = state.Line;
                            }
                            else if (countSpaces >= 4)
                            {
                                state.SetCurrentLine(ref saved);
                                countSpaces = expectedCount;
                                break;
                            }
                        }

                        if (countSpaces == expectedCount)
                        {
                            listItem.NumberOfSpaces = countSpaces;
                            return MatchLineResult.Continue;
                        }
                    }
                    else
                    {
                        while (c.IsSpaceOrTab())
                        {
                            c = state.Line.NextChar();
                            var countSpaces = preIndent + state.Line.Column - startPosition;
                            if (countSpaces >= listItem.NumberOfSpaces)
                            {
                                return MatchLineResult.Continue;
                            }
                        }
                    }
                    state.SetCurrentLine(ref saveLiner);
                }

                return TryParseListItem(ref state, preIndent);
            }

            private MatchLineResult TryParseListItem(ref BlockParserState state, int preIndent)
            {
                var isInList = state.Pending is ListItemBlock;

                var preStartPosition = state.Line.Start;

                var c = state.Line.CurrentChar;
                if (isInList)
                {
                    while (c.IsSpaceOrTab())
                    {
                        c = state.Line.NextChar();
                    }
                }
                else
                {
                    // TODO
                    //state.Line.SkipLeadingSpaces3();
                    c = state.Line.CurrentChar;
                }
                preIndent = preIndent + state.Line.Start - preStartPosition;

                var isOrdered = false;
                var bulletChar = (char) 0;
                int orderedStart = 0;
                var orderedDelimiter = (char) 0;

                var column = state.Line.Start;

                if (c.IsBulletListMarker())
                {
                    bulletChar = c;
                    preIndent++;
                }
                else if (c.IsDigit())
                {
                    int countDigit = 0;
                    while (c.IsDigit())
                    {
                        orderedStart = orderedStart*10 + c - '0';
                        c = state.Line.NextChar();
                        preIndent++;
                        countDigit++;
                    }

                    // Note that ordered list start numbers must be nine digits or less:
                    if (countDigit > 9)
                    {
                        return MatchLineResult.None;
                    }

                    // We don't have an ordered list
                    if (c != '.' && c != ')')
                    {
                        return MatchLineResult.None;
                    }
                    preIndent++;
                    isOrdered = true;
                    orderedDelimiter = c;
                }
                else
                {
                    return MatchLineResult.None;
                }

                // Skip Bullet or '.'
                state.Line.NextChar();

                // Item starting with a blank line
                int numberOfSpaces;
                if (state.IsBlankLine)
                {
                    // Use a negative number to store the number of expected chars
                    numberOfSpaces = -(preIndent + 1);
                }
                else
                {
                    var startPosition = -1;
                    int countSpaceAfterBullet = 0;
                    var saved = new StringSlice();
                    for (int i = 0; i <= 4; i++)
                    {
                        c = state.Line.CurrentChar;
                        if (!c.IsSpaceOrTab())
                        {
                            break;
                        }
                        if (i == 0)
                        {
                            startPosition = state.Line.Column;
                        }

                        var endPosition = state.Line.Column;
                        countSpaceAfterBullet = endPosition - startPosition;

                        if (countSpaceAfterBullet == 1)
                        {
                            saved = state.Line;
                        }
                        else if (countSpaceAfterBullet >= 4)
                        {
                            //state.Line.SpaceHeaderCount = countSpaceAfterBullet - 4;
                            countSpaceAfterBullet = 0;
                            state.SetCurrentLine(ref saved);
                            break;
                        }
                        state.Line.NextChar();
                    }

                    // If we haven't matched any spaces, early exit
                    if (startPosition < 0)
                    {
                        return MatchLineResult.None;
                    }
                    // Number of spaces required for the following content to be part of this list item
                    numberOfSpaces = preIndent + countSpaceAfterBullet + 1;
                }

                var newListItem = new ListItemBlock(this)
                {
                    Column = column,
                    NumberOfSpaces = numberOfSpaces
                };
                state.NewBlocks.Push(newListItem);

                var currentListItem = state.Pending as ListItemBlock;
                var currentParent = state.Pending as ListBlock ?? (ListBlock)currentListItem?.Parent;

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
                        //(numberOfSpaces < ((ListItemBlock) currentParent.LastChild).NumberOfSpaces)
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
                        Column = column,
                        IsOrdered = isOrdered,
                        BulletChar = bulletChar,
                        OrderedDelimiter = orderedDelimiter,
                        OrderedStart = orderedStart,
                    };
                    state.NewBlocks.Push(newList);
                }

                return MatchLineResult.Continue;
            }

            public override void Close(BlockParserState state)
            {
                var listBlock = state.Pending as ListBlock;

                // Process only if we have blank lines
                if (listBlock != null && listBlock.CountAllBlankLines > 0)
                {
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
                                    return;
                                }
                            }
                            isLastElement = false;
                        }
                        isLastListItem = false;
                    }
                }
            }
        }
    }
}