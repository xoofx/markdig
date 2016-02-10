using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class Block
    {
        protected Block(BlockMatcher matcher)
        {
            Matcher = matcher;
        }

        public BlockMatcher Matcher { get; }

        public Block Parent { get; internal set;  }
    }
}