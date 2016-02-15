

using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public class ListBlock : ContainerBlock
    {
        public static readonly BlockParser Parser = new ParserInternal();

        public char BulletChar { get; set; }

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

                    var c = liner.Current;
                    var startPosition = liner.Column;
                    while (Utility.IsSpaceOrTab(c))
                    {
                        var endPosition = liner.Column;
                        c = liner.NextChar();
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

                    c = liner.NextChar();

                    var startPosition = liner.Column;

                    // A space or tab is required after a list item
                    if (!Utility.IsSpaceOrTab(c))
                    {
                        return MatchLineResult.None;
                    }

                    var endPosition = startPosition;
                    for (int i = 0; i < 5; i++)
                    {
                        c = liner.NextChar();
                        if (!Utility.IsSpaceOrTab(c))
                        {
                            break;
                        }
                        endPosition = liner.Column;
                    }

                    // Number of spaces required for the following content to be part of this list item
                    var numberOfSpaces = endPosition - startPosition + 1 + preIndent;

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