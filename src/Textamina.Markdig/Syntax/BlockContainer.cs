using System.Collections.Generic;
using System.Diagnostics;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("Count = {Children.Count}")]
    public abstract class BlockContainer : Block
    {

        protected BlockContainer(BlockMatcher matcher) : base(matcher)
        {
            Children = new List<Block>();
        }

        public List<Block> Children { get; }
    }
}