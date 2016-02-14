using System.Collections.Generic;
using System.Diagnostics;

namespace Textamina.Markdig.Syntax
{
    [DebuggerDisplay("Container: {GetType().Name} Count = {Children.Count}")]
    public abstract class ContainerBlock : Block
    {
        protected ContainerBlock()
        {
            Children = new List<Block>();
        }

        public List<Block> Children { get; }
    }
}