using System.Collections.Generic;
using System.Diagnostics;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("Container: {GetType().Name} Count = {Children.Count}")]
    public abstract class BlockContainer : Block
    {
        protected BlockContainer()
        {
            Children = new List<Block>();
        }

        public List<Block> Children { get; }
    }
}