


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

        private bool HasBlankLines { get; set; }

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(BlockParserState state)
            {
                var liner = state.Line;

                // 5.2 List items 
                // TODO: Check with specs, it is not clear that list marker or bullet marker must be followed by at least 1 space

                int preIndent = 0;
                for (int i = liner.Start - 1; i >= 0; i--)
                {
                    if (liner[i].IsSpaceOrTab())
                    {
                        preIndent++;
                    }
                    else
                    {
                        break;
                    }
                }

                var saveLiner = liner.Save();

                // If we have already a ListItemBlock, we are going to try to append to it
                var listItem = state.Pending as ListItemBlock;
                if (listItem != null)
                {
                    var list = (ListBlock) listItem.Parent;

                    // Allow all blanks lines if the last block is a fenced code block
                    // Allow 1 blank line inside a list
                    // If > 1 blank line, terminate this list
                    var isBlankLine = liner.IsBlankLine();
                    //if (isBlankLine && !(state.LastBlock is FencedCodeBlock)) // TODO: Handle this case
                    if (isBlankLine)
                    {
                        // TODO: Check with a generic way (allow a block to have multiple empty lines)
                        if (!(state.LastBlock is FencedCodeBlock))
                        {
                            list.HasBlankLines = true;
                            listItem.Children.Add(BlankLineBlock.Instance);
                        }

                        if (listItem.Children.Count > 1)
                        {
                            // TODO: Close all lists and not only this one
                            return MatchLineResult.LastDiscard;
                        }

                        return MatchLineResult.Continue;
                    }

                    var c = liner.Current;
                    var startPosition = liner.Column;

                    // List Item starting with a blank line (-1)
                    if (listItem.NumberOfSpaces < 0)
                    {
                        int expectedCount = -listItem.NumberOfSpaces;
                        int countSpaces = 0;
                        var saved = new StringLine.State();
                        while (c.IsSpaceOrTab())
                        {
                            c = liner.NextChar();
                            countSpaces = preIndent + liner.Column - startPosition;
                            if (countSpaces == expectedCount)
                            {
                                saved = liner.Save();
                            }
                            else if (countSpaces >= 4)
                            {
                                liner.Restore(ref saved);
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
                            c = liner.NextChar();
                            var countSpaces = preIndent + liner.Column - startPosition;
                            if (countSpaces >= listItem.NumberOfSpaces)
                            {
                                return MatchLineResult.Continue;
                            }
                        }
                    }
                    liner.Restore(ref saveLiner);
                }

                return TryParseListItem(ref state, preIndent);
            }

            private MatchLineResult TryParseListItem(ref BlockParserState state, int preIndent)
            {
                var liner = state.Line;

                var isInList = state.Pending is ListItemBlock;

                var preStartPosition = liner.Start;

                var c = liner.Current;
                if (isInList)
                {
                    while (c.IsSpaceOrTab())
                    {
                        c = liner.NextChar();
                    }
                }
                else
                {
                    liner.SkipLeadingSpaces3();
                }
                preIndent = preIndent + liner.Start - preStartPosition;

                var isOrdered = false;
                var bulletChar = (char) 0;
                int orderedStart = 0;
                var orderedDelimiter = (char) 0;

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
                        c = liner.NextChar();
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
                liner.NextChar();

                // Item starting with a blank line
                int numberOfSpaces;
                if (liner.IsBlankLine())
                {
                    // Use a negative number to store the number of expected chars
                    numberOfSpaces = -(preIndent + 1);
                }
                else
                {
                    var startPosition = -1;
                    int countSpaceAfterBullet = 0;
                    var saved = new StringLine.State();
                    for (int i = 0; i <= 4; i++)
                    {
                        c = liner.Current;
                        if (!c.IsSpaceOrTab())
                        {
                            break;
                        }
                        if (i == 0)
                        {
                            startPosition = liner.Column;
                        }

                        var endPosition = liner.Column;
                        countSpaceAfterBullet = endPosition - startPosition;

                        if (countSpaceAfterBullet == 1)
                        {
                            saved = liner.Save();
                        }
                        else if (countSpaceAfterBullet >= 4)
                        {
                            liner.SpaceHeaderCount = countSpaceAfterBullet - 4;
                            countSpaceAfterBullet = 0;
                            liner.Restore(ref saved);
                            break;
                        }
                        liner.NextChar();
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
                    NumberOfSpaces = numberOfSpaces
                };
                state.NewBlocks.Push(newListItem);

                var currentListItem = state.Pending as ListItemBlock;
                var currentParent = state.Pending as ListBlock ?? (ListBlock)currentListItem?.Parent;

                while (currentParent != null)
                {
                    // If we have a new list item, close the previous one
                    if (currentListItem != null)
                    {
                        state.Close(currentListItem);
                        currentListItem = null;
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
                    else
                    {
                        break;
                    }

                    for (int i = state.Count - 1; i >= 1; i--)
                    {
                        var block = state[i];
                        currentListItem = block as ListItemBlock;
                        if (currentListItem != null)
                        {
                            currentParent = (ListBlock) currentListItem.Parent;
                            break;
                        }
                        else if (block is ListBlock)
                        {
                            currentParent = (ListBlock) block;
                            break;
                        }
                    }
                }

                if (currentParent == null)
                {
                    var newList = new ListBlock(this)
                    {
                        IsOrdered = isOrdered,
                        BulletChar = bulletChar,
                        OrderedDelimiter = orderedDelimiter,
                        OrderedStart = orderedStart,
                    };
                    state.NewBlocks.Push(newList);
                }

                // Make sure that we don't have any pending, as we are replacing the previous one
                state.Pending = null;

                //// A list item can begin with at most one blank line
                //if (numberOfSpaces < 0)
                //{
                //    newListItem.Children.Add(BlankLineBlock.Instance);
                //}

                return MatchLineResult.Continue;
            }

            public override void Close(BlockParserState state)
            {
                var listBlock = state.Pending as ListBlock;
                // Process only if we have blank lines
                if (listBlock != null && listBlock.HasBlankLines)
                {
                    for (int listIndex = listBlock.Children.Count - 1; listIndex >= 0; listIndex--)
                    {
                        var block = listBlock.Children[listIndex];
                        var listItem = (ListItemBlock) block;
                        var children = listItem.Children;
                        for (int i = children.Count - 1; i >= 0; i--)
                        {
                            var item = children[i];
                            if (item is BlankLineBlock)
                            {
                                if ((i == children.Count - 1 &&  listIndex < listBlock.Children.Count - 1) || (children.Count == 3 && i == 1))
                                {
                                    listBlock.IsLoose = true;
                                }
                                children.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
    }
}