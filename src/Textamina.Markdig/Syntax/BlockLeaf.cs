using System.Collections.Generic;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class BlockLeaf : Block
    {
        protected BlockLeaf(BlockMatcher matcher) : base(matcher)
        {
        }

        protected List<StringLiner> lines;

        internal void Append(StringLiner line)
        {
            if (lines == null)
            {
                lines = new List<StringLiner>();
            }
            lines.Add(line);
        }
    }
}