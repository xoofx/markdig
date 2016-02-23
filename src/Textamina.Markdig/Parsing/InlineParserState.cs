using System.Collections.Generic;
using System.Text;
using Textamina.Markdig.Helpers;
using Textamina.Markdig.Syntax;

namespace Textamina.Markdig.Parsing
{
    public class InlineParserState
    {
        public InlineParserState(StringBuilderCache stringBuilders, Document document)
        {
            StringBuilders = stringBuilders;
            Document = document;
            InlinesToClose = new List<Inline>();
        }

        public LeafBlock Block { get; internal set; }

        public StringLineGroup Lines;

        public Inline Inline { get; set; }

        public readonly List<Inline> InlinesToClose;

        public readonly Document Document;

        public StringBuilderCache StringBuilders { get;  }
    }
}