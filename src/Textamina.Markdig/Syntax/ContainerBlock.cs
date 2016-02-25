using System.Collections.Generic;
using System.Diagnostics;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("{GetType().Name} Count = {Children.Count}")]
    public abstract class ContainerBlock : Block
    {
        protected ContainerBlock(BlockParser parser) : base(parser)
        {
            Children = new List<Block>();
        }

        // TODO: Remove Children and use only inner list
        public List<Block> Children { get; }

        public Block LastChild => Children.Count > 0 ? Children[Children.Count - 1] : null;
    }
}