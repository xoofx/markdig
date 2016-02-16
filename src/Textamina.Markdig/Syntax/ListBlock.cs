

using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class ListBlock : ContainerBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public char BulletChar { get; set; }

        private int consecutiveBlankLines;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref MatchLineState state)
            {
                var liner = state.Liner;

                // 5.2 List items 
                // TODO: Check with specs, it is not clear that list marker or bullet marker must be followed by at least 1 space

                var saveLiner = liner.Save();

                // If we have already a ListItemBlock, we are going to try to append to it
                if (state.Block != null)
                {
                    var listItem = (ListItemBlock) state.Block;
                    var list = (ListBlock) listItem.Parent;

                    // Allow all blanks lines if the last block is a fenced code block
                    // Allow 1 blank line inside a list
                    // If > 1 blank line, terminate this list
                    var isBlankLine = liner.IsBlankLine();
                    if (isBlankLine && !(state.LastBlock is FencedCodeBlock))
                    {
                        list.consecutiveBlankLines++;
                        if (list.consecutiveBlankLines > 1)
                        {
                            return MatchLineResult.LastDiscard;
                        }

                        return MatchLineResult.Continue;
                    }

                    list.consecutiveBlankLines = 0;

                    var c = liner.Current;
                    var startPosition = liner.Column;
                    while (Utility.IsSpaceOrTab(c))
                    {
                        c = liner.NextChar();
                        var endPosition = liner.Column;
                        if ((endPosition - startPosition) >= listItem.NumberOfSpaces)
                        {
                            return MatchLineResult.Continue;
                        }
                    }
                    liner.Restore(ref saveLiner);
                }

                return TryParseListItem(ref state);
            }

            private MatchLineResult TryParseListItem(ref MatchLineState state)
            {
                var liner = state.Liner;

                var preStartPosition = liner.Start;
                liner.SkipLeadingSpaces3();
                var preIndent = liner.Start - preStartPosition;
                var c = liner.Current;
                if (Utility.IsBulletListMarker(c))
                {
                    var listType = c;

                    var startPosition = -1;
                    var endPosition = -1;
                    var tabToSpacesCount = 0;
                    int deltaPositions = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        c = liner.NextChar();
                        if (!Utility.IsSpaceOrTab(c))
                        {
                            break;
                        }
                        if (i == 0)
                        {
                            startPosition = liner.Column;
                        }
                        endPosition = liner.Column;

                        deltaPositions = endPosition - startPosition;
                        if (deltaPositions >= 4)
                        {
                            liner.SpaceHeaderCount = deltaPositions - 4;
                            deltaPositions = 4;
                            break;
                        }
                    }

                    // If we haven't matched any spaces, early exit
                    if (startPosition < 0)
                    {
                        return MatchLineResult.None;
                    }

                    // Number of spaces required for the following content to be part of this list item
                    var numberOfSpaces = deltaPositions + 1 + preIndent;

                    var listItem = new ListItemBlock() {NumberOfSpaces = numberOfSpaces};
                    var parentList = (state.Block as ListItemBlock)?.Parent as ListBlock;

                    // Reset the list if it is a new list or a new type of bullet
                    if (parentList == null || parentList.BulletChar != listType)
                    {
                        parentList = new ListBlock() { BulletChar = listType };
                    }
                    parentList.Children.Add(listItem);
                    listItem.Parent = parentList;

                    state.Block = listItem;

                    return MatchLineResult.Continue;
                }
                return MatchLineResult.None;
            }
        }
    }
}