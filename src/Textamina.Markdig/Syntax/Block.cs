using System;
using Textamina.Markdig.Parsers;

namespace Textamina.Markdig.Syntax
{
    public abstract class Block : MarkdownObject
    {
        protected Block(BlockParser parser)
        {
            Parser = parser;
        }

        public int Column { get; set; }

        public int Line { get; set; }

        public ContainerBlock Parent { get; internal set;  }

        public BlockParser Parser { get; }

        public bool IsOpen { get; set; }

        public bool RemoveAfterProcessInlines { get; set; }

        public event Action<InlineParserState> ProcessInlinesBegin;

        public event Action<InlineParserState> ProcessInlinesEnd;

        internal void OnProcessInlinesBegin(InlineParserState obj)
        {
            ProcessInlinesBegin?.Invoke(obj);
        }
        internal void OnProcessInlinesEnd(InlineParserState obj)
        {
            ProcessInlinesEnd?.Invoke(obj);
        }
    }
}