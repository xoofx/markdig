using System;
using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    public abstract class Block
    {
        protected Block(BlockParser parser)
        {
            Parser = parser;
        }

        public Block Parent { get; internal set;  }

        public BlockParser Parser { get; }

        public bool IsOpen { get; set; }
    }
}