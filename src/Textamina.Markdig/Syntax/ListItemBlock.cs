

namespace Textamina.Markdig.Syntax
{
    public class ListItemBlock : ContainerBlock
    {
        internal int NumberOfSpaces { get; set; }

        internal bool IsFollowedByBlankLine;
    }
}